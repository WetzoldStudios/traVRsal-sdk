using System;

namespace traVRsal.SDK
{
    [Serializable]
    public class FileDetails
    {
        public string name;
        public DateTime createDate;
        public DateTime modifyDate;
        public long size;

        public FileDetails()
        {
        }

        public FileDetails(string name, long size, DateTime createDate, DateTime modifyDate)
        {
            this.name = name;
            this.size = size;
            this.createDate = createDate;
            this.modifyDate = modifyDate;
        }

        public override string ToString()
        {
            return $"File Info ({name})";
        }
    }
}