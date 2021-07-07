using CefSharp;
using CefSharp.Wpf;
using CefSharp.Wpf.Internals;
using Modules.Translation.Custom;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
        WebClient client = new WebClient();
        #endregion

        #region Methods
        public MainPage()
        {
            CefSettings _settings = new CefSettings();
            _settings.UserAgent = "tv.danmaku.bili/6250300 (Linux; U; Android 11; zh_CN; V1824A; Build/RP1A.200720.012; Cronet/81.0.4044.156)";
            Cef.Initialize(_settings);
            InitializeComponent();
            //支持中文输入，但是ime不能定位到光标位置，但是可以在popup里面输入中文
            Browser.WpfKeyboardHandler = new WpfKeyboardHandler(Browser);
            //支持中文输入以及ime定位但是在popup里面失效，推测是popup没有实体句柄之类的
            //Browser.WpfKeyboardHandler = new WpfImeKeyboardHandler(Browser);
        }

        #endregion

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
        /// <summary>
        /// 鼠标离开的时候隐藏popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void popBro_MouseLeave(object sender, MouseEventArgs e)
        {
            if (cheHide.IsChecked.Value == true)
                popBro.SetCurrentValue(Popup.IsOpenProperty, false);
        }
        #endregion

        #region Commands
        /// <summary>
        /// 翻译命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingTranslation_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSource.Text))
                return;
            Task.Run(() =>
            {
                string msg = string.Empty;
                string lan = string.Empty;
                Dispatcher.Invoke(() =>
                {
                    msg = txtSource.Text;
                    lan = togLanguage.IsChecked.Value ? "en" : "zh-Hans";
                });
                try
                {
                    //zh-Hans
                    var url = "https://cn.bing.com/ttranslatev3?isVertical=1&&IG=FA5F953F30714718B7F4C1DB9C9EFFBC&IID=translator.5024.1";
                    var data = Encoding.UTF8.GetBytes($"&fromLang=auto-detect&text={msg}&to={lan}");
                    client.Headers.Add("Content-type", "application/x-www-form-urlencoded");
                    var result = client.UploadData(url, data);
                    var json = Encoding.UTF8.GetString(result);
                    var temp = JsonDocument.Parse(json);
                    msg = temp.RootElement.EnumerateArray().First().GetProperty("translations").EnumerateArray().First().GetProperty("text").ToString();
                }
                catch (Exception ex)
                {
                    msg = ex.Message;
                }

                Dispatcher.Invoke(() =>
                {
                    txtResult.Text = msg;
                    txtResult.SelectAll();
                    txtResult.Focus();

                });
            });
        }
        /// <summary>
        /// 格式化命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingFormat_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSource.Text))
                return;
            try
            {
                var buffer = new ArrayBufferWriter<byte>();
                var options = new JsonWriterOptions
                {
                    Indented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                };

                using var json = new Utf8JsonWriter(buffer, options);
                JsonDocument.Parse(txtSource.Text).WriteTo(json);
                json.Flush();
                txtResult.Text = Encoding.UTF8.GetString(buffer.WrittenSpan.ToArray());
                txtResult.SelectAll();
                txtResult.Focus();
            }
            catch (Exception ex)
            {
                txtResult.Text = ex.Message;
            }
        }
        /// <summary>
        /// 选择所有文本命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingSelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            txtSource.SelectAll();
        }
        /// <summary>
        /// Run命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingGo_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSource.Text))
                return;
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = txtSource.Text,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 浏览命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingBrowser_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Browser.Address = txtAddress.Text;
        }
        #endregion

        static SolidColorBrush polygonBrush = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));
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
