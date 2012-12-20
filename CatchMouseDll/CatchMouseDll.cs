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
    public class CatchMouseDll
    {
        public TouchlessMgr touchlessMgr ;
        public Aktywizacja_Obrazu Aktywacja_Obrazu = new Aktywizacja_Obrazu();
        public Markery Markery = new Markery();
        public Symulator_Myszki Symulator_Myszki = new Symulator_Myszki();

        public CatchMouseDll(System.Windows.Forms.PictureBox pictureb)
        {
            Wspolny.w_touchlessMgr = new TouchlessMgr();
            Wspolny.w_picturebox = pictureb;
            touchlessMgr = Wspolny.w_touchlessMgr;
        }

        public CatchMouseDll(System.Windows.Forms.PictureBox pictureb, TouchlessMgr  touchlessMenager)
        {
            Wspolny.w_touchlessMgr = touchlessMenager;
            Wspolny.w_picturebox = pictureb;
            touchlessMgr = Wspolny.w_touchlessMgr;
        }
        /*
        /// <summary>
        /// przechwytuje obraz z aktywnej kamery
        /// </summary>
        /// <returns>zwraca obraz PictureBoxa</returns>
        public System.Windows.Forms.PictureBox Przechwyt_Obrazu(){
            a_obrazu = new Aktywizacja_Obrazu();
            Wspolny.w_picturebox = a_obrazu.Malowanie_Kamery();
            Wspolny.w_touchlessMgr = a_obrazu.touchlessMgr;
            flaga_obraz_przechwycony = true;
            return Wspolny.w_picturebox;
        }
        /// <summary>
        /// przechwytuje obraz z aktywnej kamery
        /// </summary>
        /// <param name="numer_kamery">obraz z której kamery ma zostac zwrocony, bez parametru 1 kamera</param>
        /// <returns>zwraca obraz PictureBoxa</returns>
        public System.Windows.Forms.PictureBox Przechwyt_Obrazu(int numer_kamery)
        {
            if (flaga_obraz_przechwycony == false)
            {
                a_obrazu = new Aktywizacja_Obrazu();
                Wspolny.w_picturebox = a_obrazu.Malowanie_Kamery();
                Wspolny.w_touchlessMgr = a_obrazu.touchlessMgr;
                flaga_obraz_przechwycony = true;
            }
            else
            {
                Wspolny.w_picturebox = a_obrazu.Zmiana_Numeru_Kamery(numer_kamery);
                Wspolny.w_touchlessMgr = a_obrazu.touchlessMgr;
            }
            return Wspolny.w_picturebox;
        }
        public bool Pauza
        {
            get
            {
                return a_obrazu.pauza;
            }
            set
            {
                a_obrazu.pauza = value;
            }
        }

        public System.Windows.Forms.PictureBox dodaj_marker()
        {
            a_obrazu.doadnie_markera();
            return Wspolny.w_picturebox;
        }*/
    }
   
    public class marker_na_monitor
    {
    }
    public class symulator_myszki
    {
    }
}
