using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Modules.Translation
{
    /*===================================================
    * 类名称: ControlHelper
    * 类描述: 控件及窗体的辅助类
    * 创建人: CDKWT-DEVELOP-03
    * 创建时间: 2020-03-27 11:51:55
    * 修改人: 唐正军
    * 修改时间:
    =====================================================*/
    public static class ControlHelper
    {
        /// <summary>
        /// 根据子控件获取父控件，逐级往上查找
        /// </summary>
        /// <typeparam name="T">父控件类型</typeparam>
        /// <param name="sender">子控件</param>
        /// <returns></returns>
        public static T FindVisualParent<T>(DependencyObject sender) where T : DependencyObject
        {
            do
            {
                sender = VisualTreeHelper.GetParent(sender);
                if (sender == null) break;
            } while (!(sender is T));
            return sender as T;
        }

        /// <summary>
        /// 在父控件中查找子集，有多个符合条件的子集时，获取第一个
        /// </summary>
        /// <typeparam name="childItem">子控件类型</typeparam>
        /// <param name="obj">父控件</param>
        /// <returns></returns>
        public static childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取最近激活的窗体，如果整个应用程序不活动，返回null
        /// </summary>
        public static Window GetActiveWindow()
        {
            var k = Application.Current.Windows.Cast<Window>().FirstOrDefault(window => window.IsActive);
            return k;
        }
        /// <summary>
        /// 获取默认的蒙层容器，一般是窗体内部的宿主，获取失败时返回null
        /// </summary>
        public static Grid GetDefaultMaskContainer()
        {
            return GetActiveWindow()?.Content as Grid;
        }
        /// <summary>
        /// 获取主窗体
        /// </summary>
        /// <returns></returns>
        public static Window GetMainWindow()
        {
            return Application.Current.MainWindow;
        }
        /// <summary>
        /// 关闭宿主窗体
        /// </summary>
        /// <param name="childControl"></param>
        public static void CloseParentWindow(this FrameworkElement childControl)
        {
            FindVisualParent<Window>(childControl)?.Close();
        }
        /// <summary>
        /// 关闭宿主弹窗并返回值
        /// </summary>
        /// <param name="childControl"></param>
        /// <param name="result"></param>
        public static void CloseParentDialogWindow(this FrameworkElement childControl,bool result)
        {
            if(childControl is Window window)
            {
                window.DialogResult = result;
                return;
            }
            FindVisualParent<Window>(childControl).DialogResult = result;
        }
        /// <summary>
        /// 将多个CheckBox的Check属性按位整合成一个int
        /// </summary>
        /// <param name="checkBoxes">不定长checkbox集合</param>
        /// <returns></returns>
        public static int SumOfCheckbox(params CheckBox[] checkBoxes)
        {
                return Convert.ToInt32(string.Join(null, checkBoxes.Select(item => item.IsChecked.Value ? '1' : '0').Reverse()), 2);
        }
        /// <summary>
        /// 检查控件及其子项是否都通过了验证(递归逻辑树方式).
        /// 如果是TextBox, 检查其Text属性
        /// 如果是ComboBox, 检查SelectedItem
        /// 如果是其他控件,则只检查有没有错误标记存在
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsValidationSuccess(DependencyObject node)
        {
            if (node != null)
            {
                bool? isValid = true;
                if (Validation.GetHasError(node)) return false;
                switch (node)
                {
                    case TextBox control:
                        isValid = BindingOperations.GetBinding(control, TextBox.TextProperty)?.ValidationRules.All(v => v.Validate(control.Text, null).IsValid);
                        break;
                    case ComboBox control:
                        isValid = BindingOperations.GetBinding(control, ComboBox.SelectedItemProperty)?.ValidationRules.All(v => v.Validate(control.SelectedItem, null).IsValid);
                        break;
                }
                if (isValid == false)
                {
                    return false;
                }
            }
            //若控件校验通过, 继续递归检查子控件
            foreach (object subnode in LogicalTreeHelper.GetChildren(node))
            {
                if (subnode is DependencyObject)
                {
                    if (IsValidationSuccess((DependencyObject)subnode) == false) return false;
                }
            }
            // 所有校验都通过
            return true;
        }

        /// <summary>
        /// 通过Id设置选中项, 要求选项里必须有Id属性
        /// </summary>
        /// <param name="control"></param>
        /// <param name="Id"></param>
        public static void SelectById(this Selector control, int Id)
        {
            control.SelectedItem = control.Items.OfType<object>()
                .FirstOrDefault(x =>Convert.ToInt32(x.GetType().GetProperty("Id")?.GetValue(x))==Id);
        }
        /// <summary>
        /// 获取元素所在的宿主TabItem，需要事先把Frame.Tag设置为宿主TabItem。
        /// UI结构：TabControl-Frame-Page-Element
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static TabItem GetHostTabItem(this UIElement element)
        {
            return FindVisualParent<Frame>(element)?.Tag as TabItem;
        }
    }
}
