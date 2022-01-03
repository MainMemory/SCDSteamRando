
namespace SCDSteamRando
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			this.seedSelector = new System.Windows.Forms.NumericUpDown();
			this.randomSeed = new System.Windows.Forms.CheckBox();
			this.randomMusic = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.mainPathSelector = new System.Windows.Forms.ComboBox();
			this.maxBackJump = new System.Windows.Forms.NumericUpDown();
			this.maxForwJump = new System.Windows.Forms.NumericUpDown();
			this.presetSelector = new System.Windows.Forms.ComboBox();
			this.randomizeButton = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.spoilerLevelInfo = new System.Windows.Forms.TextBox();
			this.spoilerLevelList = new System.Windows.Forms.ListBox();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.seedSelector)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maxBackJump)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maxForwJump)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(6, 8);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(35, 13);
			label1.TabIndex = 0;
			label1.Text = "Seed:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(6, 35);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(58, 13);
			label2.TabIndex = 4;
			label2.Text = "Main Path:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(6, 61);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(114, 13);
			label3.TabIndex = 6;
			label3.Text = "Max Backwards Jump:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(6, 87);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(104, 13);
			label4.TabIndex = 8;
			label4.Text = "Max Forwards Jump:";
			// 
			// seedSelector
			// 
			this.seedSelector.Location = new System.Drawing.Point(47, 6);
			this.seedSelector.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
			this.seedSelector.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
			this.seedSelector.Name = "seedSelector";
			this.seedSelector.Size = new System.Drawing.Size(120, 20);
			this.seedSelector.TabIndex = 1;
			this.toolTip1.SetToolTip(this.seedSelector, "This value controls how things are randomized.");
			// 
			// randomSeed
			// 
			this.randomSeed.AutoSize = true;
			this.randomSeed.Location = new System.Drawing.Point(173, 7);
			this.randomSeed.Name = "randomSeed";
			this.randomSeed.Size = new System.Drawing.Size(66, 17);
			this.randomSeed.TabIndex = 2;
			this.randomSeed.Text = "Random";
			this.toolTip1.SetToolTip(this.randomSeed, "Check this box to have the randomizer generate a seed for you based on the curren" +
        "t time.");
			this.randomSeed.UseVisualStyleBackColor = true;
			this.randomSeed.CheckedChanged += new System.EventHandler(this.randomSeed_CheckedChanged);
			// 
			// randomMusic
			// 
			this.randomMusic.AutoSize = true;
			this.randomMusic.Location = new System.Drawing.Point(6, 138);
			this.randomMusic.Name = "randomMusic";
			this.randomMusic.Size = new System.Drawing.Size(110, 17);
			this.randomMusic.TabIndex = 3;
			this.randomMusic.Text = "Randomize Music";
			this.toolTip1.SetToolTip(this.randomMusic, "Check this box to shuffle the music that\'s played in each area of the game.");
			this.randomMusic.UseVisualStyleBackColor = true;
			// 
			// mainPathSelector
			// 
			this.mainPathSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mainPathSelector.Items.AddRange(new object[] {
            "Act Clear Only",
            "Any Exit"});
			this.mainPathSelector.Location = new System.Drawing.Point(70, 32);
			this.mainPathSelector.Name = "mainPathSelector";
			this.mainPathSelector.Size = new System.Drawing.Size(121, 21);
			this.mainPathSelector.TabIndex = 5;
			this.toolTip1.SetToolTip(this.mainPathSelector, "If this is set to \"Act Clear Only\", you will be able to beat the game by completi" +
        "ng all 69 stages normally. Otherwise time travel may be required to progress.");
			// 
			// maxBackJump
			// 
			this.maxBackJump.Location = new System.Drawing.Point(126, 59);
			this.maxBackJump.Maximum = new decimal(new int[] {
            68,
            0,
            0,
            0});
			this.maxBackJump.Name = "maxBackJump";
			this.maxBackJump.Size = new System.Drawing.Size(41, 20);
			this.maxBackJump.TabIndex = 7;
			this.toolTip1.SetToolTip(this.maxBackJump, "The maximum number of stages along the main path that you can get sent backwards." +
        "");
			this.maxBackJump.Value = new decimal(new int[] {
            68,
            0,
            0,
            0});
			// 
			// maxForwJump
			// 
			this.maxForwJump.Location = new System.Drawing.Point(116, 85);
			this.maxForwJump.Maximum = new decimal(new int[] {
            68,
            0,
            0,
            0});
			this.maxForwJump.Name = "maxForwJump";
			this.maxForwJump.Size = new System.Drawing.Size(41, 20);
			this.maxForwJump.TabIndex = 9;
			this.toolTip1.SetToolTip(this.maxForwJump, "The maximum number of stages along the main path that you can get sent forwards.");
			this.maxForwJump.Value = new decimal(new int[] {
            68,
            0,
            0,
            0});
			// 
			// presetSelector
			// 
			this.presetSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.presetSelector.Items.AddRange(new object[] {
            "Easiest",
            "Very Easy",
            "Easy",
            "Medium",
            "Hard",
            "Very Hard",
            "Insane",
            "All Levels",
            "Full Random"});
			this.presetSelector.Location = new System.Drawing.Point(52, 111);
			this.presetSelector.Name = "presetSelector";
			this.presetSelector.Size = new System.Drawing.Size(121, 21);
			this.presetSelector.TabIndex = 11;
			this.toolTip1.SetToolTip(this.presetSelector, "Selectable presets for backwards/forwards jumps.");
			this.presetSelector.SelectedIndexChanged += new System.EventHandler(this.presetSelector_SelectedIndexChanged);
			// 
			// randomizeButton
			// 
			this.randomizeButton.Location = new System.Drawing.Point(164, 138);
			this.randomizeButton.Name = "randomizeButton";
			this.randomizeButton.Size = new System.Drawing.Size(75, 23);
			this.randomizeButton.TabIndex = 12;
			this.randomizeButton.Text = "Randomize!";
			this.toolTip1.SetToolTip(this.randomizeButton, "Click this button to randomize the game with these settings.");
			this.randomizeButton.UseVisualStyleBackColor = true;
			this.randomizeButton.Click += new System.EventHandler(this.randomizeButton_Click);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 114);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(40, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "Preset:";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(503, 199);
			this.tabControl1.TabIndex = 12;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.randomizeButton);
			this.tabPage1.Controls.Add(label1);
			this.tabPage1.Controls.Add(this.presetSelector);
			this.tabPage1.Controls.Add(this.seedSelector);
			this.tabPage1.Controls.Add(this.label5);
			this.tabPage1.Controls.Add(this.randomSeed);
			this.tabPage1.Controls.Add(this.maxForwJump);
			this.tabPage1.Controls.Add(this.randomMusic);
			this.tabPage1.Controls.Add(label4);
			this.tabPage1.Controls.Add(label2);
			this.tabPage1.Controls.Add(this.maxBackJump);
			this.tabPage1.Controls.Add(this.mainPathSelector);
			this.tabPage1.Controls.Add(label3);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(495, 173);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Settings";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.spoilerLevelInfo);
			this.tabPage2.Controls.Add(this.spoilerLevelList);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(495, 173);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Spoilers";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// spoilerLevelInfo
			// 
			this.spoilerLevelInfo.Location = new System.Drawing.Point(225, 6);
			this.spoilerLevelInfo.Multiline = true;
			this.spoilerLevelInfo.Name = "spoilerLevelInfo";
			this.spoilerLevelInfo.ReadOnly = true;
			this.spoilerLevelInfo.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.spoilerLevelInfo.Size = new System.Drawing.Size(260, 121);
			this.spoilerLevelInfo.TabIndex = 1;
			this.spoilerLevelInfo.WordWrap = false;
			// 
			// spoilerLevelList
			// 
			this.spoilerLevelList.Enabled = false;
			this.spoilerLevelList.FormattingEnabled = true;
			this.spoilerLevelList.Location = new System.Drawing.Point(6, 6);
			this.spoilerLevelList.Name = "spoilerLevelList";
			this.spoilerLevelList.Size = new System.Drawing.Size(213, 121);
			this.spoilerLevelList.TabIndex = 0;
			this.spoilerLevelList.SelectedIndexChanged += new System.EventHandler(this.spoilerLevelList_SelectedIndexChanged);
			// 
			// MainForm
			// 
			this.AcceptButton = this.randomizeButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(503, 199);
			this.Controls.Add(this.tabControl1);
			this.Name = "MainForm";
			this.Text = "Sonic CD 2011 Randomizer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.seedSelector)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maxBackJump)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maxForwJump)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.NumericUpDown seedSelector;
		private System.Windows.Forms.CheckBox randomSeed;
		private System.Windows.Forms.CheckBox randomMusic;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ComboBox mainPathSelector;
		private System.Windows.Forms.NumericUpDown maxBackJump;
		private System.Windows.Forms.NumericUpDown maxForwJump;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox presetSelector;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Button randomizeButton;
		private System.Windows.Forms.ListBox spoilerLevelList;
		private System.Windows.Forms.TextBox spoilerLevelInfo;
	}
}