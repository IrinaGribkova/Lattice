using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace Lattice
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static double dist = 30;
        static double thickness_line = 0.4;
        //static double thickness_coordininate_plane= 1;
        private static Dictionary<double, Point> points_X = new Dictionary<double, Point>();
        private static Dictionary<double, Point> points_Y = new Dictionary<double, Point>();
        public MainWindow()
        {
            InitializeComponent();
            double k = dist;
            for (double i = (int)((g.Height / dist - 1) / 2) ; i >= -(int)((g.Height / dist -1) / 2); i--)
            {
                y1.Items.Add(i);
                y2.Items.Add(i);
                y1_2.Items.Add(i);
                y2_2.Items.Add(i);
                Point point = new Point(g.Width / 2, k);
                points_Y.Add(i, point);
                k += dist;
            }
            k = dist;
            for (double i = -(int)((g.Width / dist - 1) / 2); i <= (int)((g.Width / dist - 1) / 2); i++)
            {
                x1.Items.Add(i);
                x2.Items.Add(i);
                x1_2.Items.Add(i);
                x2_2.Items.Add(i);
                Point point = new Point(k, g.Height/ 2);
                points_X.Add(i, point);
                k += dist;
            }
        }

        enum ArrowType
        {
            None, Start, End, Both
        }

        private Path LineWithArrow(double startX, double startY, double endX, double endY, double len = 25, double width = 5, ArrowType arrowType = ArrowType.End)
        {
            var result = new Path();
            var v = new Vector(endX - startX, endY - startY);
            v.Normalize();
            var gg = new GeometryGroup();
            gg.Children.Add(new LineGeometry(new Point(startX, startY), new Point(endX, endY)));
            switch (arrowType)
            {
                case ArrowType.None:
                    break;
                case ArrowType.Start:
                    gg.Children.Add(Arrow(endX, endY, startX, startY, len, width));
                    break;
                case ArrowType.End:
                    gg.Children.Add(Arrow(startX, startY, endX, endY, len, width));
                    break;
                case ArrowType.Both:
                    gg.Children.Add(Arrow(endX, endY, startX, startY, len, width));
                    gg.Children.Add(Arrow(startX, startY, endX, endY, len, width));
                    break;
            }
            result.Data = gg;
            return result;
        }

        private Geometry Arrow(double startX, double startY, double endX, double endY, double len = 25, double width = 5)
        {
            var v = new Vector(endX - startX, endY - startY);
            v.Normalize();
            var v1 = new Vector(endX - v.X * len, endY - v.Y * len);
            var n1 = new Vector(-v.Y * width / 2 + v1.X, v.X * width / 2 + v1.Y);
            var n2 = new Vector(v.Y * width / 2 + v1.X, -v.X * width / 2 + v1.Y);
            var gg = new GeometryGroup();
            gg.Children.Add(new LineGeometry(new Point(n1.X, n1.Y), new Point(endX, endY)));
            gg.Children.Add(new LineGeometry(new Point(n2.X, n2.Y), new Point(endX, endY)));
            return gg;
        }
        static double Find_length_vector(double x1, double y1, double x2, double y2)
        {
            double len = Math.Sqrt(Math.Pow(x1 - x2, 2) * Math.Pow(y1 - y2,2));
            return len;
        }
        static Line Create_line(double x1, double y1, double x2, double y2, double thickness)
        {
            Line L = new Line();
            L.X1 = x1;
            L.Y1 = y1;
            L.X2 = x2;
            L.Y2 = y2;
            L.Stroke = Brushes.Blue;
            L.StrokeThickness = thickness;

            return L;
        }
        static Ellipse Create_point(double x, double y)
        {
            Point point = new Point();
            Ellipse elipse = new Ellipse();
            elipse.StrokeThickness = 2;
            elipse.Stroke = Brushes.Black;
            point.X = x;
            point.Y = y;
            elipse.Width = 4;
            elipse.Height = 4;
            elipse.StrokeThickness = 2;
            elipse.Stroke = Brushes.Black;
            elipse.Margin = new Thickness(point.X - 2, point.Y - 2, 0, 0);

            return elipse;
        }
      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (double i = -(g.Height/dist)/2; i <= (g.Height / dist) / 2; i++)
            {
                y1.Items.Add(i);
                y2.Items.Add(i);
            }
            for (double i = -(g.Width / dist) / 2; i <= (g.Width / dist) / 2; i++)
            {
                x1.Items.Add(i);
                x2.Items.Add(i);
            }
        }
        private void Execute_Click(object sender, RoutedEventArgs e)
        {
            for (double i = 0; i <= g.Width; i+=dist)
            {
                for (double j = 0; j <= g.Height; j+=dist)
                {                
                        g.Children.Add(Create_point(i,j));
                }
            }
            for (double i = 0; i <= g.Width; i++)
            {
                if (i % dist == 0)
                {
                    g.Children.Add(Create_line(i, 0, i, g.Height, thickness_line));
                }
            }
            for (double i = 0; i <= g.Height- dist; i++)
            {
                if (i % dist == 0 )
                {
                    g.Children.Add(Create_line(i, 0, 0, i, thickness_line));
                }
            }
            double k = dist;
            for (double i = 0; i <= g.Width; i += dist)
            {
                if (i+g.Height<=g.Width)
                {
                    g.Children.Add(Create_line(i, g.Height, (i+ g.Height), 0, thickness_line));
                }
            }
            for (double i = g.Width - g.Height + dist; i <= g.Width; i+=dist)
            {     
                    g.Children.Add(Create_line(i, g.Height, g.Width, k, thickness_line));
                    k += dist;  
            }
            var end_X = LineWithArrow( g.Width / 2, g.Height,g.Width / 2, 0, arrowType: ArrowType.End);
            end_X.Stroke = Brushes.Blue;
            var end_Y = LineWithArrow(0, g.Height / 2,g.Width, g.Height / 2, arrowType: ArrowType.End);
            end_Y.Stroke = Brushes.Blue;
            g.Children.Add(end_Y);
            g.Children.Add(end_X);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            g.Children.Clear();
        }

        private void Draw_Click(object sender, RoutedEventArgs e)
        {

            var vector_1 = LineWithArrow(points_X[Convert.ToDouble(x1.SelectedValue.ToString())].X, points_Y[Convert.ToDouble(y1.SelectedValue.ToString())].Y, points_X[Convert.ToDouble(x2.SelectedValue.ToString())].X, points_Y[Convert.ToDouble(y2.SelectedValue.ToString())].Y,arrowType: ArrowType.End);
            vector_1.Stroke = Brushes.Red;
            g.Children.Add(vector_1);
            var vector_2 = LineWithArrow(points_X[Convert.ToDouble(x1_2.SelectedValue.ToString())].X, points_Y[Convert.ToDouble(y1_2.SelectedValue.ToString())].Y, points_X[Convert.ToDouble(x2_2.SelectedValue.ToString())].X, points_Y[Convert.ToDouble(y2_2.SelectedValue.ToString())].Y, arrowType: ArrowType.End);
            vector_2.Stroke = Brushes.Red;
            g.Children.Add(vector_2);
        }
    }
}
