namespace traVRsal.SDK
{
    public sealed class PatternParser
    {
        private readonly string[] _pattern = { };
        private bool _globalLoop;
        private int _currentStep;
        private int _remainingLoops = -1;
        private int _loopCount;
        private int _loopLength;
        private int _loopStart;
        private int _loopStep;

        public PatternParser(string pattern, bool globalLoop = false)
        {
            _globalLoop = globalLoop;
            if (!string.IsNullOrEmpty(pattern))
            {
                _pattern = pattern.Split(',');
                for (int i = 0; i < _pattern.Length; i++)
                {
                    _pattern[i] = _pattern[i].Trim();
                }
            }
        }

        public int? GetNextExecution()
        {
            if (_pattern.Length == 0 || _currentStep >= _pattern.Length) return null;

            int idx = GetCalculatedIdx();

            int.TryParse(_pattern[idx], out int next);
            return next;
        }

        public string GetNextAction()
        {
            if (_pattern.Length == 0 || _currentStep >= _pattern.Length)
            {
                return null;
            }

            int idx = GetCalculatedIdx() + 1;
            return idx < _pattern.Length ? _pattern[idx] : null;
        }

        public int? Next()
        {
            if (_remainingLoops >= 0)
            {
                _loopStep += 1;
                if (_loopStep >= _loopLength)
                {
                    _loopStep = 0;
                    _remainingLoops -= 1;
                    if (_remainingLoops == 0)
                    {
                        _currentStep += 2;
                        _remainingLoops = -1;
                    }
                }
            }
            else
            {
                _currentStep += 2;
            }
            if (_globalLoop && _currentStep >= _pattern.Length)
            {
                _currentStep = 0;
            }

            return GetNextExecution();
        }

        public int GetCurrentStep()
        {
            return _currentStep;
        }

        private int GetCalculatedIdx()
        {
            string cur = _pattern[_currentStep];
            if (_remainingLoops <= 0 && cur.StartsWith("loop"))
            {
                string[] arr = cur.Split(' ');
                _loopLength = int.Parse(arr[1].Trim());

                string loopCountStr = _pattern[_currentStep + 1];
                _loopCount = loopCountStr == "*" ? int.MaxValue : int.Parse(loopCountStr);
                _loopStep = 0;
                _loopStart = _currentStep - 2 * _loopLength;
                _remainingLoops = _loopCount;
            }
            if (_remainingLoops > 0) return _loopStart + _loopStep * 2;

            return _currentStep;
        }
    }
}