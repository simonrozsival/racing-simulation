using System;
using System.Windows;

namespace Racing.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += onLoaded;
        }

        private void onLoaded(object sender, RoutedEventArgs e)
        {
            var random = new Random();

            var trackDefinitionPath = "./tracks/simple-circuit";
            var svgFileName = $"{trackDefinitionPath}/visualization.svg";
            var jsonFileName = $"{trackDefinitionPath}/track_definition.json";

            showSvgTrack(svgFileName);

            var track = TrackLoader.Load(jsonFileName);
            var agent = new RandomAgent(random);
        }

        private void showSvgTrack(string svgFileName)
        {
            svgViewBox.Source = new Uri(svgFileName, UriKind.Relative);
            maximizeDisplayedSize();
        }

        private void maximizeDisplayedSize()
        {
            var aspectRatio = svgViewBox.Width / svgViewBox.Height;
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            if (window.ActualWidth / window.ActualHeight > aspectRatio)
            {
                svgViewBox.Height = window.ActualHeight;
                svgViewBox.Width = window.ActualHeight * aspectRatio;
            }
            else
            {
                svgViewBox.Width = window.ActualWidth;
                svgViewBox.Height = window.ActualWidth / aspectRatio;
            }
            window.ResizeMode = ResizeMode.NoResize;
        }
    }
}
