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
    /// klasa odpowiedzialna za przechwytywanie obrazu z kamery do PictureBoxa
    /// </summary>
    public class Aktywizacja_Obrazu
    {
        public TouchlessMgr touchlessMgr;
        public Image ostatnia_klatka; //_latestFrame
        private System.Windows.Forms.PictureBox picturebox = new System.Windows.Forms.PictureBox();
        public bool pauza=false;
        private DateTime czasOstatniejKlatki;
        private int licznikKlatek=0;

        Marker Mark;
        private static Point srodek_zaznaczenia_markera;  //_markerCenter
        private static float promien_zaznaczenia_markera; //_markerRadius
        public bool flaga_dodanie_markera = false;

        public delegate void Zmiana_Ilosci_Markerow(object sender, EventArgs e);
        public event Zmiana_Ilosci_Markerow MarkerChanged;

        /// <summary>
        /// konstruktor 
        /// </summary>
        /// <param name="picture">pobiera obraz do przypisania kamery</param>
        /// <param name="touchlessM">glowna klasa biblioteki Touchless</param>
        public Aktywizacja_Obrazu(System.Windows.Forms.PictureBox picture, TouchlessMgr touchlessM)
        {
            touchlessMgr = touchlessM;
            picturebox = new System.Windows.Forms.PictureBox();
            picturebox = picture;
        }

        /// <summary>
        /// Zwrot obrazu z kamery
        /// </summary>
        /// <returns>zwraca PictureBoxa z obrazem kamery</returns>
        public System.Windows.Forms.PictureBox Malowanie_Kamery()
        {
            if (touchlessMgr.Cameras.Count > 0)
            {
                // przechwycenie klatki z kamery
                touchlessMgr.Cameras[0].OnImageCaptured += new EventHandler<CameraEventArgs>(OnImageCaptured);
                touchlessMgr.CurrentCamera = touchlessMgr.Cameras[0];
                //rysowanie ostatniej klatki
                picturebox.Paint += new PaintEventHandler(RysowanieOstatniegoObrazu);
                czasOstatniejKlatki = DateTime.Now;
            }
            else MessageBox.Show("Fatal error: Kamera nie jest podlaczona do komputera");   
            return picturebox;
        }

        /// <summary>
        /// Zwrot obrazu z kamery
        /// </summary>
        /// <param name="numer_kamery">Z której kamery ma zostac zwrocony obraz, bez parametru 0-wy index kamery</param>
        /// <returns>zwraca PictureBoxa z obrazem kamery</returns>
        public System.Windows.Forms.PictureBox Malowanie_Kamery(int numer_kamery)
        {
            if (touchlessMgr.Cameras.Count > numer_kamery)
            {
                // przechwycenie klatki z kamery
                touchlessMgr.Cameras[numer_kamery].OnImageCaptured += new EventHandler<CameraEventArgs>(OnImageCaptured);
                touchlessMgr.CurrentCamera = touchlessMgr.Cameras[numer_kamery];
                //rysowanie ostatniej klatki
                picturebox.Paint += new PaintEventHandler(RysowanieOstatniegoObrazu);
                czasOstatniejKlatki = DateTime.Now;
            }
            else MessageBox.Show("Fatal error: Podany numer kamery jest za duzy");           
            return picturebox;
        }

        /// <summary>
        /// Zmiana numeru kamery do przechwytywania obrazu
        /// </summary>
        /// <param name="numer_kamery">Z której kamery ma zostac zwrocony obraz</param>
        /// <returns></returns>
        public System.Windows.Forms.PictureBox Zmiana_Numeru_Kamery(int numer_kamery)
        {
            if (touchlessMgr.Cameras.Count > numer_kamery)
            {
                if (touchlessMgr.CurrentCamera != null)
                {
                    touchlessMgr.CurrentCamera.OnImageCaptured -= new EventHandler<CameraEventArgs>(OnImageCaptured);
                    touchlessMgr.CurrentCamera.Dispose();
                    touchlessMgr.CurrentCamera = null;
                    picturebox.Paint -= new PaintEventHandler(RysowanieOstatniegoObrazu);
                }
            
                // przechwycenie klatki z kamery
                touchlessMgr.Cameras[numer_kamery].OnImageCaptured += new EventHandler<CameraEventArgs>(OnImageCaptured);
                touchlessMgr.CurrentCamera = touchlessMgr.Cameras[numer_kamery];
                //rysowanie ostatniej klatki
                picturebox.Paint += new PaintEventHandler(RysowanieOstatniegoObrazu);
                czasOstatniejKlatki = DateTime.Now;
            }
            else MessageBox.Show("Fatal error: Podany numer kamery jest za duzy"); 
            return picturebox;
        }

        /// <summary>
        /// rysowanie obrazu z kamery
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">zdarzenie nadejścia nowej klatki</param>
        private void RysowanieOstatniegoObrazu(object sender, PaintEventArgs e)
        {
            if (ostatnia_klatka != null)
            {
                // Rysuje ostatnią klatke z aktywnej kamery
                e.Graphics.DrawImage(ostatnia_klatka, 0, 0, picturebox.Width, picturebox.Height);
                if (flaga_dodanie_markera)
                {
                    Pen pen = new Pen(Brushes.Red, 1);
                    e.Graphics.DrawEllipse(pen, srodek_zaznaczenia_markera.X - promien_zaznaczenia_markera, srodek_zaznaczenia_markera.Y - promien_zaznaczenia_markera,
                        2 * promien_zaznaczenia_markera, 2 * promien_zaznaczenia_markera);
                    
                }
            }
        }

        /// <summary>
        /// Przechwycenie ostatniej klatki, odświerzenie pictureboxa
        /// </summary>
        private void OnImageCaptured(object sender, CameraEventArgs args)
        {
            // Przeliczanie FPS
            licznikKlatek++;
            double milliseconds = (DateTime.Now.Ticks - czasOstatniejKlatki.Ticks) / TimeSpan.TicksPerMillisecond;
            if (pauza != true)
            {
                ostatnia_klatka = args.Image;
                picturebox.Invalidate();
            }
        }
        /// <summary>
        /// dodaje marker 
        /// </summary>
        public void doadnie_markera()
        {
            flaga_dodanie_markera = true;
            pauza = true;
            
            picturebox.MouseDown += new MouseEventHandler(picturebox_MouseDown);
            picturebox.MouseMove += new MouseEventHandler(picturebox_MouseMove);
            picturebox.MouseUp += new MouseEventHandler(picturebox_MouseUp);

        }

        /// <summary>
        /// wcisniecie przycisku myszy - wyznaczenie srodka okregu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picturebox_MouseDown(object sender, MouseEventArgs e)
        {
            srodek_zaznaczenia_markera = e.Location;
            promien_zaznaczenia_markera = 0;
        }

        /// <summary>
        /// zwolnienie przycisku myszy - dodanie nowego markera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picturebox_MouseUp(object sender, MouseEventArgs e)
        {
            int dx = e.X - srodek_zaznaczenia_markera.X;
            int dy = e.Y - srodek_zaznaczenia_markera.Y;
            promien_zaznaczenia_markera = (float)Math.Sqrt(dx * dx + dy * dy);
            // Adjust for the image/display scaling (assumes proportional scaling)
            srodek_zaznaczenia_markera.X = (srodek_zaznaczenia_markera.X * ostatnia_klatka.Width) / picturebox.Width;
            srodek_zaznaczenia_markera.Y = (srodek_zaznaczenia_markera.Y * ostatnia_klatka.Height) / picturebox.Height;
            promien_zaznaczenia_markera = (promien_zaznaczenia_markera * ostatnia_klatka.Height) / picturebox.Height;
            // Add the marker
            Mark = touchlessMgr.AddMarker("Marker #" + (touchlessMgr.MarkerCount+1).ToString(), (Bitmap)ostatnia_klatka, srodek_zaznaczenia_markera, promien_zaznaczenia_markera);

            // Restore the app to its normal state and clear the selection area adorment
            srodek_zaznaczenia_markera = new Point();
            picturebox.Image = new Bitmap(picturebox.Width, picturebox.Height);

            picturebox.MouseDown -= new MouseEventHandler(picturebox_MouseDown);
            picturebox.MouseMove -= new MouseEventHandler(picturebox_MouseMove);
            picturebox.MouseUp -= new MouseEventHandler(picturebox_MouseUp);
            /*
            if (_markerSelected != null)
            {
                // labelMarkerData.Text = _markerSelected.ToString();
                _markerSelected.OnChange -= new EventHandler<MarkerEventArgs>(OnSelectedMouseUpdate);
            }
            //  else
            {
                labelMarkerData.Text = "Marker pobrał za małą gamme kolorów \nProsze sprubować ponownie dodać marker";
                _markerSelected = (Marker)Mark;
                _markerSelected.OnChange += new EventHandler<MarkerEventArgs>(OnSelectedMouseUpdate);
                numericUpDownMarkerThresh.Value = _markerSelected.Threshold;
                checkBoxMarkerHighlight.Checked = Mark.Highlight;
                checkBoxMarkerSmoothing.Checked = Mark.SmoothingEnabled;
            } */

            flaga_dodanie_markera = false;
            pauza = false;
            MarkerChanged.Invoke(this, e);
        }

        /// <summary>
        /// przemieszczenie myszy - wyznaczenie promienia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void picturebox_MouseMove(object sender, MouseEventArgs e)
        {
            // If the user is selecting a marker, draw a circle of their selection as a selection adornment
            if (!srodek_zaznaczenia_markera.IsEmpty)
            {
                // Get the current radius
                int dx = e.X - srodek_zaznaczenia_markera.X;
                int dy = e.Y - srodek_zaznaczenia_markera.Y;
                promien_zaznaczenia_markera = (float)Math.Sqrt(dx * dx + dy * dy);
                // Cause display update
                picturebox.Invalidate();
            }
        }

    }
}
