using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using Microsoft.Kinect.VisualGestureBuilder;
using MobulaPuzzleGame.Common;
using NUI3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MobulaPuzzleGame
{
    public enum MoveDirection
    {
        None,
        Up,
        Left, 
        Right
    }
    public class PlayerInputController : DetectInputGameObject
    {
        public event Action FlyGestureDetectedHandler;
        public event Action FlyRightGestureDetectedHandler;
        public event Action FlyLeftGestureDetectedHandler;
        public event Action HappyFaceDetectedHandler;
        private PlayerMotor playerMotor;
        private MoveDirection moveDirection = MoveDirection.None;
        public PlayerInputController(BodyFrameManager manager, PlayerMotor motor) : base(manager)
        {
            playerMotor = motor;
        }

        protected override void Start()
        {
            base.Start();
        }

        public void UpdatePlayerTarget()
        {
           
            if(moveDirection == MoveDirection.Left)
            {
                playerMotor.MoveToTarget(new Vector(-1, 0));
            }
            else if (moveDirection == MoveDirection.Right)
            {
                playerMotor.MoveToTarget(new Vector(1, 0));
            }
            else if (moveDirection == MoveDirection.Up)
            {
                playerMotor.MoveToTarget(new Vector(0, -1));
            }
            //Console.WriteLine(moveDirection);

        }

        protected override void OnBodyDetected(Body body)
        {
            base.OnBodyDetected(body);
            //if(body.HandLeftState == HandState.Lasso)
            //    ...
            //Point spineBase = bodyFrameManager.MapCameraPointToScreenSpace(body, JointType.SpineBase);
        }

        bool r6 = false;
        bool r8 = false;
        protected override void OnGestureDetected(IReadOnlyDictionary<Gesture, DiscreteGestureResult> results)
        {
            base.OnGestureDetected(results);
            bool recognized = false;
            foreach (Gesture gesture in results.Keys)
            {
                DiscreteGestureResult result = results[gesture];
                if (result.Detected && result.Confidence > 0.26)
                {
                    if (result.Confidence >= 0.6 && result.Confidence <= 0.8)
                        r6 = true;

                    else if (result.Confidence >= 0.8 && result.Confidence <= 1)
                        r8 = true;

                    if (gesture.Name.Equals("directionright"))
                        moveDirection = MoveDirection.Left;
                    else if (gesture.Name.Equals("direction"))
                        moveDirection = MoveDirection.Right;

                    recognized = true;
                }
            }

            if (r6 && r8)
            {
                Console.WriteLine("flying!!!!!!!!!!!!!!!1");
                //FlyGestureDetectedHandler?.Invoke();
                //playerMotor.MoveToTarget(new Vector(0, -1));
                r6 = false;
                r8 = false;
                moveDirection = MoveDirection.Up;
            }

            if (!recognized)
                moveDirection = MoveDirection.None;
           
        }

        protected override void OnFaceDetected(FaceFrameResult result)
        {
            base.OnFaceDetected(result);
            if (result.FaceProperties[FaceProperty.Happy] == DetectionResult.Yes)
                HappyFaceDetectedHandler?.Invoke();
        }

        protected override void OnVoiceDetection(string command)
        {
            base.OnVoiceDetection(command);
            if (command.Contains("start"))
                Console.WriteLine("!!!!!!!!!!!!");
        }

        protected override void Draw(DrawingContext dc)
        {
            base.Draw(dc);

        }
    }
}
