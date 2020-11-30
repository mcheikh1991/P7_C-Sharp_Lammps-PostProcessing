using System;
using System.Collections.Generic;

namespace MyLibrary
{

    public class AtomsData
    {
        // Object Variables:
        //-------------------
        public string atom_file;
        public string[] atom_types;
        public Dictionary<int, TimeStep> timeStepDict;

        // Constructors:
        //-------------------

        public AtomsData(string _atom_file, string[] _atom_types)
        {
            atom_file = _atom_file;
            atom_types = _atom_types;
            Atom.atom_types = _atom_types;
        }

        // Functions:
        //------------

        public Dictionary<int, Atom> ReadTimeData(int requiredTimestep)
        { 
            System.IO.StreamReader atomFile = new System.IO.StreamReader(atom_file);

            Console.WriteLine("Reading atom file: " + atom_file);

            int NumberOfAtoms, Counter = 0;

            Dictionary<int, Atom> atomsDict = new Dictionary<int, Atom>();      // All atoms

            int timestep;
            string line;

            while ((line = atomFile.ReadLine()) != null && Counter == 0)//|| (atomLines.Count() > 0 && atomFile.EndOfStream))
            {
                if (line.StartsWith("ITEM: TIMESTEP"))
                {
                    line = atomFile.ReadLine();
                    timestep = int.Parse(line);

                    if (timestep == requiredTimestep)
                    {
                        Console.WriteLine("Reading data at time step: {0}", timestep);

                        line = atomFile.ReadLine();
                        line = atomFile.ReadLine();
                        NumberOfAtoms = int.Parse(line);
                        Console.WriteLine("\t Number of atoms is: {0}", NumberOfAtoms);

                        // The next four lines are the boundary files
                        string[] listLinesFromFile = new string[4];
                        for (int i = 0; i < 4; i++)
                        {
                            line = atomFile.ReadLine();
                            listLinesFromFile[i] = line;
                        }
                        GetTheDomainSize(listLinesFromFile);

                        // The next line helps in finding the variables saved by the file
                        line = atomFile.ReadLine();
                        Dictionary<string, int> atomVar = GetTheAtomVariables(line);

                        // Save the following lines to analyze later
                        Console.WriteLine("\t Reading atom data");
                        for (int i = 1; i <= NumberOfAtoms; i++)
                        {
                            line = atomFile.ReadLine();
                            //Console.WriteLine(line);
                            Atom newAtom = CreateAtom(line, atomVar);
                            atomsDict.Add(newAtom.atomId, newAtom);
                        }
                        Console.WriteLine("\t Done reading");
                        break;
                    }
                    else 
                    {
                        //Console.WriteLine("Skiping data at time step: {0}", timestep);
                        continue;
                    }
                }

            }

            atomFile.Close();
            return atomsDict;
        }

		public List<int> GetAllTimes()
		{
			List<int> AllTimes = new List<int>();
			System.IO.StreamReader atomFile = new System.IO.StreamReader(atom_file);

			string line;
			while ((line = atomFile.ReadLine()) != null)//|| (atomLines.Count() > 0 && atomFile.EndOfStream))
			{
				if (line.StartsWith("ITEM: TIMESTEP"))
				{
					line = atomFile.ReadLine();
					AllTimes.Add(int.Parse(line));
				}
				else 
				{
					continue;
				}
			}

			atomFile.Close();
			return AllTimes;
		}

