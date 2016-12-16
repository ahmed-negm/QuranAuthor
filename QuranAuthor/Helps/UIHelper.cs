using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuranAuthor.Helps
{
    public static class UIHelper
    {
        public static void MessageBox(string message)
        {
            System.Windows.MessageBox.Show(message);
        }

        public static bool Ask(string message)
        {
            var result = System.Windows.MessageBox.Show(message, "", MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }

        public static void SaveToFile(string content)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".text"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                File.WriteAllText(filename, content);
            }
        }

        public static string OpenFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".text"; // Default file extension
            dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                return File.ReadAllText(filename);
            }

            return string.Empty;
        }
    }
}
