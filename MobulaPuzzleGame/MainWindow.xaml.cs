using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common;
using Microsoft.Kinect;
using Microsoft.Kinect.VisualGestureBuilder;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using NUI3D;
using T11_VoiceControl;

namespace MobulaPuzzleGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor sensor;
        private BodyFrameManager bodyFrameManager;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sensor = KinectSensor.GetDefault();
            sensor.Open();

            //ColorFrameManager colorFrameManager = new ColorFrameManager();
            //colorFrameManager.Init(sensor, colorImg);

            VoiceRecogitionManager voiceRecogitionManager = new VoiceRecogitionManager();
            voiceRecogitionManager.Init(sensor);

            bodyFrameManager = new BodyFrameManager();
            bodyFrameManager.Init(sensor, skeletonImg, voiceRecogitionManager);

            bodyFrameManager.playerInputController.startVoiceDetectedHandler += GameStarted;
            bodyFrameManager.playerInputController.restartVoiceDetectedHandler += FailUIHide;
            bodyFrameManager.playerInputController.restartVoiceDetectedHandler += ResetPaintBar;
            bodyFrameManager.playerInputController.nextVoiceDetectedHandler += NextLevelUIHide;
           
            bodyFrameManager.playerMotor.PaintDecreaseHandler += OnPaintDecrease;
            bodyFrameManager.playerMotor.PaintRunOutHandler += FailUIAppear;
            bodyFrameManager.playerMotor.LevelClearHandler += NextLevelUIAppear;
        }

        private void GameStarted()
        {
            startPage.Visibility = Visibility.Hidden;
        }

        private void FailUIAppear()
        {
            fail.Visibility = Visibility.Visible;
        }
        private void FailUIHide()
        {
            fail.Visibility = Visibility.Hidden;
        }

        private void NextLevelUIAppear()
        {
            next.Visibility = Visibility.Visible;
        }
        private void NextLevelUIHide()
        {
            next.Visibility = Visibility.Hidden;
        }

        private void OnPaintDecrease(float decreaseHPPro)
        {
            red_blood_bar.ScaleImg(-decreaseHPPro, 0);
        }

        private void ResetPaintBar()
        {
            red_blood_bar.SetImgScale(1, 1);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            bodyFrameManager.playerInputController.startVoiceDetectedHandler -= GameStarted;
            bodyFrameManager.playerInputController.restartVoiceDetectedHandler -= FailUIHide;
            bodyFrameManager.playerInputController.restartVoiceDetectedHandler -= ResetPaintBar;
            bodyFrameManager.playerInputController.nextVoiceDetectedHandler -= NextLevelUIHide;

            bodyFrameManager.playerMotor.PaintDecreaseHandler -= OnPaintDecrease;
            bodyFrameManager.playerMotor.PaintRunOutHandler -= FailUIAppear;
            bodyFrameManager.playerMotor.LevelClearHandler -= NextLevelUIAppear;
        }
    }
}
