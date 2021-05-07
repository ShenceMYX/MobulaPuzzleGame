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
        Left, 
        Right
    }
    public class PlayerInputController : DetectInputGameObject
    {
        public event Action startVoiceDetectedHandler;
        public event Action restartVoiceDetectedHandler;
        public event Action nextVoiceDetectedHandler;
        private PlayerMotor playerMotor;
        private MoveDirection moveDirection = MoveDirection.None;
        private double titleAnlge;
        private bool moveForward = false;
        public PlayerInputController(BodyFrameManager manager, PlayerMotor motor) : base(manager)
        {
            playerMotor = motor;
            manager.playerInputController = this;
        }

        protected override void Start()
        {
            base.Start();
        }

        private DateTime startRotateTime = DateTime.Now;
        private float rotationInterval = 1.5f;
        public void UpdatePlayerTarget()
        {
            if (moveForward)
            {
                if(moveDirection == MoveDirection.None)
                {
                    playerMotor.MoveForward();
                }
                else
                {
                    if (DateTime.Now > startRotateTime)
                    {
                        if (moveDirection == MoveDirection.Left)
                        {
                            playerMotor.RotateHeadNormalClockwise(false);
                            //playerMotor.MoveToTarget(new Vector(-1, 0));
                        }
                        else if (moveDirection == MoveDirection.Right)
                        {
                            playerMotor.RotateHeadNormalClockwise(true);
                            //playerMotor.MoveToTarget(new Vector(1, 0));
                        }
                        startRotateTime = DateTime.Now.AddSeconds(rotationInterval);
                    }
                }
            }
           
        }

        protected override void OnBodyDetected(Body body)
        {
            base.OnBodyDetected(body);

            titleAnlge = GetBodyTitleAngle(body);

            if (Math.Abs(titleAnlge) > 7)
            {
                moveDirection = titleAnlge > 0 ? MoveDirection.Left : MoveDirection.Right;
            }
            else
            {
                moveDirection = MoveDirection.None;
            }
        }

        private double GetBodyTitleAngle(Body body)
        {
            Point spineBase = bodyFrameManager.MapCameraPointToScreenSpace(body, JointType.SpineBase);
            Point spineShoulder = bodyFrameManager.MapCameraPointToScreenSpace(body, JointType.SpineShoulder);
            Vector bodySpineLine = new Vector(spineShoulder.X - spineBase.X, spineShoulder.Y - spineBase.Y);

            //Console.WriteLine(spineBase.Y-lastY);
            //lastY = spineBase.Y;
            return Vector.AngleBetween(bodySpineLine, new Vector(0, -100));
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

                    //if (gesture.Name.Equals("directionright"))
                    //    moveDirection = MoveDirection.Left;
                    //else if (gesture.Name.Equals("direction"))
                    //    moveDirection = MoveDirection.Right;

                    recognized = true;
                }
            }

            if (r6 && r8)
            {
                //Console.WriteLine("flying!!!!!!!!!!!!!!!1");
                //FlyGestureDetectedHandler?.Invoke();
                //playerMotor.MoveToTarget(new Vector(0, -1));
                r6 = false;
                r8 = false;
                moveForward = true;
            }

            if (!recognized)
                moveForward = false;
        }
        

        protected override void OnVoiceDetection(string command)
        {
            base.OnVoiceDetection(command);
            if (command.Contains("start"))
            {
                if (command.Contains("the game"))
                    startVoiceDetectedHandler?.Invoke();

            }
            else if (command.Contains("replay"))
            {
                if (command.Contains("this level"))
                    restartVoiceDetectedHandler?.Invoke();
            }
            else if (command.Contains("next"))
            {
                if (command.Contains("level"))
                    nextVoiceDetectedHandler?.Invoke();
            }
        }

        
    }
}
