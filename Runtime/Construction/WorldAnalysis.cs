using System;
using System.Collections.Generic;

namespace traVRsal.SDK
{
    [Serializable]
    public class WorldAnalysis
    {
        public int totalObjects;
        public int totalAgents;
        public List<string> referencedWorlds;
        public List<string> referencedObjects;
        public List<string> referencedSpeech;

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