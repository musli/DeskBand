using CefSharp;
using CefSharp.Wpf;
using System;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Modules.Translation
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        #region Fields

        private WebClient client = new WebClient();

        #endregion Fields

        #region Methods

        public MainPage()
        {
            CefSettings _settings = new CefSettings();
            _settings.UserAgent = "tv.danmaku.bili/6250300 (Linux; U; Android 11; zh_CN; V1824A; Build/RP1A.200720.012; Cronet/81.0.4044.156)";
            Cef.Initialize(_settings);
            InitializeComponent();
            //支持中文输入，但是ime不能定位到光标位置，但是可以在popup里面输入中文
            //支持中文输入以及ime定位但是在popup里面失效，推测是popup没有实体句柄之类的
            //Browser.WpfKeyboardHandler = new WpfImeKeyboardHandler(Browser);
        }

        #endregion Methods

        #region Events

        /// <summary>
        /// 检测ctrl+v自动翻译
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSource_KeyUp(object sender, KeyEventArgs e)
        {
            //if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.V)
            //{
            //    CommandBindingTranslation_Executed(null, null);
            //}
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 跳转项目地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo
            {
                FileName = (sender as Hyperlink).NavigateUri.AbsoluteUri,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

        #endregion Events

        private static SolidColorBrush polygonBrush = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));

        /// <summary>
        /// 按钮点击开始涟漪动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;

            var time = 0.5d;
            var path = grid.Children[0] as Path;
            var ellipse = path.Data as EllipseGeometry;

            //设置圆心位置
            ellipse.Center = e.GetPosition(grid);
            //根据勾股定理计算涟漪最大长度
            var maxLength = Math.Sqrt(Math.Pow(grid.ActualWidth, 2) + Math.Pow(grid.ActualHeight, 2));
            //开始涟漪放缩动画
            ellipse.BeginAnimation(EllipseGeometry.RadiusXProperty, new DoubleAnimation(0, maxLength, new Duration(TimeSpan.FromSeconds(time))));
            //开始透明度消失动画
            path.BeginAnimation(Path.OpacityProperty, new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(time))));
        }
    }
}