using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Random_File_Opener_Win_Forms.Settings;
using Random_File_Opener_Win_Forms.Style;

// ReSharper disable LocalizableElement

namespace Random_File_Opener_Win_Forms
{
    public partial class Form1 : Form
    {
        private PictureBox[] _pictureBoxesInSequence;

        private SearchOption _searchOption = SearchOption.AllDirectories;
        private GeneratedFileListItem[] _files;
        private HashSet<int> _generatedIndexes = new HashSet<int>();
        private Random _random = new Random();
        private string _currentDirectory = string.Empty;
        private string _filter;
        private GenerateButtonColors _currentGenerateButtonColor = GenerateButtonColors.Red;

        private Point? _itemLocation = null;

        private static bool _shouldAutoGenerate = false;
        private TimeSpan _autoGenerateCooldown = TimeSpan.FromSeconds(2);

        public Form1()
        {
            InitializeComponent();

            Styler.ApplyStyles(this);

            InitialInitialize();

            Task.Run(StartAutoGenerate);
        }

        private void InitialInitialize()
        {
            _pictureBoxesInSequence = new[]
            {
                VideoThumbnailFirstPictureBox,
                VideoThumbnailSecondPictureBox,
                VideoThumbnailThirdPictureBox
            };

            // ReSharper disable once PossibleLossOfFraction
            AutoGenerateNumericUpDown.Value = (int)_autoGenerateCooldown.TotalMilliseconds / 1000;

            var settings = SettingsPuller.Pull<AppSettings>(Consts.SettingsFileName);

            _filter = settings?.Filter ?? Consts.EmptyFilter;
            FilterTextBox.Text = _filter;

            SearchModeButton.Text = _searchOption.ToFriendlyString();

            Styler.ChangeAutogenerateButtonColor(AutoGenerateButton, _currentGenerateButtonColor);

            var currentDirectory = Directory.GetCurrentDirectory();

            Initialize(currentDirectory, _filter);
        }

        private async Task StartAutoGenerate()
        {
            for (;;)
            {
                if (_shouldAutoGenerate)
                {
                    NextFileButton_Click(null, null);
                }

                await Task.Delay(_autoGenerateCooldown);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void Initialize(string directory, string filter)
        {
            _currentDirectory = directory;
            _filter = filter;
            DirectoryTextBox.Text = directory.Substring(directory.LastIndexOf('\\') + 1);

            GeneratedFilesListBox.Items.Clear();
            _files = Directory.GetFiles(directory, filter, _searchOption)
                .Select(u => {
                    var lastIndex = u.LastIndexOf("\\", StringComparison.InvariantCulture);
                    var fileName = Utilities.ExtractFileName(u, lastIndex);
                    return new GeneratedFileListItem
                    {
                        Path = u,
                        DisplayValue = fileName 
                                       + " "
                                       + Utilities.ExtractDirectory(directory, u, lastIndex),
                        FileName = fileName,
                    };
                })
                .ToArray();
            _generatedIndexes = new HashSet<int>();
        }

        private void NextFileButton_Click(object sender, EventArgs e)
        {
            if (_files.Length == 0 || (_files.Length == 1 && _generatedIndexes.Count == 1))
                return;
            
            if (_generatedIndexes.Count != 0 && _generatedIndexes.Count == _files.Length)
            {
                var lastGeneratedIndex = _generatedIndexes.Last(); 
                _generatedIndexes.Clear();
                _generatedIndexes.Add(lastGeneratedIndex);
            }
            
            var index = GetNewIndex();

            var file = _files[index];

            GeneratedFilesListBox.InvokeIfRequired(() => GeneratedFilesListBox.Items.Add(file));

            AddImageToPreview(file);
        }

        private void AddImageToPreview(GeneratedFileListItem file)
        {
            var images = ImageService.GetFitImages(file, ImagePictureBox, _pictureBoxesInSequence);

            if (images.Length == 0)
                return;
            
            if (images.Length == 1)
            {
                PlaceImageInBigPictureBox(images);
                return;
            }
            
            PlaceImageInSmallPictureBoxes(images);
        }

        private void PlaceImageInSmallPictureBoxes(Bitmap[] images)
        {
            ImagePictureBox.InvokeIfRequired(() => { ImagePictureBox.Visible = false; });

            foreach (var (pictureBox, image) in _pictureBoxesInSequence
                         .Zip(images.PadRightWithNulls(_pictureBoxesInSequence.Length), (u, v) => (PictureBox: u, Image: v)))
            {
                pictureBox.InvokeIfRequired(() =>
                {
                    pictureBox.Image = image;
                    pictureBox.Visible = true;
                });
            }
        }

        private void PlaceImageInBigPictureBox(Bitmap[] images)
        {
            ImagePictureBox.InvokeIfRequired(() =>
            {
                ImagePictureBox.Image = images[0];
                ImagePictureBox.Visible = true;
            });
            foreach (var pictureBox in _pictureBoxesInSequence)
            {
                pictureBox.InvokeIfRequired(() => { pictureBox.Visible = false; });
            }
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

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetFileDropList(new StringCollection
                {
                    ((GeneratedFileListItem)GeneratedFilesListBox.SelectedItem).Path,
                });
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                GetFileAndOpen((GeneratedFileListItem)GeneratedFilesListBox.SelectedItem, OpenVariants.OpenInExplorer);
                return;
            }
        }

        private void GetFileAndOpen(GeneratedFileListItem item, OpenVariants openVariants)
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
            _searchOption = _searchOption.Next();

            SearchModeButton.Text = _searchOption.ToFriendlyString();
            
            Initialize(_currentDirectory, _filter);
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

                if (result == CommonFileDialogResult.Ok && openFolderDialog.FileName.IsNotNullOrWhiteSpace())
                {
                    Initialize(openFolderDialog.FileName, _filter);
                }
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            GeneratedFilesListBox.Items.Clear();
        }

