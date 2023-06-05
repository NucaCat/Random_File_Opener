using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using NReco.VideoConverter;
using Random_File_Opener_Win_Forms.Settings;
using Random_File_Opener_Win_Forms.Style;

// ReSharper disable LocalizableElement

namespace Random_File_Opener_Win_Forms
{
    public partial class Form1 : Form
    {
        private PictureBox[] _pictureBoxesInSequence;

        private SearchOption _searchOption = SearchOption.AllDirectories;
        private ListItem[] _files;
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

            SearchModeButton.Text = SearchOptionFriendlyString(_searchOption);

            ChangeAutogenerateButtonColor();

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
                    return new ListItem
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

        private void AddImageToPreview(ListItem file)
        {
            var sourceImage = GetSourceImage(file);

            if (sourceImage.Length == 0)
                return;

            var resized = sourceImage
                .Select(ResizeImageToFitPictureBox)
                .ToArray();
            
            if (resized.Length == 1)
            {
                ImagePictureBox.InvokeIfRequired(() =>
                {
                    ImagePictureBox.Image = resized[0];
                    ImagePictureBox.Visible = true;
                });
                foreach (var pictureBox in _pictureBoxesInSequence)
                {
                    pictureBox.InvokeIfRequired(() =>
                    {
                        pictureBox.Visible = false;
                    });
                }
            }
            
            if (resized.Length != 1)
            {
                ImagePictureBox.InvokeIfRequired(() =>
                {
                    ImagePictureBox.Visible = false;
                });

                var images = resized.Length < _pictureBoxesInSequence.Length
                    ? resized.Concat(Enumerable.Range(0, _pictureBoxesInSequence.Length - resized.Length).Select(_ => (Bitmap)null))
                    : resized;
                
                foreach (var (pictureBox, image) in _pictureBoxesInSequence
                    .Zip(images, (u, v) => (PictureBox: u, Image: v)))
                {
                    pictureBox.InvokeIfRequired(() =>
                    {
                        pictureBox.Image = image;
                        pictureBox.Visible = true;
                    });
                }
            }
        }

        private Bitmap[] GetSourceImage(ListItem file)
        {
            var extension = ExtractExtension(file.FileName);
            
            if (Consts.ImageExtensions.Contains(extension))
                return new [] { new Bitmap(file.Path) };

            if (Consts.VideoExtensions.Contains(extension))
                return GetVideoThumbnails(file, Consts.VideoThumbnailPositions);

            return Array.Empty<Bitmap>();
        }

        private Bitmap[] GetVideoThumbnails(ListItem file, params TimeSpan[] positions)
        {
            var thumbnails = positions.Prepend(TimeSpan.FromSeconds(1))
                .AsParallel()
                .AsOrdered()
                .Select(u => GetThumbnailAtPosition(file, u))
                .Where(u => u != null)
                .ToArray();

            if (thumbnails.Length == positions.Length + 1)
                return thumbnails.Skip(1).ToArray();

            return thumbnails;
        }

        private Bitmap GetThumbnailAtPosition(ListItem file, TimeSpan position)
        {
            var ffmpeg = new FFMpegConverter();

            var thumbnailStream = new MemoryStream();
            ffmpeg.GetVideoThumbnail(file.Path, thumbnailStream, (float)position.TotalSeconds);

            if (thumbnailStream.Length == 0)
                return null;
            
            return new Bitmap(thumbnailStream);
        }

        private Bitmap ResizeImageToFitPictureBox(Bitmap sourceImage)
        {
            var (height, width) = DimensionsToFitPictureBox(sourceImage);

            var resized = ResizeImage(sourceImage, width, height);
            return resized;
        }

