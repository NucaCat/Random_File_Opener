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

        private void Initialize(string directory)
        {
            _currentDirectory = directory;
            DirectoryTextBox.Text = directory.Substring(directory.LastIndexOf('\\') + 1);

            listBox1.Items.Clear();
            _files = Directory.GetFiles(directory, "*.mp4", _searchOption)
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

            var currentDirectory = Directory.GetCurrentDirectory();

            Initialize(currentDirectory);
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
            var listItem = (ListItem)listBox1.Items[listBox1.IndexFromPoint(e.Location)];
            
            var startInfo = new ProcessStartInfo
            {
                Arguments = listItem.Path,
                FileName = "explorer.exe"
            };

            Process.Start(startInfo);
        }

        private void SearchModeButton_Click(object sender, EventArgs e)
        {
            _searchOption = ChangeSearchOptions(_searchOption);

            SearchModeButton.Text = SearchOptionFriendlyString(_searchOption);
            
            Initialize(_currentDirectory);
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
                    Initialize(openFolderDialog.FileName);
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
    }
}