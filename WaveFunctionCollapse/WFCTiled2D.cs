using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public class WFCTiled2D<T>
    {
        private static readonly (int, int)[] neighbourDirs = new (int, int)[4]
        {
            (0, 1),//top
            (1, 0),//right
            (0, -1),//bottom
            (-1, 0)//left
        };
        private static readonly int[] inverseNeighbourDir = new int[4]
        {
            2, 3, 0, 1
        };

        private int width;
        private int height;
        private StringTile2D<T>[] tiles;
        private Grid<TileResult<T>> currentGrid;
        private Random random;

        public WFCTiled2D(int width, int height, StringTile2D<T>[] tiles)
        {
            this.width = width;
            this.height = height;
            this.random = new Random();

            //Create tiles for all possible rotations of the source tiles
            List<StringTile2D<T>> list = new List<StringTile2D<T>>();
            foreach(StringTile2D<T> tile in tiles)
            {
                for(int i = 0;i < tile.rotations.Length;i++)
                {
                    if(tile.rotations[i] % 90 != 0)
                    {
                        throw new ArgumentException("Tile rotations have to be a multiple of 90!");
                    }

                    string[] sides = (string[])tile.sides.Clone();
                    RotateArray(ref sides, tile.rotations[i]);
                    list.Add(new StringTile2D<T>(tile.tileSrc, sides, new int[] { tile.rotations[i] }));
                }
            }
            this.tiles = list.ToArray();

            //Create initial result array
            currentGrid = new Grid<TileResult<T>>(width, height);
            for(int i = 0;i < width;i++)
            {
                for(int k = 0;k < height;k++)
                {
                    currentGrid[i, k] = new TileResult<T>(default!, 0)
                    {
                        tileIndex = -1,
                        entropy = int.MaxValue,
                        possibleTiles = Enumerable.Range(0, this.tiles.Length).ToList()//All tiles are possible at the beginning
                    };
                }
            }
        }

        private void RotateArray(ref string[] array, int rotation)
        {
            int steps = rotation / 90;
            for(int i = 0;i < steps;i++)
            {
                string last = array.Last();
                for(int k = array.Length - 1;k >= 1;k--)
                {
                    array[k] = array[k - 1];
                }
                array[0] = last;
            }
        }

        public bool Iterate(out Grid<TileResult<T>> currentGrid)
        {
            currentGrid = this.currentGrid;

            //Update entropy
            int lowestEntropy = int.MaxValue;
            (int x, int y) lowestIndex = default;
            for(int i = 0;i < width;i++)
            {
                for(int k = 0;k < height;k++)
                {
                    if(currentGrid[i, k].tileIndex != -1)
                    {
                        //Already set
                        continue;
                    }
                    int entropy = CalcEntropy(currentGrid, i, k, ref currentGrid[i, k].possibleTiles);
                    currentGrid[i, k].entropy = entropy;
                    if(entropy < lowestEntropy)
                    {
                        lowestEntropy = entropy;
                        lowestIndex = (i, k);
                    }
                }
            }

            //No non set tile left or stuck
            if(lowestEntropy == int.MaxValue || lowestEntropy == 0)
            {
                return false;
            }

            //Select random tile for lowest entropy tile
            int x = lowestIndex.x;
            int y = lowestIndex.y;
            int selectedTile = currentGrid[x, y].possibleTiles[new Random().Next(0, currentGrid[x, y].possibleTiles.Count)];
            currentGrid[x, y].tileIndex = selectedTile;
            currentGrid[x, y].tileSrc = this.tiles[selectedTile].tileSrc;
            currentGrid[x, y].rotation = this.tiles[selectedTile].rotations[0];
            currentGrid[x, y].entropy = 0;

            return true;
        }

        private int CalcEntropy(Grid<TileResult<T>> grid, int i, int k, ref List<int> possibleTiles)
        {
            for(int l = 0;l < neighbourDirs.Length;l++)
            {
                (int xOff, int yOff) = neighbourDirs[l];
                int x = i + xOff;
                int y = k + yOff;
                if(x < 0 || x >= width || y < 0 || y >= height || grid[x, y].tileIndex == -1)
                {
                    continue;
                }

                StringTile2D<T> neighbour = this.tiles[grid[x, y].tileIndex];
                string neighbourConnector = neighbour.sides[inverseNeighbourDir[l]];
                for(int j = possibleTiles.Count - 1;j >= 0;j--)
                {
                    if(this.tiles[possibleTiles[j]].sides[l] != neighbourConnector)//TODO this should probably be a more complex comparison for the case of longer strings
                    {
                        //Cant connect
                        possibleTiles.RemoveAt(j);
                    }
                }
            }
            return possibleTiles.Count;
        }
    }
}
