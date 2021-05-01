using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using NUI3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MobulaPuzzleGame
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

        bool r6 = false;
        bool r8 = false;
        protected override void GestureInputDetection(Gesture gesture, DiscreteGestureResult result)
        {
            base.GestureInputDetection(gesture, result);
            if (result.Confidence >= 0.6 && result.Confidence <= 0.8)
                r6 = true;

            else if (result.Confidence >= 0.8 && result.Confidence <= 1)
                r8 = true;


            if (gesture.Name.Equals("directionright"))
                Console.WriteLine("right!!!!!!!!!!1");
            else if (gesture.Name.Equals("direction"))
                Console.WriteLine("Left!!!!!!!!!!!!!11");

            if (r6 && r8)
            {
                Console.WriteLine("flying!!!!!!!!!!!!!!!1");
                r6 = false;
                r8 = false;
            }
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
