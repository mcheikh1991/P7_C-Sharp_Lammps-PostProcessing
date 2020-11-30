#!/bin/bash

mcs -t:library Point.cs Vector.cs Atom.cs TimeStep.cs Chunk.cs Lattice.cs LatticeData.cs ChunksData.cs AtomsData.cs -out:MyLibrary.dll
