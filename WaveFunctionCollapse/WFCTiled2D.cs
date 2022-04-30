using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public class WFCTiled2D<T, TTile, TConnector> where T : ITile2D<TTile, TConnector> where TConnector : IConnector
    {
        private static readonly (int, int)[] neighbourDirs = new (int, int)[4]
        {
            (0, 1),//top
            (1, 0),//right
            (0, -1),//bottom
            (-1, 0)//left
        };

        private int width;
        private int height;
        private bool useTileProbabilities;
        private T[] tiles;
        private TileVariation2D<TConnector>[] tileVariations;
        private Grid<TileResult2D<T>> currentGrid;
        private Random random;

        public WFCTiled2D(int width, int height, T[] tiles, bool useTileProbabilities = false, int seed = 0)
        {
            this.width = width;
            this.height = height;
            this.useTileProbabilities = useTileProbabilities;
            this.random = seed != default ? new Random(seed) : new Random();
            this.tiles = tiles;

            //Create tiles for all possible rotations of the source tiles
            List<TileVariation2D<TConnector>> tileVariations = new List<TileVariation2D<TConnector>>();
            for(int i = 0;i < tiles.Length;i++)
            {
                T tile = tiles[i];
                for(int k = 0;k < tile.Rotations.Length;k++)
                {
                    if(tile.Rotations[k] % 90 != 0)
                    {
                        throw new ArgumentException("Tile rotations have to be a multiple of 90!");
                    }

                    TConnector[] connectors = (TConnector[])tile.Connectors.Clone();
                    RotateArray(ref connectors, tile.Rotations[k]);
                    double prob = tile.Probability / tile.Rotations.Length;//Divide by tile variation count to make sure that all tiles have the same base probability
                    tileVariations.Add(new TileVariation2D<TConnector>(i, tile.Rotations[k], prob, connectors));
                }
            }
            this.tileVariations = tileVariations.ToArray();

            //Create initial result array
            currentGrid = new Grid<TileResult2D<T>>(width, height);
            for(int i = 0;i < width;i++)
            {
                for(int k = 0;k < height;k++)
                {
                    currentGrid[i, k] = new TileResult2D<T>(default!, 0)
                    {
                        tileIndex = -1,
                        entropy = int.MaxValue,
                        possibleTiles = Enumerable.Range(0, this.tileVariations.Length).ToList()//All tiles are possible at the beginning
                    };
                }
            }
        }

        private void RotateArray<TArr>(ref TArr[] array, int rotation)
        {
            int steps = rotation / 90;
            for(int i = 0;i < steps;i++)
            {
                TArr last = array.Last();
                for(int k = array.Length - 1;k >= 1;k--)
                {
                    array[k] = array[k - 1];
                }
                array[0] = last;
            }
        }

        public bool Iterate(out Grid<TileResult2D<T>> currentGrid)
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
            int selectedTile = useTileProbabilities ? PickRandomTileProbability(currentGrid[x, y].possibleTiles) : currentGrid[x, y].possibleTiles[random.Next(0, currentGrid[x, y].possibleTiles.Count)];
            currentGrid[x, y].tileIndex = selectedTile;
            currentGrid[x, y].selectedTile = this.tiles[this.tileVariations[selectedTile].tileIndex];
            currentGrid[x, y].rotation = this.tileVariations[selectedTile].rotation;
            currentGrid[x, y].entropy = 0;

            return true;
        }

        private int PickRandomTileProbability(List<int> possibleTiles)
        {
            double sum = possibleTiles.Sum(n => this.tileVariations[n].probability);
            double picked = random.NextDouble() * sum;
            double current = 0d;
            for(int i = 0;i < possibleTiles.Count;i++)
            {
                current += this.tileVariations[possibleTiles[i]].probability;
                if(current >= picked)
                {
                    return possibleTiles[i];
                }
            }
            return 0;
        }

        private int CalcEntropy(Grid<TileResult2D<T>> grid, int i, int k, ref List<int> possibleTiles)
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

                TileVariation2D<TConnector> neighbour = this.tileVariations[grid[x, y].tileIndex];
                TConnector neighbourConnector = neighbour.connectors[(l + 2) % 4];//Inverse side of the current neighbour dir
                for(int j = possibleTiles.Count - 1;j >= 0;j--)
                {
                    if(this.tileVariations[possibleTiles[j]].connectors[l].CanConnectTo(neighbourConnector) == false)
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
