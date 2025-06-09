using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Environments.Trs80.Dmk
{
    internal class Track
    {
        private List<Sector> m_sectors = new List<Sector>();

        private int m_dmkOffset = -1;
        private byte[] m_header = new byte[128];
        private byte[] m_data;
        private bool m_twoByteSingleDensity = true;

        public long Length
        {
            get
            {
                if (this.isEmpty())
                {
                    return 0L;
                }
                return this.m_data.LongLength;
            }
        }

        public int SectorCount
        {
            get
            {
                return this.m_sectors.Count;
            }
        }

        public bool twoByteSingleDensity
        {
            get
            {
                return this.m_twoByteSingleDensity;
            }
            set
            {
                this.m_twoByteSingleDensity = value;
            }
        }

        public int TrackNumber { get; set; }

        public int Side { get; set; }

        public IEnumerable<Sector> Sectors { get { return m_sectors; } }

        public static ushort calc_crc(ushort crc, byte bt)
        {
            uint num = Convert.ToUInt32((int)bt << 8);
            uint num2 = Convert.ToUInt32(crc);
            for (int i = 0; i < 8; i++)
            {
                bool flag = ((num2 ^ num) & 32768u) == 32768u;
                num2 = Convert.ToUInt32((long)((ulong)((ulong)num2 << 1) ^ (ulong)(flag ? 4129L : 0L)));
                num2 &= 65535u;
                num <<= 1;
            }
            return Convert.ToUInt16(num2);
        }

        public Track()
        {
            m_data = null!;
        }

        public Track(int number, int side) : this()
        {
            this.TrackNumber = number;
            this.Side = side;
        }

        public byte ReadByte(long index)
        {
            if (this.isEmpty())
            {
                return 0;
            }
            if (index >= this.m_data.LongLength)
            {
                return 0;
            }
            return this.m_data[(int)(checked((IntPtr)index))];
        }



        public byte dataMarker(int index)
        {
            if (index < 0)
            {
                return 0;
            }
            if (index >= this.m_sectors.Count)
            {
                return 0;
            }
            return this.m_sectors[index].Marker;
        }

        public byte crc1(int index)
        {
            if (index < 0)
            {
                return 0;
            }
            if (index >= this.m_sectors.Count)
            {
                return 0;
            }
            return this.m_sectors[index].Crc1;
        }

        public byte crc2(int index)
        {
            if (index < 0)
            {
                return 0;
            }
            if (index >= this.m_sectors.Count)
            {
                return 0;
            }
            return this.m_sectors[index].Crc2;
        }

        public byte datacrc1(int index)
        {
            if (index < 0)
            {
                return 0;
            }
            if (index >= this.m_sectors.Count)
            {
                return 0;
            }
            return this.m_sectors[index].DataCrc1;
        }

        public byte datacrc2(int index)
        {
            if (index < 0)
            {
                return 0;
            }
            if (index >= this.m_sectors.Count)
            {
                return 0;
            }
            return this.m_sectors[index].DataCrc2;
        }

        public bool goodHeaderCRC(int index)
        {
            return index >= 0 && index < this.m_sectors.Count && this.m_sectors[index].goodHeaderCRC;
        }

        public bool goodDataCRC(int index)
        {
            return index >= 0 && index < this.m_sectors.Count && this.m_sectors[index].goodDataCRC;
        }

        public int findSectorAndTrack(int track, int sector, bool doubleDensity)
        {
            for (int i = 0; i < this.m_sectors.Count; i++)
            {
                if (this.m_sectors[i].Track == track && this.m_sectors[i].sector == sector && this.m_sectors[i].doubleDensity == doubleDensity)
                {
                    return i;
                }
            }
            return -1;
        }

        public int reportedSector(int index)
        {
            if (index < 0)
            {
                return -1;
            }
            if (index >= this.m_sectors.Count)
            {
                return -1;
            }
            return this.m_sectors[index].sector;
        }

        public int reportedTrack(int index)
        {
            if (index < 0)
            {
                return -1;
            }
            if (index >= this.m_sectors.Count)
            {
                return -1;
            }
            return this.m_sectors[index].Track;
        }

        public bool doubleDensity(int index)
        {
            return index >= 0 && index < this.m_sectors.Count && this.m_sectors[index].doubleDensity;
        }

        public Sector? sectorData(int index)
        {
            if (index < 0)
            {
                return null;
            }
            if (index >= this.m_sectors.Count)
            {
                return null;
            }
            return this.m_sectors[index];
        }

        private bool isEmpty()
        {
            return this.m_data is null || this.m_data.Length == 0;
        }

        public void setHeader(byte[] data, int startPosition)
        {
            Buffer.BlockCopy(data, startPosition, this.m_header, 0, 128);
        }

        public void setData(byte[] data, int startPosition, int length)
        {
            this.m_dmkOffset = startPosition;
            this.m_data = new byte[length];
            Buffer.BlockCopy(data, startPosition, this.m_data, 0, length);
        }

        public void ParseSectors()
		{
			this.m_sectors.Clear();
			if (this.m_data is null)
			{
				return;
			}
			int i = 0;
			int num = this.m_header.Length;
			int num2 = 0;
			while (i < num)
			{
				int num3 = Convert.ToInt32(this.m_header[i]) + (Convert.ToInt32(this.m_header[i + 1]) << 8);
				i += 2;
				if (num3 == 0)
				{
					return;
				}
				bool flag = (num3 & 32768) == 32768;
				int num4 = (num3 & 16383) - 128;
				if (num4 >= 0 && num4 >= num2)
				{
					num2 = num4;
					if (num4 >= this.m_data.Length)
					{
						return;
					}
					Sector sector = new Sector();
					sector.doubleDensity = flag;
					if (!flag && this.m_twoByteSingleDensity)
					{
						sector.DoubleBytes = true;
					}
					byte b = this.m_data[num4];
					bool flag2 = b == 254;
					ushort num5 = 0;
					if (flag2)
					{
						num5 = Track.calc_crc(Convert.ToUInt16(flag ? 52660 : 65535), b);
					}
					num4++;
					if (!flag && this.m_twoByteSingleDensity)
					{
						num4++;
					}
					if (num4 >= this.m_data.Length)
					{
						this.m_sectors.Add(sector);
					}
					else
					{
						int track = Convert.ToInt32(this.m_data[num4]);
						if (flag2)
						{
							num5 = Track.calc_crc(num5, this.m_data[num4]);
						}
						num4++;
						if (!flag && this.m_twoByteSingleDensity)
						{
							num4++;
						}
						sector.Track = track;
						if (num4 >= this.m_data.Length)
						{
							this.m_sectors.Add(sector);
						}
						else
						{
							int side = Convert.ToInt32(this.m_data[num4]);
							if (flag2)
							{
								num5 = Track.calc_crc(num5, this.m_data[num4]);
							}
							num4++;
							if (!flag && this.m_twoByteSingleDensity)
							{
								num4++;
							}
							sector.Side = side;
							if (num4 >= this.m_data.Length)
							{
								this.m_sectors.Add(sector);
							}
							else
							{
								int sector2 = Convert.ToInt32(this.m_data[num4]);
								if (flag2)
								{
									num5 = Track.calc_crc(num5, this.m_data[num4]);
								}
								num4++;
								if (!flag && this.m_twoByteSingleDensity)
								{
									num4++;
								}
								sector.sector = sector2;
								if (num4 >= this.m_data.Length)
								{
									this.m_sectors.Add(sector);
								}
								else
								{
									int sizeCode = Convert.ToInt32(this.m_data[num4]);
									if (flag2)
									{
										num5 = Track.calc_crc(num5, this.m_data[num4]);
									}
									num4++;
									if (!flag && this.m_twoByteSingleDensity)
									{
										num4++;
									}
									sector.SizeCode = sizeCode;
									int num6 = 0;
									if (i + 1 < this.m_header.Length)
									{
										num6 = Convert.ToInt32(this.m_header[i]) + (Convert.ToInt32(this.m_header[i + 1]) << 8);
										num6 = (num6 & 16383) - 128;
										if (num6 < 0)
										{
											num6 = 0;
										}
									}
									if (flag2)
									{
										num5 = Track.calc_crc(num5, this.m_data[num4]);
										sector.Crc1 = this.m_data[num4];
									}
									num4++;
									if (!flag && this.m_twoByteSingleDensity)
									{
										num4++;
									}
									if (num4 >= this.m_data.Length)
									{
										this.m_sectors.Add(sector);
									}
									else
									{
										if (flag2)
										{
											num5 = calc_crc(num5, this.m_data[num4]);
											sector.Crc2 = this.m_data[num4];
										}
										num4++;
										if (!flag && this.m_twoByteSingleDensity)
										{
											num4++;
										}
										if (num4 >= this.m_data.Length)
										{
											this.m_sectors.Add(sector);
										}
										else
										{
											sector.goodHeaderCRC = (num5 == 0);
											sector.actualSize = this.sectorSize(sizeCode, flag);
											int num7 = num4;
											int actualSize = sector.actualSize;
											int num8 = 0;
											int num9 = 0;
											bool flag3 = false;
											ushort num10 = 0;
											while (num7 < this.m_data.Length && (num6 <= 0 || num7 < num6))
											{
												switch (this.m_data[num7])
												{
												case 248:
												case 249:
												case 250:
												case 251:
												case 253:
													if (!flag3)
													{
														flag3 = true;
														sector.Marker = this.m_data[num7];
														num10 = Track.calc_crc(Convert.ToUInt16(flag ? 52660 : 65535), this.m_data[num7]);
														num7++;
														if (!flag && this.m_twoByteSingleDensity)
														{
															num7++;
														}
														num8 = num7;
														continue;
													}
													break;
												}
												if (flag3)
												{
													num10 = Track.calc_crc(num10, this.m_data[num7]);
													num9++;
													if (num9 == actualSize)
													{
														int num11 = num7 + ((!flag && this.m_twoByteSingleDensity) ? 2 : 1);
														int num12 = num11 + ((!flag && this.m_twoByteSingleDensity) ? 2 : 1);
														if (num12 < this.m_data.Length)
														{
															byte b2 = this.m_data[num11];
															byte b3 = this.m_data[num12];
															num10 = Track.calc_crc(num10, b2);
															num10 = Track.calc_crc(num10, b3);
															sector.DataCrc1 = b2;
															sector.DataCrc2 = b3;
															sector.goodDataCRC = (num10 == 0);
															break;
														}
														break;
													}
												}
												num7++;
												if (!flag && this.m_twoByteSingleDensity)
												{
													num7++;
												}
											}
											if (num8 > 0)
											{
												if (num7 > this.m_data.Length && !flag && this.m_twoByteSingleDensity)
												{
													num9--;
												}
												if (!flag && this.m_twoByteSingleDensity)
												{
													num9 *= 2;
												}
												sector.setData(this.m_data, num8, num9);
												sector.actualSize = num9;
											}
											this.m_sectors.Add(sector);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private int sectorSize(int sizeCode, bool doubleDensity)
		{
			int num = 3;
			if (doubleDensity)
			{
				return 0x80 << sizeCode % (num + 1);
			}
			if (sizeCode <= num)
			{
				return 0x80 << sizeCode;
			}
			return 16 * ((sizeCode != 0) ? sizeCode : 256);
		}
	}
}
