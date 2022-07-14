using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public class BacktrackingStep<T>
    {
        private Grid<TileResult2D<T>> grid;
        private List<(int x, int y)> lowestEntropyTiles;

        public Grid<TileResult2D<T>> Grid => grid;
        public List<(int x, int y)> LowestEntropyTiles => lowestEntropyTiles;

        public BacktrackingStep(Grid<TileResult2D<T>> grid, List<(int x, int y)> lowestEntropyTiles)
        {
            this.grid = grid;
            this.lowestEntropyTiles = lowestEntropyTiles;
        }
    }
}
