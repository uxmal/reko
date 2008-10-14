using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Forms
{
    public class FindDialogInteractor
    {
        private FindDialog dlg;

        public FindDialog CreateDialog()
        {
            dlg = new FindDialog();
            dlg.FindText.TextChanged += new EventHandler(FindText_TextChanged);
            UpdateUI();
            return dlg;
        }

        private void UpdateUI()
        {
            dlg.FindButton.Enabled = ToHexadecimal(dlg.FindText.Text) != null;
        }

        protected void FindText_TextChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        public byte[] ToHexadecimal(string str)
        {
            List<byte> bytes = new List<byte>();
            str = str.Replace(" ", "").Replace("\t", "");
            if (str.Length == 0)
                return null;
            if (str.Length % 2 == 1)
                return null;                // input must have even # of hex digits.
            for (int i = 0; i < str.Length; i += 2)
            {
                int n = HexDigit(str[i]);
                if (n < 0)
                    return null;
                int nn = HexDigit(str[i + 1]);
                if (nn < 0)
                    return null;

                bytes.Add((byte) (n * 16 + nn));
            }
            return bytes.ToArray();
        }

        private int HexDigit(char p)
        {
            if ('0' <= p && p <= '9')
                return p - '0';
            else if ('A' <= p && p <= 'F')
                return p - 'A' + 10;
            else if ('a' <= p && p <= 'f')
                return p - 'a' + 10;
            else
                return -1;
        }
    }
}
