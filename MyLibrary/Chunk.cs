using System;
using System.Collections.Generic;

namespace MyLibrary {
    
	public class Chunk	
	{
		// Object Variables:
		//-------------------
		public int      chunkId;
		public Vector   dipoleMoment;				// (dipoleMoment_x , dipoleMoment_y, dipoleMoment_z)
		public double   dipoleMoment_mag; 			// (dipoleMoment_mag)
        public Point    centerOfMass;		
		public int      domain; 					// Incase there is two chunks 
		public double   totalPotential;
		public bool     useless_Chunk;				// If true then the chunk is useless, false then not useless
        public double   temp;
        public Vector   msd;                        // mean-squared displacement (MSD) 3 quantities are the squared dx,dy,dz displacements of the center-of-mass. 
        public double   msd_mag;                    // The 4th component is the total squared displacement, i.e. (dx*dx + dy*dy + dz*dz) of the center-of-mass.
        public Vector   torque;                     // 3 components of the torque vector, due to the forces on the individual atoms in the chunk around the center-of-mass of the chunk.

        // Constructors:
        //-------------------
        public Chunk (int _chunkId, Vector _dipoleMoment, double _dipoleMoment_mag, Point _centerOfMass, int _domain)	
		{
			chunkId = _chunkId;
			dipoleMoment = _dipoleMoment;
			dipoleMoment_mag = _dipoleMoment_mag;
			centerOfMass = _centerOfMass;
			domain = _domain;
            totalPotential = 0;
            temp = 0;
            msd = new Vector (0,0,0);
            msd_mag = 0.0;
            torque = new Vector(0, 0, 0);

            AssessIfUseless();
        }

        public Chunk(int _chunkId, Vector _dipoleMoment, double _dipoleMoment_mag, Point _centerOfMass, int _domain, double _temp)
        {
            chunkId = _chunkId;
            dipoleMoment = _dipoleMoment;
            dipoleMoment_mag = _dipoleMoment_mag;
            centerOfMass = _centerOfMass;
            domain = _domain;
            totalPotential = 0;
            temp = _temp;
            msd = new Vector(0, 0, 0);
            msd_mag = 0.0;
            torque = new Vector(0, 0, 0);

            AssessIfUseless();
        }

        public Chunk(int _chunkId, Vector _dipoleMoment, double _dipoleMoment_mag, Point _centerOfMass, int _domain, double _temp, Vector _msd, double _msd_mag)
        {
            chunkId = _chunkId;
            dipoleMoment = _dipoleMoment;
            dipoleMoment_mag = _dipoleMoment_mag;
            centerOfMass = _centerOfMass;
            domain = _domain;
            totalPotential = 0;
            temp = _temp;
            msd = _msd;
            msd_mag = _msd_mag;
            torque = new Vector(0, 0, 0);

            AssessIfUseless();
        }


        public Chunk(int _chunkId, Vector _dipoleMoment, double _dipoleMoment_mag, Point _centerOfMass, int _domain, Vector _msd, double _msd_mag)
        {
            chunkId = _chunkId;
            dipoleMoment = _dipoleMoment;
            dipoleMoment_mag = _dipoleMoment_mag;
            centerOfMass = _centerOfMass;
            domain = _domain;
            totalPotential = 0;
            temp = 0;
            msd = _msd;
            msd_mag = _msd_mag;
            torque = new Vector(0, 0, 0);

            AssessIfUseless();
        }


        public Chunk(int _chunkId, Vector _dipoleMoment, double _dipoleMoment_mag, Point _centerOfMass, int _domain, double _temp, Vector _torque)
        {
            chunkId = _chunkId;
            dipoleMoment = _dipoleMoment;
            dipoleMoment_mag = _dipoleMoment_mag;
            centerOfMass = _centerOfMass;
            domain = _domain;
            totalPotential = 0;
            temp = _temp;
            msd = new Vector(0, 0, 0); ;
            msd_mag = 0;
            torque = _torque;

            AssessIfUseless();
        }

