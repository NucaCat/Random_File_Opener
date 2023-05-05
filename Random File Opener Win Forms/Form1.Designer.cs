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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.NextFileButton = new System.Windows.Forms.Button();
            this.SearchModeButton = new System.Windows.Forms.Button();
            this.ChangeDirectory = new System.Windows.Forms.Button();
            this.DirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ClearButton = new System.Windows.Forms.Button();
            this.FilterTextBox = new System.Windows.Forms.TextBox();
            this.ApplyFilter = new System.Windows.Forms.Button();
            this.ListBoxContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenInExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.скопироватьАдресВБуферОбменаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileAddressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ListBoxContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 29;
            this.listBox1.Location = new System.Drawing.Point(250, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1002, 758);
            this.listBox1.TabIndex = 0;
            this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
            this.listBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDoubleClick);
            this.listBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseUp);
            // 
            // NextFileButton
            // 
            this.NextFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NextFileButton.Location = new System.Drawing.Point(12, 624);
            this.NextFileButton.Name = "NextFileButton";
            this.NextFileButton.Size = new System.Drawing.Size(232, 146);
            this.NextFileButton.TabIndex = 1;
            this.NextFileButton.Text = "Следующий файл";
            this.NextFileButton.UseVisualStyleBackColor = true;
            this.NextFileButton.Click += new System.EventHandler(this.NextFileButton_Click);
            // 
            // SearchModeButton
            // 
            this.SearchModeButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SearchModeButton.Location = new System.Drawing.Point(12, 12);
            this.SearchModeButton.Name = "SearchModeButton";
            this.SearchModeButton.Size = new System.Drawing.Size(232, 159);
            this.SearchModeButton.TabIndex = 2;
            this.SearchModeButton.Text = "С подпапками или без";
            this.SearchModeButton.UseVisualStyleBackColor = true;
            this.SearchModeButton.Click += new System.EventHandler(this.SearchModeButton_Click);
            // 
            // ChangeDirectory
            // 
            this.ChangeDirectory.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ChangeDirectory.Location = new System.Drawing.Point(12, 177);
            this.ChangeDirectory.Name = "ChangeDirectory";
            this.ChangeDirectory.Size = new System.Drawing.Size(232, 159);
            this.ChangeDirectory.TabIndex = 3;
            this.ChangeDirectory.Text = "Изменить папку";
            this.ChangeDirectory.UseVisualStyleBackColor = true;
            this.ChangeDirectory.Click += new System.EventHandler(this.ChangeDirectory_Click);
            // 
            // DirectoryTextBox
            // 
            this.DirectoryTextBox.Enabled = false;
            this.DirectoryTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DirectoryTextBox.Location = new System.Drawing.Point(12, 342);
            this.DirectoryTextBox.Multiline = true;
            this.DirectoryTextBox.Name = "DirectoryTextBox";
            this.DirectoryTextBox.Size = new System.Drawing.Size(232, 52);
            this.DirectoryTextBox.TabIndex = 4;
            // 
            // ClearButton
            // 
            this.ClearButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ClearButton.Location = new System.Drawing.Point(12, 540);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(232, 78);
            this.ClearButton.TabIndex = 5;
            this.ClearButton.Text = "Очистить список";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // FilterTextBox
            // 
            this.FilterTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FilterTextBox.Location = new System.Drawing.Point(12, 400);
            this.FilterTextBox.Multiline = true;
            this.FilterTextBox.Name = "FilterTextBox";
            this.FilterTextBox.Size = new System.Drawing.Size(232, 52);
            this.FilterTextBox.TabIndex = 6;
            // 
            // ApplyFilter
            // 
            this.ApplyFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ApplyFilter.Location = new System.Drawing.Point(12, 458);
            this.ApplyFilter.Name = "ApplyFilter";
            this.ApplyFilter.Size = new System.Drawing.Size(232, 78);
            this.ApplyFilter.TabIndex = 7;
            this.ApplyFilter.Text = "Применить фильтр";
            this.ApplyFilter.UseVisualStyleBackColor = true;
            this.ApplyFilter.Click += new System.EventHandler(this.ApplyFilter_Click);
            // 
            // ListBoxContextMenuStrip
            // 
            this.ListBoxContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.OpenToolStripMenuItem, this.OpenInExplorerToolStripMenuItem, this.скопироватьАдресВБуферОбменаToolStripMenuItem });
            this.ListBoxContextMenuStrip.Name = "ListBoxContextMenuStrip";
            this.ListBoxContextMenuStrip.Size = new System.Drawing.Size(287, 98);
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(286, 24);
            this.OpenToolStripMenuItem.Text = "Открыть";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // OpenInExplorerToolStripMenuItem
            // 
            this.OpenInExplorerToolStripMenuItem.Name = "OpenInExplorerToolStripMenuItem";
            this.OpenInExplorerToolStripMenuItem.Size = new System.Drawing.Size(286, 24);
            this.OpenInExplorerToolStripMenuItem.Text = "Открыть в проводнике";
            this.OpenInExplorerToolStripMenuItem.Click += new System.EventHandler(this.OpenInExplorerToolStripMenuItem_Click);
            // 
            // скопироватьАдресВБуферОбменаToolStripMenuItem
            // 
            this.скопироватьАдресВБуферОбменаToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.FileAddressToolStripMenuItem, this.FileNameToolStripMenuItem });
            this.скопироватьАдресВБуферОбменаToolStripMenuItem.Name = "скопироватьАдресВБуферОбменаToolStripMenuItem";
            this.скопироватьАдресВБуферОбменаToolStripMenuItem.Size = new System.Drawing.Size(286, 24);
            this.скопироватьАдресВБуферОбменаToolStripMenuItem.Text = "Скопировать в буфер обмена";
            // 
            // FileAddressToolStripMenuItem
            // 
            this.FileAddressToolStripMenuItem.Name = "FileAddressToolStripMenuItem";
            this.FileAddressToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.FileAddressToolStripMenuItem.Text = "Адрес";
            this.FileAddressToolStripMenuItem.Click += new System.EventHandler(this.FileAddressToolStripMenuItem_Click);
            // 
            // FileNameToolStripMenuItem
            // 
            this.FileNameToolStripMenuItem.Name = "FileNameToolStripMenuItem";
            this.FileNameToolStripMenuItem.Size = new System.Drawing.Size(152, 24);
            this.FileNameToolStripMenuItem.Text = "Название";
            this.FileNameToolStripMenuItem.Click += new System.EventHandler(this.FileNameToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 777);
            this.Controls.Add(this.ApplyFilter);
            this.Controls.Add(this.FilterTextBox);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.DirectoryTextBox);
            this.Controls.Add(this.ChangeDirectory);
            this.Controls.Add(this.SearchModeButton);
            this.Controls.Add(this.NextFileButton);
            this.Controls.Add(this.listBox1);
            this.Name = "Form1";
            this.Text = "Генератор случайных файлов";
            this.ListBoxContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ToolStripMenuItem FileAddressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FileNameToolStripMenuItem;

        private System.Windows.Forms.ToolStripMenuItem скопироватьАдресВБуферОбменаToolStripMenuItem;

        private System.Windows.Forms.ContextMenuStrip ListBoxContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem OpenInExplorerToolStripMenuItem;

        private System.Windows.Forms.TextBox FilterTextBox;
        private System.Windows.Forms.Button ApplyFilter;

        private System.Windows.Forms.Button ClearButton;

        private System.Windows.Forms.TextBox DirectoryTextBox;

        private System.Windows.Forms.Button ChangeDirectory;

        private System.Windows.Forms.Button SearchModeButton;

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button NextFileButton;

        #endregion
    }
}