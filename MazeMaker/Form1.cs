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
        Random random = new Random();
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

        static string HG = "4400";//Highground
        static string LG = "2600";//low ground
        static string BR = "6603";//wall

        string[] values = new string[] { HG, LG,BR };
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
            map = mazeToString(startMaze(maze, startWorm,1000));
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

        public bool[,] startMaze(bool[,] maze, List<int[]> startWorm, int size)
        {
            string mazeStr = "";

            //bool[,] maze = new bool[64, 64];
            int[] startPoint = new int[2] { 0, 0 };
            int[] exit = new int[2] { 0, 0 };

            startPoint[0] = random.Next(64);
            startPoint[1] = random.Next(64);

            exit[0] = random.Next(64);
            exit[1] = random.Next(64);

            //List<int[]> startWorm = new List<int[]>();
            List<int[]> endWormWorm = new List<int[]>();

            int loops = 0;
            while (startWorm.Count < size & loops<4000)
            {
                int[] newTile = nextTile(startPoint, maze,startWorm);

                if (newTile[0] != startPoint[0] | newTile[1] != startPoint[1])
                {
                    if (newTile[0] ==1 | newTile[0] == 63)
                    {
                        newTile[0] = random.Next(63);
                    }                    
                    if (newTile[1] == 1 | newTile[1] == 63)
                    {
                        newTile[1] = random.Next(63);
                    }                    

                    if (!startWorm.Exists(x => x[0] == newTile[0] & x[1] == newTile[1]))
                    {
                        startPoint[0] = newTile[0];
                        startPoint[1] = newTile[1];
                        startWorm.Add(newTile);
                    }                    
                }
                loops += 1;
            }

            if (startWorm.Count< size)
            {
                maze = startMaze(maze, startWorm, size);
            }


            return maze;

            //Console.WriteLine(maze);
        }

        public string mazeToString(bool[,] maze)
        {
            string mazeStr = "";
            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    if (maze[i, j])
                    {
                        mazeStr += "6603";
                    }
                    else
                    {
                        mazeStr += "4400";
                    }
                }
            }
            return mazeStr;

        }

        public int[] nextTile(int[] currentTile, bool[,] maze, List<int[]> startWorm)
        {
            int[] nextTile = new int[2];
            currentTile.CopyTo(nextTile, 0);
            int[] tempTile = new int[2];
            nextTile.CopyTo(tempTile, 0);


            int direction = random.Next(4);



            if (direction == 0) //up
            {
                tempTile[0] = nextTile[0] - 1;

                if (tempTile[0] < 0)
                {
                    return currentTile;                                        
                }

                if (!maze[tempTile[0], tempTile[1]])
                {
                    if (checkLeftRight(tempTile, maze))
                    {
                        maze[tempTile[0], tempTile[1]] = true;
                        return tempTile;
                    }
                    else
                    {
                        tempTile = startWorm[random.Next(startWorm.Count)];
                        return tempTile;
                    }
                }
            }
            else if (direction == 1) //down
            {
                tempTile[0] = nextTile[0] + 1;

                if (tempTile[0] > 63)
                {                  
                    return currentTile;                    
                }

                if (!maze[tempTile[0], tempTile[1]])
                {
                    if (checkLeftRight(tempTile,maze))
                    {
                        maze[tempTile[0], tempTile[1]] = true;
                        return tempTile;
                    }
                    else
                    {
                        tempTile = startWorm[random.Next(startWorm.Count)];                        
                        return tempTile;
                    }
                    
                }
            }
            else if (direction == 2) //left
            {
                tempTile[1] = nextTile[1] - 1;

                if (tempTile[1] < 0)
                {                    
                    return currentTile;                                        
                }

                if (!maze[tempTile[0], tempTile[1]])
                {
                    if (checkUpDown(tempTile, maze))
                    {
                        maze[tempTile[0], tempTile[1]] = true;
                        return tempTile;
                    }
                    else
                    {
                        tempTile = startWorm[random.Next(startWorm.Count)];
                        return tempTile;
                    }
                }
            }
            else if (direction == 3) //right
            {
                tempTile[1] = nextTile[1] + 1;

                if (tempTile[1] > 63)
                {                    
                    return currentTile;                   
                }

                if (!maze[tempTile[0], tempTile[1]])
                {
                    if (checkUpDown(tempTile, maze))
                    {
                        maze[tempTile[0], tempTile[1]] = true;
                        return tempTile;
                    }
                    else
                    {
                        tempTile = startWorm[random.Next(startWorm.Count)];
                        return tempTile;
                    }
                }
            }
            tempTile = startWorm[random.Next(startWorm.Count)];
            //tempTile[0] = random.Next(64);
            //tempTile[1] = random.Next(64);
            return tempTile;            
        }

        bool checkLeftRight(int[] tile, bool[,] maze)
        {
            try
            {
                if(!maze[tile[0], tile[1] + 1] & !maze[tile[0], tile[1] - 1])
                {
                    return true;
                }
            }
            catch (Exception err)
            {
                return false;
                //throw;
            }
            

            return false;
        }
        bool checkUpDown(int[] tile, bool[,] maze)
        {
            try
            {
                if (!maze[tile[0]+1, tile[1]] & !maze[tile[0], tile[1]-1])
                {
                    return true;
                }
            }
            catch (Exception err)
            {
                return false;
                //throw;
            }
            return false;            
        }
    }
}
