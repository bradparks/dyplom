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
        int index_zaznaczonego_markera = 0;


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
            for (int i = 0; i < CMdllka.touchlessMgr.MarkerCount; i++)
            {
                CMdllka.touchlessMgr.Markers[i].Name = (i+1).ToString();
                checkedListBox1.Items.Add(CMdllka.touchlessMgr.Markers[i].ToString(), true);
                CMdllka.touchlessMgr.Markers[i].Highlight = true;
            }
            zmiana_markera();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            pictureBox1 = CMdllka.Markery.doadnie_markera(pictureBox1);
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            for (int i = checkedListBox1.Items.Count - 1; i >= 0; i--)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    CMdllka.touchlessMgr.RemoveMarker(i);
                }
            }
            MarkersChanged(this, e);
            
        }

        private void zmiana_markera()
        {
            int ilosc_aktywnych_markerow = 0;
            
            for (int i = 0; i < CMdllka.touchlessMgr.Markers.Count; i++)
            {
                if (CMdllka.touchlessMgr.Markers[i].Highlight == true)
                {
                    ilosc_aktywnych_markerow++;
                    index_zaznaczonego_markera = i;
                }
            }
            if (ilosc_aktywnych_markerow == 1)
            {
                label1.Text = CMdllka.touchlessMgr.Markers[index_zaznaczonego_markera].ToString();
                panel1.Enabled = true;
                uzupelnienie_danych_markera(index_zaznaczonego_markera);
            }
            else
            {
                label1.Text = "Zaznacz tylko jeden marker do edycji ustawień";
                panel1.Enabled = false;
            }
        }

        private void uzupelnienie_danych_markera(int numer_markera)
        {
            numericUpDown1.Value = CMdllka.touchlessMgr.Markers[numer_markera].Threshold;
        }

        private void checkedListBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            CMdllka.touchlessMgr.Markers[checkedListBox1.SelectedIndex].Highlight = checkedListBox1.GetItemChecked(checkedListBox1.SelectedIndex);
            zmiana_markera();
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0)
                return;
            Camera c = (Camera)comboBox1.SelectedItem;
            c.ShowPropertiesDialog(this.Handle);
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            CMdllka.touchlessMgr.Markers[index_zaznaczonego_markera].Threshold =Convert.ToInt32(numericUpDown1.Value);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CMdllka.touchlessMgr.Markers[index_zaznaczonego_markera].SmoothingEnabled = checkBox1.Checked;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            CMdllka.Symulator_Myszki.Martwa_Strefa.start();
        }
    }
}
