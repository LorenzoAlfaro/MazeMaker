using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Text;

namespace MazeMaker
{
    public partial class Form1 : Form
    {
        public Timer LoadingTimer = new Timer();//Timer to give feedback to the user when something takes a long time to run
        public bool waiting = false; //when something is waiting an async response use this        
        Random random = new Random();                                
        private ByteViewer byteviewer;        
        int blocksFilled = 0;
        
        public Form1()
        {
            LoadingTimer.Tick += new EventHandler(TimerEventProcessor);            
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
            Random random = new Random();
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;            
            string map="";                        
            bool[,] maze = new bool[64, 64];            
            Button myButton = (Button)sender;
            string label = myButton.Text;
            try
            {
                readyToWait(myButton);                
                map = mazeFunctions.mazeToString(await Task.Run(()=> mazeFunctions.startMazeAsync(maze, new List<int[]>(), 2000, random, ref blocksFilled,checkBox1.Checked)));
                bool success = ByteOperations.ByteArrayToFile(ofd.FileName, ByteOperations.StringToByteArray(map),
                ByteOperations.findOffset(new byte[] { 0x4d, 0x54, 0x58, 0x4d }, ofd.FileName));//MTXM broodwar reads, 0x04A2
                success = ByteOperations.ByteArrayToFile(ofd.FileName, ByteOperations.StringToByteArray(map),
                    ByteOperations.findOffset(new byte[] { 0x54, 0x49, 0x4c, 0x45 }, ofd.FileName));//TILE staredit 0x24AA
            }
            catch (Exception err)
            {
                Console.WriteLine("failed creating map " + err.Message);                
            }
            finally
            {
                doneWaiting(myButton, label);
            }                                                                        
        }                                                                        
        private void button7_Click(object sender, EventArgs e)//update location
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            byte[] LogoDataBy = Encoding.ASCII.GetBytes(textBox1.Text);//converts the input to ASCII MRGN
            try
            {
                using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.ReadWrite))
                {                    
                    long offset2  = ByteOperations.findPattern(LogoDataBy, fs) + 4;                    
                    
                    
                    for (int i = 0; i < 4; i++)
                    {
                        Convert.ToByte(fs.ReadByte());
                    }
                   
                    
                    Location myLocation = StarcraftObj.readLocation(fs);
                    myLocation.topY = 0;
                    myLocation.bottY = 32;//bottom is bigger
                    StarcraftObj.updateLocation(myLocation, fs);
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);                
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.ReadWrite))
            {
                textBox2.Text ="0x" + Convert.ToString((ByteOperations.findPattern(Encoding.ASCII.GetBytes(textBox1.Text), fs)),16).ToUpper();
                byte[] size = new byte[] { 0, 0, 0, 0 };
                for (int i = 0; i < 4; i++)
                {
                    size[i] = Convert.ToByte(fs.ReadByte());
                }
                int j = BitConverter.ToInt32(size, 0);
                textBox3.Text = j.ToString();
            }
                
        }
    }
}
