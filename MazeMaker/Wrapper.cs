using System;
using System.IO;
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

            int hMPQ = mOpenMPQ(MPQPath);
            SFmpq.SFileSetLocale(0);

            bool success = SFmpq.MpqDeleteFile(hMPQ, FileName);

            return SFmpq.MpqCloseUpdatedArchive(hMPQ, 0);

        }

        public static void ListFiles(string Path, int hMPQ)
        {
            SFmpq.SFileOpenArchive(Path, 0, 0, ref hMPQ);


        }

        public static bool ExportFile(string MPQPath, string extractedFile, string saveFilePath)
        {
            int fNum;
            int result;
            int hMPQ = 0;

            if(!SFmpq.SFileOpenArchive(MPQPath, 0, 0, ref hMPQ))
            {
                return false;
            }

            sGetFile(hMPQ, extractedFile, saveFilePath, 1);

            SFmpq.SFileCloseArchive(hMPQ);

            return true;
        }

        public static void sGetFile(int hMPQ, string FileName, string OutPath, int UseFullPath)
        {
            byte[] buffer = new byte[3];
            int hFile=0;
            long fLen=0;
            int cNum=0;
            dynamic overLapped = 0;

            if (SFmpq.SFileOpenFileEx(hMPQ, FileName, 0, ref hFile))
            {
                fLen = SFmpq.SFileGetFileSize(hFile, 0);

                if(fLen > 0)
                {
                   Array.Resize(ref buffer, (int)fLen);
                }
                else
                {
                    Array.Resize(ref buffer, 0);
                }

                SFmpq.SFileReadFile(hFile, buffer, (int)fLen, 0, ref overLapped);
                SFmpq.SFileCloseFile(hFile);

                if(UseFullPath == 0)
                {
                    FileName = Path.GetFileName(FileName);
                }

                FileName = $@"{OutPath}\{FileName}";


                File.WriteAllBytes(FileName, buffer);

            }
        }

        public static void ImportFile(string MPQPath, string FilePath)
        {
            int hMPQ = 0;

            hMPQ = mOpenMPQ(MPQPath);

            int dwFlags = SFmpq.MAFA_REPLACE_EXISTING;

            mAddAutoFile(hMPQ, FilePath, @"staredit\scenario.chk", dwFlags);

            SFmpq.MpqCloseUpdatedArchive(hMPQ, 0);

            SFmpq.SFileOpenArchive(MPQPath, 0, 0, ref hMPQ);

            SFmpq.SFileCloseArchive(hMPQ);
        }

        public static void mAddAutoFile(int hMPQ, string File, string MPQPath, int dwFlags)
        {
            //SFmpq.MpqAddFileToArchiveEx(hMPQ, File, MPQPath, dwFlags, 0, 0);

            SFmpq.MpqAddWaveToArchive(hMPQ, File, MPQPath, dwFlags | SFmpq.MAFA_COMPRESS, 0);

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
