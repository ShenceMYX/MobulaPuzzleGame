using Microsoft.Kinect.Face;
using MobulaPuzzleGame.Common;
using NUI3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MobulaPuzzleGame.Character
{
    public class FishGroup : GameObject
    {
        private Vector position;
        private Vector initialPos;
        private float speed;
        private FaceProperty faceTrigger;
        private double distanceToPlayer;
        private double towardAngle = 0;
        private BodyFrameManager bodyManager;
        private bool followTarget = false;
        private bool cageExisit = true;
        private bool entered = false;
        private bool exited = false;
        private bool rescureCounted = false;
        public FishGroup(BodyFrameManager manager, Vector intialGridPos, float sp, double distance, FaceProperty faceType) : base(manager)
        {
            position = new Vector(Map.Instance.tileWidth* intialGridPos.X, Map.Instance.tileHeight*intialGridPos.Y);
            initialPos = intialGridPos;
            speed = sp;
            faceTrigger = faceType;
            distanceToPlayer = distance;
            bodyManager = manager;
            bodyManager.FaceInputHandler += OnFaceDetected;
            bodyFrameManager.playerInputController.restartVoiceDetectedHandler += ResetFishGroup;

        }

        ~FishGroup()
        {
            bodyManager.FaceInputHandler -= OnFaceDetected;
            bodyFrameManager.playerInputController.restartVoiceDetectedHandler -= ResetFishGroup;
        }

        private void OnFaceDetected(FaceFrameResult result)
        {
            //Console.WriteLine(PlayerMotor.Instance.enterFishZoneCount);
            if (VectorHelper.Distance(new Vector(PlayerMotor.Instance.currentPosition.X * Map.Instance.tileWidth,
                PlayerMotor.Instance.currentPosition.Y * Map.Instance.tileHeight), position) >
                Math.Sqrt(Map.Instance.tileWidth * Map.Instance.tileWidth + Map.Instance.tileHeight * Map.Instance.tileHeight))
            {
                if (exited)
                {
                    PlayerMotor.Instance.enterFishZoneCount--;
                    exited = false;
                }
                entered = true;
                return; 
            }

            if (entered)
            {
                PlayerMotor.Instance.enterFishZoneCount++;
                entered = false;
            }
            exited = true;
            switch (faceTrigger)
            {
                case FaceProperty.Happy:
                    PlayerMotor.Instance.fishImage = ResourceManager.Instance.happy;
                    break;
                case FaceProperty.MouthOpen:
                    PlayerMotor.Instance.fishImage = ResourceManager.Instance.mouthopen;
                    break;
            }
            if (result.FaceProperties[faceTrigger] == Microsoft.Kinect.DetectionResult.Yes)
            {
                followTarget = true;
                cageExisit = false;
                if (!rescureCounted)
                {
                    //Console.WriteLine(PlayerMotor.Instance.rescureFish);
                    PlayerMotor.Instance.rescureFish++;
                    rescureCounted = true;
                }
            }
        }
        protected override void Update()
        {
            base.Update();
            if (followTarget)
                MoveToTarget(PlayerMotor.Instance.currentPosition * Map.Instance.tileWidth 
                    - PlayerMotor.Instance.GetHeadDirection() * distanceToPlayer);
            //Console.Write("angle: " + towardAngle);
        }
        public void LookAtTarget(Vector target)
        {
            Vector direction = target - position;
            if (direction == VectorHelper.Zero()) return;
            towardAngle = BitmapHelper.Dir2Angle(direction) + 90;
        }

        public void MoveToTarget(Vector target)
        {
            if (VectorHelper.Distance(position, target) < 4f) { return; }
           // Console.WriteLine(" dis: " + VectorHelper.Distance(position, target));
            LookAtTarget(target);
            position = VectorHelper.MoveToward(position, target, speed);
        }

        protected override void Draw(DrawingContext dc)
        {
            base.Draw(dc);
            dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(50, 255, 255, 220)), null, 
                new Rect(position.X, position.Y , Map.Instance.tileWidth , Map.Instance.tileHeight));
           
            dc.DrawEllipse(new SolidColorBrush(Color.FromArgb(50, 255, 25, 120)), null, 
                new Point(position.X, position.Y),
                Math.Sqrt(Map.Instance.tileWidth * Map.Instance.tileWidth + Map.Instance.tileHeight * Map.Instance.tileHeight), 
                Math.Sqrt(Map.Instance.tileWidth * Map.Instance.tileWidth + Map.Instance.tileHeight * Map.Instance.tileHeight));
            
            dc.DrawRotatedImage(ResourceManager.Instance.sfish1, 
                new Point(position.X + Map.Instance.tileWidth/2, position.Y+ Map.Instance.tileHeight/2), 
                new Rect(0, 0, Map.Instance.tileWidth, Map.Instance.tileHeight), towardAngle);

            if(cageExisit)
                dc.DrawImage(ResourceManager.Instance.cage, new Rect(position.X, position.Y, Map.Instance.tileWidth, Map.Instance.tileHeight));
        }

        public void ResetFishGroup()
        {
            followTarget = false;
            cageExisit = true;
            entered = false;
            exited = false;
            rescureCounted = false;
            towardAngle = 0;
            position = new Vector(Map.Instance.tileWidth * initialPos.X, Map.Instance.tileHeight * initialPos.Y);
        }


    }
}

