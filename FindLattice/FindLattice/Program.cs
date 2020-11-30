	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Linq;
	using System.Threading;
	using System.Text;
    using MyLibrary;


namespace FindLattice
{
    class MainClass {
           public static void Main(string[] args) {

            /*
            if(args.Length != 2)
            {
                throw new ArgumentException("the number of arguments should be 2; The atom file and the lattice type");
            }
            */

            string atomFile = args[0]; // First Argument: atom_dump
            System.IO.StreamWriter outFile = new System.IO.StreamWriter("lattice_dump"); // Last Argument: atom_dump_with_Lattice
            string Lattice_Type;

            if (args[1] == "TiO2" || args[1] == "SiO2")
            {
                Lattice_Type = args[1];
            }
            else
            {
                throw new ArgumentException("Undefined lattice.");
            }

            string lattice_atoms = "ON";
            if (args.Length == 3)
            {
                lattice_atoms = args[2];
                outFile = new System.IO.StreamWriter("lattice_dump_contour");
            }

            Console.WriteLine("What are the atoms used in the simulation: (Write them in the order they were used)");
            string[] atom_types = Console.ReadLine().Split();

            //-------------------------------------------------------------------------------------------------------------//
            // TASK 1: Read the atom_dump file at time
            //-------------------------------------------------------------------------------------------------------------//
            #region Task3

            AtomsData atomsData = new AtomsData(atomFile, atom_types);
            List<int> AllTimes = atomsData.GetAllTimes();
            //List<int> AllTimes = new List<int>();
            //AllTimes.Add(0);
            LatticeData latticeData = new LatticeData(Lattice_Type);
            foreach (int time in AllTimes) // Loop through List with foreach
            {
                Dictionary<int, Atom> atomsDict  = atomsData.ReadTimeData(time);      // All atoms

                Dictionary<int, Atom> Ti_atomsDict = new Dictionary<int, Atom>();
                Dictionary<int, Atom> O_atomsDict  = new Dictionary<int, Atom>();  
                Dictionary<int, Atom> Si_atomsDict = new Dictionary<int, Atom>();  
                 
                foreach(KeyValuePair<int, Atom> entry in atomsDict)
                {
                    int atomId = entry.Key;
                    Atom newAtom = entry.Value;


                    switch (newAtom.symbol)
                    {
                        case "Ti":
                            Ti_atomsDict.Add(atomId, newAtom);
                            break;
                        case "O":
                            O_atomsDict.Add(atomId, newAtom);
                            break;
                        case "Si":
                            Si_atomsDict.Add(atomId, newAtom);
                            break;
                    }
                }


                //-------------------------------------------------------------------------------------------------------------//
                // TASK 2: Analyze the atom data
                //-------------------------------------------------------------------------------------------------------------//

                Console.WriteLine("Creating the Lattices");
                Dictionary<int, Lattice> Lattice_Dict;
                //Dictionary<int, Lattice> Lattice_Dict = latticeData.ReadLatticeDict(Ti_atomsDict, O_atomsDict);
                //Lattice_Dict = latticeData.CreateLatticeDictSiO2(atomsDict);
                if (time == 0)
                {
                    if (Lattice_Type == "TiO2")
                    {
                        Lattice_Dict = latticeData.CreateLatticeDictTiO2(Ti_atomsDict, O_atomsDict);
                    }
                    else //(Lattice_Type == "SiO2")
                    {
                        Lattice_Dict = latticeData.CreateLatticeDictSiO2(atomsDict, Si_atomsDict);
                    }
                }
                else
                {
                    if (Lattice_Type == "TiO2")
                    {
                        Lattice_Dict = latticeData.UpdateLatticeDictTiO2(Ti_atomsDict, O_atomsDict);
                    }
                    else //(Lattice_Type == "SiO2")
                    {
                        Lattice_Dict = latticeData.UpdateLatticeDictSiO2(Si_atomsDict, O_atomsDict);
                    }
                }


                Console.WriteLine("Number of Lattices created is: " + Lattice_Dict.Count.ToString());
                Console.WriteLine("-------------------------------");

                //-------------------------------------------------------------------------------------------------------------//
                // TASK 3: Writting the output file
                //-------------------------------------------------------------------------------------------------------------//

                StringBuilder buffer = new StringBuilder(1100000);
                /*
                double Lx = 27.5622;
                double Ly = 27.5622;
                double Lz = 38.4631-(-5.9174);
                */

                buffer.AppendLine("ITEM: TIMESTEP");
                buffer.AppendLine(time.ToString());
                buffer.AppendLine("ITEM: NUMBER OF ATOMS");
                if (lattice_atoms == "ON")
                {
                    buffer.AppendLine((Lattice_Dict.Count * 9).ToString());
                }
                else
                {
                    buffer.AppendLine((Lattice_Dict.Count).ToString());
                }
                buffer.AppendLine("ITEM: BOX BOUNDS pp pp ff");
                buffer.AppendLine("0.0e+00 10.0e+01");
                buffer.AppendLine("0.0e+00 10.0e+01");
                buffer.AppendLine("0.0e+00 10.0e+01");
                buffer.AppendLine("ITEM: ATOMS id type x y z dip_x dip_y dip_z mag_dip q temp");
                foreach (KeyValuePair<int, Lattice> item in Lattice_Dict)
                {
                    if (lattice_atoms == "ON")
                    {
                        buffer.Append(item.Value.PrintToFileAtoms());
                    }
                    else 
                    {
                        buffer.AppendLine(item.Value.PrintToFile());
                    }
                }
                outFile.Write(buffer);
                buffer.Clear();
            }

            outFile.Close();

            #endregion

        }
    }
}