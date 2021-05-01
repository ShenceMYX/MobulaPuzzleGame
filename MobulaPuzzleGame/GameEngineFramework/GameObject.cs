using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using Microsoft.Kinect.VisualGestureBuilder;
using NUI3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MobulaPuzzleGame
{
    public class GameObject
    {
        protected BodyFrameManager bodyFrameManager;
        public GameObject(BodyFrameManager manager) 
        {
            bodyFrameManager = manager;
            bodyFrameManager.StartHandler += Start;
            bodyFrameManager.UpateHandler += Update;
            bodyFrameManager.DrawHandler += Draw;
            
        }



        ~GameObject()
        {
            bodyFrameManager.StartHandler -= Start;
            bodyFrameManager.UpateHandler -= Update;
            bodyFrameManager.DrawHandler -= Draw;
          
        }

       
        protected virtual void Start() { }
        protected virtual void Update() { }
        protected virtual void Draw(DrawingContext dc) { }

    }
}
