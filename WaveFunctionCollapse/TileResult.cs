using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public class TileResult<T>
    {
        public T tileSrc;
        public int rotation;
        public int tileIndex = -1;
        public int entropy = int.MaxValue;
        public List<int> possibleTiles = new List<int>();

        public TileResult(T tileSrc, int rotation)
        {
            this.tileSrc = tileSrc;
            this.rotation = rotation;
        }
    }
}
