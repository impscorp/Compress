using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Compress;

namespace CompressUI.Views
{
    public partial class MainWindow : Window
    {
        public Border Loading { get; set; }
        public string? Savepath { get; set; }
        public string? Loadpath { get; set; }
        
        public string Treepath { get; set; }
        public Encode Encode { get; set; }
        
        public static Window? Window { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Encode = new Encode();
            Loading = this.FindControl<Border>("Border");
            Window = this;

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
        private async void Button_LoadFile(object? sender, RoutedEventArgs routedEventArgs)
        {
            this.FindControl<TextBox>("Box").Text = "";
            Loadpath = await OpenFolder();
            if (Loadpath != null)
            {
                this.FindControl<TextBox>("path").Text = Loadpath;
            }
        }
        
        async Task<string> CompressTask()
        {
            Savepath = await SaveFile();
            if (Savepath != null)
            {
                Encode.EncodeString(Loadpath!, Treepath, Savepath); 
            }
            this.FindControl<TextBox>("path").Text = Savepath;
            Loadpath = Savepath;
            return "Compress";
        }
        async Task<string> DecompressTask()
        {
            //Treepath = Encode.codeTablepath;
            string tempPath = Path.GetTempPath();
            //macos get access to the temp folder
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                tempPath = "/private" + tempPath;
            }
            string filename = Path.GetFileNameWithoutExtension(this.FindControl<TextBox>("path").Text);
            Treepath = tempPath + filename + "_codeTable.tree";
            var y = Encode.DecodeString(Treepath, Loadpath);
            Savepath = await SaveFile();
            if (Savepath != null)
            {
                //if savepath extension is .huff replace it with .txt
                if (Path.GetExtension(Savepath) == ".huff")
                {
                    Savepath = Path.ChangeExtension(Savepath, ".txt");
                }
                File.WriteAllText(Savepath, y);
                this.FindControl<TextBox>("Box").Text = y;
            }
            return "Decompress";
        }
        public async Task<string> OpenFolder()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open File";
            dialog.Filters.Add(new FileDialogFilter { Name = "Text", Extensions = { "txt" , "rtf", "huff" } });
            dialog.Filters.Add(new FileDialogFilter { Name = "All", Extensions = { "*" } });
            var result = await dialog.ShowAsync(Window);
            return result[0];
        }

        public async Task<string> SaveFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save File";
            dialog.InitialFileName = Path.GetFileName(this.FindControl<TextBox>("path").Text);
            dialog.Filters.Add(new FileDialogFilter { Name = "Text", Extensions = { "huff", "txt"} });
            var result = await dialog.ShowAsync(Window);
            return result;
        }
    }
}