using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Journey
    {
        public const int SUM_OF_SEGMENTS = -1;

        [Header("Configuration")]
        public string key;
        [DefaultValue(SUM_OF_SEGMENTS)]
        public int length = SUM_OF_SEGMENTS;
        public List<Segment> segments;

        public Journey()
        {
            segments = new List<Segment>();
        }

        public Journey(int length) : this()
        {
            this.length = length;
        }

        public List<Zone> GetZones()
        {
            return segments.SelectMany(s => s.components).GroupBy(c => c.key).Select(g => g.First().zone).ToList();
        }

        public Journey WithSegment(Segment segment)
        {
            segments.Add(segment);

            return this;
        }

        public override string ToString()
        {
            return $"Journey '{key}'";
        }
    }
}