using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public class StringTile2D<T>
    {
        public T tileSrc;
        public string[] sides;
        public int[] rotations;

        public string Top => sides[0];
        public string Right => sides[1];
        public string Bottom => sides[2];
        public string Left => sides[3];

        public StringTile2D(T tileSrc, string[] sides, int[] rotations)
        {
            this.tileSrc = tileSrc;
            this.sides = sides;
            this.rotations = rotations;
        }
    }
}
