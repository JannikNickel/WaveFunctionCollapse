﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WaveFunctionCollapse;
using System.Threading;

namespace WFCUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Settings
        private const double emptyTileSizeMult = 0.95d;
        private const int iterationDelay = 10;
        private const bool animate = false;
        private static readonly string[] tileset0 = new string[]
        {
            "./Tiles/Tileset_0_T0.png",
            "./Tiles/Tileset_0_T1.png",
            "./Tiles/Tileset_0_T2.png",
            "./Tiles/Tileset_0_T3.png",
            "./Tiles/Tileset_0_T4.png"
        };

        //UI elements
        private Canvas canvas;
        private FrameworkElement[,]? uiGrid;

        public MainWindow()
        {
            InitializeComponent();
            canvas = this.MainCanvas;

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Tiles
            //top, right, bottom, left in clockwise order
            StringTile2D<BitmapImage>[] tiles = new StringTile2D<BitmapImage>[]
            {
                new StringTile2D<BitmapImage>(LoadImage(tileset0[0]), new string[] { "0", "0", "0", "0" }, new int[] { 0, 90, 180, 270 }),
                new StringTile2D<BitmapImage>(LoadImage(tileset0[1]), new string[] { "0", "1", "0", "1" }, new int[] { 0/*, 90, 180, 270*/ }),
                new StringTile2D<BitmapImage>(LoadImage(tileset0[2]), new string[] { "1", "1", "0", "0" }, new int[] { 0, 90, 180, 270 }),
                new StringTile2D<BitmapImage>(LoadImage(tileset0[3]), new string[] { "0", "1", "1", "1" }, new int[] { 0, 90, 180, 270 }),
                new StringTile2D<BitmapImage>(LoadImage(tileset0[4]), new string[] { "1", "1", "1", "1" }, new int[] { 0, 90, 180, 270 })
            };

            WFCTiled2D<BitmapImage> wfc = new WFCTiled2D<BitmapImage>(32, 32, tiles);
            if(animate == true)
            {
                Thread t = new Thread(async () =>
                {
                    while(wfc.Iterate(out Grid<TileResult<BitmapImage>> currentGrid) == true)
                    {
                        await Dispatcher.InvokeAsync(() => DrawGrid(currentGrid));
                        Thread.Sleep(iterationDelay);
                    }
                });
                t.IsBackground = true;
                t.Start();
            }
            else
            {
                Grid<TileResult<BitmapImage>> currentGrid;
                while(wfc.Iterate(out currentGrid) == true)
                {
                    
                }
                DrawGrid(currentGrid);
            }
        }

        private void DrawGrid(Grid<TileResult<BitmapImage>> grid, bool cached = true)
        {
            if(cached == false)
            {
                canvas.Children.Clear();
            }
            else if(uiGrid == null || grid.Width != uiGrid.GetLength(0) || grid.Height != uiGrid.GetLength(1))
            {
                canvas.Children.Clear();
                uiGrid = new FrameworkElement[grid.Width, grid.Height];
            }

            double cw = canvas.ActualWidth;
            double ch = canvas.ActualHeight;
            double tileWidth = cw / grid.Width;
            double tileHeight = ch / grid.Height;
            for(int i = 0;i < grid.Width;i++)
            {
                for(int k = 0;k < grid.Height;k++)
                {
                    double sizeMult = grid[i, k].tileSrc == null ? emptyTileSizeMult : 1d;
                    double w = tileWidth * sizeMult;
                    double h = tileHeight * sizeMult;

                    FrameworkElement? element = uiGrid?[i, k];
                    if(element != null && (grid[i, k].tileSrc == null ? (element is not Rectangle) : (element is not Image)))
                    {
                        canvas.Children.Remove(element);
                        element = null;
                    }
                    if(element == null)
                    {
                        element = grid[i, k].tileSrc != null ? new ImageEx() : new Rectangle();
                        if(cached == true && uiGrid != null)
                        {
                            uiGrid[i, k] = element;
                        }

                        if(element is Image image)
                        {
                            image.Stretch = Stretch.Fill;
                            image.Source = grid[i, k].tileSrc;
                            RotateTransform transform = new RotateTransform(grid[i, k].rotation, w * 0.5d, h * 0.5d);
                            image.RenderTransform = transform;
                        }
                        else if(element is Rectangle rect)
                        {
                            rect.Fill = Brushes.LightGray;
                        }

                        canvas.Children.Add(element);
                        element.Width = w;
                        element.Height = h;
                        Canvas.SetLeft(element, i * tileWidth + (tileWidth - w) * 0.5d);
                        Canvas.SetBottom(element, k * tileHeight + (tileHeight - h) * 0.5d);
                    }

                    //Render entropy
                    //TextBlock text = new TextBlock();
                    //text.Text = grid[i, k].entropy.ToString();
                    //canvas.Children.Add(text);
                    //Canvas.SetLeft(text, i * tileWidth + 5d);
                    //Canvas.SetBottom(text, k * tileHeight);
                }
            }
        }

        private static BitmapImage LoadImage(string src)
        {
            Console.WriteLine(src);
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(AppDomain.CurrentDomain.BaseDirectory + src, UriKind.Absolute);
            bitmap.EndInit();
            return bitmap;
        }
    }
}