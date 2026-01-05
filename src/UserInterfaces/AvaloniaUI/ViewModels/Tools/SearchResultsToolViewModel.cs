#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Dock.Model.ReactiveUI.Controls;
using Dock.Model.ReactiveUI.Core;
using ReactiveUI;
using Reko.Core.Services;
using Reko.Gui;
using Reko.Gui.Services;
using Reko.Gui.ViewModels.Tools;
using System;
using System.Collections.ObjectModel;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels.Tools
{
    public class SearchResultsToolViewModel : Tool
    {
        private readonly IServiceProvider services;
        private ISearchResult? searchResult;

        public SearchResultsToolViewModel(IServiceProvider services)
        {
            this.services = services;
            this.SearchResults = new();
        }

        public FindResultsViewModel ViewModel { get; set; } = null!;

        public string Friend { get; } = "Friend";

        public ObservableCollection<SearchResultItem> SearchResults { get; }


        public ISearchResultView? SearchResultsView
        {
            get => searchResultsView;
            set
            {
                if (this.searchResultsView == value)
                    return;
                this.RaiseAndSetIfChanged(ref this.searchResultsView, value);
                ReloadColumns();
            }
        }
        private ISearchResultView? searchResultsView;


        private void ReloadColumns()
        {
            if (this.searchResult is null)
                return;
            SearchResults.Clear();
            this.searchResult.CreateColumns();
            for (int i = 0; i < this.searchResult.Count; ++i)
            {
                var item = this.searchResult.GetItem(i);
                SearchResults.Add(item);
            }
        }

        public void Show(ISearchResult result)
        {
            this.searchResult = result;

            if (Owner is DockBase dock)
            {
                dock.ActiveDockable = this;
            }
            if (SearchResultsView is null)
                return;
            this.searchResult.View = SearchResultsView;
            ReloadColumns();
        }

        public void OnGotFocus()
        {
            var srSvc = this.services.GetService<ISearchResultService>();
            if (srSvc is { })
            {
                this.services.RequireService<ICommandRouterService>().ActiveCommandTarget = (ICommandTarget) srSvc;
            }
        }
    }
}
