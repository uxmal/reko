#region License
/* 
 * Copyright (C) 1999-2021 Pavel Tomin.
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

using Reko.Core.Scripts;
using Reko.Core.Services;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Timers;

namespace Reko.Core
{
    using FileWatchersMap = ConcurrentDictionary<string, FileSystemWatcher>;

    /// <summary>
    /// Watch project files, reload them if they were changed.
    /// </summary>
    public class ProjectFilesWatcher
    {
        private readonly IDecompilerService decompilerSvc;
        private readonly DecompilerEventListener eventListener;
        private readonly ILoader loader;
        private readonly FileWatchersMap scriptWatchers;
        private readonly ConcurrentDictionary<string, bool> changedFiles;
        private IDecompiler? decompiler;
        private Project? project;

        public ProjectFilesWatcher(IServiceProvider services)
        {
            this.decompilerSvc = services.RequireService<IDecompilerService>();
            this.eventListener = services.RequireService<DecompilerEventListener>();
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
                if (decompiler != null)
                {
                    decompiler.ProjectChanged -= OnProjectChanged;
                }
                decompiler = value;
                if (value != null)
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
                if (project != null)
                {
                    RemoveScripts(project.ScriptFiles);
                    project.ScriptFiles.CollectionChanged -= OnScriptsChanged;
                }
                project = value;
                if (value != null)
                {
                    AddScripts(value.ScriptFiles);
                    value.ScriptFiles.CollectionChanged += OnScriptsChanged;
                }
            }
        }

        private void AddScripts(IEnumerable<ScriptFile> newScripts)
        {
            foreach(ScriptFile script in newScripts)
            {
                var directoryName = Path.GetDirectoryName(script.Filename);
                var fileName = Path.GetFileName(script.Filename);
                var watcher = new FileSystemWatcher(directoryName, fileName);
                if (scriptWatchers.TryAdd(script.Filename, watcher))
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
                if (scriptWatchers.TryRemove(script.Filename, out var watcher))
                {
                    watcher.Changed -= OnScriptFileChanged;
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                }
            }
        }

        private void ReloadScript(ScriptFile scriptFile)
        {
            var fileName = scriptFile.Filename;
            try
            {
                var bytes = loader.LoadImageBytes(fileName, 0);
                var stream = new MemoryStream(bytes);
                using var rdr = new StreamReader(stream);
                scriptFile.Evaluate(rdr.ReadToEnd());
            }
            catch(Exception ex)
            {
                eventListener.Error(ex, $"Failed to load script {fileName}.");
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
            var timer = new Timer(200)
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
                Where(s => Path.GetFullPath(s.Filename) == fullPath).
                FirstOrDefault();
            if (scriptFile != null)
            {
                ReloadScript(scriptFile);
            }
        }

        private void OnScriptsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
            case NotifyCollectionChangedAction.Add:
                AddScripts(e.NewItems.OfType<ScriptFile>());
                break;
            default:
                throw new NotImplementedException();
            }
        }

        private void OnProjectChanged(object sender, EventArgs e)
        {
            this.Project = Decompiler?.Project;
        }

        private void OnDecompilerChanged(object sender, EventArgs e)
        {
            this.Decompiler = decompilerSvc.Decompiler;
        }
    }
}
