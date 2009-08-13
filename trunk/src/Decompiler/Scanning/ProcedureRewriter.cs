/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using System;
using System.Collections.Generic;

namespace Decompiler.Scanning
{
	/// <summary>
	/// Rewrites the code of a scanned procedure into intermediate code.
	/// </summary>
	public class ProcedureRewriter : IProcedureRewriter
	{
		private IRewriterHost host;
		private Procedure proc;
		private Rewriter rewriter; 
		private Dictionary<Address,Block> blocksVisited;

        public ProcedureRewriter(IRewriterHost host, Procedure proc)
        {
            this.host = host;
            this.proc = proc;
            this.blocksVisited = new Dictionary<Address, Block>();
        }

        public CodeEmitter CreateEmitter(Block block)
        {
            return new CodeEmitter(rewriter.Architecture, host, proc, block);
        }

		public void HandleFallThrough(Block block, Address addr)
		{
			Procedure procTarget = host.GetProcedureAtAddress(addr, proc.Frame.ReturnAddressSize);
			if (procTarget != null)
			{
				rewriter.EmitCallAndReturn(procTarget);
			}
			else
			{
				RewriteBlock(addr, block);
			}
		}

		/// <summary>
		/// A block is fallthrough if there are no edges leaving it.
		/// </summary>
		/// <param name="block">Block to be tested for fall-through-ness</param>
		/// <returns>true if the block is fallthrough</returns>
		private bool IsBlockFallThrough(Block block)
		{
			return block.Succ.Count == 0;
		}

        public Block RewriteBlock(Address addr, Block pred)
        {
            Block block;
            if (blocksVisited.TryGetValue(addr, out block))
            {
                Block.AddEdge(pred, block);
                return block;
            }

            // Locate the image map block corresponding to the address.

            ImageMapItem item;
            if (host.Image.Map.TryFindItemExact(addr, out item))
            {
                ImageMapBlock raw = item as ImageMapBlock;
                if (raw != null)
                {

                    // Create a new block in the procedure.

                    block = new Block(proc, addr);
                    blocksVisited.Add(addr, block);
                    Block.AddEdge(pred, block);

                    rewriter.RewriteInstructions(addr, raw.Size, block);

                    if (IsBlockFallThrough(block))
                    {
                        HandleFallThrough(block, raw.Address + raw.Size);
                    }
                    return block;
                }
            }

            block = proc.ExitBlock;
            Block.AddEdge(pred, block);
            return block;
        }

        public Rewriter Rewriter
        {
            get { return rewriter; }
            set { rewriter = value; }
        }
	}
}
