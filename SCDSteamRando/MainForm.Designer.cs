
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
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label6;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.seedSelector = new System.Windows.Forms.NumericUpDown();
			this.randomSeed = new System.Windows.Forms.CheckBox();
			this.randomMusic = new System.Windows.Forms.CheckBox();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.mainPathSelector = new System.Windows.Forms.ComboBox();
			this.maxBackJump = new System.Windows.Forms.NumericUpDown();
			this.maxForwJump = new System.Windows.Forms.NumericUpDown();
			this.presetSelector = new System.Windows.Forms.ComboBox();
			this.randomizeButton = new System.Windows.Forms.Button();
			this.backJumpProb = new System.Windows.Forms.NumericUpDown();
			this.allowSameLevel = new System.Windows.Forms.CheckBox();
			this.modeSelector = new System.Windows.Forms.ComboBox();
			this.separateSoundtracks = new System.Windows.Forms.CheckBox();
			this.saveLogButton = new System.Windows.Forms.Button();
			this.makeChartButton = new System.Windows.Forms.Button();
			this.randomItemMode = new System.Windows.Forms.ComboBox();
			this.randomTimePosts = new System.Windows.Forms.CheckBox();
			this.replaceCheckpoints = new System.Windows.Forms.CheckBox();
			this.randomPalettes = new System.Windows.Forms.CheckBox();
			this.ufoDifficulty = new System.Windows.Forms.ComboBox();
			this.randomUFOs = new System.Windows.Forms.CheckBox();
			this.randomWater = new System.Windows.Forms.CheckBox();
			this.addWaterOnly = new System.Windows.Forms.CheckBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label7 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.spoilerLevelInfo = new System.Windows.Forms.TextBox();
			this.spoilerLevelList = new System.Windows.Forms.ListBox();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.seedSelector)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maxBackJump)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.maxForwJump)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.backJumpProb)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.tabPage3.SuspendLayout();
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
			label2.Location = new System.Drawing.Point(3, 6);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(58, 13);
			label2.TabIndex = 0;
			label2.Text = "Main Path:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(3, 32);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(114, 13);
			label3.TabIndex = 2;
			label3.Text = "Max Backwards Jump:";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(3, 58);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(104, 13);
			label4.TabIndex = 4;
			label4.Text = "Max Forwards Jump:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(3, 111);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(40, 13);
			label5.TabIndex = 9;
			label5.Text = "Preset:";
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(3, 87);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(142, 13);
			label6.TabIndex = 6;
			label6.Text = "Backwards Jump Probability:";
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
			this.randomMusic.Location = new System.Drawing.Point(8, 6);
			this.randomMusic.Name = "randomMusic";
			this.randomMusic.Size = new System.Drawing.Size(110, 17);
			this.randomMusic.TabIndex = 6;
			this.randomMusic.Text = "Randomize Music";
			this.toolTip1.SetToolTip(this.randomMusic, "Check this box to shuffle the music that\'s played in each area of the game.");
			this.randomMusic.UseVisualStyleBackColor = true;
			// 
			// mainPathSelector
			// 
			this.mainPathSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.mainPathSelector.Items.AddRange(new object[] {
            "Act Clear",
            "Time Travel",
            "Any Exit"});
			this.mainPathSelector.Location = new System.Drawing.Point(67, 3);
			this.mainPathSelector.Name = "mainPathSelector";
			this.mainPathSelector.Size = new System.Drawing.Size(121, 21);
			this.mainPathSelector.TabIndex = 1;
			this.toolTip1.SetToolTip(this.mainPathSelector, "Which exits from a level are allowed to be part of the main path.");
			// 
			// maxBackJump
			// 
			this.maxBackJump.Location = new System.Drawing.Point(123, 30);
			this.maxBackJump.Maximum = new decimal(new int[] {
            69,
            0,
            0,
            0});
			this.maxBackJump.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.maxBackJump.Name = "maxBackJump";
			this.maxBackJump.Size = new System.Drawing.Size(41, 20);
			this.maxBackJump.TabIndex = 3;
			this.toolTip1.SetToolTip(this.maxBackJump, "The maximum number of stages along the main path that you can get sent backwards." +
        "");
			this.maxBackJump.Value = new decimal(new int[] {
            69,
            0,
            0,
            0});
			// 
			// maxForwJump
			// 
			this.maxForwJump.Location = new System.Drawing.Point(113, 56);
			this.maxForwJump.Maximum = new decimal(new int[] {
            70,
            0,
            0,
            0});
			this.maxForwJump.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.maxForwJump.Name = "maxForwJump";
			this.maxForwJump.Size = new System.Drawing.Size(41, 20);
			this.maxForwJump.TabIndex = 5;
			this.toolTip1.SetToolTip(this.maxForwJump, "The maximum number of stages along the main path that you can get sent forwards.");
			this.maxForwJump.Value = new decimal(new int[] {
            70,
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
			this.presetSelector.Location = new System.Drawing.Point(49, 108);
			this.presetSelector.Name = "presetSelector";
			this.presetSelector.Size = new System.Drawing.Size(121, 21);
			this.presetSelector.TabIndex = 10;
			this.toolTip1.SetToolTip(this.presetSelector, "Selectable presets for backwards/forwards jumps.");
			this.presetSelector.SelectedIndexChanged += new System.EventHandler(this.presetSelector_SelectedIndexChanged);
			// 
			// randomizeButton
			// 
			this.randomizeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.randomizeButton.Location = new System.Drawing.Point(416, 224);
			this.randomizeButton.Name = "randomizeButton";
			this.randomizeButton.Size = new System.Drawing.Size(75, 23);
			this.randomizeButton.TabIndex = 7;
			this.randomizeButton.Text = "Randomize!";
			this.toolTip1.SetToolTip(this.randomizeButton, "Click this button to randomize the game with these settings.");
			this.randomizeButton.UseVisualStyleBackColor = true;
			this.randomizeButton.Click += new System.EventHandler(this.randomizeButton_Click);
			// 
			// backJumpProb
			// 
			this.backJumpProb.Location = new System.Drawing.Point(151, 82);
			this.backJumpProb.Name = "backJumpProb";
			this.backJumpProb.Size = new System.Drawing.Size(45, 20);
			this.backJumpProb.TabIndex = 7;
			this.toolTip1.SetToolTip(this.backJumpProb, "The probability that a backwards jump will be chosen instead of a forwards jump.");
			this.backJumpProb.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
			// 
			// allowSameLevel
			// 
			this.allowSameLevel.AutoSize = true;
			this.allowSameLevel.Location = new System.Drawing.Point(202, 83);
			this.allowSameLevel.Name = "allowSameLevel";
			this.allowSameLevel.Size = new System.Drawing.Size(155, 17);
			this.allowSameLevel.TabIndex = 8;
			this.allowSameLevel.Text = "Allow Jumps to Same Level";
			this.toolTip1.SetToolTip(this.allowSameLevel, "If checked, warps may take you to the start of the level you\'re in currently.");
			this.allowSameLevel.UseVisualStyleBackColor = true;
			this.allowSameLevel.CheckedChanged += new System.EventHandler(this.allowSameLevel_CheckedChanged);
			// 
			// modeSelector
			// 
			this.modeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.modeSelector.FormattingEnabled = true;
			this.modeSelector.Items.AddRange(new object[] {
            "Shuffle All Stages w/ Warps",
            "Shuffle Rounds",
            "Shuffle Acts",
            "Shuffle Time Periods",
            "Branching Paths",
            "Segments",
            "Reverse Branching",
            "Wild",
            "Shadow"});
			this.modeSelector.Location = new System.Drawing.Point(49, 32);
			this.modeSelector.Name = "modeSelector";
			this.modeSelector.Size = new System.Drawing.Size(190, 21);
			this.modeSelector.TabIndex = 4;
			this.toolTip1.SetToolTip(this.modeSelector, "If you don\'t know what this is, please read the mod\'s description.");
			this.modeSelector.SelectedIndexChanged += new System.EventHandler(this.modeSelector_SelectedIndexChanged);
			// 
			// separateSoundtracks
			// 
			this.separateSoundtracks.AutoSize = true;
			this.separateSoundtracks.Location = new System.Drawing.Point(124, 6);
			this.separateSoundtracks.Name = "separateSoundtracks";
			this.separateSoundtracks.Size = new System.Drawing.Size(135, 17);
			this.separateSoundtracks.TabIndex = 8;
			this.separateSoundtracks.Text = "Separate JP/US Music";
			this.toolTip1.SetToolTip(this.separateSoundtracks, "If this is checked, the JP and US soundtracks will be randomized separately. Past" +
        " music will be included in both pools.");
			this.separateSoundtracks.UseVisualStyleBackColor = true;
			// 
			// saveLogButton
			// 
			this.saveLogButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.saveLogButton.AutoSize = true;
			this.saveLogButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.saveLogButton.Enabled = false;
			this.saveLogButton.Location = new System.Drawing.Point(417, 163);
			this.saveLogButton.Name = "saveLogButton";
			this.saveLogButton.Size = new System.Drawing.Size(72, 23);
			this.saveLogButton.TabIndex = 2;
			this.saveLogButton.Text = "Save Log...";
			this.toolTip1.SetToolTip(this.saveLogButton, "Click this button to generate a text log containing your settings and the level p" +
        "rogression.");
			this.saveLogButton.UseVisualStyleBackColor = true;
			this.saveLogButton.Click += new System.EventHandler(this.saveLogButton_Click);
			// 
			// makeChartButton
			// 
			this.makeChartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.makeChartButton.AutoSize = true;
			this.makeChartButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.makeChartButton.Enabled = false;
			this.makeChartButton.Location = new System.Drawing.Point(330, 163);
			this.makeChartButton.Name = "makeChartButton";
			this.makeChartButton.Size = new System.Drawing.Size(81, 23);
			this.makeChartButton.TabIndex = 3;
			this.makeChartButton.Text = "Make Chart...";
			this.toolTip1.SetToolTip(this.makeChartButton, "Click this button to generate an image showing the progression between levels.");
			this.makeChartButton.UseVisualStyleBackColor = true;
			this.makeChartButton.Click += new System.EventHandler(this.makeChartButton_Click);
			// 
			// randomItemMode
			// 
			this.randomItemMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.randomItemMode.FormattingEnabled = true;
			this.randomItemMode.Items.AddRange(new object[] {
            "Off",
            "One to One",
            "One to One Per Stage",
            "Wild"});
			this.randomItemMode.Location = new System.Drawing.Point(103, 29);
			this.randomItemMode.Name = "randomItemMode";
			this.randomItemMode.Size = new System.Drawing.Size(201, 21);
			this.randomItemMode.TabIndex = 9;
			this.toolTip1.SetToolTip(this.randomItemMode, "One to One: The types of items will be shuffled between each other.\r\nOne to One P" +
        "er Stage: Same as One to One, but reshuffled each stage.\r\nWild: Each monitor has" +
        " a random item.");
			// 
			// randomTimePosts
			// 
			this.randomTimePosts.AutoSize = true;
			this.randomTimePosts.Location = new System.Drawing.Point(6, 56);
			this.randomTimePosts.Name = "randomTimePosts";
			this.randomTimePosts.Size = new System.Drawing.Size(134, 17);
			this.randomTimePosts.TabIndex = 11;
			this.randomTimePosts.Text = "Randomize Time Posts";
			this.toolTip1.SetToolTip(this.randomTimePosts, "Time Posts will randomly be set to Past or Future.");
			this.randomTimePosts.UseVisualStyleBackColor = true;
			// 
			// replaceCheckpoints
			// 
			this.replaceCheckpoints.AutoSize = true;
			this.replaceCheckpoints.Location = new System.Drawing.Point(146, 56);
			this.replaceCheckpoints.Name = "replaceCheckpoints";
			this.replaceCheckpoints.Size = new System.Drawing.Size(208, 17);
			this.replaceCheckpoints.TabIndex = 12;
			this.replaceCheckpoints.Text = "Replace Checkpoints With Time Posts";
			this.toolTip1.SetToolTip(this.replaceCheckpoints, "Replaces all checkpoints with a time post set to a random destination.");
			this.replaceCheckpoints.UseVisualStyleBackColor = true;
			// 
			// randomPalettes
			// 
			this.randomPalettes.AutoSize = true;
			this.randomPalettes.Location = new System.Drawing.Point(6, 79);
			this.randomPalettes.Name = "randomPalettes";
			this.randomPalettes.Size = new System.Drawing.Size(120, 17);
			this.randomPalettes.TabIndex = 13;
			this.randomPalettes.Text = "Randomize Palettes";
			this.toolTip1.SetToolTip(this.randomPalettes, "Randomly hue-shifts all the game\'s palettes.");
			this.randomPalettes.UseVisualStyleBackColor = true;
			// 
			// ufoDifficulty
			// 
			this.ufoDifficulty.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ufoDifficulty.FormattingEnabled = true;
			this.ufoDifficulty.Items.AddRange(new object[] {
            "Easy",
            "Medium",
            "Hard",
            "Wild"});
			this.ufoDifficulty.Location = new System.Drawing.Point(177, 100);
			this.ufoDifficulty.Name = "ufoDifficulty";
			this.ufoDifficulty.Size = new System.Drawing.Size(121, 21);
			this.ufoDifficulty.TabIndex = 16;
			this.toolTip1.SetToolTip(this.ufoDifficulty, "Sets the difficulty level for the Special Stages.");
			// 
			// randomUFOs
			// 
			this.randomUFOs.AutoSize = true;
			this.randomUFOs.Location = new System.Drawing.Point(6, 102);
			this.randomUFOs.Name = "randomUFOs";
			this.randomUFOs.Size = new System.Drawing.Size(109, 17);
			this.randomUFOs.TabIndex = 14;
			this.randomUFOs.Text = "Randomize UFOs";
			this.toolTip1.SetToolTip(this.randomUFOs, "The number and placements of UFOs in the Special Stages will be randomized.");
			this.randomUFOs.UseVisualStyleBackColor = true;
			// 
			// randomWater
			// 
			this.randomWater.AutoSize = true;
			this.randomWater.Location = new System.Drawing.Point(6, 127);
			this.randomWater.Name = "randomWater";
			this.randomWater.Size = new System.Drawing.Size(111, 17);
			this.randomWater.TabIndex = 17;
			this.randomWater.Text = "Randomize Water";
			this.toolTip1.SetToolTip(this.randomWater, "Water will be randomly added, removed, or adjusted in levels.\r\nMay make the game " +
        "unwinnable.");
			this.randomWater.UseVisualStyleBackColor = true;
			// 
			// addWaterOnly
			// 
			this.addWaterOnly.AutoSize = true;
			this.addWaterOnly.Location = new System.Drawing.Point(123, 127);
			this.addWaterOnly.Name = "addWaterOnly";
			this.addWaterOnly.Size = new System.Drawing.Size(101, 17);
			this.addWaterOnly.TabIndex = 18;
			this.addWaterOnly.Text = "Only Add Water";
			this.toolTip1.SetToolTip(this.addWaterOnly, "If checked, water will only be added to levels that didn\'t have it.\r\nIt will not " +
        "be altered in levels that already had water.");
			this.addWaterOnly.UseVisualStyleBackColor = true;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(503, 218);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.modeSelector);
			this.tabPage1.Controls.Add(this.label7);
			this.tabPage1.Controls.Add(this.panel1);
			this.tabPage1.Controls.Add(label1);
			this.tabPage1.Controls.Add(this.seedSelector);
			this.tabPage1.Controls.Add(this.randomSeed);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(495, 192);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Level Order";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 35);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(37, 13);
			this.label7.TabIndex = 3;
			this.label7.Text = "Mode:";
			// 
			// panel1
			// 
			this.panel1.AutoSize = true;
			this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.panel1.Controls.Add(label2);
			this.panel1.Controls.Add(this.allowSameLevel);
			this.panel1.Controls.Add(label3);
			this.panel1.Controls.Add(this.backJumpProb);
			this.panel1.Controls.Add(this.mainPathSelector);
			this.panel1.Controls.Add(label6);
			this.panel1.Controls.Add(this.maxBackJump);
			this.panel1.Controls.Add(label4);
			this.panel1.Controls.Add(this.maxForwJump);
			this.panel1.Controls.Add(this.presetSelector);
			this.panel1.Controls.Add(label5);
			this.panel1.Location = new System.Drawing.Point(3, 56);
			this.panel1.Margin = new System.Windows.Forms.Padding(0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(360, 132);
			this.panel1.TabIndex = 5;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.addWaterOnly);
			this.tabPage3.Controls.Add(this.randomWater);
			this.tabPage3.Controls.Add(this.ufoDifficulty);
			this.tabPage3.Controls.Add(this.label9);
			this.tabPage3.Controls.Add(this.randomUFOs);
			this.tabPage3.Controls.Add(this.randomPalettes);
			this.tabPage3.Controls.Add(this.replaceCheckpoints);
			this.tabPage3.Controls.Add(this.randomTimePosts);
			this.tabPage3.Controls.Add(this.label8);
			this.tabPage3.Controls.Add(this.randomItemMode);
			this.tabPage3.Controls.Add(this.separateSoundtracks);
			this.tabPage3.Controls.Add(this.randomMusic);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(495, 192);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Misc";
			this.tabPage3.UseVisualStyleBackColor = true;
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(121, 103);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(50, 13);
			this.label9.TabIndex = 15;
			this.label9.Text = "Difficulty:";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(6, 32);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(91, 13);
			this.label8.TabIndex = 10;
			this.label8.Text = "Randomize Items:";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.makeChartButton);
			this.tabPage2.Controls.Add(this.saveLogButton);
			this.tabPage2.Controls.Add(this.spoilerLevelInfo);
			this.tabPage2.Controls.Add(this.spoilerLevelList);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(495, 192);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Spoilers";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// spoilerLevelInfo
			// 
			this.spoilerLevelInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.spoilerLevelInfo.Location = new System.Drawing.Point(225, 6);
			this.spoilerLevelInfo.Multiline = true;
			this.spoilerLevelInfo.Name = "spoilerLevelInfo";
			this.spoilerLevelInfo.ReadOnly = true;
			this.spoilerLevelInfo.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
			this.spoilerLevelInfo.Size = new System.Drawing.Size(264, 151);
			this.spoilerLevelInfo.TabIndex = 1;
			this.spoilerLevelInfo.WordWrap = false;
			// 
			// spoilerLevelList
			// 
			this.spoilerLevelList.Enabled = false;
			this.spoilerLevelList.FormattingEnabled = true;
			this.spoilerLevelList.Location = new System.Drawing.Point(6, 6);
			this.spoilerLevelList.Name = "spoilerLevelList";
			this.spoilerLevelList.Size = new System.Drawing.Size(213, 173);
			this.spoilerLevelList.TabIndex = 0;
			this.spoilerLevelList.SelectedIndexChanged += new System.EventHandler(this.spoilerLevelList_SelectedIndexChanged);
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.DefaultExt = "txt";
			this.saveFileDialog1.Filter = "Text Files|*.txt;*.log";
			this.saveFileDialog1.RestoreDirectory = true;
			// 
			// saveFileDialog2
			// 
			this.saveFileDialog2.DefaultExt = "png";
			this.saveFileDialog2.Filter = "PNG Files|*.png";
			// 
			// MainForm
			// 
			this.AcceptButton = this.randomizeButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(503, 259);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.randomizeButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "Sonic CD 2011 Randomizer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.Load += new System.EventHandler(this.MainForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.seedSelector)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maxBackJump)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.maxForwJump)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.backJumpProb)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.tabPage3.ResumeLayout(false);
			this.tabPage3.PerformLayout();
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
		private System.Windows.Forms.ComboBox presetSelector;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Button randomizeButton;
		private System.Windows.Forms.ListBox spoilerLevelList;
		private System.Windows.Forms.TextBox spoilerLevelInfo;
		private System.Windows.Forms.NumericUpDown backJumpProb;
		private System.Windows.Forms.CheckBox allowSameLevel;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.ComboBox modeSelector;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox separateSoundtracks;
		private System.Windows.Forms.Button saveLogButton;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.Button makeChartButton;
		private System.Windows.Forms.SaveFileDialog saveFileDialog2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox randomItemMode;
		private System.Windows.Forms.CheckBox randomTimePosts;
		private System.Windows.Forms.CheckBox replaceCheckpoints;
		private System.Windows.Forms.CheckBox randomPalettes;
		private System.Windows.Forms.CheckBox randomUFOs;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.ComboBox ufoDifficulty;
		private System.Windows.Forms.CheckBox randomWater;
		private System.Windows.Forms.CheckBox addWaterOnly;
	}
}