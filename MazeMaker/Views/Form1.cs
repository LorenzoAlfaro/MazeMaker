using System;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;

namespace MazeMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            LoadingTimer.Tick += new EventHandler(TimerEventProcessor);

        }
        
        Controller Controller = new Controller();

        Model model = Model.Instance();

        // Provide feedback to the user when waiting
        public Timer LoadingTimer = new Timer();

        // When something is waiting an async response use this
        public bool waiting = false;

        private ByteViewer byteviewer;


        private void loadByteViewer()
        {
            byteviewer = new ByteViewer();
            byteviewer.Location = new Point(8, 46);
            byteviewer.Size = new Size(600, 338);
            byteviewer.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            byteviewer.SetBytes(new byte[] { });

            tabControl1.TabPages[2].Controls.Add(byteviewer);
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
            Cursor = Cursors.WaitCursor;
            myButton.Text = "Loading";
            myButton.Enabled = false;
            LoadingTimer.Interval = 500;
            LoadingTimer.Start();
        }

        public void doneWaiting(Button button, string previousBttnLabel)
        {
            waiting = false;
            Cursor = Cursors.Default;
            button.Text = previousBttnLabel;
            button.Enabled = true;
        }
        
        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CHK Files(*.CHK;)|*.CHK;";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            loadByteViewer();

            byteviewer.SetFile(ofd.FileName);
        }

        private async void button7_Click(object sender, EventArgs e)
        {

            Button myButton = (Button)sender;
            string label = myButton.Text;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Map Files(*.SCM;)|*.SCM;";
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

            ofd.Filter = "CHK Files(*.CHK;)|*.CHK;";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            Tuple<string, string>  t = Controller.SearchLabel(ofd.FileName, textBox1.Text);

            textBox2.Text = t.Item1;
            textBox3.Text = t.Item2;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CHK Files(*.CHK;)|*.CHK;";
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            Controller.FindUnits(ofd.FileName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() != DialogResult.OK)
                return;

            model.NewMapPath = fbd.SelectedPath;

            Form1_Load(sender, e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text += $" {version()}";
            textBox4.Text = model.NewMapPath;
        }

        private string version()
        {
            try
            {
                return System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch (Exception ex)
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
    }
}
