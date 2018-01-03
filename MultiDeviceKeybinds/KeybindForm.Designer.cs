namespace MultiDeviceKeybinds
{
    partial class KeybindForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeybindForm));
            this.AddEditButton = new System.Windows.Forms.Button();
            this.EnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.NameTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ConditionComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ConditionArgTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.ConditionArgsTakenLabel = new System.Windows.Forms.Label();
            this.MacroComboBox = new System.Windows.Forms.ComboBox();
            this.MacroArgsTakenLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.MacroArgTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ConditionDescriptionLabel = new System.Windows.Forms.Label();
            this.MacroDescriptionLabel = new System.Windows.Forms.Label();
            this.CancelDialogButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.KeysTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.ActivateIfMoreKeysPressedCheckBox = new System.Windows.Forms.CheckBox();
            this.AllowOtherKeybindsCheckBox = new System.Windows.Forms.CheckBox();
            this.MacroArgsListBox = new System.Windows.Forms.ListBox();
            this.RemoveMacroArgButton = new System.Windows.Forms.Button();
            this.AddMacroArgButton = new System.Windows.Forms.Button();
            this.AddConditionArgButton = new System.Windows.Forms.Button();
            this.RemoveConditionArgButton = new System.Windows.Forms.Button();
            this.ConditionArgsListBox = new System.Windows.Forms.ListBox();
            this.ConditionArgTypeComboBox = new System.Windows.Forms.ComboBox();
            this.MacroArgTypeComboBox = new System.Windows.Forms.ComboBox();
            this.ActivateOnKeyDownCheckBox = new System.Windows.Forms.CheckBox();
            this.ActivateOnKeyUpCheckBox = new System.Windows.Forms.CheckBox();
            this.ActivateOnHoldCheckBox = new System.Windows.Forms.CheckBox();
            this.MatchKeysOrderCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // AddEditButton
            // 
            this.AddEditButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AddEditButton.BackColor = System.Drawing.Color.White;
            this.AddEditButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.AddEditButton.Enabled = false;
            this.AddEditButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddEditButton.Location = new System.Drawing.Point(6, 578);
            this.AddEditButton.Name = "AddEditButton";
            this.AddEditButton.Size = new System.Drawing.Size(253, 23);
            this.AddEditButton.TabIndex = 0;
            this.AddEditButton.Text = "Add as new keybind";
            this.AddEditButton.UseVisualStyleBackColor = false;
            this.AddEditButton.Click += new System.EventHandler(this.AddEditButton_Click);
            // 
            // EnabledCheckBox
            // 
            this.EnabledCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EnabledCheckBox.AutoSize = true;
            this.EnabledCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.EnabledCheckBox.Location = new System.Drawing.Point(475, 21);
            this.EnabledCheckBox.Name = "EnabledCheckBox";
            this.EnabledCheckBox.Size = new System.Drawing.Size(62, 17);
            this.EnabledCheckBox.TabIndex = 1;
            this.EnabledCheckBox.Text = "Enabled";
            this.EnabledCheckBox.UseVisualStyleBackColor = true;
            this.EnabledCheckBox.CheckedChanged += new System.EventHandler(this.EnabledCheckBox_CheckedChanged);
            // 
            // NameTextBox
            // 
            this.NameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.NameTextBox.Location = new System.Drawing.Point(6, 19);
            this.NameTextBox.Margin = new System.Windows.Forms.Padding(3, 3, 3, 4);
            this.NameTextBox.Name = "NameTextBox";
            this.NameTextBox.Size = new System.Drawing.Size(463, 20);
            this.NameTextBox.TabIndex = 2;
            this.NameTextBox.TextChanged += new System.EventHandler(this.NameTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 99);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Condition:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 303);
            this.label3.Margin = new System.Windows.Forms.Padding(0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Macro:";
            // 
            // ConditionComboBox
            // 
            this.ConditionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConditionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ConditionComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ConditionComboBox.FormattingEnabled = true;
            this.ConditionComboBox.Items.AddRange(new object[] {
            "No condition"});
            this.ConditionComboBox.Location = new System.Drawing.Point(6, 115);
            this.ConditionComboBox.Name = "ConditionComboBox";
            this.ConditionComboBox.Size = new System.Drawing.Size(531, 21);
            this.ConditionComboBox.TabIndex = 6;
            this.ConditionComboBox.SelectedIndexChanged += new System.EventHandler(this.ConditionComboBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 178);
            this.label4.Margin = new System.Windows.Forms.Padding(0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Condition arguments:";
            // 
            // ConditionArgTextBox
            // 
            this.ConditionArgTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConditionArgTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ConditionArgTextBox.Location = new System.Drawing.Point(162, 253);
            this.ConditionArgTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 1);
            this.ConditionArgTextBox.Name = "ConditionArgTextBox";
            this.ConditionArgTextBox.Size = new System.Drawing.Size(375, 20);
            this.ConditionArgTextBox.TabIndex = 8;
            this.ConditionArgTextBox.TextChanged += new System.EventHandler(this.ConditionArgTextBox_TextChanged);
            this.ConditionArgTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ConditionArgTextBox_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 152);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(136, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Condition arguments taken:";
            // 
            // ConditionArgsTakenLabel
            // 
            this.ConditionArgsTakenLabel.AutoSize = true;
            this.ConditionArgsTakenLabel.Location = new System.Drawing.Point(3, 165);
            this.ConditionArgsTakenLabel.Margin = new System.Windows.Forms.Padding(0);
            this.ConditionArgsTakenLabel.MaximumSize = new System.Drawing.Size(518, 130);
            this.ConditionArgsTakenLabel.Name = "ConditionArgsTakenLabel";
            this.ConditionArgsTakenLabel.Size = new System.Drawing.Size(33, 13);
            this.ConditionArgsTakenLabel.TabIndex = 10;
            this.ConditionArgsTakenLabel.Text = "None";
            // 
            // MacroComboBox
            // 
            this.MacroComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MacroComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MacroComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MacroComboBox.FormattingEnabled = true;
            this.MacroComboBox.Items.AddRange(new object[] {
            "No macro (block keys)"});
            this.MacroComboBox.Location = new System.Drawing.Point(6, 319);
            this.MacroComboBox.Name = "MacroComboBox";
            this.MacroComboBox.Size = new System.Drawing.Size(531, 21);
            this.MacroComboBox.TabIndex = 11;
            this.MacroComboBox.SelectedIndexChanged += new System.EventHandler(this.MacroComboBox_SelectedIndexChanged);
            // 
            // MacroArgsTakenLabel
            // 
            this.MacroArgsTakenLabel.AutoSize = true;
            this.MacroArgsTakenLabel.Location = new System.Drawing.Point(3, 369);
            this.MacroArgsTakenLabel.Margin = new System.Windows.Forms.Padding(0);
            this.MacroArgsTakenLabel.MaximumSize = new System.Drawing.Size(518, 130);
            this.MacroArgsTakenLabel.Name = "MacroArgsTakenLabel";
            this.MacroArgsTakenLabel.Size = new System.Drawing.Size(33, 13);
            this.MacroArgsTakenLabel.TabIndex = 15;
            this.MacroArgsTakenLabel.Text = "None";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 356);
            this.label7.Margin = new System.Windows.Forms.Padding(0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(122, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "Macro arguments taken:";
            // 
            // MacroArgTextBox
            // 
            this.MacroArgTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MacroArgTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.MacroArgTextBox.Location = new System.Drawing.Point(162, 457);
            this.MacroArgTextBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 1);
            this.MacroArgTextBox.Name = "MacroArgTextBox";
            this.MacroArgTextBox.Size = new System.Drawing.Size(375, 20);
            this.MacroArgTextBox.TabIndex = 13;
            this.MacroArgTextBox.TextChanged += new System.EventHandler(this.MacroArgTextBox_TextChanged);
            this.MacroArgTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MacroArgTextBox_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(3, 382);
            this.label8.Margin = new System.Windows.Forms.Padding(0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Macro arguments:";
            // 
            // ConditionDescriptionLabel
            // 
            this.ConditionDescriptionLabel.AutoSize = true;
            this.ConditionDescriptionLabel.Location = new System.Drawing.Point(3, 139);
            this.ConditionDescriptionLabel.Margin = new System.Windows.Forms.Padding(0);
            this.ConditionDescriptionLabel.MaximumSize = new System.Drawing.Size(518, 86);
            this.ConditionDescriptionLabel.Name = "ConditionDescriptionLabel";
            this.ConditionDescriptionLabel.Size = new System.Drawing.Size(81, 13);
            this.ConditionDescriptionLabel.TabIndex = 16;
            this.ConditionDescriptionLabel.Text = "Checks nothing";
            // 
            // MacroDescriptionLabel
            // 
            this.MacroDescriptionLabel.AutoSize = true;
            this.MacroDescriptionLabel.Location = new System.Drawing.Point(3, 343);
            this.MacroDescriptionLabel.Margin = new System.Windows.Forms.Padding(0);
            this.MacroDescriptionLabel.MaximumSize = new System.Drawing.Size(518, 86);
            this.MacroDescriptionLabel.Name = "MacroDescriptionLabel";
            this.MacroDescriptionLabel.Size = new System.Drawing.Size(70, 13);
            this.MacroDescriptionLabel.TabIndex = 17;
            this.MacroDescriptionLabel.Text = "Does nothing";
            // 
            // CancelDialogButton
            // 
            this.CancelDialogButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelDialogButton.BackColor = System.Drawing.Color.White;
            this.CancelDialogButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelDialogButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelDialogButton.Location = new System.Drawing.Point(284, 578);
            this.CancelDialogButton.Name = "CancelDialogButton";
            this.CancelDialogButton.Size = new System.Drawing.Size(253, 23);
            this.CancelDialogButton.TabIndex = 18;
            this.CancelDialogButton.Text = "Cancel";
            this.CancelDialogButton.UseVisualStyleBackColor = false;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 523);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.MaximumSize = new System.Drawing.Size(537, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(528, 52);
            this.label6.TabIndex = 19;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // KeysTextBox
            // 
            this.KeysTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.KeysTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.KeysTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.KeysTextBox.Location = new System.Drawing.Point(6, 59);
            this.KeysTextBox.Name = "KeysTextBox";
            this.KeysTextBox.ReadOnly = true;
            this.KeysTextBox.Size = new System.Drawing.Size(531, 20);
            this.KeysTextBox.TabIndex = 20;
            this.KeysTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.KeysTextBox_KeyDown);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(3, 43);
            this.label9.Margin = new System.Windows.Forms.Padding(0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(443, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Keys (click the box below and press the desired keys, use the device that this ke" +
    "ybind is for):";
            // 
            // ActivateIfMoreKeysPressedCheckBox
            // 
            this.ActivateIfMoreKeysPressedCheckBox.AutoSize = true;
            this.ActivateIfMoreKeysPressedCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ActivateIfMoreKeysPressedCheckBox.Location = new System.Drawing.Point(358, 82);
            this.ActivateIfMoreKeysPressedCheckBox.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.ActivateIfMoreKeysPressedCheckBox.Name = "ActivateIfMoreKeysPressedCheckBox";
            this.ActivateIfMoreKeysPressedCheckBox.Size = new System.Drawing.Size(179, 17);
            this.ActivateIfMoreKeysPressedCheckBox.TabIndex = 22;
            this.ActivateIfMoreKeysPressedCheckBox.Text = "Activate if more keys are pressed";
            this.ActivateIfMoreKeysPressedCheckBox.UseVisualStyleBackColor = true;
            this.ActivateIfMoreKeysPressedCheckBox.CheckedChanged += new System.EventHandler(this.ActivateIfMoreKeysPressedCheckBox_CheckedChanged);
            // 
            // AllowOtherKeybindsCheckBox
            // 
            this.AllowOtherKeybindsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AllowOtherKeybindsCheckBox.AutoSize = true;
            this.AllowOtherKeybindsCheckBox.Checked = true;
            this.AllowOtherKeybindsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AllowOtherKeybindsCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AllowOtherKeybindsCheckBox.Location = new System.Drawing.Point(6, 506);
            this.AllowOtherKeybindsCheckBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.AllowOtherKeybindsCheckBox.Name = "AllowOtherKeybindsCheckBox";
            this.AllowOtherKeybindsCheckBox.Size = new System.Drawing.Size(164, 17);
            this.AllowOtherKeybindsCheckBox.TabIndex = 23;
            this.AllowOtherKeybindsCheckBox.Text = "Allow other keybinds to trigger";
            this.AllowOtherKeybindsCheckBox.UseVisualStyleBackColor = true;
            this.AllowOtherKeybindsCheckBox.CheckedChanged += new System.EventHandler(this.AllowOtherKeybindsCheckBox_CheckedChanged);
            // 
            // MacroArgsListBox
            // 
            this.MacroArgsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MacroArgsListBox.FormattingEnabled = true;
            this.MacroArgsListBox.HorizontalScrollbar = true;
            this.MacroArgsListBox.Location = new System.Drawing.Point(6, 398);
            this.MacroArgsListBox.Name = "MacroArgsListBox";
            this.MacroArgsListBox.Size = new System.Drawing.Size(531, 56);
            this.MacroArgsListBox.TabIndex = 24;
            this.MacroArgsListBox.SelectedIndexChanged += new System.EventHandler(this.MacroArgsListBox_SelectedIndexChanged);
            // 
            // RemoveMacroArgButton
            // 
            this.RemoveMacroArgButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveMacroArgButton.BackColor = System.Drawing.Color.White;
            this.RemoveMacroArgButton.Enabled = false;
            this.RemoveMacroArgButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RemoveMacroArgButton.Location = new System.Drawing.Point(387, 481);
            this.RemoveMacroArgButton.Name = "RemoveMacroArgButton";
            this.RemoveMacroArgButton.Size = new System.Drawing.Size(150, 23);
            this.RemoveMacroArgButton.TabIndex = 25;
            this.RemoveMacroArgButton.Text = "Remove macro argument";
            this.RemoveMacroArgButton.UseVisualStyleBackColor = false;
            this.RemoveMacroArgButton.Click += new System.EventHandler(this.RemoveMacroArgButton_Click);
            // 
            // AddMacroArgButton
            // 
            this.AddMacroArgButton.BackColor = System.Drawing.Color.White;
            this.AddMacroArgButton.Enabled = false;
            this.AddMacroArgButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddMacroArgButton.Location = new System.Drawing.Point(6, 481);
            this.AddMacroArgButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.AddMacroArgButton.Name = "AddMacroArgButton";
            this.AddMacroArgButton.Size = new System.Drawing.Size(150, 23);
            this.AddMacroArgButton.TabIndex = 26;
            this.AddMacroArgButton.Text = "Add macro argument";
            this.AddMacroArgButton.UseVisualStyleBackColor = false;
            this.AddMacroArgButton.Click += new System.EventHandler(this.AddMacroArgButton_Click);
            // 
            // AddConditionArgButton
            // 
            this.AddConditionArgButton.BackColor = System.Drawing.Color.White;
            this.AddConditionArgButton.Enabled = false;
            this.AddConditionArgButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.AddConditionArgButton.Location = new System.Drawing.Point(6, 277);
            this.AddConditionArgButton.Name = "AddConditionArgButton";
            this.AddConditionArgButton.Size = new System.Drawing.Size(150, 23);
            this.AddConditionArgButton.TabIndex = 29;
            this.AddConditionArgButton.Text = "Add condition argument";
            this.AddConditionArgButton.UseVisualStyleBackColor = false;
            this.AddConditionArgButton.Click += new System.EventHandler(this.AddConditionArgButton_Click);
            // 
            // RemoveConditionArgButton
            // 
            this.RemoveConditionArgButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveConditionArgButton.BackColor = System.Drawing.Color.White;
            this.RemoveConditionArgButton.Enabled = false;
            this.RemoveConditionArgButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RemoveConditionArgButton.Location = new System.Drawing.Point(387, 277);
            this.RemoveConditionArgButton.Name = "RemoveConditionArgButton";
            this.RemoveConditionArgButton.Size = new System.Drawing.Size(150, 23);
            this.RemoveConditionArgButton.TabIndex = 28;
            this.RemoveConditionArgButton.Text = "Remove condition argument";
            this.RemoveConditionArgButton.UseVisualStyleBackColor = false;
            this.RemoveConditionArgButton.Click += new System.EventHandler(this.RemoveConditionArgButton_Click);
            // 
            // ConditionArgsListBox
            // 
            this.ConditionArgsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConditionArgsListBox.FormattingEnabled = true;
            this.ConditionArgsListBox.HorizontalScrollbar = true;
            this.ConditionArgsListBox.Location = new System.Drawing.Point(6, 194);
            this.ConditionArgsListBox.Name = "ConditionArgsListBox";
            this.ConditionArgsListBox.Size = new System.Drawing.Size(531, 56);
            this.ConditionArgsListBox.TabIndex = 27;
            this.ConditionArgsListBox.SelectedIndexChanged += new System.EventHandler(this.ConditionArgsListBox_SelectedIndexChanged);
            // 
            // ConditionArgTypeComboBox
            // 
            this.ConditionArgTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ConditionArgTypeComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ConditionArgTypeComboBox.FormattingEnabled = true;
            this.ConditionArgTypeComboBox.Location = new System.Drawing.Point(6, 253);
            this.ConditionArgTypeComboBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.ConditionArgTypeComboBox.Name = "ConditionArgTypeComboBox";
            this.ConditionArgTypeComboBox.Size = new System.Drawing.Size(150, 21);
            this.ConditionArgTypeComboBox.TabIndex = 30;
            this.ConditionArgTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.ConditionArgTypeComboBox_SelectedIndexChanged);
            // 
            // MacroArgTypeComboBox
            // 
            this.MacroArgTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MacroArgTypeComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MacroArgTypeComboBox.FormattingEnabled = true;
            this.MacroArgTypeComboBox.Location = new System.Drawing.Point(6, 457);
            this.MacroArgTypeComboBox.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.MacroArgTypeComboBox.Name = "MacroArgTypeComboBox";
            this.MacroArgTypeComboBox.Size = new System.Drawing.Size(150, 21);
            this.MacroArgTypeComboBox.TabIndex = 31;
            this.MacroArgTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.MacroArgTypeComboBox_SelectedIndexChanged);
            // 
            // ActivateOnKeyDownCheckBox
            // 
            this.ActivateOnKeyDownCheckBox.AutoSize = true;
            this.ActivateOnKeyDownCheckBox.Checked = true;
            this.ActivateOnKeyDownCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ActivateOnKeyDownCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ActivateOnKeyDownCheckBox.Location = new System.Drawing.Point(6, 82);
            this.ActivateOnKeyDownCheckBox.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.ActivateOnKeyDownCheckBox.Name = "ActivateOnKeyDownCheckBox";
            this.ActivateOnKeyDownCheckBox.Size = new System.Drawing.Size(126, 17);
            this.ActivateOnKeyDownCheckBox.TabIndex = 32;
            this.ActivateOnKeyDownCheckBox.Text = "Activate on key down";
            this.ActivateOnKeyDownCheckBox.UseVisualStyleBackColor = true;
            this.ActivateOnKeyDownCheckBox.CheckedChanged += new System.EventHandler(this.ActivateOnKeyDownCheckBox_CheckedChanged);
            // 
            // ActivateOnKeyUpCheckBox
            // 
            this.ActivateOnKeyUpCheckBox.AutoSize = true;
            this.ActivateOnKeyUpCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ActivateOnKeyUpCheckBox.Location = new System.Drawing.Point(246, 82);
            this.ActivateOnKeyUpCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.ActivateOnKeyUpCheckBox.Name = "ActivateOnKeyUpCheckBox";
            this.ActivateOnKeyUpCheckBox.Size = new System.Drawing.Size(112, 17);
            this.ActivateOnKeyUpCheckBox.TabIndex = 33;
            this.ActivateOnKeyUpCheckBox.Text = "Activate on key up";
            this.ActivateOnKeyUpCheckBox.UseVisualStyleBackColor = true;
            this.ActivateOnKeyUpCheckBox.CheckedChanged += new System.EventHandler(this.ActivateOnKeyUpCheckBox_CheckedChanged);
            // 
            // ActivateOnHoldCheckBox
            // 
            this.ActivateOnHoldCheckBox.AutoSize = true;
            this.ActivateOnHoldCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ActivateOnHoldCheckBox.Location = new System.Drawing.Point(132, 82);
            this.ActivateOnHoldCheckBox.Margin = new System.Windows.Forms.Padding(0);
            this.ActivateOnHoldCheckBox.Name = "ActivateOnHoldCheckBox";
            this.ActivateOnHoldCheckBox.Size = new System.Drawing.Size(114, 17);
            this.ActivateOnHoldCheckBox.TabIndex = 34;
            this.ActivateOnHoldCheckBox.Text = "Activate when held";
            this.ActivateOnHoldCheckBox.UseVisualStyleBackColor = true;
            this.ActivateOnHoldCheckBox.CheckedChanged += new System.EventHandler(this.ActivateOnHoldCheckBox_CheckedChanged);
            // 
            // MatchKeysOrderCheckBox
            // 
            this.MatchKeysOrderCheckBox.AutoSize = true;
            this.MatchKeysOrderCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MatchKeysOrderCheckBox.Location = new System.Drawing.Point(457, 41);
            this.MatchKeysOrderCheckBox.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.MatchKeysOrderCheckBox.Name = "MatchKeysOrderCheckBox";
            this.MatchKeysOrderCheckBox.Size = new System.Drawing.Size(80, 17);
            this.MatchKeysOrderCheckBox.TabIndex = 35;
            this.MatchKeysOrderCheckBox.Text = "Match order";
            this.MatchKeysOrderCheckBox.UseVisualStyleBackColor = true;
            this.MatchKeysOrderCheckBox.CheckedChanged += new System.EventHandler(this.MatchKeysOrderCheckBox_CheckedChanged);
            // 
            // KeybindForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(543, 607);
            this.Controls.Add(this.MatchKeysOrderCheckBox);
            this.Controls.Add(this.ActivateOnHoldCheckBox);
            this.Controls.Add(this.ActivateOnKeyUpCheckBox);
            this.Controls.Add(this.ActivateOnKeyDownCheckBox);
            this.Controls.Add(this.MacroArgTypeComboBox);
            this.Controls.Add(this.ConditionArgTypeComboBox);
            this.Controls.Add(this.AddMacroArgButton);
            this.Controls.Add(this.RemoveMacroArgButton);
            this.Controls.Add(this.MacroArgsListBox);
            this.Controls.Add(this.AllowOtherKeybindsCheckBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.KeysTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.CancelDialogButton);
            this.Controls.Add(this.MacroDescriptionLabel);
            this.Controls.Add(this.ConditionDescriptionLabel);
            this.Controls.Add(this.MacroArgsTakenLabel);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.MacroArgTextBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.MacroComboBox);
            this.Controls.Add(this.ConditionArgsTakenLabel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.ConditionArgTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ConditionComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.NameTextBox);
            this.Controls.Add(this.EnabledCheckBox);
            this.Controls.Add(this.AddEditButton);
            this.Controls.Add(this.ActivateIfMoreKeysPressedCheckBox);
            this.Controls.Add(this.AddConditionArgButton);
            this.Controls.Add(this.RemoveConditionArgButton);
            this.Controls.Add(this.ConditionArgsListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "KeybindForm";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Keybind management";
            this.Resize += new System.EventHandler(this.KeybindForm_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AddEditButton;
        private System.Windows.Forms.CheckBox EnabledCheckBox;
        private System.Windows.Forms.TextBox NameTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox ConditionArgTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label ConditionArgsTakenLabel;
        private System.Windows.Forms.Label MacroArgsTakenLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox MacroArgTextBox;
        private System.Windows.Forms.Label label8;
        internal System.Windows.Forms.ComboBox ConditionComboBox;
        internal System.Windows.Forms.ComboBox MacroComboBox;
        private System.Windows.Forms.Label ConditionDescriptionLabel;
        private System.Windows.Forms.Label MacroDescriptionLabel;
        private System.Windows.Forms.Button CancelDialogButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox KeysTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox ActivateIfMoreKeysPressedCheckBox;
        private System.Windows.Forms.CheckBox AllowOtherKeybindsCheckBox;
        private System.Windows.Forms.ListBox MacroArgsListBox;
        private System.Windows.Forms.Button RemoveMacroArgButton;
        private System.Windows.Forms.Button AddMacroArgButton;
        private System.Windows.Forms.Button AddConditionArgButton;
        private System.Windows.Forms.Button RemoveConditionArgButton;
        private System.Windows.Forms.ListBox ConditionArgsListBox;
        private System.Windows.Forms.ComboBox ConditionArgTypeComboBox;
        private System.Windows.Forms.ComboBox MacroArgTypeComboBox;
        private System.Windows.Forms.CheckBox ActivateOnKeyDownCheckBox;
        private System.Windows.Forms.CheckBox ActivateOnKeyUpCheckBox;
        private System.Windows.Forms.CheckBox ActivateOnHoldCheckBox;
        private System.Windows.Forms.CheckBox MatchKeysOrderCheckBox;
    }
}