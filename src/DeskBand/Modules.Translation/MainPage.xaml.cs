using DotNet.AccelerateBall;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Modules.Translation
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        #region Methods

        private static string currentPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); // System.Environment.CurrentDirectory;
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

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // 这里是你要执行的循环体代码
            cpu1test += 1;
            this.CPU1T.Content = "." + cpu1test;
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
            bgWorker.WorkerReportsProgress = true;
            string ip = txtIp;
            string user = txtUser;
            string password = txtPassword;

            string formatSensor = "-I lanplus -H {0} -U {1} -P {2} sensor";
            string parametersSensor = string.Format(formatSensor, ip, user, password);

            string fullExecuteSensor = ipmitoolPath + " " + parametersSensor;

            // 创建自定义的 RGB 颜色

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
                            cpu_flag++;
                            int CPU1_int = int.Parse(src[1].Substring(0, 3));
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Color customColorLow = Color.FromRgb(254, 254, 254);
                                Color customColorMid = Color.FromRgb(253, 213, 59);
                                Color customColorHig = Color.FromRgb(235, 158, 206);

                                // 创建对应的画刷对象
                                SolidColorBrush customBrushLow = new SolidColorBrush(customColorLow);
                                SolidColorBrush customBrushMid = new SolidColorBrush(customColorMid);
                                SolidColorBrush customBrushHig = new SolidColorBrush(customColorHig);
                                CPU1T.Content = CPU1_int + "c";
                                if (CPU1_int >= 85)
                                {
                                    this.CPU1T.Foreground = customBrushHig;
                                }
                                else if (CPU1_int >= 60)
                                {
                                    this.CPU1T.Foreground = customBrushMid;
                                }
                                else
                                {
                                    this.CPU1T.Foreground = customBrushLow;
                                }
                            });
                            continue;
                        }
                        if (cpu_flag == 2 && temp[0].StartsWith("Temp"))
                        {
                            int CPU2_int = int.Parse(src[1].Substring(0, 3));
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Color customColorLow = Color.FromRgb(254, 254, 254);
                                Color customColorMid = Color.FromRgb(253, 213, 59);
                                Color customColorHig = Color.FromRgb(235, 158, 206);

                                // 创建对应的画刷对象
                                SolidColorBrush customBrushLow = new SolidColorBrush(customColorLow);
                                SolidColorBrush customBrushMid = new SolidColorBrush(customColorMid);
                                SolidColorBrush customBrushHig = new SolidColorBrush(customColorHig);

                                CPU2T.Content = CPU2_int + "c";
                                if (CPU2_int >= 85)
                                {
                                    this.CPU2T.Foreground = customBrushHig;
                                }
                                else if (CPU2_int >= 60)
                                {
                                    this.CPU2T.Foreground = customBrushMid;
                                }
                                else
                                {
                                    this.CPU2T.Foreground = customBrushLow;
                                }
                            });
                            continue;
                        }
                        if (temp[0].StartsWith("CPU Usage"))
                        {
                            string[] usage_list = src[1].Split('.');
                            int usage_int = int.Parse(usage_list[0]);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Color customColorLow = Color.FromRgb(254, 254, 254);
                                Color customColorMid = Color.FromRgb(253, 213, 59);
                                Color customColorHig = Color.FromRgb(235, 158, 206);

                                // 创建对应的画刷对象
                                SolidColorBrush customBrushLow = new SolidColorBrush(customColorLow);
                                SolidColorBrush customBrushMid = new SolidColorBrush(customColorMid);
                                SolidColorBrush customBrushHig = new SolidColorBrush(customColorHig);

                                CpuUsage.Content = usage_int + "%";
                                if (usage_int >= 90)
                                {
                                    this.CpuUsage.Foreground = customBrushHig;
                                }
                                else
                                {
                                    this.CpuUsage.Foreground = customBrushLow;
                                }
                            });
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
    }
}