using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using CyberBiology.Core;
using CyberBiology.Core.Serialization;
using CyberBiology.UI;
using Microsoft.Win32;

namespace CyberBiology
{
    class CyberBiologyViewModel : Screen
    {
        private readonly WorldDrawer _worldDrawer = new WorldDrawer();
        private World _world;

        public CyberBiologyViewModel()
        {
            DisplayName = "CyberBiology 1.0.0";
        }

        public bool IsBusy { get; private set; }

        private bool _isPaused;
        private bool _addRandomBot;
        private bool _randomMutations;
        private bool _clearWorld;
        private bool _newWorld;

        public void ClearWorld()
        {
            IsBusy = true;
            _clearWorld = true;
        }

        public void AddRandomBot()
        {
            IsBusy = true;
            _addRandomBot = true;
        }

        public void RandomMutations()
        {
            IsBusy = true;
            _randomMutations = true;
        }

        public void NewWorld()
        {
            IsBusy = true;
            _newWorld = true;
        }

        public async void SaveWorld()
        {
            try
            {
                _isPaused = true;
                var saveFileDialog = new SaveFileDialog
                {
                    AddExtension = true,
                    DefaultExt = ".cbw",
                    Filter = "CyberBiology World|*.cbw"
                };
                if (saveFileDialog.ShowDialog(Application.Current.MainWindow) != true)
                {
                    return;
                }

                IsBusy = true;
                var path = saveFileDialog.FileName;

                await Task.Factory.StartNew(() =>
                {
                    var worldSerializer = new WorldSerializer();
                    worldSerializer.Save(World.Instance, path);
                });
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to save world" + Environment.NewLine + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                _isPaused = false;
                IsBusy = false;
            }
        }

        public async void OpenWorld()
        {
            try
            {
                _isPaused = true;
                var openFileDialog = new OpenFileDialog
                {
                    AddExtension = true,
                    CheckFileExists = true,
                    DefaultExt = ".cbw",
                    Filter = "CyberBiology World|*.cbw"
                };
                if (openFileDialog.ShowDialog(Application.Current.MainWindow) != true)
                {
                    return;
                }

                var path = openFileDialog.FileName;

                IsBusy = true;

                await Task.Factory.StartNew(() =>
                {
                    var worldSerializer = new WorldSerializer();
                    var worldDto = worldSerializer.Load(path);

                    World.Instance.Clear();
                    World.Instance.LoadWorld(worldDto);
                });
            }
            catch (Exception e)
            {
                MessageBox.Show("Failed to open world" + Environment.NewLine + e.Message, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                _isPaused = false;
                IsBusy = false;
            }
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            Width = 512;
            Height = 512;
           
            if (view is CyberBiologyView cyberBiologyView)
            {
                Width = (int)cyberBiologyView.ImageGrid.ActualWidth;
                Height = (int)cyberBiologyView.ImageGrid.ActualHeight;
            }

            Task.Factory.StartNew(() =>
            {
                _world = new World(Width / WorldDrawer.CellSize, Height / WorldDrawer.CellSize);
                _newWorld = true;

                SizeX = _world.Width;
                SizeY = _world.Height;

                Execute.OnUIThread(() =>
                {
                    WorldImage = _worldDrawer.Create(_world);
                    _worldDrawer.BaseDraw(_world, WorldImage);
                });

                while (true)
                {
                    if (_newWorld)
                    {
                        _newWorld = false;
                        
                        _world.Clear();
                        for (int i = 0; i < Width * Height / 100; i++)
                        {
                            _world.AddRandomBot();
                        }
                        _world.CreateAdam();

                        IsBusy = false;
                    }

                    if (_isPaused)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    if (_clearWorld)
                    {
                        _clearWorld = false;
                        _world.Clear();
                        IsBusy = false;
                    }
                    if (_addRandomBot)
                    {
                        _addRandomBot = false;
                        _world.AddRandomBot();
                        IsBusy = false;
                    }
                    if (_randomMutations)
                    {
                        _randomMutations = false;
                        _world.RandomMutations();
                        IsBusy = false;
                    }
                    _world.NextIterationInParallel();
                    //_world.NextIteration();

                    var k = (_world.Population + _world.Organic) / 5000 + 1;
                    if (_world.Iteration % k == 0)
                    {
                        Execute.OnUIThread(() =>
                        {
                            _worldDrawer.Draw(_world, WorldImage);
                        });
                    }

                    Iteration = _world.Iteration;
                    Population = _world.Population;
                    Organic = _world.Organic;
                    Empty = _world.Empty;
                }
            }, TaskCreationOptions.LongRunning);
        }
        
        public int SizeX { get; private set; }
        public int SizeY { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public WriteableBitmap WorldImage { get; private set; }

        public int Iteration { get; private set; }
        public int Population { get; private set; }
        public int Organic { get; private set; }
        public int Empty { get; private set; }
    }
}
