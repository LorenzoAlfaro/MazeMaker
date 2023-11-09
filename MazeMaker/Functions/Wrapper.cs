using System;
using System.IO;

namespace MazeMaker
{
    public static class WrapperMpq
    {                        
        public static int mOpenMPQ(string FileName)
        {
            int DefaultMaxFiles = 0;
            int DefaultBlockSize = 0;

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
        
        public static bool ExportFile(string MPQPath, string extractedFile, string saveFilePath)
        {
            int fNum;
            int result;
            int hMPQ = 0;

            if(!SFmpq.SFileOpenArchive(MPQPath, 0, 0, ref hMPQ))
            {
                return false;
            }

            sGetFile(hMPQ, extractedFile, saveFilePath, 0);

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
            int hMPQ = mOpenMPQ(MPQPath);

            int dwFlags = SFmpq.MAFA_REPLACE_EXISTING;

            mAddAutoFile(hMPQ, FilePath, @"staredit\scenario.chk", dwFlags);

            SFmpq.SFileCloseArchive(hMPQ);
        }

        public static void mAddAutoFile(int hMPQ, string File, string MPQPath, int dwFlags)
        {            
            SFmpq.MpqAddWaveToArchive(hMPQ, File, MPQPath, dwFlags | SFmpq.MAFA_COMPRESS, 0);
        }        
    }
}
