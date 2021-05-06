using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Common
{
    static class TransformHelper
    {
        //Code Reference:https://blog.csdn.net/qq_18995513/article/details/72765269
        public static void TranslateImg(this System.Windows.Controls.Image transformImage, float x, float y)
        {
            TransformGroup tg = transformImage.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                TranslateTransform tt = tgnew.Children[0] as TranslateTransform;
                tt.X += x;
                tt.Y += y;
            }
            transformImage.RenderTransform = tgnew;
        }

        public static void TransformImgToTargetPos(this System.Windows.Controls.Image transformImage, float x, float y)
        {
            TransformGroup tg = transformImage.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                TranslateTransform tt = tgnew.Children[0] as TranslateTransform;
                tt.X = x;
                tt.Y = y;
            }
            transformImage.RenderTransform = tgnew;
        }

        public static void ScaleImg(this System.Windows.Controls.Image transformImage, float x, float y)
        {
            TransformGroup tg = transformImage.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                ScaleTransform st = tgnew.Children[2] as ScaleTransform;
                st.ScaleX += x;
                st.ScaleY += y;
            }
            transformImage.RenderTransform = tgnew;
        }

        public static void SetImgScale(this System.Windows.Controls.Image transformImage, float x, float y)
        {
            TransformGroup tg = transformImage.RenderTransform as TransformGroup;
            var tgnew = tg.CloneCurrentValue();
            if (tgnew != null)
            {
                ScaleTransform st = tgnew.Children[2] as ScaleTransform;
                st.ScaleX = x;
                st.ScaleY = y;
            }
            transformImage.RenderTransform = tgnew;
        }
    }
}
