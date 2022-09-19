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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.NextFileButton = new System.Windows.Forms.Button();
            this.SearchModeButton = new System.Windows.Forms.Button();
            this.ChangeDirectory = new System.Windows.Forms.Button();
            this.DirectoryTextBox = new System.Windows.Forms.TextBox();
            this.ClearButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 29;
            this.listBox1.Location = new System.Drawing.Point(250, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(1002, 613);
            this.listBox1.TabIndex = 0;
            this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
            this.listBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDoubleClick);
            // 
            // NextFileButton
            // 
            this.NextFileButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.NextFileButton.Location = new System.Drawing.Point(12, 484);
            this.NextFileButton.Name = "NextFileButton";
            this.NextFileButton.Size = new System.Drawing.Size(232, 159);
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
            this.ClearButton.Location = new System.Drawing.Point(12, 400);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(232, 78);
            this.ClearButton.TabIndex = 5;
            this.ClearButton.Text = "Очистить список";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1264, 655);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.DirectoryTextBox);
            this.Controls.Add(this.ChangeDirectory);
            this.Controls.Add(this.SearchModeButton);
            this.Controls.Add(this.NextFileButton);
            this.Controls.Add(this.listBox1);
            this.Name = "Form1";
            this.Text = "Генератор случайных фалов";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button ClearButton;

        private System.Windows.Forms.TextBox DirectoryTextBox;

        private System.Windows.Forms.Button ChangeDirectory;

        private System.Windows.Forms.Button SearchModeButton;

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button NextFileButton;

        #endregion
    }
}