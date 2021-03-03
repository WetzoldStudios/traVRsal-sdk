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
        public static Direction[] ALL_DIRECTIONS = {Direction.West, Direction.East, Direction.South, Direction.North, Direction.Top, Direction.Bottom};
        public static Direction[] HORIZONTAL_DIRECTIONS = {Direction.West, Direction.East, Direction.South, Direction.North};

        public enum Direction
        {
            West,
            East,
            South,
            North,
            Top,
            Bottom,
            Random_Side,
            Free_Side,
            None,
            Path_Ahead,
            Path_Back,
            Same,
            Opposite,
            Corner
        }

        public Vector2Int position = EMPTY;
        public Direction direction = Direction.South;
        public Vector2Int anchor = EMPTY;
        public Vector2Int scale = Vector2Int.one;
        public int height = 1;
        public int y;
        public int maxY = -1;
        public bool flipX;
        public bool flipZ;
        public bool socket;
        public bool snap;
        public bool autoFill;
        public bool isVirtual;

        public string targetZone;
        public string targetLocation;
        public int targetFloor = int.MinValue;
        public string locationId;

        public string key;
        public string lowKey;
        public string baseKey;
        public string name;
        public string layerName;
        public string variable;
        public TMProperty[] properties;

        // runtime data
        [HideInInspector] public bool isSpawned;
        [HideInInspector] public int autoIdx;
        [HideInInspector] public Vector2Int originalPosition;
        [HideInInspector] public int[] layerNeighbours = new int[4];
        [HideInInspector] public bool instantiated;

        [NonSerialized] public Transform node;

        public BasicEntity()
        {
        }

        public BasicEntity(string name)
        {
            this.name = Path.GetFileNameWithoutExtension(name);
            key = name;
            lowKey = key.ToLower();
        }

        public BasicEntity(BasicEntity copyFrom)
        {
            anchor = copyFrom.anchor;
            position = copyFrom.position;
            scale = copyFrom.scale;
            height = copyFrom.height;
            y = copyFrom.y;
            maxY = copyFrom.maxY;
            flipX = copyFrom.flipX;
            flipZ = copyFrom.flipZ;
            socket = copyFrom.socket;
            autoFill = copyFrom.autoFill;
            snap = copyFrom.snap;
            isVirtual = copyFrom.isVirtual;
            targetZone = copyFrom.targetZone;
            targetLocation = copyFrom.targetLocation;
            locationId = copyFrom.locationId;
            direction = copyFrom.direction;

            key = copyFrom.key;
            baseKey = copyFrom.baseKey;
            lowKey = copyFrom.lowKey;
            name = copyFrom.name;
            layerName = copyFrom.layerName;
            variable = copyFrom.variable;
            properties = SDKUtil.CopyProperties(copyFrom.properties);
        }

        public override bool Equals(object obj)
        {
            return obj is BasicEntity entity &&
                   position.Equals(entity.position) &&
                   scale.Equals(entity.scale) &&
                   anchor.Equals(entity.anchor) &&
                   height == entity.height &&
                   y == entity.y &&
                   flipX == entity.flipX &&
                   flipZ == entity.flipZ &&
                   direction == entity.direction &&
                   targetZone == entity.targetZone &&
                   targetLocation == entity.targetLocation &&
                   locationId == entity.locationId &&
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
            hashCode = hashCode * -1521134295 + y.GetHashCode();
            hashCode = hashCode * -1521134295 + flipX.GetHashCode();
            hashCode = hashCode * -1521134295 + flipZ.GetHashCode();
            hashCode = hashCode * -1521134295 + direction.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(targetZone);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(targetLocation);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(locationId);
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