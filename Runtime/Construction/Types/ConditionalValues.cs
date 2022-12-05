namespace traVRsal.SDK
{
    public sealed class ConditionalValues
    {
        public string condition;
        public string[] values;

        public ConditionalValues()
        {
        }

        public ConditionalValues(string condition, string[] values)
        {
            this.condition = condition;
            this.values = values;
        }

        public override string ToString()
        {
            return $"Conditional Values ({condition})";
        }
    }
}