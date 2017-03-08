using System;
using System.IO;
using System.Linq;
using TechnoRex.ResultProvider.Types;

namespace TechnoRex.ResultProvider
{
    [Serializable()]
    public class ResultConfig
    {
        public string AppName { get; set; }
        public string LogDirPath { get; set; }
        public bool LogOnlyWhenError { get; set; }
        public bool NoTrace { get; set; }

        internal string GetProperLogDirPath()
        {
            return Path.Combine(LogDirPath, AppName);
        }
        public void ClearLogDirPath()
        {
            try
            {
                if (string.IsNullOrEmpty(LogDirPath)) return;
                if (Directory.Exists(LogDirPath) == false) return;
                Directory.Delete(LogDirPath, true);
            }
            catch (Exception)
            {
            }
        }
        public void ClearLogDirIfBiggerThan(int size, FileSizeType sizeType)
        {
            if (string.IsNullOrEmpty(LogDirPath)) return;
            if (Directory.Exists(LogDirPath) == false) return;
            var dInfo = new DirectoryInfo(LogDirPath);
            var sizeOfDir = DirectorySize(dInfo, true);

            switch (sizeType)
            {
                case FileSizeType.KB:
                    {
                        if (sizeOfDir >= (double)sizeOfDir / 1024)
                            ClearLogDirPath();
                    }; break;

                case FileSizeType.MB:
                    {
                        if (sizeOfDir > (double)sizeOfDir / (1024 * 1024))
                            ClearLogDirPath();
                    }; break;
            }
        }

        private long DirectorySize(DirectoryInfo dInfo, bool includeSubDir)
        {
            // Enumerate all the files
            var totalSize = dInfo.EnumerateFiles()
                .Sum(file => file.Length);

            // If Subdirectories are to be included
            if (includeSubDir)
                totalSize += dInfo.EnumerateDirectories()
                    .Sum(dir => DirectorySize(dir, true));
            return totalSize;
        }
    }
}