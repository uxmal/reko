#region License
/* 
 * Copyright (C) 1999-2025 Pavel Tomin.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core;
using Reko.Core.Loading;
using Reko.Core.Scripts;
using Reko.Core.Services;
using Reko.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Timers;

namespace Reko.Gui
{
    using FileWatchersMap = ConcurrentDictionary<ImageLocation, FileSystemWatcher>;

    /// <summary>
    /// Watch project files, reload them if they were changed.
    /// </summary>
    public class ProjectFilesWatcher
    {
        public const int MsecReloadDelay = 200;

        private readonly IDecompilerService decompilerSvc;
        private readonly IEventListener eventListener;
        private readonly ILoader loader;
        private readonly FileWatchersMap scriptWatchers;
        private readonly ConcurrentDictionary<string, bool> changedFiles;
        private IDecompiler? decompiler;
        private Project? project;

        public ProjectFilesWatcher(IServiceProvider services)
        {
            this.decompilerSvc = services.RequireService<IDecompilerService>();
            this.eventListener = services.RequireService<IEventListener>();
            this.loader = services.RequireService<ILoader>();
            this.Decompiler = decompilerSvc.Decompiler;
            decompilerSvc.DecompilerChanged += OnDecompilerChanged;
            this.scriptWatchers = new FileWatchersMap();
            this.changedFiles = new ConcurrentDictionary<string, bool>();
        }

        public IDecompiler? Decompiler
        {
            get => decompiler;
            set
            {
                if (decompiler is not null)
                {
                    decompiler.ProjectChanged -= OnProjectChanged;
                }
                decompiler = value;
                if (value is not null)
                {
                    value.ProjectChanged += OnProjectChanged;
                }
                this.Project = Decompiler?.Project;
            }
        }

        public Project? Project
        {
            get => project;
            set
            {
                if (project is not null)
                {
                    RemoveScripts(project.ScriptFiles);
                    project.ScriptFiles.CollectionChanged -= OnScriptsChanged;
                }
                project = value;
                if (value is not null)
                {
                    AddScripts(value.ScriptFiles);
                    value.ScriptFiles.CollectionChanged += OnScriptsChanged;
                }
            }
        }

        private void AddScripts(IEnumerable<ScriptFile> newScripts)
        {
            foreach (ScriptFile script in newScripts)
            {
                if (script.Location.HasFragments)
                    continue;   // Can't watch inside archives...
                var fullPath = script.Location.FilesystemPath;
                var directoryName = Path.GetDirectoryName(fullPath)!;
                var fileName = Path.GetFileName(fullPath);
                
                var watcher = new FileSystemWatcher(directoryName, fileName);
                if (scriptWatchers.TryAdd(script.Location, watcher))
                {
                    watcher.NotifyFilter = NotifyFilters.LastWrite;
                    watcher.Changed += OnScriptFileChanged;
                    watcher.EnableRaisingEvents = true;
                }
                else
                {
                    watcher.Dispose();
                }
            }
        }

        private void RemoveScripts(IEnumerable<ScriptFile> oldScripts)
        {
            foreach (ScriptFile script in oldScripts)
            {
                if (scriptWatchers.TryRemove(script.Location, out var watcher))
                {
                    watcher.Changed -= OnScriptFileChanged;
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
            }
        }

        private void ReloadScript(ScriptFile scriptFile)
        {
            var scriptUri = scriptFile.Location;
            try
            {
                var bytes = loader.LoadFileBytes(scriptUri.FilesystemPath);
                var stream = new MemoryStream(bytes);
                using var rdr = new StreamReader(stream);
                scriptFile.Evaluate(rdr.ReadToEnd());
            }
            catch (Exception ex)
            {
                eventListener.Error(ex, $"Failed to load script {scriptUri}.");
            }
        }

        private void OnScriptFileChanged(object sender, FileSystemEventArgs e)
        {
            var fullPath = e.FullPath;
            if (!changedFiles.TryAdd(fullPath, true))
                return;
            // Multiple Changed event can be raised in some cases. And file
            // access error can be occurred when processing first event. Use
            // timer to delay processing event.
            var timer = new Timer(MsecReloadDelay)
            {
                AutoReset = false,
            };
            timer.Elapsed += (sender, e) =>
            {
                timer.Dispose();
                if (!changedFiles.TryRemove(fullPath, out _))
                    return;
                OnScriptFileChanged(Project, fullPath);
            };
            timer.Start();
        }

        private void OnScriptFileChanged(Project? project, string fileName)
        {
            if (project is null)
                return;
            var fullPath = Path.GetFullPath(fileName);
            var scriptFile = project.ScriptFiles.
                Where(s => Path.GetFullPath(s.Location.FilesystemPath) == fullPath).
                FirstOrDefault();
            if (scriptFile is not null)
            {
                ReloadScript(scriptFile);
            }
        }

        private void OnScriptsChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems is not null)
                {
                    AddScripts(e.NewItems.OfType<ScriptFile>());
                }
                break;
            default:
                throw new NotImplementedException();
            }
        }

        private void OnProjectChanged(object? sender, EventArgs e)
        {
            this.Project = Decompiler?.Project;
        }

        private void OnDecompilerChanged(object? sender, EventArgs e)
        {
            this.Decompiler = decompilerSvc.Decompiler;
        }
    }
}
