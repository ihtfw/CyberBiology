namespace CyberBiology.Core
{
    public class Color
    {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }

        public void CopyFrom(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public void Reset()
        {
            R = 170;
            G = 170;
            B = 170;
        }

        public void GoGreen(float value)
        {
            // добавляем зелени
            G = G + value;
            if (G > 255) G = 255;
            var nm = value / 2f;
            // убавляем красноту
            R = R - nm;
            if (R < 0) B += R;
            // убавляем синеву
            B = B - nm;
            if (B < 0) R += B;
            if (R < 0) R = 0;
            if (B < 0) B = 0;
        }

        public void GoBlue(float num)
        {
            // добавляем синевы
            B = B + num;
            if (B > 255) B = 255;
            var nm = num / 2f;
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
        public void GoRed(float num)
        {
            // добавляем красноты
            R = R + num;
            if (R > 255) R = 255;
            var nm = num / 2f;
            // убавляем зелень
            G = G - nm;
            if (G < 0) B += G;
            // убавляем синеву
            B = B - nm;
            if (B < 0) G += B;
            if (B < 0) B = 0;
            if (G < 0) G = 0;
        }
        
        public static byte Limit(int value)
        {
            if (value < 0) return 0;
            if (value > 255) return 255;

            return (byte)value;
        }
    }
}