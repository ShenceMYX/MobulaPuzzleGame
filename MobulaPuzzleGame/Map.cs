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
    public class Map : GameObject
    {
        public static Map Instance { get; private set; }
        private const int mapWidth = 10;
        private const int mapHeight = 5;
        public int tileWidth { get; private set; } = 216;
        public int tileHeight { get; private set; } = 216;
        private int[,] mapLayoutGrid;
        private int[,] initialMapLayout;
        private PlayerMotor player;
        private int[,] inkRandIndex;
        private int[,] inkRandAngle;
        public float mapOffsetX { get; private set; } = 0;
        public float mapOffsetY { get; private set; } = 10;
        public Map(BodyFrameManager manager) : base(manager)
        {
            Instance = this;
            player = PlayerMotor.Instance;

            initialMapLayout = new int[mapHeight, mapWidth]
            {
                {0,0,0,0,0,0,0,0,1,5 },
                {0,0,1,0,0,0,1,0,0,0 },
                {0,0,0,0,1,0,1,0,1,1 },
                {0,0,1,0,0,0,0,0,0,0 },
                {0,0,0,0,1,0,0,0,0,0 }
            };
            inkRandIndex = new int[mapHeight, mapWidth];
            inkRandAngle = new int[mapHeight, mapWidth];
            mapLayoutGrid = initialMapLayout;
            bodyFrameManager.playerInputController.restartVoiceDetectedHandler += ResetMap;
        }

        ~Map()
        {
            bodyFrameManager.playerInputController.restartVoiceDetectedHandler -= ResetMap;

        }

        public void CollisionDetect()
        {
            //Console.WriteLine("tie: " + player.targetGridPosX + ","+ player.targetGridPosY);
            if (player.targetGridPosX < 0) PlayerMotor.Instance.targetGridPosX = 0;
            if (player.targetGridPosY < 0) PlayerMotor.Instance.targetGridPosY = 0;
            if (player.targetGridPosX > mapWidth-1) PlayerMotor.Instance.targetGridPosX = mapWidth-1;
            if (player.targetGridPosY > mapHeight-1) PlayerMotor.Instance.targetGridPosY = mapHeight-1;
            if (GetTile(player.targetGridPosX, player.targetGridPosY) == 1)
            {
                PlayerMotor.Instance.targetGridPosX = player.curGridPosX;
                PlayerMotor.Instance.targetGridPosY = player.curGridPosY;
            }
        }

        public int GetTile(int x, int y)
        {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                return mapLayoutGrid[y, x];
            else
                return -1;
        }

        public void SetTile(int x, int y, int type)
        {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                mapLayoutGrid[y, x] = type;
        }

        public void UpdateInk(int x, int y)
        {
            inkRandIndex[y, x] = PlayerMotor.Instance.randomInkIndex;
            inkRandAngle[y, x] = PlayerMotor.Instance.randomInkAngle;
        }

        private void DrawTile(DrawingContext drawingContext, int id, int x, int y)
        {
            if (id==1)
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(255, 0, 255)), null, new Rect(x*tileWidth, y*tileHeight, tileWidth, tileHeight));
            if (id == 2)
            {
                //drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(255, 0, 0)), null, new Rect(x * tileWidth, y * tileHeight, tileWidth, tileHeight));
                drawingContext.DrawRotatedImage(ResourceManager.Instance.inks[inkRandIndex[y, x]], new Point(x * tileWidth + tileWidth/2-30, y * tileHeight+ tileHeight/2-30), new Rect(0, 0, tileWidth+60, tileHeight+60), inkRandAngle[y,x]);
            }
            if (id == 3)
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(0, 0, 255)), null, new Rect(x * tileWidth, y * tileHeight , tileWidth, tileHeight));
              //  drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(0, 0, 0)), null, new Rect(0, 0, 2220, 1080));
        }

        protected override void Draw(DrawingContext dc)
        {
            base.Draw(dc);
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    int tileID = GetTile(i , j);
                   
                    if (tileID != 0)
                        DrawTile(dc, tileID, i, j);
                }
            }
            PlayerMotor.Instance.Draw(dc);
        }

        private void ResetMap()
        {
            mapLayoutGrid = new int[mapHeight, mapWidth]
            {
                {0,0,0,0,0,0,0,0,1,5 },
                {0,0,1,3,0,0,1,0,0,0 },
                {0,0,0,0,1,0,1,0,1,1 },
                {0,0,1,0,0,0,3,0,0,0 },
                {0,0,0,0,1,0,0,0,0,0 }
            };
            //Console.WriteLine(GetTile(0, 3));
        }
    }
}
