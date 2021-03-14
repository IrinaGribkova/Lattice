using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace Lattice
{
    public class Gram_Schmidt
    {
        List<Vector> vectors = new List<Vector>();

        public Gram_Schmidt(List<Vector> v)
        {
            vectors = v;
        }

        public List<Vector> Calculate_vector(List<Vector> v)
        {
            List<Vector> u = new List<Vector>(v.Count);
            u.Add(v[0]);
            for (int i = 1; i < v.Count; i++)
            {
                Vector u_i = new Vector();
                u_i = v[i];
                for (int j = 0; j < i; j++)
                {
                    u_i -= Proj(u[j], v[i]);
                }
                u.Add(u_i);
            }
            return  u;
        }

        private Vector Proj(Vector u, Vector v)
        {
            return Scalar_product(u, v) / Scalar_product(u, u) * u;
        }

        private double Scalar_product(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y;
        }
    }
}
