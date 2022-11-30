using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace AutoClick
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool ShallWeClick;
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private int StartDelay = 5; 


        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public MainWindow()
        {
            ShallWeClick = false;
            InitializeComponent();

        }

        private void ButtonAutoClick_Click(object sender, RoutedEventArgs e)
        {
            ShallWeClick = ShallWeClick ? false : true;
            Random random = new Random();
            Clicker(random.Next(0, 500), random.Next(0, 500)); 
            
        }

        private void Clicker(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
            Console.WriteLine("clicked!"); 
        }

        private void StartDelay_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            StartDelay = (int)e.NewValue;
        }



    }
}
