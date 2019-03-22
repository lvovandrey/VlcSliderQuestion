using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace VlcSliderQuestion
{
    /// <summary>
    /// Логика взаимодействия для Player.xaml
    /// </summary>
    public partial class Player : UserControl
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position",
          typeof(double), typeof(Player), new FrameworkPropertyMetadata()); 
        
        public double Position { get { return (double)GetValue(PositionProperty); } set { SetValue(PositionProperty, value); } }

        System.Windows.Threading.DispatcherTimer timer;
        Vlc.DotNet.Forms.VlcControl VLC;

        public Player() : base()
        {
            InitializeComponent();
            VLC = vlc.MediaPlayer;//просто удобнее обращаться
            VLC.VlcLibDirectory = new System.IO.DirectoryInfo(@"c:\Program Files\VideoLAN\VLC\"); //Путь может отличаться.
            VLC.EndInit();
            VLC.Play(new Uri(@"http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4"));

            timer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += new EventHandler(timerTick);
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            Position = 1000 * (double)VLC.Time / VLC.Length; 
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (VLC.IsPlaying) VLC.Pause();
            else VLC.Play();  
        }

        private void TimeSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
        }

        private void TimeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            VLC.Time = (long)(VLC.Length * Position/1000 );
            timer.Start();
        }

        public void ChangePosition(double value)
        {
            timer.Stop();
            Position = value;
            VLC.Time = (long)(VLC.Length * Position / 1000);
            timer.Start();
        }
    }
}
