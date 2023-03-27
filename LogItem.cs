namespace Exam_SearchForbiddenWords_Karvatyuk
{
    internal class LogItem
    {
        public string Word { get; set; }
        public string FilePath { get; set; }
        public int[] Indexes { get; set; }

        public LogItem(string word, string filePath, int[] indexes)
        {
            Word = word;
            FilePath = filePath;
            Indexes = indexes;
        }
    }
}
