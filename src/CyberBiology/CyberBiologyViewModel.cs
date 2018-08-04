using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using CyberBiology.Core;
using CyberBiology.UI;

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
                _world.CreateAdam();
                
                Execute.OnUIThread(() =>
                {
                    WorldImage = _worldDrawer.Create(_world);
                    _worldDrawer.BaseDraw(_world, WorldImage);
                });

                while (true)
                {
                    _world.NextIterationInParallel();

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
                }
            }, TaskCreationOptions.LongRunning);
        }
        
        public int Width { get; private set; }
        public int Height { get; private set; }

        public WriteableBitmap WorldImage { get; private set; }

        public int Iteration { get; private set; }
        public int Population { get; private set; }
        public int Organic { get; private set; }
    }
}
