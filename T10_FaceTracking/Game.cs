using Microsoft.Kinect;
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

namespace T10_FaceTracking
{
    //Main Game Flow
    public class Game : GameObject
    {
        public Game(BodyFrameManager manager) : base(manager)
        {
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
        }

        protected override void Draw(DrawingContext dc)
        {
            base.Draw(dc);
        }
    }
}
