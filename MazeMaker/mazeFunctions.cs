using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeMaker
{
    public struct Location
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
    public static class mazeFunctions
    {        
        public static Task<bool[,]> startMazeAsync (bool[,] maze, List<int[]> startWorm, int size, Random random, ref int blocksFilled, bool regionOrCorridors)
        {
            return Task.FromResult(startMaze(maze, startWorm, size, random, ref blocksFilled, regionOrCorridors));
        }

        public static bool[,] startMaze(bool[,] maze, List<int[]> startWorm, int size, Random random, ref int blocksFilled, bool regionOrCorridors)
        {
            int[] startPoint= new int[2] { random.Next(64), random.Next(64) };
            if (startWorm.Count >0)
            {
                //startWorm[random.Next(startWorm.Count)].CopyTo(startPoint,0);
            }
                         
            maze[startPoint[0], startPoint[1]] = true;
            startWorm.Add(startPoint);
            
            int loops = 0;

            while (startWorm.Count < size & loops < 8000)
            {                                
                int[] newTile = nextTile(startPoint, maze, startWorm, random, regionOrCorridors);
                
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
                maze = startMaze(maze, startWorm, size, random, ref blocksFilled, regionOrCorridors);
            }
            return maze;            
        }

        public static int[] nextTile(int[] currentTile, bool[,] maze, List<int[]> startWorm, Random random, bool regionOrCorridors)
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

            if (tempTile[0] < 0 | tempTile[0] > 63 | tempTile[1] < 0 | tempTile[1] > 63)
            {
                currentTile.CopyTo(tempTile, 0);
                return tempTile;
            }

            if (!maze[tempTile[0], tempTile[1]])//is it already been change?
            {
                if (upDown)
                {
                    if (checkLeftRight(tempTile, maze, regionOrCorridors))
                    {
                        maze[tempTile[0], tempTile[1]] = true;
                        return tempTile;
                    }
                }
                else
                {
                    if (checkUpDown(tempTile, maze, regionOrCorridors))
                    { 
                        maze[tempTile[0], tempTile[1]] = true;
                        return tempTile;
                    }
                }                                
            }
            currentTile.CopyTo(tempTile, 0);
            return tempTile;            
        }


        public static bool checkLeftRight(int[] tile, bool[,] maze, bool regionsOrCorridors)
        {
            bool leftEmpty = false;
            bool rightEmpty = false;

            if (tile[1] + 1 <64)
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
        public static bool checkUpDown(int[] tile, bool[,] maze, bool regionsOrCorridors)
        {
            bool downEmpty = false;
            bool upEmpty = false;

            if (tile[0] + 1 < 64)
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
        public static string mazeToString(bool[,] maze)
        {
            string mazeStr = "";
            for (int i = 0; i < 64; i++)
            {
                for (int j = 0; j < 64; j++)
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
    }
}
