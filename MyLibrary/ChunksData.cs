using System;
using System.Collections.Generic;

namespace MyLibrary
{

    public class ChunksData
    {
        // Object Variables:
        //-------------------
        public string chunk_file;
        public int domain;
        public Dictionary<int, TimeStep> timeStepDict;

        // Constructors:
        //-------------------

        public ChunksData(string _chunk_files, int _domain)
        {
            chunk_file = _chunk_files;
            domain = _domain;
            timeStepDict = new Dictionary<int, TimeStep>();
            string line;

            System.IO.StreamReader CurrentchunksFile = new System.IO.StreamReader(chunk_file);

            Console.WriteLine("Reading chunk file " + chunk_file);

            int timestep = 0;//, numberOfChunks, chunksCreated;
            int find_pos_chunk = 0;

            Dictionary<string, int> chunkVariables = new Dictionary<string, int>(); // Find the position of each word

            while ((line = CurrentchunksFile.ReadLine()) != null)
            {

                string[] chunkLineSplit = line.Split(' ');

                // Step 1: Skip the lines starting with "#"
                //-----------------------------------------
                if (line.StartsWith("#"))
                {
                    // Finding the location of the variables
                    if (find_pos_chunk == 0 && chunkLineSplit[1] == "Row")
                    {
                        chunkVariables = GetTheChunkVariables(line);
                    }
                    continue;
                }

                // Step 2: If the line has two entries only then it means a new time step
                //-------------------------------------------------------------------------
                if (chunkLineSplit.Length == 2)
                {
                    timestep = int.Parse(chunkLineSplit[0]);
                    timeStepDict.Add(timestep, new TimeStep(timestep));
                    continue;
                }

                // Step 3: Create a new chunk and add it the chunk dictionary
                //--------------------------------------------------------------
                Chunk newChunk = CreateChunk(chunkLineSplit, chunkVariables);
                timeStepDict[timestep].chunksDict.Add(newChunk.chunkId, newChunk);
            }

            CurrentchunksFile.Close();
        }

