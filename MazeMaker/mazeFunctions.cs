using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeMaker
{
    public static class mazeFunctions
    {        
        public static Task<bool[,]> startMazeAsync (bool[,] maze, List<int[]> startWorm, int size, Random random, ref int blocksFilled)
        {
            return Task.FromResult(startMaze(maze, startWorm, size, random, ref blocksFilled));
        }

        public static bool[,] startMaze(bool[,] maze, List<int[]> startWorm, int size, Random random, ref int blocksFilled)
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
            while (startWorm.Count < size & loops < 4000)
            {
                blocksFilled = startWorm.Count;
                int[] newTile = nextTile(startPoint, maze, startWorm, random);

                if (newTile[0] != startPoint[0] | newTile[1] != startPoint[1])
                {
                    if (newTile[0] == 1 | newTile[0] == 63)
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

            if (startWorm.Count < size)
            {
                maze = startMaze(maze, startWorm, size, random, ref blocksFilled);
            }


            return maze;

            //Console.WriteLine(maze);
        }

        public static int[] nextTile(int[] currentTile, bool[,] maze, List<int[]> startWorm, Random random)
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


        public static bool checkLeftRight(int[] tile, bool[,] maze)
        {
            try
            {
                if (!maze[tile[0], tile[1] + 1] & !maze[tile[0], tile[1] - 1])
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
        public static bool checkUpDown(int[] tile, bool[,] maze)
        {
            try
            {
                if (!maze[tile[0] + 1, tile[1]] & !maze[tile[0], tile[1] - 1])
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

        public static string mazeToString(bool[,] maze)
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
    }
}
