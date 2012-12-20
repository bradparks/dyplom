using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TouchlessLib;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;


namespace CatchMouseDll
{

    class Podklasy
    {
    }

    /// <summary>
    /// Wzorzec projektowy Singletona
    /// </summary>
    public sealed class Wspolny
    {
        public static TouchlessMgr w_touchlessMgr;
        public static System.Windows.Forms.PictureBox w_picturebox;
        public static bool pauza;
        public static Image ostatnia_klatka; //_latestFrame
        public static bool flaga_dodanie_markera;
        public static Point srodek_zaznaczenia_markera;  //_markerCenter
        public static float promien_zaznaczenia_markera; //_markerRadius

        public static int X()
        {
            int suma_x = 0;
            int ilosc = 0;
            object obi = new object();
            foreach (Marker mk in Wspolny.w_touchlessMgr.Markers)
            {
                ilosc++;
                suma_x += mk.CurrentData.X;
            }
            suma_x = suma_x / ilosc;
            return suma_x;
        }

        public static int Y()
        {
            int suma_y = 0;
            int ilosc = 0;
            object obi = new object();
            foreach (Marker mk in Wspolny.w_touchlessMgr.Markers)
            {
                ilosc++;
                suma_y += mk.CurrentData.Y;
            }
            suma_y = suma_y / ilosc;
            return suma_y;
        }
    }
    /// <summary>
    /// klasa odpowiedzialna za przechwytywanie obrazu z kamery do Wspolny.w_pictureboxa
    /// </summary>
    public class Aktywizacja_Obrazu
    {
        
        private DateTime czasOstatniejKlatki;
        private int licznikKlatek=0;
        

        /// <summary>
        /// konstruktor 
        /// </summary>
        /// <param name="picture">pobiera obraz do przypisania kamery</param>
        /// <param name="touchlessM">glowna klasa biblioteki Touchless</param>
        public Aktywizacja_Obrazu()
        {
            Wspolny.pauza = false;
        }

        /// <summary>
        /// Zwrot obrazu z kamery
        /// </summary>
        /// <returns>zwraca Wspolny.w_pictureboxa z obrazem kamery</returns>
        public System.Windows.Forms.PictureBox Przechwyt_Obrazu()
        {
            if (Wspolny.w_touchlessMgr.Cameras.Count > 0)
            {
                // przechwycenie klatki z kamery
                Wspolny.w_touchlessMgr.Cameras[0].OnImageCaptured += new EventHandler<CameraEventArgs>(OnImageCaptured);
                Wspolny.w_touchlessMgr.CurrentCamera = Wspolny.w_touchlessMgr.Cameras[0];
                //rysowanie ostatniej klatki
                Wspolny.w_picturebox.Paint += new PaintEventHandler(RysowanieOstatniegoObrazu);
                czasOstatniejKlatki = DateTime.Now;
            }
            else MessageBox.Show("Fatal error: Kamera nie jest podlaczona do komputera");   
            return Wspolny.w_picturebox;
        }

        /// <summary>
        /// Zwrot obrazu z kamery
        /// </summary>
        /// <param name="numer_kamery">Z której kamery ma zostac zwrocony obraz, bez parametru 0-wy index kamery</param>
        /// <returns>zwraca Wspolny.w_pictureboxa z obrazem kamery</returns>
        public System.Windows.Forms.PictureBox Przechwyt_Obrazu(int numer_kamery)
        {
            if (Wspolny.w_touchlessMgr.Cameras.Count > numer_kamery)
            {
                // przechwycenie klatki z kamery
                Wspolny.w_touchlessMgr.Cameras[numer_kamery].OnImageCaptured += new EventHandler<CameraEventArgs>(OnImageCaptured);
                Wspolny.w_touchlessMgr.CurrentCamera = Wspolny.w_touchlessMgr.Cameras[numer_kamery];
                //rysowanie ostatniej klatki
                Wspolny.w_picturebox.Paint += new PaintEventHandler(RysowanieOstatniegoObrazu);
                czasOstatniejKlatki = DateTime.Now;
            }
            else MessageBox.Show("Fatal error: Podany numer kamery jest za duzy");           
            return Wspolny.w_picturebox;
        }

