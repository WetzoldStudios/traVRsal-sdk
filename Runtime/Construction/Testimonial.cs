using System;

namespace traVRsal.SDK
{
    [Serializable]
    public sealed class Testimonial
    {
        public string text;
        public string author;
        public string date;
        public string source;

        public Testimonial()
        {
        }

        public override string ToString()
        {
            return $"Testimonial ({author})";
        }
    }
}