using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Random_File_Opener_Win_Forms
{
    public partial class Form1 : Form
    {
        private SearchOption _searchOption = SearchOption.AllDirectories;
        private ListItem[] _files;
        private HashSet<int> _generatedIndexes = new HashSet<int>();
        private Random _random = new Random();
        private string _currentDirectory = string.Empty;
        private string _filter = "*.mp4";
        private string _emptyFilter = "*";

        private void Initialize(string directory, string filter)
        {
            _currentDirectory = directory;
            _filter = filter;
            DirectoryTextBox.Text = directory.Substring(directory.LastIndexOf('\\') + 1);

            listBox1.Items.Clear();
            _files = Directory.GetFiles(directory, filter, _searchOption)
                .Select(u => {
                    var lastIndex = u.LastIndexOf("\\", StringComparison.InvariantCulture);
                    return new ListItem
                    {
                        Path = u,
                        DisplayValue = ExtractFileName(u, lastIndex) 
                                       + " "
                                       + ExtractDirectory(directory, u, lastIndex) 
                    };
                })
                .ToArray();
            _generatedIndexes = new HashSet<int>();
        }

        private string ExtractDirectory(string directory, string u, int lastIndex)
        {
            var str = RemoveTrailingSlash(u.Substring(directory.Length + 1, lastIndex - directory.Length));
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;

            return $"({str})";
        }

        private static string ExtractFileName(string u, int lastIndex)
        {
            return u.Substring(lastIndex + 1);
        }

        private string RemoveTrailingSlash(string substring)
        {
            if (!string.IsNullOrWhiteSpace(substring) && substring.Last() == '\\')
                return substring.Substring(0, substring.Length - 1);

            return substring;
        }

        public Form1()
        {
            InitializeComponent();

            SearchModeButton.Text = SearchOptionFriendlyString(_searchOption);
            FilterTextBox.Text = _filter;

            var currentDirectory = Directory.GetCurrentDirectory();

            Initialize(currentDirectory, _filter);
        }

        private string SearchOptionFriendlyString(SearchOption searchOption)
        {
            if (searchOption == SearchOption.AllDirectories)
                return "С подпапками";

            if (searchOption == SearchOption.TopDirectoryOnly)
                return "Без подпапок";
            
            return string.Empty;
        }

        private void NextFileButton_Click(object sender, EventArgs e)
        {
            if (_generatedIndexes.Count == _files.Length)
                return;
            
            var index = GetNewIndex();

            var file = _files[index];

            listBox1.Items.Add(file);
        }

        private int GetNewIndex()
        {
            for (;;)
            {
                var index = _random.Next(0, _files.Length);
                if (!_generatedIndexes.Contains(index))
                {
                    _generatedIndexes.Add(index);
                    return index;
                }
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            try
            {
                var indexFromPoint = listBox1.IndexFromPoint(e.Location);
                if (indexFromPoint == -1)
                    return;

                var listItem = (ListItem)listBox1.Items[indexFromPoint];
            
                var startInfo = new ProcessStartInfo
                {
                    Arguments = listItem.Path,
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
            catch (Exception exception)
            {
                using (var writer = new StreamWriter("log.txt"))
                {
                    writer.WriteLine(exception.Message);
                    writer.WriteLine();
                    writer.WriteLine(exception.StackTrace);
                    writer.WriteLine();
                    writer.WriteLine($"index: {listBox1.IndexFromPoint(e.Location)}");
                    writer.WriteLine($"listBox1.Items.Count: {listBox1.Items.Count}");
                    Dump(writer);
                }
                throw;
            }
        }

        private void SearchModeButton_Click(object sender, EventArgs e)
        {
            _searchOption = ChangeSearchOptions(_searchOption);

            SearchModeButton.Text = SearchOptionFriendlyString(_searchOption);
            
            Initialize(_currentDirectory, _filter);
        }

        private SearchOption ChangeSearchOptions(SearchOption searchOption)
        {
            if (searchOption == SearchOption.AllDirectories)
                return SearchOption.TopDirectoryOnly;

            return SearchOption.AllDirectories;
        }

        private void ChangeDirectory_Click(object sender, EventArgs e)
        {
            using (var openFolderDialog = new CommonOpenFileDialog
                  {
                      InitialDirectory = _currentDirectory,
                      IsFolderPicker = true
                  })
            {
                var result = openFolderDialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(openFolderDialog.FileName))
                {
                    Initialize(openFolderDialog.FileName, _filter);
                }
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetData(DataFormats.StringFormat, listBox1.SelectedItem.ToString());
            }
        }

        private class ListItem
        {
            public string DisplayValue { get; set; }
            public string Path { get; set; }

            public override string ToString()
                => DisplayValue;
        }

        private void ApplyFilter_Click(object sender, EventArgs e)
        {
            if (FilterTextBox.Text == _filter)
                return;
            
            if (string.IsNullOrWhiteSpace(FilterTextBox.Text))
                FilterTextBox.Text = _emptyFilter;

            _filter = FilterTextBox.Text;
            Initialize(_currentDirectory, _filter);
        }

        private void Dump(StreamWriter writer)
        {
            writer.WriteLine($"_searchOption: {_searchOption.ToString()}");
            writer.WriteLine($"_currentDirectory: {_currentDirectory}");

            writer.WriteLine();
            writer.WriteLine($"_files.Count: {_files.Length}");

            writer.WriteLine();
            writer.WriteLine($"_generatedIndexes.Count: {_generatedIndexes.Count}");
            foreach (var index in _generatedIndexes)
            {
                writer.WriteLine($"    {index}");
            }
        }
    }
}