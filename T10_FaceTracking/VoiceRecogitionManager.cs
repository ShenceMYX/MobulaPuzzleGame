using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using T11_VoiceControl;

namespace T10_FaceTracking
{
    public class VoiceRecogitionManager
    {
        private KinectSensor sensor;
        private RecognizerInfo kinectRecognizerInfo;
        private SpeechRecognitionEngine recognizer;
        private TextBlock displayVoiceText;
        private TextBlock beamAngleTxt;
        private TextBlock beamAngleConfidenceTxt;
        public event Action<string> CommandInputHandler;
        public void Init(KinectSensor s, TextBlock voice_t, TextBlock angle_t, TextBlock confident_t)
        {
            sensor = s;
            displayVoiceText = voice_t;
            beamAngleTxt = angle_t;
            beamAngleConfidenceTxt = confident_t;

            AudioBeamFrameReader audioBeamFrameReader = sensor.AudioSource.OpenReader();
            audioBeamFrameReader.FrameArrived += AudioBeamFrameReader_FrameArrived;

            kinectRecognizerInfo = FindKinectRecognizerInfo();
            if (kinectRecognizerInfo != null)
            {
                recognizer = new SpeechRecognitionEngine(kinectRecognizerInfo);
            }

            BuildCommands();

            BuildGrammar();

            IReadOnlyList<AudioBeam> audioBeamList = sensor.AudioSource.AudioBeams;
            System.IO.Stream audioStream = audioBeamList[0].OpenInputStream();

            KinectAudioStream kinectAudioStream = new KinectAudioStream(audioStream);
            // let the convertStream know speech is going active
            kinectAudioStream.SpeechActive = true;

            this.recognizer.SetInputToAudioStream(
                kinectAudioStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));

            // recognize words repeatedly and asynchronously
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
            recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            displayVoiceText.Text = "Content: " + e.Result.Text + "; confidence: " + e.Result.Confidence;

            if (e.Result.Confidence < 0.1) return;


            string command = e.Result.Text.ToLower();

            CommandInputHandler?.Invoke(command);
            //if (command.Contains("start"))
            //    Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!11");
           
        }

        private void AudioBeamFrameReader_FrameArrived(object sender, AudioBeamFrameArrivedEventArgs e)
        {
            using (AudioBeamFrameList frameList = e.FrameReference.AcquireBeamFrames())
            {
                if (frameList == null) return;

                // Only one audio beam is supported. Get the sub frame list for this beam
                IReadOnlyList<AudioBeamSubFrame> subFrameList = frameList[0].SubFrames;

                // Loop over all sub frames, extract audio buffer and beam information
                foreach (AudioBeamSubFrame subFrame in subFrameList)
                {
                    beamAngleTxt.Text = "Beam Angle: " + subFrame.BeamAngle * 180 / Math.PI;
                    beamAngleConfidenceTxt.Text = "Confidence: " + subFrame.BeamAngleConfidence;
                }
            }
        }

        private RecognizerInfo FindKinectRecognizerInfo()
        {
            var recognizers =
                SpeechRecognitionEngine.InstalledRecognizers();

            foreach (RecognizerInfo recInfo in recognizers)
            {
                // look at each recognizer info value 
                // to find the one that works for Kinect
                if (recInfo.AdditionalInfo.ContainsKey("Kinect"))
                {
                    string details = recInfo.AdditionalInfo["Kinect"];
                    if (details == "True"
            && recInfo.Culture.Name == "en-US")
                    {
                        // If we get here we have found 
                        // the info we want to use
                        return recInfo;
                    }
                }
            }
            return null;
        }

        private Choices colorCommands = new Choices();
        private void BuildCommands() // call it in Window_Loaded()
        {
          
        }


        private void BuildGrammar() // call it Window_Loaded()
        {
            GrammarBuilder grammarBuilder = new GrammarBuilder();

            grammarBuilder.Append("start");

            // the same culture as the recognizer (US English)
            grammarBuilder.Culture = kinectRecognizerInfo.Culture;

            Grammar grammar = new Grammar(grammarBuilder);


            recognizer.LoadGrammar(grammar);
            
        }
    }
}
