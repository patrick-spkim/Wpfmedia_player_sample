using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace winmediaplayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool sldrDragStart = false;
        private bool fullscreen = false;
        private DispatcherTimer DoubleClickTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            DoubleClickTimer.Interval = TimeSpan.FromMilliseconds(GetDoubleClickTime());
            DoubleClickTimer.Tick += (s, e) => DoubleClickTimer.Stop();

        }

        [DllImport("user32.dll")]
        private static extern uint GetDoubleClickTime();

        private void MediaMain_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!DoubleClickTimer.IsEnabled)
            {
                DoubleClickTimer.Start();
            }
            else
            {
                if (!fullscreen)
                {
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;
                }
                else
                {
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = WindowState.Normal;
                }

                fullscreen = !fullscreen;
            }
        }
        private void MediaMain_MediaOpened(object sender, RoutedEventArgs e)
        {
            sldrPlayTime.Minimum = 0;
            sldrPlayTime.Maximum = mediaMain.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void MediaMain_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaMain.Stop();
        }
        private void MediaMain_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show("Failed to play video : " + e.ErrorException.Message.ToString());
        }


        private void sldrPlayTime_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaMain.Source == null)
                return;

            lblPlayTime.Content = String.Format("{0} / {1}", mediaMain.Position.ToString(@"mm\:ss"), mediaMain.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
        }

        private void SldrPlayTime_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            sldrDragStart = true;
            mediaMain.Pause();
        }

        private void SldrPlayTime_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            mediaMain.Position = TimeSpan.FromSeconds(sldrPlayTime.Value);
            mediaMain.Play();
            sldrDragStart = false;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (mediaMain.Source == null)
                return;
            mediaMain.Play();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (mediaMain.Source == null)
                return;
            mediaMain.Stop();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (mediaMain.Source == null)
                return;
            mediaMain.Pause();
        }

        private void SldrVolume_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {

        }


        private void SldrVolume_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            mediaMain.Volume = sldrPlayTime.Value;
        }



        private void btnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog()
            {
                DefaultExt = ".avi",
                Filter = "All files (*.*)|*.*",
                Multiselect = false
            };
            if (dlg.ShowDialog() == true)
            {
                MediaInfo(dlg.FileName);
                mediaMain.Source = new Uri(dlg.FileName);

                mediaMain.Volume = 0.5; mediaMain.SpeedRatio = 1;

                DispatcherTimer timer = new DispatcherTimer()
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                timer.Tick += TimerTickHandler;
                timer.Start();
                mediaMain.Play();
            }
        }
        void TimerTickHandler(object sender, EventArgs e)
        {
            if (sldrDragStart)
                return;

            if (mediaMain.Source == null || !mediaMain.NaturalDuration.HasTimeSpan)
            {
                lblPlayTime.Content = "No file selected...";
                return;
            }
            sldrPlayTime.Value = mediaMain.Position.TotalSeconds;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!fullscreen)
            {
                Window.WindowStyle = WindowStyle.None;
                Window.WindowState = WindowState.Maximized;
                fullscreen = true;
            }
            else
            {
                Window.WindowStyle = WindowStyle.ThreeDBorderWindow;
                Window.WindowState = WindowState.Normal;
                fullscreen = false;
            }




        }



        private void MediaInfo(string FileName)
        {
            string Option_Complete;
            string[] msg = new string[50];
            string[] general = new string[9];
            string[] video = new string[23];
            string[] audio = new string[4];

            var tempMediaInfo = new MediaInfo();
            tempMediaInfo.Open(FileName);
            general[0] = "General";
            tempMediaInfo.Option("Inform", "General;%CompleteName%"); general[1] = "- CompleteName : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "General;%Format/String%"); general[2] = "- Format : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "General;%Format_Profile%"); general[3] = "- Format_Profile : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "General;%CodecID/String%"); general[4] = "- CodecID : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "General;%FileSize/String%"); general[5] = "- FileSize : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "General;%Duration/String%"); general[6] = "- Duration : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "General;%OverallBitRate/String%"); general[7] = "- OverallBitRate : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "General;%Encoded_Application/String%"); general[8] = "- Encoded_Application : " + tempMediaInfo.Inform();// 1 h 38 min


            video[0] = "Video";
            tempMediaInfo.Option("Inform", "Video;%Format/String%"); video[1] = "- Format : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%Format/Info%"); video[2] = "- Format/Info : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%Format_Profile%"); video[3] = "- Format_Profile : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%Format_Settings%"); video[4] = "- Format_Settings : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%Format_Settings_RefFrames/String%"); video[5] = "- Format_Settings_RefFrames : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%CodecID%"); video[6] = "- CodecID : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%CodecID/Info%"); video[7] = "- CodecID/Info : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%Duration/String%"); video[8] = "- Duration : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%BitRate/String%"); video[9] = "- BitRate : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%BitRate_Maximum/String%"); video[10] = "- BitRate_Maximum : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%Width/String%"); video[11] = "- Width : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%Height/String%"); video[12] = "- Height : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%DisplayAspectRatio/String%"); video[13] = "- DisplayAspectRatio : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%FrameRate_Mode/String%"); video[14] = "- FrameRate_Mode : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%FrameRate/String%"); video[15] = "- FrameRate : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%ColorSpace%"); video[16] = "- ColorSpace : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%ChromaSubsampling/String%"); video[17] = "- ChromaSubsampling : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%BitDepth/String%"); video[18] = "- BitDepth : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%ScanType/String%"); video[19] = "- ScanType : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%Bits-(Pixel*Frame)%"); video[20] = "- Bits-(Pixel*Frame) : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%StreamSize/String%"); video[21] = "- StreamSize : " + tempMediaInfo.Inform();// 1 h 38 min
            tempMediaInfo.Option("Inform", "Video;%Encoded_Library/String%"); video[22] = "- Encoded_Library : " + tempMediaInfo.Inform();// 1 h 38 min
            //tempMediaInfo.Option("Inform", "Video;%Encoded_Library_Settings%"); video[23] = "- Encoded_Library_Settings : " + tempMediaInfo.Inform();// 1 h 38 min
            //tempMediaInfo.Option("Inform", "Video;%CodecConfigurationBox%"); video[24] = "- CodecConfigurationBox : " + tempMediaInfo.Inform();// 1 h 38 min


            audio[0] = "Audio";
            tempMediaInfo.Option("Inform", "Audio;%SamplingRate/String%"); audio[1] = "- Audio SamplingRate : " + tempMediaInfo.Inform();// 44.1 kHz
            tempMediaInfo.Option("Inform", "Audio;%Language/String%, %Channel(s)% channels, %Codec/String%, %SamplingRate/String%, %BitRate/String%"); audio[2] = "- Audio Language : " + tempMediaInfo.Inform(); // ,2 channels,, 44.1kHz, 160 kb/s
            tempMediaInfo.Option("Inform", "Audio;%BitDepth/String%"); audio[3] = "- Audio BitDepth : " + tempMediaInfo.Inform();// 

            Option_Complete = tempMediaInfo.Option("Complete", "1");                         // end of session
            string result = tempMediaInfo.Inform();
            lblMediaInfo.Content = string.Join(Environment.NewLine, general);
            lblMediaInfo.Content += Environment.NewLine + Environment.NewLine;
            lblMediaInfo.Content += string.Join(Environment.NewLine, video);
            lblMediaInfo.Content += Environment.NewLine + Environment.NewLine;
            lblMediaInfo.Content += string.Join(Environment.NewLine, audio);

            tempMediaInfo.Close();
            tempMediaInfo.delete_pointeur();

        }

    }
}
