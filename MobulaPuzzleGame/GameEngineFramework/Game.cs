using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using Microsoft.Kinect.VisualGestureBuilder;
using NUI3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MobulaPuzzleGame
{
    //Main Game Flow
    public class Game : GameObject
    {
        private PlayerInputController playerInput;
        private PlayerMotor playerMotor;
        private Map map;
        public Game(BodyFrameManager manager) : base(manager)
        {
            playerMotor = new PlayerMotor(manager, new Vector(2, 2), 0.02f);
            playerInput = new PlayerInputController(manager, playerMotor);
            map = new Map(manager);
            TinyFish fish = new TinyFish(manager, new Vector(400, 400), 4f, 70,FaceProperty.Happy);
            TinyFish fish1 = new TinyFish(manager, new Vector(420, 410), 4f, 70,FaceProperty.Happy);
            TinyFish fish2 = new TinyFish(manager, new Vector(410, 430), 4f, 70,FaceProperty.Happy);
        }
        ~Game()
        {
        }
        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            playerInput.UpdatePlayerTarget();
            map.CollisionDetect();
            playerMotor.Movement();
        }

        protected override void Draw(DrawingContext dc)
        {
            base.Draw(dc);
        }
    }
}
