namespace Random_File_Opener_Win_Forms
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.GeneratedFilesListBox = new System.Windows.Forms.ListBox();
            this.NextFileButton = new System.Windows.Forms.Button();
            this.SearchModeButton = new System.Windows.Forms.Button();
            this.ChangeDirectoryButton = new System.Windows.Forms.Button();
            this.DirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ClearButton = new System.Windows.Forms.Button();
            this.FilterTextBox = new System.Windows.Forms.TextBox();
            this.ApplyFilterButton = new System.Windows.Forms.Button();
            this.ListBoxContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileAddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteSelectedThumbnailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UseFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImagePictureBox = new System.Windows.Forms.PictureBox();
            this.AutoGenerateButton = new System.Windows.Forms.Button();
            this.AutoGenerateNumericUpDown = new FlatNumericUpDown();
            this.VideoThumbnailFirstPictureBox = new System.Windows.Forms.PictureBox();
            this.VideoThumbnailSecondPictureBox = new System.Windows.Forms.PictureBox();
            this.VideoThumbnailThirdPictureBox = new System.Windows.Forms.PictureBox();
            this.HideControlsButton = new System.Windows.Forms.Button();
            this.ExportButton = new System.Windows.Forms.Button();
            this.ExportProgressBar = new System.Windows.Forms.ProgressBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.FilePathLabel = new System.Windows.Forms.Label();
            this.FileSizeLabel = new System.Windows.Forms.Label();
            this.FileNameLabel = new System.Windows.Forms.Label();
            this.ListBoxContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImagePictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.AutoGenerateNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoThumbnailFirstPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoThumbnailSecondPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoThumbnailThirdPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GeneratedFilesListBox
            // 
            this.GeneratedFilesListBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.GeneratedFilesListBox.FormattingEnabled = true;
            this.GeneratedFilesListBox.ItemHeight = 29;
            this.GeneratedFilesListBox.Location = new System.Drawing.Point(1024, 6);
            this.GeneratedFilesListBox.Name = "GeneratedFilesListBox";
            this.GeneratedFilesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.GeneratedFilesListBox.Size = new System.Drawing.Size(868, 700);
            this.GeneratedFilesListBox.TabIndex = 0;
            this.GeneratedFilesListBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GeneratedFilesListBox_KeyDown);
            this.GeneratedFilesListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.GeneratedFilesListBox_MouseDoubleClick);
            this.GeneratedFilesListBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GeneratedFilesListBox_MouseUp);
            // 
            // NextFileButton
            // 
            this.NextFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NextFileButton.Location = new System.Drawing.Point(12, 892);
            this.NextFileButton.Name = "NextFileButton";
            this.NextFileButton.Size = new System.Drawing.Size(232, 91);
            this.NextFileButton.TabIndex = 1;
            this.NextFileButton.Text = "Следующий файл";
            this.NextFileButton.UseVisualStyleBackColor = true;
            this.NextFileButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NextFileButton_Click);
            // 
            // SearchModeButton
            // 
            this.SearchModeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SearchModeButton.Location = new System.Drawing.Point(12, 6);
            this.SearchModeButton.Name = "SearchModeButton";
            this.SearchModeButton.Size = new System.Drawing.Size(232, 91);
            this.SearchModeButton.TabIndex = 2;
            this.SearchModeButton.Text = "С подпапками или без";
            this.SearchModeButton.UseVisualStyleBackColor = true;
            this.SearchModeButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.SearchModeButton_Click);
            // 
            // ChangeDirectoryButton
            // 
            this.ChangeDirectoryButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ChangeDirectoryButton.Location = new System.Drawing.Point(12, 103);
            this.ChangeDirectoryButton.Name = "ChangeDirectoryButton";
            this.ChangeDirectoryButton.Size = new System.Drawing.Size(232, 91);
            this.ChangeDirectoryButton.TabIndex = 3;
            this.ChangeDirectoryButton.Text = "Изменить папку";
            this.ChangeDirectoryButton.UseVisualStyleBackColor = true;
            this.ChangeDirectoryButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChangeDirectory_Click);
            // 
            // DirectoryTextBox
            // 
            this.DirectoryTextBox.Enabled = false;
            this.DirectoryTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DirectoryTextBox.Location = new System.Drawing.Point(12, 348);
            this.DirectoryTextBox.Multiline = true;
            this.DirectoryTextBox.Name = "DirectoryTextBox";
            this.DirectoryTextBox.Size = new System.Drawing.Size(232, 52);
            this.DirectoryTextBox.TabIndex = 4;
            // 
            // ClearButton
            // 
            this.ClearButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ClearButton.Location = new System.Drawing.Point(12, 557);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(232, 91);
            this.ClearButton.TabIndex = 5;
            this.ClearButton.Text = "Очистить список";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ClearButton_Click);
            // 
            // FilterTextBox
            // 
            this.FilterTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FilterTextBox.Location = new System.Drawing.Point(12, 406);
            this.FilterTextBox.Multiline = true;
            this.FilterTextBox.Name = "FilterTextBox";
            this.FilterTextBox.Size = new System.Drawing.Size(232, 52);
            this.FilterTextBox.TabIndex = 6;
            this.FilterTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FilterTextBox_KeyDown);
            // 
            // ApplyFilterButton
            // 
            this.ApplyFilterButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ApplyFilterButton.Location = new System.Drawing.Point(12, 464);
            this.ApplyFilterButton.Name = "ApplyFilterButton";
            this.ApplyFilterButton.Size = new System.Drawing.Size(232, 91);
            this.ApplyFilterButton.TabIndex = 7;
            this.ApplyFilterButton.Text = "Применить фильтр";
            this.ApplyFilterButton.UseVisualStyleBackColor = true;
            this.ApplyFilterButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ApplyFilter_Click);
            // 
            // ListBoxContextMenuStrip
            // 
            this.ListBoxContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.OpenToolStripMenuItem, this.OpenInExplorerToolStripMenuItem, this.CopyToClipboardToolStripMenuItem, this.DeleteToolStripMenuItem, this.DeleteSelectedThumbnailsToolStripMenuItem, this.UseFolderToolStripMenuItem });
            this.ListBoxContextMenuStrip.Name = "ListBoxContextMenuStrip";
            this.ListBoxContextMenuStrip.Size = new System.Drawing.Size(239, 136);
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.OpenToolStripMenuItem.Text = "Открыть";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // OpenInExplorerToolStripMenuItem
            // 
            this.OpenInExplorerToolStripMenuItem.Name = "OpenInExplorerToolStripMenuItem";
            this.OpenInExplorerToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.OpenInExplorerToolStripMenuItem.Text = "Открыть в проводнике";
            this.OpenInExplorerToolStripMenuItem.Click += new System.EventHandler(this.OpenInExplorerToolStripMenuItem_Click);
            // 
            // CopyToClipboardToolStripMenuItem
            // 
            this.CopyToClipboardToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.FileToolStripMenuItem, this.FileAddressToolStripMenuItem, this.FileNameToolStripMenuItem });
            this.CopyToClipboardToolStripMenuItem.Name = "CopyToClipboardToolStripMenuItem";
            this.CopyToClipboardToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.CopyToClipboardToolStripMenuItem.Text = "Скопировать в буфер обмена";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.FileToolStripMenuItem.Text = "Файл";
            this.FileToolStripMenuItem.Click += new System.EventHandler(this.FileToolStripMenuItem_Click);
            // 
            // FileAddressToolStripMenuItem
            // 
            this.FileAddressToolStripMenuItem.Name = "FileAddressToolStripMenuItem";
            this.FileAddressToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.FileAddressToolStripMenuItem.Text = "Адрес";
            this.FileAddressToolStripMenuItem.Click += new System.EventHandler(this.FileAddressToolStripMenuItem_Click);
            // 
            // FileNameToolStripMenuItem
            // 
            this.FileNameToolStripMenuItem.Name = "FileNameToolStripMenuItem";
            this.FileNameToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.FileNameToolStripMenuItem.Text = "Название";
            this.FileNameToolStripMenuItem.Click += new System.EventHandler(this.FileNameToolStripMenuItem_Click);
            // 
            // DeleteToolStripMenuItem
            // 
            this.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
            this.DeleteToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.DeleteToolStripMenuItem.Text = "Удалить";
            this.DeleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
            // 
            // DeleteSelectedThumbnailsToolStripMenuItem
            // 
            this.DeleteSelectedThumbnailsToolStripMenuItem.Name = "DeleteSelectedThumbnailsToolStripMenuItem";
            this.DeleteSelectedThumbnailsToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.DeleteSelectedThumbnailsToolStripMenuItem.Text = "Удалить кэш превью";
            this.DeleteSelectedThumbnailsToolStripMenuItem.Click += new System.EventHandler(this.DeleteSelectedThumbnailsToolStripMenuItem_Click);
            // 
            // UseFolderToolStripMenuItem
            // 
            this.UseFolderToolStripMenuItem.Name = "UseFolderToolStripMenuItem";
            this.UseFolderToolStripMenuItem.Size = new System.Drawing.Size(238, 22);
            this.UseFolderToolStripMenuItem.Text = "Использовать папку";
            this.UseFolderToolStripMenuItem.Click += new System.EventHandler(this.UseFolderToolStripMenuItem_Click);
            // 
            // ImagePictureBox
            // 
            this.ImagePictureBox.Location = new System.Drawing.Point(250, 6);
            this.ImagePictureBox.Name = "ImagePictureBox";
            this.ImagePictureBox.Size = new System.Drawing.Size(768, 977);
            this.ImagePictureBox.TabIndex = 8;
            this.ImagePictureBox.TabStop = false;
            // 
            // AutoGenerateButton
            // 
            this.AutoGenerateButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.AutoGenerateButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AutoGenerateButton.ForeColor = System.Drawing.Color.Black;
            this.AutoGenerateButton.Location = new System.Drawing.Point(12, 654);
            this.AutoGenerateButton.Name = "AutoGenerateButton";
            this.AutoGenerateButton.Size = new System.Drawing.Size(232, 91);
            this.AutoGenerateButton.TabIndex = 9;
            this.AutoGenerateButton.Text = "Автогенерация";
            this.AutoGenerateButton.UseVisualStyleBackColor = false;
            this.AutoGenerateButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AutoGenerate_Click);
            // 
            // AutoGenerateNumericUpDown
            // 
            this.AutoGenerateNumericUpDown.DecimalPlaces = 2;
            this.AutoGenerateNumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.AutoGenerateNumericUpDown.Increment = new decimal(new int[] { 25, 0, 0, 131072 });
            this.AutoGenerateNumericUpDown.Location = new System.Drawing.Point(12, 749);
            this.AutoGenerateNumericUpDown.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            this.AutoGenerateNumericUpDown.Minimum = new decimal(new int[] { 25, 0, 0, 131072 });
            this.AutoGenerateNumericUpDown.Name = "AutoGenerateNumericUpDown";
            this.AutoGenerateNumericUpDown.Size = new System.Drawing.Size(120, 40);
            this.AutoGenerateNumericUpDown.TabIndex = 10;
            this.AutoGenerateNumericUpDown.Value = new decimal(new int[] { 25, 0, 0, 131072 });
            this.AutoGenerateNumericUpDown.ValueChanged += new System.EventHandler(this.AutoGenerateNumericUpDown_ValueChanged);
            // 
            // VideoThumbnailFirstPictureBox
            // 
            this.VideoThumbnailFirstPictureBox.Location = new System.Drawing.Point(250, 6);
            this.VideoThumbnailFirstPictureBox.Name = "VideoThumbnailFirstPictureBox";
            this.VideoThumbnailFirstPictureBox.Size = new System.Drawing.Size(768, 321);
            this.VideoThumbnailFirstPictureBox.TabIndex = 11;
            this.VideoThumbnailFirstPictureBox.TabStop = false;
            this.VideoThumbnailFirstPictureBox.Visible = false;
            // 
            // VideoThumbnailSecondPictureBox
            // 
            this.VideoThumbnailSecondPictureBox.Location = new System.Drawing.Point(250, 333);
            this.VideoThumbnailSecondPictureBox.Name = "VideoThumbnailSecondPictureBox";
            this.VideoThumbnailSecondPictureBox.Size = new System.Drawing.Size(768, 321);
            this.VideoThumbnailSecondPictureBox.TabIndex = 12;
            this.VideoThumbnailSecondPictureBox.TabStop = false;
            this.VideoThumbnailSecondPictureBox.Visible = false;
            // 
            // VideoThumbnailThirdPictureBox
            // 
            this.VideoThumbnailThirdPictureBox.Location = new System.Drawing.Point(250, 662);
            this.VideoThumbnailThirdPictureBox.Name = "VideoThumbnailThirdPictureBox";
            this.VideoThumbnailThirdPictureBox.Size = new System.Drawing.Size(768, 321);
            this.VideoThumbnailThirdPictureBox.TabIndex = 13;
            this.VideoThumbnailThirdPictureBox.TabStop = false;
            this.VideoThumbnailThirdPictureBox.Visible = false;
            // 
            // HideControlsButton
            // 
            this.HideControlsButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.HideControlsButton.Location = new System.Drawing.Point(12, 795);
            this.HideControlsButton.Name = "HideControlsButton";
            this.HideControlsButton.Size = new System.Drawing.Size(232, 91);
            this.HideControlsButton.TabIndex = 14;
            this.HideControlsButton.Text = "Скрыть";
            this.HideControlsButton.UseVisualStyleBackColor = true;
            this.HideControlsButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HideControlsButton_Click);
            // 
            // ExportButton
            // 
            this.ExportButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ExportButton.Location = new System.Drawing.Point(12, 200);
            this.ExportButton.Name = "ExportButton";
            this.ExportButton.Size = new System.Drawing.Size(232, 91);
            this.ExportButton.TabIndex = 15;
            this.ExportButton.Text = "Выгрузить";
            this.ExportButton.UseVisualStyleBackColor = true;
            this.ExportButton.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ExportButton_Click);
            // 
            // ExportProgressBar
            // 
            this.ExportProgressBar.Location = new System.Drawing.Point(12, 297);
            this.ExportProgressBar.Name = "ExportProgressBar";
            this.ExportProgressBar.Size = new System.Drawing.Size(232, 45);
            this.ExportProgressBar.Step = 1;
            this.ExportProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ExportProgressBar.TabIndex = 16;
            this.ExportProgressBar.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.FilePathLabel);
            this.panel1.Controls.Add(this.FileSizeLabel);
            this.panel1.Controls.Add(this.FileNameLabel);
            this.panel1.Location = new System.Drawing.Point(1024, 709);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(868, 274);
            this.panel1.TabIndex = 17;
            // 
            // FilePathLabel
            // 
            this.FilePathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FilePathLabel.Location = new System.Drawing.Point(6, 115);
            this.FilePathLabel.Name = "FilePathLabel";
            this.FilePathLabel.Size = new System.Drawing.Size(859, 100);
            this.FilePathLabel.TabIndex = 2;
            this.FilePathLabel.Text = "Путь:";
            // 
            // FileSizeLabel
            // 
            this.FileSizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileSizeLabel.Location = new System.Drawing.Point(6, 215);
            this.FileSizeLabel.Name = "FileSizeLabel";
            this.FileSizeLabel.Size = new System.Drawing.Size(859, 40);
            this.FileSizeLabel.TabIndex = 1;
            this.FileSizeLabel.Text = "Размер:";
            // 
            // FileNameLabel
            // 
            this.FileNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileNameLabel.Location = new System.Drawing.Point(6, 7);
            this.FileNameLabel.Name = "FileNameLabel";
            this.FileNameLabel.Size = new System.Drawing.Size(859, 100);
            this.FileNameLabel.TabIndex = 0;
            this.FileNameLabel.Text = "Файл:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1001);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ExportProgressBar);
            this.Controls.Add(this.ExportButton);
            this.Controls.Add(this.HideControlsButton);
            this.Controls.Add(this.VideoThumbnailThirdPictureBox);
            this.Controls.Add(this.VideoThumbnailSecondPictureBox);
            this.Controls.Add(this.VideoThumbnailFirstPictureBox);
            this.Controls.Add(this.AutoGenerateNumericUpDown);
            this.Controls.Add(this.AutoGenerateButton);
            this.Controls.Add(this.ImagePictureBox);
            this.Controls.Add(this.ApplyFilterButton);
            this.Controls.Add(this.FilterTextBox);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.DirectoryTextBox);
            this.Controls.Add(this.ChangeDirectoryButton);
            this.Controls.Add(this.SearchModeButton);
            this.Controls.Add(this.NextFileButton);
            this.Controls.Add(this.GeneratedFilesListBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.ListBoxContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ImagePictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.AutoGenerateNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoThumbnailFirstPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoThumbnailSecondPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VideoThumbnailThirdPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label FilePathLabel;

        private System.Windows.Forms.Label FileSizeLabel;

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label FileNameLabel;

        private System.Windows.Forms.ToolStripMenuItem UseFolderToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem DeleteSelectedThumbnailsToolStripMenuItem;

        private System.Windows.Forms.ProgressBar ExportProgressBar;

        private System.Windows.Forms.Button ExportButton;

        private System.Windows.Forms.Button HideControlsButton;

        private System.Windows.Forms.ToolStripMenuItem DeleteToolStripMenuItem;

        private System.Windows.Forms.PictureBox VideoThumbnailThirdPictureBox;

        private System.Windows.Forms.PictureBox VideoThumbnailFirstPictureBox;

        private System.Windows.Forms.PictureBox VideoThumbnailSecondPictureBox;

        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;

        private FlatNumericUpDown AutoGenerateNumericUpDown;

        private System.Windows.Forms.Button AutoGenerateButton;

        private System.Windows.Forms.PictureBox ImagePictureBox;

        private System.Windows.Forms.ToolStripMenuItem FileAddressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileNameToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem CopyToClipboardToolStripMenuItem;

        private System.Windows.Forms.ContextMenuStrip ListBoxContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenInExplorerToolStripMenuItem;

        private System.Windows.Forms.TextBox FilterTextBox;
        private System.Windows.Forms.Button ApplyFilterButton;

        private System.Windows.Forms.Button ClearButton;

        private System.Windows.Forms.TextBox DirectoryTextBox;

        private System.Windows.Forms.Button ChangeDirectoryButton;

        private System.Windows.Forms.Button SearchModeButton;

        private System.Windows.Forms.ListBox GeneratedFilesListBox;
        private System.Windows.Forms.Button NextFileButton;

        #endregion
    }
}