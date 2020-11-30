using System;
using System.Collections.Generic;

namespace MyLibrary
{
    public class LatticeData
    {
        // Object Variables:
        //-------------------
        public string Lattic_Type;
        public Dictionary<int, Lattice> Original_Lattice_Dict;

        // Constructors:
        //-------------------

        public LatticeData(string _Lattic_Type)
        {
            Lattic_Type = _Lattic_Type;
        }


        public Dictionary<int, Lattice> CreateLatticeDictTiO2(Dictionary<int, Atom> Ti_atomsDict, Dictionary<int, Atom> O_atomsDict)
        {
            int Counter;

            double Ti_O_dist = 1000;
            double Ti_Ti_dist = 1000;

            double max_Ti_O_dist = 2.2;
            double max_Ti_Ti_dist = 3.8; // From Center Ti to Boundary Ti
            double min_Ti_Ti_dist = 3.2; // From Center Ti to Boundary Ti

            Dictionary<int, Lattice> Lattice_Dict = new Dictionary<int, Lattice>();
            Counter = 1; // Count the TiO2 Lattices Found
                             //int c = 1;

            foreach (KeyValuePair<int, Atom> item in Ti_atomsDict)
            {
                List<Atom> atoms_TiO2_Lattice = new List<Atom>();
                Atom Ti_atom = item.Value; // Main atom
                                               //Console.WriteLine("----------------------------------");
                                               //Console.WriteLine(Ti_atom);
                atoms_TiO2_Lattice.Add(Ti_atom);// Center Ti atom

                foreach (KeyValuePair<int, Atom> item2 in O_atomsDict)
                {
                    Atom O_atom = item2.Value;
                    Ti_O_dist = Ti_atom.CalculateDistanceTo(O_atom);
                    if (Ti_O_dist <= max_Ti_O_dist)
                    {
                       atoms_TiO2_Lattice.Add(O_atom);// Center O atom
                    }
                }
                //Console.WriteLine(atoms_TiO2_Lattice.Count);

                foreach (KeyValuePair<int, Atom> item2 in Ti_atomsDict)
                {
                    Atom Ti_atom_2 = item2.Value;
                    Ti_Ti_dist = Ti_atom.CalculateDistanceTo(Ti_atom_2);
                    if (Ti_Ti_dist <= max_Ti_Ti_dist && Ti_Ti_dist >= min_Ti_Ti_dist)
                    {
                        atoms_TiO2_Lattice.Add(Ti_atom_2); // Boundary Ti atoms
                                                               //Console.WriteLine(Ti_atom_2);
                                                               //Console.WriteLine(Ti_Ti_dist);
                    }
                }

                if (atoms_TiO2_Lattice.Count == 15)
                {
                    Lattice TiO2 = new Lattice(Counter, atoms_TiO2_Lattice);
                    //Console.WriteLine(TiO2);
                    Lattice_Dict.Add(Counter, TiO2);
                    Counter++;
                }
            }
            Original_Lattice_Dict = Lattice_Dict;
            return Lattice_Dict;
        } 


        public Dictionary<int, Lattice> CreateLatticeDictSiO2(Dictionary<int, Atom> All_atomsDict, Dictionary<int, Atom> Si_atomsDict)
        {

            Dictionary<int, Lattice> Lattice_Dict = new Dictionary<int, Lattice>();
            int numberOfSiO2Lattic = Si_atomsDict.Count / 3;

            for (int Counter = 1; Counter <= numberOfSiO2Lattic; Counter++)
            {
                List<Atom> atoms_SiO2_Lattice = new List<Atom>();
                for (int i=0;i<9; i++)
                {
                    atoms_SiO2_Lattice.Add(All_atomsDict[(Counter-1)*9+ 1 + i]);
                }

                if (atoms_SiO2_Lattice.Count == 9)
                {
                    Lattice SiO2 = new Lattice(Counter, atoms_SiO2_Lattice);
                    Console.WriteLine(SiO2);
                    Lattice_Dict.Add(Counter, SiO2);
                }
            }

            Original_Lattice_Dict = Lattice_Dict;
            return Lattice_Dict;
        }
        

