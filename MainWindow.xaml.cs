using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Exam_SearchForbiddenWords_Karvatyuk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        string fbWordsListPath = string.Empty;
        string searchingFolderPath = string.Empty;
        IEnumerable<string> textFiles = Enumerable.Empty<string>();
        int tfcount;
        string[] fbWordsList = null;
        string[] ext = new string[] { "*.txt", "*.ini", "*.log", "*.html" };
        string logString = string.Empty;
        string fow = string.Empty;
        int fowTimes = 0;
        LogItem[] logItems = null;
        List<LogItem> itemsList = null;
        LogWindow logWindow = null;
        int p;
        string sp = "0 / 0";
        Visibility visible = Visibility.Hidden;
        bool enabled = false;

        public IEnumerable<string> TextFiles
        {
            get { return textFiles; }
            set { textFiles = value; }
        }

        public string[] FbWordsList
        {
            get { return fbWordsList; }
            set { fbWordsList = value; }
        }

        public int P
        {
            get
            { return p; }
            set
            {
                if (p != value)
                {
                    p = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Sp
        {
            get { return sp; }
            set { sp = value; OnPropertyChanged(); }
        }

        public Visibility Visible
        { get { return visible; } set { visible = value; OnPropertyChanged(); } }

        public bool Enabled
        { get { return enabled; } set { enabled = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public MainWindow()
        {
            InitializeComponent();
            btStart.IsEnabled = false;

            cbExt.ItemsSource = ext;

            DataContext = this;
        }

        private void Click_btSearchFolderDialogOpen(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbdialog = new FolderBrowserDialog();
            fbdialog.SelectedPath = Directory.GetCurrentDirectory();
            if (fbdialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                searchingFolderPath = fbdialog.SelectedPath;
                tbkSFolder.Text = searchingFolderPath;
                if (fbWordsListPath != string.Empty)
                    btStart.IsEnabled = true;
            }
        }

        private void Click_btFbWordList(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = $"Text files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                fbWordsListPath = openFileDialog.FileName;
                tbkFileList.Text = fbWordsListPath;

                fbWordsList = ParallelReadFile(fbWordsListPath);

                if (searchingFolderPath != string.Empty)
                    btStart.IsEnabled = true;
            }
        }

        private IEnumerable<string> GetDirectoryFiles(string rootPath, string patternMatch, SearchOption searchOption)
        {
            var foundFiles = Enumerable.Empty<string>();
            if (searchOption == SearchOption.AllDirectories)
            {
                try
                {
                    IEnumerable<string> subDirs = Directory.EnumerateDirectories(rootPath);
                    foreach (string dir in subDirs)
                    {
                        foundFiles = foundFiles.Concat(GetDirectoryFiles(dir, patternMatch, searchOption));
                    }
                }
                catch (UnauthorizedAccessException) { }
                catch (PathTooLongException) { }
            }

            try
            {
                foundFiles = foundFiles.Concat(Directory.EnumerateFiles(rootPath, patternMatch));
            }
            catch (UnauthorizedAccessException) { }

            return foundFiles;
        }

        private string[] ParallelReadFile(string filePath)
        {
            List<string> wordsList = new List<string>();
            Parallel.ForEach<string>(File.ReadLines(filePath), line =>
            {
                foreach (string w in line.Split(' '))
                {
                    wordsList.Add(w);
                }
            });

            return wordsList.ToArray();
        }

        private async Task Worker_DoWork()
        {
            await Task.Run(() =>
            {
                try
                {

                    Parallel.ForEach(textFiles, filePath =>
                    {
                        string fileText = File.ReadAllText(filePath);
                        Thread[] threads = new Thread[fbWordsList.Length];
                        for (int i = 0; i < threads.Length; i++)
                        {
                            threads[i] = new Thread(() =>
                            {
                                MatchCollection matches = Regex.Matches(fileText, fbWordsList[i], RegexOptions.IgnoreCase);
                                if (matches.Count > 0)
                                {
                                    int[] indexes = new int[matches.Count];
                                    for (int j = 0; j < matches.Count; j++)
                                        indexes[j] = matches[j].Index;
                                    itemsList.Add(new LogItem(fbWordsList[i], filePath, indexes));
                                }
                                //Thread.Sleep(20);
                            });

                            threads[i].Start();
                            threads[i].Join();
                        };

                        pbSearchProgress.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            ++P;
                            Sp = $"{P} / {tfcount}";
                            if (P == tfcount)
                            {
                                Thread.Sleep(1000);
                                Visible = Visibility.Visible;
                                Enabled = true;
                            }
                        }));
                    });

                }
                catch (Exception ex) { System.Windows.MessageBox.Show(ex.Message); }

                logItems = itemsList.ToArray();
            });
        }

        private void PrintResult()
        {
            logString = string.Empty;
            logString += $"Log_{DateTime.Now}\n";
            logString += $"Root searching directory: {searchingFolderPath}\n";
            logString += $"Text file of forbidden words: {fbWordsListPath}\n\n";

            foreach (string w in fbWordsList)
            {
                bool isFound = false;
                int max = 0;
                logString += $" - \"{w}\":";
                foreach (LogItem logItem in logItems)
                {
                    if (logItem.Word == w)
                    {
                        isFound = true;
                        logString += $"\n\tfile: {logItem.FilePath}\n";
                        logString += $"\tfounded: {logItem.Indexes.Length}\n";
                        logString += $"\tindexes:";
                        foreach (int i in logItem.Indexes) logString += $" {i}";
                        logString += $"\n\n";

                        max += logItem.Indexes.Length;
                    }
                }

                if(max > fowTimes)
                {
                    fow = w;
                    fowTimes = max;
                }

                if (isFound == false) logString += $" not found!\n\n";
                logString += $"-------------================-------------\n\n";
            }

            logString += $"Frequently occurring word: {fow} = {fowTimes}\n\n";

            logWindow = new LogWindow(this);
            logWindow.tbkLog.Text = logString;
            logWindow.Show();
        }

        private async void Click_btStartSearching(object sender, RoutedEventArgs e)
        {

            P = 0;
            Visible = Visibility.Hidden;
            Enabled = false;
            textFiles = GetDirectoryFiles(searchingFolderPath, cbExt.SelectedValue.ToString(), SearchOption.AllDirectories);
            tfcount = textFiles.Count();
            pbSearchProgress.Maximum = textFiles.Count();
            pbSearchProgress.Value = 0;
            itemsList = new List<LogItem>();

            await Worker_DoWork();

        }

        private void OnClick_btInfo(object sender, RoutedEventArgs e)
        {
            PrintResult();
        }
    }
}
