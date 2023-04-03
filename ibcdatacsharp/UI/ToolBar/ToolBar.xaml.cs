using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ibcdatacsharp.UI.Common;
using System.Windows.Media.Animation;
using ibcdatacsharp.UI.Filters;
using ibcdatacsharp.UI.SagitalAngles;
using System.Diagnostics;
using ibcdatacsharp.DeviceList.TreeClasses;

namespace ibcdatacsharp.UI.ToolBar
{
    /// <summary>
    /// Lógica de interacción para ToolBar.xaml
    /// </summary>
    public partial class ToolBar : Page
    {
        private const int ANIMATION_MS = 100;
        private const int INITIAL_ICON_SIZE = 30;
        private const int PRESSED_ICON_SIZE = 25;
        private const int INITIAL_FONT_SIZE = 11;
        private const int PRESSED_FONT_SIZE = 9;

        private DeviceList.DeviceList deviceList;
        public delegate void rotationSequenceEventHandler(object sender, Utils.RotSeq rotSeq);
        public rotationSequenceEventHandler rotationSequenceEvent;

        public ToolBar()
        {
            InitializeComponent();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            DataContext = mainWindow.virtualToolBar.properties;
            FilterManager filterManager = mainWindow.filterManager;
            if(mainWindow.deviceList.Content == null)
            {
                mainWindow.deviceList.Loaded += (s, e) =>
                {
                    deviceList = mainWindow.deviceList.Content as DeviceList.DeviceList;
                };
            }
            else
            {
                deviceList = mainWindow.deviceList.Content as DeviceList.DeviceList;
            }
            filters.ItemsSource = filterManager.filters;
            filters.SelectionChanged += (object sender, SelectionChangedEventArgs e) =>
            {
                Filter selected = (sender as ComboBox).SelectedItem as Filter;
                filterManager.changeFilter(selected);
            };
            filters.SelectedIndex = 0;
        }
        private void initButtonAnimations()
        {
            void buttonPressed(object sender, RoutedEventArgs e)
            {
                Button button = (Button)sender;
                if (button.IsEnabled)
                {
                    StackPanel stackPanel = Helpers.GetChildOfType<StackPanel>(button);
                    Image image = Helpers.GetChildOfType<Image>(stackPanel);
                    TextBlock text = Helpers.GetChildOfType<TextBlock>(stackPanel);

                    DoubleAnimation imageAnimation = new DoubleAnimation();

                    imageAnimation.From = INITIAL_ICON_SIZE;
                    imageAnimation.To = PRESSED_ICON_SIZE;
                    imageAnimation.Duration = TimeSpan.FromMilliseconds(ANIMATION_MS);

                    DoubleAnimation textAnimation = new DoubleAnimation();

                    textAnimation.From = INITIAL_FONT_SIZE;
                    textAnimation.To = PRESSED_FONT_SIZE;
                    textAnimation.Duration = TimeSpan.FromMilliseconds(ANIMATION_MS);

                    image.BeginAnimation(Image.WidthProperty, imageAnimation);
                    text.BeginAnimation(TextBlock.FontSizeProperty, textAnimation);
                }
            }
            void buttonReleased(object sender, RoutedEventArgs e)
            {
                Button button = (Button)sender;
                if (button.IsEnabled)
                {
                    StackPanel stackPanel = Helpers.GetChildOfType<StackPanel>(button);
                    Image image = Helpers.GetChildOfType<Image>(stackPanel);
                    TextBlock text = Helpers.GetChildOfType<TextBlock>(stackPanel);

                    DoubleAnimation imageAnimation = new DoubleAnimation();

                    imageAnimation.From = PRESSED_ICON_SIZE;
                    imageAnimation.To = INITIAL_ICON_SIZE;
                    imageAnimation.Duration = TimeSpan.FromMilliseconds(ANIMATION_MS);

                    DoubleAnimation textAnimation = new DoubleAnimation();

                    textAnimation.From = PRESSED_FONT_SIZE;
                    textAnimation.To = INITIAL_FONT_SIZE;
                    textAnimation.Duration = TimeSpan.FromMilliseconds(ANIMATION_MS);

                    image.BeginAnimation(Image.WidthProperty, imageAnimation);
                    text.BeginAnimation(TextBlock.FontSizeProperty, textAnimation);
                }
            }
            List<Button> buttons = new List<Button>();
            buttons.Add(scan);
            buttons.Add(connect);
            buttons.Add(disconnect);
            buttons.Add(openCamera);
            buttons.Add(record);
            buttons.Add(capture);
            buttons.Add(pause);
            buttons.Add(stop);
            buttons.Add(capturedFiles);
            foreach (Button button in buttons)
            {
                button.PreviewMouseLeftButtonDown += buttonPressed;
                button.PreviewMouseLeftButtonUp += buttonReleased;
            }
        }
        private void deactivateButtons()
        {
            connect.IsEnabled = false;
            disconnect.IsEnabled = false;
            openCamera.IsEnabled = false;
            record.IsEnabled = false;
            capture.IsEnabled = false;
            pause.IsEnabled = false;
            stop.IsEnabled = false;
            //capturedFiles.IsEnabled = true;
        }
        public void activateButtons()
        {
            connect.IsEnabled = true;
            disconnect.IsEnabled = true;
            openCamera.IsEnabled = true;
            record.IsEnabled = true;
            capture.IsEnabled = true;
            pause.IsEnabled = true;
            stop.IsEnabled = true;
            //capturedFiles.IsEnabled = true;
        }
        // Cambia el icono del boton Pause
        public void changePauseState(PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/Blue/pause-blue-icon.png"));
                pauseText.Text = "Pause";
            }
            else if (pauseState == PauseState.Play)
            {
                pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/Blue/play-pause-blue-icon.png"));
                pauseText.Text = "Play";
            }
        }
        // Cambia el icono del boton Record
        public void changeRecordState(RecordState recordState)
        {
            if (recordState == RecordState.RecordStopped)
            {
                recordImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/Blue/record-stop-blue-icon.png"));
                recordText.Text = "Record Stopped";
            }
            else if (recordState == RecordState.Recording)
            {
                recordImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/record-recording-icon.png"));
                recordText.Text = "Recording...";
            }
        }

        private void rotationAccept(object sender, RoutedEventArgs e)
        {
            try
            {
                if(deviceList.numIMUsUsed != 2)
                {
                    Trace.WriteLine("Debes seleccionar 2 IMUs, has seleccionado " + deviceList.numIMUsUsed);
                    return;
                }
                if (!IMUInfo.allRotationJointsUsed())
                {
                    Trace.WriteLine("Debes usar ambas joints (qbase y qmob)");
                    return;
                }
                Utils.RotSeq rotSeq = (Utils.RotSeq)rotationSeq.SelectedValue;
                rotationSequenceEvent?.Invoke(this, rotSeq);
            }
            catch (NullReferenceException)
            {
                Trace.WriteLine("selecciona un modo de rotacion");
            }
        }
    }
}
