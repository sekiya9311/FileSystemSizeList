using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace FileSystemSizeList.Models
{
    public class FileSystemInfo
        : IEquatable<FileSystemInfo>
    {
        public string Path { get; }

        public string Name => System.IO.Path.GetFileName(Path);

        public long ByteSize { get; }

        public double MegaByteSize => ByteSize / 1e6;

        public long FileCount { get; }

        public FileSystemInfo(string path, long byteSize, long fileCount)
        {
            Path = path;
            ByteSize = byteSize;
            FileCount = fileCount;
        }

        public bool Equals([AllowNull] FileSystemInfo other)
            => Path == other?.Path && ByteSize == other?.ByteSize;
    }
}
