using Reactive.Bindings;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileSystemSizeList.Models
{
    public class MainModel
    {
        private readonly ReactiveCollection<FileSystemInfo> _fileSystemInfoList;
        public ReadOnlyReactiveCollection<FileSystemInfo> FileSystemInfoList { get; }

        public MainModel()
        {
            _fileSystemInfoList = new ReactiveCollection<FileSystemInfo>();
            FileSystemInfoList = _fileSystemInfoList.ToReadOnlyReactiveCollection();
        }

        public Task UpdateFileSystemInfoListAsync(string path)
        {
            _fileSystemInfoList.ClearOnScheduler();

            (long byteSize, long fileCount) Dfs(string curPath)
            {
                if (File.Exists(curPath))
                    return (new FileInfo(curPath).Length, 1);

                return Directory
                    .EnumerateFileSystemEntries(curPath)
                    .Select(Dfs)
                    .DefaultIfEmpty()
                    .Aggregate((acc, cur) => (
                        acc.byteSize + cur.byteSize,
                        acc.fileCount + cur.fileCount));
            }

            var tasks = Directory.EnumerateFileSystemEntries(path)
                .Select(nxtPath => Task.Run(() =>
                {
                    var (size, count) = Dfs(nxtPath);
                    _fileSystemInfoList.AddOnScheduler(
                        new FileSystemInfo(nxtPath, size, count));
                }));
            return Task.WhenAll(tasks);
        }
    }
}
