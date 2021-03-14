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

        Point point_start_v1;
        Point point_end_v1;

        Brush color_vector;

        private static Dictionary<double, Point> points_X = new Dictionary<double, Point>();
        private static Dictionary<double, Point> points_Y = new Dictionary<double, Point>();
        private static List<Vector> vectors = new List<Vector>();
        private static List<Path> vect_on_plane = new List<Path>();

        private static List<Ellipse> ellipse_on_plane = new List<Ellipse>();

        public MainWindow()
        {
            InitializeComponent();
            double k = dist;

            Center_y1.SelectionChanged += OnSelectionChanged;
            Center_x1.SelectionChanged += OnSelectionChanged;

            for (double i = (int)((g.Height / dist - 1) / 2) ; i >= -(int)((g.Height / dist -1) / 2); i--)
            {
                y1.Items.Add(i);
                y2.Items.Add(i);
                Center_y1.Items.Add(i);    
                Point point = new Point(g.Width / 2, k);
                points_Y.Add(i, point);
                k += dist;
            }
            k = dist;
            for (double i = -(int)((g.Width / dist - 1) / 2); i <= (int)((g.Width / dist - 1) / 2); i++)
            {
                x1.Items.Add(i);
                Center_x1.Items.Add(i);
                x2.Items.Add(i);
                Point point = new Point(k, g.Height/ 2);
                points_X.Add(i, point);
                k += dist;
            }
            k = dist;
            for (double i = 0; i <= g.Width; i += dist) // Create points on coodrinate plane
            {
                for (double j = 0; j <= g.Height; j += dist)
                {
                    g.Children.Add(Create_point(i, j, Brushes.Black));
                }
            }
            for (double i = 0; i <= g.Width; i += dist)
            {
                g.Children.Add(Create_line(i, 0, i, g.Height, thickness_line));

                if (i <= g.Height - dist)
                    g.Children.Add(Create_line(i, 0, 0, i, thickness_line));
                if (i <= g.Width - g.Height)
                    g.Children.Add(Create_line(i, g.Height, i + g.Height, 0, thickness_line));
                if (i >= g.Width - g.Height + dist)
                {
                    g.Children.Add(Create_line(i, g.Height, g.Width, k, thickness_line));
                    k += dist;
                }
            }
            Add_digits(points_X, ref g);
            Add_digits(points_Y, ref g);
            Add_vector(g.Width / 2, g.Height, g.Width / 2, 0, ref g, Brushes.Blue, false);
            Add_vector(0, g.Height / 2, g.Width, g.Height / 2, ref g, Brushes.Blue, false);
        }

       
        enum ArrowType
        {
            None, Start, End, Both
        }

        private static Path LineWithArrow(double startX, double startY, double endX, double endY, double len = 25, double width = 5, ArrowType arrowType = ArrowType.End)
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

        private static Geometry Arrow(double startX, double startY, double endX, double endY, double len = 25, double width = 5)
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
        static Ellipse Create_point(double x, double y, Brush color)
        {
            Point point = new Point();
            Ellipse elipse = new Ellipse();
            elipse.StrokeThickness = 2;
            elipse.Stroke = color;
            point.X = x;
            point.Y = y;
            elipse.Width = 4;
            elipse.Height = 4;
            elipse.StrokeThickness = 2;
            elipse.Stroke = color;
            elipse.Margin = new Thickness(point.X - 2, point.Y - 2, 0, 0);
            
            return elipse;       
        }

        Ellipse CreateEllipse(double width, double height, double desiredCenterX, double desiredCenterY )
        {
            Ellipse ellipse = new Ellipse { Width = width, Height = height };
            double left = desiredCenterX - (width / 2);
            double top = desiredCenterY - (height / 2);

            ellipse.Margin = new Thickness(left, top, 0, 0);
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = Brushes.Red;
            ellipse_on_plane.Add(ellipse);
            return ellipse;
        }

        static void Add_digits(Dictionary<double, Point> pnts, ref Canvas gr)
        {
            TextBlock textBlock;
            foreach (var p in pnts)
            {
                textBlock = new TextBlock();
                textBlock.Text = p.Key.ToString();
                Canvas.SetLeft(textBlock, p.Value.X + 2);
                Canvas.SetTop(textBlock, p.Value.Y + 2);
                gr.Children.Add(textBlock);
            }
        }
        static void Add_vector(double startX, double startY, double endX, double endY, ref Canvas gr, Brush brushes, bool add)
        {
            Vector v = new Vector(endX - startX, endY - startY); 
            var vector = LineWithArrow(startX, startY, endX, endY, arrowType: ArrowType.End);
            vector.Stroke = brushes;
                if (!vectors.Contains(v))
                {
                    gr.Children.Add(vector);
                    if (add)
                    {
                        vectors.Add(v);
                        vect_on_plane.Add(vector);
                    }   
                }
                else MessageBox.Show("This vector already exists on coordinate plane!");   
        }
        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Center_x1.SelectedItem != null)
            {
                Width_e.Items.Clear();
                for (double i = 1; i < g.Width / dist / 2 - Math.Abs(Convert.ToDouble(Center_x1.SelectedValue.ToString())); i++)
                {
                    Width_e.Items.Add(i * 2);
                }
            }
            if (Center_y1.SelectedItem != null)
            {
                Height_e.Items.Clear();
                for (double i = 1; i < g.Height / dist / 2 - Math.Abs(Convert.ToDouble(Center_y1.SelectedValue.ToString())); i++)
                {
                    Height_e.Items.Add(i * 2);
                }
            }
        }

        private Point Get_fractional_coordinates(double x, double y)
        {
            Point point = new Point();

            point.X = (x - (int)x)*dist + points_X[(int)x].X;
            //if(y >= -1)
            //point.Y = -(y - (int)y) * dist + points_Y[(int)y].Y;
            //else point.Y =  points_Y[(int)y +1].Y - Math.Abs(y - (int)y)* dist ;
            if(y <= 1)
            point.Y = points_Y[(int)y].Y - (y - (int)y) * dist ;
            else point.Y = - (y - (int)y) * dist + points_Y[(int)y - 1].Y;
            return point;
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
            try
            {
                point_start_v1 = new Point(Convert.ToDouble(x1.SelectedValue.ToString()), Convert.ToDouble(y1.SelectedValue.ToString()));
                point_end_v1 = new Point(Convert.ToDouble(x2.SelectedValue.ToString()), Convert.ToDouble(y2.SelectedValue.ToString()));
                Add_vector(points_X[point_start_v1.X].X, points_Y[point_start_v1.Y].Y, points_X[point_end_v1.X].X, points_Y[point_end_v1.Y].Y, ref g, Brushes.Red, true);
            }
            catch
            {
              MessageBox.Show("Input coordinates, please!");
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                g.Children.Remove(vect_on_plane[vect_on_plane.Count - 1]);
            }
            catch
            {
                MessageBox.Show("Input at least one vector, please!");
            }
        }

        private void Delete_Ellipse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                g.Children.Remove(ellipse_on_plane[ellipse_on_plane.Count - 1]);
                ellipse_on_plane.Remove(ellipse_on_plane[ellipse_on_plane.Count - 1]);
            }
            catch
            {
                MessageBox.Show("Input at least one ellipse, please!");
            }
        }

        private void Draw_Ellipse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double center_x = points_X[Convert.ToDouble(Center_x1.SelectedValue.ToString())].X;
                double center_y = points_Y[Convert.ToDouble(Center_y1.SelectedValue.ToString())].Y;
                double width = Convert.ToDouble(Width_e.SelectedValue.ToString()) * dist;
                double height = Convert.ToDouble(Height_e.SelectedValue.ToString()) * dist;
                g.Children.Add(CreateEllipse(width, height, center_x, center_y));
            }
            catch
            {
                MessageBox.Show("Input data for ellipse, please!");
            }
        }
        
    }
}
