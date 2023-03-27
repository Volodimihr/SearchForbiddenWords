using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Exam_SearchForbiddenWords_Karvatyuk
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern IntPtr ShowWindow(IntPtr hwnd, int nCmdShow);
        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        bool isFileExist = false, isDirExist = false;
        string fbWordsListPath = string.Empty;
        string searchingFolderPath = string.Empty;
        IEnumerable<string> textFiles = Enumerable.Empty<string>();
        string ext = string.Empty;
        object obj = null;
        int tfcount = 0;
        string[] fbWordsList = null;
        string logString = string.Empty;
        string fow = string.Empty;
        int fowTimes = 0;
        LogItem[] logItems = null;
        List<LogItem> itemsList = null;
        const char block = '■';
        int p = 0;

        protected override void OnStartup(StartupEventArgs e)
        {

            IntPtr handle = GetConsoleWindow();

            if (e.Args.Length > 0)
            {
                if (e.Args[0] == "help" || e.Args[0] == "-h")
                {
                    Console.WriteLine("\n SearchForbiddenWords.exe [text file with list of forbidden words] [searching directory] [searching file extension]\n");
                    Console.WriteLine(" Exaple: SearchForbiddenWords.exe C:\\Stock\\forbiddenwords.txt E:\\Documents *.txt\n");
                    
                    App.Current.Shutdown();
                    return;
                }

                if (ArgsIsCorrect(e.Args))
                {
                    p = 0;
                    obj = new object();
                    ext = e.Args[2];
                    textFiles = GetDirectoryFiles(searchingFolderPath, ext, SearchOption.AllDirectories);
                    tfcount = textFiles.Count();

                    fbWordsList = ParallelReadFile(fbWordsListPath);

                    itemsList = new List<LogItem>();

                    Console.WriteLine();
                    DoWork();

                    GetResult();

                    char q = '\0';
                    Console.Write("\n Show results? (y/n) ");
                    q = Console.ReadKey().KeyChar;
                    if (q == 'y' || q == 'Y')
                    {
                        Console.WriteLine(logString);
                    }

                    Console.Write("\n Save results? (y/n) ");
                    q = Console.ReadKey().KeyChar;
                    if (q == 'y' || q == 'Y')
                    {
                        File.WriteAllText($"Log_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log", logString);
                    }

                    Console.Write("\n Rewrite and save duplicates? (y/n) ");
                    q = Console.ReadKey().KeyChar;
                    if (q == 'y' || q == 'Y')
                    {
                        try
                        {
                            foreach (string filePath in textFiles)
                            {
                                bool isFbFile = false;
                                string fileText = File.ReadAllText(filePath);
                                foreach (string fbWord in fbWordsList)
                                {
                                    if (fileText.Contains(fbWord)) isFbFile = true;

                                    fileText = Regex.Replace(fileText, fbWord, "*******", RegexOptions.IgnoreCase);
                                }

                                if (isFbFile)
                                {
                                    string newPath = Path.GetDirectoryName(filePath) + "\\"
                                        + Path.GetFileNameWithoutExtension(filePath) + $"_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}"
                                        + Path.GetExtension(filePath);

                                    File.Move(filePath, newPath);
                                    File.WriteAllText(filePath, fileText);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }

                App.Current.Shutdown();
            }
            else
            {
                StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
                base.OnStartup(e);

                ShowWindow(handle, SW_HIDE);
            }
        }

        private bool ArgsIsCorrect(string[] args)
        {
            string filePath = Path.GetFullPath(args[0]);
            string dirPath = Path.GetFullPath(args[1]);

            if (File.Exists(filePath))
            {
                fbWordsListPath = filePath;
                isFileExist = true;
            }
            else
                Console.WriteLine($" File \"{filePath}\" does not exist!");

            if (Directory.Exists(dirPath))
            {
                searchingFolderPath = dirPath;
                isDirExist = true;
            }
            else
                Console.WriteLine($" Directory \"{dirPath}\" does not exist!");

            if (isFileExist && isDirExist)
            {
                Console.WriteLine(" File: " + fbWordsListPath);
                Console.WriteLine(" Directory: " + searchingFolderPath);

                return true;
            }
            else return false;
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

        private void DoWork()
        {
            Console.CursorVisible = false;
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
                        });

                        threads[i].Start();
                        threads[i].Join();

                        Thread.Sleep(10);
                    };

                    lock (obj)
                    {
                        ++p;
                        Console.SetCursorPosition(0, Console.CursorTop);
                        Console.Write($" {p} / {tfcount} [");
                        int percent = (p * 100) / tfcount;
                        int b = (int)((percent / 5f) + .5f);
                        for (var i = 0; i < 20; ++i)
                        {
                            if (i >= b)
                                Console.Write(' ');
                            else
                                Console.Write(block);
                        }
                        Console.Write("] {0,3:##0}%", percent);
                    }
                });

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
            finally { Console.WriteLine("\n\n"); Console.CursorVisible = true; }

            logItems = itemsList.ToArray();
        }

        private void GetResult()
        {
            logString = string.Empty;
            logString += $"Log_{DateTime.Now}\n";
            logString += $"Root searching directory: {searchingFolderPath}\n";
            logString += $"Text file of forbidden words: {fbWordsListPath}\n\n";

            foreach (string w in fbWordsList)
            {
                int max = 0;
                bool isFound = false;
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

                if (max > fowTimes)
                {
                    fow = w;
                    fowTimes = max;
                }

                if (isFound == false) logString += $" not found!\n\n";
                logString += $"-------------================-------------\n\n";
            }

            logString += $"Frequently occurring word: {fow} = {fowTimes}\n\n";
        }
    }
}
