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
    public class DetectInputGameObject : GameObject
    {
        public DetectInputGameObject(BodyFrameManager manager) : base(manager)
        {
            bodyFrameManager.BodyInputHandler += BodyInputDetection;
            bodyFrameManager.GestureInputHandler += GestureInputDetection;
            bodyFrameManager.VoiceRecoManager.CommandInputHandler += VoiceInputDetection;
            bodyFrameManager.FaceInputHandler += FaceInputDetection;
        }



        ~DetectInputGameObject()
        {
            bodyFrameManager.BodyInputHandler -= BodyInputDetection;
            bodyFrameManager.GestureInputHandler -= GestureInputDetection;
            bodyFrameManager.VoiceRecoManager.CommandInputHandler -= VoiceInputDetection;
            bodyFrameManager.FaceInputHandler -= FaceInputDetection;
        }

        protected virtual void FaceInputDetection(FaceFrameResult result) { }
        protected virtual void VoiceInputDetection(string command) { }
        protected virtual void BodyInputDetection(Body body) { }
        protected virtual void GestureInputDetection(Gesture gesture, DiscreteGestureResult result) { }

    }
}