        // Private function that finds the location of each variable
        private Dictionary<string,int> GetTheAtomVariables(string lineFromFile)
        {
            Dictionary<string, int> atomVariables = new Dictionary<string, int>();

            if (lineFromFile.StartsWith("ITEM: ATOMS"))
            {
                string[] atomLineSplit = lineFromFile.Split(' ');
                int c = 0;

                foreach (string word in atomLineSplit)
                {

                    if (word != "ITEM:" && word!= "ATOMS")
                    {
                        string newWord = "";
                        //Console.WriteLine(word);
                        if (word == "id" || word == "type" || word == "xs" || word == "ys" || word == "zs" || 
                            word == "q"  || word == "vx"  || word == "vy" || word == "vx" || word == "fx" ||
                            word == "fy" || word == "fz" || word == "x" || word == "y" || word == "z") { newWord = word; }
                        else if (word == "v_atomdipole_x") { newWord = "dip_x"; }
                        else if (word == "v_atomdipole_y") { newWord = "dip_y"; }
                        else if (word == "v_atomdipole_z") { newWord = "dip_z"; }
                        else if (word == "v_atomdipole") { newWord = "dip_mag"; }
                        else if (word.StartsWith("v_") )  { newWord = word.Replace("v_","");}
                        else { newWord = word; } // default case

                        //Console.WriteLine(newWord);
                        atomVariables.Add(newWord, c);
                        c++;
                    }
                }
            }
            else
            {
                throw new System.ArgumentException("Incorrect line. Can not deduce the variables from this line");
            }

            return atomVariables;
        }


        private void GetTheDomainSize(string[] listLinesFromFile)
        { 

            if (listLinesFromFile[0].StartsWith("ITEM: BOX BOUNDS") && listLinesFromFile.Length == 4)
            {
                string[] atomLineSplit_x = listLinesFromFile[1].Split(' ');
                Atom.xlo = Double.Parse(atomLineSplit_x[0]);
                Atom.xhi = Double.Parse(atomLineSplit_x[1]);

                string[] atomLineSplit_y = listLinesFromFile[2].Split(' ');
                Atom.ylo = Double.Parse(atomLineSplit_y[0]);
                Atom.yhi = Double.Parse(atomLineSplit_y[1]);

                string[] atomLineSplit_z = listLinesFromFile[3].Split(' ');
                Atom.zlo = Double.Parse(atomLineSplit_z[0]);
                Atom.zhi = Double.Parse(atomLineSplit_z[1]);
            }
            else
            {
                throw new System.ArgumentException("Incorrect set of line. Can not deduce the bounday size from this set");
            }
        }


        private Atom CreateAtom(string line, Dictionary<string,int> atomVar)
        {
            string[] lineSplit = line.Split(' ');

            int atomId = int.Parse(lineSplit[atomVar["id"]]);
            int atomType = int.Parse(lineSplit[atomVar["type"]]);
            double atomCharge = double.Parse(lineSplit[atomVar["q"]]);
            int atomChunk = 0; //int.Parse(lineSplit[atomVar["atomchunk"]]);
            /*
            if (atomChunk == 0 && atomVar.ContainsKey("atomchunk2"))
            {
                atomChunk = int.Parse(lineSplit[atomVar["atomchunk2"]]);
            }
            */

            string location_type;
            Point location;
            if (atomVar.ContainsKey("xs"))
            {
                location = new Point(double.Parse(lineSplit[atomVar["xs"]]), double.Parse(lineSplit[atomVar["ys"]]), double.Parse(lineSplit[atomVar["zs"]]));
                location_type = "scale";
            }
            else // (atomVar.ContainsKey("x"))
            {
                location = new Point(double.Parse(lineSplit[atomVar["x"]]), double.Parse(lineSplit[atomVar["y"]]), double.Parse(lineSplit[atomVar["z"]]));
                location_type = "real";
            }

            Atom newAtom = new Atom(atomId, location, location_type, atomType, atomCharge, atomChunk);

            if (atomVar.ContainsKey("fx"))
            {
                newAtom.force = new Vector( double.Parse(lineSplit[atomVar["fx"]]), double.Parse(lineSplit[atomVar["fy"]]), double.Parse(lineSplit[atomVar["fz"]]) );
            }

            if (atomVar.ContainsKey("vx"))
            {
                newAtom.velocity.x = double.Parse(lineSplit[atomVar["vx"]]);
            }
            if (atomVar.ContainsKey("vy"))
            {
                newAtom.velocity.y = double.Parse(lineSplit[atomVar["vy"]]);
            }
            if (atomVar.ContainsKey("vz"))
            {
                newAtom.velocity.z = double.Parse(lineSplit[atomVar["vz"]]);
            }
            return newAtom;
        }
    }
}


