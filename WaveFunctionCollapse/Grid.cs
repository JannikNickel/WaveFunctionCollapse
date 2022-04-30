using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public class Grid<T>
    {
        private readonly int width;
        private readonly int height;
        private T[,] grid;

        public int Width => width;
        public int Height => height;

        public T this[int x, int y]
        {
            get => grid[x, y];
            set => grid[x, y] = value;
        }

        public Grid(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.grid = new T[width, height];
        }
    }
}
