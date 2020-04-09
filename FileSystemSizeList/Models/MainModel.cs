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

            long Dfs(string curPath)
            {
                if (File.Exists(curPath))
                    return new FileInfo(curPath).Length;

                return Directory
                    .EnumerateFileSystemEntries(curPath)
                    .Select(x =>
                    {
                        var nxtPath = Path.Combine(curPath, x);
                        return Dfs(nxtPath);
                    })
                    .Sum();
            }

            var tasks = Directory.EnumerateFileSystemEntries(path)
                .Select(names => Task.Run(() =>
                {
                    var nxtPath = Path.Combine(path, names);
                    _fileSystemInfoList.AddOnScheduler(
                        new FileSystemInfo(names, Dfs(nxtPath)));
                }));
            return Task.WhenAll(tasks);
        }
    }
}
