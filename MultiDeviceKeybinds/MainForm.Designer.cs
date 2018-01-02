namespace MultiDeviceKeybinds
{
    partial class MainForm
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
            this.KeyPressOutputLabel = new System.Windows.Forms.Label();
            this.AddKeybindButton = new System.Windows.Forms.Button();
            this.EditKeybindButton = new System.Windows.Forms.Button();
            this.RemoveKeybindButton = new System.Windows.Forms.Button();
            this.DevicesKeybindsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.DevicesListView = new System.Windows.Forms.ListView();
            this.DeviceNameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DeviceIDColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.KeybindsListView = new MultiDeviceKeybinds.CustomListView();
            this.NameColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.KeysColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ConditionColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MacroColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ShowConsoleCheckBox = new System.Windows.Forms.CheckBox();
            this.StartWithWindowsCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.DevicesKeybindsSplitContainer)).BeginInit();
            this.DevicesKeybindsSplitContainer.Panel1.SuspendLayout();
            this.DevicesKeybindsSplitContainer.Panel2.SuspendLayout();
            this.DevicesKeybindsSplitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // KeyPressOutputLabel
            // 
            this.KeyPressOutputLabel.AutoSize = true;
            this.KeyPressOutputLabel.Location = new System.Drawing.Point(3, 3);
            this.KeyPressOutputLabel.Margin = new System.Windows.Forms.Padding(0);
            this.KeyPressOutputLabel.Name = "KeyPressOutputLabel";
            this.KeyPressOutputLabel.Size = new System.Drawing.Size(84, 52);
            this.KeyPressOutputLabel.TabIndex = 0;
            this.KeyPressOutputLabel.Text = "KeyPress output\r\n\r\n\r\n.";
            // 
            // AddKeybindButton
            // 
            this.AddKeybindButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddKeybindButton.BackColor = System.Drawing.Color.White;
            this.AddKeybindButton.Enabled = false;
            this.AddKeybindButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddKeybindButton.Location = new System.Drawing.Point(6, 360);
            this.AddKeybindButton.Name = "AddKeybindButton";
            this.AddKeybindButton.Size = new System.Drawing.Size(298, 23);
            this.AddKeybindButton.TabIndex = 2;
            this.AddKeybindButton.Text = "Add Keybind";
            this.AddKeybindButton.UseVisualStyleBackColor = false;
            this.AddKeybindButton.Click += new System.EventHandler(this.AddKeybindButton_Click);
            // 
            // EditKeybindButton
            // 
            this.EditKeybindButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EditKeybindButton.BackColor = System.Drawing.Color.White;
            this.EditKeybindButton.Enabled = false;
            this.EditKeybindButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EditKeybindButton.Location = new System.Drawing.Point(310, 360);
            this.EditKeybindButton.Name = "EditKeybindButton";
            this.EditKeybindButton.Size = new System.Drawing.Size(352, 23);
            this.EditKeybindButton.TabIndex = 3;
            this.EditKeybindButton.Text = "Edit Keybind";
            this.EditKeybindButton.UseVisualStyleBackColor = false;
            this.EditKeybindButton.Click += new System.EventHandler(this.EditKeybindButton_Click);
            // 
            // RemoveKeybindButton
            // 
            this.RemoveKeybindButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveKeybindButton.BackColor = System.Drawing.Color.White;
            this.RemoveKeybindButton.Enabled = false;
            this.RemoveKeybindButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RemoveKeybindButton.Location = new System.Drawing.Point(668, 360);
            this.RemoveKeybindButton.Name = "RemoveKeybindButton";
            this.RemoveKeybindButton.Size = new System.Drawing.Size(298, 23);
            this.RemoveKeybindButton.TabIndex = 4;
            this.RemoveKeybindButton.Text = "Remove Keybind";
            this.RemoveKeybindButton.UseVisualStyleBackColor = false;
            this.RemoveKeybindButton.Click += new System.EventHandler(this.RemoveKeybindButton_Click);
            // 
            // DevicesKeybindsSplitContainer
            // 
            this.DevicesKeybindsSplitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DevicesKeybindsSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.DevicesKeybindsSplitContainer.Location = new System.Drawing.Point(6, 58);
            this.DevicesKeybindsSplitContainer.Name = "DevicesKeybindsSplitContainer";
            // 
            // DevicesKeybindsSplitContainer.Panel1
            // 
            this.DevicesKeybindsSplitContainer.Panel1.Controls.Add(this.DevicesListView);
            // 
            // DevicesKeybindsSplitContainer.Panel2
            // 
            this.DevicesKeybindsSplitContainer.Panel2.Controls.Add(this.KeybindsListView);
            this.DevicesKeybindsSplitContainer.Size = new System.Drawing.Size(960, 296);
            this.DevicesKeybindsSplitContainer.SplitterDistance = 298;
            this.DevicesKeybindsSplitContainer.SplitterWidth = 6;
            this.DevicesKeybindsSplitContainer.TabIndex = 5;
            this.DevicesKeybindsSplitContainer.TabStop = false;
            // 
            // DevicesListView
            // 
            this.DevicesListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DevicesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.DeviceNameColumn,
            this.DeviceIDColumn});
            this.DevicesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DevicesListView.FullRowSelect = true;
            this.DevicesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.DevicesListView.HideSelection = false;
            this.DevicesListView.Location = new System.Drawing.Point(0, 0);
            this.DevicesListView.MultiSelect = false;
            this.DevicesListView.Name = "DevicesListView";
            this.DevicesListView.Size = new System.Drawing.Size(298, 296);
            this.DevicesListView.TabIndex = 0;
            this.DevicesListView.TabStop = false;
            this.DevicesListView.UseCompatibleStateImageBehavior = false;
            this.DevicesListView.View = System.Windows.Forms.View.Details;
            this.DevicesListView.SelectedIndexChanged += new System.EventHandler(this.DevicesListView_SelectedIndexChanged);
            this.DevicesListView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DevicesListView_KeyPress);
            this.DevicesListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DevicesListView_MouseClick);
            // 
            // DeviceNameColumn
            // 
            this.DeviceNameColumn.Text = "Device Name";
            this.DeviceNameColumn.Width = 180;
            // 
            // DeviceIDColumn
            // 
            this.DeviceIDColumn.Text = "Device ID";
            this.DeviceIDColumn.Width = 100;
            // 
            // KeybindsListView
            // 
            this.KeybindsListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.KeybindsListView.CheckBoxes = true;
            this.KeybindsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameColumn,
            this.KeysColumn,
            this.ConditionColumn,
            this.MacroColumn});
            this.KeybindsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.KeybindsListView.FullRowSelect = true;
            this.KeybindsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.KeybindsListView.HideSelection = false;
            this.KeybindsListView.Location = new System.Drawing.Point(0, 0);
            this.KeybindsListView.MultiSelect = false;
            this.KeybindsListView.Name = "KeybindsListView";
            this.KeybindsListView.Size = new System.Drawing.Size(656, 296);
            this.KeybindsListView.TabIndex = 1;
            this.KeybindsListView.TabStop = false;
            this.KeybindsListView.UseCompatibleStateImageBehavior = false;
            this.KeybindsListView.View = System.Windows.Forms.View.Details;
            this.KeybindsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.KeybindsListView_ItemChecked);
            this.KeybindsListView.SelectedIndexChanged += new System.EventHandler(this.KeybindsListView_SelectedIndexChanged);
            this.KeybindsListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.KeybindsListView_MouseDoubleClick);
            // 
            // NameColumn
            // 
            this.NameColumn.Text = "Name";
            this.NameColumn.Width = 140;
            // 
            // KeysColumn
            // 
            this.KeysColumn.Text = "Keys";
            this.KeysColumn.Width = 120;
            // 
            // ConditionColumn
            // 
            this.ConditionColumn.Text = "Condition";
            this.ConditionColumn.Width = 185;
            // 
            // MacroColumn
            // 
            this.MacroColumn.Text = "Macro";
            this.MacroColumn.Width = 185;
            // 
            // ShowConsoleCheckBox
            // 
            this.ShowConsoleCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowConsoleCheckBox.AutoSize = true;
            this.ShowConsoleCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ShowConsoleCheckBox.Location = new System.Drawing.Point(882, 37);
            this.ShowConsoleCheckBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 1);
            this.ShowConsoleCheckBox.Name = "ShowConsoleCheckBox";
            this.ShowConsoleCheckBox.Size = new System.Drawing.Size(90, 17);
            this.ShowConsoleCheckBox.TabIndex = 6;
            this.ShowConsoleCheckBox.Text = "Show console";
            this.ShowConsoleCheckBox.UseVisualStyleBackColor = true;
            this.ShowConsoleCheckBox.CheckedChanged += new System.EventHandler(this.ShowConsoleCheckBox_CheckedChanged);
            // 
            // StartWithWindowsCheckBox
            // 
            this.StartWithWindowsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StartWithWindowsCheckBox.AutoSize = true;
            this.StartWithWindowsCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartWithWindowsCheckBox.Location = new System.Drawing.Point(768, 37);
            this.StartWithWindowsCheckBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 1);
            this.StartWithWindowsCheckBox.Name = "StartWithWindowsCheckBox";
            this.StartWithWindowsCheckBox.Size = new System.Drawing.Size(114, 17);
            this.StartWithWindowsCheckBox.TabIndex = 7;
            this.StartWithWindowsCheckBox.Text = "Start with Windows";
            this.StartWithWindowsCheckBox.UseVisualStyleBackColor = true;
            this.StartWithWindowsCheckBox.CheckedChanged += new System.EventHandler(this.StartWithWindowsCheckBox_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(972, 389);
            this.Controls.Add(this.StartWithWindowsCheckBox);
            this.Controls.Add(this.ShowConsoleCheckBox);
            this.Controls.Add(this.RemoveKeybindButton);
            this.Controls.Add(this.EditKeybindButton);
            this.Controls.Add(this.AddKeybindButton);
            this.Controls.Add(this.KeyPressOutputLabel);
            this.Controls.Add(this.DevicesKeybindsSplitContainer);
            this.MinimumSize = new System.Drawing.Size(934, 428);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Multi Device Keybinds";
            this.DevicesKeybindsSplitContainer.Panel1.ResumeLayout(false);
            this.DevicesKeybindsSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DevicesKeybindsSplitContainer)).EndInit();
            this.DevicesKeybindsSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label KeyPressOutputLabel;
        private MultiDeviceKeybinds.CustomListView KeybindsListView;
        private System.Windows.Forms.ColumnHeader KeysColumn;
        private System.Windows.Forms.ColumnHeader MacroColumn;
        private System.Windows.Forms.Button AddKeybindButton;
        private System.Windows.Forms.Button EditKeybindButton;
        private System.Windows.Forms.Button RemoveKeybindButton;
        private System.Windows.Forms.ColumnHeader ConditionColumn;
        private System.Windows.Forms.SplitContainer DevicesKeybindsSplitContainer;
        private System.Windows.Forms.ListView DevicesListView;
        private System.Windows.Forms.ColumnHeader DeviceNameColumn;
        private System.Windows.Forms.ColumnHeader NameColumn;
        private System.Windows.Forms.ColumnHeader DeviceIDColumn;
        private System.Windows.Forms.CheckBox ShowConsoleCheckBox;
        private System.Windows.Forms.CheckBox StartWithWindowsCheckBox;
    }
}

