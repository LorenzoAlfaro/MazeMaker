using System;
using System.Runtime.InteropServices;

namespace MazeMaker
{
    public static class WrapperMpq
    {        
        
        //string mazePath = Environment.ExpandEnvironmentVariables(@"C:\Users\%USERNAME%\Dropbox\VIDEO GAMES\SC MAPS\MPQExport\Maze.scm");
        //string extractPath = Environment.ExpandEnvironmentVariables(@"C:\Users\%USERNAME%\Dropbox\VIDEO GAMES\SC MAPS\MPQExport");
        

        [DllImport("Called.dll", CharSet = CharSet.Unicode)]
        public static extern string FunctionCalled([MarshalAs(UnmanagedType.BStr)] string input); //LPWstr return first three letters
        //AnsBStr gets "敨汬o"
        //LPTStr gets "hel"
        //BStr gets "hello"
        //TBStr gets "hello"
        //AnsiBStr gets "hello"

        [DllImport("Called.dll", CharSet = CharSet.Unicode)]
        public static extern string FunctionCalled2([MarshalAs(UnmanagedType.AnsiBStr)] string input);


        public static int mOpenMPQ(string FileName)
        {
            int DefaultMaxFiles = 1024;
            int DefaultBlockSize = 3;
            
            
            int hMPQ = SFmpq.MpqOpenArchiveForUpdateEx(FileName, SFmpq.MOAU_OPEN_EXISTING | SFmpq.MOAU_MAINTAIN_LISTFILE, DefaultMaxFiles, DefaultBlockSize);

            if (hMPQ == 0 || hMPQ == SFmpq.INVALID_HANDLE_VALUE)
            {
                hMPQ = SFmpq.MpqOpenArchiveForUpdateEx(FileName, SFmpq.MOAU_CREATE_NEW | SFmpq.MOAU_MAINTAIN_LISTFILE, DefaultMaxFiles, DefaultBlockSize);
            }

            if(hMPQ != 0 && hMPQ != SFmpq.INVALID_HANDLE_VALUE)
            {
                return hMPQ;
            }

            return 0;
        }

        public static int DeleteFile(string MPQPath, string FileName)
        {
            //bool close = SFmpq.SFileCloseArchive(hMPQ);            

            int hMPQ = mOpenMPQ(MPQPath);
            SFmpq.SFileSetLocale(0);

            bool success = SFmpq.MpqDeleteFile(hMPQ, FileName);

            return SFmpq.MpqCloseUpdatedArchive(hMPQ, 0);            
                                
        }

        public static void ListFiles(string Path, int hMPQ)
        {
            SFmpq.SFileOpenArchive(Path, 0, 0, ref hMPQ);


        }

        //List.ListItems.Item(fNum).ListSubItems(4).Tag = 0
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
    }
}
