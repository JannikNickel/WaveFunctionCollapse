using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public class Tile2D<T, TConnector> : ITile2D<T, TConnector> where TConnector : IConnector
    {
        public T Data { get; set; }
        public double Probability { get; set; }
        public TConnector[] Connectors { get; set; }
        public int[] Rotations { get; set; }

        public Tile2D(T data, double probability, TConnector top, TConnector right, TConnector bottom, TConnector left, int[] rotations)
        {
            Data = data;
            Probability = probability;
            Connectors = new TConnector[4] { top, right, bottom, left };
            Rotations = rotations.Length > 0 ? rotations : new int[] { 0 };
        }
    }
}
