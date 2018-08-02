﻿namespace CyberBiology.Core
{
    public class BotColor
    {
        public int R { get; private set; }
        public int G { get; private set; }
        public int B { get; private set; }

        public void CopyFrom(BotColor botColor)
        {
            R = botColor.R;
            G = botColor.G;
            B = botColor.B;
        }

        public void GoGreen(int value)
        {
            // добавляем зелени
            G = G + value;
            if (G > 255) G = 255;
            var nm = value / 2;
            // убавляем красноту
            R = R - nm;
            if (R < 0) B += R;
            // убавляем синеву
            B = B - nm;
            if (B < 0) R += B;
            if (R < 0) R = 0;
            if (B < 0) B = 0;
        }

        public void GoBlue(int num)
        {
            // добавляем синевы
            B = B + num;
            if (B > 255) B = 255;
            var nm = num / 2;
            // убавляем зелень
            G = G - nm;
            if (G < 0) R +=  G;
            // убавляем красноту
            R = R - nm;
            if (R < 0) G +=  R;
            if (R < 0) R = 0;
            if (G < 0) G = 0;
        }

        //жжжжжжжжжжжжжжжжжжжхжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжжж
        //=== делаем бота более красным на экране         ======
        //=== in - номер бота, на сколько окраснить       ======
        public void GoRed(int num)
        {
            // добавляем красноты
            R = R + num;
            if (R > 255) R = 255;
            var nm = num / 2;
            // убавляем зелень
            G = G - nm;
            if (G < 0) B += G;
            // убавляем синеву
            B = B - nm;
            if (B < 0) G += B;
            if (B < 0) B = 0;
            if (G < 0) G = 0;
        }

        public void Adam()
        {
            R = 170;
            G = 170;
            B = 170;
        }
    }
}