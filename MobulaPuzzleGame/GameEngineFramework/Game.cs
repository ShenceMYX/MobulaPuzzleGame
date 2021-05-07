using Microsoft.Kinect;
using Microsoft.Kinect.Face;
using Microsoft.Kinect.VisualGestureBuilder;
using MobulaPuzzleGame.Character;
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
        private bool gameStarted = false;
        public Game(BodyFrameManager manager) : base(manager)
        {
            playerMotor = new PlayerMotor(manager, new Vector(0, 4), 0.02f, 14);
            playerInput = new PlayerInputController(manager, playerMotor);
            map = new Map(manager);
            FishGroup fishGroup = new FishGroup(manager, new Vector(3, 2), 4f, 100, FaceProperty.Happy);
            FishGroup fishGroup2 = new FishGroup(manager, new Vector(6, 2), 4f, 100, FaceProperty.Happy);
            RegisterUIEvents();
        }

        ~Game()
        {
            UnregisterUIEvents();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            if (!gameStarted) return;
            playerInput.UpdatePlayerTarget();
            map.CollisionDetect();
            playerMotor.Movement();
        }

        private void SetGameStart()
        {
            gameStarted = true;
        }

        private void SetGameNotStart()
        {
            gameStarted = false;
        }

        private void RegisterUIEvents()
        {
            playerInput.startVoiceDetectedHandler += SetGameStart;
            playerInput.nextVoiceDetectedHandler += SetGameStart;
            playerInput.restartVoiceDetectedHandler += SetGameStart;
            playerMotor.PaintRunOutHandler += SetGameNotStart;
            playerMotor.LevelClearHandler += SetGameNotStart;
        }
        private void UnregisterUIEvents()
        {
            playerInput.startVoiceDetectedHandler -= SetGameStart;
            playerInput.nextVoiceDetectedHandler -= SetGameStart;
            playerInput.restartVoiceDetectedHandler -= SetGameStart;
            playerMotor.PaintRunOutHandler -= SetGameNotStart;
            playerMotor.LevelClearHandler -= SetGameNotStart;
        }

    }
}
