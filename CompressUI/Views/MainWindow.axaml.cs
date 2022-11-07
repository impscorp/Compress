using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

namespace CompressUI.Views
{
    public partial class MainWindow : Window
    {
        public Border Loading { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            Loading = this.FindControl<Border>("Border");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private async void Button_Compress(object? sender, RoutedEventArgs e)
        {
            Loading.IsVisible = true;
            await Dispatcher.UIThread.InvokeAsync(CompressTask, DispatcherPriority.Input);
            Loading.IsVisible = false;
        }

        private async void Button_Decompress(object? sender, RoutedEventArgs e)
        {
            Loading.IsVisible = true;
            await Dispatcher.UIThread.InvokeAsync(DecompressTask, DispatcherPriority.Input);
            Loading.IsVisible = false;
        }
        
        async Task<string> CompressTask()
        {

            var x = File.ReadAllText("kühl copy.rtf");
            Compress.Encode.EncodeString(x, "tree","huff");

            return "Compress";
            
        }
        async Task<string> DecompressTask()
        {

            var y = Compress.Encode.DecodeString("tree", "huff");
            File.WriteAllText("test2.rtf", y);
            this.FindControl<TextBox>("Box").Text = y;

            return "success";
            
        }
    }
}