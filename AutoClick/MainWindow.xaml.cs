using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoClick
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private int StartDelay, PerSecond, Duration;
        private Button MainButton;
        private readonly BackgroundWorker worker; 

        /// <summary>
        /// Struct representing a point.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        /// <summary>
        /// Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public MainWindow()
        {
            StartDelay = 0;
            PerSecond = 0;
            Duration = 0;
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.ProgressChanged += worker_ProgressChanged;

        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                Console.WriteLine("Escape Pressed!");
                if (worker.WorkerSupportsCancellation)
                {
                    worker.CancelAsync();
                    UpdateButtonText("AutoClick!");
                }
            }
        }

        private void ButtonAutoClick_Click(object sender, RoutedEventArgs e)
        {
            //Execute Clicker
            Console.WriteLine("Test!");
            if(MainButton == null)
            {
                if (sender.GetType() == typeof(Button))
                    MainButton = (Button)sender;                
            }

            if(!worker.IsBusy)
            {
                worker.RunWorkerAsync();
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {

            for (int delay = StartDelay; delay > 0; delay--)
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }
                worker.ReportProgress((int)((StartDelay - delay / StartDelay) * 100), $"delay: {delay}");
                System.Threading.Thread.Sleep(1000);
            }

            int timeBetweenClicks = (int)(1000 / PerSecond);
            int duration = Duration * 1000;

            worker.ReportProgress(1, "Clicking!");

            for (int totalTime=0; totalTime < duration;) 
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }
                //Do click
                Clicker(); 

                //Sleep Until Next Iteration
                totalTime+= timeBetweenClicks;
                System.Threading.Thread.Sleep(timeBetweenClicks);
            }
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateButtonText(e.UserState.ToString());
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            UpdateButtonText("AutoClick!");
        }


        private void UpdateButtonText(string text)
        {
            MainButton.Content= text;
            MainButton.UpdateLayout();
        }

        private void MoveMouseToPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        private POINT GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            return lpPoint;
        }

        private void Clicker()
        {
            POINT mouseLocation = GetCursorPosition();     
            mouse_event(MOUSEEVENTF_LEFTDOWN, mouseLocation.X, mouseLocation.Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, mouseLocation.X, mouseLocation.Y, 0, 0);
        }

        private void StartDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            StartDelay = (int)e.NewValue;
        }

        private void PerSecond_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            PerSecond = (int)e.NewValue;
        }

        private void Duration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Duration = (int)e.NewValue;
        }

    }
}
