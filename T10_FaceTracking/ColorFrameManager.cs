using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows;

namespace NUI3D
{
    public class ColorFrameManager
    {
        private KinectSensor sensor;

        private FrameDescription colorFrameDescription = null;
        private byte[] colorData = null;
        private WriteableBitmap colorImageBitmap = null;

        public void Init(KinectSensor s, Image wpfImageForDisplay)
        {
            sensor = s;

            ColorFrameReaderInit(wpfImageForDisplay); 
        }

        private void ColorFrameReaderInit(Image wpfImageForDisplay)
        {
            // Open the reader for the color frames
            ColorFrameReader colorFrameReader = sensor.ColorFrameSource.OpenReader();

            // register an event handler for FrameArrived 
            colorFrameReader.FrameArrived += ColorFrameReader_FrameArrived;
            colorFrameDescription = sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);
            // intermediate storage for receiving frame data from the sensor 
            colorData = new byte[colorFrameDescription.LengthInPixels * colorFrameDescription.BytesPerPixel];

            // bitmap buffer 
            colorImageBitmap = new WriteableBitmap(
                      colorFrameDescription.Width,
                      colorFrameDescription.Height,
                      96, // dpi-x
                      96, // dpi-y
                      PixelFormats.Bgr32, // pixel format  
                      null);

            if (wpfImageForDisplay != null)
                 wpfImageForDisplay.Source = colorImageBitmap;
        }

        private void ColorFrameReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            // using statement automatically takes care of disposing of 
            // the ColorFrame object when you are done using it
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame == null) return;

                // Since we are not using the raw color format, convert the data to our desired format first 
                colorFrame.CopyConvertedFrameDataToArray(colorData, ColorImageFormat.Bgra);

                // output data                 
                colorImageBitmap.WritePixels(
                   new Int32Rect(0, 0,
                   colorFrameDescription.Width, colorFrameDescription.Height), // source rect
                   colorData, // pixel data
                              // stride: width in bytes of a single row of pixel data
                   colorFrameDescription.Width * (int)(colorFrameDescription.BytesPerPixel),
                   0 // offset 
                );
            }
        }
    }
}
