using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Random_File_Opener_Win_Forms.CustomComponents.MessageBox;
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
        private readonly Random _random = new Random();
        private string _currentDirectory = string.Empty;
        private string _filter = Consts.EmptyFilter;
        private GenerateButtonColors _currentGenerateButtonColor = GenerateButtonColors.Red;

        private GeneratedFileListItem _item;

        private static bool _shouldAutoGenerate;
        private TimeSpan _autoGenerateCooldown = TimeSpan.FromSeconds(2);

        private readonly CustomMessageBox _messageBox = new CustomMessageBox();

        private Control[] _hideableControls;
        private PictureBox[] _nonHideableControls;

        public Form1()
        {
            InitializeComponent();

            InitialInitialize();

            Task.Run(StartAutoGenerate);
        }

        private void InitialInitialize()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            _pictureBoxesInSequence = new[]
            {
                VideoThumbnailFirstPictureBox,
                VideoThumbnailSecondPictureBox,
                VideoThumbnailThirdPictureBox
            };

            // ReSharper disable once PossibleLossOfFraction
            AutoGenerateNumericUpDown.Value = (int)_autoGenerateCooldown.TotalMilliseconds / 1000;

            var settings = SettingsPuller.Pull<AppSettings>(Consts.SettingsFileName);

            Styler.FillFromSettings(settings?.Styles);
            Styler.ApplyStyles(this);
            Styler.ApplyStyles(_messageBox);
            Styler.ApplyStylesToMessageBox(_messageBox);

            _filter = settings?.Filter ?? _filter;
            FilterTextBox.Text = _filter;

            Consts.CacheImages = settings?.CacheImages ?? Consts.CacheImages;

            Consts.VideoThumbnailPositions = settings?.VideoThumbnailPositions.IsEmpty() == true
                ? Consts.VideoThumbnailPositions
                // ReSharper disable once PossibleNullReferenceException
                : settings.VideoThumbnailPositions;

            SearchModeButton.Text = _searchOption.ToFriendlyString();

            Styler.ChangeAutogenerateButtonColor(AutoGenerateButton, _currentGenerateButtonColor);

            var currentDirectory = Directory.GetCurrentDirectory();

            foreach (var pictureBox in Controls.OfType<PictureBox>())
            {
                pictureBox.MouseUp += PictureBox_MouseUp;
                pictureBox.MouseDoubleClick += PictureBox_MouseDoubleClick;
            }

            _nonHideableControls = Controls.OfType<PictureBox>().ToArray();
            
            _hideableControls = Controls.OfType<Control>()
                .Where(u => !_nonHideableControls.Contains(u))
                .ToArray();

            Initialize(currentDirectory, _filter);
        }

        private async Task StartAutoGenerate()
        {
            for (;;)
            {
                if (_shouldAutoGenerate)
                {
                    NextFileButton_Click();
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

            _files.Initialize( GetFiles(directory, filter)
                .Select(u => GeneratedFileListItem.FromString(u, directory))
                .ToList()
                .Shuffle(_random));
        }

        private IEnumerable<string> GetFiles(string directory, string filter)
        {
            var initialFiles = Directory.GetFiles(directory, filter, _searchOption);

            if (_searchOption == SearchOption.TopDirectoryOnly)
                return initialFiles;

            var directories = Directory.GetDirectories(directory, filter, _searchOption);

            if (directories.IsEmpty())
                return initialFiles;

            var endFiles = directories
                .SelectMany(u => Directory.GetFiles(u, "*", _searchOption));

            return initialFiles.Concat(endFiles).Distinct();
        }

        private void NextFileButton_Click(object sender = null, EventArgs e = null)
        {
            var file = _files.GetCurrentAndMoveNext();
            if (file == null)
                return;

            AddImageToPreview(file);

            if (!file.AddedToListBox)
            {
                file.AddedToListBox = true;
                GeneratedFilesListBox.InvokeIfRequired(() =>
                {
                    GeneratedFilesListBox.Items.Add(file);
                    GeneratedFilesListBox.Focus();
                });
            }

            GeneratedFilesListBox.InvokeIfRequired(() =>
            {
                GeneratedFilesListBox.SelectedItem = file;
                GeneratedFilesListBox.Focus();
            });
        }

        private void AddImageToPreview(GeneratedFileListItem file)
        {
            var images = file.Images.IsNotEmpty()
            ? file.Images
            : ImageService.GetFitImages(file, ImagePictureBox, _pictureBoxesInSequence);

            if (Consts.CacheImages)
                file.Images = images;
            
            if (images.Length == 0)
            {
                PlaceImageInBigPictureBox(null);
                return;
            }
            
            if (images.Length == 1)
            {
                PlaceImageInBigPictureBox(images[0]);
                return;
            }
            
            PlaceImageInSmallPictureBoxes(images);
        }

        private void PlaceImageInSmallPictureBoxes(Bitmap[] images)
        {
            ImagePictureBox.InvokeIfRequired(() => { ImagePictureBox.Visible = false; });

            foreach (var (pictureBox, image) in _pictureBoxesInSequence
                .Zip(images.PadRightWithNulls(_pictureBoxesInSequence.Length),
                    (u, v) => (PictureBox: u, Image: v)))
            {
                PlaceImageInPictureBox(pictureBox, image);
            }
        }

        private void PlaceImageInPictureBox(PictureBox pictureBox, Bitmap image)
        {
            pictureBox.InvokeIfRequired(() =>
            {
                pictureBox.Image = image;
                pictureBox.Visible = true;
                if (image != null)
                {
                    var horizontalPadding = (pictureBox.Size.Width - image.Width) / 2;
                    var verticalPadding = (pictureBox.Size.Height - image.Height) / 2;
                    pictureBox.Padding = new Padding
                    {
                        Left =  horizontalPadding,
                        Right = horizontalPadding,
                        Top = verticalPadding,
                        Bottom = verticalPadding,
                    };
                }
            });
        }

        private void PlaceImageInBigPictureBox(Bitmap image)
        {
            PlaceImageInPictureBox(ImagePictureBox, image);

            _pictureBoxesInSequence.ForAll(u => u.InvokeIfRequired(() => { u.Visible = false; }));
        }

        private void GeneratedFilesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var listItem = GeneratedFilesListBox.SelectedFile();
            if (listItem == null)
                return;
            
            OpenFile(listItem, OpenVariants.OpenFile);
        }

        private void GeneratedFilesListBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            var listItem = GeneratedFilesListBox.SelectedFile();
            if (listItem == null)
                return;

            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetFileDropList(new StringCollection
                {
                    listItem.Path,
                });
                return;
            }

            if (e.KeyCode == Keys.Down 
                || e.KeyCode == Keys.Up 
                || e.KeyCode == Keys.Left 
                || e.KeyCode == Keys.Right)
            {
                MoveInListBox(e.KeyCode);
                return;
            }

            if (e.KeyCode == Keys.Enter)
            {
                OpenFile(listItem, OpenVariants.OpenInExplorer);
                return;
            }

            if (e.KeyCode == Keys.Space)
            {
                NextFileButton_Click();
                return;
            }

            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedFile();
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        private void MoveInListBox(Keys keyCode)
        {
            var itemToSet = keyCode == Keys.Down || keyCode == Keys.Right
                ? DownItemToSet()
                : UpItemToSet();

            GeneratedFilesListBox.SelectedItem = itemToSet;
            AddImageToPreview((GeneratedFileListItem)itemToSet);
        }

        private object UpItemToSet()
        {
            var isFirst = GeneratedFilesListBox.SelectedIndex == 0;
            var itemToSet = isFirst
                ? GeneratedFilesListBox.Items[GeneratedFilesListBox.Items.Count - 1]
                : GeneratedFilesListBox.Items[GeneratedFilesListBox.SelectedIndex - 1];
            return itemToSet;
        }

        private object DownItemToSet()
        {
            var isLast = GeneratedFilesListBox.SelectedIndex == GeneratedFilesListBox.Items.Count - 1;
            var itemToSet = isLast
                ? GeneratedFilesListBox.Items[0]
                : GeneratedFilesListBox.Items[GeneratedFilesListBox.SelectedIndex + 1];
            return itemToSet;
        }

        private void DeleteSelectedFile()
        {
            var selectedItem = GeneratedFilesListBox.SelectedFile();
            if (selectedItem == null)
                return;

            var result = _messageBox.ShowMessageBox(text: $"Вы действительно хотите удалить файл: {selectedItem.FileName}?", CustomMessageBox.YesNoButtons);
            if (result != DialogResult.Yes)
                return;

            try
            {
                File.Delete(selectedItem.Path);
            }
            catch (Exception e)
            {
                _messageBox.ShowMessageBox(text: e.Message, CustomMessageBox.OkButtons);
                return;
            }

            _files.Delete(selectedItem);
            
            GeneratedFilesListBox.Items.RemoveAt(GeneratedFilesListBox.SelectedIndex);

            selectedItem.Images.ForAll(u => u.Dispose());

            if (Consts.ImageExtensions.Contains(selectedItem.Extension))
            {
                ImagePictureBox.Image?.Dispose();
                ImagePictureBox.Image = null;

                _pictureBoxesInSequence.ForAll(u =>
                {
                    u.Image?.Dispose();
                    u.Image = null;
                });
            }

            selectedItem.Images = Array.Empty<Bitmap>();
        }

        private void OpenFile(GeneratedFileListItem item, OpenVariants openVariants)
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
            _files.ForAll(u => u.AddedToListBox = false);

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
            => OpenFile(_item, OpenVariants.OpenFile);

        private void OpenInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
            => OpenFile(_item, OpenVariants.OpenInExplorer);

        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
            => DeleteSelectedFile();

        private void GeneratedFilesListBox_MouseUp(object sender, MouseEventArgs e)
        {
            var item = Utilities.ListItemFromPoint(GeneratedFilesListBox, e.Location);
            if (item == null)
                return;

            GeneratedFilesListBox.SelectedItem = item;

            HandleMouseUpOnMouseUp(e, item);
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (((PictureBox)sender).Image == null)
                return;

            var item = GeneratedFilesListBox.SelectedFile();
            if (item == null)
                return;

            HandleMouseUpOnMouseUp(e, item);
        }

        private void PictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (((PictureBox)sender).Image == null)
                return;

            var item = GeneratedFilesListBox.SelectedFile();
            if (item == null)
                return;

            OpenFile(item, OpenVariants.OpenFile);
        }

        private void HandleMouseUpOnMouseUp(MouseEventArgs e, GeneratedFileListItem item)
        {
            _item = null;

            if (e.Button == MouseButtons.Right)
            {
                _item = item;
                AddImageToPreview(item);
                ListBoxContextMenuStrip.Show(Cursor.Position);
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                AddImageToPreview(item);
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var exceptionLogFileName = "ExceptionLog.txt";

            var e = (Exception) args.ExceptionObject;

            var fullExceptionLogFileName = $"{_currentDirectory}\\{exceptionLogFileName}";
            using (var writer = new StreamWriter(fullExceptionLogFileName))
            {
                writer.WriteLine($"Message: {e.Message}");
                writer.WriteLine($"Stack trace: \n{e.StackTrace}");

                writer.WriteLine("----------------------------------------------------");
                writer.WriteLine($"Current directory: {_currentDirectory}");
                
                writer.WriteLine("----------------------------------------------------");
                writer.WriteLine("Current item:");
                writer.WriteLine($"Path: {_files.Current.Path}");
                writer.WriteLine($"Name: {_files.Current.FileName}");
                writer.WriteLine($"Display value: {_files.Current.DisplayValue}");
            }
            
            var result = _messageBox.ShowMessageBox($"Необработанное исключение. Лог записан в {exceptionLogFileName}. Показать?", CustomMessageBox.YesNoButtons);
            if (result == DialogResult.Yes)
            {
                var startInfo = new ProcessStartInfo
                {
                    Arguments = $"/select, \"{fullExceptionLogFileName}\"",
                    FileName = "explorer.exe",
                };

                Process.Start(startInfo);
            }

            Application.Exit();
        }

        private void FileAddressToolStripMenuItem_Click(object sender, EventArgs e)
            => CopyItemToClipboard(CopyOptions.Path);

        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
            => CopyItemToClipboard(CopyOptions.File);

        private void FileNameToolStripMenuItem_Click(object sender, EventArgs e)
            => CopyItemToClipboard(CopyOptions.FileName);

        private void CopyItemToClipboard(CopyOptions option)
        {
            if (option == CopyOptions.FileName)
                Clipboard.SetData(DataFormats.StringFormat, _item.FileName);
            
            if (option == CopyOptions.Path)
                Clipboard.SetData(DataFormats.StringFormat, _item.Path);
            
            if (option == CopyOptions.File)
                Clipboard.SetFileDropList(new StringCollection { _item.Path });

            _item = null;
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

        private void HideControlsButton_Click(object sender, EventArgs e)
        {
            _hideableControls.ForAll(u => u.Visible = false);
            _nonHideableControls.ForAll(u => u.BackColor = Styles.Background);
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            _hideableControls.ForAll(u => u.Visible = true);
            _nonHideableControls.ForAll(u => u.BackColor = Styles.Surface);
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            var outputFolder = ShowOpenFileDialogAndGetDirectory();
            if (outputFolder.IsNullOrWhiteSpace())
                return;

            var dialogResult = _messageBox.ShowMessageBox($"Удалить все файлы в папке {outputFolder}?", CustomMessageBox.YesNoButtons);
            var shouldDelete = dialogResult == DialogResult.Yes;

            Task.Run(CopyFilesAndClearDirectoryIfRequired(outputFolder, shouldDelete));
        }

        private static string ShowOpenFileDialogAndGetDirectory()
        {
            using (var openFolderDialog = new CommonOpenFileDialog
                   {
                       InitialDirectory = "",
                       IsFolderPicker = true
                   })
            {
                var result = openFolderDialog.ShowDialog();

                if (result == CommonFileDialogResult.Ok && openFolderDialog.FileName.IsNotNullOrWhiteSpace())
                    return openFolderDialog.FileName;
                
                return string.Empty;
            }
        }

        private Func<Task> CopyFilesAndClearDirectoryIfRequired(string outputFolder, bool shouldClearOutputDirectory)
        {
            return async () =>
            {
                ExportButton.InvokeIfRequired(() => ExportButton.Enabled = false);
                ExportProgressBar.InvokeIfRequired(() =>
                {
                    ExportProgressBar.Value = ExportProgressBar.Minimum;
                    ExportProgressBar.Maximum = _files.All.Count;
                    ExportProgressBar.Visible = true;
                });

                if (shouldClearOutputDirectory)
                    ClearDirectory(outputFolder);

                foreach (var file in _files.All.Select((u, index) => (File: u, Index: index)))
                {
                    var outputFileName = $"{outputFolder}\\{file.Index.ToString().PadLeft(7, '0')}.{file.File.Extension}";

                    try
                    {
                        File.Copy(file.File.Path, outputFileName, overwrite: true);
                        ExportProgressBar.InvokeIfRequired(() => ExportProgressBar.PerformStep());
                    }
                    catch (Exception exception)
                    {
                        _messageBox.ShowMessageBox(text: exception.Message, CustomMessageBox.OkButtons);
                        break;
                    }
                }
                
                await Task.Delay(1000);

                ExportButton.InvokeIfRequired(() => ExportButton.Enabled = true);
                ExportProgressBar.InvokeIfRequired(() =>
                {
                    ExportProgressBar.Visible = false;
                    ExportProgressBar.Value = ExportProgressBar.Minimum;
                });
            };
        }

        private static void ClearDirectory(string outputFolder)
        {
            var di = new DirectoryInfo(outputFolder);

            foreach (var file in di.EnumerateFiles())
            {
                file.Delete();
            }
        }

        private enum OpenVariants
        {
            OpenFile = 0,
            OpenInExplorer = 1
        }

        private enum CopyOptions
        {
            FileName = 0,
            Path = 1,
            File = 2,
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