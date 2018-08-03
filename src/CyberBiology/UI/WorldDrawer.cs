using System.Windows.Media;
using System.Windows.Media.Imaging;
using CyberBiology.Core;

namespace CyberBiology.UI
{
    public class WorldDrawer
    {
        public Color BackgroundColor { get; set; } = Colors.White;
        public Color BorderColor { get; set; } = Colors.Brown;

        public Color BotBorderColor { get; set; } = Colors.Black;

        public Color OrganicColor { get; set; } = Color.FromRgb(200, 200, 200);

        public const int CellSize = 4;

        public WriteableBitmap Create(World world)
        {
            return BitmapFactory.New(world.Width * CellSize + 2, world.Height * CellSize + 2);
        }

        public void BaseDraw(World world, WriteableBitmap bmp)
        {
            using (bmp.GetBitmapContext())
            {
                bmp.FillRectangleWH(0, 0, (int)bmp.Width, (int)bmp.Height, BackgroundColor);
                bmp.DrawRectangleWH(0, 0, (int)bmp.Width - 1, (int)bmp.Height - 1, BorderColor);
            }
        }

        public void Draw(World world, WriteableBitmap bmp)
        {
            using (bmp.GetBitmapContext())
            {
                bmp.FillRectangleWH(0, 0, (int)bmp.Width, (int)bmp.Height, BackgroundColor);
                bmp.DrawRectangleWH(0, 0, (int)bmp.Width - 1, (int)bmp.Height - 1, BorderColor);

                for (int y = 0; y < world.Height; y++)
                {
                    for (int x = 0; x < world.Width; x++)
                    {
                        var bot = world.Matrix[x,y];

                        var x1 = 1 + x * CellSize;
                        var y1 = 1 + y * CellSize;

                        if (bot == null)
                        {
                            //bmp.FillRectangleWH(x1, y1, CellSize, CellSize, BackgroundColor);
                            continue;
                        }

                        if (bot.IsOrganic)
                        {
                            bmp.FillRectangleWH(x1, y1, CellSize, CellSize, OrganicColor);
                            continue;
                        }

                        if (bot.IsAlive)
                        {
                            bmp.DrawRectangleWH(x1, y1, CellSize, CellSize, BotBorderColor);

                            int green = bot.Color.G - bot.Color.G * bot.Health / 2000;
                            
                            int blue = (int)(bot.Color.B * 0.8 - bot.Color.B * bot.Mineral / 2000);

                            var color = Color.FromRgb((byte)bot.Color.R, BotColor.Limit(green), BotColor.Limit(blue));

                            bmp.FillRectangleWH(x1 + 1, y1 + 1, CellSize - 1, CellSize - 1, color);
                        }
                    }
                }
            }
        }
    }
}
