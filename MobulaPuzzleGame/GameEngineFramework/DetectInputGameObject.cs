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
            bodyFrameManager.BodyInputHandler += OnBodyDetected;
            bodyFrameManager.GestureInputHandler += OnGestureDetected;
            bodyFrameManager.VoiceRecoManager.CommandInputHandler += OnVoiceDetection;
            bodyFrameManager.FaceInputHandler += OnFaceDetected;
        }



        ~DetectInputGameObject()
        {
            bodyFrameManager.BodyInputHandler -= OnBodyDetected;
            bodyFrameManager.GestureInputHandler -= OnGestureDetected;
            bodyFrameManager.VoiceRecoManager.CommandInputHandler -= OnVoiceDetection;
            bodyFrameManager.FaceInputHandler -= OnFaceDetected;
        }

        protected virtual void OnFaceDetected(FaceFrameResult result) { }
        protected virtual void OnVoiceDetection(string command) { }
        protected virtual void OnBodyDetected(Body body) { }
        protected virtual void OnGestureDetected(IReadOnlyDictionary<Gesture, DiscreteGestureResult> result) { }

    }
}
