using NUI3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MobulaPuzzleGame.Common
{
    public class ResourceManager
    {
        public static ResourceManager Instance { get; private set; }
        private BodyFrameManager bodyFrameManager;
        public BitmapImage fish { get; private set; }
        public BitmapImage tinyFish { get; private set; }
        public BitmapImage happy { get; private set; }
        public BitmapImage mouthopen { get; private set; }
        public BitmapImage rocktr { get; private set; }
        public BitmapImage rockbl { get; private set; }
        public BitmapImage bg { get; private set; }
        public BitmapImage blocks { get; private set; }
        public BitmapImage sfish1 { get; private set; }
        public BitmapImage sfish2 { get; private set; }
        public BitmapImage start { get; private set; }
        public BitmapImage cage { get; private set; }

        public BitmapImage[] inks { get; private set; } = new BitmapImage[4];
        public ResourceManager(BodyFrameManager manager)
        {
            Instance = this;
            bodyFrameManager = manager;
            bodyFrameManager.LoadImageHandler += LoadImage;
        }

        ~ResourceManager()
        {
            bodyFrameManager.LoadImageHandler -= LoadImage;
        }

        private void LoadImage()
        {
            tinyFish = new BitmapImage(new Uri("Images/sunglass.png", UriKind.Relative));
            fish = new BitmapImage(new Uri("Images/fish.png", UriKind.Relative));
            happy = new BitmapImage(new Uri("Images/happy.png", UriKind.Relative));
            mouthopen = new BitmapImage(new Uri("Images/mouthopen.png", UriKind.Relative));
            bg = new BitmapImage(new Uri("Images/bg.png", UriKind.Relative));
            blocks = new BitmapImage(new Uri("Images/blocks.png", UriKind.Relative));
            sfish1 = new BitmapImage(new Uri("Images/sfish1.png", UriKind.Relative));
            sfish2 = new BitmapImage(new Uri("Images/sfish2.png", UriKind.Relative));
            start = new BitmapImage(new Uri("Images/start.png", UriKind.Relative));
            rocktr = new BitmapImage(new Uri("Images/toprightRock.png", UriKind.Relative));
            rockbl = new BitmapImage(new Uri("Images/bottomleftrock.png", UriKind.Relative));
            cage = new BitmapImage(new Uri("Images/cage.png", UriKind.Relative));

            for (int i = 0; i < inks.Length; i++)
            {
                inks[i] = new BitmapImage(new Uri(String.Format("Images/ink{0}.png", i+1), UriKind.Relative));
            }
        }
    }
}