        // Private function that finds the location of each variable
        private Dictionary<string,int> GetTheChunkVariables(string lineFromFile)
        {
            Dictionary<string, int> chunkVariables = new Dictionary<string, int>();

            if (lineFromFile.StartsWith("#"))
            {
                string[] chunkLineSplit = lineFromFile.Split(' ');
                int c = 0;

                foreach (string word in chunkLineSplit)
                {

                    if (word != "#")
                    {
                        string newWord = "";
                        //Console.WriteLine(word);
                        if (word == "Row") { newWord = "id"; }
                        else if (word.IndexOf("dipolechunk", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            if (word.IndexOf("[1]", StringComparison.OrdinalIgnoreCase) >= 0) { newWord = "dip_x"; }
                            else if (word.IndexOf("[2]", StringComparison.OrdinalIgnoreCase) >= 0) { newWord = "dip_y"; }
                            else if (word.IndexOf("[3]", StringComparison.OrdinalIgnoreCase) >= 0) { newWord = "dip_z"; }
                            else if (word.IndexOf("[4]", StringComparison.OrdinalIgnoreCase) >= 0) { newWord = "dip_mag"; }
                        }
                        else if (word.IndexOf("chunkcenter", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            if (word.IndexOf("[1]", StringComparison.OrdinalIgnoreCase) >= 0) { newWord = "xc"; }
                            else if (word.IndexOf("[2]", StringComparison.OrdinalIgnoreCase) >= 0) { newWord = "yc"; }
                            else if (word.IndexOf("[3]", StringComparison.OrdinalIgnoreCase) >= 0) { newWord = "zc"; }
                        }
                        else if (word.IndexOf("chunktemp", StringComparison.OrdinalIgnoreCase) >= 0) { newWord = "temp"; }
                        /*
                        else if (word == "c_dipolechunk[1]" || word == "c_dipolechunk2[1]" || word == "c_dipoleChunkFe[1]") { newWord = "dip_x"; }
                        else if (word == "c_dipolechunk[2]" || word == "c_dipolechunk2[2]" || word == "c_dipoleChunkFe[2]") { newWord = "dip_y"; }
                        else if (word == "c_dipolechunk[3]" || word == "c_dipolechunk2[3]" || word == "c_dipoleChunkFe[3]") { newWord = "dip_z"; }
                        else if (word == "c_dipolechunk[4]" || word == "c_dipolechunk2[4]" || word == "c_dipoleChunkFe[4]") { newWord = "dip_mag"; }
                        else if (word == "c_chunkcenter[1]" || word == "c_chunkcenter2[1]") { newWord = "xc"; }
                        else if (word == "c_chunkcenter[2]" || word == "c_chunkcenter2[2]") { newWord = "yc"; }
                        else if (word == "c_chunkcenter[3]" || word == "c_chunkcenter2[3]") { newWord = "zc"; }
                        else if (word == "c_chunktemp[1]" || word == "c_chunktemp2[1]") { newWord = "temp"; }
                        */                            
                        else if (word == "c_chunkmsd[1]" || word == "c_chunkmsd2[1]") { newWord = "msd_x"; }
                        else if (word == "c_chunkmsd[2]" || word == "c_chunkmsd2[2]") { newWord = "msd_y"; }
                        else if (word == "c_chunkmsd[3]" || word == "c_chunkmsd2[3]") { newWord = "msd_z"; }
                        else if (word == "c_chunkmsd[3]" || word == "c_chunkmsd2[3]") { newWord = "msd_mag"; }
                        else if (word == "c_chunktorque[1]" || word == "c_chunktorque2[1]") { newWord = "torque_x"; }
                        else if (word == "c_chunktorque[2]" || word == "c_chunktorque2[2]") { newWord = "torque_y"; }
                        else if (word == "c_chunktorque[3]" || word == "c_chunktorque2[3]") { newWord = "torque_z"; }                  
                        else { newWord = word; } // default case

                        //Console.WriteLine(newWord);
                        chunkVariables.Add(newWord, c);
                        c++;
                    }
                }
            }
            else
            {
                throw new System.ArgumentException("Incorrect line. Can not deduce the variables from this line");
            }

            return chunkVariables;
        }


        private Chunk CreateChunk(string[] chunkLineSplit, Dictionary<string,int> chunkVariables)
        {
            int chunkId = int.Parse(chunkLineSplit[chunkVariables["id"]]);
            Vector DipoleMoment = new Vector(double.Parse(chunkLineSplit[chunkVariables["dip_x"]]), double.Parse(chunkLineSplit[chunkVariables["dip_y"]]), double.Parse(chunkLineSplit[chunkVariables["dip_z"]]));
            double DipoleMoment_Mag = double.Parse(chunkLineSplit[chunkVariables["dip_mag"]]);
            Point CenterOfMass = new Point(double.Parse(chunkLineSplit[chunkVariables["xc"]]), double.Parse(chunkLineSplit[chunkVariables["yc"]]), double.Parse(chunkLineSplit[chunkVariables["zc"]]));

            Chunk newChunk = new Chunk(chunkId, DipoleMoment, DipoleMoment_Mag, CenterOfMass, domain);

            if (chunkVariables.ContainsKey("temp"))
            {
                newChunk.temp = double.Parse(chunkLineSplit[chunkVariables["temp"]]);
            }

            if (chunkVariables.ContainsKey("msd_mag"))
            {
                newChunk.msd.x = double.Parse(chunkLineSplit[chunkVariables["msd_x"]]);
                newChunk.msd.y = double.Parse(chunkLineSplit[chunkVariables["msd_y"]]);
                newChunk.msd.z = double.Parse(chunkLineSplit[chunkVariables["msd_z"]]);
                newChunk.msd_mag = double.Parse(chunkLineSplit[chunkVariables["msd_mag"]]);
            }

            if (chunkVariables.ContainsKey("torque_x"))
            {
                newChunk.torque.x = double.Parse(chunkLineSplit[chunkVariables["torque_x"]]);
                newChunk.torque.y = double.Parse(chunkLineSplit[chunkVariables["torque_y"]]);
                newChunk.torque.z = double.Parse(chunkLineSplit[chunkVariables["torque_z"]]);
            }

            return newChunk;
        }
    }
}


