using System;
using System.IO;
using System.Collections.Generic;
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
using System.Windows.Interop;
using System.Threading;

namespace FlashDefender
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool HookStop = true;

        int WM_DEVICECHANGE = 537;
        IntPtr flashOut = (IntPtr)32772;
        IntPtr flashIn = (IntPtr)32768;

        public MainWindow()
        {
            InitializeComponent();
        }

        //protected override void WndProc(ref Message msg)
        //{
        //    int WM_DEVICECHANGE = 537;
        //    IntPtr flashOut = (IntPtr)32772;
        //    IntPtr flashIn = (IntPtr)32768;

        //    if (msg.Msg == WM_DEVICECHANGE)
        //    {
        //        //textBox1.AppendText("Изменение устройства " + msg.WParam + " " + msg.LParam + "\r\n");
        //        if (msg.WParam == flashIn)
        //            MessageBox.Show("Обнаружена флешка");
        //        if (msg.WParam == flashOut)
        //            MessageBox.Show("Флешка извлечена");
        //    }
        //    base.WndProc(ref msg);
        //}


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (HookStop)
                return IntPtr.Zero;


            if (msg == WM_DEVICECHANGE)
            {
                if (wParam == flashIn)
                {
                    string str = "Обнаружена флешка\n" + "msg=" + msg + " hwnd=" + hwnd +
                        " wParam=" + wParam + " lParam=" + lParam + " handled=" + handled;
                    listBoxLOG.Items.Add(str);

                    DriveInfo[] driveInfo = DriveInfo.GetDrives();
                    foreach (DriveInfo drive in driveInfo)
                    {
                        if (drive.DriveType == DriveType.Removable)
                        {
                            str = drive.Name + " очищается";
                            listBoxLOG.Items.Add(str);

                            DirectoryInfo directoryInfo = new DirectoryInfo(drive.RootDirectory.FullName);
                            //directoryInfo.Delete(true);
                            DirectoryInfo[] alldir = directoryInfo.GetDirectories();
                            FileInfo[] allfiles = directoryInfo.GetFiles();

                            foreach (FileInfo fInfo in allfiles)
                                fInfo.Delete();
                            foreach (DirectoryInfo directory in alldir)
                                if (directory.Name != "System Volume Information")
                                    directory.Delete(true);

                            str = drive.Name + " полностью очищен, всего наилучшего";
                            listBoxLOG.Items.Add(str);
                        }
                    }
                }
                if (wParam == flashOut)
                {
                    string str = "Флешка извлечена\n" + "msg=" + msg + " hwnd=" + hwnd +
                        " wParam=" + wParam + " lParam=" + lParam + " handled=" + handled;
                    listBoxLOG.Items.Add(str);
                }
            }

            return IntPtr.Zero;
        }



        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            listBoxLOG.Items.Clear();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            HookStop = false;
            btnStop.IsEnabled = true;
            btnStart.IsEnabled = false;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            HookStop = true;
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }
    }
}
