using System;

namespace Reko.Environments.Trs80 // DMKViewer
{
	internal class CompareItem
	{
		private int m_start;

		private int m_length;

		public int start
		{
			get
			{
				return this.m_start;
			}
			set
			{
				this.m_start = value;
			}
		}

		public int length
		{
			get
			{
				return this.m_length;
			}
			set
			{
				this.m_length = value;
			}
		}

		public void clear()
		{
			this.m_start = (this.m_length = 0);
		}
	}
}
