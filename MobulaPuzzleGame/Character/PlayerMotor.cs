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
using System.Windows.Media.Imaging;

namespace MobulaPuzzleGame
{
    public class PlayerMotor : GameObject
    {
        public static PlayerMotor Instance { get; private set; }
        public Vector currentPosition { get; private set; }
        private Vector initialPosition;
        public int targetGridPosX { get; set; }
        public int targetGridPosY { get; set; }
        public int curGridPosX { get; private set; }
        public int curGridPosY { get; private set; }
        private float movingSpeed;
        public double towardAngle { get; private set; } = 0;
        private Vector headDirection = new Vector(0, -1);
        private bool targetReached = true;
        private float maxPaints = 10;
        private float remainPaints;
        private bool paintCounted = false;
        private BodyFrameManager bodyManager;
        public BitmapImage fishImage { get; set; }
        public int enterFishZoneCount { get; set; } = 0;
        public event Action<float> PaintDecreaseHandler;
        public event Action PaintRunOutHandler;
        public event Action LevelClearHandler;
        public int rescureFish { get; set; } = 0;
        private Random rand = new Random();
        public int randomInkIndex { get; private set; }
        public int randomInkAngle { get; private set; }

        public PlayerMotor(BodyFrameManager manager, Vector intiailPos, float sp, int maxPaintCount) : base(manager)
        {
            Instance = this;
            initialPosition = intiailPos;
            currentPosition = intiailPos;
            curGridPosX = (int)intiailPos.X;
            curGridPosY = (int)intiailPos.Y;
            targetGridPosX = (int)intiailPos.X;
            targetGridPosY = (int)intiailPos.Y;
            movingSpeed = sp;
            maxPaints = maxPaintCount;
            remainPaints = maxPaintCount;
            bodyManager = manager;
            bodyManager.playerMotor = this;
        }

        ~PlayerMotor()
        {
            bodyFrameManager.playerInputController.restartVoiceDetectedHandler -= ResetPlayer;
        }

        protected override void Start()
        {
            base.Start();
            bodyFrameManager.playerInputController.restartVoiceDetectedHandler += ResetPlayer;
        }

        public void RotateHeadNormalClockwise(bool clockWise)
        {
            double x = headDirection.X;
            double y = headDirection.Y;
            if (!clockWise)
            {
                headDirection.X = y;
                headDirection.Y = -x;
                towardAngle -= 90;
            }
            else
            {
                headDirection.X = -y;
                headDirection.Y = x;
                towardAngle += 90;
            }
        }

        public Vector GetHeadDirection()
        {
            return headDirection;
        }
        public void MoveForward()
        {
            if (targetReached)
            {
                targetGridPosX = curGridPosX + (int)headDirection.X;
                targetGridPosY = curGridPosY + (int)headDirection.Y;
            }
        }

        public void LookAtTarget(Vector direction)
        {
            if (direction == VectorHelper.Zero()) return;
            towardAngle = direction.X * 90;
        }
        public void MoveToTarget(Vector direction)
        {
            if (targetReached)
            {
                LookAtTarget(direction);
                targetGridPosX = curGridPosX + (int)direction.X;
                targetGridPosY = curGridPosY + (int)direction.Y;
            }
        }

        public void Movement()
        {
            //Console.WriteLine("cur: " + currentPosition + " , " + "tar: " + newPositionX + " , " + newPositionY + ":::" + VectorHelper.Distance(currentPosition, new Vector(newPositionX, newPositionY)));
            if (VectorHelper.Distance(currentPosition, new Vector(targetGridPosX, targetGridPosY)) > 0.1)
            {
                currentPosition = VectorHelper.MoveToward(currentPosition, new Vector(targetGridPosX, targetGridPosY), movingSpeed);
                targetReached = false;
                paintCounted = false;
            }
            else
            {
                targetReached = true;
                curGridPosX = targetGridPosX;
                curGridPosY = targetGridPosY;
                if(Map.Instance.GetTile(curGridPosX, curGridPosY) != 2)
                {
                    int tileID = Map.Instance.GetTile(curGridPosX, curGridPosY);
                    if (!paintCounted)
                    {
                        GenerateRandomInk();
                        Map.Instance.UpdateInk(curGridPosX, curGridPosY);
                        remainPaints--;
                        paintCounted = true;
                        PaintDecreaseHandler?.Invoke(1 / maxPaints);
                    }
                    Map.Instance.SetTile(curGridPosX, curGridPosY, 2);
                    
                    if (tileID == 5 && remainPaints >=0)
                    {
                        if(rescureFish == 2)
                            LevelClearHandler?.Invoke();
                    }
                    else if (remainPaints <= 0 && tileID != 5)
                        PaintRunOutHandler?.Invoke();
                }

                //Console.WriteLine(remainPaints+" "+1/maxPaints);
            }
        }

        public void Draw(DrawingContext dc)
        {
            //Console.WriteLine("pos:" + currentPosition.X + "," + currentPosition.Y);
            dc.DrawRotatedImage(enterFishZoneCount==rescureFish? ResourceManager.Instance.fish: fishImage, new Point(currentPosition.X * Map.Instance.tileWidth-20 + Map.Instance.tileWidth/2, 
                currentPosition.Y * Map.Instance.tileHeight+ Map.Instance.tileHeight/2-20), 
                new Rect(0, 0, Map.Instance.tileWidth+80, Map.Instance.tileHeight+80), towardAngle);
            //dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(100, 0, 255, 0)), null, new Rect(targetGridPosX * Map.Instance.tileWidth, targetGridPosY * Map.Instance.tileHeight, Map.Instance.tileWidth, Map.Instance.tileHeight));
            //dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(100, 0, 255, 255)), null, new Rect(curGridPosX * Map.Instance.tileWidth, curGridPosY * Map.Instance.tileHeight, Map.Instance.tileWidth, Map.Instance.tileHeight));
        }

        private void GenerateRandomInk()
        {
            randomInkIndex = rand.Next(0, 4);
            randomInkAngle = rand.Next(0, 360);
        }

        public void ResetPlayer()
        {
            currentPosition = initialPosition;
            curGridPosX = (int)initialPosition.X;
            curGridPosY = (int)initialPosition.Y;
            targetGridPosX = (int)initialPosition.X;
            targetGridPosY = (int)initialPosition.Y;
            remainPaints = maxPaints;
            towardAngle = 0;
            headDirection = new Vector(0, -1);
            paintCounted = false;
            targetReached = true;
            enterFishZoneCount = 0;
            rescureFish = 0;
        }
    }
}
