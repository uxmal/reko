using DMKViewer.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DMKViewer
{
	public class Form1 : Form
	{
		internal enum ViewMode
		{
			MODE_TRACK,
			MODE_SECTOR
		}


		private void changeMode(Form1.ViewMode newMode)
		{
			if (newMode == this.m_mode)
			{
				return;
			}
			this.m_mode = newMode;
			this.inTrackEvent = true;
			this.clearComparison(true);
			this.m_trackUpDown1.Value = 0m;
			this.m_sectorUpDown1.Value = 0m;
			this.m_actualTrack.Text = string.Empty;
			if (newMode == Form1.ViewMode.MODE_TRACK)
			{
				this.nextDifferenceToolStripMenuItem.Enabled = false;
				this.previousDifferenceToolStripMenuItem.Enabled = false;
				this.m_actualSector.Visible = false;
				this.m_sectorUpDown1.Visible = false;
				this.m_PhysicalLabel.Visible = false;
				this.allowEditingToolStripMenuItem.Enabled = false;
				this.m_editMessage.Visible = false;
				this.m_hexBox.ReadOnly = true;
				this.updateTrackNumber(0);
			}
			else
			{
				this.m_sectorUpDown1.Visible = true;
				this.m_PhysicalLabel.Visible = true;
				this.m_actualSector.Visible = true;
				this.updateSectorNumber(0, 0);
				this.allowEditingToolStripMenuItem.Enabled = this.allowEditingToolStripMenuItem.Checked;
				this.m_hexBox.ReadOnly = !this.allowEditingToolStripMenuItem.Checked;
				this.m_editMessage.Visible = this.m_hexBox.ReadOnly;
				this.nextDifferenceToolStripMenuItem.Enabled = (this.m_compareTracks.Count<Track>() > 0);
				this.previousDifferenceToolStripMenuItem.Enabled = (this.m_compareTracks.Count<Track>() > 0);
			}
			this.inTrackEvent = false;
		}

		private void buildTrackList(ref byte[] rawDMK, ref List<Track> listToProcess, int trackLength)
		{
			listToProcess.Clear();
			if (trackLength == 0)
			{
				return;
			}
			int i = 16;
			int num = rawDMK.Length;
			while (i < num)
			{
				if (num - i <= 128)
				{
					return;
				}
				Track track = new Track();
				if (this.m_singleDensityOnly || this.m_mixedDensity)
				{
					track.twoByteSingleDensity = false;
				}
				track.setHeader(rawDMK, i);
				i += 128;
				int num2 = num - i;
				if (num2 > trackLength - 128)
				{
					num2 = trackLength - 128;
				}
				track.setData(rawDMK, i, num2);
				track.parseSectors();
				i += num2;
				listToProcess.Add(track);
			}
		}

		private bool parseDMKHeader2()
		{
			if (this.m_rawDMK2 == null)
			{
				this.m_compareInfo.Text = "Empty file!";
				return false;
			}
			this.m_track2Length = 0;
			string text = this.m_header2Info.Text;
			if (this.m_rawDMK2.Length < 16)
			{
				this.m_compareInfo.Text = "Invalid file!";
				return false;
			}
			this.m_track2Length = Convert.ToInt32(this.m_rawDMK2[2]) + (Convert.ToInt32(this.m_rawDMK2[3]) << 8);
			text = text + " Track Length:0x" + this.m_track2Length.ToString("X");
			text += " ";
			if ((this.m_rawDMK2[4] & 16) == 16)
			{
				text += "Single sided only ";
			}
			if ((this.m_rawDMK2[4] & 64) == 64)
			{
				text += "Single Density only ";
			}
			if ((this.m_rawDMK2[4] & 128) == 128)
			{
				text += " Mixed Density (older format)";
			}
			this.m_header2Info.Text = text;
			return true;
		}

		private bool parseDMKHeader1(string filename)
		{
			if (this.m_rawDMK1 == null)
			{
				this.m_header1Info.Text = "Empty file!";
				return false;
			}
			this.m_track1Length = 0;
			if (this.m_rawDMK1.Length < 16)
			{
				return false;
			}
			byte b = this.m_rawDMK1[0];
			string text;
			if (b != 0)
			{
				if (b != 255)
				{
					text = filename + " Unknown read/write flag:" + this.m_rawDMK1[0].ToString("X");
				}
				else
				{
					text = filename + " Read Only";
				}
			}
			else
			{
				text = filename + " Read/Write";
			}
			text += " Track Length:";
			this.m_track1Length = Convert.ToInt32(this.m_rawDMK1[2]) + (Convert.ToInt32(this.m_rawDMK1[3]) << 8);
			if (this.m_track1Length > 10560 || this.m_track1Length < 3264)
			{
				text = " Invalid track length:" + this.m_track1Length.ToString();
				this.m_header1Info.Text = text;
				return false;
			}
			int arg_105_0 = (this.m_rawDMK1.Length - 16) / this.m_track1Length;
			text = text + "0x" + this.m_track1Length.ToString("X");
			text += " ";
			if ((this.m_rawDMK1[4] & 16) == 16)
			{
				text += "Single sided only ";
			}
			if ((this.m_rawDMK1[4] & 64) == 64)
			{
				text += "Single Density only ";
				this.m_singleDensityOnly = true;
			}
			if ((this.m_rawDMK1[4] & 128) == 128)
			{
				text += " Mixed Density (older format)";
				this.m_mixedDensity = true;
			}
			this.m_header1Info.Text = text;
			return true;
		}



		private void open_Click(object sender, EventArgs e)
		{
			string text = Settings.Default.InputPath;
			if (string.IsNullOrEmpty(text))
			{
				text = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			}
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Select DMK File";
			openFileDialog.CheckFileExists = true;
			openFileDialog.CheckPathExists = true;
			openFileDialog.InitialDirectory = text;
			openFileDialog.Filter = "DMK files (*.dmk)|*.dmk|All files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;
			openFileDialog.RestoreDirectory = true;
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string fileName = openFileDialog.FileName;
			string fileName2 = Path.GetFileName(fileName);
			this.m_currentFile = string.Empty;
			this.compareToolStripMenuItem.Enabled = false;
			this.allowEditingToolStripMenuItem.Enabled = false;
			this.m_hexBox.ByteProvider = null;
			this.m_hexBox2.ByteProvider = null;
			this.m_tracks.Clear();
			this.m_currentTrack = -1;
			this.m_compareTracks.Clear();
			this.nextDifferenceToolStripMenuItem.Enabled = false;
			this.previousDifferenceToolStripMenuItem.Enabled = false;
			this.m_compareInfo.Text = string.Empty;
			this.m_header2Info.Text = string.Empty;
			this.disableUpDowns();
			Settings.Default.InputPath = Path.GetDirectoryName(fileName);
			Settings.Default.Save();
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.m_rawDMK1 = binaryReader.ReadBytes((int)fileStream.Length);
				}
			}
			if (this.parseDMKHeader1(fileName2))
			{
				this.buildTrackList(ref this.m_rawDMK1, ref this.m_tracks, this.m_track1Length);
				this.m_currentFile = fileName;
				this.initUpDowns();
				if (this.m_tracks.Count > 0)
				{
					this.compareToolStripMenuItem.Enabled = true;
					if (this.m_mode == Form1.ViewMode.MODE_SECTOR)
					{
						this.allowEditingToolStripMenuItem.Enabled = true;
						this.m_hexBox.ReadOnly = true;
						this.allowEditingToolStripMenuItem.Checked = false;
						this.m_editMessage.Visible = true;
					}
				}
			}
		}

		private void showComparison()
		{
			if (!this.m_differenceBar.Enabled)
			{
				return;
			}
			if (this.m_sectorToCompare == -1)
			{
				return;
			}
			int value = this.m_differenceBar.Value;
			int index = Convert.ToInt32(this.m_trackUpDown1.Value);
			int num = this.m_compareTracks[index].differenceStart(this.m_sectorToCompare, value);
			int num2 = this.m_compareTracks[index].differenceLength(this.m_sectorToCompare, value);
			if (num2 == 0)
			{
				this.m_hexBox2.Select(0L, 0L);
				this.m_hexBox.Select(0L, 0L);
				return;
			}
			this.m_hexBox2.Select((long)num, (long)num2);
			this.m_hexBox.Select((long)num, (long)num2);
		}

		private void clearComparison(bool noProvider)
		{
			this.m_differenceBar.Enabled = false;
			this.m_differenceBar.Visible = false;
			this.m_hexBox2.Select(0L, 0L);
			this.m_hexBox.Select(0L, 0L);
			if (noProvider)
			{
				this.m_hexBox2.ByteProvider = null;
			}
			this.m_compareInfo.Text = string.Empty;
			this.m_sectorToCompare = -1;
		}

		private void syncCompareTrack()
		{
			if (this.m_hexBox.ByteProvider == null)
			{
				this.clearComparison(true);
				return;
			}
			int num = Convert.ToInt32(this.m_trackUpDown1.Value);
			if (num >= this.m_compareTracks.Count)
			{
				this.clearComparison(true);
				this.m_compareInfo.Text = "Comparison file has fewer tracks than first file";
				return;
			}
			if (this.m_mode == Form1.ViewMode.MODE_TRACK)
			{
				this.m_hexBox2.ByteProvider = this.m_compareTracks[num];
				return;
			}
			int index = Convert.ToInt32(this.m_sectorUpDown1.Value);
			int num2 = this.m_tracks[num].reportedSector(index);
			if (num2 == -1)
			{
				this.clearComparison(true);
				this.m_compareInfo.Text = "No comparison possible";
				return;
			}
			int num3 = this.m_tracks[num].reportedTrack(index);
			if (num3 == -1)
			{
				this.clearComparison(true);
				this.m_compareInfo.Text = "No comparison possible";
				return;
			}
			bool doubleDensity = this.m_tracks[num].doubleDensity(index);
			int num4 = this.m_compareTracks[num].findSectorAndTrack(num3, num2, doubleDensity);
			if (num4 == -1)
			{
				this.clearComparison(true);
				this.m_compareInfo.Text = "No matching sector found";
				return;
			}
			this.m_hexBox2.ByteProvider = this.m_compareTracks[num].sectorData(num4);
			this.m_hexBox2.Invalidate();
			this.m_sectorToCompare = num4;
			long length = this.m_hexBox.ByteProvider.Length;
			long length2 = this.m_hexBox2.ByteProvider.Length;
			this.m_compareTracks[num].buildComparison(this.m_hexBox.ByteProvider, this.m_sectorToCompare);
			bool flag = this.m_compareTracks[num].hasDifferences(this.m_sectorToCompare);
			this.m_compareInfo.Text = (flag ? "Sectors differ" : "Sectors identical");
			if (length != length2)
			{
				Label expr_1DA = this.m_compareInfo;
				expr_1DA.Text += " Note: Sectors are different size.";
			}
			if (!this.m_compareTracks[num].goodDataCRC(this.m_sectorToCompare))
			{
				Label expr_20E = this.m_compareInfo;
				expr_20E.Text += " (bad Data CRC)";
			}
			this.m_differenceBar.Value = 0;
			this.m_differenceBar.Enabled = flag;
			this.m_differenceBar.Visible = flag;
			byte b = this.m_compareTracks[num].datacrc1(this.m_sectorToCompare);
			byte b2 = this.m_compareTracks[num].datacrc2(this.m_sectorToCompare);
			byte b3 = this.m_compareTracks[num].dataMarker(this.m_sectorToCompare);
			if (flag)
			{
				this.m_differenceBar.Maximum = this.m_compareTracks[num].differenceCount(this.m_sectorToCompare) - 1;
				this.showComparison();
				if (length == length2)
				{
					Label expr_2CE = this.m_compareInfo;
					expr_2CE.Text = expr_2CE.Text + " Data CRC:" + b.ToString("X2") + b2.ToString("X2");
				}
			}
			else
			{
				this.clearComparison(false);
				if (length == length2)
				{
					this.m_compareInfo.Text = "Data CRC:" + b.ToString("X2") + b2.ToString("X2");
				}
				this.m_differenceBar.Maximum = 0;
			}
			if (b3 != 0)
			{
				Label expr_34D = this.m_compareInfo;
				expr_34D.Text = expr_34D.Text + " Data Marker:" + b3.ToString("X2");
			}
		}

		private void updateTrackNumber(int track)
		{
			if (this.m_mode == Form1.ViewMode.MODE_SECTOR)
			{
				return;
			}
			if (this.m_tracks.Count<Track>() == 0)
			{
				this.m_currentTrack = 0;
				this.m_actualTrack.Text = string.Empty;
				this.clearComparison(true);
				return;
			}
			this.m_currentTrack = track;
			this.m_hexBox.ByteProvider = this.m_tracks[track];
			if (this.m_compareTracks.Count > 0)
			{
				this.syncCompareTrack();
			}
		}

		private void updateSectorText(int track, int sector)
		{
			int num = this.m_tracks[track].reportedSector(sector);
			bool flag = this.m_tracks[track].doubleDensity(sector);
			byte b = this.m_tracks[track].dataMarker(sector);
			if (num == -1)
			{
				this.m_actualSector.Text = "Bad Sector Header";
				return;
			}
			string text = "Sector " + num.ToString() + (flag ? " (double density)" : " (single density)");
			if (!this.m_tracks[track].goodHeaderCRC(sector))
			{
				text += " (bad ID CRC)";
			}
			if (!this.m_tracks[track].goodDataCRC(sector))
			{
				text += " (bad Data CRC)";
			}
			byte b2 = this.m_tracks[track].datacrc1(sector);
			byte b3 = this.m_tracks[track].datacrc2(sector);
			text = text + " Data CRC:" + b2.ToString("X2") + b3.ToString("X2");
			if (b != 0)
			{
				text = text + " Data Marker:" + b.ToString("X2");
			}
			this.m_actualSector.Text = text;
		}

		private void byteProvider_Changed(object sender, EventArgs e)
		{
			if (this.m_currentTrack != -1)
			{
				this.m_tracks[this.m_currentTrack].modified = true;
				int sector = Convert.ToInt32(this.m_sectorUpDown1.Value);
				this.updateSectorText(this.m_currentTrack, sector);
			}
		}

		private void updateSectorNumber(int track, int sector)
		{
			if (this.m_mode == Form1.ViewMode.MODE_TRACK)
			{
				return;
			}
			if (this.m_tracks.Count == 0)
			{
				this.m_actualTrack.Text = string.Empty;
				this.m_actualSector.Text = string.Empty;
				this.clearComparison(true);
				this.m_currentTrack = track;
				return;
			}
			if (this.m_currentTrack != -1)
			{
				this.m_tracks[this.m_currentTrack].applySectorChanges();
			}
			this.m_currentTrack = track;
			if (this.m_hexBox.ByteProvider != null)
			{
				this.m_hexBox.ByteProvider.Changed -= new EventHandler(this.byteProvider_Changed);
			}
			this.m_hexBox.ByteProvider = this.m_tracks[track].sectorData(sector);
			if (this.m_hexBox.ByteProvider != null)
			{
				this.m_hexBox.ByteProvider.Changed += new EventHandler(this.byteProvider_Changed);
			}
			int num = this.m_tracks[track].reportedTrack(sector);
			if (num == -1)
			{
				this.m_actualTrack.Text = "Bad Sector Header";
			}
			else
			{
				this.m_actualTrack.Text = "Track " + num.ToString();
			}
			this.updateSectorText(track, sector);
			if (this.m_compareTracks.Count > 0)
			{
				this.syncCompareTrack();
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void m_trackUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if (this.inTrackEvent)
			{
				return;
			}
			this.inTrackEvent = true;
			int num = 0;
			int num2 = Convert.ToInt32(this.m_trackUpDown1.Value);
			if (this.m_mode == Form1.ViewMode.MODE_TRACK)
			{
				this.updateTrackNumber(num2);
				this.inTrackEvent = false;
				return;
			}
			if (num2 < this.m_tracks.Count)
			{
				num = this.m_tracks[Convert.ToInt32(this.m_trackUpDown1.Value)].sectorCount;
			}
			if (num == 0)
			{
				this.m_sectorUpDown1.Maximum = (this.m_sectorUpDown1.Value = 0m);
				this.updateSectorNumber(Convert.ToInt32(this.m_trackUpDown1.Value), Convert.ToInt32(this.m_sectorUpDown1.Value));
				this.inTrackEvent = false;
				return;
			}
			this.m_sectorUpDown1.Value = 0m;
			int num3 = num - 1;
			if (num3 != this.m_sectorUpDown1.Maximum)
			{
				if (num3 == 0)
				{
					this.m_sectorUpDown1.Maximum = 0m;
				}
				else
				{
					this.m_sectorUpDown1.Maximum = num3;
				}
			}
			this.updateSectorNumber(Convert.ToInt32(this.m_trackUpDown1.Value), Convert.ToInt32(this.m_sectorUpDown1.Value));
			this.inTrackEvent = false;
		}

		private void m_sectorUpDown1_ValueChanged(object sender, EventArgs e)
		{
			if (!this.inTrackEvent)
			{
				this.updateSectorNumber(Convert.ToInt32(this.m_trackUpDown1.Value), Convert.ToInt32(this.m_sectorUpDown1.Value));
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
		}

		private void compareToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string text = Settings.Default.InputPath;
			if (string.IsNullOrEmpty(text))
			{
				text = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			}
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Select DMK File to compare";
			openFileDialog.CheckFileExists = true;
			openFileDialog.CheckPathExists = true;
			openFileDialog.InitialDirectory = text;
			openFileDialog.Filter = "DMK files (*.dmk)|*.dmk|All files (*.*)|*.*";
			openFileDialog.FilterIndex = 1;
			openFileDialog.RestoreDirectory = true;
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string fileName = openFileDialog.FileName;
			if (string.Compare(fileName, this.m_currentFile, true) == 0)
			{
				MessageBox.Show("Cannot compare file to itself", "Compare Error");
				return;
			}
			Settings.Default.InputPath = Path.GetDirectoryName(fileName);
			Settings.Default.Save();
			this.m_hexBox2.ByteProvider = null;
			this.m_compareTracks.Clear();
			this.nextDifferenceToolStripMenuItem.Enabled = false;
			this.previousDifferenceToolStripMenuItem.Enabled = false;
			this.m_compareInfo.Text = string.Empty;
			this.m_header2Info.Text = string.Empty;
			this.m_rawDMK2 = null;
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					this.m_rawDMK2 = binaryReader.ReadBytes((int)fileStream.Length);
				}
			}
			this.m_header2Info.Visible = false;
			this.m_header2Info.Text = "File: " + Path.GetFileName(fileName);
			if (this.parseDMKHeader2())
			{
				this.buildTrackList(ref this.m_rawDMK2, ref this.m_compareTracks, this.m_track2Length);
			}
			else
			{
				this.m_header2Info.Text = string.Empty;
			}
			this.m_header2Info.Visible = true;
			this.nextDifferenceToolStripMenuItem.Enabled = (this.m_compareTracks.Count<Track>() > 0);
			this.previousDifferenceToolStripMenuItem.Enabled = (this.m_compareTracks.Count<Track>() > 0);
			this.syncCompareTrack();
		}

		private void m_differenceBar_Scroll(object sender, EventArgs e)
		{
			this.showComparison();
		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Copyright 2014 by Alan Page\nhttp://virtualfloppy.blogspot.ca", "Version 1.01");
		}

		private void trackModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.changeMode(Form1.ViewMode.MODE_TRACK);
			this.sectorModeToolStripMenuItem.Checked = false;
		}

		private void sectorModeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			this.changeMode(Form1.ViewMode.MODE_SECTOR);
			this.trackModeToolStripMenuItem.Checked = false;
		}

		private void allowEditingToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.allowEditingToolStripMenuItem.Checked)
			{
				this.m_hexBox.ReadOnly = false;
			}
			else
			{
				this.m_hexBox.ReadOnly = true;
			}
			this.m_editMessage.Visible = this.m_hexBox.ReadOnly;
		}

		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string text = Settings.Default.InputPath;
			if (string.IsNullOrEmpty(text))
			{
				text = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Title = "Save DMK File";
			saveFileDialog.CheckPathExists = true;
			saveFileDialog.CreatePrompt = true;
			saveFileDialog.InitialDirectory = text;
			saveFileDialog.Filter = "DMK files (*.dmk)|*.dmk|All files (*.*)|*.*";
			saveFileDialog.FilterIndex = 1;
			saveFileDialog.RestoreDirectory = true;
			if (saveFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string fileName = saveFileDialog.FileName;
			Settings.Default.InputPath = Path.GetDirectoryName(fileName);
			Settings.Default.Save();
			foreach (Track current in this.m_tracks)
			{
				current.applySectorChanges();
				if (current.HasChanges())
				{
					current.applyChanges(this.m_rawDMK1);
				}
			}
			try
			{
				using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
					{
						binaryWriter.Write(this.m_rawDMK1);
					}
				}
			}
			catch (Exception)
			{
				MessageBox.Show("Error saving file", "File Error");
			}
		}


	}
}