        private void ApplyFilter_Click(object sender, EventArgs e)
        {
            if (FilterTextBox.Text == _filter)
                return;
            
            if (FilterTextBox.Text.IsNullOrWhiteSpace())
                FilterTextBox.Text = Consts.EmptyFilter;

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
            var listItem = Utilities.ListItemFromPoint(GeneratedFilesListBox, location);
            if (listItem == null)
                return;

            GetFileAndOpen(listItem, openVariant);
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _itemLocation = null;
            
            var item = Utilities.ListItemFromPoint(GeneratedFilesListBox, e.Location);
            if (item == null)
                return;

            if (e.Button == MouseButtons.Right)
            {
                _itemLocation = e.Location;
                ListBoxContextMenuStrip.Show(Cursor.Position);
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                AddImageToPreview(item);
            }
        }

        private void FileAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_itemLocation.HasValue)
            {
                MessageBox.Show($"{nameof(_itemLocation)} is null");
                return;
            }
            
            var fileFromLocation = Utilities.ListItemFromPoint(GeneratedFilesListBox, _itemLocation.Value);
            Clipboard.SetData(DataFormats.StringFormat, fileFromLocation.Path);
        }

        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_itemLocation.HasValue)
            {
                MessageBox.Show($"{nameof(_itemLocation)} is null");
                return;
            }
            
            var fileFromLocation = Utilities.ListItemFromPoint(GeneratedFilesListBox, _itemLocation.Value);
            Clipboard.SetFileDropList(new StringCollection
            {
                fileFromLocation.Path,
            });
        }

        private void FileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_itemLocation.HasValue)
            {
                MessageBox.Show($"{nameof(_itemLocation)} is null");
                return;
            }
            
            var fileFromLocation = Utilities.ListItemFromPoint(GeneratedFilesListBox, _itemLocation.Value);
            Clipboard.SetData(DataFormats.StringFormat, fileFromLocation.FileName);
        }

        private void AutoGenerate_Click(object sender, EventArgs e)
        {
            _currentGenerateButtonColor = _currentGenerateButtonColor.Next();

            Styler.ChangeAutogenerateButtonColor(AutoGenerateButton, _currentGenerateButtonColor);

            _shouldAutoGenerate = !_shouldAutoGenerate;
        }

        private void AutoGenerateNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _autoGenerateCooldown = TimeSpan.FromMilliseconds((int)(AutoGenerateNumericUpDown.Value * 1000));
        }
        
        private enum OpenVariants
        {
            OpenFile = 0,
            OpenInExplorer = 1
        }
    }
}