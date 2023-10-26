using System;
using System.IO;
using System.Threading.Tasks;

namespace MazeMaker
{
    public sealed class Controller
    {
        Model Model = Model.Instance();

        Random random = new Random();
        
        
        public async Task CreateNewMap(string selectedMapPath)
        {                       
            Model.openTiles.Clear();

            string time_date = DateTime.Now.ToString("HH:mm:ss").Replace(":", "-");

            const string internalCHKPath = @"staredit\scenario.chk";

            const string StarCraftMaps = @"C:\Users\%USERNAME%\Documents\StarCraft\Maps\Download\Muestras";

            string MapFolder = Path.GetDirectoryName(selectedMapPath);

            string MapName = Path.GetFileNameWithoutExtension(selectedMapPath);

            string Mapextension = Path.GetExtension(selectedMapPath);

            string CloneMapPath = $@"{MapFolder}\{MapName}-cloned{Mapextension}";

            string ExportedCHK = $@"{MapFolder}\{internalCHKPath}";

            string NewMapPath = Environment.ExpandEnvironmentVariables(
                $@"{StarCraftMaps}\{MapName}-{time_date}{Mapextension}");


            File.Copy(selectedMapPath, CloneMapPath, true);
            
            WrapperMpq.ExportFile(CloneMapPath, internalCHKPath, MapFolder);
                        
            await modifyMap(ExportedCHK);
                        
            WrapperMpq.DeleteFile(CloneMapPath, internalCHKPath);

            WrapperMpq.ImportFile(CloneMapPath, ExportedCHK);
                        
            File.Move(CloneMapPath, NewMapPath);            
        }

        public async Task modifyMap(string chkPath)
        {
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

            //MTXM broodwar reads, 0x04A2
            BO.ByteArrayToFile(chkPath, BO.StringToByteArray(Model.Map),
             BO.findOffset(new byte[] { 0x4d, 0x54, 0x58, 0x4d }, chkPath));
            //TILE staredit 0x24AA
            BO.ByteArrayToFile(chkPath, BO.StringToByteArray(Model.Map),
             BO.findOffset(new byte[] { 0x54, 0x49, 0x4c, 0x45 }, chkPath));

            using (var fs = new FileStream(chkPath, FileMode.Open, FileAccess.ReadWrite))
            {
                mazeFunctions.updateUnits(fs, Model.openTiles, random);
                mazeFunctions.updateLocations(fs, Model.openTiles, random);
            }
        }
    }
}
