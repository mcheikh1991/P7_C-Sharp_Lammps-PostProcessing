using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MyLibrary
{
	public class TimeStep
	{
		public int timestep;
		public Dictionary<int, Chunk> chunksDict;  // from the first  chunking file
		//public Dictionary<int, Chunk> chunks2; // from the second chunking file

        public TimeStep (int _timestep) 
        {
            timestep   = _timestep;
            chunksDict = new Dictionary<int, Chunk> ();
            //chunks2 = new Dictionary<int, Chunk> ();
            //chunks2 = new Dictionary<int, Chunk>();
        }
	}
}

