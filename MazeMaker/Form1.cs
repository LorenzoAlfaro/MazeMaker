﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Project1;
using VB6Wrapper;
using VB6BackendDLL;


namespace MazeMaker
{
    public partial class Form1 : Form
    {
        [DllImport("Called.dll", CharSet = CharSet.Unicode) ]
        public static extern string FunctionCalled(
            [MarshalAs(UnmanagedType.LPStr)]  string input);

        [DllImport("Called.dll", CharSet = CharSet.Unicode)]
        public static extern string FunctionCalled2(
           [MarshalAs(UnmanagedType.LPStr)] string input);


        [DllImport("Called.dll", CharSet = CharSet.Unicode)]
        public static extern string GetAString(int index);

        string mazePath = Environment.ExpandEnvironmentVariables(@"C:\Users\%USERNAME%\Dropbox\VIDEO GAMES\SC MAPS\Maze.scm");

        string extractPath = Environment.ExpandEnvironmentVariables(@"C:\Users\%USERNAME%\Dropbox\VIDEO GAMES\SC MAPS");
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BackendDLL whirlwind = new BackendDLL();

            bool sucesssss = whirlwind.updateServiceTotals(false, "6789", "2462", "string");
            
            
            //MazeMaker.Classes.SFmpq.AboutSFMpq();

            //string version = Classes.SFmpq.MpqGetVersionString();
            string value = "dfdf";
            Class1 vb6 = new Class1();


            Wrapper wrapperClass = new Wrapper();

            dynamic files = wrapperClass.ListFiles(mazePath);//.Item(1);

            bool success = wrapperClass.ExportFile(mazePath, files.Item(2), extractPath);

            value = vb6.ModuleFun("str");
            
            //value = GetAString( 0);

            int hMPQ = 0;


             success = Classes.SFmpq.SFileOpenArchive(mazePath, 0, 0, ref hMPQ);


            List<FILELISTENTRY> ListedFiles = new List<FILELISTENTRY>();

    

            bool close = Classes.SFmpq.SFileCloseArchive(hMPQ);

        }
    }
}