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

using Hotkeys;
using System.Runtime.InteropServices;


namespace CatchMouse
{
    public partial class Form1 : Form
    {
        private Hotkeys.GlobalHotkey ghk;
        CatchMouseDll.CatchMouseDll CMdllka;
        int index_zaznaczonego_markera = 0;
        List<Camera> Camra = new List<Camera>();
        
        public Form1()
        {
            InitializeComponent();
            CMdllka = new CatchMouseDll.CatchMouseDll(pictureBox1);
            pictureBox1 = CMdllka.Aktywacja_Obrazu.Przechwyt_Obrazu();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < CMdllka.touchlessMgr.Cameras.Count; i++)
            {
                if(CMdllka.touchlessMgr.Cameras[i].ToString().Contains("VDP")==false )
                    Camra.Add(CMdllka.touchlessMgr.Cameras[i]);
            }
            comboBox1.DataSource = Camra;
            CMdllka.Markery.MarkerChanged += new Markery.Zmiana_Ilosci_Markerow(MarkersChanged);
            ghk = new Hotkeys.GlobalHotkey(Constants.CTRL, Keys.Q, this);
            ghk.Register();
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

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Hotkeys.Constants.WM_HOTKEY_MSG_ID)
            {
                //MessageBox.Show("Koniec Myszki");
                CMdllka.Symulator_Myszki.Martwa_Strefa.Stop();
            }
            base.WndProc(ref m);
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
            //CMdllka.touchlessMgr.Cameras.IndexOf(Camra[comboBox1.SelectedIndex]);
            pictureBox1 = CMdllka.Aktywacja_Obrazu.Przechwyt_Obrazu(CMdllka.touchlessMgr.Cameras.IndexOf(Camra[comboBox1.SelectedIndex]));
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

        #region strefa
        private void button4_Click(object sender, EventArgs e)
        {
            if (CMdllka.Symulator_Myszki.Martwa_Strefa.flga_dzialania == false)
            {
                List<int> lista = new List<int>();
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (checkedListBox1.GetItemChecked(i) == true)
                        lista.Add(i);
                }
                CMdllka.Symulator_Myszki.Martwa_Strefa.Start(lista);
                button4.Text = "Stop";
            }
            else
            {
                CMdllka.Symulator_Myszki.Martwa_Strefa.Stop();
                button4.Text = "Start";
            }
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            CMdllka.Symulator_Myszki.Martwa_Strefa.martwa_x = Convert.ToInt32(numericUpDown4.Value);
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            CMdllka.Symulator_Myszki.Martwa_Strefa.martwa_y = Convert.ToInt32(numericUpDown3.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            CMdllka.Symulator_Myszki.Martwa_Strefa.przyspieszenie_x = Convert.ToInt32(numericUpDown2.Value);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            CMdllka.Symulator_Myszki.Martwa_Strefa.przyspieszenie_y = Convert.ToInt32(numericUpDown3.Value);
        }

        #endregion
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = CMdllka.Symulator_Myszki.Martwa_Strefa.curent_x.ToString();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        
    }
}
