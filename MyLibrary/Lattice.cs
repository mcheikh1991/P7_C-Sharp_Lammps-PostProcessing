using System;
using System.Collections.Generic;

namespace MyLibrary
{
    public class Lattice
    {
		public static double k = 1.38066e-23; // Boltzmanns Constant [J/K]
		public static double Na = 6.022140857e23; // Avogadro constant [mol^-1]
		public static double A  = 1e-10; // Angstrom [m]
		public static double ps = 1e-12; // Picosecond [s]
		public static double g  = 1e-3; // Grams [kg]

        // Object Variables:
        //-------------------
        public int latticeId;
        public List<Atom> atoms;
        public Point centerOfMass;
        public Point centerOfGeometry;
        public Vector dipoleMoment;
        public double dipoleMoment_mag;
        public int numberOfAtoms;
        public double temperature;


        // Constructors:
        //-------------------
        public Lattice(int _latticeId, List<Atom> _atoms)
        {
            latticeId = _latticeId;
            atoms = _atoms;

            numberOfAtoms = atoms.Count;
            CalculateCenterOfGeometry();
            CalculateCenterOfMass();
            CalculateDipole();
            CalculateTemperature();
        }

        // Functions:
        //-------------------
        public void CalculateCenterOfGeometry()
        {
            centerOfGeometry = new Point(0.0, 0.0, 0.0);
            foreach (Atom single_atom in atoms) // Loop through List with foreach
            {
                centerOfGeometry += single_atom.location_real;
            }
            centerOfGeometry = centerOfGeometry / numberOfAtoms;
        }

        public void CalculateCenterOfMass()
        {
            centerOfMass = new Point(0.0, 0.0, 0.0);
            double totalMass = 0.0;
            foreach (Atom single_atom in atoms) // Loop through List with foreach
            {
                centerOfMass += (single_atom.location_real * single_atom.mass) ;
                totalMass += single_atom.mass;
            }
            centerOfMass = centerOfMass / totalMass;
        }


        public void CalculateDipole()
        {
            dipoleMoment = new Vector(0.0, 0.0, 0.0);
            Vector Displacement;

            foreach (Atom single_atom in atoms) // Loop through List with foreach
            {
                Displacement = new Vector(centerOfGeometry, single_atom.location_real);
                dipoleMoment += (Displacement * single_atom.charge);
            }

            dipoleMoment_mag = dipoleMoment.magnitude();
        }

        public double CalculateCharge()
        {
            double totalCharge = 0.0;

            foreach (Atom single_atom in atoms) // Loop through List with foreach
            {
                totalCharge += (single_atom.charge);
            }

            return totalCharge;
        }

        public void CalculateTemperature()
        {

            temperature = 0.0;
            double kineticEnergy = 0.0;
			double v;
			double m;

            foreach (Atom single_atom in atoms) // Loop through List with foreach
            {
				m = single_atom.mass* (g/ Na); // [Kg]
				v = single_atom.velocity.magnitude()*(A/ps); //[m/s]
				kineticEnergy += (0.5*m*v*v); // 0.5*m*v^2
            }

            kineticEnergy = kineticEnergy / atoms.Count; // Average Kinetic Energy
            temperature = (2.0 * kineticEnergy) / (3.0 * k);
        }

        // Method that enable what is written on Console when printing
        public override string ToString()
        {
            string AllAtomID = "";
            foreach (Atom single_atom in atoms) // Loop through List with foreach
            {
                AllAtomID += single_atom.atomId.ToString() + " ";
            }

            return string.Format("ID: {0}\nDipoleMoment: ({1}, {2}, {3})\nDipoleMoment_Mag: {4}\ncenter of Geometry: ({5}, {6}, {7})\ncenter of Mass: ({8}, {9}, {10})\nTemperature: {11}\nNumber of Atoms: {12}\nAtom IDs:{13}",
                latticeId, dipoleMoment.x, dipoleMoment.y, dipoleMoment.z, dipoleMoment_mag, centerOfGeometry.x, centerOfGeometry.y, centerOfGeometry.z, centerOfMass.x, centerOfMass.y, centerOfMass.z, temperature, numberOfAtoms, AllAtomID);
        }

        public string PrintToFile(double Lx, double Ly, double Lz) // Normalized location
        {
            return string.Format("{0} 1 {1} {2} {3} {4} {5} {6} {7} {8} {9}",
                latticeId, centerOfGeometry.x/Lx, centerOfGeometry.y/Ly, centerOfGeometry.z/Lz, dipoleMoment.x, dipoleMoment.y, dipoleMoment.z, dipoleMoment_mag, CalculateCharge(), temperature);
        }

        public string PrintToFile() // Real location
        {
            return string.Format("{0} {0} {1} {2} {3} {4} {5} {6} {7} {8} {9}",
                latticeId, centerOfGeometry.x, centerOfGeometry.y, centerOfGeometry.z, dipoleMoment.x, dipoleMoment.y, dipoleMoment.z, dipoleMoment_mag, CalculateCharge(), temperature);
        }

        public string PrintToFileAtoms() // Real location
        {
            string Output = "";
            foreach (Atom single_atom in atoms) // Loop through List with foreach
            {
                Output += single_atom.PrintToFile(dipoleMoment, temperature);
                Output += "\n";
            }
            //Output = Output.Remove(Output.Length - 2, 1);
            return Output;
        }


    }
}
