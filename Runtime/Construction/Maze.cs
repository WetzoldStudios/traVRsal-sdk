using System;
using System.Collections.Generic;
using UnityEngine;

namespace traVRsal.SDK
{
    [Serializable]
    public class Maze
    {
        public enum MazeType
        {
            Maze, Path, Spline
        }

        public MazeType type = MazeType.Maze;
        public string layerName;
        public List<BasicEntity> exits = new List<BasicEntity>();
        public List<BasicEntity> obstacles = new List<BasicEntity>();
        public List<BasicEntity> path;
        public TMProperty[] properties;
        public Vector2Int start;
        public Vector2Int end;
        public Vector2Int size;
        public Vector2Int playSize;
        public float holeProbability = 0;
        public int innerMaxY = -1;
        public int holeCount = 0;
        public int autoSeed = 0;

        public Maze() { }

        public Maze(int seed) : this()
        {
            autoSeed = seed;
        }

        public override string ToString()
        {
            return $"Maze {layerName} ({type})";
        }
    }
}