        /// <summary>
        /// Zmiana numeru kamery do przechwytywania obrazu
        /// </summary>
        /// <param name="numer_kamery">Z której kamery ma zostac zwrocony obraz</param>
        /// <returns></returns>
        public System.Windows.Forms.PictureBox Zmiana_Numeru_Kamery(int numer_kamery)
        {
            if (Wspolny.w_touchlessMgr.Cameras.Count > numer_kamery)
            {
                if (Wspolny.w_touchlessMgr.CurrentCamera != null)
                {
                    Wspolny.w_touchlessMgr.CurrentCamera.OnImageCaptured -= new EventHandler<CameraEventArgs>(OnImageCaptured);
                    Wspolny.w_touchlessMgr.CurrentCamera.Dispose();
                    Wspolny.w_touchlessMgr.CurrentCamera = null;
                    Wspolny.w_picturebox.Paint -= new PaintEventHandler(RysowanieOstatniegoObrazu);
                }
            
                // przechwycenie klatki z kamery
                Wspolny.w_touchlessMgr.Cameras[numer_kamery].OnImageCaptured += new EventHandler<CameraEventArgs>(OnImageCaptured);
                Wspolny.w_touchlessMgr.CurrentCamera = Wspolny.w_touchlessMgr.Cameras[numer_kamery];
                //rysowanie ostatniej klatki
                Wspolny.w_picturebox.Paint += new PaintEventHandler(RysowanieOstatniegoObrazu);
                czasOstatniejKlatki = DateTime.Now;
            }
            else MessageBox.Show("Fatal error: Podany numer kamery jest za duzy"); 
            return Wspolny.w_picturebox;
        }

        /// <summary>
        /// rysowanie obrazu z kamery
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">zdarzenie nadejścia nowej klatki</param>
        private void RysowanieOstatniegoObrazu(object sender, PaintEventArgs e)
        {
            if (Wspolny.ostatnia_klatka != null)
            {
                // Rysuje ostatnią klatke z aktywnej kamery
                e.Graphics.DrawImage(Wspolny.ostatnia_klatka, 0, 0, Wspolny.w_picturebox.Width, Wspolny.w_picturebox.Height);
                if (Wspolny.flaga_dodanie_markera == true)
                {
                    Pen pen = new Pen(Brushes.Red, 1);
                    e.Graphics.DrawEllipse(pen, Wspolny.srodek_zaznaczenia_markera.X - Wspolny.promien_zaznaczenia_markera,
                        Wspolny.srodek_zaznaczenia_markera.Y - Wspolny.promien_zaznaczenia_markera,
                        2 * Wspolny.promien_zaznaczenia_markera, 2 * Wspolny.promien_zaznaczenia_markera);

                }
            }
        }

        /// <summary>
        /// Przechwycenie ostatniej klatki, odświerzenie Wspolny.w_pictureboxa
        /// </summary>
        private void OnImageCaptured(object sender, CameraEventArgs args)
        {
            // Przeliczanie FPS
            licznikKlatek++;
            double milliseconds = (DateTime.Now.Ticks - czasOstatniejKlatki.Ticks) / TimeSpan.TicksPerMillisecond;
            if (Wspolny.pauza == false)
            {
                Wspolny.ostatnia_klatka = args.Image;
                Wspolny.w_picturebox.Invalidate();
            }
        }
    }

