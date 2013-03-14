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
        public Usrednianie Usrednianie = new Usrednianie();
    }

    public class Dane_Myszki
    {
        public DateTime CurrTime = DateTime.Now;
        [DllImport("user32")]
        protected static extern int SetCursorPos(int x, int y);
    }
    /// <summary>
    /// Klasa odpoweidzialna za symulacje myszy z strefą bezruchu
    /// </summary>
    public class Martwa_Strefa : Dane_Myszki
    {
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
                start_x = Wspolny.X(_markerSelected);
                start_y = Wspolny.Y(_markerSelected);

                ilosc_markerow = Wspolny.w_touchlessMgr.Markers.Count;
            }
            curent_x = Cursor.Position.X;
            curent_y = Cursor.Position.Y;

            int poz_x = 0;
            int poz_y = 0;

            poz_x = Wspolny.X(_markerSelected);
            poz_y = Wspolny.Y(_markerSelected);
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

    public class Usrednianie : Dane_Myszki
    {
        public int czas_otwarcia_okienka = 5;
        public int liczba_usrednianych_klatek = 5;
        public double curent_x = 0;
        public double curent_y = 0;

        public bool flga_dzialania = false;
        private List<int> x = new List<int>();
        private List<int> y = new List<int>();
        List<Marker> _markerSelected = new List<Marker>();

        int rodzaj_sredniej = 0;
        Click myForm;

        public Usrednianie()
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

        public string Rodzaj_sredniej
        {
            get
            {
                if (rodzaj_sredniej == 0)
                    return "SMA";
                else if (rodzaj_sredniej == 1)
                    return "WMA";
                else 
                    return "EMA";
            }
            set
            {
                if (value == "SMA" || value == "sma" || value == "0")
                    rodzaj_sredniej = 0;
                else if (value == "WMA" || value == "wma" || value == "1")
                    rodzaj_sredniej = 1;
                else if (value == "EMA" || value == "ema" || value == "2")
                    rodzaj_sredniej = 2;
            }
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
        private double potega(double wartosc, int potega)
        {
            double zwrot = 0;
            for (int i=0;i<potega;i++)
            {
                zwrot = wartosc * zwrot; 
            }
            return zwrot;
        }
        private void UpdateMarkerDataInUI(MarkerEventData data)
        {
            if (x.Count < liczba_usrednianych_klatek)
            {
                x.Add(Wspolny.X(_markerSelected));
                y.Add(Wspolny.Y(_markerSelected));
            }
            else
            {
                x.RemoveAt(0);
                x.Add(Wspolny.X(_markerSelected));
                y.RemoveAt(0);
                y.Add(Wspolny.Y(_markerSelected));

                curent_x = Cursor.Position.X;
                curent_y = Cursor.Position.Y;

                int x_suma = 0;
                int y_suma = 0;
                
                int x_wynik=0;
                int y_wynik=0;
                if (rodzaj_sredniej == 0)
                {
                    for (int i = 0; i < liczba_usrednianych_klatek; i++)
                    {
                        x_suma += x[i];
                        y_suma += y[i];
                    }
                    x_wynik = Convert.ToInt32(x_suma / liczba_usrednianych_klatek - 1);
                    y_wynik = Convert.ToInt32(y_suma / liczba_usrednianych_klatek - 1);
                }
                else if (rodzaj_sredniej == 1)
                {
                    int dzielnik=0;
                    for (int i = 0; i < liczba_usrednianych_klatek; i++)
                    {
                        if (i == 0)
                        {
                            x_suma += x[i];
                            y_suma += y[i];
                        }
                        else
                        {
                            x_suma += x[i] * i;
                            y_suma += y[i] * i;
                        }
                        dzielnik +=i;
                    }
                    x_wynik = Convert.ToInt32(x_suma / dzielnik);
                    y_wynik = Convert.ToInt32(y_suma / dzielnik);
                }
                else if (rodzaj_sredniej == 2)
                {
                    int dzielnik = 0;
                    double alfa = 2 / liczba_usrednianych_klatek;
                    for (int i = 0; i < liczba_usrednianych_klatek; i++)
                    {
                        if (i == 0)
                        {
                            x_suma += x[i];
                            y_suma += y[i];
                            dzielnik += 1;
                        }
                        else
                        {
                            int a = Convert.ToInt32(potega((1 - alfa), i));
                            x_suma += x[i] * a;
                            y_suma += y[i] * a;
                            dzielnik += a;
                        }
                        
                    }
                    x_wynik = Convert.ToInt32(x_suma / dzielnik);
                    y_wynik = Convert.ToInt32(y_suma / dzielnik);
                }
                double y_screen_max = SystemInformation.PrimaryMonitorSize.Height;
                double x_screen_max = SystemInformation.PrimaryMonitorSize.Width;
                double x_camera_max = Wspolny.ostatnia_klatka.Width;
                double y_camera_max = Wspolny.ostatnia_klatka.Height;
                double x_waga = x_screen_max / x_camera_max;
                double y_waga = y_screen_max / y_camera_max;
                SetCursorPos(Convert.ToInt32(x_wynik*x_waga),Convert.ToInt32(y_wynik*y_waga));
               // MessageBox.Show(x_wynik.ToString() + "  " + x_waga.ToString() + "   " + Convert.ToInt32(x_wynik * x_waga).ToString() +"\n"+
                //    x_screen_max.ToString() + "  " + x_camera_max.ToString());
            }
        }
    }
}
