using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CatchMouseDll;
using TouchlessLib;

namespace CatchMouse
{
    public partial class Form1 : Form
    {
        CatchMouseDll.CatchMouseDll CMdllka;

        public Form1()
        {
            InitializeComponent();
            CMdllka = new CatchMouseDll.CatchMouseDll(pictureBox1);
            pictureBox1 = CMdllka.Aktywacja_Obrazu.Przechwyt_Obrazu();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource= CMdllka.touchlessMgr.Cameras;
            CMdllka.Markery.MarkerChanged += new Markery.Zmiana_Ilosci_Markerow(MarkersChanged);
        }

        private void MarkersChanged(object sender, EventArgs e)
        {
            checkedListBox1.Items.Clear();
            foreach (Marker mk in CMdllka.touchlessMgr.Markers)
            {
                checkedListBox1.Items.Add(mk.ToString(),true);
                mk.Highlight = true;
            }
        }


        

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
            return;
            Camera c = (Camera)comboBox1.SelectedItem;
            c.ShowPropertiesDialog(this.Handle);
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Markey = CMdllka.touchlessMgr;
            Properties.Settings.Default.Save();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            pictureBox1 = CMdllka.Markery.doadnie_markera(pictureBox1);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            button3.Text = "oko";
            Properties.Settings.Default.Setting = button2.Text + 1;
            Properties.Settings.Default.Save();
            for (int i = checkedListBox1.Items.Count - 1; i >= 0; i--)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    CMdllka.touchlessMgr.RemoveMarker(i);
                }
            }
            MarkersChanged(this, e);
        }

        private void checkedListBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            CMdllka.touchlessMgr.Markers[checkedListBox1.SelectedIndex].Highlight = checkedListBox1.GetItemChecked(checkedListBox1.SelectedIndex);
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {

        }
    }
}
