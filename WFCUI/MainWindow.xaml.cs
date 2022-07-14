using System;
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
using System.Text.RegularExpressions;

namespace WFCUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Tileset data
        private static readonly string[] tileset0 = new string[]
        {
            "./Tiles/Tileset_0_T0.png",
            "./Tiles/Tileset_0_T1.png",
            "./Tiles/Tileset_0_T2.png",
            "./Tiles/Tileset_0_T3.png",
            "./Tiles/Tileset_0_T4.png"
        };
        private static readonly string[] tileset1 = new string[]
        {
            "./Tiles/Tileset_1_T0.png",
            "./Tiles/Tileset_1_T1.png",
            "./Tiles/Tileset_1_T2.png",
            "./Tiles/Tileset_1_T3.png",
            "./Tiles/Tileset_1_T4.png",
            "./Tiles/Tileset_1_T5.png",
            "./Tiles/Tileset_1_T6.png",
            "./Tiles/Tileset_1_T7.png",
            "./Tiles/Tileset_1_T8.png",
            "./Tiles/Tileset_1_T9.png",
            "./Tiles/Tileset_1_T10.png",
            "./Tiles/Tileset_1_T11.png",
            "./Tiles/Tileset_1_T12.png",
            "./Tiles/Tileset_1_T13.png"
        };
        //Tiles connectors -> top, right, bottom, left in clockwise order
        private static readonly IReadOnlyDictionary<string, Tile2D<BitmapImage, StringConnector>[]> tilesets = new Dictionary<string, Tile2D<BitmapImage, StringConnector>[]>()
        {
            { "Pipes", new Tile2D<BitmapImage, StringConnector>[5]
                {
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset0[0]), 1d, "0", "0", "0", "0", new int[] { 0, 90, 180, 270 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset0[1]), 0.25d, "0", "1", "0", "1", new int[] { 0/*, 90, 180, 270*/ }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset0[2]), 1d, "1", "1", "0", "0", new int[] { 0, 90, 180, 270 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset0[3]), 1d, "0", "1", "1", "1", new int[] { 0, 90, 180, 270 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset0[4]), 1d, "1", "1", "1", "1", new int[] { 0, 90, 180, 270 })
                }
            },
            { "Circuit", new Tile2D<BitmapImage, StringConnector>[]
                {
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[0]), 1d, "RRR", "RRR", "RRR", "RRR", new int[] { 0 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[1]), 1d, "RRR", "RRR", "RRG", "GRR", new int[] { 0, 90, 180, 270 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[2]), 1d, "ROR", "RRR", "ROR", "RRR", new int[] { 0, 90 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[3]), 1d, "RRR", "ROR", "ROR", "ROR", new int[] { 0, 90, 180, 270 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[4]), 1d, "ROR", "ROR", "RRR", "RRR", new int[] { 0, 90, 180, 270 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[5]), 1d, "RRR", "ROR", "RRR", "ROR", new int[] { 0, 90 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[6]), 1d, "ROR", "RRG", "GGG", "GRR", new int[] { 0, 90, 180, 270 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[7]), 1d, "ROR", "RRR", "RRR", "RRR", new int[] { 0, 90, 180, 270 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[8]), 1d, "RLR", "RRR", "ROR", "RRR", new int[] { 0, 90, 180, 270 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[9]), 1d, "RRR", "RLR", "RRR", "RLR", new int[] { 0, 90 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[10]), 1d, "ROR", "ROR", "RRR", "RRR", new int[] { 0, 90, 180, 270 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[11]), 1d, "GGG", "GGG", "GGG", "GGG", new int[] { 0, 90 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[12]), 1d, "ROR", "RLR", "ROR", "RLR", new int[] { 0, 90 }),
                    new Tile2D<BitmapImage, StringConnector>(LoadImage(tileset1[13]), 1d, "ROR", "ROR", "ROR", "ROR", new int[] { 0 })
                    //TODO fix the connecting to itself for some tiles (https://github.com/CodingTrain/Wave-Function-Collapse/issues/23)
                }
            },
        };

        //UI settings
        private const double emptyTileSizeMult = 0.95d;

        //UI elements
        private Canvas canvas;
        private FrameworkElement[,]? uiGrid;

        //Runtime control
        private bool isRunning = false;
        private CancellationTokenSource cancellationTokenSource;

        public MainWindow()
        {
            InitializeComponent();
            canvas = this.MainCanvas;

            cancellationTokenSource = new CancellationTokenSource();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //UI stuff
            tilesetCombobox.ItemsSource = tilesets.Select(n => n.Key);
            tilesetCombobox.SelectedIndex = 0;
            sizeSlider.ValueChanged += delegate
            {
                if(isRunning == false)
                {
                    //Draw empty grid if slider changed
                    ShowEmptyGrid((int)Math.Round(sizeSlider.Value));
                }
            };
            ShowEmptyGrid((int)Math.Round(sizeSlider.Value));
            seedTextbox.PreviewTextInput += delegate(object sender, TextCompositionEventArgs e)
            {
                if(new Regex("^[0-9]*$").IsMatch(e.Text) == false)
                {
                    e.Handled = false;
                }
            };
            seedButton.Click += delegate
            {
                NewRandomSeed();
            };
            seedTextbox.Text = "0";
            startButton.Click += delegate
            {
                //Read settings from UI and start generation
                Tile2D<BitmapImage, StringConnector>[] tileset = tilesets[(string)tilesetCombobox.SelectedItem];
                bool animate = animateCheckbox.IsChecked.Value;
                int iterationDelay = animate == true ? (int)Math.Round(delaySlider.Value * 1000) : 0;
                int size = (int)Math.Round(sizeSlider.Value);
                bool outputTiles = outputTilesCheckbox.IsChecked.Value;
                bool stopIfNoSolution = stopIfNoSolutionCheckbox.IsChecked.Value;
                bool restartIfNoSolution = restartIfNoSolutionCheckbox.IsChecked.Value;
                bool backtracking = backtrackingCheckbox.IsChecked.Value;
                int seed = int.Parse(seedTextbox.Text);

                Generate(tileset, animate, iterationDelay, size, outputTiles, stopIfNoSolution, restartIfNoSolution, backtracking, seed);
            };
            return;
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            CancelCurrentGeneration();
        }

        private void NewRandomSeed()
        {
            seedTextbox.Text = Random.Shared.Next().ToString();
        }

        private void ShowEmptyGrid(int size)
        {
            Grid<TileResult2D<Tile2D<BitmapImage, StringConnector>>> emptyGrid = new Grid<TileResult2D<Tile2D<BitmapImage, StringConnector>>>(size, size);
            for(int i = 0;i < size;i++)
            {
                for(int k = 0;k < size;k++)
                {
                    emptyGrid[i, k] = new TileResult2D<Tile2D<BitmapImage, StringConnector>>(null, 0);
                }
            }
            DrawGrid(emptyGrid);
        }

        private void Generate(Tile2D<BitmapImage, StringConnector>[] tileset, bool animate, int iterationDelay, int size, bool outputTiles, bool stopIfNoSolution, bool restartIfNoSolution, bool backtracking, int seed)
        {
            //Cancel old task
            CancelCurrentGeneration();

            //Generate new cancellation token
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            //Generate wfc object
            isRunning = true;
            WFCTiled2D<Tile2D<BitmapImage, StringConnector>, BitmapImage, StringConnector> wfc = new WFCTiled2D<Tile2D<BitmapImage, StringConnector>, BitmapImage, StringConnector>(size, size, tileset, false, seed, backtracking);

            //Show debug tiles
            if(outputTiles == true)
            {
                foreach(var item in wfc.TileVariations)
                {
                    TilePreview window = new TilePreview();
                    window.Show();
                    window.Init(tileset[item.tileIndex].Data, item.rotation, item.connectors.Select(n => n.ToString()).ToArray());
                }
            }

            Thread t = new Thread(() =>
            {
                Grid<TileResult2D<Tile2D<BitmapImage, StringConnector>>> currentGrid = null;
                while(token.IsCancellationRequested == false)
                {
                    WFCResult result = wfc.Iterate(out currentGrid, stopIfNoSolution);
                    if(result == WFCResult.Error && restartIfNoSolution == true)
                    {
                        wfc.Reset();
                    }
                    else if(result != WFCResult.Step)
                    {
                        break;
                    }

                    if(animate == true)
                    {
                        Dispatcher.Invoke(() => DrawGrid(currentGrid, true));

                        if(iterationDelay > 0)
                        {
                            Thread.Sleep(iterationDelay);
                        }
                    }
                }
                if(animate == false)
                {
                    Dispatcher.Invoke(() => DrawGrid(currentGrid, true));
                }
                Dispatcher.Invoke(() => isRunning = false);
            });
            t.IsBackground = true;
            t.Start();
        }

        private void CancelCurrentGeneration()
        {
            if(cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
        }

        private void DrawGrid(Grid<TileResult2D<Tile2D<BitmapImage, StringConnector>>> grid, bool cached = true)
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
                    double sizeMult = grid[i, k].selectedTile == null ? emptyTileSizeMult : 1d;
                    double w = tileWidth * sizeMult;
                    double h = tileHeight * sizeMult;

                    FrameworkElement? element = uiGrid?[i, k];
                    if(element != null)
                    {
                        bool tileChanged = (grid[i, k].selectedTile == null ? (element is not Rectangle) : (element is not Image));
                        //Also test if the actual image has to be changed
                        if(grid[i, k].selectedTile != null && element is Image eImg)
                        {
                            if(grid[i, k].selectedTile.Data != (BitmapImage)eImg.Source)
                            {
                                tileChanged = true;
                            }
                            if(eImg.RenderTransform is RotateTransform rt && rt.Angle != grid[i, k].rotation)
                            {
                                tileChanged = true;
                            }
                        }
                        if(tileChanged == true)
                        {
                            canvas.Children.Remove(element);
                            element = null;
                        }
                    }
                    if(element == null)
                    {
                        element = grid[i, k].selectedTile != null ? new ImageEx() : new Rectangle();
                        if(cached == true && uiGrid != null)
                        {
                            uiGrid[i, k] = element;
                        }

                        if(element is Image image)
                        {
                            image.Stretch = Stretch.Fill;
                            image.Source = grid[i, k].selectedTile.Data;
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
