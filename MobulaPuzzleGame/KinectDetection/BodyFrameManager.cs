using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using Microsoft.Kinect.VisualGestureBuilder;
using MobulaPuzzleGame;
using MobulaPuzzleGame.Common;

namespace NUI3D
{
    public class BodyFrameManager
    {
        private KinectSensor sensor;

        private Body[] bodies;
        private Body closestBody;

        private bool mapToColorSpace = true;

        private DrawingGroup drawingGroup;
        private DrawingImage drawingImg;
        private double drawingImgWidth = 1920, drawingImgHeight = 1080;

        private JointType[] bones = {
                                // Torso
                    JointType.Head, JointType.Neck,
                    JointType.Neck, JointType.SpineShoulder,
                    JointType.SpineShoulder, JointType.SpineMid,
                    JointType.SpineMid, JointType.SpineBase,
                    JointType.SpineShoulder, JointType.ShoulderRight,
                    JointType.SpineShoulder, JointType.ShoulderLeft,
                    JointType.SpineBase, JointType.HipRight,
                    JointType.SpineBase, JointType.HipLeft,

                    // Right Arm
                    JointType.ShoulderRight, JointType.ElbowRight,
                    JointType.ElbowRight, JointType.WristRight,
                    JointType.WristRight, JointType.HandRight,
                    JointType.HandRight, JointType.HandTipRight,
                    JointType.WristRight, JointType.ThumbRight,

                    // Left Arm
                    JointType.ShoulderLeft, JointType.ElbowLeft,
                    JointType.ElbowLeft, JointType.WristLeft,
                    JointType.WristLeft, JointType.HandLeft,
                    JointType.HandLeft, JointType.HandTipLeft,
                    JointType.WristLeft, JointType.ThumbLeft,

                    // Right Leg
                    JointType.HipRight, JointType.KneeRight,
                    JointType.KneeRight, JointType.AnkleRight,
                    JointType.AnkleRight, JointType.FootRight,
                
                    // Left Leg
                    JointType.HipLeft, JointType.KneeLeft,
                    JointType.KneeLeft, JointType.AnkleLeft,
                    JointType.AnkleLeft, JointType.FootLeft,
        };

        private double jointSize = 5;
        private double boneThickness = 10;

        private bool showHandStates = false;

        private VisualGestureBuilderFrameSource vgbFrameSource;
        private VisualGestureBuilderDatabase vgbDb;
        private VisualGestureBuilderFrameReader vgbFrameReader;
        private TextBlock recognitionResult;

        public event Action UpateHandler;
        public event Action StartHandler;
        public event Action<Body> BodyInputHandler;
        public event Action<FaceFrameResult> FaceInputHandler;
        public event Action<IReadOnlyDictionary<Gesture, DiscreteGestureResult>> GestureInputHandler;
        public event Action<DrawingContext> DrawHandler;
        public event Action LoadImageHandler;
        public PlayerInputController playerInputController { get; set; }
        public PlayerMotor playerMotor { get; set; }
        public VoiceRecogitionManager VoiceRecoManager { get; private set; }
        public void ShowHandStates(bool show = true)
        {
            showHandStates = show;
        }

        public void Init(KinectSensor s, Image wpfImageForDisplay, TextBlock textForGesture, VoiceRecogitionManager voiceManager, Boolean toColorSpace = true)
        {
            recognitionResult = textForGesture;
            sensor = s;
            VoiceRecoManager = voiceManager;

            if (toColorSpace) // map the skeleton to the color space
            {
                drawingImgWidth = sensor.ColorFrameSource.FrameDescription.Width;
                drawingImgHeight = sensor.ColorFrameSource.FrameDescription.Height;
            }
            else // map the skeleton to the depth space 
            {
                drawingImgWidth = sensor.DepthFrameSource.FrameDescription.Width;
                drawingImgHeight = sensor.DepthFrameSource.FrameDescription.Height;
            }
            DrawingGroupInit(wpfImageForDisplay);

            mapToColorSpace = toColorSpace;

            VgbInit();

            BodyFrameReaderInit();

            //ResetJointColors();

            FaceFrameReaderInit();


            Game game = new Game(this);
            ResourceManager resourceManager = new ResourceManager(this);

            LoadImages();

            StartHandler?.Invoke();

        }

