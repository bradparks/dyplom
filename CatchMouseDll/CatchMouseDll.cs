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
        public TouchlessMgr touchlessMgr = new TouchlessMgr();
        private System.Windows.Forms.PictureBox picturebox;
        public Aktywizacja_Obrazu a_obrazu;
        private bool flaga_obraz_przechwycony = false;


        public CatchMouseDll(System.Windows.Forms.PictureBox pictureb)
        {
            touchlessMgr = new TouchlessMgr();
            picturebox = pictureb; 
        }
        /// <summary>
        /// przechwytuje obraz z aktywnej kamery
        /// </summary>
        /// <returns>zwraca obraz PictureBoxa</returns>
        public System.Windows.Forms.PictureBox Przechwyt_Obrazu(){
            a_obrazu = new Aktywizacja_Obrazu(picturebox,touchlessMgr);
            picturebox = a_obrazu.Malowanie_Kamery();
            touchlessMgr = a_obrazu.touchlessMgr;
            flaga_obraz_przechwycony = true;
            return picturebox;
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
                a_obrazu = new Aktywizacja_Obrazu(picturebox, touchlessMgr);
                picturebox = a_obrazu.Malowanie_Kamery();
                touchlessMgr = a_obrazu.touchlessMgr;
                flaga_obraz_przechwycony = true;
            }
            else
            {
                picturebox = a_obrazu.Zmiana_Numeru_Kamery(numer_kamery);
                touchlessMgr = a_obrazu.touchlessMgr;
            }
            return picturebox;
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
            return picturebox;
        }
    }
   
    public class marker_na_monitor
    {
    }
    public class symulator_myszki
    {
    }
}
