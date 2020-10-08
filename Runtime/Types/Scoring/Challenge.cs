using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class Challenge
    {
        public Run originalRun;
        public Run playerRun;
        public Run bestRun;

        public int tries_success;
        public int tries_failed;

        public Challenge()
        {
        }

        public override string ToString()
        {
            return $"Remote Challenge Data {originalRun}";
        }
    }
}