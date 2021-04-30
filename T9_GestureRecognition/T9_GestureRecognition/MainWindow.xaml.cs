using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using NUI3D;

namespace T9_GestureRecognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;
        private DepthFrameManager depthFrameManager;

        private VisualGestureBuilderFrameSource vgbFrameSource;
        private VisualGestureBuilderDatabase vgbDb;
        private VisualGestureBuilderFrameReader vgbFrameReader;

        private BodyFrameReader bodyFrameReader;

        public MainWindow()
        {
            InitializeComponent();            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();
            sensor.Open();

            depthFrameManager = new DepthFrameManager();
            depthFrameManager.Init(sensor, depthImg);

            // second parameter: initial body TrackingId
            vgbFrameSource = new VisualGestureBuilderFrameSource(sensor, 0);

            // Create a gesture database using the pre-trained ones with VGB
            // @ to allow \ in the name 
            vgbDb = new VisualGestureBuilderDatabase(@".\Gestures\Three.gbd");

            vgbFrameSource.AddGestures(vgbDb.AvailableGestures);

            vgbFrameReader = vgbFrameSource.OpenReader();
            vgbFrameReader.FrameArrived += VgbFrameReader_FrameArrived;

            // -------------------------------------------
            bodyFrameReader = sensor.BodyFrameSource.OpenReader();
            bodyFrameReader.FrameArrived += BodyFrameReader_FrameArrived;
        }

        bool r1 = false;
        bool r6 = false;
        bool r7 = false;
        bool r8 = false;
        bool r9 = false;

        private void VgbFrameReader_FrameArrived(object sender, VisualGestureBuilderFrameArrivedEventArgs e)
        {
            using (VisualGestureBuilderFrame vgbFrame = e.FrameReference.AcquireFrame())
            {
                if (vgbFrame == null) return;

                IReadOnlyDictionary<Gesture, DiscreteGestureResult> results =
                    vgbFrame.DiscreteGestureResults;


           

                if (results != null)
                {
                    // Check if any of the gestures is recognized 
                    bool recognized = false;
                   

                    Brush brush = Brushes.Black;

                    foreach (Gesture gesture in results.Keys)
                    {
                        DiscreteGestureResult result = results[gesture];
                        if (result.Detected && result.Confidence > 0.26)
                        {
                            recognitionResult.Text = gesture.Name + " gesture; confidence: " + result.Confidence;
                            recognized = true;
                            if (result.Confidence >= 0.6 && result.Confidence<=0.8)
                                r6 = true;
                            
                            else if (result.Confidence >= 0.8 && result.Confidence <= 1)
                                r8 = true;

                            else if (result.Confidence == 1)
                                r1 = true;

                            // class exercise 

                             if (gesture.Name.Equals("directionright"))
                               brush = Brushes.Green;
                             else if (gesture.Name.Equals("direction"))
                             brush = Brushes.Purple;
                        }
                    }
                    if ( r6 &&  r8 )
                    {
                        brush = Brushes.Red;
                        r6 = false;
                        r8 = false;
                        r1 = false;

                    }
                    if (!recognized) recognitionResult.Text = "No gesture recognized";

                    recognitionResult.Foreground = brush; // class exercise 
                }
            }
        }

        private void BodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {            
            // if (vgbFrameSource.IsTrackingIdValid == false)
            {
                using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
                {
                    if (bodyFrame == null) return;

                    Body body = GetClosestBody(bodyFrame);
                    if (body != null)
                        vgbFrameSource.TrackingId = body.TrackingId;
                }
            }

        }

        private Body GetClosestBody(BodyFrame bodyFrame)
        {
            Body[] bodies = new Body[6];
            bodyFrame.GetAndRefreshBodyData(bodies);

            Body closestBody = null;
            foreach (Body b in bodies)
            {
                if (b.IsTracked)
                {
                    if (closestBody == null) closestBody = b;
                    else
                    {
                        Joint newHeadJoint = b.Joints[JointType.Head];
                        Joint oldHeadJoint = closestBody.Joints[JointType.Head];
                        if (newHeadJoint.TrackingState == TrackingState.Tracked &&
                        newHeadJoint.Position.Z < oldHeadJoint.Position.Z)
                        {
                            closestBody = b;
                        }
                    }
                }
            }
            return closestBody;
        }


    }
}
