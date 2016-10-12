using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace SoundAndVideo
{
    /// <summary>
    /// Interaction logic for AnimatedVideoWindow.xaml
    /// </summary>

    public partial class AnimatedVideoWindow : System.Windows.Window
    {

        public AnimatedVideoWindow()
        {
            InitializeComponent();
        }

        private void cmdPlayCode_Click(object sender, RoutedEventArgs e)
        {
            // Create the timeline.
            // This isn't required, but it allows you to configure details
            // that wouldn't otherwise be possible (like repetition).
            MediaTimeline timeline = new MediaTimeline(new Uri("test.mpg", UriKind.Relative));            
            timeline.RepeatBehavior = RepeatBehavior.Forever;

            // Create the clock, which is shared with the MediaPlayer.
            MediaClock clock = timeline.CreateClock();
            MediaPlayer player = new MediaPlayer();
            player.Clock = clock;

            // Create the VideoDrawing.
            VideoDrawing videoDrawing = new VideoDrawing();
            videoDrawing.Rect = new Rect(150, 0, 100, 100);
            videoDrawing.Player = player;

            // Assign the DrawingBrush.
            DrawingBrush brush = new DrawingBrush(videoDrawing);
            this.Background = brush;

            // Start the timeline.
            clock.Controller.Begin();


        }
    }
}