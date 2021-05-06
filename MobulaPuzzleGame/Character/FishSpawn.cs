using Microsoft.Kinect.Face;
using NUI3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MobulaPuzzleGame.Character
{
    public class FishSpawn
    {
        private int fishCount;
        public FishSpawn(BodyFrameManager manager,int fishAmount, int x, int y, FaceProperty faceProperty)
        {
            fishCount = fishAmount;

            for (int i = 0; i < fishCount; i++)
            {
                TinyFish fish = new TinyFish(manager, new Vector(400, 400), 4f, 70,FaceProperty.Happy);
                //TinyFish fish1 = new TinyFish(manager, new Vector(420, 410), 4f, 70,FaceProperty.Happy);
                //TinyFish fish2 = new TinyFish(manager, new Vector(410, 430), 4f, 70,FaceProperty.Happy);
            }

        }
    }
}
