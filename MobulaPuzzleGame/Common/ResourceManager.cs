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
            fish = new BitmapImage(new Uri("Images/emoji_smiling.png", UriKind.Relative));
        }
    }
}
