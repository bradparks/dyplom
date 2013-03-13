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

        private static List<int> x_difrent = new List<int>();
        private static List<int> y_difrent = new List<int>();

        /// <summary>
        /// Uśrednione pozycje X
        /// </summary>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static int X(List<Marker> mark)
        {
            for (int i=0;x_difrent.Count < mark.Count;i++)
            {
                if (i == 0)
                {
                    x_difrent.Add(mark[i].CurrentData.X);
                }
                else
                {
                    x_difrent.Add(mark[i].CurrentData.X - mark[i - 1].CurrentData.X);
                    MessageBox.Show(x_difrent.ToString());
                }
            }
            int suma_x = 0;
            int ilosc = 0;
            for (int i=0; i< mark.Count;i++)
            {
                if (i == 0)
                {//blad ??
                    if (x_difrent.Count == 1)
                    {
                        return mark[0].CurrentData.X;
                    }
                    else if ((x_difrent[i + 1] - 5) < (mark[i + 1].CurrentData.X - mark[i].CurrentData.X) || (x_difrent[i + 1] + 5) > (mark[i + 1].CurrentData.X - mark[i].CurrentData.X))
                    {
                        suma_x += mark[i].CurrentData.X;
                        ilosc++;
                    }
                    else
                        MessageBox.Show("xo" + x_difrent[i + 1].ToString() + " != " + mark[i + 1].CurrentData.X.ToString() + " - " + mark[i].CurrentData.X.ToString());
                }
                else
                {
                    //sprawdzam czy marker nie został zgubiony 
                    if ((x_difrent[i] - 5) < (mark[i].CurrentData.X - mark[i - 1].CurrentData.X) || (x_difrent[i] + 5) > (mark[i].CurrentData.X - mark[i - 1].CurrentData.X))
                    {
                        suma_x += mark[i].CurrentData.X;
                        if (mark[i].CurrentData.X > 0)
                            ilosc++;
                    }
                    else
                        MessageBox.Show("xd" + x_difrent[i].ToString() + " != " + mark[i].CurrentData.X.ToString() + " - " + mark[i - 1].CurrentData.X.ToString());
                }
            }
            if ( ilosc!=0)
                suma_x = suma_x / ilosc;
            
            //MessageBox.Show(suma_x.ToString());
            return suma_x;
        }

        /// <summary>
        /// Uśrednione pozycje Y
        /// </summary>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static int Y(List<Marker> mark)
        {
            for (int i = 0; y_difrent.Count < mark.Count; i++)
            {
                if (i == 0)
                {
                    y_difrent.Add(mark[i].CurrentData.Y);
                }
                else
                {
                    y_difrent.Add(mark[i].CurrentData.Y - mark[i - 1].CurrentData.Y);
                }
            }

            int suma_y = 0;
            int ilosc = 0;
            for (int i = 0; i < mark.Count; i++)
            {
                if (i == 0)
                {
                    if (y_difrent.Count == 1)
                    {
                        return mark[0].CurrentData.Y;
                    }
                    else if ((y_difrent[i + 1] - 5) < mark[i + 1].CurrentData.Y - mark[i].CurrentData.Y || (y_difrent[i + 1] + 5) > mark[i + 1].CurrentData.Y - mark[i].CurrentData.Y)
                    {
                        suma_y += mark[i].CurrentData.Y;
                        ilosc++;
                    }
                    else
                        MessageBox.Show("yo"+y_difrent[i+1].ToString() + " != " + mark[i+1].CurrentData.Y.ToString() + " - " + mark[i].CurrentData.Y.ToString());
                }
                else
                {
                    //sprawdzam czy marker nie został zgubiony 
                    if ((y_difrent[i] - 5) < mark[i].CurrentData.Y - mark[i - 1].CurrentData.Y || (y_difrent[i] + 5) > mark[i].CurrentData.Y - mark[i - 1].CurrentData.Y)
                    {
                        suma_y += mark[i].CurrentData.Y;
                        if (mark[i].CurrentData.Y > 0)
                            ilosc++;
                    }
                    else
                        MessageBox.Show("yd"+y_difrent[i].ToString()+" != "+mark[i].CurrentData.Y.ToString()+" - "+mark[i - 1].CurrentData.Y.ToString());
                }
            }
            if (ilosc>0)
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
        private double Fps=0;
        /// <summary>
        /// Pobiera ilość klatek na sekunde z obecnej kamery
        /// </summary>
        public double fps
        {
            get { return Fps; }
        }
        private bool przechwyt = false;
        private int numer_aktywnej_kamery = 0;

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
                if (przechwyt)
                {
                    // przechwycenie klatki z kamery
                    Wspolny.w_touchlessMgr.Cameras[0].OnImageCaptured += new EventHandler<CameraEventArgs>(OnImageCaptured);
                    Wspolny.w_touchlessMgr.CurrentCamera = Wspolny.w_touchlessMgr.Cameras[0];
                    //rysowanie ostatniej klatki
                    Wspolny.w_picturebox.Paint += new PaintEventHandler(RysowanieOstatniegoObrazu);
                    czasOstatniejKlatki = DateTime.Now;
                    numer_aktywnej_kamery = 0;
                    przechwyt = true;
                }
                else
                {
                    try
                    {
                        Wspolny.w_touchlessMgr.Cameras[0].OnImageCaptured -= new EventHandler<CameraEventArgs>(OnImageCaptured);
                        Wspolny.w_picturebox.Paint -= new PaintEventHandler(RysowanieOstatniegoObrazu);
                        // przechwycenie klatki z kamery
                        Wspolny.w_touchlessMgr.Cameras[0].OnImageCaptured += new EventHandler<CameraEventArgs>(OnImageCaptured);
                        Wspolny.w_touchlessMgr.CurrentCamera = Wspolny.w_touchlessMgr.Cameras[0];
                        //rysowanie ostatniej klatki
                        Wspolny.w_picturebox.Paint += new PaintEventHandler(RysowanieOstatniegoObrazu);
                        czasOstatniejKlatki = DateTime.Now;
                        numer_aktywnej_kamery = 0;
                    }
                    catch
                    {
                        MessageBox.Show("Błąd usuniecia kamery"); 
                    }
                }
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
                if (przechwyt)
                {
                    // przechwycenie klatki z kamery
                    Wspolny.w_touchlessMgr.Cameras[numer_kamery].OnImageCaptured += new EventHandler<CameraEventArgs>(OnImageCaptured);
                    Wspolny.w_touchlessMgr.CurrentCamera = Wspolny.w_touchlessMgr.Cameras[numer_kamery];
                    //rysowanie ostatniej klatki
                    Wspolny.w_picturebox.Paint += new PaintEventHandler(RysowanieOstatniegoObrazu);
                    czasOstatniejKlatki = DateTime.Now;
                    this.numer_aktywnej_kamery = numer_kamery;
                    przechwyt = true;
                }
                else
                {
                    try
                    {
                        Wspolny.w_touchlessMgr.Cameras[this.numer_aktywnej_kamery].OnImageCaptured -= new EventHandler<CameraEventArgs>(OnImageCaptured);
                        Wspolny.w_picturebox.Paint -= new PaintEventHandler(RysowanieOstatniegoObrazu);
                        // przechwycenie klatki z kamery
                        Wspolny.w_touchlessMgr.Cameras[numer_kamery].OnImageCaptured += new EventHandler<CameraEventArgs>(OnImageCaptured);
                        Wspolny.w_touchlessMgr.CurrentCamera = Wspolny.w_touchlessMgr.Cameras[numer_kamery];
                        //rysowanie ostatniej klatki
                        Wspolny.w_picturebox.Paint += new PaintEventHandler(RysowanieOstatniegoObrazu);
                        czasOstatniejKlatki = DateTime.Now;
                        this.numer_aktywnej_kamery = numer_kamery;
                    }
                    catch
                    {
                        MessageBox.Show("Błąd usuniecia kamery");
                    }
                }
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
            if (milliseconds >= 1000)
            {
                Fps= Math.Round((licznikKlatek * 1000.0 / milliseconds),2);
                licznikKlatek = 0;
                czasOstatniejKlatki = DateTime.Now;
            }
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
    

}
