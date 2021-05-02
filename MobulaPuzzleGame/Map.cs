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
        private const int mapWidth = 16;
        private const int mapHeight = 9;
        public int tileWidth { get; private set; } = 120;
        public int tileHeight { get; private set; } = 120;
        private int[,] mapLayoutGrid;
        private PlayerMotor player;
        
        private int playerPosX;
        private int playerPosY;
        public Map(BodyFrameManager manager) : base(manager)
        {
            Instance = this;
            player = PlayerMotor.Instance;
            
            mapLayoutGrid = new int[mapHeight, mapWidth]
            {
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1 },
                {1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1 },
                {1,0,0,0,0,0,0,0,0,1,0,0,0,0,0,1 },
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 },
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1 }
            };
        }
       
        public void CollisionDetect()
        {
            //Console.WriteLine("tie: " + player.targetGridPosX + ","+ player.targetGridPosY);
            if(GetTile(player.targetGridPosX, player.targetGridPosY) == 1)
            {
                PlayerMotor.Instance.targetGridPosX = player.curGridPosX;
                PlayerMotor.Instance.targetGridPosY = player.curGridPosY;
            }
        }

        private int GetTile(int x, int y)
        {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                return mapLayoutGrid[y, x];
            else
                return 1;
        }

        public void SetTile(int x, int y, int type)
        {
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                mapLayoutGrid[y, x] = type;
        }

        private void DrawTile(DrawingContext drawingContext, int id, int x, int y)
        {
            if(id==1)
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(255, 0, 255)), null, new Rect(x*tileWidth, y*tileHeight, tileWidth, tileHeight));
            if (id == 2)
                drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(255, 0, 0)), null, new Rect(x * tileWidth, y * tileHeight, tileWidth, tileHeight));
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
    }
}
