using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeMaker
{
    public sealed class Controller
    {
        Model Model = Model.Instance();

        Random random = new Random();

        //int blocksFilled = 0;

        public async void CreateNewMap(string sourceFile)
        {
            // Select a unmodified .scm file
            // copy the file
            // extract the .chk file of the clone
            // delete the .chk file of the clone
            // transfomr the .chk
            // import the file .chk to the cloned .scm,
            // test the map in staredit and starcraft

            Model.openTiles.Clear();

            string fileName = Path.GetFileNameWithoutExtension(sourceFile);

            string newPath = Path.GetDirectoryName(sourceFile);

            string ext = Path.GetExtension(sourceFile);

            string destFile = $@"{newPath}\{fileName}-cloned{ext}";

            File.Copy(sourceFile, destFile, true);

            bool success = WrapperMpq.ExportFile(destFile, @"staredit\scenario.chk", newPath);

            string chkPath = $@"{newPath}\staredit\scenario.chk";

            int height;
            int width;
            
            using (var fs = new FileStream(chkPath, FileMode.Open, FileAccess.ReadWrite))
            {
                width = mazeFunctions.mapWidth(fs);
                height = mazeFunctions.mapHeight(fs);
            }

            bool[,] maze = new bool[height, width];
            
            Model.Map = mazeFunctions.mazeToString(await Task.Run(() => mazeFunctions.startMazeAsync(
                maze, Model.openTiles, (width * height / 2), random, ref Model.blocksFilled, Model.Checked, width, height)), width, height);

            success = BO.ByteArrayToFile(chkPath, BO.StringToByteArray(Model.Map),
             BO.findOffset(new byte[] { 0x4d, 0x54, 0x58, 0x4d }, chkPath));//MTXM broodwar reads, 0x04A2
            success = BO.ByteArrayToFile(chkPath, BO.StringToByteArray(Model.Map),
             BO.findOffset(new byte[] { 0x54, 0x49, 0x4c, 0x45 }, chkPath));//TILE staredit 0x24AA

            using (var fs = new FileStream(chkPath, FileMode.Open, FileAccess.ReadWrite))
            {
                mazeFunctions.updateUnits(fs, Model.openTiles, random);
                mazeFunctions.updateLocations(fs, Model.openTiles, random);

            }

            int success2 = WrapperMpq.DeleteFile(destFile, @"staredit\scenario.chk");
            
            WrapperMpq.ImportFile(destFile, chkPath);

            string time_date = DateTime.Now.ToString("HH:mm:ss").Replace(":", "-");

            string destFilePath = Environment.ExpandEnvironmentVariables(
                $@"C:\Users\%USERNAME%\Documents\StarCraft\Maps\Download\Muestras\{fileName}-{time_date}{ext}");

            File.Move(destFile, destFilePath);
            //C:\Users\loren\Documents\StarCraft\Maps\Download\Muestras
        }


    }
}