        public Chunk(int _chunkId, Vector _dipoleMoment, double _dipoleMoment_mag, Point _centerOfMass, int _domain, Vector _msd, double _msd_mag, Vector _torque)
        {
            chunkId = _chunkId;
            dipoleMoment = _dipoleMoment;
            dipoleMoment_mag = _dipoleMoment_mag;
            centerOfMass = _centerOfMass;
            domain = _domain;
            totalPotential = 0;
            temp = 0;
            msd = _msd;
            msd_mag = _msd_mag;
            torque = _torque;

            AssessIfUseless();
        }

        public Chunk(int _chunkId, Vector _dipoleMoment, double _dipoleMoment_mag, Point _centerOfMass, int _domain, double _temp, Vector _msd, double _msd_mag, Vector _torque)
        {
            chunkId = _chunkId;
            dipoleMoment = _dipoleMoment;
            dipoleMoment_mag = _dipoleMoment_mag;
            centerOfMass = _centerOfMass;
            domain = _domain;
            totalPotential = 0;
            temp = _temp;
            msd = _msd;
            msd_mag = _msd_mag;
            torque = _torque;

            AssessIfUseless();
        }

        // Functions:
        //-------------------

        // Assess whether the chunk is useless or not:
        private void AssessIfUseless()
        {
            if (dipoleMoment_mag == 0 && centerOfMass.x == 0 && centerOfMass.y == 0 && centerOfMass.z == 0)
            {
                useless_Chunk = true;
            }
            else
            {
                useless_Chunk = false;
            }
        }


        // Calculate the TotalPotential of all chunks in a single chunkList on the current chunkList
		public void CalculateTotalPotential(TimeStep ts, int chunkDomain) //(Dictionary<int, Chunk> chunks,Dictionary<int, Chunk> chunks2)
		{
            totalPotential = 0;

			if (!useless_Chunk) 
            {
				foreach (int key in ts.chunksDict.Keys) 
                {
					if ((domain == chunkDomain && key == chunkId) || ts.chunksDict[key].useless_Chunk) 
                    {
						continue; // Skipping own chunk or useless ones
					}
					Chunk chunk = ts.chunksDict[key];
					Vector r = new Vector (chunk.centerOfMass, centerOfMass); // Vector from Far away chunk to current chunk
                    totalPotential += (1 / (4 * Math.PI)) * (chunk.dipoleMoment.dot (r) / Math.Pow (r.magnitude (), 3));
				}

			}

		}

        // Calculate the TotalPotential of all chunks in all chunkList on the current chunk
        public void CalculateTotalPotential(List<ChunksData> chunksList, int timeStep) //(Dictionary<int, Chunk> chunks,Dictionary<int, Chunk> chunks2)
        {
            totalPotential = 0;

            if (!useless_Chunk)
            {
                int counter = 0;

                foreach (ChunksData currentChunkData in chunksList)
                {
                    TimeStep ts = currentChunkData.timeStepDict[timeStep];

                    foreach (int key in ts.chunksDict.Keys)
                    {
                        if (domain == counter && key == chunkId || ts.chunksDict[key].useless_Chunk)
                        {
                            continue;
                        }
                        Chunk chunk = ts.chunksDict[key];
                        Vector r = new Vector(chunk.centerOfMass, centerOfMass); // Vector from Far away chunk to current chunk
                        totalPotential += (1 / (4 * Math.PI)) * (chunk.dipoleMoment.dot(r) / Math.Pow(r.magnitude(), 3));
                    }
                    counter++;
                }

            }

        }

        // Method that enable what is written on Console when printing
        public override string ToString()
		{
            string line = String.Format("ID: {0}\nDipoleMoment: {1} {2} {3}\nDipoleMoment_Mag: {4}\ncenterOfMass: {5} {6} {7}\nDomain: {8}\nTotalPotential: {9}\n",
                chunkId, dipoleMoment.x, dipoleMoment.y, dipoleMoment.z, dipoleMoment_mag, centerOfMass.x, centerOfMass.y, centerOfMass.z, domain, totalPotential) ;

            if (temp != 0)
            {
                line += String.Format("temp: {0}\n",  temp);
            }

            if (msd_mag != 0)
            {
                line += String.Format("msd: {0} {1} {2}\nmsd_Mag: {3}",  msd.x , msd.y, msd.z, msd_mag);
            }

            if (torque.magnitude() != 0 )
            {
                line += String.Format("torque: {0} {1} {2}", torque.x , torque.y, torque.z);
            }

            return line;
		}

	}
}

