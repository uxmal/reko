#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Gui
{
    public interface ISearchService
    {
    }

    public class SearchService : IObserver<ICodeLocation>
    {
        private IDisposable? subscription;

        public void StartSearch(IObservable<ICodeLocation> newSearcher, ISearchResult result)
        {
            if (subscription is not null)
            {
                subscription.Dispose();
            }
            subscription = newSearcher.Subscribe(this);
        }

        void IObserver<ICodeLocation>.OnCompleted()
        {
            subscription?.Dispose();
            subscription = null;
        }

        void IObserver<ICodeLocation>.OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        void IObserver<ICodeLocation>.OnNext(ICodeLocation value)
        {
            throw new NotImplementedException();
        }
    }
}
