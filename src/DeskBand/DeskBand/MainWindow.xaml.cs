using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //本窗口句柄
                var help = new WindowInteropHelper(this);
                var curHandle = help.Handle;
                //处理alt+tab可以看见本程序
                int exStyle = User.GetWindowLong(curHandle, User.GWL_EXSTYLE);
                exStyle |= (int)Win32.User.WS_EX_TOOLWINDOW;
                User.SetWindowLong(curHandle, User.GWL_EXSTYLE, exStyle);

                //查找任务栏容器
                var iconParentPtr = (IntPtr)User.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
                iconParentPtr = (IntPtr)User.FindWindowEx(iconParentPtr, IntPtr.Zero, "ReBarWindow32", null);
                //获取任务栏宽高
                var rect = new RECT() { };
                User.GetWindowRect(iconParentPtr, ref rect);
                var height = rect.Bottom - rect.Top;
                var width = rect.Right - rect.Left;
                var parent = iconParentPtr;
                //嵌入本窗体到任务栏容器
                User.SetParent(curHandle, parent);
                User.SetWindowPos(curHandle, (IntPtr)(0), width - 90, 0, 90, height, 0x0040);
                //设置任务栏变短一点，给本窗体留出空间显示
                iconParentPtr = (IntPtr)User.FindWindowEx(iconParentPtr, IntPtr.Zero, "MSTaskSwWClass", null);
                User.SetWindowPos(iconParentPtr, (IntPtr)(0), 0, 0, width - 90, height, 0x0040);
            }
            catch (Exception ex)
            {
                MessageBox.Show("出错:" + ex.Message);
            }
        }
    }
}
