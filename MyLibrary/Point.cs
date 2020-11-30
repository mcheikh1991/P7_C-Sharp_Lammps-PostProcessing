using System;

namespace MyLibrary
{
	public class Point 
    {
		public double x = 0;
		public double y = 0;
		public double z = 0;

		public Point (double _x, double _y, double _z)	{
			x = _x;
			y = _y;
			z = _z;
		}

		public static Point operator + (Point b, Point c) {
			return new Point(
				b.x + c.x,
				b.y + c.y,
				b.z + c.z
			);
		}

		public static Point operator- (Point b, Point c) {
			return new Point(
				b.x - c.x,
				b.y - c.y,
				b.z - c.z
			);
		}

		public static Point operator- (Point b) {
			return new Point (
				-b.x,
				-b.y,
				-b.z
			);
		}

		public static Point operator/ (Point a, double b) {
			return new Point (
				a.x / b,
				a.y / b,
				a.z / b
			);
		}

        public static Point operator *(Point a, double b)
        {
            return new Point(
                a.x * b,
                a.y * b,
                a.z * b
            );
        }

        public override string ToString()
        {
            return string.Format
            (
               "{0} {1} {2}",x, y, z
            );
        }
    }
}

