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
using System;
using System.Collections.Generic;

namespace Reko.Scripts.Python
{
    using EventHandlersMap = Dictionary<ScriptEvent, List<Action<object>>>;

    /// <summary>
    /// Event handlers for processing Reko events in user-defined scripts.
    /// </summary>
    public class RekoEventsAPI
    {
        private readonly Dictionary<string, ScriptEvent> events;
        private readonly EventHandlersMap handlers;

        public RekoEventsAPI()
        {
            events = new Dictionary<string, ScriptEvent>
            {
                {"program_loaded", ScriptEvent.OnProgramLoaded},
            };
            handlers = new EventHandlersMap();
        }

        public ScriptEvent? GetEventByName(string name)
        {
            if (!events.TryGetValue(name, out var eventType))
            {
                return null;
            }
            return eventType;
        }

        public void AddEventHandler(ScriptEvent @event, Action<object> handler)
        {
            if (!handlers.TryGetValue(@event, out var eventHandlers))
            {
                eventHandlers = new List<Action<object>>();
                handlers[@event] = eventHandlers;
            }
            eventHandlers.Add(handler);
        }

        public void FireEvent(ScriptEvent @event, object programWrapper)
        {
            if (!handlers.TryGetValue(@event, out var eventHandlers))
                return;
            foreach (var eventHandler in eventHandlers)
            {
                eventHandler(programWrapper);
            }
        }
    }
}
