/*===================================================
* 类名称: DispatcherUIThread
* 类描述: 
* 创建人: musli
* 创建时间: 2020/9/29 11:27:53
* 修改人: 
* 修改时间:
* 版本： @version 1.0
=====================================================*/
using System;
using System.Collections.Generic;
using System.Printing;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace Modules.Translation.Custom
{
    /// <summary>
    /// 子UI线程
    /// </summary>
    public class ChildUIThread : IDisposable
    {
        /// <summary>
        /// 子UI线程
        /// </summary>
        private static Thread childThread;
        /// <summary>
        /// 子UI线程调度器
        /// </summary>
        private static Dispatcher childDispatcher;

        /// <summary>
        /// 线程同步等待事件 (表示线程同步事件在一个等待线程释放后收到信号时自动重置。)
        /// </summary>
        private AutoResetEvent threadResetEvnt;
        /// <summary>
        /// 当前实例调度器
        /// </summary>
        public Dispatcher Dispatcher { get; private set; }

        /// <summary>
        /// 开始一个新的UI线程调度器
        /// </summary>
        public void Start()
        {
            //只运行一条UI线程
            if (childThread == null)
            {
                threadResetEvnt = new AutoResetEvent(false);
                childThread = new Thread(InternalRunning);
                childThread.Priority = ThreadPriority.Highest;
                childThread.IsBackground = true;
                childThread.SetApartmentState(ApartmentState.STA);
                childThread.Start();
                threadResetEvnt.WaitOne();
                threadResetEvnt.Dispose();
                threadResetEvnt = null;
            }
            Dispatcher = childDispatcher;
        }
        /// <summary>
        /// 关闭当前调度器
        /// </summary>
        public void Close()
        {
            if (Dispatcher != null)
                Dispatcher.InvokeShutdown();
        }
        /// <summary>
        /// 在异步UI线程里面获取调度器
        /// </summary>
        [SecurityCritical]
        private void InternalRunning()
        {
            childDispatcher = Dispatcher.CurrentDispatcher;
            threadResetEvnt.Set();
            Dispatcher.Run();
        }

        /// <summary>
        /// 释放本类所占用的资源
        /// </summary>
        public void Dispose()
        {
            if (Dispatcher != null)
            {
                this.Close();
                Dispatcher = null;
            }
        }
    }
}
