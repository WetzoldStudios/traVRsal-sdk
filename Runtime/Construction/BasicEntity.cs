using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace traVRsal.SDK
{
    [Serializable]
    public class BasicEntity
    {
        public static Vector2Int EMPTY = new Vector2Int(int.MinValue, int.MinValue);

        // positions must match direction enum
        public static Direction[] ALL_DIRECTIONS = new Direction[6] { Direction.WEST, Direction.EAST, Direction.SOUTH, Direction.NORTH, Direction.TOP, Direction.BOTTOM };
        public static Direction[] HORIZONTAL_DIRECTIONS = new Direction[4] { Direction.WEST, Direction.EAST, Direction.SOUTH, Direction.NORTH };

        public enum Direction
        {
            WEST, EAST, SOUTH, NORTH, TOP, BOTTOM, RANDOM_SIDE, FREE_SIDE, NONE, PATH_AHEAD, PATH_BACK, SAME, OPPOSITE
        }

        public Vector2Int position = EMPTY;
        public Vector2Int scale = Vector2Int.one;
        public Vector2Int anchor = EMPTY;
        public int height = 1;
        public int yGrid = 0;
        public bool flipX = false;
        public bool flipZ = false;
        public bool socket = false;
        public bool snap = false;
        public bool autoFill = false;
        public bool isVirtual = false;
        public string targetRoom;
        public Direction direction = Direction.SOUTH;

        public string key;
        public string lowKey;
        public string baseKey;
        public string name;
        public string layerName;
        public string variable;
        public TMProperty[] properties;

        [HideInInspector]
        public int autoIdx;
        [HideInInspector]
        public Vector2Int originalPosition;
        [HideInInspector]
        public int[] layerNeighbours = new int[4];
        [HideInInspector]
        public bool instantiated = false;

        public BasicEntity() { }

        public BasicEntity(string name)
        {
            this.name = Path.GetFileNameWithoutExtension(name);
            this.key = name;
            this.lowKey = this.key.ToLower();
        }

        public BasicEntity(BasicEntity copyFrom)
        {
            anchor = copyFrom.anchor;
            position = copyFrom.position;
            scale = copyFrom.scale;
            height = copyFrom.height;
            yGrid = copyFrom.yGrid;
            flipX = copyFrom.flipX;
            flipZ = copyFrom.flipZ;
            socket = copyFrom.socket;
            autoFill = copyFrom.autoFill;
            snap = copyFrom.snap;
            isVirtual = copyFrom.isVirtual;
            targetRoom = copyFrom.targetRoom;
            direction = copyFrom.direction;

            key = copyFrom.key;
            baseKey = copyFrom.baseKey;
            lowKey = copyFrom.lowKey;
            name = copyFrom.name;
            layerName = copyFrom.layerName;
            variable = copyFrom.variable;

            if (copyFrom.properties != null)
            {
                properties = new TMProperty[copyFrom.properties.Length];
                for (int i = 0; i < properties.Length; i++)
                {
                    properties[i] = new TMProperty(copyFrom.properties[i].name, copyFrom.properties[i].type, copyFrom.properties[i].value);
                }
            }
        }

        public override bool Equals(object obj)
        {
            return obj is BasicEntity entity &&
                   position.Equals(entity.position) &&
                   scale.Equals(entity.scale) &&
                   anchor.Equals(entity.anchor) &&
                   height == entity.height &&
                   yGrid == entity.yGrid &&
                   flipX == entity.flipX &&
                   flipZ == entity.flipZ &&
                   direction == entity.direction &&
                   key == entity.key &&
                   name == entity.name &&
                   layerName == entity.layerName &&
                   variable == entity.variable &&
                   EqualityComparer<TMProperty[]>.Default.Equals(properties, entity.properties) &&
                   autoIdx == entity.autoIdx;
        }

        public override int GetHashCode()
        {
            var hashCode = 813519686;
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2Int>.Default.GetHashCode(position);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2Int>.Default.GetHashCode(scale);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector2Int>.Default.GetHashCode(anchor);
            hashCode = hashCode * -1521134295 + height.GetHashCode();
            hashCode = hashCode * -1521134295 + yGrid.GetHashCode();
            hashCode = hashCode * -1521134295 + flipX.GetHashCode();
            hashCode = hashCode * -1521134295 + flipZ.GetHashCode();
            hashCode = hashCode * -1521134295 + direction.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(key);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(layerName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(variable);
            hashCode = hashCode * -1521134295 + EqualityComparer<TMProperty[]>.Default.GetHashCode(properties);
            hashCode = hashCode * -1521134295 + autoIdx.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"Entity {key} ({position}, {direction})";
        }
    }
}