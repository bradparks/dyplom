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
            pictureBox1 = CMdllka.Przechwyt_Obrazu();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource= CMdllka.touchlessMgr.Cameras;
            CMdllka.a_obrazu.MarkerChanged += new Aktywizacja_Obrazu.Zmiana_Ilosci_Markerow(MarkersChanged);
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

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1 = CMdllka.dodaj_marker();
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CMdllka.touchlessMgr.Markers[checkedListBox1.SelectedIndex].Highlight = checkedListBox1.GetItemChecked(checkedListBox1.SelectedIndex);
             
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = checkedListBox1.Items.Count-1; i >=0 ; i--)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    CMdllka.touchlessMgr.RemoveMarker(i);
                }
            }
            MarkersChanged(this, e);
        }
    }
}