        public Point MapCameraPointToScreenSpace(Body body, JointType jointType)
        {
            Point screenPt = new Point(0, 0);
            if (mapToColorSpace) // to color space 
            {
                ColorSpacePoint pt = sensor.CoordinateMapper.MapCameraPointToColorSpace(
                body.Joints[jointType].Position);
                screenPt.X = pt.X;
                screenPt.Y = pt.Y;
            }
            else // to depth space
            {
                DepthSpacePoint pt = sensor.CoordinateMapper.MapCameraPointToDepthSpace(
                    body.Joints[jointType].Position);
                screenPt.X = pt.X;
                screenPt.Y = pt.Y;
            }
            return screenPt;
        }

        private void VgbInit()
        {
            // second parameter: initial body TrackingId
            vgbFrameSource = new VisualGestureBuilderFrameSource(sensor, 0);

            // Create a gesture database using the pre-trained ones with VGB
            // @ to allow \ in the name 
            vgbDb = new VisualGestureBuilderDatabase(@".\Gestures\Three.gbd");

            vgbFrameSource.AddGestures(vgbDb.AvailableGestures);

            vgbFrameReader = vgbFrameSource.OpenReader();
            vgbFrameReader.FrameArrived += VgbFrameReader_FrameArrived;
        }

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
                    GestureInputHandler?.Invoke(results);
                    foreach (Gesture gesture in results.Keys)
                    {
                        DiscreteGestureResult result = results[gesture];
                        if (result.Detected)
                        {
                            recognitionResult.Text = gesture.Name + " gesture recognized; confidence: " + result.Confidence;
                            recognized = true;
                        }
                        
                    }
                    if (!recognized) recognitionResult.Text = "No gesture recognized";

