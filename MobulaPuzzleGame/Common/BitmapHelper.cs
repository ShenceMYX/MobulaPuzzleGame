using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MobulaPuzzleGame
{
    public static class BitmapHelper
    {
        public static void DrawRotatedImage(this DrawingContext dc, ImageSource imageSource, Point imageCenter, Rect imageSize, double angle)
        {
            imageSize.X = imageSize.Y = 0;

            TranslateTransform translation = new TranslateTransform(imageCenter.X - imageSize.Width / 2,
                                                        imageCenter.Y - imageSize.Height / 2);
            dc.PushTransform(translation); // apply translation 
            {
                // rotate with respect to image center 
                RotateTransform rotation = new RotateTransform(angle,
                    imageSize.Width / 2, imageSize.Height / 2);
                dc.PushTransform(rotation); // apply rotation 
                dc.DrawImage(imageSource, imageSize); // no translation in r_gun (X = 0, Y = 0)
                dc.Pop(); // reset rotation 
            }
            dc.Pop(); // reset translation
        }

        public static void DrawFlippedImage(this DrawingContext dc, ImageSource imageSource, Point imageCenter, Rect imageSize)
        {
            imageSize.X = imageSize.Y = 0;

            TranslateTransform translation = new TranslateTransform(imageCenter.X +136,
                                                        imageCenter.Y);
            dc.PushTransform(translation); // apply translation 
            {
                ScaleTransform fliping = new ScaleTransform(-1, 1);
                dc.PushTransform(fliping); // apply rotation 
                dc.DrawImage(imageSource, imageSize); // no translation in r_gun (X = 0, Y = 0)
                dc.Pop(); // reset rotation 
            }
            dc.Pop(); // reset translation
        }

        public static void DrawFlipImage(this DrawingContext dc, ImageSource imageSource, Point imageCenter, Rect imageSize)
        {
            imageSize.X = imageSize.Y = 0;

            TranslateTransform translation = new TranslateTransform(imageCenter.X, imageCenter.Y);
            dc.PushTransform(translation); // apply translation 
            {
                ScaleTransform fliping = new ScaleTransform(-1, 1);
                dc.PushTransform(fliping); // apply rotation 
                {
                    TranslateTransform returntranslation = new TranslateTransform(-imageSize.Width, 0);
                    dc.PushTransform(returntranslation);
                    dc.DrawImage(imageSource, imageSize); // no translation in r_gun (X = 0, Y = 0)
                    dc.Pop();
                }
                dc.Pop(); // reset rotation 
            }
            dc.Pop(); // reset translation
        }

        public static double Dir2Angle(Vector dir) // exercise 7 
        {
            return Math.Atan2(dir.Y, dir.X) / Math.PI * 180;
        }
    }
}
