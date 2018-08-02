using System.Windows.Media;
using System.Windows.Media.Imaging;
using CyberBiology.Core;

namespace CyberBiology.UI
{
    public class WorldDrawer
    {
        public Color BackgroundColor { get; set; } = Colors.White;
        public Color BorderColor { get; set; } = Colors.Brown;

        public Color OrganicColor { get; set; } = Color.FromRgb(200, 200, 200);
        
        public WriteableBitmap Create(World world)
        {
            return BitmapFactory.New(world.Width * 4 + 2, world.Height * 4 + 2);
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

                        if (bot == null)
                        {
                            bmp.FillRectangleWH(1 + x * 4, 1 + y * 4, 4, 4, BackgroundColor);
                        }
                        else if ((bot.IsOrganic))
                        {
                            bmp.FillRectangleWH(1 + x * 4, 1 + y * 4, 4, 4, OrganicColor);
                        }
                        else if (bot.IsAlive)
                        {
                            bmp.DrawRectangleWH(1 + x * 4, 1 + y * 4, 4, 4, Colors.Black);

                            int green = bot.Color.G - ((bot.Color.G * bot.health) / 2000);
                            if (green < 0) green = 0;
                            if (green > 255) green = 255;
                            int blue = (int)(bot.Color.B * 0.8 - ((bot.Color.B * bot.mineral) / 2000));
                            var color = Color.FromRgb((byte)bot.Color.R, (byte) green, (byte) blue);

                            bmp.FillRectangleWH(1 + x * 4 + 1, 1 + y * 4 + 1, 3, 3, color);
                        }
                    }
                }
            }
        }
    }
}
