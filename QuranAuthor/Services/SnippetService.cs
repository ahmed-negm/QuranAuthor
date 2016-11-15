using QuranAuthor.Models;
using QuranAuthor.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace QuranAuthor.Services
{
    public class SnippetService
    {
        private ChapterRepository chapterRepository;
        private VerseRepository verseRepository;

        public SnippetService()
        {
            this.chapterRepository = new ChapterRepository();
            this.verseRepository = new VerseRepository();
        }

        public Snippet ExtractSnippet(string rtf)
        {
            var snippet = new Snippet();

            var reachTextBox = new RichTextBox();

            MemoryStream stream = new MemoryStream(ASCIIEncoding.Default.GetBytes(rtf));
            reachTextBox.Selection.Load(stream, DataFormats.Rtf);

            var text = new TextRange(reachTextBox.Document.ContentStart, reachTextBox.Document.ContentEnd).Text;

            var terms = text.Split(Environment.NewLine.ToArray())[2].Trim().Split(':');

            var chapter = this.chapterRepository.GetChapters().FirstOrDefault(C => C.Name == terms[0].Trim());

            if(chapter == null)
            {
                throw new Exception("Can't find chapter called: " + terms[0].Trim());
            }

            snippet.ChapterId = chapter.Id;

            var verseNumbers = terms[1].Trim().Split('-');

            if (verseNumbers.Length == 1)
            {
                snippet.StartVerse = ParseArabicNumber(verseNumbers[0].Trim());
                snippet.EndVerse = ParseArabicNumber(verseNumbers[0].Trim());
            }
            else
            {
                snippet.StartVerse = ParseArabicNumber(verseNumbers[0].Trim());
                snippet.EndVerse = ParseArabicNumber(verseNumbers[1].Trim());
            }

            var verses = this.verseRepository.GetVerses(snippet.ChapterId, snippet.StartVerse, snippet.EndVerse);

            if(!verses.Any())
            {
                throw new Exception(string.Format("No verses for chapter: {0}, start: {1}, end: {2}", snippet.ChapterId, snippet.StartVerse, snippet.EndVerse));
            }

            snippet.Page = verses[0].Page;

            snippet.Text = verses.Select(V => V.Text).Aggregate((A, B) => A + Environment.NewLine + B);

            snippet.Rtf = rtf;

            return snippet;
        }

        private int ParseArabicNumber(string arabicNumbers)
        {
            string EnglishNumbers = "";
            for (int i = 0; i < arabicNumbers.Length; i++)
            {
                EnglishNumbers += char.GetNumericValue(arabicNumbers, i);
            }
            return Convert.ToInt32(EnglishNumbers);
        }
         
    }
}
