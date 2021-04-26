using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using System.Text.Json;
using System.Buffers;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace Modules.Translation
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }
        WebClient client = new WebClient();
        /// <summary>
        /// 回车执行翻译
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Task.Run(() =>
            {
                string msg = string.Empty;
                Dispatcher.Invoke(() =>
                {
                    msg = txtSource.Text;
                });
                try
                {
                    var url = "https://cn.bing.com/ttranslatev3?isVertical=1&&IG=FA5F953F30714718B7F4C1DB9C9EFFBC&IID=translator.5024.1";
                    var data = Encoding.UTF8.GetBytes($"&fromLang=auto-detect&text={msg}&to=zh-Hans");
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
                    txtSource.Text += msg;
                    txtSource.Select(txtSource.Text.Length - msg.Length, msg.Length);

                });
            });
        }
        /// <summary>
        /// 双击格式化json
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingFormat_Executed(object sender, ExecutedRoutedEventArgs e)
        {

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
                txtSource.Text = Encoding.UTF8.GetString(buffer.WrittenSpan.ToArray());
                txtSource.SelectAll();
            }
            catch (Exception)
            {
            }
        }

    }
}
