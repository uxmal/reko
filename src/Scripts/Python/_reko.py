# Copyright (C) 1999-2023 Pavel Tomin.
#
# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation; either version 2, or (at your option)
# any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; see the file COPYING.  If not, write to
# the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.

'''This module contains Reko API classes.
WARNING: This is a part of internal API. Do not import it directly to your
scripts.
'''

class RekoEventHandlers:
    __slots__ = '_reko_handlers', '_event'

    def __init__(self, reko_handlers, event):
        self._reko_handlers = reko_handlers
        self._event = event

    def __iadd__(self, handler):
        self._reko_handlers.AddEventHandler(self._event, handler)
        return self

class RekoEvents:
    __slots__ = '_reko_handlers'

    def __init__(self, reko_handlers):
        self._reko_handlers = reko_handlers

    def __getattr__(self, name):
        event = self._reko_handlers.GetEventByName(name)
        if event is None:
            raise AttributeError("Unknown event: '{}'".format(name))
        return RekoEventHandlers(self._reko_handlers, event)

    def __setattr__(self, name, value):
        # Allow '<handlers> += <new-handler>' statement.
        # Do not raise exception in this case. Just ignore.
        if isinstance(value, RekoEventHandlers):
            return
        super().__setattr__(name, value)

class Reko:
    __slots__ = '_events'

    def __init__(self, reko_handlers):
        self._events = RekoEvents(reko_handlers)

    @property
    def on(self):
        return self._events
