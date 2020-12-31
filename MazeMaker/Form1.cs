using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using VB6BackendDLL;
using System.ComponentModel.Design;
using VB6Wrapper;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace MazeMaker
{
    public partial class Form1 : Form
    {
        public Timer LoadingTimer = new Timer();//Timer to give feedback to the user when something takes a long time to run
        public bool waiting = false; //when something is waiting an async response use this

        int hMPQ = 0; //handle to MPQ
        Random random = new Random();
        Wrapper wrapperClass;

        [DllImport("Called.dll", CharSet = CharSet.Unicode) ]
        public static extern string FunctionCalled([MarshalAs(UnmanagedType.BStr)]  string input); //LPWstr return first three letters 
        //AnsBStr gets "敨汬o"
        //LPTStr gets "hel"
        //BStr gets "hello"
        //TBStr gets "hello"
        //AnsiBStr gets "hello"

        [DllImport("Called.dll", CharSet = CharSet.Unicode)]
        public static extern string FunctionCalled2([MarshalAs(UnmanagedType.AnsiBStr)] string input);
       
        string mazePath = Environment.ExpandEnvironmentVariables(@"C:\Users\%USERNAME%\Dropbox\VIDEO GAMES\SC MAPS\MPQExport\Maze.scm");

        string extractPath = Environment.ExpandEnvironmentVariables(@"C:\Users\%USERNAME%\Dropbox\VIDEO GAMES\SC MAPS\MPQExport");

        private ByteViewer byteviewer;

        static string HG = "4400";//Highground
        static string LG = "2600";//low ground
        static string BR = "6603";//wall
        int blocksFilled = 0;

        string[] values = new string[] { HG, LG,BR };
        public Form1()
        {
            LoadingTimer.Tick += new EventHandler(TimerEventProcessor);
            wrapperClass = new Wrapper();
            InitializeComponent();
            loadByteViewer();
        }
        private void loadByteViewer()
        {
            byteviewer = new ByteViewer();
            byteviewer.Location = new Point(8, 46);
            byteviewer.Size = new Size(600, 338);
            byteviewer.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            byteviewer.SetBytes(new byte[] { });
            this.Controls.Add(byteviewer);
        }
        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            Timer myTimer = (Timer)myObject;
            if (waiting)
            {

                myTimer.Stop();

                label1.Text = blocksFilled.ToString(); 

                if (label9.Text == "Loading.")
                {
                    label9.Text = "Loading..";
                }
                else if (label9.Text == "Loading..")
                {
                    label9.Text = "Loading...";
                }
                else
                {
                    label9.Text = "Loading.";
                }

                myTimer.Interval = 500;
                myTimer.Start();

            }
            else
            {
                myTimer.Stop();
                label9.Text = "Done";

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string hello = FunctionCalled2("hello");                                                 
            //MazeMaker.Classes.SFmpq.AboutSFMpq();
            //string version = Classes.SFmpq.MpqGetVersionString();
            string value = "dfdf";            
            
            dynamic files = wrapperClass.ListFiles(mazePath);//.Item(1);
            foreach (var item in files)
            {
                listBox1.Items.Add(item);
            }            
                                                
            //bool success = Classes.SFmpq.SFileOpenArchive(mazePath, 0, 0, ref hMPQ);
            //List<FILELISTENTRY> ListedFiles = new List<FILELISTENTRY>();    
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //bool success = wrapperClass.ExportFile(mazePath, files.Item(2), extractPath);
            bool success = wrapperClass.ExportFile(mazePath, listBox1.SelectedItem.ToString(), extractPath);
            
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            bool close = Classes.SFmpq.SFileCloseArchive(hMPQ);
        }

        private void button3_Click(object sender, EventArgs e)//Delete file from MPQ
        {
           // bool close = Classes.SFmpq.SFileCloseArchive(hMPQ);
            //MOAU_CREATE_NEW

            
            //hMPQ = Classes.SFmpq.MpqOpenArchiveForUpdateEx(mazePath, Classes.SFmpq.MOAU_OPEN_EXISTING | Classes.SFmpq.MOAU_MAINTAIN_LISTFILE, 1024, 3);
            //Classes.SFmpq.SFileSetLocale(0);//List.ListItems.Item(fNum).ListSubItems(4).Tag = 0
            bool success= wrapperClass.DeleteFile(mazePath, listBox1.SelectedItem.ToString());

            
           //= Classes.SFmpq.MpqDeleteFile(hMPQ, listBox1.SelectedItem.ToString());

            //int value =  Classes.SFmpq.MpqCloseUpdatedArchive(hMPQ,0);
        }

        private void button4_Click(object sender, EventArgs e)//import
        {                        
            wrapperClass.ImportFile(mazePath,"");
        }

        private void button5_Click(object sender, EventArgs e)//load byte viewer
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            byteviewer.SetFile(ofd.FileName);
        }

        private async void button6_Click(object sender, EventArgs e)//create file
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            //string hexString = "44004400440044004400440044004400";
            string hexString = "66036603660366036603260066036603";
            string hexStringEnd = "44004400440044004400440026006603";

           

            string row = hexString + hexString + hexString + hexString + hexString + hexString + hexString + hexStringEnd;

            string map="";
            System.Random random = new System.Random();
            for (int i = 0; i < 4096; i++)
            {
                map += values[random.Next(3)];
            }
            bool[,] maze = new bool[64, 64];
            List<int[]> startWorm = new List<int[]>();

            Button myButton = (Button)sender;
            string label = myButton.Text;
            try
            {
                readyToWait(myButton);
                
                map = mazeFunctions.mazeToString(await Task.Run(()=> mazeFunctions.startMazeAsync(maze, startWorm, 2000, random, ref blocksFilled,checkBox1.Checked)));
            }
            catch (Exception err)
            {
                Console.WriteLine("failed creating map");
                //throw;
            }
            finally
            {
                doneWaiting(myButton, label);
            }

            bool success = ByteArrayToFile(ofd.FileName, StringToByteArray(map), 0x04A2);//MTXM broodwar reads
             success = ByteArrayToFile(ofd.FileName, StringToByteArray(map), 0x24AA);//TILE staredit
        }
                        
        public bool ByteArrayToFile(string fileName, byte[] byteArray, long offset)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Write))
                {
                    fs.Seek(offset, SeekOrigin.Begin);// 0x04A2
                    fs.Write(byteArray, 0, byteArray.Length); //04A1 = 1185
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }
        
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private string createMap()
        {
            string HG = "6603";
            string LG = "4400";
            string W = "2600";
            string[] tiles = new string[] { HG, LG, W };
            string result = "";
            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    result += tiles[random.Next(3)];
                }
            }
            return result;
        }

        public void readyToWait(Button myButton)
        {
            waiting = true;
            this.Cursor = Cursors.WaitCursor;
            myButton.Text = "Loading";
            myButton.Enabled = false;

            LoadingTimer.Interval = 500;
            LoadingTimer.Start();
        }

        public void doneWaiting(Button button, string previousBttnLabel)
        {
            waiting = false;
            this.Cursor = Cursors.Default;
            button.Text = previousBttnLabel;
            button.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
