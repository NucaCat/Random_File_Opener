using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Random_File_Opener_Win_Forms.CustomComponents.MessageBox;
using Random_File_Opener_Win_Forms.Settings;
using Random_File_Opener_Win_Forms.Style;
using Serilog;

// ReSharper disable UsingStatementResourceInitialization
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
        private string _cacheDirectory = string.Empty;
        private string _filter = Consts.EmptyFilter;
        private bool _exportOnlyVisible;
        private bool _showPreview;
        private GenerateButtonColors _currentGenerateButtonColor = GenerateButtonColors.Red;

        private static bool _shouldAutoGenerate;
        private TimeSpan _autoGenerateCooldown = TimeSpan.FromSeconds(2);

        private readonly CustomMessageBox _messageBox = new CustomMessageBox();

        private Control[] _hideableControls;
        private Control[] _nonHideableControls;
        private readonly string _logFileName = "!Log.txt";

        public Form1()
        {
            InitializeComponent();

            InitialInitialize();

            Task.Run(StartAutoGenerate);
        }

        private void InitialInitialize()
        {
            var file = new FileInfo(_logFileName);
            if (file.Exists)
                file.Delete();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(_logFileName, outputTemplate: "{Message:lj}{NewLine}")
                .MinimumLevel.Debug()
                .CreateLogger();
            Application.ThreadException += UnhandledExceptionHandlerThread;
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

            _exportOnlyVisible = settings?.ExportOnlyVisible ?? false;
            _showPreview = settings?.ShowPreview ?? false;
            _filter = settings?.Filter ?? _filter;
            FilterTextBox.Text = SanitizeFilterForUi(_filter);

            Consts.Cache = settings?.Cache ?? Consts.Cache;

            Consts.VideoThumbnailPositions = settings?.VideoThumbnailPositions ?? Consts.VideoThumbnailPositions;

            SearchModeButton.Text = _searchOption.ToFriendlyString();

            Styler.ChangeAutogenerateButtonColor(AutoGenerateButton, _currentGenerateButtonColor);

            var currentDirectory = Directory.GetCurrentDirectory();

            foreach (var pictureBox in Controls.OfType<PictureBox>())
            {
                pictureBox.MouseUp += PictureBox_MouseUp;
                pictureBox.MouseDoubleClick += PictureBox_MouseDoubleClick;
            }

            _nonHideableControls = Array.Empty<Control>()
                .Concat(Controls.OfType<PictureBox>())
                .Concat(Controls.OfType<ProgressBar>())
                .ToArray();
            
            _hideableControls = Controls.OfType<Control>()
                .Where(u => !_nonHideableControls.Contains(u))
                .ToArray();

            Initialize(currentDirectory, _filter);

            WarmupJit();
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
            _cacheDirectory = $"{Directory.GetCurrentDirectory()}\\{Consts.Cache.CacheLocation}";
            if (Consts.Cache.CacheLocation.IsNotNullOrWhiteSpace())
                Directory.CreateDirectory(_cacheDirectory);
            _filter = filter;
            DirectoryTextBox.Text = directory.Substring(directory.LastIndexOf('\\') + 1);

            GeneratedFilesListBox.Items.Clear();

            _files.Initialize(GetFiles(directory, filter)
                .Select(u => GeneratedFileListItem.FromString(u, directory))
                .ToList()
                .Shuffle(_random));
        }

        private IEnumerable<FileInfo> GetFiles(string directory, string filter)
        {
            var initialFiles = new DirectoryInfo(directory).EnumerateFiles(filter, _searchOption)
                .WhereIf(_cacheDirectory.IsNotNullOrWhiteSpace(), u => !u.FullName.StartsWith(_cacheDirectory));

            if (_searchOption == SearchOption.TopDirectoryOnly)
                return initialFiles;

            var directories = Directory.GetDirectories(directory, filter, _searchOption)
                .WhereIf(_cacheDirectory.IsNotNullOrWhiteSpace(), u => !u.StartsWith(_cacheDirectory))
                .ToArray();

            if (directories.IsEmpty())
                return initialFiles;

            var endFiles = directories
                .SelectMany(u => new DirectoryInfo(u).EnumerateFiles("*", _searchOption))
                .WhereIf(_cacheDirectory.IsNotNullOrWhiteSpace(), u => !u.FullName.StartsWith(_cacheDirectory));

            return initialFiles
                .Concat(endFiles)
                .DistinctBy(u => u.FullName);
        }

        private void NextFileButton_Click(object sender = null, EventArgs e = null)
        {
            var file = _files.GetCurrent();
            if (file == null)
                return;

            Log.Logger.Debug("-----------------------");
            Log.Logger.Debug(nameof(NextFileButton_Click));
            SelectFile(file);
            GeneratedFilesListBox.ClearSelected();
            GeneratedFilesListBox.SelectedItem = file;
        }

        private void SelectFile(GeneratedFileListItem file)
        {
            Log.Logger.Debug($"{nameof(SelectFile)} {file.Dump()}");

            _files.SelectFile(file);
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
                GeneratedFilesListBox.Focus();
            });
        }

        private void AddImageToPreview(GeneratedFileListItem file)
        {
            if (!_showPreview)
                return;

            var images = GetImages(file);

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

        private Bitmap[] GetImages(GeneratedFileListItem file)
        {
            if (file.Images.IsNotEmpty())
                return file.Images;

            if (Consts.Cache.SaveCacheOnDisc && Consts.VideoExtensions.Contains(file.Extension))
            {
                file.Images = Consts.VideoThumbnailPositions
                    .Select((u, index) => file.GetPathForHash(_cacheDirectory, index))
                    .Where(File.Exists)
                    .Select(u => new Bitmap(u))
                    .ToArray();
                if (file.Images.IsNotEmpty())
                    return file.Images;
            }

            var images = ImageService.GetFitImages(file, ImagePictureBox, _pictureBoxesInSequence);
            file.Images = images;

            if (Consts.Cache.SaveCacheOnDisc && file.Images.IsNotEmpty() && Consts.VideoExtensions.Contains(file.Extension))
            {
                foreach (var (image, index) in file.Images.Select((u, index) => (u, index)))
                {
                    image.Save(file.GetPathForHash(_cacheDirectory, index));
                }
            }

            return images;
        }

        private void PlaceImageInSmallPictureBoxes(Bitmap[] images)
        {
            ImagePictureBox.InvokeIfRequired(() =>
            {
                ImagePictureBox.Image = null;
                ImagePictureBox.Visible = false;
            });

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

            _pictureBoxesInSequence.ForAll(u => u.InvokeIfRequired(() =>
            {
                u.Image = null;
                u.Visible = false;
            }));
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

            SuppressIfRequired(e);

            if (GeneratedFilesListBox.SelectedItems.Count > 1)
                GeneratedFilesListBox_KeyDownForMultipleFiles(e);
            else
                GeneratedFilesListBox_KeyDownForOneFile(e);
        }

        private void GeneratedFilesListBox_KeyDownForOneFile(KeyEventArgs e)
        {
            var listItem = GeneratedFilesListBox.SelectedFile();
            if (listItem == null)
                return;

            if (e.Control && e.KeyCode == Keys.C)
            {
                Clipboard.SetFileDropList(new StringCollection
                {
                    listItem.PathToFile,
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
                OpenFile(listItem, OpenVariants.OpenFile);
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

        private void GeneratedFilesListBox_KeyDownForMultipleFiles(KeyEventArgs e)
        {
            var listItem = GeneratedFilesListBox.SelectedFiles();
            if (listItem == null)
                return;

            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedFiles();
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        private static void SuppressIfRequired(KeyEventArgs e)
        {
            if (e.Control
                || e.KeyCode == Keys.Down
                || e.KeyCode == Keys.Up
                || e.KeyCode == Keys.Left
                || e.KeyCode == Keys.Right
                || e.KeyCode == Keys.Enter
                || e.KeyCode == Keys.Space
                || e.KeyCode == Keys.Delete)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void MoveInListBox(Keys keyCode)
        {
            Log.Logger.Debug("-----------------------");
            Log.Logger.Debug(nameof(MoveInListBox));

            var itemToSet = keyCode == Keys.Down || keyCode == Keys.Right
                ? DownItemToSet()
                : UpItemToSet();

            SelectFile(itemToSet);
        }

        private GeneratedFileListItem UpItemToSet()
        {
            var isFirst = GeneratedFilesListBox.SelectedIndex == 0;
            var itemToSet = isFirst
                ? GeneratedFilesListBox.Items[GeneratedFilesListBox.Items.Count - 1]
                : GeneratedFilesListBox.Items[GeneratedFilesListBox.SelectedIndex - 1];
            return (GeneratedFileListItem)itemToSet;
        }

        private GeneratedFileListItem DownItemToSet()
        {
            var isLast = GeneratedFilesListBox.SelectedIndex == GeneratedFilesListBox.Items.Count - 1;
            var itemToSet = isLast
                ? GeneratedFilesListBox.Items[0]
                : GeneratedFilesListBox.Items[GeneratedFilesListBox.SelectedIndex + 1];
            return (GeneratedFileListItem)itemToSet;
        }

        private void DeleteSelectedFile()
        {
            var selectedItem = GeneratedFilesListBox.SelectedFile();
            if (selectedItem == null)
                return;

            Log.Logger.Debug("-----------------------");
            Log.Logger.Debug($"{nameof(DeleteSelectedFile)} {selectedItem.Dump()}");

            var result = _messageBox.ShowMessageBox(text: $"Вы действительно хотите удалить файл: {selectedItem.FileName}?", CustomMessageBox.YesNoButtons);
            if (result != DialogResult.Yes)
                return;

            try
            {
                File.Delete(selectedItem.PathToFile);
                DeleteCachedThumbnails(selectedItem);
            }
            catch (Exception e)
            {
                _messageBox.ShowMessageBox(text: e.Message, CustomMessageBox.OkButtons);
                return;
            }
            
            var selectedIndex = GeneratedFilesListBox.SelectedIndex;
            Log.Logger.Debug($"selectedIndex: {selectedIndex}");

            GeneratedFilesListBox.Items.RemoveAt(GeneratedFilesListBox.SelectedIndex);

            _files.Delete(selectedItem);
            
            if (GeneratedFilesListBox.Items.Count > selectedIndex)
            {
                var nextFile = (GeneratedFileListItem)GeneratedFilesListBox.Items[selectedIndex];
                SelectFile(nextFile);
            } else if (GeneratedFilesListBox.Items.Count != 0)
            {
                var nextFile = (GeneratedFileListItem)GeneratedFilesListBox.Items[selectedIndex - 1];
                SelectFile(nextFile);
            }
        }

        private void DeleteSelectedFiles()
        {
            var selectedItems = GeneratedFilesListBox.SelectedFiles();
            if (selectedItems.IsEmpty())
                return;

            Log.Logger.Debug("-----------------------");
            Log.Logger.Debug($"{nameof(DeleteSelectedFiles)}");

            var result = _messageBox.ShowMessageBox(text: $"Вы действительно хотите удалить файлы?", CustomMessageBox.YesNoButtons);
            if (result != DialogResult.Yes)
                return;

            
            foreach (var item in selectedItems)
            {
                try
                {
                    File.Delete(item.PathToFile);
                    DeleteCachedThumbnails(item);
                }
                catch (Exception e)
                {
                    _messageBox.ShowMessageBox(text: e.Message, CustomMessageBox.OkButtons);
                    return;
                }
            }
            
            foreach (var selectedItem in selectedItems.Reverse())
            {
                GeneratedFilesListBox.Items.Remove(selectedItem);
            }
            _files.Delete(selectedItems);
            
            GeneratedFilesListBox.ClearSelected();
        }

        private void DisposeImages(GeneratedFileListItem selectedItem)
        {
            selectedItem.Images.ForAll(u => u.Dispose());
            selectedItem.Images = Array.Empty<Bitmap>();

            ImagePictureBox.Image?.Dispose();
            ImagePictureBox.Image = null;

            _pictureBoxesInSequence.ForAll(u =>
            {
                u.Image?.Dispose();
                u.Image = null;
            });
        }

        private void DeleteSelectedThumbnails()
        {
            var selectedItem = GeneratedFilesListBox.SelectedFile();
            if (selectedItem == null)
                return;

            var countDeleted = DeleteCachedThumbnails(selectedItem);

            _messageBox.ShowMessageBox(text: countDeleted == 0 ? "Кэш превью изображений не найден" : $"Было удалено {countDeleted} превью изображений", CustomMessageBox.OkButtons);
        }

        private int DeleteCachedThumbnails(GeneratedFileListItem selectedItem)
        {
            DisposeImages(selectedItem);

            if (!Consts.VideoExtensions.Contains(selectedItem.Extension))
                return 0;

            var count = 0;

            foreach (var index in Consts.VideoThumbnailPositions.Select((u, index) => index))
            {
                var hash = selectedItem.GetPathForHash(_cacheDirectory, index);
                if (File.Exists(hash))
                {
                    count++;
                    File.Delete(hash);
                }
            }

            return count;
        }

        private void OpenFile(GeneratedFileListItem item, OpenVariants openVariants)
        {
            var startInfo = new ProcessStartInfo
            {
                Arguments = openVariants == OpenVariants.OpenFile
                ? $"\"{item.PathToFile}\""
                : $"/select, \"{item.PathToFile}\"",
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
            ApplyFilter();
            NextFileButton.Select();
        }

        private void ApplyFilter()
        {
            if (FilterTextBox.Text == _filter)
                return;

            FilterTextBox.Text = SanitizeFilterForUi(FilterTextBox.Text);
            _filter = FilterTextBox.Text.IsNullOrWhiteSpace() ? "*" : $"*{FilterTextBox.Text}*";

            Initialize(_currentDirectory, _filter);
        }

        private string SanitizeFilterForUi(string text) => text.Replace("*", "");

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasOneSelectedFile())
               OpenFile(GeneratedFilesListBox.SelectedFile(), OpenVariants.OpenFile);
        }

        private void OpenInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasOneSelectedFile())
               OpenFile(GeneratedFilesListBox.SelectedFile(), OpenVariants.OpenInExplorer);
        }

        // TODO v.chumachenko only delete is supported with multiselect
        private void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasMultipleSelectedFiles())
            {
                DeleteSelectedFiles();
                return;
            }
            DeleteSelectedFile();
        }

        private void DeleteSelectedThumbnailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasOneSelectedFile())
                DeleteSelectedThumbnails();
        }

        private void GeneratedFilesListBox_MouseUp(object sender, MouseEventArgs e)
        {
            var item = Utilities.ListItemFromPoint(GeneratedFilesListBox, e.Location);
            if (item == null)
                return;

            HandleMouseUpOnMouseUp(e, item);
        }

        private void FilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;

            e.Handled = true;
            e.SuppressKeyPress = true;
            ApplyFilter();
            NextFileButton.Select();
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
            if (e.Button == MouseButtons.Right)
            {
                Log.Logger.Debug("-----------------------");
                Log.Logger.Debug($"{nameof(HandleMouseUpOnMouseUp)} Right");

                // SelectFile(item, mode);
                ListBoxContextMenuStrip.Show(Cursor.Position);
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                Log.Logger.Debug("-----------------------");
                Log.Logger.Debug($"{nameof(HandleMouseUpOnMouseUp)} Left");

                SelectFile(item);
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        private void UnhandledExceptionHandlerThread(object sender, ThreadExceptionEventArgs exception)
        {
            HandleUnhandledException(exception.Exception);
        }

        private void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            HandleUnhandledException((Exception)args.ExceptionObject);
        }

        private void HandleUnhandledException(Exception e)
        {
            Log.Logger.Debug("-----------------------");
            Log.Logger.Debug(nameof(HandleUnhandledException));

            Log.Logger.Debug($"Message: {e.Message}");
            Log.Logger.Debug($"Stack trace: {e.StackTrace}");

            Log.Logger.Debug("----------------------------------------------------");
            Log.Logger.Debug($"Current directory: {_currentDirectory}");
            
            Log.Logger.Debug("----------------------------------------------------");
            Log.Logger.Debug($"Current item: {_files.Current.Dump()}");
            Log.Logger.Debug($"CurrentIndex: {_files.CurrentIndex}");
            Log.Logger.Debug($"SelectedIndex in list box: {GeneratedFilesListBox.SelectedIndex}");
            
            var result = _messageBox.ShowMessageBox($"Необработанное исключение. Лог записан в {_logFileName}. Показать?", CustomMessageBox.YesNoButtons);
            if (result == DialogResult.Yes)
            {
                var startInfo = new ProcessStartInfo
                {
                    Arguments = $"/select, \"{_logFileName}\"",
                    FileName = "explorer.exe",
                };

                Process.Start(startInfo);
            }

            Application.Exit();
        }

        private void FileAddressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasOneSelectedFile())
                CopyItemToClipboard(CopyOptions.Path);
        }

        private void FileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasOneSelectedFile())
                CopyItemToClipboard(CopyOptions.File);
        }

        private void FileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasOneSelectedFile())
                CopyItemToClipboard(CopyOptions.FileName);
        }

        private void UseFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (HasMultipleSelectedFiles())
                return;

            var selectedFile = GeneratedFilesListBox.SelectedFile();
            if (selectedFile is null)
                return;

            var directory = selectedFile.Directory.IsNullOrWhiteSpace()
                ? _currentDirectory
                : $"{_currentDirectory}\\{selectedFile.Directory}";
            Initialize(directory, _filter);
        }

        private void CopyItemToClipboard(CopyOptions option)
        {
            var selectedFile = GeneratedFilesListBox.SelectedFile();
            if (selectedFile is null)
                return;
            
            if (option == CopyOptions.FileName)
                Clipboard.SetData(DataFormats.StringFormat, selectedFile.FileName);
            
            if (option == CopyOptions.Path)
                Clipboard.SetData(DataFormats.StringFormat, selectedFile.PathToFile);
            
            if (option == CopyOptions.File)
                Clipboard.SetFileDropList(new StringCollection { selectedFile.PathToFile });
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
                var files = _files.All
                    .WhereIf(_exportOnlyVisible, u => u.AddedToListBox);
                
                ExportButton.InvokeIfRequired(() => ExportButton.Enabled = false);
                ExportProgressBar.InvokeIfRequired(() =>
                {
                    ExportProgressBar.Value = ExportProgressBar.Minimum;
                    ExportProgressBar.Maximum = files.Count();
                    ExportProgressBar.Visible = true;
                });

                if (shouldClearOutputDirectory)
                    ClearDirectory(outputFolder);

                foreach (var file in files
                    .Select((u, index) => (File: u, Index: index)))
                {
                    var outputFileName = $"{outputFolder}\\{file.Index.ToString().PadLeft(7, '0')}.{file.File.Extension}";

                    try
                    {
                        File.Copy(file.File.PathToFile, outputFileName, overwrite: true);
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

        private bool HasMultipleSelectedFiles()
            => GeneratedFilesListBox.SelectedItems.Count > 1;

        private bool HasOneSelectedFile()
            => GeneratedFilesListBox.SelectedItems.Count == 1;

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
        
        
        #region JitWarmup
        private static void WarmupJit()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new MyFFMpegConverter();
        }
        #endregion JitWarmup
    }
}