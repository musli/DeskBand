using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Win32;

namespace DeskBand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hello");
        }

        IntPtr panelHandle = IntPtr.Zero;
        IntPtr taskBarHandle = IntPtr.Zero;
        IntPtr windowHandle = IntPtr.Zero;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //本窗口句柄
                windowHandle = new WindowInteropHelper(this).Handle;

                //处理alt+tab可以看见本程序
                int exStyle = User.GetWindowLong(windowHandle, User.GWL_EXSTYLE);
                exStyle |= (int)Win32.User.WS_EX_TOOLWINDOW;
                User.SetWindowLong(windowHandle, User.GWL_EXSTYLE, exStyle);

                //查找任务栏容器
                var iconParentPtr = (IntPtr)User.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
                panelHandle = (IntPtr)User.FindWindowEx(iconParentPtr, IntPtr.Zero, "ReBarWindow32", null);

                //获取任务栏宽高
                var size = GetWindowSize(panelHandle);
                //嵌入本窗体到任务栏容器
                User.SetParent(windowHandle, panelHandle);
                User.SetWindowPos(windowHandle, (IntPtr)(0), (int)size.Width - 90, 0, 90, (int)size.Height, 0x0040);
                //设置任务栏变短一点，给本窗体留出空间显示
                taskBarHandle = (IntPtr)User.FindWindowEx(iconParentPtr, IntPtr.Zero, "MSTaskSwWClass", null);
                User.SetWindowPos(taskBarHandle, (IntPtr)(0), 0, 0, (int)size.Width - 90, (int)size.Height, 0x0040);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出错:" + ex.Message);
            }

            Task.Run(CheckUpdateWindowPosition);
        }
        /// <summary>
        /// 检查更新窗体位置
        /// </summary>
        public void CheckUpdateWindowPosition()
        {

            while (true)
            {
                Thread.Sleep(500);

                var size = GetWindowSize(panelHandle);

                if (size.Width < size.Height)
                {
                    //任务栏为竖着的
                    User.SetWindowPos(windowHandle, (IntPtr)(0), 0, (int)size.Height - 30, (int)size.Width, 30, 0x0040);
                    User.SetWindowPos(taskBarHandle, (IntPtr)(0), 0, 0, (int)size.Width, (int)size.Height - 30, 0x0040);
                }
                else
                {
                    //任务栏为横着的
                    User.SetWindowPos(windowHandle, (IntPtr)(0), (int)size.Width - 90, 0, 90, (int)size.Height, 0x0040);
                    User.SetWindowPos(taskBarHandle, (IntPtr)(0), 0, 0, (int)size.Width - 90, (int)size.Height, 0x0040);

                }
            }
        }
        /// <summary>
        /// 获取窗体大小
        /// </summary>
        /// <param name="intPtr"></param>
        /// <returns></returns>
        private Size GetWindowSize(IntPtr intPtr)
        {
            //获取任务栏宽高
            var rect = new RECT() { };
            User.GetWindowRect(panelHandle, ref rect);
            return new Size { Width = rect.Right - rect.Left, Height = rect.Bottom - rect.Top };
        }
    }
}