                    recognitionResult.Foreground = brush; // class exercise 
                }
            }
        }

        private void BodyFrameReaderInit()
        {
            BodyFrameReader bodyFrameReader = sensor.BodyFrameSource.OpenReader();
            bodyFrameReader.FrameArrived += BodyFrameReader_FrameArrived; 
            
            // BodyCount: maximum number of bodies that can be tracked at one time
            bodies = new Body[sensor.BodyFrameSource.BodyCount];
        }

        private void BodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame == null) return;

                bodyFrame.GetAndRefreshBodyData(bodies);

                using (DrawingContext dc = drawingGroup.Open())
                {
                    // draw a transparent background to set the render size
                    dc.DrawRectangle(Brushes.Transparent, null,
                            new Rect(0.0, 0.0, drawingImgWidth, drawingImgHeight));

                   

                    closestBody = GetClosestBody(bodyFrame);
                    if (closestBody == null) return;

                    BodyInputHandler?.Invoke(closestBody);
                    UpateHandler?.Invoke();
                    DrawHandler?.Invoke(dc);

                    DrawSkeleton(closestBody, dc);
                    vgbFrameSource.TrackingId = closestBody.TrackingId;
                    // -----------------------------------------------

                    if (faceFrameSource.IsTrackingIdValid == false)
                    {
                        if (closestBody.IsTracked)
                            faceFrameSource.TrackingId = closestBody.TrackingId;
                    }
                    else
                    {
                        FaceInputHandler?.Invoke(faceFrameResult);
                        // class exercise 1
                        DrawFace(dc, faceFrameResult);
                    }
                }
            }
        }

        private void DrawingGroupInit(Image wpfImageForDisplay) // called in Window_Loaded 
        {
            drawingGroup = new DrawingGroup();
            drawingImg = new DrawingImage(drawingGroup);
            wpfImageForDisplay.Source = drawingImg;

            // prevent drawing outside of our render area
            drawingGroup.ClipGeometry = new RectangleGeometry(
                                        new Rect(0.0, 0.0, 2120, drawingImgHeight));
        }

        private void DrawSkeleton(Body body, DrawingContext dc)
        {
            for (int i = 0; i < bones.Length; i += 2)
            {
                DrawBone(body, dc, bones[i], bones[i + 1]);
            }

            foreach (JointType jt in body.Joints.Keys)
            {
                Point pt = MapCameraPointToScreenSpace(body, jt);
                //Brush brush = jointBrushes[jt];
                dc.DrawEllipse(Brushes.LightGreen, null, pt, jointSize, jointSize);
            }

            if (showHandStates)
            {
                // Visualize the hand tracking states             
                VisualizeHandState(body, dc, JointType.HandLeft, body.HandLeftState);
                VisualizeHandState(body, dc, JointType.HandRight, body.HandRightState);
            }
        }

        private void DrawBone(Body body, DrawingContext dc, JointType j0, JointType j1)
        {
            Point pt0 = MapCameraPointToScreenSpace(body, j0);
            Point pt1 = MapCameraPointToScreenSpace(body, j1);

            dc.DrawLine(new Pen(Brushes.DarkGreen, boneThickness), pt0, pt1);
        }

        private void VisualizeHandState(Body body, DrawingContext dc, JointType jointType, HandState handState)
        {
            SolidColorBrush green = new SolidColorBrush(Color.FromArgb(100, 0, 255, 0));
            SolidColorBrush red = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0));
            SolidColorBrush blue = new SolidColorBrush(Color.FromArgb(100, 0, 0, 255));
            double radius = 80;
            Point hand_pt = MapCameraPointToScreenSpace(body, jointType);
            switch (handState)
            {
                case HandState.Closed:
                    dc.DrawEllipse(red, null, hand_pt, radius, radius);
                    break;
                case HandState.Open:
                    dc.DrawEllipse(green, null, hand_pt, radius, radius);
                    break;
                case HandState.Lasso:
                    dc.DrawEllipse(blue, null, hand_pt, radius, radius);
                    break;
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

        // ----------------------------------------------
        FaceFrameFeatures faceFrameFeatures =
            FaceFrameFeatures.BoundingBoxInColorSpace
            | FaceFrameFeatures.PointsInColorSpace
            | FaceFrameFeatures.RotationOrientation
            | FaceFrameFeatures.FaceEngagement
            | FaceFrameFeatures.Glasses
            | FaceFrameFeatures.Happy
            | FaceFrameFeatures.LeftEyeClosed
            | FaceFrameFeatures.RightEyeClosed
            | FaceFrameFeatures.LookingAway
            | FaceFrameFeatures.MouthMoved
            | FaceFrameFeatures.MouthOpen;

        private FaceFrameSource faceFrameSource;
        private void FaceFrameReaderInit() // to be called in Init()
        {
            // second parameter: initial body tracking ID
            faceFrameSource = new FaceFrameSource(sensor, 0, faceFrameFeatures);
            FaceFrameReader faceFrameReader = faceFrameSource.OpenReader();

            faceFrameReader.FrameArrived += FaceFrameReader_FrameArrived;
        }

        private FaceFrameResult faceFrameResult = null;

        private void FaceFrameReader_FrameArrived(object sender, FaceFrameArrivedEventArgs e)
        {
            using (FaceFrame faceFrame = e.FrameReference.AcquireFrame())
            {
                if (faceFrame == null) return;

                faceFrameResult = faceFrame.FaceFrameResult;
            }
        }

        private void DrawFace(DrawingContext dc, FaceFrameResult result)
        {
            // validate face frame result 
            if (IsValidateFaceResult(result) == false) return;

            // class exercise 1: 
            // draw the face bounding box            
            RectI faceBoxSource = result.FaceBoundingBoxInColorSpace;
            Rect faceRect = new Rect(faceBoxSource.Left, faceBoxSource.Top,
        faceBoxSource.Right - faceBoxSource.Left, faceBoxSource.Bottom - faceBoxSource.Top);

            dc.DrawRectangle(null, new Pen(Brushes.Purple, 5), faceRect);            

            // class exercise 2:
            // draw face points
            foreach (PointF point in result.FacePointsInColorSpace.Values)
            {
                dc.DrawEllipse(null, new Pen(Brushes.LightCyan, 5), new Point(point.X, point.Y), 1, 1);
            }            

            // expression 
            // extract each face property information and store it in faceText
            string faceText = string.Empty;
            if (result.FaceProperties != null)
            {
                foreach (var item in result.FaceProperties)
                {
                    faceText += item.Key.ToString() + " : ";

                    // consider a "maybe" as a "no" to restrict 
                    // the detection result refresh rate
                    if (item.Value == DetectionResult.Maybe)
                        faceText += DetectionResult.No + "\n";
                    else
                        faceText += item.Value.ToString() + "\n";
                }
            }

            if (result.FaceRotationQuaternion != null)
            {
                int pitch, yaw;
                ExtractFaceRotationInDegrees(result.FaceRotationQuaternion, out pitch, out yaw, out roll);
                faceText += "FaceYaw : " + yaw + "\n" +
                            "FacePitch : " + pitch + "\n" +
                            "FaceRoll : " + roll + "\n";
            }

            // render the face property and face rotation information
            Point faceTextLayout = new Point(1200, 300); //  faceBoxSource.Right, faceBoxSource.Bottom);
            dc.DrawText(new FormattedText(
                        faceText,
                        System.Globalization.CultureInfo.GetCultureInfo("en-us"),
                        FlowDirection.LeftToRight,
                        new Typeface("Georgia"),
                        30, Brushes.Red),
                    faceTextLayout);
            
            // class exercise 3: draw emoji depending on the face properties  
            DrawImages(dc, faceRect, result);
        }

        private bool IsValidateFaceResult(FaceFrameResult faceResult)
        {
            bool isFaceValid = faceResult != null;

            if (isFaceValid)
            {
                RectI faceBox = faceResult.FaceBoundingBoxInColorSpace;

                // check if we have a valid rectangle within the bounds of the screen space
                isFaceValid = (faceBox.Right - faceBox.Left) > 0 &&
                                (faceBox.Bottom - faceBox.Top) > 0 &&
                                faceBox.Right <= drawingImgWidth &&
                                faceBox.Bottom <= drawingImgHeight;
            }
            return isFaceValid;
        }

        // class exercise 3: load images
        private BitmapImage face_smiling;
        private BitmapImage face_no_smiling;
        private BitmapImage glasses;
        private double glassesAspectRatio;

        private int roll;

        private void LoadImages()
        {
            LoadImageHandler?.Invoke();
            face_no_smiling = new BitmapImage(
                new Uri("Images/emoji_no_smiling.png", UriKind.Relative));

            face_smiling = new BitmapImage(
                new Uri("Images/emoji_smiling.png", UriKind.Relative));

            glasses = new BitmapImage(
                new Uri("Images/sunglass.png", UriKind.Relative));
            glassesAspectRatio = glasses.Width / glasses.Height;
        }

        // class exercise 3: draw images 
        private void DrawImages(DrawingContext dc, Rect faceRect, FaceFrameResult result)
        {
            // draw a smiling face if the user is happy 
            if (result.FaceProperties[FaceProperty.Happy] == DetectionResult.Yes)
                //dc.DrawImage(face_smiling, faceRect);
                DrawRotatingImage(dc, face_smiling, faceRect, -roll); // class exercise 4            
            else
               // dc.DrawImage(face_no_smiling, faceRect);
               DrawRotatingImage(dc, face_no_smiling, faceRect, -roll); // class exercise 4           

            // draw glasses
            if (result.FaceProperties[FaceProperty.WearingGlasses] == DetectionResult.Yes)
            {
                PointF leftEye = result.FacePointsInColorSpace[FacePointType.EyeLeft];
                PointF rightEye = result.FacePointsInColorSpace[FacePointType.EyeRight];
                float eyeY = (leftEye.Y + rightEye.Y) / 2;
                Rect gR = new Rect(0, 0, faceRect.Width, 100);
                gR.Height = faceRect.Width / glassesAspectRatio;
                gR.X = faceRect.Left;
                gR.Y = eyeY - faceRect.Height * 0.1;
                // dc.DrawImage(glasses, gR);
                // class exercise 4: draw the rotating glasses
                Point faceCenter = new Point((faceRect.Left + faceRect.Right) / 2, (faceRect.Top + faceRect.Bottom) / 2);
                DrawRotatingImage(dc, glasses, gR, faceCenter, -roll);
            }
        }

        private void ExtractFaceRotationInDegrees(Vector4 rotQuaternion, out int pitch, out int yaw, out int roll)
        {
            double x = rotQuaternion.X;
            double y = rotQuaternion.Y;
            double z = rotQuaternion.Z;
            double w = rotQuaternion.W;

            // convert face rotation quaternion to Euler angles in degrees
            double yawD, pitchD, rollD;
            pitchD = Math.Atan2(2 * ((y * z) + (w * x)), (w * w) - (x * x) - (y * y) + (z * z)) / Math.PI * 180.0;
            yawD = Math.Asin(2 * ((w * y) - (x * z))) / Math.PI * 180.0;
            rollD = Math.Atan2(2 * ((x * y) + (w * z)), (w * w) + (x * x) - (y * y) - (z * z)) / Math.PI * 180.0;

            // clamp the values to a multiple of the specified increment to control the refresh rate
            double increment = 5.0;
            pitch = (int)(Math.Floor((pitchD + ((increment / 2.0) * (pitchD > 0 ? 1.0 : -1.0))) / increment) * increment);
            yaw = (int)(Math.Floor((yawD + ((increment / 2.0) * (yawD > 0 ? 1.0 : -1.0))) / increment) * increment);
            roll = (int)(Math.Floor((rollD + ((increment / 2.0) * (rollD > 0 ? 1.0 : -1.0))) / increment) * increment);
        }

        // Draw an image which rotates around a specific rotation center by angle (in degree)
        private void DrawRotatingImage(DrawingContext dc, ImageSource imageSource, Rect imageRect, Point rotationCenter, double angle)
        {
            TranslateTransform translation = new TranslateTransform(imageRect.X, imageRect.Y);
            dc.PushTransform(translation); // apply translation 
            {
                RotateTransform rotation = new RotateTransform(angle, rotationCenter.X - imageRect.X, rotationCenter.Y - imageRect.Y);
                dc.PushTransform(rotation); // apply rotation 
                Rect tempR = new Rect(0, 0, imageRect.Width, imageRect.Height); // translation already applied before
                dc.DrawImage(imageSource, tempR);
                dc.Pop(); // reset rotation 
            }
            dc.Pop(); // reset translation 
        }

        // Draw an image which rotates around the image center by angle (in degree)
        private void DrawRotatingImage(DrawingContext dc, ImageSource imageSource, Rect imageRect, double angle)
        {
            Point imgCenter = new Point((imageRect.Left + imageRect.Right) / 2, (imageRect.Top + imageRect.Bottom) / 2);
            DrawRotatingImage(dc, imageSource, imageRect, imgCenter, angle);
        }

        //private Dictionary<JointType, Brush> jointBrushes = new Dictionary<JointType, Brush>();

        //public void ResetJointColors()
        //{
        //    foreach (JointType jt in Enum.GetValues(typeof(JointType)))
        //        jointBrushes[jt] = Brushes.LightGreen;
        //}

        //public void SetJointColor(JointType jt, Brush brush)
        //{
        //    jointBrushes[jt] = brush;
        //}

    }
}
