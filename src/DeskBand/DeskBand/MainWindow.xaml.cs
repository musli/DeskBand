using Modules.Translation;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Win32;

namespace DeskBand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //往左边偏移
        private int toLeftOffset = 0;

        private int custom_width = 135;

        public MainWindow()
        {
            InitializeComponent();
        }

        private IntPtr panelHandle = IntPtr.Zero;
        private IntPtr taskBarHandle = IntPtr.Zero;
        private IntPtr windowHandle = IntPtr.Zero;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //本窗口句柄
                windowHandle = new WindowInteropHelper(this).Handle;

                //处理alt+tab可以看见本程序
                int exStyle = User.GetWindowLong(windowHandle, User.GWL_EXSTYLE);
                exStyle |= User.WS_EX_TOOLWINDOW;
                User.SetWindowLong(windowHandle, User.GWL_EXSTYLE, exStyle);

                //查找任务栏容器
                var iconParentPtr = (IntPtr)User.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
                panelHandle = User.FindWindowEx(iconParentPtr, IntPtr.Zero, "ReBarWindow32", null);

                //获取任务栏宽高
                var size = GetWindowSize(panelHandle);
                //嵌入本窗体到任务栏容器
                User.SetParent(windowHandle, panelHandle);
                User.SetWindowPos(windowHandle, 0, (int)size.Width - custom_width - toLeftOffset, 0, custom_width, (int)size.Height, 0x0040);
                //User.SetWindowPos(windowHandle, (IntPtr)(0), 800, 0, 300, (int)size.Height, 0x0040);
                //设置任务栏变短一点，给本窗体留出空间显示
                taskBarHandle = User.FindWindowEx(panelHandle, IntPtr.Zero, "MSTaskSwWClass", null);
                User.SetWindowPos(taskBarHandle, 0, 0, 0, (int)size.Width - custom_width - toLeftOffset, (int)size.Height, 0x0040);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出错:" + ex.Message);
            }

            Task.Run(CheckUpdateWindowPosition);
            frame.Navigate(new MainPage());
        }

        /// <summary>
        /// 检查更新窗体位置
        /// </summary>
        public void CheckUpdateWindowPosition()
        {
            var oldSize = Size.Empty;
            while (true)
            {
                Thread.Sleep(500);

                var size = GetWindowSize(panelHandle);

                if (size.Width < size.Height)
                {
                    //任务栏为竖着的
                    User.SetWindowPos(windowHandle, 0, 0, (int)size.Height - 30, (int)size.Width, 30, 0x0040);
                    User.SetWindowPos(taskBarHandle, 0, 0, 0, (int)size.Width, (int)size.Height - 30, 0x0040);
                }
                else
                {
                    var newSize = new Size(size.Width - 300 - toLeftOffset, size.Height);
                    if (oldSize != newSize)//解决一直重新设置taskBarHandle窗口的位置会导致其他的任务栏软件闪的问题(其他任务栏软件应该有监控taskBarHandle大小改变事件，重新设置自己的布局导致的删)
                    {
                        Task.Run(() =>//解决用户拖动任务栏过快导致oldSize=newSize，布局还没刷新过来的问题
                        {
                            Thread.Sleep(500);
                            //任务栏为横着的
                            User.SetWindowPos(windowHandle, 0, (int)size.Width - custom_width - toLeftOffset, 0, custom_width, (int)size.Height, 0x0040);
                            User.SetWindowPos(taskBarHandle, 0, 0, 0, (int)size.Width - custom_width - toLeftOffset, (int)size.Height, 0x0040);
                            oldSize = newSize;
                        });
                    }
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

        /// <summary>
        /// 退出还原任务栏布局
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            var size = GetWindowSize(panelHandle);
            User.SetWindowPos(taskBarHandle, 0, 0, 0, (int)size.Width, (int)size.Height, 0x0040);
        }
    }
}