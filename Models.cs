using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericSearch
{
    public class Point
    {
        public int row, col;
        public Point(int r, int c)
        {
            row = r;
            col = c;
        }

        protected Point() { }

        public static bool operator ==(Point a, Point b)
        {
            return (a.row == b.row && a.col == b.col);
        }

        public static bool operator !=(Point a, Point b)
        {
            return !(a.row == b.row && a.col == b.col);
        }

        public override bool Equals(Object p)
        {
            if (row == ((Point)p).row && col == ((Point)p).col)
            {
                return true;
            }
            return false;
        }

        public static Point offset(Point target, Point center, Func<int, int, int> Operation = null)
        {
            if (Operation == null)
            {
                Operation = Methods.Add;
            }
            return new Point(Operation(center.row, target.row), Operation(center.col, target.col));
        }

        public Point clone()
        {
            return new Point(row, col);
        }

        public override string ToString()
        {
            return row + "," + col;
        }
    }

    public static class Methods
    {
        public static int Clamp(int num, int min, int max)
        {
            if (num < 0) return 0;
            if (num > 255) return 255;
            return num;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Program.rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static int Add(int a, int b)
        {
            return a + b;
        }

        public static int Subtract(int a, int b)
        {
            return a - b;
        }

        public static int Multiply(int a, int b)
        {
            return a * b;
        }

        public static bool GreaterThan<T>(int a, int b)
        {
            return a.CompareTo(b) > 0;
        }

        public static bool LessThan(int a, int b)
        {
            return a.CompareTo(b) < 0;
        }

        public static string Merge(this List<string> a)
        {
            string result = "";
            foreach (string s in a)
            {
                result += s;
            }
            return result;
        }
    }
}
