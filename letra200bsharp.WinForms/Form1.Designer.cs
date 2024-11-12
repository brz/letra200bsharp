namespace CetraSharp.WinFormsTest
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BluetoothDevicesListBox = new ListBox();
            DevicesLabel = new Label();
            RefreshButton = new Button();
            ImageLabel = new Label();
            BrowseButton = new Button();
            PrintButton = new Button();
            PathLabel = new Label();
            SuspendLayout();
            // 
            // BluetoothDevicesListBox
            // 
            BluetoothDevicesListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            BluetoothDevicesListBox.DisplayMember = "Name";
            BluetoothDevicesListBox.FormattingEnabled = true;
            BluetoothDevicesListBox.ItemHeight = 15;
            BluetoothDevicesListBox.Location = new Point(12, 34);
            BluetoothDevicesListBox.Name = "BluetoothDevicesListBox";
            BluetoothDevicesListBox.Size = new Size(581, 154);
            BluetoothDevicesListBox.TabIndex = 0;
            BluetoothDevicesListBox.ValueMember = "Id";
            // 
            // DevicesLabel
            // 
            DevicesLabel.AutoSize = true;
            DevicesLabel.Location = new Point(12, 9);
            DevicesLabel.Name = "DevicesLabel";
            DevicesLabel.Size = new Size(50, 15);
            DevicesLabel.TabIndex = 1;
            DevicesLabel.Text = "Devices:";
            // 
            // RefreshButton
            // 
            RefreshButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            RefreshButton.Enabled = false;
            RefreshButton.Location = new Point(518, 5);
            RefreshButton.Name = "RefreshButton";
            RefreshButton.Size = new Size(75, 23);
            RefreshButton.TabIndex = 2;
            RefreshButton.Text = "Refresh";
            RefreshButton.UseVisualStyleBackColor = true;
            RefreshButton.Click += RefreshButton_Click;
            // 
            // ImageLabel
            // 
            ImageLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            ImageLabel.AutoSize = true;
            ImageLabel.Location = new Point(12, 228);
            ImageLabel.Name = "ImageLabel";
            ImageLabel.Size = new Size(43, 15);
            ImageLabel.TabIndex = 3;
            ImageLabel.Text = "Image:";
            // 
            // BrowseButton
            // 
            BrowseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            BrowseButton.Location = new Point(518, 220);
            BrowseButton.Name = "BrowseButton";
            BrowseButton.Size = new Size(75, 23);
            BrowseButton.TabIndex = 4;
            BrowseButton.Text = "Browse...";
            BrowseButton.UseVisualStyleBackColor = true;
            BrowseButton.Click += BrowseButton_Click;
            // 
            // PrintButton
            // 
            PrintButton.Anchor = AnchorStyles.Bottom;
            PrintButton.Location = new Point(263, 265);
            PrintButton.Name = "PrintButton";
            PrintButton.Size = new Size(75, 23);
            PrintButton.TabIndex = 5;
            PrintButton.Text = "Print";
            PrintButton.UseVisualStyleBackColor = true;
            PrintButton.Click += PrintButton_Click;
            // 
            // PathLabel
            // 
            PathLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            PathLabel.AutoSize = true;
            PathLabel.Location = new Point(61, 228);
            PathLabel.Name = "PathLabel";
            PathLabel.Size = new Size(47, 15);
            PathLabel.TabIndex = 6;
            PathLabel.Text = "<Path>";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(605, 299);
            Controls.Add(PathLabel);
            Controls.Add(PrintButton);
            Controls.Add(BrowseButton);
            Controls.Add(ImageLabel);
            Controls.Add(RefreshButton);
            Controls.Add(DevicesLabel);
            Controls.Add(BluetoothDevicesListBox);
            Name = "MainForm";
            Text = "Dymo LetraTag 200B WinForms Demo";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox BluetoothDevicesListBox;
        private Label DevicesLabel;
        private Button RefreshButton;
        private Label ImageLabel;
        private Button BrowseButton;
        private Button PrintButton;
        private Label PathLabel;
    }
}