        public Dictionary<int, Lattice> UpdateLatticeDictTiO2(Dictionary<int, Atom> Ti_atomsDict, Dictionary<int, Atom> O_atomsDict)
        {
            Dictionary<int, Lattice> Lattice_Dict = new Dictionary<int, Lattice>();
            int Counter = 1;
            foreach (KeyValuePair<int, Lattice> item in Original_Lattice_Dict)
            {
                Lattice Original_TiO2 = item.Value;
                List<Atom> updated_atoms_TiO2_Lattice = new List<Atom>();

                foreach (Atom single_atom in Original_TiO2.atoms) // Loop through List with foreach
                {
                    if (single_atom.symbol == "Ti")
                    {
                        foreach (KeyValuePair<int, Atom> item2 in Ti_atomsDict)
                        {
                            Atom Ti_atom_2 = item2.Value;
                            if (single_atom.atomId == Ti_atom_2.atomId)
                            {
                                Ti_atom_2.FixPeriodicBoundary(single_atom, 10);
                                updated_atoms_TiO2_Lattice.Add(Ti_atom_2);
                                break;
                            }
                        }
                    }
                    else if (single_atom.symbol == "O")
                    {
                        foreach (KeyValuePair<int, Atom> item2 in O_atomsDict)
                        {
                            Atom O_atom_2 = item2.Value;
                            if (single_atom.atomId == O_atom_2.atomId)
                            {
                                O_atom_2.FixPeriodicBoundary(single_atom, 10);
                                updated_atoms_TiO2_Lattice.Add(O_atom_2);
                                break;
                            }
                        }
                    }
                }

                if (updated_atoms_TiO2_Lattice.Count == 15)
                {
                    Lattice TiO2 = new Lattice(item.Key, updated_atoms_TiO2_Lattice);
                    TiO2.centerOfGeometry = Original_TiO2.centerOfGeometry; // Use the original Location
                    Lattice_Dict.Add(Counter, TiO2);
                    Counter++;
                }
                else
                {
                    throw new ArgumentException("Error: Missing Atoms");
                }

            }
            return Lattice_Dict;

        }

        public Dictionary<int, Lattice> UpdateLatticeDictSiO2(Dictionary<int, Atom> Si_atomsDict, Dictionary<int, Atom> O_atomsDict)
        {
            Dictionary<int, Lattice> Lattice_Dict = new Dictionary<int, Lattice>();
            int Counter = 1;
            foreach (KeyValuePair<int, Lattice> item in Original_Lattice_Dict)
            {
                Lattice Original_SiO2 = item.Value;
                List<Atom> updated_atoms_SiO2_Lattice = new List<Atom>();

                foreach (Atom single_atom in Original_SiO2.atoms) // Loop through List with foreach
                {
                    if (single_atom.symbol == "Si")
                    {
                        foreach (KeyValuePair<int, Atom> item2 in Si_atomsDict)
                        {
                            Atom Si_atom_2 = item2.Value;
                            if (single_atom.atomId == Si_atom_2.atomId)
                            {
                                //Si_atom_2.FixPeriodicBoundary(single_atom, 10);
                                updated_atoms_SiO2_Lattice.Add(Si_atom_2);
                                break;
                            }
                        }
                    }
                    else if (single_atom.symbol == "O")
                    {
                        foreach (KeyValuePair<int, Atom> item2 in O_atomsDict)
                        {
                            Atom O_atom_2 = item2.Value;
                            if (single_atom.atomId == O_atom_2.atomId)
                            {
                                //O_atom_2.FixPeriodicBoundary(single_atom, 10);
                                updated_atoms_SiO2_Lattice.Add(O_atom_2);
                                break;
                            }
                        }
                    }
                }

                if (updated_atoms_SiO2_Lattice.Count == 9)
                {
                    Lattice SiO2 = new Lattice(item.Key, updated_atoms_SiO2_Lattice);
                    SiO2.centerOfGeometry = Original_SiO2.centerOfGeometry; // Use the original Location
                    Lattice_Dict.Add(Counter, SiO2);
                    Counter++;
                }
                else
                {
                    throw new ArgumentException("Error. Missing Atoms. Atoms found " + updated_atoms_SiO2_Lattice.Count.ToString() + " it should be 9 in lattice " + Counter.ToString());
                }

            }
            return Lattice_Dict;

        }

    }

}
