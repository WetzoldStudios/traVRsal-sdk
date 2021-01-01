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

        public string distroPathStandaloneWin;
        public bool distroExistsStandaloneWin;
        public long distroSizeStandaloneWin;

        public string distroPathStandaloneLinux;
        public bool distroExistsStandaloneLinux;
        public long distroSizeStandaloneLinux;

        public bool showDetails = true;

        public VerificationResult()
        {
            checkTime = DateTime.Now;
        }
    }
}