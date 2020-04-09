using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileSystemSizeList.Models
{
    public class MainModel
    {
        private readonly ObservableCollection<FileSystemInfo> _fileSystemInfoList;
        public ReadOnlyObservableCollection<FileSystemInfo> FileSystemInfoList { get; }

        public MainModel()
        {
            _fileSystemInfoList = new ObservableCollection<FileSystemInfo>();
            FileSystemInfoList = new ReadOnlyObservableCollection<FileSystemInfo>(
                _fileSystemInfoList
            );
        }

        public Task UpdateFileSystemInfoListAsync(string path)
        {
            _fileSystemInfoList.Clear();

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
                    _fileSystemInfoList.Add(
                        new FileSystemInfo(names, Dfs(nxtPath)));
                }));
            return Task.WhenAll(tasks);
        }
    }
}