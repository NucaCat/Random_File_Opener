using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;

namespace Random_File_Opener_Win_Forms
{
    public partial class Form1 : Form
    {
        private static readonly string _emptyFilter = "*";
        private static readonly string _settingsFileName = "!appsettings.json";

        private SearchOption _searchOption = SearchOption.AllDirectories;
        private ListItem[] _files;
        private HashSet<int> _generatedIndexes = new HashSet<int>();
        private Random _random = new Random();
        private string _currentDirectory = string.Empty;
        private string _filter;

        private Point? _itemLocation = null;

        public Form1()
        {
            InitializeComponent();

            var settings = GetSettingsFromFile();

            _filter = settings?.Filter ?? _emptyFilter;
            FilterTextBox.Text = _filter;

            SearchModeButton.Text = SearchOptionFriendlyString(_searchOption);

            var currentDirectory = Directory.GetCurrentDirectory();

            Initialize(currentDirectory, _filter);
        }

        private static Settings GetSettingsFromFile()
        {
            var fileExists = File.Exists(_settingsFileName);
            if (!fileExists)
                return null;
            
            var settingsJson = new StreamReader(_settingsFileName).ReadToEnd();

            var settings = JsonConvert.DeserializeObject<Settings>(settingsJson);
            return settings;
        }

        private void Initialize(string directory, string filter)
        {
            _currentDirectory = directory;
            _filter = filter;
            DirectoryTextBox.Text = directory.Substring(directory.LastIndexOf('\\') + 1);

            listBox1.Items.Clear();
            _files = Directory.GetFiles(directory, filter, _searchOption)
                .Select(u => {
                    var lastIndex = u.LastIndexOf("\\", StringComparison.InvariantCulture);
                    var fileName = ExtractFileName(u, lastIndex);
                    return new ListItem
                    {
                        Path = u,
                        DisplayValue = fileName 
                                       + " "
                                       + ExtractDirectory(directory, u, lastIndex),
                        FileName = fileName
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
            GetFileFromPointAndOpen(e.Location, OpenVariants.OpenFile);
        }

        private ListItem ListItemFromPoint(Point point)
        {
            var indexFromPoint = listBox1.IndexFromPoint(point);
            if (indexFromPoint == -1)
                return null;

            var listItem = (ListItem)listBox1.Items[indexFromPoint];
            return listItem;
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetData(DataFormats.StringFormat, listBox1.SelectedItem.ToString());
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                GetFileAndOpen((ListItem)listBox1.SelectedItem, OpenVariants.OpenInExplorer);
                return;
            }
        }

        private void GetFileAndOpen(ListItem item, OpenVariants openVariants)
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments = openVariants == OpenVariants.OpenFile
                ? item.Path
                : $"/select, \"{item.Path}\"",
                FileName = "explorer.exe",
            };

            Process.Start(startInfo);
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

        private void ApplyFilter_Click(object sender, EventArgs e)
        {
            if (FilterTextBox.Text == _filter)
                return;
            
            if (string.IsNullOrWhiteSpace(FilterTextBox.Text))
                FilterTextBox.Text = _emptyFilter;

            _filter = FilterTextBox.Text;
            Initialize(_currentDirectory, _filter);
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_itemLocation.HasValue)
            {
                MessageBox.Show($"{nameof(_itemLocation)} is null");
                return;
            }
            GetFileFromPointAndOpen(_itemLocation.Value, OpenVariants.OpenFile);
        }

        private void OpenInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_itemLocation.HasValue)
            {
                MessageBox.Show($"{nameof(_itemLocation)} is null");
                return;
            }
            GetFileFromPointAndOpen(_itemLocation.Value, OpenVariants.OpenInExplorer);
        }

        private void GetFileFromPointAndOpen(Point location, OpenVariants openVariant)
        {
            var listItem = ListItemFromPoint(location);
            if (listItem == null)
                return;

            GetFileAndOpen(listItem, openVariant);
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _itemLocation = null;
            
            if (e.Button != MouseButtons.Right) 
                return;

            var item = ListItemFromPoint(e.Location);
            if (item != null)
            {
                _itemLocation = e.Location;
                ListBoxContextMenuStrip.Show(Cursor.Position);
            }
        }

        private void FileAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_itemLocation.HasValue)
            {
                MessageBox.Show($"{nameof(_itemLocation)} is null");
                return;
            }
            
            var fileFromLocation = ListItemFromPoint(_itemLocation.Value);
            Clipboard.SetData(DataFormats.StringFormat, fileFromLocation.Path);
        }

        private void FileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_itemLocation.HasValue)
            {
                MessageBox.Show($"{nameof(_itemLocation)} is null");
                return;
            }
            
            var fileFromLocation = ListItemFromPoint(_itemLocation.Value);
            Clipboard.SetData(DataFormats.StringFormat, fileFromLocation.FileName);
        }

        private class ListItem
        {
            public string DisplayValue { get; set; }
            public string Path { get; set; }
            public string FileName { get; set; }

            public override string ToString()
                => DisplayValue;
        }
        
        private enum OpenVariants
        {
            OpenFile = 0,
            OpenInExplorer = 1
        }
    }
}