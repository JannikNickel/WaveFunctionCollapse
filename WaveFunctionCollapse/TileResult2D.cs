using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public class TileResult2D<T> : ICloneable
    {
        public T selectedTile;
        public int rotation;

        internal int tileIndex = -1;
        public int entropy = int.MaxValue;
        internal List<int> possibleTiles = new List<int>();

        public TileResult2D(T selectedTile, int rotation)
        {
            this.selectedTile = selectedTile;
            this.rotation = rotation;
        }

        public void Reset(List<int> possibleTiles)
        {
            selectedTile = default!;
            rotation = 0;
            tileIndex = -1;
            entropy = int.MaxValue;
            this.possibleTiles = possibleTiles;
        }

        public object Clone()
        {
            return new TileResult2D<T>(selectedTile, rotation)
            {
                tileIndex = tileIndex,
                entropy = entropy,
                possibleTiles = possibleTiles.Select(n => n).ToList()
            };
        }
    }
}
