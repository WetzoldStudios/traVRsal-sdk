﻿using UnityEngine;

namespace traVRsal.SDK
{
    public class TransformAligner : MonoBehaviour
    {
        public Transform source;
        public Vector3 positionOffset;

        private void Update()
        {
            transform.position = source.position + positionOffset;
            transform.rotation = source.rotation;
        }
    }
}