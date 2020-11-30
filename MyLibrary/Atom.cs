using System;
using System.Collections.Generic;

namespace MyLibrary
{
    public class Atom
    {
        // Static Class Variables:
        //------------------------
        public static double xlo;
        public static double xhi;
        public static double ylo;
        public static double yhi;
        public static double zlo;
        public static double zhi;
        public static string[] atom_types;

        // Object Variables:
        //-------------------
        public int atomId;
        public Point location;      // This is the location read from the atom_dump file which is already scaled (i.e. between 0 and 1)
        public Point location_real;  // This is the real location
        public int atomType;
        public double charge;
        public string symbol;
        public int chunk;
        public double mass;
        public Vector force;
        public Vector velocity;

        // Constructors:
        //-------------------
        public Atom(int _atomId, Point _location, string location_type, int _atomType, double _charge, int _chunk)
        {
            atomId      = _atomId;
            location    = _location;
            atomType    = _atomType;
            charge      = _charge;
            chunk       = _chunk;
            symbol      = atom_types[atomType-1];
            switch (symbol)
            {
                case "Ba":
                    // Barium
                    mass = 137.327;
                    break;
                case "Ti":
                    // Titanium
                    mass = 47.867;
                    break;
                case "O":
                    // Oxygen
                    mass = 15.999;
                    break;
                case "Mg":
                    // Magnesium
                    mass = 24.305; 
                    break;
                case "Al":
                    // Aluminum
                    mass = 26.98;
                    break;
                case "Si":
                    // Silicon
                    mass = 28.0855;
                    break;
                case "C":
                    // Carbon
                    mass = 12.0107;
                    break;
                default:
                    throw new ArgumentException("Undefiend Atom type");
            }

            if (location_type == "scale")
            {
                location_real = new Point(location.x * (xhi - xlo) + xlo, location.y * (yhi - ylo) + ylo, location.z * (zhi - zlo) + zlo);
            }
            else 
            {
                location_real = location;
            }

            force = new Vector(0, 0, 0);
            velocity = new Vector(0, 0, 0);
        }


        // Functions:
        //-------------------
        public double CalculateDistanceTo(Atom NeighbourAtom)
        {
            Vector AB = new Vector(location_real, NeighbourAtom.location_real);
            return AB.magnitude();
        }

        public Vector CalculateVectorTo(Atom NeighbourAtom)
        {
            Vector AB = new Vector(location_real, NeighbourAtom.location_real);
            return AB;
        }

        public void FixPeriodicBoundary(Atom otherAtom, double tolDistance)
        {
            /* This function comparess an atom to the same atom at a previous time step and fixes its location if it crossed a periodic boundary*/
            Vector AB = new Vector(location_real, otherAtom.location_real);

            if (AB.magnitude() > tolDistance) // Periodic Boundary Crossed
            {
                if (AB.x > tolDistance) // Periodic Boundary Crossed at x-axes
                {
                    if( Math.Abs(otherAtom.location_real.x - xlo) > Math.Abs(otherAtom.location_real.x - xhi) ) // Crossed the xhi boundary
                    {
                        location_real.x = location_real.x + xhi - xlo;
                    }
                    else
                    {
                        location_real.x = location_real.x - xhi + xlo;
                    }
                }

                if (AB.y > tolDistance) // Periodic Boundary Crossed at y-axes
                {
                    if (Math.Abs(otherAtom.location_real.y - ylo) > Math.Abs(otherAtom.location_real.y - yhi)) // Crossed the yhi boundary
                    {
                        location_real.y = location_real.y + yhi - ylo;
                    }
                    else
                    {
                        location_real.y = location_real.y - yhi + ylo;
                    }
                }

                if (AB.z > tolDistance) // Periodic Boundary Crossed at z-axes
                {
                    if (Math.Abs(otherAtom.location_real.z - zlo) > Math.Abs(otherAtom.location_real.z - zhi)) // Crossed the zhi boundary
                    {
                        location_real.z = location_real.z + zhi - zlo;
                    }
                    else
                    {
                        location_real.z = location_real.z - zhi + zlo;
                    }
                }

            }
        }

        // Method that enable what is written on Console when printing
        public override string ToString()
        {
            return string.Format("ID: {0}\nSymbol: {1}\nCharge: {2}\nChunk: {3}\nLocation: ({4}, {5}, {6})\nReal Location: ({7}, {8}, {9})",
                atomId, symbol, charge, chunk, location.x, location.y, location.z, location_real.x , location_real.y , location_real.z
            );
        }

        public string PrintToFile() // Real location
        {
            return string.Format("{0} {1} {2} {3} {4}", atomId, atomType, location_real.x, location_real.y, location_real.z);
        }

        public string PrintToFile(Vector dipoleMoment, double temp) // Real location
        {
            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}",
                atomId, atomType, location_real.x, location_real.y, location_real.z, dipoleMoment.x, dipoleMoment.y, dipoleMoment.z, dipoleMoment.magnitude(), charge, temp);
        }
    }
}

