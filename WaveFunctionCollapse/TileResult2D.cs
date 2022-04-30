﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public class TileResult2D<T>
    {
        public T selectedTile;
        public int rotation;

        internal int tileIndex = -1;
        internal int entropy = int.MaxValue;
        internal List<int> possibleTiles = new List<int>();

        public TileResult2D(T selectedTile, int rotation)
        {
            this.selectedTile = selectedTile;
            this.rotation = rotation;
        }
    }
}
