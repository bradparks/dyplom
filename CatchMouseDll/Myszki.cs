using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TouchlessLib;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

using Hotkeys;

namespace CatchMouseDll
{
    class Myszki
    {
    }


    public class Symulator_Myszki
    {
        public Martwa_Strefa Martwa_Strefa = new Martwa_Strefa();
    }

    public class Dane_Myszki
    {
        public DateTime CurrTime = DateTime.Now;
    }
    /// <summary>
    /// Klasa odpoweidzialna za symulacje myszy z strefą bezruchu
    /// </summary>
    public class Martwa_Strefa : Dane_Myszki
    {
        [DllImport("user32")]
        private static extern int SetCursorPos(int x, int y);

        /// <summary>
        /// Open window when cursor stop ower time[s]
        /// </summary>
        public int czas_otwarcia_okienka = 5;
        public int martwa_x = 5;
        public int martwa_y = 5;
        public int przyspieszenie_x = 11;
        public int przyspieszenie_y = 11;
        public bool flga_dzialania = false;

        private int start_x = 0;
        private int start_y = 0;

        public double curent_x = 0;
        public double curent_y = 0;
        List<Marker> _markerSelected = new List<Marker>();
        int ilosc_markerow;

        Click myForm;

        public Martwa_Strefa()
        {
            myForm = new Click(this);
            myForm.Show();
            myForm.Visible = false;
        }

        public void Start(List<int> numerek)
        {
            for (int i = 0; i < numerek.Count; i++)
            {
                _markerSelected.Add(Wspolny.w_touchlessMgr.Markers[numerek[i]]);
                _markerSelected[i].OnChange += new EventHandler<MarkerEventArgs>(OnSelectedMouseUpdate);
            }
            flga_dzialania = true;
            base.CurrTime = DateTime.Now;
        }
        /// <summary>
        /// Zatrzymanie Martwej Strefy
        /// </summary>
        public void Stop()
        {
            foreach (Marker mk in _markerSelected)
            {
                mk.OnChange -= new EventHandler<MarkerEventArgs>(OnSelectedMouseUpdate);
            }
            _markerSelected.Clear();
            flga_dzialania = false;
        }

        public void OnSelectedMouseUpdate(object sender, MarkerEventArgs args)
        {
            myForm.BeginInvoke(new Action<MarkerEventData>(UpdateMarkerDataInUI), new object[] { args.EventData });
        }

        private void UpdateMarkerDataInUI(MarkerEventData data)
        {
            if (ilosc_markerow != Wspolny.w_touchlessMgr.Markers.Count)
            {
                start_x = data.X;
                start_y = data.Y;

                ilosc_markerow = Wspolny.w_touchlessMgr.Markers.Count;
            }
            curent_x = Cursor.Position.X;
            curent_y = Cursor.Position.Y;

            int poz_x = 0;
            int poz_y = 0;

            poz_x = data.X;
            poz_y = data.Y;
            if (start_x > poz_x + martwa_x || start_x < poz_x - martwa_x)
            {
                curent_x += (start_x - poz_x) * (przyspieszenie_x / 10) / _markerSelected.Count;
                if (Convert.ToInt32(curent_x) <= 10)
                    curent_x = 10;
                SetCursorPos(Convert.ToInt32(curent_x), Convert.ToInt32(curent_y));
                base.CurrTime = DateTime.Now;
            }


            if (start_y > poz_y + martwa_y || start_y < poz_y - martwa_y)
            {
                curent_y -= (start_y - poz_y) * (przyspieszenie_y / 10) / _markerSelected.Count;
                if (Convert.ToInt32(curent_y) <= 10)
                    curent_y = 10;
                SetCursorPos(Convert.ToInt32(curent_x), Convert.ToInt32(curent_y));
                base.CurrTime = DateTime.Now;
            }
            //sprawdzam czy zostal przekroczony czas bezruchu potrzebny do otwarcia okna przycisku myszy
            if ((DateTime.Now.Second - base.CurrTime.Second) > czas_otwarcia_okienka)
            {
                if (myForm.Visible == false)
                {
                    myForm.Location = new Point(Convert.ToInt32(curent_x) - 100, Convert.ToInt32(curent_y) - 100);
                    this.myForm.Visible = true;
                }
            }
        }
    }
}
