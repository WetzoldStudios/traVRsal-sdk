using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class VerificationResult
    {
        public DateTime checkTime;
        public long sourceSize;
        public bool documentationExists;
        public string documentationPath;

        public string distroPathAndroid;
        public bool distroExistsAndroid;
        public long distroSizeAndroid;

        public string distroPathStandalone;
        public bool distroExistsStandalone;
        public long distroSizeStandalone;

        public bool showDetails = true;

        public VerificationResult()
        {
            checkTime = DateTime.Now;
        }
    }
}