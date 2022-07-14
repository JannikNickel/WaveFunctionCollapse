using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public class Grid<T> where T : ICloneable
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

        public IEnumerable<T> Flatten()
        {
            for(int i = 0;i < width;i++)
            {
                for(int k = 0;k < height;k++)
                {
                    yield return grid[i, k];
                }
            }
        }

        public Grid<T> Clone()
        {
            Grid<T> clone = new Grid<T>(Width, Height);
            for(int i = 0;i < width;i++)
            {
                for(int k = 0;k < height;k++)
                {
                    clone[i, k] = (T)grid[i, k].Clone();
                }
            }
            return clone;
        }
    }
}
