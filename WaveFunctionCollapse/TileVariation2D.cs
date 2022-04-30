using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    internal struct TileVariation2D<TConnector> where TConnector : IConnector
    {
        public int tileIndex;
        public int rotation;
        public double probability;
        public TConnector[] connectors;

        public TileVariation2D(int tileIndex, int rotation, double probability, TConnector[] connectors)
        {
            this.tileIndex = tileIndex;
            this.rotation = rotation;
            this.probability = probability;
            this.connectors = connectors;
        }
    }
}
