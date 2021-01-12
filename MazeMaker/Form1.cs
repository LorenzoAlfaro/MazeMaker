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
using System.Text;

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

            long offset2 = 0;
            using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
            {
                byte[] matchBytes = { 0x4d, 0x54, 0x58, 0x4d };//MTXM
                offset2 = findPattern(matchBytes, fs)+8;

            }
            long offset3 = 0;
            using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
            {
                byte[] matchBytes = { 0x54, 0x49, 0x4c, 0x45 };
                offset3 = findPattern(matchBytes, fs) + 8;//TILE
            }

            //bool success = ByteArrayToFile(ofd.FileName, StringToByteArray(map), 0x04A2);//MTXM broodwar reads
            bool success = ByteArrayToFile(ofd.FileName, StringToByteArray(map), offset2);//MTXM broodwar reads
            success = ByteArrayToFile(ofd.FileName, StringToByteArray(map), offset3);//TILE staredit 0x24AA
        }
                        
        public bool ByteArrayToFile(string fileName, byte[] byteArray, long offset)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite))
                {
                    //byte[] matchBytes = { };
                    //long offset2 = findPattern(matchBytes, fs,0);
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

        public long findPattern(byte[] pattern, FileStream fs)
        {
            long offset = -1;

            int byteRead = 1;
            while (byteRead!=-1)
            {
                byteRead = fs.ReadByte();

                if (byteRead== pattern[0])
                {
                    bool completeMatch = true;
                    for (int i = 1; i < pattern.Count(); i++)
                    {

                        byteRead = fs.ReadByte();
                        if (byteRead != pattern[i])
                        {
                            completeMatch = false;
                            break;
                            //keep going
                        }
                    }
                    if (completeMatch)
                    {
                        return fs.Position-pattern.Count();//this returns the start position of the size. NOT the start position of the string pattern
                    }
                }                
            }

            return offset;
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

        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            byte[] LogoDataBy = ASCIIEncoding.ASCII.GetBytes(textBox1.Text);

            try
            {
                using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.ReadWrite))
                {
                    //byte[] matchBytes = {0x4d,0x54,0x58,0x4d };
                    byte[] matchBytes = LogoDataBy;

                    long offset2  = findPattern(matchBytes, fs) + 4;
                    textBox2.Text = offset2.ToString();

                    byte[] size = new byte[]{0,0,0,0};

                    for (int i = 0; i < 4; i++)
                    {
                        size[i] = Convert.ToByte(fs.ReadByte());
                    }

                    //if (BitConverter.IsLittleEndian)
                        //Array.Reverse(size);

                    int j = BitConverter.ToInt32(size, 0);

                    Location myLocation = readLocation(fs);

                    myLocation.topY = 0;
                    myLocation.bottY = 32;//bottom is bigger

                    updateLocation(myLocation, fs);



                    Console.WriteLine("int: {0}", j);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);                
            }

        }

        private Location readLocation(FileStream fs)
        {
            Location newLocation = new Location();

            newLocation.offSet = fs.Position;

            byte[] size = new byte[] { 0, 0, 0, 0 };
            for (int i = 0; i < 4; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }

            int leftX = BitConverter.ToInt32(size, 0);

            for (int i = 0; i < 4; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }

            int topY = BitConverter.ToInt32(size, 0);

            for (int i = 0; i < 4; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }

            int rightX = BitConverter.ToInt32(size, 0);

            for (int i = 0; i < 4; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }

            int bottY = BitConverter.ToInt32(size, 0);

            size = new byte[] { 0, 0};

            for (int i = 0; i < 2; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }

            short stringNumber = BitConverter.ToInt16(size, 0);

            for (int i = 0; i < 2; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }

            short flags = BitConverter.ToInt16(size, 0);

            

            newLocation.leftX = leftX;
            newLocation.topY = topY;
            newLocation.rightX = rightX;
            newLocation.bottY = bottY;
            newLocation.stringNumber = stringNumber;
            newLocation.flags = flags;



            return newLocation;
        }

        private void updateLocation(Location loc, FileStream fs)
        {
            fs.Position = loc.offSet;

            byte[] leftX = BitConverter.GetBytes(loc.leftX);
            byte[] topY = BitConverter.GetBytes(loc.topY);
            byte[] rightX = BitConverter.GetBytes(loc.rightX);
            byte[] bottY = BitConverter.GetBytes(loc.bottY);
            
            byte[] stringNumber = BitConverter.GetBytes(loc.stringNumber);

            byte[] flags = BitConverter.GetBytes(loc.flags);

            byte[] fullArray = leftX.Concat(topY.Concat(rightX.Concat(bottY.Concat(stringNumber.Concat(flags)))))
                .ToArray();

            foreach (byte myByte in fullArray)
            {
                fs.WriteByte(myByte);
            }
        }


    }
}
