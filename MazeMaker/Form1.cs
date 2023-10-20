using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Text;

using VB6Wrapper;

namespace MazeMaker
{
    public partial class Form1 : Form
    {
        public Timer LoadingTimer = new Timer();//Timer to give feedback to the user when something takes a long time to run
        public bool waiting = false; //when something is waiting an async response use this
        Random random = new Random();
        private ByteViewer byteviewer;
        int blocksFilled = 0;
        Wrapper wrapperClass;
        string map = "";
        List<int[]> openTiles = new List<int[]>();
        int hMPQ = 0; //handle to MPQ

        public Form1()
        {
            LoadingTimer.Tick += new EventHandler(TimerEventProcessor);
            wrapperClass = new Wrapper();
            InitializeComponent();
            //loadByteViewer();
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
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            bool close = SFmpq.SFileCloseArchive(hMPQ);
        }
        private void Form1_Load(object sender, EventArgs e)
        {

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
            openTiles.Clear();

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;


            Button myButton = (Button)sender;
            string label = myButton.Text;

            try
            {
                readyToWait(myButton);
                int height;
                int width;
                using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.ReadWrite))
                {

                    width = mazeFunctions.mapWidth(fs);
                    height = mazeFunctions.mapHeight(fs);

                }
                bool[,] maze = new bool[height, width];
                map = mazeFunctions.mazeToString(await Task.Run(() => mazeFunctions.startMazeAsync(maze, openTiles, (width * height / 2), random, ref blocksFilled, checkBox1.Checked, width, height)), width, height);



                    bool success = BO.ByteArrayToFile(ofd.FileName, BO.StringToByteArray(map),
                     BO.findOffset(new byte[] { 0x4d, 0x54, 0x58, 0x4d }, ofd.FileName));//MTXM broodwar reads, 0x04A2
                    success = BO.ByteArrayToFile(ofd.FileName, BO.StringToByteArray(map),
                     BO.findOffset(new byte[] { 0x54, 0x49, 0x4c, 0x45 }, ofd.FileName));//TILE staredit 0x24AA


            }
            catch (Exception err)
            {
                throw err;
                //Console.WriteLine("failed creating map " + err.Message);
            }
            finally
            {
                doneWaiting(myButton, label);
            }

            using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.ReadWrite))
            {
                mazeFunctions.updateUnits(fs, openTiles, random);
                mazeFunctions.updateLocations(fs, openTiles, random);

            }
        }
        private async void button7_Click(object sender, EventArgs e)//start with a clean .scm file
        {
            //select a unmodified .scm file
            // copy the file
            // extract the .chk file of the clone
            // delete the .chk file of the clone
            // transfomr the .chk
            // import the file .chk to the cloned .scm,
            // test the map in staredit and starcraft
            openTiles.Clear();
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            string sourceFile = ofd.FileName;
            string fileName = Path.GetFileNameWithoutExtension(sourceFile);

            string newPath = Path.GetDirectoryName(sourceFile);

            string ext = Path.GetExtension(sourceFile);

            string destFile = newPath + "\\" + fileName + "-cloned" + ext;

            File.Copy(sourceFile, destFile, true);

            dynamic files = wrapperClass.ListFiles(destFile);//.Item(1);
            foreach (var item in files)
            {
                listBox1.Items.Add(item);
            }
            string extractPath = "";
            bool success = WrapperMpq.ExportFile(destFile, @"staredit\scenario.chk", newPath);


            string chkPath = newPath + @"\staredit\scenario.chk";

            Button myButton = (Button)sender;
            string label = myButton.Text;
            try
            {
                readyToWait(myButton);
                int height;
                int width;
                using (var fs = new FileStream(chkPath, FileMode.Open, FileAccess.ReadWrite))
                {

                    width = mazeFunctions.mapWidth(fs);
                    height = mazeFunctions.mapHeight(fs);

                }
                bool[,] maze = new bool[height, width];
                map = mazeFunctions.mazeToString(await Task.Run(() => mazeFunctions.startMazeAsync(maze, openTiles, (width * height / 2), random, ref blocksFilled, checkBox1.Checked, width, height)), width, height);



                success = BO.ByteArrayToFile(chkPath, BO.StringToByteArray(map),
                 BO.findOffset(new byte[] { 0x4d, 0x54, 0x58, 0x4d }, chkPath));//MTXM broodwar reads, 0x04A2
                success = BO.ByteArrayToFile(chkPath, BO.StringToByteArray(map),
                 BO.findOffset(new byte[] { 0x54, 0x49, 0x4c, 0x45 }, chkPath));//TILE staredit 0x24AA


            }
            catch (Exception err)
            {
                throw err;
                //Console.WriteLine("failed creating map " + err.Message);
            }
            finally
            {
                doneWaiting(myButton, label);
            }

            using (var fs = new FileStream(chkPath, FileMode.Open, FileAccess.ReadWrite))
            {
                mazeFunctions.updateUnits(fs, openTiles, random);
                mazeFunctions.updateLocations(fs, openTiles, random);

            }


            int success2 = WrapperMpq.DeleteFile(destFile, @"staredit\scenario.chk");

            //wrapperClass.ImportFile(destFile, chkPath);
            WrapperMpq.ImportFile(destFile, chkPath);

            string time_date = DateTime.Now.ToString("HH:mm:ss").Replace(":", "-");

            string destFilePath = Environment.ExpandEnvironmentVariables($@"C:\Users\%USERNAME%\Documents\StarCraft\Maps\Download\Muestras\{fileName}-{time_date}{ext}");

            File.Move(destFile, destFilePath);
            //C:\Users\loren\Documents\StarCraft\Maps\Download\Muestras
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.ReadWrite))
            {
                textBox2.Text ="0x" + Convert.ToString((BO.findPattern(Encoding.ASCII.GetBytes(textBox1.Text), fs)),16).ToUpper();
                //fs has its offSet moved to the end of the four bytes label, starting the other 4bytes of the section size, no need to set the offset
                int j = BO.readInt32(fs);
                textBox3.Text = j.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            List<Unit> Units = new List<Unit>();
            using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.ReadWrite))
            {

                BO.findPattern(Encoding.ASCII.GetBytes("UNIT"), fs);
                int count = BO.readInt32(fs)/36;

                for (int i = 0; i < count; i++)
                {
                    Unit thisUnit = StarcraftObj.readUnit(fs);
                    Units.Add(thisUnit);
                }


                for (short i = 0; i < count; i++)
                {
                    Units[i].x = (short)random.Next(2048);
                    Units[i].y = (short)random.Next(2048);
                    Console.WriteLine(Units[i].x.ToString());
                }


                for (int i = 0; i < count; i++)
                {
                    StarcraftObj.updateUnit(Units[i], fs);
                }
            }
        }
    }
}
