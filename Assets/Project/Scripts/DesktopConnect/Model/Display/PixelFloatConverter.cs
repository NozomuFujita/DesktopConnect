namespace DisplaySystem
{
    public class PixelFloatConverter
    {
        private int displayWidthPixel;
        private int displayHeightPixel;
        private float displayWidthFloat;
        private float displayHeightFloat;


        /// <summary>
        /// メインディスプレイの解像度を、Unityでのワールド座標系に変換　refで渡しつつ、今後の変換処理でも使うのでセット
        /// </summary>
        public PixelFloatConverter(int displayWidthPixel, int displayHeightPixel, ref float displayWidthFloat, ref float displayHeightFloat)
        {
            displayWidthFloat = displayHeightFloat * (float)displayWidthPixel / ((float)displayHeightPixel * 2);

            this.displayWidthPixel = displayWidthPixel;
            this.displayHeightPixel = displayHeightPixel;
            this.displayWidthFloat = displayWidthFloat;
            this.displayHeightFloat = displayHeightFloat;
        }


        /// <summary>
        /// Pixel座標からワールド座標系へ　(X座標)
        /// </summary>
        public float ConvertPixel2WorldX(int pixelX)
        {
            return ((float)(pixelX / displayWidthPixel) - 0.5f) * displayWidthFloat * 2;
        }


        /// <summary>
        /// Pixel座標からワールド座標系へ　(Y座標)
        /// </summary>
        public float ConvertPixel2WorldY(int pixelY)
        {
            return (1.0f - (float)(pixelY / displayHeightPixel)) * displayHeightFloat;
        }


        /// <summary>
        /// Pixel座標からワールド座標系へ　(X座標)
        /// </summary>
        public int ConvertWorld2PixelX(int worldX)
        {
            return (int)((worldX / (displayWidthFloat * 2)) + 0.5f) * displayWidthPixel;
        }


        /// <summary>
        /// Pixel座標からワールド座標系へ　(Y座標)
        /// </summary>
        public int ConvertWorld2PixelY(int worldY)
        {
            return (int)(1.0f - (worldY / displayHeightFloat)) * displayHeightPixel;
        }
    }
}
