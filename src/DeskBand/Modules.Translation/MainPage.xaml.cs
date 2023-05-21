using DotNet.AccelerateBall;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Modules.Translation
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        #region Methods

        private static string currentPath = System.Reflection.Assembly.GetExecutingAssembly().Location; // System.Environment.CurrentDirectory;
        private static string configFileName = "\\config.ini";
        private static string ipmitoolPath = currentPath + "\\ipmitool.exe";
        private static string configFilePath = currentPath + configFileName;

        private static string defaultIp = "192.168.1.100";
        private static string defaultUser = "root";
        private static string defaultPassword = "calvin";
        private static string defaultConfigSection = "ipmi";
        private static int cpu1test = 1;

        private string txtIp;
        private string txtUser;
        private string txtPassword;
        private static System.Timers.Timer timer;

        public MainPage()
        {
            InitializeComponent();
            initParameter();
            StartMonitor(); //初始化网络流量监控
            //支持中文输入，但是ime不能定位到光标位置，但是可以在popup里面输入中文
            //支持中文输入以及ime定位但是在popup里面失效，推测是popup没有实体句柄之类的
            //Browser.WpfKeyboardHandler = new WpfImeKeyboardHandler(Browser);
        }

        #endregion Methods

        public void initParameter()
        {
            if (File.Exists(configFilePath))
            {
                string ip = IniHelper.Read(defaultConfigSection, "ip", defaultIp, configFilePath);
                string user = IniHelper.Read(defaultConfigSection, "user", defaultUser, configFilePath);
                string password = IniHelper.Read(defaultConfigSection, "password", defaultPassword, configFilePath);
                txtIp = ip;
                txtUser = user;
                txtPassword = password;
            }
            else
            {
                IniHelper.Write(defaultConfigSection, "ip", defaultIp, configFilePath);
                IniHelper.Write(defaultConfigSection, "user", defaultUser, configFilePath);
                IniHelper.Write(defaultConfigSection, "password", defaultPassword, configFilePath);
                txtIp = defaultIp;
                txtUser = defaultUser;
                txtPassword = defaultPassword;
            }
        }

        private static string execute(string parameter)
        {
            Process process = null;
            string result = string.Empty;
            try
            {
                process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();

                process.StandardInput.WriteLine(parameter + "& exit");
                process.StandardInput.AutoFlush = true;
                result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ExceptionOccurred:{ 0},{ 1}", ex.Message, ex.StackTrace.ToString());
                return null;
            }
        }

        /*开始监控*/

        private void StartMonitor()
        {
            timer = new System.Timers.Timer(1000);
            // 设置定时器触发事件的处理方法
            timer.Elapsed += background_FetchStates_DoWork;

            // 设置定时器为可重复触发
            timer.AutoReset = false;

            // 启动定时器
            timer.Start();
        }

        private void background_FetchStates_DoWork(object sender, ElapsedEventArgs e)
        {
            BackgroundWorker bgWorker = new BackgroundWorker();
            string ip = txtIp;
            string user = txtUser;
            string password = txtPassword;

            string formatSensor = "-I lanplus -H {0} -U {1} -P {2} sensor";
            string parametersSensor = string.Format(formatSensor, ip, user, password);

            string fullExecuteSensor = ipmitoolPath + " " + parametersSensor;

            bgWorker.WorkerReportsProgress = true;
            while (true)
            {
                bgWorker.ReportProgress(0, "start");

                string result = execute(fullExecuteSensor);

                result = result.Replace("\r\n", "\n");
                string[] sensorList = result.Split('\n', '\r');

                //this.CPU1.Text = "." + cpu1test;
                int cpu_flag = 1;
                foreach (var item in sensorList)
                {
                    if (item.Contains("Temp") || item.Contains("CPU Usage"))
                    {
                        string[] temp = new string[8];
                        var src = item.Split('|');
                        temp[0] = src[0];
                        temp[1] = src[1];
                        if (cpu_flag == 1 && temp[0].StartsWith("Temp"))
                        {
                            this.CPU1T.Content = src[1].Substring(0, 3);
                            cpu_flag++;
                            int CPU1_int = int.Parse(src[1].Substring(0, 3));
                            //if (CPU1_int >= 85)
                            //{
                            //    this.CPU1.ForeColor = System.Drawing.Color.FromArgb(235, 158, 206);
                            //}
                            //else if (CPU1_int >= 60)
                            //{
                            //    this.CPU1.ForeColor = System.Drawing.Color.FromArgb(253, 213, 59);
                            //}
                            //else
                            //{
                            //    this.CPU1.ForeColor = System.Drawing.Color.FromArgb(254, 254, 254);
                            //}
                            continue;
                        }
                        if (cpu_flag == 2 && temp[0].StartsWith("Temp"))
                        {
                            this.CPU2T.Content = src[1].Substring(0, 3);
                            int CPU2_int = int.Parse(src[1].Substring(0, 3));
                            //if (CPU2_int >= 85)
                            //{
                            //    this.CPU2.ForeColor = System.Drawing.Color.FromArgb(235, 158, 206);
                            //}
                            //else if (CPU2_int >= 60)
                            //{
                            //    this.CPU2.ForeColor = System.Drawing.Color.FromArgb(253, 213, 59);
                            //}
                            //else
                            //{
                            //    this.CPU2.ForeColor = System.Drawing.Color.FromArgb(254, 254, 254);
                            //}
                            continue;
                        }
                        if (temp[0].StartsWith("CPU Usage"))
                        {
                            string[] usage_list = src[1].Split('.');
                            this.CpuUsage.Content = usage_list[0];
                            //int usage_int = int.Parse(usage_list[0]);
                            //if (usage_int >= 90)
                            //{
                            //    this.CpuUsage.ForeColor = System.Drawing.Color.FromArgb(212, 59, 26);
                            //}
                            //else
                            //{
                            //    this.CpuUsage.ForeColor = System.Drawing.Color.FromArgb(254, 254, 254);
                            //}
                        }
                        //lstViewSensor.Items.Add(new ListViewItem(temp));
                        bgWorker.ReportProgress(1, temp);
                    }
                }
                bgWorker.ReportProgress(100, "completed");
                // 停止后台任务
            }
            // 释放资源
            bgWorker.Dispose();
        }

        #region Events

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion Events

        private static SolidColorBrush polygonBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(128, 255, 255, 255));

        /// <summary>
        /// 按钮点击开始涟漪动画
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;

            var time = 0.5d;
            var path = grid.Children[0] as System.Windows.Shapes.Path;
            var ellipse = path.Data as EllipseGeometry;

            //设置圆心位置
            ellipse.Center = e.GetPosition(grid);
            //根据勾股定理计算涟漪最大长度
            var maxLength = Math.Sqrt(Math.Pow(grid.ActualWidth, 2) + Math.Pow(grid.ActualHeight, 2));
            //开始涟漪放缩动画
            ellipse.BeginAnimation(EllipseGeometry.RadiusXProperty, new DoubleAnimation(0, maxLength, new Duration(TimeSpan.FromSeconds(time))));
            //开始透明度消失动画
            path.BeginAnimation(System.Windows.Shapes.Path.OpacityProperty, new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(time))));
        }
    }
}