﻿using System;
using System.Collections.Generic;

namespace MazeMaker
{
    class Model
    {
        private Model() { }

        private static Model _instance;

        private static readonly object _lock = new object();

        public static  Model Instance()
        {
            if(_instance==null)
            {
                lock (_lock)
                {
                    _instance = new Model();
                }
            }

            return _instance;
        }

        public string MapPath = "";

        public string Map = "";

        public List<int[]> openTiles = new List<int[]>();

        public bool Checked = false;

        public int blocksFilled = 0;

        public string NewMapPath = @"C:\Users\%USERNAME%\Documents\StarCraft\Maps\Mazes";


    }
}
