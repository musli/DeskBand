/*===================================================
* 类名称: AsyncCanvas
* 类描述: 
* 创建人: musli
* 创建时间: 2020/9/29 11:28:06
* 修改人: 
* 修改时间:
* 版本： @version 1.0
=====================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Modules.Translation.Custom
{
    /// <summary>
    /// 异步UI线程画布
    /// </summary>
    public class ChildUICanvas : UIElement
    {
        /// <summary>
        /// 连接UI线程的主线程宿主容器
        /// </summary>
        private HostVisual hostVisual;
        /// <summary>
        /// 虚拟容器
        /// </summary>
        private ContainerVisual visualContainer;
        /// <summary>
        /// 命中测试矩形
        /// </summary>
        private Rect hitTestRect;
        public Rect FinalRect { get; private set; }
        public ChildUICanvas()
        {
            hostVisual = new HostVisual();
            var thread = new ChildUIThread();
            thread.Start();
            thread.Dispatcher.Invoke(() =>
            {
                visualContainer = new ContainerVisual();
                //连接主线程的UI
                var tempTarget = new VisualTarget(hostVisual);
                //连接子UI线程的UI
                tempTarget.RootVisual = visualContainer;
            });
            //将主线程宿主容器添加到视觉树
            this.AddVisualChild(hostVisual);
        }
        /// <summary>
        /// 获取UI线程的调度器，用于在主线程委托操作UI线程创建的控件
        /// </summary>
        public Dispatcher UIDispatcher
        {
            get
            {
                return visualContainer.Dispatcher;
            }
        }
        /// <summary>
        /// 获取子元素
        /// </summary>
        public VisualCollection Children
        {
            get
            {
                return visualContainer.Children;
            }
        }
        /// <summary>
        /// 重写命中测试，让UI线程的UI能够响应基础输入
        /// </summary>
        /// <param name="hitTestParameters"></param>
        /// <returns></returns>
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            if (hitTestRect.Contains(hitTestParameters.HitPoint))
                return new PointHitTestResult(this, hitTestParameters.HitPoint);
            return base.HitTestCore(hitTestParameters);
        }
        /// <summary>
        /// 重写布局系统，让UI线程的UI正常布局 (在派生类中重写时，提供测量逻辑来适当地调整此元素的大小，兼顾任何子元素内容的大小。)
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>
        protected override Size MeasureCore(Size availableSize)
        {
            if (availableSize.Height > 0 && availableSize.Width > 0 && !(double.IsInfinity(availableSize.Height) || double.IsInfinity(availableSize.Width)))
                return availableSize;
            return new Size(200, 200);
        }
        /// <summary>
        /// 重写布局系统，让多线程UI元素能正常布局(定义 WPF 核心级别排列布局定义的模板。)
        /// </summary>
        /// <param name="finalRect"></param>
        protected override void ArrangeCore(Rect finalRect)
        {
            base.ArrangeCore(finalRect);
            base.RenderSize = finalRect.Size;
            hitTestRect = finalRect;
            FinalRect = finalRect;
        }

        /// <summary>
        /// 指定 Visual 的子元素数量。
        /// </summary>
        protected override int VisualChildrenCount => 1;
        /// <summary>
        /// 指定需要渲染的Visual对象
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override Visual GetVisualChild(int index)
        {
            return hostVisual;
        }
    }
}