        private (int height, int width) DimensionsToFitPictureBox(Bitmap sourceImage)
        {
            var height = sourceImage.Height;
            var width = sourceImage.Width;

            if (height < ImagePictureBox.Size.Height && width < ImagePictureBox.Size.Width)
            {
                var hRatio = ImagePictureBox.Size.Height / (double)height;
                var wRatio = ImagePictureBox.Size.Width / (double)width;

                var minRatio = Math.Min(hRatio, wRatio);
                height = (int)(height * minRatio);
                width = (int)(width * minRatio);
                return (height, width);
            }

            if (height > ImagePictureBox.Size.Height)
            {
                var ratio = (double)height / ImagePictureBox.Size.Height;
                height = (int)Math.Ceiling(height / ratio);
                width = (int)Math.Ceiling(width / ratio);
            }

            if (width > ImagePictureBox.Size.Width)
            {
                var ratio = (double)width / ImagePictureBox.Size.Width;
                height = (int)Math.Ceiling(height / ratio);
                width = (int)Math.Ceiling(width / ratio);
            }

            return (height, width);
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private string ExtractExtension(string fileName)
            => fileName.Substring(fileName.LastIndexOf('.') + 1).ToUpper();

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
            var indexFromPoint = GeneratedFilesListBox.IndexFromPoint(point);
            if (indexFromPoint == -1)
                return null;

            var listItem = (ListItem)GeneratedFilesListBox.Items[indexFromPoint];
            return listItem;
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetFileDropList(new StringCollection
                {
                    ((ListItem)GeneratedFilesListBox.SelectedItem).Path,
                });
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                GetFileAndOpen((ListItem)GeneratedFilesListBox.SelectedItem, OpenVariants.OpenInExplorer);
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
            GeneratedFilesListBox.Items.Clear();
        }

        private void ApplyFilter_Click(object sender, EventArgs e)
        {
            if (FilterTextBox.Text == _filter)
                return;
            
            if (string.IsNullOrWhiteSpace(FilterTextBox.Text))
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
            var listItem = ListItemFromPoint(location);
            if (listItem == null)
                return;

            GetFileAndOpen(listItem, openVariant);
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _itemLocation = null;
            
            var item = ListItemFromPoint(e.Location);
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
            
            var fileFromLocation = ListItemFromPoint(_itemLocation.Value);
            Clipboard.SetData(DataFormats.StringFormat, fileFromLocation.Path);
        }

        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_itemLocation.HasValue)
            {
                MessageBox.Show($"{nameof(_itemLocation)} is null");
                return;
            }
            
            var fileFromLocation = ListItemFromPoint(_itemLocation.Value);
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

        private void AutoGenerate_Click(object sender, EventArgs e)
        {
            _currentGenerateButtonColor = ChangeGenerateButtonColors(_currentGenerateButtonColor);

            ChangeAutogenerateButtonColor();

            _shouldAutoGenerate = !_shouldAutoGenerate;
        }

        private void ChangeAutogenerateButtonColor()
        {
            AutoGenerateButton.BackColor = Consts.GenerateButtonColors[_currentGenerateButtonColor].Main;
            AutoGenerateButton.ForeColor = Consts.GenerateButtonColors[_currentGenerateButtonColor].On;
            
            AutoGenerateButton.FlatAppearance.MouseDownBackColor = Consts.GenerateButtonColors[_currentGenerateButtonColor].Main;
            AutoGenerateButton.FlatAppearance.MouseOverBackColor = Consts.GenerateButtonColors[_currentGenerateButtonColor].Main;
        }


        private GenerateButtonColors ChangeGenerateButtonColors(GenerateButtonColors searchOption)
        {
            if (searchOption == GenerateButtonColors.Green)
                return GenerateButtonColors.Red;

            return GenerateButtonColors.Green;
        }

        private void AutoGenerateNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _autoGenerateCooldown = TimeSpan.FromMilliseconds((int)(AutoGenerateNumericUpDown.Value * 1000));
        }
    }
    

    internal static class ControlExtensions
    {
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action);
                return;
            }

            action();
        }
    }
}