﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
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
        private ArrayWithPointer<GeneratedFileListItem> _files = new ArrayWithPointer<GeneratedFileListItem>();
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

            Styles.FillFromSettings(settings?.Styles);
            Styler.ApplyStyles(this);

            _filter = settings?.Filter ?? Consts.EmptyFilter;
            Consts.VideoThumbnailPositions = settings?.VideoThumbnailPositions.IsEmpty() == true
                ? Consts.VideoThumbnailPositions
                // ReSharper disable once PossibleNullReferenceException
                : settings.VideoThumbnailPositions;
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
            _files = new ArrayWithPointer<GeneratedFileListItem>
            {
                Entities = Directory.GetFiles(directory, filter, _searchOption)
                    .Shuffle(_random)
                    .Select(u =>
                    {
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
                    .ToArray(),
            };
        }

        private void NextFileButton_Click(object sender, EventArgs e)
        {
            var file = _files.GetCurrentAndMoveNext();
            if (file == null)
                return;

            GeneratedFilesListBox.InvokeIfRequired(() => GeneratedFilesListBox.Items.Add(file));

            AddImageToPreview(file);
        }

        private void AddImageToPreview(GeneratedFileListItem file)
        {
            file.Images = file.Images.Length != 0
            ? file.Images
            : ImageService.GetFitImages(file, ImagePictureBox, _pictureBoxesInSequence);

            if (file.Images.Length == 0)
                return;
            
            if (file.Images.Length == 1)
            {
                PlaceImageInBigPictureBox(file.Images);
                return;
            }
            
            PlaceImageInSmallPictureBoxes(file.Images);
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

        private void GeneratedFilesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            GetFileFromPointAndOpen(e.Location, OpenVariants.OpenFile);
        }

        private void GeneratedFilesListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetFileDropList(new StringCollection
                {
                    ((GeneratedFileListItem)GeneratedFilesListBox.SelectedItem).Path,
                });
                return;
            }
            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                var highlightedItem = GeneratedFilesListBox.SelectedItem;
                if (highlightedItem == null)
                    return;
                
                if (e.KeyCode == Keys.Up && GeneratedFilesListBox.SelectedIndex != 0)
                    AddImageToPreview((GeneratedFileListItem)GeneratedFilesListBox.Items[GeneratedFilesListBox.SelectedIndex - 1]);
                
                if (e.KeyCode == Keys.Down && GeneratedFilesListBox.SelectedIndex != GeneratedFilesListBox.Items.Count - 1)
                    AddImageToPreview((GeneratedFileListItem)GeneratedFilesListBox.Items[GeneratedFilesListBox.SelectedIndex + 1]);
                
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

        private void GeneratedFilesListBox_MouseUp(object sender, MouseEventArgs e)
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
            => CopyItemToClipboard(CopyOptions.Path);

        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
            => CopyItemToClipboard(CopyOptions.File);

        private void FileNameToolStripMenuItem_Click(object sender, EventArgs e)
            => CopyItemToClipboard(CopyOptions.FileName);

        private void CopyItemToClipboard(CopyOptions option)
        {
            if (!_itemLocation.HasValue)
                return;
            
            var fileFromLocation = Utilities.ListItemFromPoint(GeneratedFilesListBox, _itemLocation.Value);
            
            if (option == CopyOptions.FileName)
                Clipboard.SetData(DataFormats.StringFormat, fileFromLocation.FileName);
            
            if (option == CopyOptions.Path)
                Clipboard.SetData(DataFormats.StringFormat, fileFromLocation.Path);
            
            if (option == CopyOptions.File)
                Clipboard.SetFileDropList(new StringCollection { fileFromLocation.Path });
        }

        private enum CopyOptions
        {
            FileName,
            Path,
            File
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

        #region dark title bar 
        [DllImport("DwmApi")] //System.Runtime.InteropServices
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }
        #endregion dark title bar
    }
}