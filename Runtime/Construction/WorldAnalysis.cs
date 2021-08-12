using System;
using System.Collections.Generic;

namespace traVRsal.SDK
{
    [Serializable]
    public class WorldAnalysis
    {
        public List<string> referencedWorlds;
        public List<string> referencedObjects;
        public List<string> referencedSpeech;
        public int totalObjects;

        public WorldAnalysis()
        {
            referencedWorlds = new List<string>();
            referencedObjects = new List<string>();
            referencedSpeech = new List<string>();
        }

        public override string ToString()
        {
            return "World Analysis";
        }
    }
}