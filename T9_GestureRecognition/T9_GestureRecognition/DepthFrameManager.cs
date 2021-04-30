using System.Windows.Media.Imaging;

using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows;

namespace NUI3D
{
    class DepthFrameManager
    {
        private KinectSensor sensor;

        private FrameDescription depthFrameDescription = null;
        private ushort[] depthData = null;
        private int bytesPerPixel = 4;
        private byte[] depthPixels = null;
        private WriteableBitmap depthImageBitmap = null;
        private System.Windows.Controls.Image wpfImage = null;

        
        public void Init(KinectSensor s, System.Windows.Controls.Image wpfImageForDisplay)
        {
            sensor = s;
            wpfImage = wpfImageForDisplay; 
            DepthFrameReaderInit();
        }

        private void DepthFrameReaderInit()
        {
            // Open the reader for the depth frames
            DepthFrameReader depthFrameReader = sensor.DepthFrameSource.OpenReader();

            // register an event handler for FrameArrived 
            depthFrameReader.FrameArrived += DepthFrameReader_FrameArrived;

            // allocate storage for depth data
            depthFrameDescription = sensor.DepthFrameSource.FrameDescription;
            // 16 - bit unsigned integer per pixel
            depthData = new ushort[depthFrameDescription.LengthInPixels];

            // initialization for displaying depth data
            // to associate 4-byte color for each pixel             
            depthPixels = new byte[depthFrameDescription.LengthInPixels * bytesPerPixel];

            depthImageBitmap = new WriteableBitmap(
                                       depthFrameDescription.Width, // 512 
                                       depthFrameDescription.Height, // 424
                                       96, 96, PixelFormats.Bgr32, null);
            wpfImage.Source = depthImageBitmap;
        }

        private void DepthFrameReader_FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            // using statement automatically takes care of disposing of 
            // the DepthFrame object when you are done using it
            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame == null) return;

                DepthVisualization(depthFrame);                                 
            }
        }        

        private void DepthVisualization(DepthFrame depthFrame)
        {
            depthFrame.CopyFrameDataToArray(depthData);

            // depthData --> depthPixels 
            for (int i = 0; i < depthData.Length; ++i)
            {
                ushort depth = depthData[i];

                ushort minDepth = depthFrame.DepthMinReliableDistance; // 500 
                ushort maxDepth = depthFrame.DepthMaxReliableDistance; // 4500 

                byte depthByte = (byte)((depth - minDepth) * 255.0 / (maxDepth - minDepth));
                depthByte = (byte)(255 - depthByte);

                depthPixels[bytesPerPixel * i] = depthByte;
                depthPixels[bytesPerPixel * i + 1] = depthByte;
                depthPixels[bytesPerPixel * i + 2] = depthByte;
            }

            depthImageBitmap.WritePixels(
              new Int32Rect(0, 0, depthFrameDescription.Width, depthFrameDescription.Height),
              depthPixels, // BGR32 pixel data
                           // stride: width in bytes of a single row of pixel data
              depthFrameDescription.Width * bytesPerPixel, 0);
        }        
    }
}
