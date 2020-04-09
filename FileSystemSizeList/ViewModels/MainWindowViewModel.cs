using FileSystemSizeList.Models;
using Prism.Mvvm;
using Reactive.Bindings;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSystemSizeList.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private ReactivePropertySlim<string> _selectedPath;

        public ReadOnlyReactivePropertySlim<string> SelectedPath
            => _selectedPath.ToReadOnlyReactivePropertySlim();

        public ReadOnlyReactiveCollection<FileSystemInfo> FileSystemInfoList { get; }

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
        }

        public Task SelectPathAsync()
        {
            using var dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK)
                return Task.CompletedTask;

            _selectedPath.Value = dialog.SelectedPath;
            return _model.UpdateFileSystemInfoListAsync(
                _selectedPath.Value
            );
        }
    }
}