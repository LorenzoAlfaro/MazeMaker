using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MazeMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //MazeMaker.Classes.SFmpq.AboutSFMpq();

            //string version = Classes.SFmpq.MpqGetVersionString();

            int hMPQ = 0;
            bool success = Classes.SFmpq.SFileOpenArchive(@"C:\Users\Lorenzo\Dropbox\VIDEO GAMES\SC MAPS\Maze.scm", 0, 0, ref hMPQ);

            bool close = Classes.SFmpq.SFileCloseArchive(hMPQ);

        }
    }
}
