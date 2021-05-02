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
    public class PlayerMotor : GameObject
    {
        public static PlayerMotor Instance { get; private set; }
        public Vector currentPosition { get; private set; }
        public int targetGridPosX { get; set; }
        public int targetGridPosY { get; set; }
        public int curGridPosX { get; private set; }
        public int curGridPosY { get; private set; }
        private float movingSpeed;
        public double towardAngle { get; private set; } = 0;
        private Vector headDirection = new Vector(0, -1);
        private bool targetReached = true;
        public PlayerMotor(BodyFrameManager manager, Vector intiailPos, float sp) : base(manager)
        {
            Instance = this;
            currentPosition = intiailPos;
            curGridPosX = (int)intiailPos.X;
            curGridPosY = (int)intiailPos.Y;
            targetGridPosX = (int)intiailPos.X;
            targetGridPosY = (int)intiailPos.Y;
            movingSpeed = sp;
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
            }
            else
            {
                targetReached = true;
                Map.Instance.SetTile(curGridPosX, curGridPosY, 2);
                curGridPosX = targetGridPosX;
                curGridPosY = targetGridPosY;
            }
        }

        public void Draw(DrawingContext dc)
        {
            //Console.WriteLine("pos:" + currentPosition.X + "," + currentPosition.Y);
            dc.DrawRotatedImage(ResourceManager.Instance.fish, new Point(currentPosition.X * Map.Instance.tileWidth+60, currentPosition.Y * Map.Instance.tileHeight+60), new Rect(0, 0, 120, 120), towardAngle);
            dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(100, 0, 255, 0)), null, new Rect(targetGridPosX * 120, targetGridPosY * 120, 120, 120));
            dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(100, 0, 255, 255)), null, new Rect(curGridPosX * 120, curGridPosY * 120, 120, 120));
        }
    }
}
