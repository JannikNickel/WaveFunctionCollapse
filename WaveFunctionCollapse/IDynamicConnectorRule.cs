using System;
using System.Collections.Generic;

namespace WaveFunctionCollapse
{
    public interface IDynamicConnectorRule2D
    {
        public bool CanConnect(int tile1Index, (int x, int y) tile1Pos, int tile1ConnectorIndex, int tile2Index, (int x, int y) tile2Pos, int tile2ConnectorIndex);
    }
}
