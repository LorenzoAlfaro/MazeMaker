using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeMaker
{
    public class Location
    {
        public int leftX;//u32
        public int topY;//u32
        public int rightX;//u32
        public int bottY;
        public Int16 stringNumber;//u16
        public Int16 flags;//u16     
        public long offSet;
        //4+4+4+4+2+2=20
    }

    public class Unit
    {
        public int instanceNum;
        public Int16 x;//center of the unit
        public Int16 y;
        public Int16 ID;
        public Int16 AddonNydus;
        public Int16 Properties;
        public Int16 mapProperties;
        public Byte PlayerNumber;
        public Byte HP;
        public Byte Shield;
        public Byte Energy;
        public int ResourceAmount;
        public Int16 unitHangar;
        public Int16 unitFlags;
        public int unused;
        public int AddonNydusLink;
        public long offSet;       
    }
    public static class mazeFunctions
    {        
        public static Task<bool[,]> startMazeAsync (bool[,] maze, List<int[]> startWorm, int size, Random random, ref int blocksFilled, bool regionOrCorridors, int width, int height)
        {
            return Task.FromResult(startMaze(maze, startWorm, size, random, ref blocksFilled, regionOrCorridors,width,height));
        }
        public static bool[,] startMaze(bool[,] maze, List<int[]> startWorm, int size, Random random, ref int blocksFilled, bool regionOrCorridors, int width,int height)
        {
            int[] startPoint;
            if (startWorm.Count!=0)
            {
                startPoint = startWorm[random.Next(startWorm.Count)];
            }
            else
            {
                startPoint = new int[2] { random.Next(height), random.Next(width) };
                startWorm.Add(startPoint);
            }

            
            if (startWorm.Count >0)
            {
                //startWorm[random.Next(startWorm.Count)].CopyTo(startPoint,0);
            }
                         
            maze[startPoint[0], startPoint[1]] = true;
            
            
            int loops = 0;

            while (startWorm.Count < size & loops < 8000)
            {                                
                int[] newTile = nextTile(startPoint, maze, startWorm, random, regionOrCorridors,width,height);
                
                if (!startWorm.Exists(x => x[0] == newTile[0] & x[1] == newTile[1]))
                {
                    if (newTile[0] != startPoint[0] ^ newTile[1] != startPoint[1]) //XOR
                    {
                        newTile.CopyTo(startPoint, 0);                        
                        startWorm.Add(newTile);
                    }
                }
                blocksFilled = startWorm.Count;//use to report to the outside world my progress
                loops += 1;
            }

            if (startWorm.Count < size)
            {
                maze = startMaze(maze, startWorm, size, random, ref blocksFilled, regionOrCorridors, width,height);
            }
            return maze;            
        }
        public static int[] nextTile(int[] currentTile, bool[,] maze, List<int[]> startWorm, Random random, bool regionOrCorridors, int width, int height)
        {            
            int[] tempTile = new int[2];
            currentTile.CopyTo(tempTile, 0);                        
            
            int direction = random.Next(4);
            bool upDown = false;
            if (direction == 0) //up
            {
                upDown = true;
                tempTile[0] = currentTile[0] - 1;                
            }
            else if (direction == 1) //down
            {
                upDown = true;
                tempTile[0] = currentTile[0] + 1;                
            }
            else if (direction == 2) //left
            {
                tempTile[1] = currentTile[1] - 1;                
            }
            else if (direction == 3) //right
            {
                tempTile[1] = currentTile[1] + 1;                
            }
            //check boundaries

            if (tempTile[0] < 0 | tempTile[0] > (height-1) | tempTile[1] < 0 | tempTile[1] > (width - 1))
            {
                currentTile.CopyTo(tempTile, 0);
                return tempTile;
            }

            if (!maze[tempTile[0], tempTile[1]])//is it already been change?
            {
                if (upDown)
                {
                    if (checkLeftRight(tempTile, maze, regionOrCorridors, width))
                    {
                        maze[tempTile[0], tempTile[1]] = true;
                        return tempTile;
                    }
                }
                else
                {
                    if (checkUpDown(tempTile, maze, regionOrCorridors, height))
                    { 
                        maze[tempTile[0], tempTile[1]] = true;
                        return tempTile;
                    }
                }                                
            }
            currentTile.CopyTo(tempTile, 0);
            return tempTile;            
        }
        public static bool checkLeftRight(int[] tile, bool[,] maze, bool regionsOrCorridors, int width)
        {
            bool leftEmpty = false;
            bool rightEmpty = false;

            if (tile[1] + 1 < width)
            {
                rightEmpty = !maze[tile[0], tile[1] + 1];
            }

            if (tile[1] - 1 > 0)
            {
                leftEmpty = !maze[tile[0], tile[1] - 1];
            }

            if (regionsOrCorridors)
            {
                return rightEmpty | leftEmpty;
            }
            return rightEmpty & leftEmpty;
                        
        }
        public static bool checkUpDown(int[] tile, bool[,] maze, bool regionsOrCorridors, int height)
        {
            bool downEmpty = false;
            bool upEmpty = false;

            if (tile[0] + 1 < height)
            {
                downEmpty = !maze[tile[0] + 1, tile[1]];
            }
            if (tile[0] - 1 > 0)
            {
                upEmpty = !maze[tile[0] -1, tile[1]];
            }
            if (regionsOrCorridors)
            {
                return downEmpty | upEmpty;   // Or makes it very spaced out, and makes it more checkboard pattern         
            }
            return downEmpty & upEmpty;   // Or makes it very spaced out, and makes it more checkboard pattern         
        }
        //public static int[] nextTileOld(int[] currentTile, bool[,] maze, List<int[]> startWorm, Random random)
        //{
        //    int[] nextTile = new int[2];
        //    currentTile.CopyTo(nextTile, 0);

        //    int[] tempTile = new int[2];
        //    nextTile.CopyTo(tempTile, 0);

        //    int direction = random.Next(4);

        //    if (direction == 0) //up
        //    {
        //        tempTile[0] = nextTile[0] - 1;

        //        if (tempTile[0] < 0)
        //        {
        //            return currentTile;
        //        }

        //        if (!maze[tempTile[0], tempTile[1]])
        //        {
        //            if (checkLeftRight(tempTile, maze))
        //            {
        //                maze[tempTile[0], tempTile[1]] = true;
        //                return tempTile;
        //            }
        //            else
        //            {
        //                tempTile = startWorm[random.Next(startWorm.Count)];
        //                return tempTile;
        //            }
        //        }
        //    }
        //    else if (direction == 1) //down
        //    {
        //        tempTile[0] = nextTile[0] + 1;

        //        if (tempTile[0] > 63)
        //        {
        //            return currentTile;
        //        }

        //        if (!maze[tempTile[0], tempTile[1]])
        //        {
        //            if (checkLeftRight(tempTile, maze))
        //            {
        //                maze[tempTile[0], tempTile[1]] = true;
        //                return tempTile;
        //            }
        //            else
        //            {
        //                tempTile = startWorm[random.Next(startWorm.Count)];
        //                return tempTile;
        //            }

        //        }
        //    }
        //    else if (direction == 2) //left
        //    {
        //        tempTile[1] = nextTile[1] - 1;

        //        if (tempTile[1] < 0)
        //        {
        //            return currentTile;
        //        }

        //        if (!maze[tempTile[0], tempTile[1]])
        //        {
        //            if (checkUpDown(tempTile, maze))
        //            {
        //                maze[tempTile[0], tempTile[1]] = true;
        //                return tempTile;
        //            }
        //            else
        //            {
        //                tempTile = startWorm[random.Next(startWorm.Count)];
        //                return tempTile;
        //            }
        //        }
        //    }
        //    else if (direction == 3) //right
        //    {
        //        tempTile[1] = nextTile[1] + 1;

        //        if (tempTile[1] > 63)
        //        {
        //            return currentTile;
        //        }

        //        if (!maze[tempTile[0], tempTile[1]])
        //        {
        //            if (checkUpDown(tempTile, maze))
        //            {
        //                maze[tempTile[0], tempTile[1]] = true;
        //                return tempTile;
        //            }
        //            else
        //            {
        //                tempTile = startWorm[random.Next(startWorm.Count)];
        //                return tempTile;
        //            }
        //        }
        //    }
        //    tempTile = startWorm[random.Next(startWorm.Count)];

        //    return tempTile;
        //}
        public static string mazeToString(bool[,] maze, int width, int height)
        {
            string mazeStr = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (maze[i, j])
                    {
                        mazeStr += "2600"; //low ground
                    }
                    else
                    {
                        
                        mazeStr += "6603";
                    }
                }
            }
            return mazeStr;
        }
        public static string createMap(Random random)
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
        public static int mapWidth(FileStream fs)
        {
            fs.Position = 0;
            long offSet = BO.findPattern(Encoding.ASCII.GetBytes("DIM "),fs);
            int size = BO.readInt32(fs);            
            return BO.readInt16(fs);
        }
        public static int mapHeight(FileStream fs)
        {
            fs.Position = 0;
            long offSet = BO.findPattern(Encoding.ASCII.GetBytes("DIM "), fs);
            int size = BO.readInt32(fs);
            BO.readInt16(fs);//            
            return BO.readInt16(fs);
        }

        public static void updateUnits(FileStream fs, List<int[]> openTiles, Random random)
        {
            fs.Position = 0;
            BO.findPattern(Encoding.ASCII.GetBytes("UNIT"), fs);
            int count = BO.readInt32(fs) / 36;
            List<Unit> Units = new List<Unit>();
            for (int i = 0; i < count; i++)
            {
                Unit thisUnit = StarcraftObj.readUnit(fs);
                Units.Add(thisUnit);
            }
            for (short i = 0; i < count; i++)
            {
                int[] position = openTiles[random.Next(openTiles.Count)];

                Units[i].x = (short)(16 + position[1] * 32);
                Units[i].y = (short)(16 + position[0] * 32);
                //Console.WriteLine(Units[i].x.ToString());
            }
            for (int i = 0; i < count; i++)
            {
                StarcraftObj.updateUnit(Units[i], fs);
            }
        }        
        public static void updateLocations(FileStream fs, List<int[]>openTiles,Random random)
        {
            fs.Position = 0;
            BO.findPattern(Encoding.ASCII.GetBytes("MRGN"), fs);
            fs.Position = fs.Position + 4;//skip the section size 4 bytes

            List<Location> myLocations = new List<Location>();
            for (int i = 0; i < 255; i++)
            {
                Location myLocation = StarcraftObj.readLocation(fs);
                if (myLocation.stringNumber > 1 & i != 63)
                {
                    myLocations.Add(myLocation);
                }
            }

            foreach (Location loc in myLocations)
            {
                int[] position = openTiles[random.Next(openTiles.Count)];
                loc.leftX = (short)(position[1] * 32);
                loc.rightX = loc.leftX + 32;
                loc.topY = (short)(position[0] * 32); ;
                loc.bottY = loc.topY + 32;
            }
            foreach (Location loc in myLocations)
            {
                StarcraftObj.updateLocation(loc, fs);
            }
        }
        
    }
}
