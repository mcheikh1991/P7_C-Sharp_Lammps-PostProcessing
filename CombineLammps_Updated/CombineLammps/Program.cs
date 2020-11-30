	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using System.Linq;
	using System.Threading;
	using System.Text;
    using MyLibrary;

namespace CombineLammps
{
    class MainClass 
    {

        public static void Main(string[] args) {

            System.IO.StreamReader atomFile = new System.IO.StreamReader(args[0]); // First Argument: atom_dump
            System.IO.StreamWriter outFile;  // atom_dump after combine
            bool Potential_From_Chunk = true;
            if (Potential_From_Chunk)
            {
                outFile = new System.IO.StreamWriter(args[0] + "_after_combine"); // Last Argument: atom_dump after combine
                Console.WriteLine("The potential will be calculated from the chunks to chunks");
            }
            else
            {
                outFile = new System.IO.StreamWriter(args[0] + "_after_combine_2");
                Console.WriteLine("The potential will be calculated from the atoms to chunks");
            }

            //-------------------------------------------------------------------------------------------------------------//
            // TASK 1: Create Chunks from chunk_dump and chunk2_dump
            //-------------------------------------------------------------------------------------------------------------//
            #region Task1

            List<ChunksData> chunksList = new List<ChunksData>();

            for (int domain = 1; domain < args.Length; domain++) 
            {
                chunksList.Add( new ChunksData(args[domain], domain-1) );
            }
            #endregion

            //-------------------------------------------------------------------------------------------------------------//
            // TASK 2: Calculate the total potentail for all chunks
            //-------------------------------------------------------------------------------------------------------------//
            #region Task2

            Console.WriteLine("Calculating the total potential");

            foreach (ChunksData currentChunkData in chunksList)
            {
                Dictionary<int, TimeStep> timeStepDict = currentChunkData.timeStepDict;

                foreach (int timeStep in timeStepDict.Keys)
                {
                    foreach (int chunkID in timeStepDict[timeStep].chunksDict.Keys)
                    {
                        timeStepDict[timeStep].chunksDict[chunkID].CalculateTotalPotential(chunksList,timeStep);
                        //timeStepDict[timeStep].chunksDict[chunkID].CalculateTotalPotential(timeStepDict[timeStep], domain)
                    }
                }
            }
            //Console.WriteLine(chunksList[0].timeStepDict[50000].chunksDict[8].totalPotential);
            //Console.WriteLine(chunksList[0].timeStepDict[50000].chunksDict[8].temp);
            #endregion


            //-------------------------------------------------------------------------------------------------------------//
            // TASK 3: Read the atom_dump file and update it
            //-------------------------------------------------------------------------------------------------------------//

            #region Task3

            Console.WriteLine("Reading and updating atom_dump file");

            StringBuilder buffer = new StringBuilder(1100000);

            List<string> atomLines = new List<string>(); // Creates a list of strings

            ThreadPool.SetMinThreads(100, 100);

            int NumberOfAtoms, Counter = 0;

			//int[] pos_atom = Enumerable.Repeat(-1, 26).ToArray();
            Dictionary<string, int> pos_atom = new Dictionary<string, int>();
            int find_pos_atom = 0;
            string line;
            int timestep;

            while ((line = atomFile.ReadLine()) != null )//|| (atomLines.Count() > 0 && atomFile.EndOfStream))
            {

                Counter++;

                if (line.StartsWith("ITEM: TIMESTEP"))
                {
                    buffer.AppendLine(line);

                    line = atomFile.ReadLine();
                    buffer.AppendLine(line);
                    timestep = int.Parse(line);

                    line = atomFile.ReadLine();
                    buffer.AppendLine(line);

                    line = atomFile.ReadLine();
                    buffer.AppendLine(line);

                    NumberOfAtoms = int.Parse(line);

                    // Skip the next five lines
                    for (int i=1; i <= 4; i++)
                    {
                        line = atomFile.ReadLine();
                        buffer.AppendLine(line);
                    }

                    // Find the position of each variable
                    line = atomFile.ReadLine();
                    if (find_pos_atom == 0)
                    {
                        //Console.WriteLine(line);
                        string[] atomlineSplit = line.Split(' ');
                        int c = 0;
                        int chunks_found = 0;
                        string newWord;
                        foreach (string word in  atomlineSplit)
                        {
                            newWord = word;
                            if (word != "ITEM:" && word != "ATOMS")
                            {
                                if (word.ToLower().IndexOf("atomchunk", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    chunks_found++;
                                    if (chunks_found == 1)
                                    {
                                        newWord = "v_atomchunk";
                                    }
                                    else
                                    {
                                        newWord = "v_atomchunk2";
                                    }
                                }
                                //Console.WriteLine(newWord);
                                pos_atom.Add(newWord.ToLower(), c);
                                c++;
                            }
                        }
                        find_pos_atom += 1;
                    }
                    buffer.AppendLine(line);

                    // Save the following lines to analyze later
                    for (int i=1; i <= NumberOfAtoms; i++)
                    {
                        line = atomFile.ReadLine();
                        atomLines.Add(line);
                    }
						
                    // Starting the analysis of the saved lines
                    ParallelOptions opt = new ParallelOptions(); // ParallelOptions: Stores options that configure the operation of methods on the Parallel class.
                    opt.MaxDegreeOfParallelism = 8;

                    Parallel.ForEach (atomLines, opt, atomLine =>
                    {
                        //Console.WriteLine(atomLine + "|" + Task.CurrentId.ToString());
                        //Console.WriteLine(Task.CurrentId);
                        string[] lineSplit = atomLine.Split(' ');

                        int domain = 0;
                        int chunkId = int.Parse(lineSplit[pos_atom["v_atomchunk"]]);

                        TimeStep ts = chunksList[0].timeStepDict[timestep];
                        Dictionary<int, Chunk> chunksDict = ts.chunksDict;

                        if (pos_atom.ContainsKey("v_atomchunk2") && chunkId == 0)
                        {
                            if (int.Parse(lineSplit[pos_atom["v_atomchunk2"]]) != 0)
                            {
                                chunkId = int.Parse(lineSplit[pos_atom["v_atomchunk2"]]);
                                ts = chunksList[1].timeStepDict[timestep];
                                chunksDict = ts.chunksDict;
                                domain++;
                            }
                        }

                        if (chunksDict.ContainsKey(chunkId) && chunkId != 0)
                        {
                            Chunk targetChunk = chunksDict[chunkId];
                            lineSplit[pos_atom["v_atomdipole_x"]] = Math.Round(targetChunk.dipoleMoment.x, 5).ToString();           // [11]v_atomdipole_x 
                            lineSplit[pos_atom["v_atomdipole_y"]] = Math.Round(targetChunk.dipoleMoment.y, 5).ToString();           // [12]v_atomdipole_y 
                            lineSplit[pos_atom["v_atomdipole_z"]] = Math.Round(targetChunk.dipoleMoment.z, 5).ToString();           // [13]v_atomdipole_z 
                            lineSplit[pos_atom["v_atomdipole"]] = Math.Round(targetChunk.dipoleMoment_mag, 5).ToString();           // [14]v_atomdipole 
                            if (pos_atom.ContainsKey("v_temperature"))
                            {
                                lineSplit[pos_atom["v_temperature"]] = Math.Round(targetChunk.temp, 5).ToString();                      // v_temperature 
                            }
                            if (pos_atom.ContainsKey("v_msd"))
                            {
                                lineSplit[pos_atom["v_msd_x"]] = Math.Round(targetChunk.msd.x, 5).ToString();
                                lineSplit[pos_atom["v_msd_y"]] = Math.Round(targetChunk.msd.y, 5).ToString();
                                lineSplit[pos_atom["v_msd_z"]] = Math.Round(targetChunk.msd.z, 5).ToString();
                                lineSplit[pos_atom["v_msd"]] = Math.Round(targetChunk.msd_mag, 5).ToString();
                            }
                            if (pos_atom.ContainsKey("v_torque_x"))
                            {
                                lineSplit[pos_atom["v_torque_x"]] = Math.Round(targetChunk.torque.x, 5).ToString();
                                lineSplit[pos_atom["v_torque_y"]] = Math.Round(targetChunk.torque.y, 5).ToString();
                                lineSplit[pos_atom["v_torque_z"]] = Math.Round(targetChunk.torque.z, 5).ToString();
                            }
                            if (Potential_From_Chunk == true)
                            {
                                lineSplit[pos_atom["v_potential"]] = Math.Round(targetChunk.totalPotential, 5).ToString();
                            }
                            else
                            {
                                Point centerOfMass = new Point(double.Parse(lineSplit[pos_atom["xs"]]), double.Parse(lineSplit[pos_atom["ys"]]), double.Parse(lineSplit[pos_atom["zs"]]));
                                lineSplit[pos_atom["v_potential"]] = Math.Round(CalculatePotential(chunkId, domain, timestep, centerOfMass, chunksList), 5).ToString();
                            }


                        }

                        string updatedLine = "";
                        foreach (string s in lineSplit)
                        {
                            updatedLine += s;
                            updatedLine += " ";
                        }

                        lock (buffer)
                        {
                            buffer.AppendLine(updatedLine);
                        }

                    }
                    );

                    outFile.Write(buffer);
                    buffer.Clear();
                    atomLines.Clear();
                }

            }

            outFile.Flush();

            atomFile.Close();
            outFile.Close();

            #endregion
        }
   

        public static double CalculatePotential(int chunkId, int domain, int timestep, Point centerOfMass, List<ChunksData> chunksList) 
        {
            double totalPotential = 0;
            int counter = 0;

            foreach(ChunksData currentChunkData in chunksList)
            {
                TimeStep ts = currentChunkData.timeStepDict[timestep];

                foreach (int key in ts.chunksDict.Keys)
                {
                    if (domain == counter && key == chunkId)
                    {
                        continue;
                    }
                    Chunk chunk = ts.chunksDict[key];
                    Vector r = new Vector(chunk.centerOfMass, centerOfMass); // Vector from Far away chunk to current chunk
                    totalPotential += (1 / (4 * Math.PI)) * (chunk.dipoleMoment.dot(r) / Math.Pow(r.magnitude(), 3) );
                }

                counter++;
            }

            return totalPotential;
        }


    }
}