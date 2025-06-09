using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.Trs80.Dmk
{
	internal class Sector
	{
		private int SectorNumber = -1;
		private int m_actualSize;
		private bool m_doubleDensity;
		private byte[] m_data;
		private bool m_goodHeaderCRC;
		private bool m_goodDataCRC;

        public Sector()
        {
            this.TrackOffset = -1;
            this.SectorNumber = -1;
            this.Track = -1;
            this.Side = -1;
            this.SizeCode = -1;
            m_data = null!;
        }

		public long Length
		{
			get
			{
				if (this.isEmpty())
				{
					return 0L;
				}
                if (this.DoubleBytes)
                    return m_data.Length / 2;
                else
                    return m_data.Length;
			}
		}

		public int TrackOffset { get; private set; }

		public byte Marker { get; set; }

        public byte Crc1 { get; set; }
        public byte Crc2 { get; set; }
		public byte DataCrc1 { get; set; }
		public byte DataCrc2 { get; set; }

		public bool DoubleBytes { get; set; }

		public int Track { get; set; }

		public int Side { get; set; }

		public int SizeCode { get; set; }

        public int actualSize
		{
			get
			{
				return this.m_actualSize;
			}
			set
			{
				this.m_actualSize = value;
			}
		}

		public int sector
		{
			get
			{
				return this.SectorNumber;
			}
			set
			{
				this.SectorNumber = value;
			}
		}

		public bool doubleDensity
		{
			get
			{
				return this.m_doubleDensity;
			}
			set
			{
				this.m_doubleDensity = value;
			}
		}

		public bool goodHeaderCRC
		{
			get
			{
				return this.m_goodHeaderCRC;
			}
			set
			{
				this.m_goodHeaderCRC = value;
			}
		}

        public byte[]  GetData()
        {
            if (DoubleBytes)
            {
                byte[] data = new byte[(m_data.Length + 1) / 2];
                for (int i = 0; i < data.Length; ++i)
                {
                    data[i] = m_data[i * 2];
                }
                return data;
            }
            else
            {
                return m_data;
            }
        }

        public bool goodDataCRC
		{
			get
			{
				return this.m_goodDataCRC;
			}
			set
			{
				this.m_goodDataCRC = value;
			}
		}

		public byte ReadByte(long index)
		{
			if (this.isEmpty())
			{
				return 0;
			}
			if (this.DoubleBytes)
			{
				index *= 2L;
			}
			if (index >= this.m_data.LongLength)
			{
				return 0;
			}
			return this.m_data[index];
		}

		public void UpdateCRC()
		{
			ushort num = Dmk.Track.calc_crc(Convert.ToUInt16(this.doubleDensity ? 52660 : 65535), this.Marker);
			for (int i = 0; i < this.actualSize; i++)
			{
				if (this.DoubleBytes)
				{
					i++;
				}
				num = Dmk.Track.calc_crc(num, this.m_data[i]);
			}
			this.DataCrc1 = (byte)(num >> 8);
			this.DataCrc2 = (byte)(num & 255);
			this.m_goodDataCRC = true;
		}


		public void setData(byte[] data, int startPosition, int length)
		{
			this.TrackOffset = startPosition;
			this.m_data = new byte[length];
			Buffer.BlockCopy(data, startPosition, this.m_data, 0, length);
			this.m_actualSize = length;
		}

		public bool isEmpty()
		{
			return this.m_data is null || this.m_data.Length == 0;
		}
	}
}
