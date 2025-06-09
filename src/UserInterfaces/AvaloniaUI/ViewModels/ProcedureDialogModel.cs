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

using ReactiveUI;
using Reko.Core;
using Reko.Core.Hll.C;
using Reko.Core.Serialization;

namespace Reko.UserInterfaces.AvaloniaUI.ViewModels
{
    public class ProcedureDialogModel : ReactiveObject
    {
        private readonly Program program;
        private readonly UserProcedure userProc;

        public ProcedureDialogModel(Program program, UserProcedure userProc)
        {
            this.program = program;
            this.userProc = userProc;
            this.Name = userProc.Name;
            this.Decompile = userProc.Decompile;
            this.Signature = userProc.CSignature;
            this.IsAllocator = userProc.Characteristics.Allocator;
            this.IsAlloca = userProc.Characteristics.IsAlloca;
            this.Terminates = userProc.Characteristics.Terminates;
            this.VarargsFormatParser = userProc.Characteristics.VarargsParserClass;
            OnSignatureChanged();
        }

        public string? Name
        {
            get => name;
            set => this.RaiseAndSetIfChanged(ref name, value, nameof(Name));
        }

        public UserProcedure? GetValue()
        {
            var CSignature = this.Signature?.Trim();
            if (string.IsNullOrEmpty(CSignature))
                CSignature = null;

            var procName = this.Name;
            if (string.IsNullOrEmpty(procName))
            {
                procName = program.NamingPolicy.ProcedureName(userProc.Address);
            }
            var usb = new UserSignatureBuilder(program);
            var procNew = new UserProcedure(userProc.Address, procName)
            {
                CSignature = CSignature,
                Name = procName,
                Signature = usb.ParseFunctionDeclaration(CSignature)?.Signature,
                Decompile = this.Decompile,

                Characteristics = new ProcedureCharacteristics
                {
                    Allocator = this.IsAllocator,
                    Terminates = this.Terminates,
                    IsAlloca = this.IsAlloca,
                    VarargsParserClass = !string.IsNullOrWhiteSpace(this.VarargsFormatParser)
                        ? this.VarargsFormatParser
                        : null,
                }
            };
            return procNew;
        }

        private string? name;

        public string? Comment
        {
            get => comment;
            set => this.RaiseAndSetIfChanged(ref comment, value, nameof(Comment));
        }
        public string? comment;

        public bool Decompile
        {
            get => decompile;
            set => this.RaiseAndSetIfChanged(ref decompile, value, nameof(Decompile));
        }
        private bool decompile;

        public bool IsAllocator
        {
            get => isAllocator;
            set => this.RaiseAndSetIfChanged(ref isAllocator, value, nameof(IsAllocator));
        }
        private bool isAllocator;

        public bool IsAlloca
        {
            get => isAlloca;
            set => this.RaiseAndSetIfChanged(ref isAlloca, value, nameof(IsAlloca));
        }
        private bool isAlloca;

        public string? Signature
        {
            get => signature;
            set {
                this.RaiseAndSetIfChanged(ref signature, value, nameof(Signature));
                OnSignatureChanged();
            }
        }
        private string? signature;

        public bool Terminates
        {
            get => terminates;
            set => this.RaiseAndSetIfChanged(ref terminates, value, nameof(Terminates));
        }
        private bool terminates;

        public string? VarargsFormatParser
        {
            get => varargsFormatParser;
            set => this.RaiseAndSetIfChanged(ref varargsFormatParser, value);
        }
        private string? varargsFormatParser;

        private void OnSignatureChanged()
        {
            // Attempt to parse the signature.
            if (this.Signature is null)
            {
                EnableControls(true);
                return;
            }
            var CSignature = this.Signature.Trim();
            ProcedureBase_v1? sProc = null;
            bool isValid;
            if (!string.IsNullOrEmpty(CSignature))
            {
                var usb = new UserSignatureBuilder(program);
                sProc = usb.ParseFunctionDeclaration(CSignature);
                isValid = (sProc is not null);
            }
            else
            {
                CSignature = null;
                isValid = true;
            }
            if (isValid)
            {
                if (sProc is not null)
                    this.Name = sProc.Name;
            }
            EnableControls(isValid);
        }

        public bool NameEnabled
        {
            get => nameEnabled;
            set => this.RaiseAndSetIfChanged(ref nameEnabled, value, nameof(NameEnabled));
        }
        private bool nameEnabled;

        public bool OkEnabled
        {
            get => okEnabled;
            set => this.RaiseAndSetIfChanged(ref okEnabled, value, nameof(OkEnabled));
        }
        private bool okEnabled;

        public bool SignatureError
        {
            get => signatureError;
            set => this.RaiseAndSetIfChanged(ref signatureError, value, nameof(SignatureError));
        }
        private bool signatureError;

        private void EnableControls(bool signatureIsValid)
        {
            this.OkEnabled = signatureIsValid;
            this.NameEnabled = string.IsNullOrEmpty(this.Signature);
            this.SignatureError = !signatureIsValid;
        }
    }
}
