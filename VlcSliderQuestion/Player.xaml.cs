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
    public delegate void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e);


    /// <summary>
    /// Логика взаимодействия для Player.xaml
    /// </summary>
    public partial class Player : UserControl
    {


        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position",
          typeof(double), typeof(Player),
          new FrameworkPropertyMetadata(new PropertyChangedCallback(PositionPropertyChangedCallback)));

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration",
          typeof(TimeSpan), typeof(Player),
          new FrameworkPropertyMetadata(new PropertyChangedCallback(DurationPropertyChangedCallback)));

        public static readonly DependencyProperty CurTimeProperty = DependencyProperty.Register("CurTime",
            typeof(TimeSpan), typeof(Player),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(CurTimePropertyChangedCallback)));

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source",
            typeof(Uri), typeof(Player),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(SourcePropertyChangedCallback)));

        public double Position
        {
            get
            {
                return (double)GetValue(PositionProperty);
            }
            set {

                SetValue(PositionProperty, value);
            }
        }
        public TimeSpan Duration { get { return (TimeSpan)GetValue(DurationProperty); } set { SetValue(DurationProperty, value); } }
        public TimeSpan CurTime { get { return (TimeSpan)GetValue(CurTimeProperty); } set { SetValue(CurTimeProperty, value); } }
        public Uri Source { get { return (Uri)GetValue(SourceProperty); } set { SetValue(SourceProperty, value); } }

        public event PropertyChanged OnPositionChanged;
        public event PropertyChanged OnDurationChanged;
        public event PropertyChanged OnCurTimeChanged;
        public event PropertyChanged OnSourceChanged;

        static void PositionPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((Player)d).OnDurationChanged != null)
                ((Player)d).OnDurationChanged(d, e);
        }
        static void DurationPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((Player)d).OnDurationChanged != null)
                ((Player)d).OnDurationChanged(d, e);
        }
        static void CurTimePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((Player)d).OnCurTimeChanged != null)
                ((Player)d).OnCurTimeChanged(d, e);
        }
        static void SourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (((Player)d).OnSourceChanged != null)
                ((Player)d).OnSourceChanged(d, e);
        }


        System.Windows.Threading.DispatcherTimer timer;

        public Player() : base()
        {
            InitializeComponent();
            vlc.MediaPlayer.VlcLibDirectory = new System.IO.DirectoryInfo(@"c:\Program Files\VideoLAN\VLC\");
            vlc.MediaPlayer.EndInit();
            vlc.MediaPlayer.Play(new Uri(@"http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4"));

            OnPositionChanged += Player_OnPositionChanged;
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = TimeSpan.FromSeconds(0.1);
            timer.Start();
            Position = 50;
        }

        private void Player_OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!vlc.MediaPlayer.IsPlaying)
            {
                timer.Start();
                vlc.MediaPlayer.Play();
                vlc.MediaPlayer.Time = (long)Math.Round(Position * Duration.TotalMilliseconds / 1000);
            }
        }

        private void timerTick(object sender, EventArgs e)
        {
            if (vlc.MediaPlayer.Video != null)
            {
                Duration = TimeSpan.FromMilliseconds(vlc.MediaPlayer.Length);
                CurTime = TimeSpan.FromMilliseconds(vlc.MediaPlayer.Time);
        //        Position = 1000 * CurTime.TotalMilliseconds / Duration.TotalMilliseconds;
                TimeTextBox.Text = CurTime.ToString();
            }
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (vlc.MediaPlayer.IsPlaying) vlc.MediaPlayer.Pause();
            else vlc.MediaPlayer.Play();  
        }

        private void TimeSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            timer.Stop();
            vlc.MediaPlayer.Pause();
        }

        private void TimeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
//            timer.Start();
//{            vlc.MediaPlayer.Play();
//            vlc.MediaPlayer.Time = (long)Math.Round(Position * Duration.TotalMilliseconds / 1000);
        }


        //#region mvvm
        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        //}
        //#endregion
    }
}
