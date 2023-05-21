using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Modules.Translation
{
    /// <summary>
    /// Attach property for control
    /// </summary>
    public class ControlAttach
    {
        #region 按钮附加图标

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.RegisterAttached("Icon", typeof(UIElement), typeof(ControlAttach), new FrameworkPropertyMetadata()
            {
                DefaultValue = null,
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            });

        #endregion 按钮附加图标

        #region 鼠标悬停时显示的内容

        // Using a DependencyProperty as the backing store for MouseOverContent. This enables
        // animation, styling, binding, etc...fdfsd
        public static readonly DependencyProperty MouseOverContentProperty =
            DependencyProperty.RegisterAttached("MouseOverContent", typeof(UIElement), typeof(ControlAttach), new FrameworkPropertyMetadata() { DefaultValue = null, BindsTwoWayByDefault = true });

        #endregion 鼠标悬停时显示的内容

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