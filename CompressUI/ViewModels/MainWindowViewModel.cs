using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Input;
using Avalonia.Controls;
using Compress;
using CompressUI.Views;
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