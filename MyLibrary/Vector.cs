using System;

namespace MyLibrary
{
	public class Vector	
    {
		public double x = 0;
		public double y = 0;
		public double z = 0;

        // Constructors:
        //--------------
		public Vector (double _x, double _y, double _z) {
			x = _x;
			y = _y;
			z = _z;
		}

		public Vector (Point from, Point to)
         {
			x = to.x - from.x;
			y = to.y - from.y;
			z = to.z - from.z;
		}


        // Functions:
        //--------------
        public double dot(Vector other) 
        {
			return x * other.x + y * other.y + z * other.z;
		}

		public double magnitude() 
        {
			return Math.Sqrt (Math.Pow (x, 2) +  Math.Pow (y, 2) + Math.Pow (z, 2));
		}

        // this is third one 'Equals'
        public bool Equals(Vector b)
        {
            return (x == b.x && y == b.y && z == b.z);
        }

        public override string ToString()
        {
            return string.Format
            (
               "{0} {1} {2}", x, y, z
            );
        }

        // Operators
        //-----------

        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vector operator- (Vector a, Vector b) {
			return new Vector (a.x-b.x, a.y-b.y, a.z-b.z);
		}

		public static Vector operator- (Vector b) {
			return new Vector (-b.x, -b.y, -b.z);
		}

        public static Vector operator/ (Vector a, double b) {
			return new Vector (a.x/b, a.y/b, a.z/b);
		}

        public static Vector operator *(Vector a, double b)
        {
            return new Vector(a.x * b, a.y * b, a.z * b);
        }

        public static Vector operator *(Vector a, int b)
        {
            return new Vector(a.x * b, a.y * b, a.z * b);
        }
    }
}

