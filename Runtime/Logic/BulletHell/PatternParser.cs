namespace traVRsal.SDK
{
    public class PatternParser
    {
        private string[] pattern = { };
        private bool globalLoop;
        private int currentStep;
        private int remainingLoops = -1;
        private int loopCount;
        private int loopLength;
        private int loopStart;
        private int loopStep;

        public PatternParser(string pattern, bool globalLoop = false)
        {
            this.globalLoop = globalLoop;
            if (!string.IsNullOrEmpty(pattern))
            {
                this.pattern = pattern.Split(',');
                for (int i = 0; i < this.pattern.Length; i++)
                {
                    this.pattern[i] = this.pattern[i].Trim().ToLower();
                }
            }
        }

        public int? GetNextExecution()
        {
            if (pattern.Length == 0 || currentStep >= pattern.Length) return null;

            int idx = GetCalculatedIdx();
            return int.Parse(pattern[idx]);
        }

        public string GetNextAction()
        {
            if (pattern.Length == 0 || currentStep >= pattern.Length)
            {
                return null;
            }

            int idx = GetCalculatedIdx() + 1;
            return pattern[idx];
        }

        public int? Next()
        {
            if (remainingLoops >= 0)
            {
                loopStep += 1;
                if (loopStep >= loopLength)
                {
                    loopStep = 0;
                    remainingLoops -= 1;
                    if (remainingLoops == 0)
                    {
                        currentStep += 2;
                        remainingLoops = -1;
                    }
                }
            }
            else
            {
                currentStep += 2;
            }
            if (globalLoop && currentStep >= pattern.Length)
            {
                currentStep = 0;
            }

            return GetNextExecution();
        }

        public int GetCurrentStep()
        {
            return currentStep;
        }

        private int GetCalculatedIdx()
        {
            string cur = pattern[currentStep];
            if (remainingLoops <= 0 && cur.StartsWith("loop"))
            {
                string[] arr = cur.Split(' ');
                loopLength = int.Parse(arr[1].Trim());

                string loopCountStr = pattern[currentStep + 1];
                loopCount = loopCountStr == "*" ? int.MaxValue : int.Parse(loopCountStr);
                loopStep = 0;
                loopStart = currentStep - 2 * loopLength;
                remainingLoops = loopCount;
            }
            if (remainingLoops > 0) return loopStart + loopStep * 2;

            return currentStep;
        }
    }
}