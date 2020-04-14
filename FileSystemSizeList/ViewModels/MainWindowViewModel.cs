using System;
using FileSystemSizeList.Models;
using Prism.Mvvm;
using Reactive.Bindings;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using System.Linq;

namespace FileSystemSizeList.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ReactivePropertySlim<string> _selectedPath;

        public ReadOnlyReactivePropertySlim<string> SelectedPath
            => _selectedPath.ToReadOnlyReactivePropertySlim();

        public ReadOnlyReactiveCollection<FileSystemInfo> FileSystemInfoList { get; }

        public ReadOnlyReactivePropertySlim<long> FileCountSum { get; }

        public ReadOnlyReactivePropertySlim<long> ByteSizeSum { get; }

        public AsyncReactiveCommand SelectPathCommand { get; }

        private readonly MainModel _model;

        public MainWindowViewModel()
        {
            _model = new MainModel();

            _selectedPath = new ReactivePropertySlim<string>();
            SelectPathCommand = new AsyncReactiveCommand()
                .WithSubscribe(SelectPathAsync);
            FileSystemInfoList = _model.FileSystemInfoList
                .ToReadOnlyReactiveCollection();

            FileCountSum = FileSystemInfoList
                .CollectionChangedAsObservable()
                .ToUnit()
                .Select(_ => FileSystemInfoList.Sum(x => x.FileCount))
                .ToReadOnlyReactivePropertySlim();
            ByteSizeSum = FileSystemInfoList
                .CollectionChangedAsObservable()
                .ToUnit()
                .Select(_ => FileSystemInfoList.Sum(x => x.ByteSize))
                .ToReadOnlyReactivePropertySlim();
        }

        public Task SelectPathAsync()
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
                return Task.CompletedTask;

            _selectedPath.Value = dialog.SelectedPath;
            return _model.UpdateFileSystemInfoListAsync(
                dialog.SelectedPath
            );
        }
    }
}