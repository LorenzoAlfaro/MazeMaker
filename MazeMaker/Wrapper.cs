using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VB6Wrapper;

namespace MazeMaker
{
    class WrapperMpq
    {
        int hMPQ = 0; //handle to MPQ
        string mazePath = Environment.ExpandEnvironmentVariables(@"C:\Users\%USERNAME%\Dropbox\VIDEO GAMES\SC MAPS\MPQExport\Maze.scm");
        string extractPath = Environment.ExpandEnvironmentVariables(@"C:\Users\%USERNAME%\Dropbox\VIDEO GAMES\SC MAPS\MPQExport");
        WrapperMpq()
        {
            wrapperClass = new Wrapper();
        }

        Wrapper wrapperClass;

        [DllImport("Called.dll", CharSet = CharSet.Unicode)]
        public static extern string FunctionCalled([MarshalAs(UnmanagedType.BStr)] string input); //LPWstr return first three letters 
        //AnsBStr gets "敨汬o"
        //LPTStr gets "hel"
        //BStr gets "hello"
        //TBStr gets "hello"
        //AnsiBStr gets "hello"

        [DllImport("Called.dll", CharSet = CharSet.Unicode)]
        public static extern string FunctionCalled2([MarshalAs(UnmanagedType.AnsiBStr)] string input);

        public bool delete()
        {
            // bool close = Classes.SFmpq.SFileCloseArchive(hMPQ);
            //MOAU_CREATE_NEW


            //hMPQ = Classes.SFmpq.MpqOpenArchiveForUpdateEx(mazePath, Classes.SFmpq.MOAU_OPEN_EXISTING | Classes.SFmpq.MOAU_MAINTAIN_LISTFILE, 1024, 3);
            //Classes.SFmpq.SFileSetLocale(0);//List.ListItems.Item(fNum).ListSubItems(4).Tag = 0
            //bool success = wrapperClass.DeleteFile(mazePath, listBox1.SelectedItem.ToString());

            //= Classes.SFmpq.MpqDeleteFile(hMPQ, listBox1.SelectedItem.ToString());

            //int value =  Classes.SFmpq.MpqCloseUpdatedArchive(hMPQ,0)


            //wrapperClass.ImportFile(mazePath, "");
            //bool success = wrapperClass.ExportFile(mazePath, files.Item(2), extractPath);
            //bool success = wrapperClass.ExportFile(mazePath, listBox1.SelectedItem.ToString(), extractPath);


            //string hello = FunctionCalled2("hello");                                                 
            //MazeMaker.Classes.SFmpq.AboutSFMpq();
            //string version = Classes.SFmpq.MpqGetVersionString();
            //string value = "dfdf";

            //dynamic files = wrapperClass.ListFiles(mazePath);//.Item(1);
            //foreach (var item in files)
            //{
            //    listBox1.Items.Add(item);
            //}
            //bool success = Classes.SFmpq.SFileOpenArchive(mazePath, 0, 0, ref hMPQ);
            //List<FILELISTENTRY> ListedFiles = new List<FILELISTENTRY>();    


            //bool close = Classes.SFmpq.SFileCloseArchive(hMPQ); //call when finish the file

            return false;
        }
    }
}
