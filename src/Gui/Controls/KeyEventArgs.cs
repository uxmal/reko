#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Gui.Controls
{
    public class KeyEventArgs : EventArgs
    {
        //   keyData:
        //     A Keys representing the key that was pressed, combined with
        //     any modifier flags that indicate which CTRL, SHIFT, and ALT keys were pressed
        //     at the same time. Possible values are obtained be applying the bitwise OR (|)
        //     operator to constants from the Keys enumeration.
        public KeyEventArgs(Keys keyData)
        {
            this.KeyData = keyData;
        }

        public Keys KeyData { get; private set; }
        public bool Handled { get; set; }
        public bool SuppressKeyPress { get; set; }
    }

    [Flags]
    public enum Keys
    {
        //     The bitmask to extract modifiers from a key value.
        Modifiers = -65536,
        //     No key pressed.
        None = 0,
        //     The left mouse button.
        LButton = 1,
        //     The right mouse button.
        RButton = 2,
        //     The CANCEL key.
        Cancel = 3,
        //     The middle mouse button (three-button mouse).
        MButton = 4,
        //     The first x mouse button (five-button mouse).
        XButton1 = 5,
        //     The second x mouse button (five-button mouse).
        XButton2 = 6,
        //     The BACKSPACE key.
        Back = 8,
        //     The TAB key.
        Tab = 9,
        //     The LINEFEED key.
        LineFeed = 10,
        //     The CLEAR key.
        Clear = 12,
        //     The RETURN key.
        Return = 13,
        //     The ENTER key.
        Enter = 13,
        //     The SHIFT key.
        ShiftKey = 16,
        //     The CTRL key.
        ControlKey = 17,
        //     The ALT key.
        Menu = 18,
        //     The PAUSE key.
        Pause = 19,
        //     The CAPS LOCK key.
        Capital = 20,
        //     The CAPS LOCK key.
        CapsLock = 20,
        //     The IME Kana mode key.
        KanaMode = 21,
        //     The IME Hanguel mode key. (maintained for compatibility; use HangulMode)
        HanguelMode = 21,
        //     The IME Hangul mode key.
        HangulMode = 21,
        //     The IME Junja mode key.
        JunjaMode = 23,
        //     The IME final mode key.
        FinalMode = 24,
        //     The IME Hanja mode key.
        HanjaMode = 25,
        //     The IME Kanji mode key.
        KanjiMode = 25,
        //     The ESC key.
        Escape = 27,
        //     The IME convert key.
        IMEConvert = 28,
        //     The IME nonconvert key.
        IMENonconvert = 29,
        //     The IME accept key, replaces Keys.IMEAceept.
        IMEAccept = 30,
        //     The IME accept key. Obsolete, IMEAccept instead.
        IMEAceept = 30,
        //     The IME mode change key.
        IMEModeChange = 31,
        //     The SPACEBAR key.
        Space = 32,
        //     The PAGE UP key.
        Prior = 33,
        //     The PAGE UP key.
        PageUp = 33,
        //     The PAGE DOWN key.
        Next = 34,
        //     The PAGE DOWN key.
        PageDown = 34,
        //     The END key.
        End = 35,
        //     The HOME key.
        Home = 36,
        //     The LEFT ARROW key.
        Left = 37,
        //     The UP ARROW key.
        Up = 38,
        //     The RIGHT ARROW key.
        Right = 39,
        //     The DOWN ARROW key.
        Down = 40,
        //     The SELECT key.
        Select = 41,
        //     The PRINT key.
        Print = 42,
        //     The EXECUTE key.
        Execute = 43,
        //     The PRINT SCREEN key.
        Snapshot = 44,
        //     The PRINT SCREEN key.
        PrintScreen = 44,
        //     The INS key.
        Insert = 45,
        //     The DEL key.
        Delete = 46,
        //     The HELP key.
        Help = 47,
        //     The 0 key.
        D0 = 48,
        //     The 1 key.
        D1 = 49,
        //     The 2 key.
        D2 = 50,
        //     The 3 key.
        D3 = 51,
        //     The 4 key.
        D4 = 52,
        //     The 5 key.
        D5 = 53,
        //     The 6 key.
        D6 = 54,
        //     The 7 key.
        D7 = 55,
        //     The 8 key.
        D8 = 56,
        //     The 9 key.
        D9 = 57,
        //     The A key.
        A = 65,
        //     The B key.
        B = 66,
        //     The C key.
        C = 67,
        //     The D key.
        D = 68,
        //     The E key.
        E = 69,
        //     The F key.
        F = 70,
        //     The G key.
        G = 71,
        //     The H key.
        H = 72,
        //     The I key.
        I = 73,
        //     The J key.
        J = 74,
        //     The K key.
        K = 75,
        //     The L key.
        L = 76,
        //     The M key.
        M = 77,
        //     The N key.
        N = 78,
        //     The O key.
        O = 79,
        //     The P key.
        P = 80,
        //     The Q key.
        Q = 81,
        //     The R key.
        R = 82,
        //     The S key.
        S = 83,
        //     The T key.
        T = 84,
        //     The U key.
        U = 85,
        //     The V key.
        V = 86,
        //     The W key.
        W = 87,
        //     The X key.
        X = 88,
        //     The Y key.
        Y = 89,
        //     The Z key.
        Z = 90,
        //     The left Windows logo key (Microsoft Natural Keyboard).
        LWin = 91,
        //     The right Windows logo key (Microsoft Natural Keyboard).
        RWin = 92,
        //     The application key (Microsoft Natural Keyboard).
        Apps = 93,
        //     The computer sleep key.
        Sleep = 95,
        //     The 0 key on the numeric keypad.
        NumPad0 = 96,
        //     The 1 key on the numeric keypad.
        NumPad1 = 97,
        //     The 2 key on the numeric keypad.
        NumPad2 = 98,
        //     The 3 key on the numeric keypad.
        NumPad3 = 99,
        //     The 4 key on the numeric keypad.
        NumPad4 = 100,
        //     The 5 key on the numeric keypad.
        NumPad5 = 101,
        //     The 6 key on the numeric keypad.
        NumPad6 = 102,
        //     The 7 key on the numeric keypad.
        NumPad7 = 103,
        //     The 8 key on the numeric keypad.
        NumPad8 = 104,
        //     The 9 key on the numeric keypad.
        NumPad9 = 105,
        //     The multiply key.
        Multiply = 106,
        //     The add key.
        Add = 107,
        //     The separator key.
        Separator = 108,
        //     The subtract key.
        Subtract = 109,
        //     The decimal key.
        Decimal = 110,
        //     The divide key.
        Divide = 111,
        //     The F1 key.
        F1 = 112,
        //     The F2 key.
        F2 = 113,
        //     The F3 key.
        F3 = 114,
        //     The F4 key.
        F4 = 115,
        //     The F5 key.
        F5 = 116,
        //     The F6 key.
        F6 = 117,
        //     The F7 key.
        F7 = 118,
        //     The F8 key.
        F8 = 119,
        //     The F9 key.
        F9 = 120,
        //     The F10 key.
        F10 = 121,
        //     The F11 key.
        F11 = 122,
        //     The F12 key.
        F12 = 123,
        //     The F13 key.
        F13 = 124,
        //     The F14 key.
        F14 = 125,
        //     The F15 key.
        F15 = 126,
        //     The F16 key.
        F16 = 127,
        //     The F17 key.
        F17 = 128,
        //     The F18 key.
        F18 = 129,
        //     The F19 key.
        F19 = 130,
        //     The F20 key.
        F20 = 131,
        //     The F21 key.
        F21 = 132,
        //     The F22 key.
        F22 = 133,
        //     The F23 key.
        F23 = 134,
        //     The F24 key.
        F24 = 135,
        //     The NUM LOCK key.
        NumLock = 144,
        //     The SCROLL LOCK key.
        Scroll = 145,
        //     The left SHIFT key.
        LShiftKey = 160,
        //     The right SHIFT key.
        RShiftKey = 161,
        //     The left CTRL key.
        LControlKey = 162,
        //     The right CTRL key.
        RControlKey = 163,
        //     The left ALT key.
        LMenu = 164,
        //     The right ALT key.
        RMenu = 165,
        //     The browser back key (Windows 2000 or later).
        BrowserBack = 166,
        //     The browser forward key (Windows 2000 or later).
        BrowserForward = 167,
        //     The browser refresh key (Windows 2000 or later).
        BrowserRefresh = 168,
        //     The browser stop key (Windows 2000 or later).
        BrowserStop = 169,
        //     The browser search key (Windows 2000 or later).
        BrowserSearch = 170,
        //     The browser favorites key (Windows 2000 or later).
        BrowserFavorites = 171,
        //     The browser home key (Windows 2000 or later).
        BrowserHome = 172,
        //     The volume mute key (Windows 2000 or later).
        VolumeMute = 173,
        //     The volume down key (Windows 2000 or later).
        VolumeDown = 174,
        //     The volume up key (Windows 2000 or later).
        VolumeUp = 175,
        //     The media next track key (Windows 2000 or later).
        MediaNextTrack = 176,
        //     The media previous track key (Windows 2000 or later).
        MediaPreviousTrack = 177,
        //     The media Stop key (Windows 2000 or later).
        MediaStop = 178,
        //     The media play pause key (Windows 2000 or later).
        MediaPlayPause = 179,
        //     The launch mail key (Windows 2000 or later).
        LaunchMail = 180,
        //     The select media key (Windows 2000 or later).
        SelectMedia = 181,
        //     The start application one key (Windows 2000 or later).
        LaunchApplication1 = 182,
        //     The start application two key (Windows 2000 or later).
        LaunchApplication2 = 183,
        //     The OEM Semicolon key on a US standard keyboard (Windows 2000 or later).
        OemSemicolon = 186,
        //     The OEM 1 key.
        Oem1 = 186,
        //     The OEM plus key on any country/region keyboard (Windows 2000 or later).
        Oemplus = 187,
        //     The OEM comma key on any country/region keyboard (Windows 2000 or later).
        Oemcomma = 188,
        //     The OEM minus key on any country/region keyboard (Windows 2000 or later).
        OemMinus = 189,
        //     The OEM period key on any country/region keyboard (Windows 2000 or later).
        OemPeriod = 190,
        //     The OEM question mark key on a US standard keyboard (Windows 2000 or later).
        OemQuestion = 191,
        //     The OEM 2 key.
        Oem2 = 191,
        //     The OEM tilde key on a US standard keyboard (Windows 2000 or later).
        Oemtilde = 192,
        //     The OEM 3 key.
        Oem3 = 192,
        //     The OEM open bracket key on a US standard keyboard (Windows 2000 or later).
        OemOpenBrackets = 219,
        //     The OEM 4 key.
        Oem4 = 219,
        //     The OEM pipe key on a US standard keyboard (Windows 2000 or later).
        OemPipe = 220,
        //     The OEM 5 key.
        Oem5 = 220,
        //     The OEM close bracket key on a US standard keyboard (Windows 2000 or later).
        OemCloseBrackets = 221,
        //     The OEM 6 key.
        Oem6 = 221,
        //     The OEM singled/double quote key on a US standard keyboard (Windows 2000 or later).
        OemQuotes = 222,
        //     The OEM 7 key.
        Oem7 = 222,
        //     The OEM 8 key.
        Oem8 = 223,
        //     The OEM angle bracket or backslash key on the RT 102 key keyboard (Windows 2000
        //     or later).
        OemBackslash = 226,
        //     The OEM 102 key.
        Oem102 = 226,
        // Summary:
        //     The PROCESS KEY key.
        ProcessKey = 229,
        //
        // Summary:
        //     Used to pass Unicode characters as if they were keystrokes. The Packet key value
        //     is the low word of a 32-bit virtual-key value used for non-keyboard input methods.
        Packet = 231,
        //     The ATTN key.
        Attn = 246,
        //     The CRSEL key.
        Crsel = 247,
        //     The EXSEL key.
        Exsel = 248,
        //     The ERASE EOF key.
        EraseEof = 249,
        //     The PLAY key.
        Play = 250,
        //     The ZOOM key.
        Zoom = 251,
        //     A constant reserved for future use.
        NoName = 252,
        //     The PA1 key.
        Pa1 = 253,
        //     The CLEAR key.
        OemClear = 254,
        //     The bitmask to extract a key code from a key value.
        KeyCode = 0xFFFF,
        //     The SHIFT modifier key.
        Shift = 0x10000,
        //     The CTRL modifier key.
        Control = 0x20000,
        //     The ALT modifier key.
        Alt = 0x40000
    }
}
