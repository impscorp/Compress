using System.IO;
using System.Text;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace CompressUI.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Button_Compress(object? sender, RoutedEventArgs e)
        {
            //find ui element
            var border = this.FindControl<Border>("Border");
            border.IsVisible = true;
            var x = File.ReadAllText("kühl copy.rtf");
            byte[] input = Encoding.ASCII.GetBytes(x);
            Compress.Encode.EncodeString(input, "test");
            border.IsVisible = false;
        }
    }
}