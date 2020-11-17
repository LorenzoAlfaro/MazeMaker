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

namespace MazeMaker
{
    public partial class Form1 : Form
    {
        int hMPQ = 0; //handle to MPQ
        Wrapper wrapperClass;

        [DllImport("Called.dll", CharSet = CharSet.Unicode) ]
        public static extern string FunctionCalled(
            [MarshalAs(UnmanagedType.BStr)]  string input); //LPWstr return first three letters 
        //AnsBStr gets "敨汬o"
        //LPTStr gets "hel"
        //BStr gets "hello"
        //TBStr gets "hello"
        //AnsiBStr gets "hello"

        [DllImport("Called.dll", CharSet = CharSet.Unicode)]
        public static extern string FunctionCalled2(
           [MarshalAs(UnmanagedType.AnsiBStr)] string input);


       

        string mazePath = Environment.ExpandEnvironmentVariables(@"C:\Users\%USERNAME%\Dropbox\VIDEO GAMES\SC MAPS\MPQExport\Maze.scm");

        string extractPath = Environment.ExpandEnvironmentVariables(@"C:\Users\%USERNAME%\Dropbox\VIDEO GAMES\SC MAPS\MPQExport");

        private ByteViewer byteviewer;
        public Form1()
        {           
            
            wrapperClass = new Wrapper();
            InitializeComponent();

            byteviewer = new ByteViewer();
            byteviewer.Location = new Point(8, 46);
            byteviewer.Size = new Size(600, 338);
            byteviewer.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            byteviewer.SetBytes(new byte[] { });
            this.Controls.Add(byteviewer);
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

        private void button3_Click(object sender, EventArgs e)
        {
           // bool close = Classes.SFmpq.SFileCloseArchive(hMPQ);
            //MOAU_CREATE_NEW

            
            //hMPQ = Classes.SFmpq.MpqOpenArchiveForUpdateEx(mazePath, Classes.SFmpq.MOAU_OPEN_EXISTING | Classes.SFmpq.MOAU_MAINTAIN_LISTFILE, 1024, 3);
            //Classes.SFmpq.SFileSetLocale(0);//List.ListItems.Item(fNum).ListSubItems(4).Tag = 0
            bool success= wrapperClass.DeleteFile(mazePath, listBox1.SelectedItem.ToString());

            
           //= Classes.SFmpq.MpqDeleteFile(hMPQ, listBox1.SelectedItem.ToString());

            //int value =  Classes.SFmpq.MpqCloseUpdatedArchive(hMPQ,0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //import
            
            wrapperClass.ImportFile(mazePath,"");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            byteviewer.SetFile(ofd.FileName);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            //string hexString = "44004400440044004400440044004400";
            string hexString = "66036603660366036603660366036603";
            string hexStringEnd = "44004400440044004400440026006603";

            string row = hexString + hexString + hexString + hexString + hexString + hexString + hexString + hexStringEnd;

            bool success = ByteArrayToFile(ofd.FileName, StringToByteArray(row), 0x04A2);//MTXM broodwar reads
             success = ByteArrayToFile(ofd.FileName, StringToByteArray(row), 0x24AA);//TILE staredit


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
    }
}