    public class Markery
    {
        public delegate void Zmiana_Ilosci_Markerow(object sender, EventArgs e);
        public event Zmiana_Ilosci_Markerow MarkerChanged;
        protected void OnMarkerChanged()
        {
            if (MarkerChanged != null)
                MarkerChanged(this, EventArgs.Empty);
        }
        /// <summary>
        /// dodaje marker 
        /// </summary>
        public System.Windows.Forms.PictureBox doadnie_markera(PictureBox picturebox)
        {
            Wspolny.w_picturebox = picturebox;
            Wspolny.pauza = true;
            
            Wspolny.w_picturebox.MouseDown += new MouseEventHandler(w_picturebox_MouseDown);
            Wspolny.w_picturebox.MouseMove += new MouseEventHandler(w_picturebox_MouseMove);
            Wspolny.w_picturebox.MouseUp += new MouseEventHandler(w_picturebox_MouseUp);
            return Wspolny.w_picturebox;
        }

        /// <summary>
        /// wcisniecie przycisku myszy - wyznaczenie srodka okregu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void w_picturebox_MouseDown(object sender, MouseEventArgs e)
        {
            Wspolny.flaga_dodanie_markera = true;
            Wspolny.srodek_zaznaczenia_markera = e.Location;
            Wspolny.promien_zaznaczenia_markera = 0;
        }

        /// <summary>
        /// zwolnienie przycisku myszy - dodanie nowego markera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void w_picturebox_MouseUp(object sender, MouseEventArgs e)
        {
            int dx = e.X - Wspolny.srodek_zaznaczenia_markera.X;
            int dy = e.Y - Wspolny.srodek_zaznaczenia_markera.Y;
            Wspolny.promien_zaznaczenia_markera = (float)Math.Sqrt(dx * dx + dy * dy);
            // Adjust for the image/display scaling (assumes proportional scaling)

            Wspolny.srodek_zaznaczenia_markera.X = (Wspolny.srodek_zaznaczenia_markera.X * Wspolny.ostatnia_klatka.Width) / Wspolny.w_picturebox.Width;
            Wspolny.srodek_zaznaczenia_markera.Y = (Wspolny.srodek_zaznaczenia_markera.Y * Wspolny.ostatnia_klatka.Height) / Wspolny.w_picturebox.Height;
            Wspolny.promien_zaznaczenia_markera = (Wspolny.promien_zaznaczenia_markera * Wspolny.ostatnia_klatka.Height) / Wspolny.w_picturebox.Height;
            // Add the marker
            Wspolny.w_touchlessMgr.AddMarker( (Wspolny.w_touchlessMgr.MarkerCount + 1).ToString(), (Bitmap)Wspolny.ostatnia_klatka, Wspolny.srodek_zaznaczenia_markera, Wspolny.promien_zaznaczenia_markera);

            // Restore the app to its normal state and clear the selection area adorment
            Wspolny.srodek_zaznaczenia_markera = new Point();
            Wspolny.w_picturebox.Image = new Bitmap(Wspolny.w_picturebox.Width, Wspolny.w_picturebox.Height);
            
            Wspolny.w_picturebox.MouseDown -= new MouseEventHandler(w_picturebox_MouseDown);
            Wspolny.w_picturebox.MouseMove -= new MouseEventHandler(w_picturebox_MouseMove);
            Wspolny.w_picturebox.MouseUp -= new MouseEventHandler(w_picturebox_MouseUp);

            Wspolny.flaga_dodanie_markera = false;
            Wspolny.pauza = false;
            OnMarkerChanged();
            
            //MarkerChanged.Invoke(this, new Zmiana_Ilosci_Markerow(this,e) );
        }

        /// <summary>
        /// przemieszczenie myszy - wyznaczenie promienia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void w_picturebox_MouseMove(object sender, MouseEventArgs e)
        {
            // If the user is selecting a marker, draw a circle of their selection as a selection adornment
            if (!Wspolny.srodek_zaznaczenia_markera.IsEmpty)
            {
                // Get the current radius
                int dx = e.X - Wspolny.srodek_zaznaczenia_markera.X;
                int dy = e.Y - Wspolny.srodek_zaznaczenia_markera.Y;
                Wspolny.promien_zaznaczenia_markera = (float)Math.Sqrt(dx * dx + dy * dy);
                // Cause display update
                Wspolny.w_picturebox.Invalidate();
            }
        }
    }
    

