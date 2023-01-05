using System.Windows.Input;
using ReactiveUI;

namespace CompressUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            
            Searchfolder = ReactiveCommand.Create(() =>
            {
            });
        }
        public ICommand Searchfolder { get; }

    }
    
}