using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Shapes;

namespace Modules.Translation
{
    /// <summary>
    /// Attach  property for control
    /// </summary>
    public class ControlAttach
    {
        #region 按钮附加图标
        public static UIElement GetIcon(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(IconProperty);
        }

        public static void SetIcon(DependencyObject obj, UIElement value)
        {
            obj.SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(UIElement), typeof(ControlAttach), new FrameworkPropertyMetadata()
            {
                DefaultValue=null,
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger=UpdateSourceTrigger.PropertyChanged
            });

        #endregion

        #region 鼠标悬停时显示的内容
        public static UIElement GetMouseOverContent(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(MouseOverContentProperty);
        }

        public static void SetMouseOverContent(DependencyObject obj, UIElement value)
        {
            obj.SetValue(MouseOverContentProperty, value);
        }
        
        // Using a DependencyProperty as the backing store for MouseOverContent.  This enables animation, styling, binding, etc...fdfsd
        public static readonly DependencyProperty MouseOverContentProperty =
            DependencyProperty.RegisterAttached("MouseOverContent", typeof(UIElement), typeof(ControlAttach),new FrameworkPropertyMetadata() { DefaultValue=null,BindsTwoWayByDefault=true});
        #endregion


        public static Brush GetHeaderBackground(DependencyObject obj)
        {
            return (Brush)obj.GetValue(HeaderBackgroundProperty);
        }

        public static void SetHeaderBackground(DependencyObject obj, Brush value)
        {
            obj.SetValue(HeaderBackgroundProperty, value);
        }

        /// <summary>
        /// 控件头部背景颜色
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.RegisterAttached("HeaderBackground", typeof(Brush), typeof(ControlAttach), new PropertyMetadata(null));


        public static CornerRadius GetCornerRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(CornerRadiusProperty);
        }

        public static void SetCornerRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// 控件边缘圆角值
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.RegisterAttached("CornerRadius", typeof(CornerRadius), typeof(ControlAttach), new PropertyMetadata(default(CornerRadius)));



    }
}
