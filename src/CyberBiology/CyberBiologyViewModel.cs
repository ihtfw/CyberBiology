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
                _world = new World(Width / 4, Height / 4);
                _world.CreateAdam();

                Execute.OnUIThread(() =>
                {
                    WorldImage = _worldDrawer.Create(_world);
                });
                
                while (true)
                {
                    _world.NextGeneration();
                    if (_world.Generation % 10 == 0)
                    {
                        Execute.OnUIThread(() =>
                        {
                            _worldDrawer.Draw(_world, WorldImage);
                        });
                    }

                    Generation = _world.Generation;
                    Population = _world.Population;
                    Organic = _world.Organic;
                }
            }, TaskCreationOptions.LongRunning);
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public WriteableBitmap WorldImage { get; private set; }

        public int Generation { get; private set; }
        public int Population { get; private set; }
        public int Organic { get; private set; }
    }
}
