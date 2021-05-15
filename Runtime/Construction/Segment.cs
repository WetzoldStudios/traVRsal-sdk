using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace traVRsal.SDK
{
    [Serializable]
    public class Segment
    {
        public const int SUM_OF_COMPONENTS = -1;

        public enum Order
        {
            Random, Fixed
        }

        [DefaultValue(true)]
        public bool enabled = true;
        public string key;
        public string chapter;
        [DefaultValue(SUM_OF_COMPONENTS)]
        public int length = SUM_OF_COMPONENTS;
        public List<SegmentComponent> components;
        public Order order = Order.Random;

        public Segment()
        {
            components = new List<SegmentComponent>();
        }

        public Segment(string key, Order order = Order.Random, int length = SUM_OF_COMPONENTS) : this()
        {
            this.key = key;
            this.order = order;
            this.length = length;
        }

        public Segment WithComponent(SegmentComponent component)
        {
            components.Add(component);

            return this;
        }

        public override string ToString()
        {
            return $"Segment ({key})";
        }
    }
}