using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Exam_SearchForbiddenWords_Karvatyuk
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window
    {
        IEnumerable<string> textFiles = Enumerable.Empty<string>();
        string[] fbWordsList = null;

        public LogWindow(MainWindow mainwindow)
        {
            InitializeComponent();

            textFiles = mainwindow.TextFiles;
            fbWordsList = mainwindow.FbWordsList;
        }

        private void Click_btSaveToFile(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), $"Log_{DateTime.Now}.log");
            File.WriteAllText($"Log_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.log", tbkLog.Text);
        }

        private void Click_btClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DubRewBt_Click(object sender, RoutedEventArgs e)
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
                        string newPath = Path.GetDirectoryName(filePath)+ "\\"
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
}
