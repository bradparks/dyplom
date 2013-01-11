using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace CatchMouseDll
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern void mouse_event
            (MouseEventType dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32")]
        public static extern int SetCursorPos(int x, int y);

       

        public Form1()
        {
            InitializeComponent();
        }

        public enum MouseEventType : int
        {
            LeftDown = 0x02,
            LeftUp = 0x04,
            RightDown = 0x08,
            RightUp = 0x10
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //SetCursorPos(myParent.stary_curent_x, myParent.stary_curent_y);
            //myParent.zamknij_okno_myszy();
            mouse_event(MouseEventType.LeftDown, Cursor.Position.X, Cursor.Position.Y, 0, 0);
            mouse_event(MouseEventType.LeftUp, Cursor.Position.X, Cursor.Position.Y, 0, 0);
            //myParent.CurrTime = DateTime.Now;
        }
    }
}