    public class Symulator_Myszki
    {
        public Martwa_Strefa Martwa_Strefa = new Martwa_Strefa() ;
    }
    public class Martwa_Strefa
    {
        public int martwa_x=5;
        public int martwa_y=5;
        public double przyspieszenie_x = 1;
        public double przyspieszenie_y = 1;

        int start_x = 0;
        int start_y = 0;
        double curent_x = 0;
        double curent_y = 0;
        int stary_curent_x;
        int stary_curent_y;
        DateTime CurrTime;
        private static Marker _markerSelected;
        int ilosc_markerow;

        public void start()
        {
            _markerSelected.OnChange -= new EventHandler<MarkerEventArgs>(OnSelectedMouseUpdate);
        }

        public void OnSelectedMouseUpdate(object sender, MarkerEventArgs args)
        {/*
            this.BeginInvoke(new Action<MarkerEventData>(UpdateMouseDataInUI), new object[] { args.EventData });
        }

        private void UpdateMouseDataInUI(MarkerEventData data)
        {*/
            if (ilosc_markerow != Wspolny.w_touchlessMgr.Markers.Count)
            {
                start_x = Wspolny.w_touchlessMgr.Markers[Wspolny.w_touchlessMgr.Markers.Count - 1].CurrentData.X;
                start_y = Wspolny.w_touchlessMgr.Markers[Wspolny.w_touchlessMgr.Markers.Count - 1].CurrentData.Y;
                curent_x = 100;
                curent_y = 100;
                ilosc_markerow = Wspolny.w_touchlessMgr.Markers.Count;


            }
            int poz_x = 0;
            int poz_y = 0;

            poz_x = Wspolny.w_touchlessMgr.Markers[0].CurrentData.X;
            poz_y = Wspolny.w_touchlessMgr.Markers[0].CurrentData.Y;

          //  if (flaga_mouse == true)
            {

                poz_x = Wspolny.X();
                poz_y = Wspolny.Y();
                if (start_x > poz_x + martwa_x || start_x < poz_x - martwa_x)
                {
                    if (curent_x < SystemInformation.PrimaryMonitorSize.Width)
                    {
                        curent_x += (start_x - poz_x) * przyspieszenie_x / 10;
                        if (Convert.ToInt32(curent_x) <= 10)
                            curent_x = 10;
                       // SetCursorPos(Convert.ToInt32(curent_x), Convert.ToInt32(curent_y));
                        CurrTime = DateTime.Now;
                    }
                    else
                    {
                        curent_x -= 5;
                    }
                }


                if (start_y > poz_y + martwa_y || start_y < poz_y - martwa_y)
                {
                    if (curent_y < SystemInformation.PrimaryMonitorSize.Height - 20)
                    {
                        curent_y -= (start_y - poz_y) * przyspieszenie_y / 10;
                        if (Convert.ToInt32(curent_y) <= 20)
                            curent_y = 10;
                     //   SetCursorPos(Convert.ToInt32(curent_x), Convert.ToInt32(curent_y));
                        CurrTime = DateTime.Now;
                    }
                    else
                    {
                        curent_y -= 5;
                    }
                }
/*
                if ((DateTime.Now.Second - CurrTime.Second) > czas_reakcji)
                {
                    if (flaga_okna_myszy == false)
                        stary_curent_x = Convert.ToInt32(curent_x);
                    stary_curent_y = Convert.ToInt32(curent_y);
                    myForm.Location = new Point(Convert.ToInt32(curent_x) - 100, Convert.ToInt32(curent_y) - 100);
                    myForm.Visible = true;
                }*/
            }
        }
    }
}
