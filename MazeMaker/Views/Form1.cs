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
        Controller Controller = new Controller();

        Model model = Model.Instance();

        // Provide feedback to the user when waiting
        public Timer LoadingTimer = new Timer();

        // When something is waiting an async response use this
        public bool waiting = false; 
        
        Random random = new Random();
        
        private ByteViewer byteviewer;
                                      
        public Form1()
        {
            LoadingTimer.Tick += new EventHandler(TimerEventProcessor);
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
                label1.Text = model.blocksFilled.ToString();
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
        
        private async void button7_Click(object sender, EventArgs e)
        {

            Button myButton = (Button)sender;
            string label = myButton.Text;
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            readyToWait(myButton);
            try
            {
                await Controller.CreateNewMap(ofd.FileName);
            }
            catch (Exception err)
            {
                throw err;
            }
            finally
            {
                doneWaiting(myButton, label);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;
            using (var fs = new FileStream(ofd.FileName, FileMode.Open, FileAccess.ReadWrite))
            {
                textBox2.Text = "0x" + Convert.ToString((BO.findPattern(Encoding.ASCII.GetBytes(textBox1.Text), fs)), 16).ToUpper();
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
                int count = BO.readInt32(fs) / 36;

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
