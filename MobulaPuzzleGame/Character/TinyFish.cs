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

namespace MobulaPuzzleGame
{
    public class TinyFish : GameObject
    {
        private Vector position;
        private float speed;
        private FaceProperty faceTrigger;
        private double distanceToPlayer;
        private double towardAngle = 0;
        private BodyFrameManager bodyManager;
        private bool followTarget = false;
        private Vector randomOffset;
        public TinyFish(BodyFrameManager manager, Vector intialPos, float sp,  double distance, FaceProperty faceType) : base(manager)
        {
            position = intialPos;
            speed = sp;
            faceTrigger = faceType;
            distanceToPlayer = distance;
            bodyManager = manager;
            bodyManager.FaceInputHandler += OnFaceDetected;
            InitRandOffset();
            
        }
        //Code Reference: cnblogs.com/jiading/p/9902835.html
        private void InitRandOffset()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            int iRoot = BitConverter.ToInt32(buffer, 0);
            Random rand = new Random(iRoot);
            randomOffset = new Vector(60 + rand.Next(-60, 60), 60 + rand.Next(-30, 30));
        }

        ~TinyFish()
        {
            bodyManager.FaceInputHandler -= OnFaceDetected;
        }

        private void OnFaceDetected(FaceFrameResult result)
        {
            //Console.WriteLine("dist: " + VectorHelper.Distance(PlayerMotor.Instance.currentPosition*120, position) + "player: "+ PlayerMotor.Instance.currentPosition*120+" fish: " + position);
            if (VectorHelper.Distance(new Vector(PlayerMotor.Instance.currentPosition.X*120, PlayerMotor.Instance.currentPosition.Y * 120), position) > Math.Sqrt(Map.Instance.tileWidth* Map.Instance.tileWidth+Map.Instance.tileHeight* Map.Instance.tileHeight)) return;
            if(result.FaceProperties[faceTrigger] == Microsoft.Kinect.DetectionResult.Yes)
                followTarget = true;       
        }
        protected override void Update()
        {
            base.Update();
            if (followTarget)
                MoveToTarget(PlayerMotor.Instance.currentPosition*120 + randomOffset - PlayerMotor.Instance.GetHeadDirection() * distanceToPlayer);
            //MoveToTarget(new Vector(1000, 1000));
            Console.Write("angle: " + towardAngle);
        }
        public void LookAtTarget(Vector target)
        {
            Vector direction = target - position;
            if (direction == VectorHelper.Zero()) return;
            towardAngle = BitmapHelper.Dir2Angle(direction)+90;
        }

        public void MoveToTarget(Vector target)
        {
            if (VectorHelper.Distance(position, target) < 4f) {  return; }
            Console.WriteLine(" dis: " + VectorHelper.Distance(position, target));
            LookAtTarget(target);
            position = VectorHelper.MoveToward(position, target, speed);
        }

        protected override void Draw(DrawingContext dc)
        {
            base.Draw(dc);
            dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(100, 255, 255, 220)), null, new Rect(position.X, position.Y, 120, 120));
            dc.DrawEllipse(new SolidColorBrush(Color.FromArgb(100, 255, 25, 120)), null, new Point(position.X, position.Y), Math.Sqrt(Map.Instance.tileWidth * Map.Instance.tileWidth + Map.Instance.tileHeight * Map.Instance.tileHeight), Math.Sqrt(Map.Instance.tileWidth * Map.Instance.tileWidth + Map.Instance.tileHeight * Map.Instance.tileHeight));
            dc.DrawRotatedImage(ResourceManager.Instance.tinyFish, new Point(position.X, position.Y), new Rect(0, 0, 50, 50), towardAngle);
        }

    }
}
