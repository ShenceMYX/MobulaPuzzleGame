using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using NUI3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace T10_FaceTracking
{
    public class Player : GameObject
    {
        public Player(BodyFrameManager manager) : base(manager)
        {
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void BodyInputDetection(Body body)
        {
            base.BodyInputDetection(body);
            //if(body.HandLeftState == HandState.Lasso)
            //    ...
            //Point spineBase = bodyFrameManager.MapCameraPointToScreenSpace(body, JointType.SpineBase);
        }

        protected override void GestureInputDetection(Gesture gesture)
        {
            base.GestureInputDetection(gesture);
            //if(gesture.Name.Equals(""))
            //    ...
        }

        protected override void VoiceInputDetection(string command)
        {
            base.VoiceInputDetection(command);
            if (command.Contains("start"))
                Console.WriteLine("!!!!!!!!!!!!");
        }

        protected override void Draw(DrawingContext dc)
        {
            base.Draw(dc);
        }
    }
}
