using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class Segment
    {
        public const int SumOfComponents = -1;

        public enum Order
        {
            Random,
            Fixed
        }

        [DefaultValue(true)] public bool enabled = true;
        public string key;
        public string chapter;
        [DefaultValue(SumOfComponents)] public int length = SumOfComponents;
        public List<SegmentComponent> components;
        public Order order = Order.Random;

        public Segment()
        {
            components = new List<SegmentComponent>();
        }

        public Segment(string key, Order order = Order.Random, int length = SumOfComponents) : this()
        {
            this.key = key;
            this.order = order;
            this.length = length;
        }

        public Segment WithChapter(string chapter)
        {
            this.chapter = chapter;
            return this;
        }

        public Segment WithComponent(SegmentComponent component)
        {
            components.Add(component);
            return this;
        }

        public override string ToString()
        {
            return $"Segment '{key}'";
        }
    }
}