
using Modules.Translation.Custom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Modules.Translation
{
    public partial class KwtButtonStyle
    {
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

        private void Grid_MouseLeftAsyncButtonDown(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            var canvas = grid.Children[0] as ChildUICanvas;
            var center = e.GetPosition(grid);
            var width = grid.ActualWidth;
            var height = grid.ActualHeight;
            canvas.UIDispatcher.InvokeAsync(new Action(() =>
            {
                try
                {
                    Debug.WriteLine(Thread.CurrentThread.GetHashCode() + "线程");

                    var path = new Path { Fill = polygonBrush, Style = null };
                    var ellipse = new EllipseGeometry() { RadiusY = 20 };
                    path.Data = ellipse;
                    //path.Arrange(new Rect(0, 0, 200, 40));
                    path.Arrange(canvas.FinalRect);
                    canvas.Children.Add(path);
                    var time = 1d;
                    //设置圆心位置
                    ellipse.Center = center;
                    //根据勾股定理计算涟漪最大长度
                    var maxLength = Math.Sqrt(Math.Pow(width, 2) + Math.Pow(height, 2));
                    //开始涟漪放缩动画
                    ellipse.BeginAnimation(EllipseGeometry.RadiusXProperty, new DoubleAnimation(0, maxLength, new Duration(TimeSpan.FromSeconds(time))));
                    ellipse.BeginAnimation(EllipseGeometry.RadiusYProperty, new DoubleAnimation(0, maxLength, new Duration(TimeSpan.FromSeconds(time))));
                    //开始透明度消失动画
                    var animation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(time)));
                    animation.Completed += (s1, e1) =>
                    {
                        canvas.Children.Remove(path);
                    };
                    path.BeginAnimation(Path.OpacityProperty, animation);


                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }));
        }

    }
}
