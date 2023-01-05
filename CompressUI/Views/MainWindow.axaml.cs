using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Compress;
using ApplicationException = System.ApplicationException;

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
        
        /// <summary>
        /// the button event to compress the file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Compress(object? sender, RoutedEventArgs e)
        {
            Loading.IsVisible = true;
            await Dispatcher.UIThread.InvokeAsync(CompressTask, DispatcherPriority.Input);
            Loading.IsVisible = false;
        }
        
        /// <summary>
        /// the button event for decompressing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Button_Decompress(object? sender, RoutedEventArgs e)
        {
            Loading.IsVisible = true;
            await Dispatcher.UIThread.InvokeAsync(DecompressTask, DispatcherPriority.Input);
            Loading.IsVisible = false;
        }
        /// <summary>
        /// gets the file path and puts it into the loadpath variable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="routedEventArgs"></param>
        private async void Button_LoadFile(object? sender, RoutedEventArgs routedEventArgs)
        {
            try
            {
                this.FindControl<TextBox>("Box").Text = "";
                Loadpath = await OpenFolder();
                if (Loadpath != null)
                {
                    this.FindControl<TextBox>("path").Text = Loadpath;
                }
            }
            catch (ApplicationException)
            {
                this.FindControl<TextBox>("path").Text = "canceled";
            }

        }
        /// <summary>
        /// the task to compress the file
        /// </summary>
        /// <returns></returns>
        async Task<string> CompressTask()
        {
            Savepath = await SaveFile();
            if (Savepath != null)
            {
                Encode.EncodeString(Loadpath!, Treepath, Savepath); 
                //check if encode is successful
                if (File.Exists(Savepath))
                {
                    this.FindControl<TextBox>("Box").Text = "Compressed\r" + Math.Round(Encode.CompareFiles(Loadpath, Savepath)) + "%";
                }
                else
                {
                    this.FindControl<TextBox>("Box").Text = "Failed, check if file is empty";
                }
  
            }
            this.FindControl<TextBox>("path").Text = Savepath;
            Loadpath = Savepath;
            return "Compress";
        }
        /// <summary>
        /// the task to decompress the file
        /// </summary>
        /// <returns></returns>
        async Task<string> DecompressTask()
        {
            //Treepath = Encode.codeTablepath;
            // string tempPath = Path.GetTempPath();
            // //macos get access to the temp folder
            // if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            // {
            //     tempPath = "/private" + tempPath;
            // }
            string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string filename = Path.GetFileNameWithoutExtension(this.FindControl<TextBox>("path").Text);
            Treepath = exePath + filename + "_codeTable.tree";
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
                var x = Encode.CompareFiles(Savepath, Loadpath);
                this.FindControl<TextBox>("Box").Text = "Compression rate: " + Math.Round(x, 2) + "%" + "\n\n" + "Decoded String: \n" + y;
            }
            return "Decompress";
        }
        /// <summary>
        /// task to open a openfiledialog
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public async Task<string> OpenFolder()
        {
            
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Open File";
            dialog.Filters?.Add(new FileDialogFilter { Name = "Text", Extensions = { "txt" , "rtf", "huff" } });
            var result = await dialog.ShowAsync(Window);
            if (result != null)
            {
                return result[0];
            }
            throw new IndexOutOfRangeException("Canceled");
            
        }
        public async Task<string> SaveFile()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save File";
            dialog.InitialFileName = Path.GetFileName(this.FindControl<TextBox>("path").Text);
            dialog.Filters.Add(new FileDialogFilter { Name = "Text", Extensions = { "huff", "txt"} });
            dialog.DefaultExtension = "huff";
            var result = await dialog.ShowAsync(Window);
            return result;
        }
    }
}