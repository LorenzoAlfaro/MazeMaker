﻿using System;
using System.IO;
using System.Linq;


namespace MazeMaker
{
    public static class BO
    {
        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static long findOffset(byte[] matchBytes, string path)
        {            
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {                
                return findPattern(matchBytes, fs) + 8;//byte[] matchBytes = { 0x4d, 0x54, 0x58, 0x4d };//MTXM
            }
        }

        public static long findPattern(byte[] pattern, FileStream fs)
        {
            long offset = -1;
            int byteRead = 1;
            while (byteRead != -1)
            {
                byteRead = fs.ReadByte();
                if (byteRead == pattern[0])
                {
                    bool completeMatch = true;
                    for (int i = 1; i < pattern.Count(); i++)
                    {
                        byteRead = fs.ReadByte();
                        if (byteRead != pattern[i])
                        {
                            completeMatch = false;
                            break;
                            //keep going
                        }
                    }
                    if (completeMatch)
                    {
                        // This returns the start position of the size.
                        // NOT the start position of the string pattern.
                        return fs.Position - pattern.Count();
                    }
                }
            }
            return offset;
        }
        
        public static bool ByteArrayToFile(string path, byte[] byteArray, long offset)
        {
            try
            {
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
                {
                    //byte[] matchBytes = { };
                    //long offset2 = findPattern(matchBytes, fs,0);
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

        public static int readInt32(FileStream fs)
        {            
            byte[] size = new byte[] { 0, 0, 0, 0 };
            for (int i = 0; i < 4; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }
            return BitConverter.ToInt32(size, 0);
        }

        public static short readInt16(FileStream fs)
        {
            byte[] size = new byte[] { 0, 0};
            for (int i = 0; i < 2; i++)
            {
                size[i] = Convert.ToByte(fs.ReadByte());
            }
            return BitConverter.ToInt16(size, 0);
        }
    }
}
