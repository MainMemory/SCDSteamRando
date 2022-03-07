using IniFile;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SCDSteamRando
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		Settings settings;

		private void MainForm_Load(object sender, EventArgs e)
		{
			if (!Directory.Exists("Scripts"))
			{
				MessageBox.Show(this, "Decompiled scripts not found. You must have the scripts installed for randomized music to work.\n\nIf you want to have randomized music, please download the scripts and place them in the \"Scripts\" folder.", "Scripts Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				randomMusic.Enabled = false;
				separateSoundtracks.Enabled = false;
				randomUFOs.Enabled = false;
				ufoDifficulty.Enabled = false;
			}
			settings = Settings.Load();
			seedSelector.Value = settings.Seed;
			randomSeed.Checked = settings.RandomSeed;
			modeSelector.SelectedIndex = (int)settings.Mode;
			mainPathSelector.SelectedIndex = (int)settings.MainPath;
			maxBackJump.Value = settings.MaxBackJump;
			maxForwJump.Value = settings.MaxForwJump;
			backJumpProb.Value = settings.BackJumpProb;
			allowSameLevel.Checked = settings.AllowSameLevel;
			if (randomMusic.Enabled)
				randomMusic.Checked = settings.RandomMusic;
			separateSoundtracks.Checked = settings.SeparateSoundtracks;
			randomItemMode.SelectedIndex = (int)settings.RandomItems;
			randomTimePosts.Checked = settings.RandomTimePosts;
			replaceCheckpoints.Checked = settings.ReplaceCheckpoints;
			randomPalettes.Checked = settings.RandomPalettes;
			if (randomUFOs.Enabled)
				randomUFOs.Checked = settings.RandomUFOs;
			ufoDifficulty.SelectedIndex = (int)settings.UFODifficulty;
			randomWater.Checked = settings.RandomWater;
			addWaterOnly.Checked = settings.AddWaterOnly;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			settings.Seed = (int)seedSelector.Value;
			settings.RandomSeed = randomSeed.Checked;
			settings.Mode = (Modes)modeSelector.SelectedIndex;
			settings.MainPath = (MainPath)mainPathSelector.SelectedIndex;
			settings.MaxBackJump = (int)maxBackJump.Value;
			settings.MaxForwJump = (int)maxForwJump.Value;
			settings.BackJumpProb = (int)backJumpProb.Value;
			settings.AllowSameLevel = allowSameLevel.Checked;
			settings.RandomMusic = randomMusic.Checked;
			settings.SeparateSoundtracks = separateSoundtracks.Checked;
			settings.RandomItems = (ItemMode)randomItemMode.SelectedIndex;
			settings.RandomTimePosts = randomTimePosts.Checked;
			settings.ReplaceCheckpoints = replaceCheckpoints.Checked;
			settings.RandomPalettes = randomPalettes.Checked;
			settings.RandomUFOs = randomUFOs.Checked;
			settings.UFODifficulty = (UFODifficulty)ufoDifficulty.SelectedIndex;
			settings.RandomWater = randomWater.Checked;
			settings.AddWaterOnly = addWaterOnly.Checked;
			settings.Save();
		}

		private void randomSeed_CheckedChanged(object sender, EventArgs e)
		{
			seedSelector.Enabled = !randomSeed.Checked;
		}

		private void modeSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			panel1.Enabled = modeSelector.SelectedIndex == 0;
		}

		private void allowSameLevel_CheckedChanged(object sender, EventArgs e)
		{
			maxBackJump.Minimum = maxForwJump.Minimum = allowSameLevel.Checked ? 0 : 1;
		}

		private const int stagecount = 70;
		readonly (int b, int f, int p)[] presets =
		{
			(1, stagecount, 0), // Easiest
			(5, 60, 5), // Very Easy
			(10, 50, 10), // Easy
			(15, 40, 15), // Normal
			(30, 30, 40), // Hard
			(45, 15, 70), // Very Hard
			(69, 1, 100), // Insane
			(1, 1, 0), // All Levels
			(69, stagecount, 50) // Full Random
		};
		private void presetSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (presetSelector.SelectedIndex == -1) return;
			(int b, int f, int p) = presets[presetSelector.SelectedIndex];
			maxBackJump.Value = b;
			maxForwJump.Value = f;
			backJumpProb.Value = p;
		}

		static readonly string[] RoundNames =
		{
			"Palmtree Panic",
			"Collision Chaos",
			"Tidal Tempest",
			"Quartz Quadrant",
			"Wacky Workbench",
			"Stardust Speedway",
			"Metallic Madness"
		};

		static readonly string[] ZoneNames =
		{
			"Zone 1 Present",
			"Zone 1 Past",
			"Zone 1 Good Future",
			"Zone 1 Bad Future",
			"Zone 2 Present",
			"Zone 2 Past",
			"Zone 2 Good Future",
			"Zone 2 Bad Future",
			"Zone 3 Good Future",
			"Zone 3 Bad Future"
		};

		static readonly Regex musregex = new Regex(@"SetMusicTrack\((.+),([0-9]+),([0-9]+)\)", RegexOptions.Compiled);
		static readonly Regex ssboundregex = new Regex(@"Stage\.([XY]Boundary[12])=([0-9]+)", RegexOptions.Compiled);
		readonly int[] stageids = new int[stagecount + 1];
		readonly Stage[] stages = new Stage[stagecount];
		private void randomizeButton_Click(object sender, EventArgs e)
		{
			int seed;
			if (randomSeed.Checked)
			{
				seed = (int)DateTime.Now.Ticks;
				seedSelector.Value = seed;
			}
			else
				seed = (int)seedSelector.Value;
			string path = Directory.CreateDirectory(@"mods\Rando").FullName;
			File.WriteAllText(Path.Combine(path, "mod.ini"), Properties.Resources.mod_ini);
			if (Directory.Exists(Path.Combine(path, "Data")))
				Directory.Delete(Path.Combine(path, "Data"), true);
			VirtualDirectory vdir = new VirtualDirectory("Data");
			if (Directory.Exists("Scripts"))
				vdir.AddDirectory("Scripts").ScanDirectory("Scripts");
			RSDKv3.DataFile dataFile = null;
			if (File.Exists("Data.rsdk"))
			{
				dataFile = new RSDKv3.DataFile("Data.rsdk");
				foreach (var item in dataFile.files)
					vdir.AddFile(item.fullFilename.Substring(item.fullFilename.IndexOf('/') + 1), "Data.rsdk");
			}
			else if (Directory.Exists("Data"))
				vdir.ScanDirectory("Data");
			string[] mods = null;
			if (File.Exists(@"mods\SCDSteamModLoader.ini"))
			{
				ModLoaderInfo info = IniSerializer.Deserialize<ModLoaderInfo>(@"mods\SCDSteamModLoader.ini");
				if (info.Mods != null)
					mods = info.Mods.Select(a => Path.Combine(Directory.GetCurrentDirectory(), "mods", a, "Data")).Where(a => Directory.Exists(a)).ToArray();
			}
			else if (File.Exists(@"mods\modconfig.ini"))
			{
				DecompModInfo info = IniSerializer.Deserialize<DecompModInfo>(@"mods\modconfig.ini");
				if (info.Mods?.Mods != null)
					mods = info.Mods.Mods.Where(a => a.Value).Select(a => Path.Combine(Directory.GetCurrentDirectory(), "mods", a.Key, "Data")).Where(a => Directory.Exists(a)).ToArray();
			}
			if (mods != null)
				foreach (string mod in mods)
					vdir.ScanDirectory(mod);
			VirtualFile vfile = vdir.GetFile("Game/GameConfig.bin");
			RSDKv3.GameConfig gc;
			if (vfile.SourcePath == "Data.rsdk")
				using (MemoryStream ms = new MemoryStream(dataFile.files.Single(a => a.fullFilename == vfile.FullName).fileData))
					gc = new RSDKv3.GameConfig(ms);
			else
				gc = new RSDKv3.GameConfig(vfile.SourcePath);
			Directory.CreateDirectory(Path.Combine(path, @"Data\Palettes"));
			Directory.CreateDirectory(Path.Combine(path, @"Data\Scripts\Credits"));
			Directory.CreateDirectory(Path.Combine(path, @"Data\Scripts\Global"));
			Directory.CreateDirectory(Path.Combine(path, @"Data\Scripts\Menu"));
			Directory.CreateDirectory(Path.Combine(path, @"Data\Scripts\R8"));
			Directory.CreateDirectory(Path.Combine(path, @"Data\Scripts\Secrets"));
			Directory.CreateDirectory(Path.Combine(path, @"Data\Sprites\Secrets"));
			Random r = new Random(seed);
			for (int i = 0; i < stagecount; i++)
			{
				stageids[i] = i;
				stages[i] = new Stage(i);
			}
			stageids[stagecount] = stagecount;
			settings.Mode = (Modes)modeSelector.SelectedIndex;
			if (randomTimePosts.Checked || replaceCheckpoints.Checked)
			{
				int lamppostid = gc.objects.FindIndex(a => a.name == "LampPost") + 1;
				int pastpostid = gc.objects.FindIndex(a => a.name == "Past Post") + 1;
				int futurepostid = gc.objects.FindIndex(a => a.name == "Future Post") + 1;
				var regstg = gc.stageLists[1].list;
				bool allstg = false;
				if (randomTimePosts.Checked)
					switch (settings.Mode)
					{
						case Modes.AllStagesWarps:
						case Modes.BranchingPaths:
						case Modes.Segments:
						case Modes.Wild:
							allstg = true;
							break;
					}
				for (int i = 0; i < stagecount; i++)
				{
					if (i % 10 > 7) continue;
					vfile = vdir.GetFile($"Stages/{regstg[i].folder}/Act{regstg[i].id}.bin");
					RSDKv3.Scene scn;
					if (vfile.SourcePath == "Data.rsdk")
						using (MemoryStream ms = new MemoryStream(dataFile.files.Single(a => a.fullFilename == vfile.FullName).fileData))
							scn = new RSDKv3.Scene(ms);
					else
						scn = new RSDKv3.Scene(vfile.SourcePath);
					bool haspast = scn.entities.Any(a => a.type == pastpostid);
					bool hasfuture = scn.entities.Any(a => a.type == futurepostid);
					if ((haspast && hasfuture) || allstg)
					{
						var posts = scn.entities.Where(a => a.type == pastpostid || a.type == futurepostid).ToList();
						if (replaceCheckpoints.Checked)
							posts.AddRange(scn.entities.Where(a => a.type == lamppostid));
						if (posts.Count > 1 || allstg)
						{
							haspast = false;
							hasfuture = false;
							foreach (var item in posts)
							{
								if (item.type == lamppostid)
									item.propertyValue = 0;
								item.type = (byte)(r.Next(2) == 0 ? pastpostid : futurepostid);
								if (item.type == pastpostid)
									haspast = true;
								else
									hasfuture = true;
							}
							if (!allstg)
							{
								if (!haspast)
								{
									posts[r.Next(posts.Count)].type = (byte)pastpostid;
									haspast = true;
								}
								if (!hasfuture)
								{
									posts[r.Next(posts.Count)].type = (byte)futurepostid;
									hasfuture = true;
								}
							}
							stages[i].HasPastPost = haspast;
							stages[i].HasFuturePost = hasfuture;
							vfile.SourcePath = Path.Combine(path, vfile.FullName);
							Directory.CreateDirectory(Path.GetDirectoryName(vfile.SourcePath));
							scn.write(vfile.SourcePath);
						}
					}
				}
			}
			bool timewarp = true;
			switch (settings.Mode)
			{
				case Modes.AllStagesWarps:
					{
						Shuffle(r, stageids, stagecount);
						switch ((MainPath)mainPathSelector.SelectedIndex)
						{
							case MainPath.ActClear:
								for (int i = 0; i < stagecount; i++)
									stages[stageids[i]].Clear = stageids[i + 1];
								break;
							case MainPath.TimeTravel:
								for (int i = 0; i < stagecount; i++)
								{
									int next = stageids[i + 1];
									if (stages[stageids[i]].HasPastPost && stages[stageids[i]].HasFuturePost)
										switch (r.Next(2))
										{
											case 0:
												stages[stageids[i]].Past = next;
												break;
											case 1:
												stages[stageids[i]].Future = next;
												break;
										}
									else if (stages[stageids[i]].HasPastPost)
										stages[stageids[i]].Past = next;
									else if (stages[stageids[i]].HasFuturePost)
										stages[stageids[i]].Future = next;
									else
										stages[stageids[i]].Clear = next;
								}
								break;
							case MainPath.AnyExit:
								for (int i = 0; i < stagecount; i++)
								{
									int next = stageids[i + 1];
									if (stages[stageids[i]].HasPastPost && stages[stageids[i]].HasFuturePost)
										switch (r.Next(3))
										{
											case 0:
												stages[stageids[i]].Clear = next;
												break;
											case 1:
												stages[stageids[i]].Past = next;
												break;
											case 2:
												stages[stageids[i]].Future = next;
												break;
										}
									else if (stages[stageids[i]].HasPastPost)
										switch (r.Next(2))
										{
											case 0:
												stages[stageids[i]].Clear = next;
												break;
											case 1:
												stages[stageids[i]].Past = next;
												break;
										}
									else if (stages[stageids[i]].HasFuturePost)
										switch (r.Next(2))
										{
											case 0:
												stages[stageids[i]].Clear = next;
												break;
											case 1:
												stages[stageids[i]].Future = next;
												break;
										}
									else
										stages[stageids[i]].Clear = next;
								}
								break;
						}
						for (int i = 0; i < stagecount; i++)
						{
							Stage stg = stages[stageids[i]];
							int min, max;
							if (stg.Clear == -1)
							{
								if (r.Next(100) < backJumpProb.Value && (i > 0 || backJumpProb.Value == 100))
								{
									min = Math.Max(i - (int)maxBackJump.Value, 0);
									max = Math.Max(i - (int)maxBackJump.Minimum + 1, 0);
								}
								else
								{
									min = i + (int)maxForwJump.Minimum;
									max = Math.Min(i + (int)maxForwJump.Value + 1, stagecount + 1);
								}
								stg.Clear = stageids[r.Next(min, max)];
							}
							if (stg.HasPastPost && stg.Past == -1)
							{
								if (r.Next(100) < backJumpProb.Value && (i > 0 || backJumpProb.Value == 100))
								{
									min = Math.Max(i - (int)maxBackJump.Value, 0);
									max = Math.Max(i - (int)maxBackJump.Minimum + 1, 0);
								}
								else
								{
									min = i + (int)maxForwJump.Minimum;
									max = Math.Min(i + (int)maxForwJump.Value + 1, stagecount + 1);
								}
								stg.Past = stageids[r.Next(min, max)];
							}
							if (stg.HasFuturePost && stg.Future == -1)
							{
								if (r.Next(100) < backJumpProb.Value && (i > 0 || backJumpProb.Value == 100))
								{
									min = Math.Max(i - (int)maxBackJump.Value, 0);
									max = Math.Max(i - (int)maxBackJump.Minimum + 1, 0);
								}
								else
								{
									min = i + (int)maxForwJump.Minimum;
									max = Math.Min(i + (int)maxForwJump.Value + 1, stagecount + 1);
								}
								stg.Future = stageids[r.Next(min, max)];
							}
						}
					}
					break;
				case Modes.Rounds:
					{
						for (int i = 0; i < stagecount; i++)
						{
							Stage stg = stages[i];
							switch (i % 10)
							{
								case 0:
									stg.Clear = i + 4;
									stg.Past = i + 1;
									stg.Future = i + 3;
									stg.GoodFuture = i + 2;
									break;
								case 1:
									stg.Clear = i + 3;
									stg.Future = i - 1;
									break;
								case 2:
									stg.Clear = i + 2;
									stg.Past = i - 2;
									break;
								case 3:
									stg.Clear = i + 1;
									stg.Past = i - 3;
									break;
								case 4:
									stg.Clear = i + 5;
									stg.ClearGF = i + 4;
									stg.Past = i + 1;
									stg.Future = i + 3;
									stg.GoodFuture = i + 2;
									break;
								case 5:
									stg.Clear = i + 4;
									stg.ClearGF = i + 3;
									stg.Future = i - 1;
									break;
								case 6:
									stg.Clear = i + 3;
									stg.ClearGF = i + 2;
									stg.Past = i - 2;
									break;
								case 7:
									stg.Clear = i + 2;
									stg.Past = i - 3;
									break;
							}
						}
						int[] rounds = new int[8];
						Shuffle(r, rounds, 7);
						rounds[7] = 7;
						for (int i = 0; i < 7; i++)
						{
							for (int j = 0; j < 10; j++)
								stageids[(i * 10) + j] = (rounds[i] * 10) + j;
							stages[(rounds[i] * 10) + 8].Clear = rounds[i + 1] * 10;
							stages[(rounds[i] * 10) + 9].Clear = rounds[i + 1] * 10;
						}
						timewarp = false;
					}
					break;
				case Modes.Acts:
					{
						for (int i = 0; i < stagecount; i++)
						{
							Stage stg = stages[i];
							if (stg.Act < 2)
								switch (stg.TimePeriod)
								{
									case TimePeriods.Present:
										stg.Past = i + 1;
										stg.Future = i + 3;
										stg.GoodFuture = i + 2;
										break;
									case TimePeriods.Past:
										stg.Future = i - 1;
										break;
									case TimePeriods.GoodFuture:
										stg.Past = i - 2;
										break;
									case TimePeriods.BadFuture:
										stg.Past = i - 3;
										break;
								}
						}
						int[] rounds = new int[7];
						int[] order = new int[7];
						for (int i = 0; i < 7; i++)
						{
							rounds[i] = i;
							order[i] = r.Next();
						}
						Array.Sort(order, rounds);
						for (int i = 0; i < 7; i++)
						{
							for (int j = 0; j < 4; j++)
								stageids[(i * 10) + j] = (rounds[i] * 10) + j;
							order[i] = r.Next();
						}
						Array.Sort(order, rounds);
						for (int i = 0; i < 7; i++)
						{
							for (int j = 0; j < 4; j++)
								stageids[(i * 10) + j + 4] = (rounds[i] * 10) + j + 4;
							order[i] = r.Next();
						}
						Array.Sort(order, rounds);
						for (int i = 0; i < 7; i++)
							for (int j = 0; j < 2; j++)
								stageids[(i * 10) + j + 8] = (rounds[i] * 10) + j + 8;
						for (int i = 0; i < stagecount - 2; i++)
						{
							Stage stg = stages[stageids[i]];
							switch (i % 10)
							{
								case 0:
									stg.Clear = stageids[i + 4];
									break;
								case 1:
									stg.Clear = stageids[i + 3];
									break;
								case 2:
									stg.Clear = stageids[i + 2];
									break;
								case 3:
									stg.Clear = stageids[i + 1];
									break;
								case 4:
									stg.Clear = stageids[i + 5];
									stg.ClearGF = stageids[i + 4];
									break;
								case 5:
									stg.Clear = stageids[i + 4];
									stg.ClearGF = stageids[i + 3];
									break;
								case 6:
									stg.Clear = stageids[i + 3];
									stg.ClearGF = stageids[i + 2];
									break;
								case 7:
									stg.Clear = stageids[i + 2];
									break;
								case 8:
									stg.Clear = stageids[i + 2];
									break;
								case 9:
									stg.Clear = stageids[i + 1];
									break;
							}
						}
						stages[stageids[stagecount - 2]].Clear = stagecount;
						stages[stageids[stagecount - 1]].Clear = stagecount;
						timewarp = false;
					}
					break;
				case Modes.TimePeriods:
					{
						int[] rounds = new int[7];
						for (int i = 0; i < 7; i++)
							rounds[i] = i;
						int[] order = new int[7];
						for (int i = 0; i < 10; i++)
						{
							for (int j = 0; j < 7; j++)
								order[j] = r.Next();
							Array.Sort(order, rounds);
							for (int j = 0; j < 7; j++)
								stageids[(j * 10) + i] = (rounds[j] * 10) + i;
						}
						for (int i = 0; i < stagecount - 2; i++)
						{
							Stage stg = stages[stageids[i]];
							switch (i % 10)
							{
								case 0:
									stg.Clear = stageids[i + 4];
									stg.Past = stageids[i + 1];
									stg.Future = stageids[i + 3];
									stg.GoodFuture = stageids[i + 2];
									break;
								case 1:
									stg.Clear = stageids[i + 3];
									stg.Future = stageids[i - 1];
									break;
								case 2:
									stg.Clear = stageids[i + 2];
									stg.Past = stageids[i - 2];
									break;
								case 3:
									stg.Clear = stageids[i + 1];
									stg.Past = stageids[i - 3];
									break;
								case 4:
									stg.Clear = stageids[i + 5];
									stg.ClearGF = stageids[i + 4];
									stg.Past = stageids[i + 1];
									stg.Future = stageids[i + 3];
									stg.GoodFuture = stageids[i + 2];
									break;
								case 5:
									stg.Clear = stageids[i + 4];
									stg.ClearGF = stageids[i + 3];
									stg.Future = stageids[i - 1];
									break;
								case 6:
									stg.Clear = stageids[i + 3];
									stg.ClearGF = stageids[i + 2];
									stg.Past = stageids[i - 2];
									break;
								case 7:
									stg.Clear = stageids[i + 2];
									stg.Past = stageids[i - 3];
									break;
								case 8:
									stg.Clear = stageids[i + 2];
									break;
								case 9:
									stg.Clear = stageids[i + 1];
									break;
							}
						}
						stages[stageids[stagecount - 2]].Clear = stagecount;
						stages[stageids[stagecount - 1]].Clear = stagecount;
					}
					break;
				case Modes.BranchingPaths:
					{
						List<int> stagepool = new List<int>(stageids.Take(stagecount));
						List<int> curset = new List<int>() { r.Next(stagecount) };
						stagepool.Remove(curset[0]);
						List<int> ids2 = new List<int>() { curset[0] };
						while (stagepool.Count > 0)
						{
							List<int> newset = new List<int>();
							for (int i = 0; i < curset.Count; i++)
							{
								Stage stg = stages[curset[i]];
								stg.Clear = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
								if (!newset.Contains(stg.Clear))
									newset.Add(stg.Clear);
								if (stg.HasPastPost)
								{
									stg.Past = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
									if (!newset.Contains(stg.Past))
										newset.Add(stg.Past);
								}
								if (stg.HasFuturePost)
								{
									stg.Future = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
									if (!newset.Contains(stg.Future))
										newset.Add(stg.Future);
								}
							}
							stagepool.RemoveAll(a => newset.Contains(a));
							curset = newset;
							ids2.AddRange(newset);
						}
						foreach (int item in curset)
							stages[item].Clear = stagecount;
						ids2.CopyTo(stageids);
					}
					break;
				case Modes.Segments:
					{
						int[] regstg = new int[stagecount - (stagecount / 10 * 2)];
						int[] bossstg = new int[stagecount / 10 * 2];
						int[] regord = new int[regstg.Length];
						int[] bossord = new int[bossstg.Length];
						for (int i = 0; i < stagecount / 10; i++)
						{
							for (int j = 0; j < 8; j++)
							{
								regstg[i * 8 + j] = i * 10 + j;
								regord[i * 8 + j] = r.Next();
							}
							for (int j = 0; j < 2; j++)
							{
								bossstg[i * 2 + j] = i * 10 + j + 8;
								bossord[i * 2 + j] = r.Next();
							}
						}
						Array.Sort(regord, regstg);
						Array.Sort(bossord, bossstg);
						for (int i = 0; i < stagecount / 10; i++)
						{
							for (int j = 0; j < 8; j++)
								stageids[i * 10 + j] = regstg[i * 8 + j];
							for (int j = 0; j < 2; j++)
								stageids[i * 10 + j + 8] = bossstg[i * 2 + j];
						}
						int segbase = 0;
						for (int i = 0; i < stagecount; i++)
						{
							Stage stg = stages[stageids[i]];
							if (i % 10 == 0)
								segbase = i;
							int next;
							if (i >= stagecount - 2)
								next = stagecount;
							else if (i % 10 == 8 || i % 10 == 7)
								next = stageids[i + 2];
							else
								next = stageids[i + 1];
							stages[stageids[i]].Clear = next;
							if (i % 10 == 7)
								stages[stageids[i]].ClearGF = stageids[segbase + 8];
							if (stg.HasPastPost)
								stg.Past = stageids[r.Next(8) + segbase];
							if (stg.HasFuturePost)
								stg.Future = stageids[r.Next(8) + segbase];
						}
					}
					break;
				case Modes.Wild:
					{
						Queue<int> stgq = new Queue<int>();
						stgq.Enqueue(r.Next(stagecount));
						List<int> neword = new List<int>(stagecount);
						while (neword.Count < stagecount)
						{
							if (stgq.Count == 0)
							{
								foreach (var id in stageids.Except(neword))
									if (id != stagecount)
										stgq.Enqueue(id);
							}
							int i = stgq.Dequeue();
							neword.Add(i);
							Stage stg = stages[stageids[i]];
							stg.Clear = r.Next(stagecount + 1);
							if (stg.Clear != stagecount && !neword.Contains(stg.Clear) && !stgq.Contains(stg.Clear))
								stgq.Enqueue(stg.Clear);
							if (stg.HasPastPost)
							{
								stg.Past = r.Next(stagecount + 1);
								if (stg.Past != stagecount && !neword.Contains(stg.Past) && !stgq.Contains(stg.Past))
									stgq.Enqueue(stg.Past);
							}
							if (stg.HasFuturePost)
							{
								stg.Future = r.Next(stagecount + 1);
								if (stg.Future != stagecount && !neword.Contains(stg.Future) && !stgq.Contains(stg.Future))
									stgq.Enqueue(stg.Future);
							}
						}
						neword.CopyTo(stageids);
					}
					break;
				case Modes.Shadow:
					{
						int[] present = new int[14];
						int[] past = new int[14];
						int[] goodfuture = new int[14];
						int[] badfuture = new int[14];
						int[] boss = new int[14];
						for (int i = 0; i < 7; i++)
						{
							present[i * 2] = i * 10;
							past[i * 2] = i * 10 + 1;
							goodfuture[i * 2] = i * 10 + 2;
							badfuture[i * 2] = i * 10 + 3;
							present[i * 2 + 1] = i * 10 + 4;
							past[i * 2 + 1] = i * 10 + 5;
							goodfuture[i * 2 + 1] = i * 10 + 6;
							badfuture[i * 2 + 1] = i * 10 + 7;
							boss[i * 2] = i * 10 + 8;
							boss[i * 2 + 1] = i * 10 + 9;
						}
						Shuffle(r, present);
						Shuffle(r, past);
						Shuffle(r, goodfuture);
						Shuffle(r, badfuture);
						Shuffle(r, boss);
						Queue<int>[] queues = new Queue<int>[] { new Queue<int>(present), new Queue<int>(past), new Queue<int>(goodfuture), new Queue<int>(badfuture) };
						List<int> neword = new List<int>(stagecount);
						foreach (var set in ShadowStageSet.StageList)
						{
							foreach (var stg in set.stages)
								neword.Add(queues[(int)stg.timePeriod].Dequeue());
							foreach (var stg in set.bosses)
								neword.Add(boss[stg]);
						}
						int last = neword.Count;
						int[] rest = queues.SelectMany(a => a).ToArray();
						Shuffle(r, rest);
						neword.AddRange(rest);
						neword.Add(boss[12]);
						neword.Add(boss[13]);
						int ind = 0;
						foreach (var set in ShadowStageSet.StageList)
						{
							int next = set.stages.Count + set.bosses.Count;
							if (set.stages[0].timePeriod == TimePeriods.Present)
								++next;
							foreach (var item in set.stages)
							{
								Stage stg = stages[neword[ind]];
								if (item.boss2 != -1)
								{
									stg.Future = boss[item.boss];
									stg.Past = boss[item.boss2];
									stages[boss[item.boss]].Clear = neword[last];
									stages[boss[item.boss2]].Clear = neword[last];
								}
								else if (item.boss != -1)
								{
									Stage bossstg = stages[boss[item.boss]];
									bossstg.Clear = neword[ind + next];
									switch (item.timePeriod)
									{
										case TimePeriods.Present:
											bossstg.Future = neword[ind + next - 1];
											bossstg.Past = neword[ind + next + 1];
											break;
										case TimePeriods.Past:
										case TimePeriods.GoodFuture:
											bossstg.Future = neword[ind + next - 1];
											break;
										case TimePeriods.BadFuture:
											bossstg.Past = neword[ind + next + 1];
											break;
									}
									stg.Clear = boss[item.boss];
								}
								else
								{
									stg.Clear = neword[ind + next];
									switch (item.timePeriod)
									{
										case TimePeriods.Present:
											stg.Future = neword[ind + next - 1];
											stg.Past = neword[ind + next + 1];
											break;
										case TimePeriods.Past:
										case TimePeriods.GoodFuture:
											stg.Future = neword[ind + next - 1];
											break;
										case TimePeriods.BadFuture:
											stg.Past = neword[ind + next + 1];
											break;
									}
								}
								++ind;
							}
							ind += set.bosses.Count;
						}
						for (; ind < neword.Count - 2; ind++)
						{
							stages[neword[ind]].Clear = neword[ind + 1];
							stages[neword[ind]].Future = neword[Math.Min(ind + 2, neword.Count - 1)];
						}
						stages[boss[12]].Clear = boss[13];
						stages[boss[13]].Clear = stagecount;
						neword.CopyTo(stageids);
					}
					break;
			}
			string tmpstr = Properties.Resources.LoadSaveMenu_template;
			tmpstr = tmpstr.Replace("//REPLACE1", $"SaveRAM[ArrayPos1]={stageids[0] + 1}");
			tmpstr = tmpstr.Replace("//REPLACE2", $"Stage.ListPos={stageids[0]}");
			string newpath = Path.Combine(path, @"Data\Scripts\Menu\LoadSaveMenu.txt");
			File.WriteAllText(newpath, tmpstr);
			vdir.AddFile("Scripts/Menu/LoadSaveMenu.txt", newpath);
			newpath = Path.Combine(path, @"Data\Scripts\R8\Amy.txt");
			File.WriteAllText(newpath, Properties.Resources.Amy);
			vdir.AddFile("Scripts/R8/Amy.txt", newpath);
			if (vdir.FileExists("Scripts/Credits/CreditsControl.txt"))
				tmpstr = File.ReadAllText(vdir.GetFile("Scripts/Credits/CreditsControl.txt").SourcePath);
			else
				tmpstr = Properties.Resources.CreditsControl;
			newpath = Path.Combine(path, @"Data\Scripts\Credits\CreditsControl.txt");
			File.WriteAllText(newpath, tmpstr.Replace("Stage.ListPos=1", "Stage.ListPos=8"));
			vdir.AddFile("Scripts/Credits/CreditsControl.txt", newpath);
			if (vdir.FileExists("Scripts/Global/StageSetup.txt"))
				tmpstr = File.ReadAllText(vdir.GetFile("Scripts/Global/StageSetup.txt").SourcePath);
			else
				tmpstr = Properties.Resources.StageSetup;
			newpath = Path.Combine(path, @"Data\Scripts\Global\StageSetup.txt");
			File.WriteAllText(newpath, tmpstr.Replace("Transporter_Destroyed=1", "Transporter_Destroyed=0"));
			vdir.AddFile("Scripts/Global/StageSetup.txt", newpath);
			tmpstr = Properties.Resources.TailsUnlock_template;
			tmpstr = tmpstr.Replace("//REPLACE1", $"SaveRAM[ArrayPos1]={stageids[0] + 1}");
			tmpstr = tmpstr.Replace("//REPLACE2", $"Stage.ListPos={stageids[0]}");
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			int numopts = 10;
			if (settings.Mode == Modes.AllStagesWarps)
				numopts = 15;
			sb.AppendLine($"\tObject[32].Value6={(numopts + 6) / 7 - 1}");
			int optypos = 487;
			sb.AppendLine("\tTempValue4=100");
			sb.AppendLine("\t// Build");
			sb.AppendLine("\tArrayPos1++");
			sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],8,TempValue4,{optypos})");
			sb.AppendLine("\tObject[ArrayPos1].Priority=1");
			sb.AppendLine("\tObject[ArrayPos1].Value1=TempValue1");
			DateTime buildDate = File.GetLastWriteTimeUtc(Application.ExecutablePath);
			sb.AppendLine($"\tObject[ArrayPos1].Value2={buildDate.Year}");
			sb.AppendLine($"\tObject[ArrayPos1].Value3={buildDate.Month}");
			sb.AppendLine($"\tObject[ArrayPos1].Value4={buildDate.Day}");
			sb.AppendLine($"\tObject[ArrayPos1].Value5={buildDate.Hour}");
			sb.AppendLine($"\tObject[ArrayPos1].Value6={buildDate.Minute}");
			optypos += 38;
			sb.AppendLine("\t// Seed");
			sb.AppendLine("\tArrayPos1++");
			sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],9,TempValue4,{optypos})");
			sb.AppendLine("\tObject[ArrayPos1].Priority=1");
			sb.AppendLine("\tObject[ArrayPos1].Value1=TempValue1");
			sb.AppendLine($"\tObject[ArrayPos1].Value2={Math.Abs(seed)}");
			if (seed < 0)
			{
				int seed2 = seed;
				int digits = 0;
				do
				{
					seed2 /= 10;
					digits++;
				}
				while (seed2 != 0);
				sb.AppendLine($"\tObject[ArrayPos1].Value3={digits * 8}");
			}
			optypos += 38;
			sb.AppendLine("\t// Mode");
			sb.AppendLine("\tArrayPos1++");
			sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],10,TempValue4,{optypos})");
			sb.AppendLine("\tObject[ArrayPos1].Priority=1");
			sb.AppendLine("\tObject[ArrayPos1].Value0=71");
			sb.AppendLine($"\tObject[ArrayPos1].Value1={(int)settings.Mode + 86}");
			if (settings.Mode == Modes.AllStagesWarps)
			{
				optypos += 38;
				sb.AppendLine("\t// Main path");
				sb.AppendLine("\tArrayPos1++");
				sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],10,TempValue4,{optypos})");
				sb.AppendLine("\tObject[ArrayPos1].Priority=1");
				sb.AppendLine("\tObject[ArrayPos1].Value0=72");
				sb.AppendLine($"\tObject[ArrayPos1].Value1={mainPathSelector.SelectedIndex + 94}");
				optypos += 38;
				sb.AppendLine("\t// Max BW jump");
				sb.AppendLine("\tArrayPos1++");
				sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],11,TempValue4,{optypos})");
				sb.AppendLine("\tObject[ArrayPos1].Priority=1");
				sb.AppendLine("\tObject[ArrayPos1].Value1=TempValue1");
				sb.AppendLine("\tObject[ArrayPos1].Value2=73");
				sb.AppendLine($"\tObject[ArrayPos1].Value3={maxBackJump.Value}");
				optypos += 38;
				sb.AppendLine("\t// Max FW jump");
				sb.AppendLine("\tArrayPos1++");
				sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],11,TempValue4,{optypos})");
				sb.AppendLine("\tObject[ArrayPos1].Priority=1");
				sb.AppendLine("\tObject[ArrayPos1].Value1=TempValue1");
				sb.AppendLine("\tObject[ArrayPos1].Value2=74");
				sb.AppendLine($"\tObject[ArrayPos1].Value3={maxForwJump.Value}");
				optypos = 487;
				sb.AppendLine("\tTempValue4+=Screen.XSize");
				sb.AppendLine("\tTempValue4-=88");
				sb.AppendLine("\t// BW prob");
				sb.AppendLine("\tArrayPos1++");
				sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],11,TempValue4,{optypos})");
				sb.AppendLine("\tObject[ArrayPos1].Priority=1");
				sb.AppendLine("\tObject[ArrayPos1].Value1=TempValue1");
				sb.AppendLine("\tObject[ArrayPos1].Value1+=TempValue4");
				sb.AppendLine("\tObject[ArrayPos1].Value1-=100");
				sb.AppendLine("\tObject[ArrayPos1].Value2=75");
				sb.AppendLine($"\tObject[ArrayPos1].Value3={backJumpProb.Value}");
				optypos += 38;
				sb.AppendLine("\t// Allow same");
				sb.AppendLine("\tArrayPos1++");
				sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],10,TempValue4,{optypos})");
				sb.AppendLine("\tObject[ArrayPos1].Priority=1");
				sb.AppendLine("\tObject[ArrayPos1].Value0=76");
				sb.AppendLine($"\tObject[ArrayPos1].Value1={(allowSameLevel.Checked ? 97 : 98)}");
			}
			optypos += 38;
			sb.AppendLine("\t// Music");
			sb.AppendLine("\tArrayPos1++");
			sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],10,TempValue4,{optypos})");
			sb.AppendLine("\tObject[ArrayPos1].Priority=1");
			sb.AppendLine("\tObject[ArrayPos1].Value0=77");
			sb.AppendLine($"\tObject[ArrayPos1].Value1={(randomMusic.Checked ? (separateSoundtracks.Checked ? 102 : 101) : 100)}");
			optypos += 38;
			sb.AppendLine("\t// Items");
			sb.AppendLine("\tArrayPos1++");
			sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],10,TempValue4,{optypos})");
			sb.AppendLine("\tObject[ArrayPos1].Priority=1");
			sb.AppendLine("\tObject[ArrayPos1].Value0=78");
			sb.AppendLine($"\tObject[ArrayPos1].Value1={new[] { 100, 103, 104, 92 }[randomItemMode.SelectedIndex]}");
			optypos += 38;
			sb.AppendLine("\t// Time posts");
			sb.AppendLine("\tArrayPos1++");
			sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],10,TempValue4,{optypos})");
			sb.AppendLine("\tObject[ArrayPos1].Priority=1");
			sb.AppendLine("\tObject[ArrayPos1].Value0=79");
			sb.AppendLine($"\tObject[ArrayPos1].Value1={(randomTimePosts.Checked ? 99 : 100)}");
			if (settings.Mode == Modes.AllStagesWarps)
				optypos += 38;
			else
			{
				optypos = 487;
				sb.AppendLine("\tTempValue4+=Screen.XSize");
				sb.AppendLine("\tTempValue4-=88");
			}
			sb.AppendLine("\t// Checkpoints");
			sb.AppendLine("\tArrayPos1++");
			sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],10,TempValue4,{optypos})");
			sb.AppendLine("\tObject[ArrayPos1].Priority=1");
			sb.AppendLine("\tObject[ArrayPos1].Value0=80");
			sb.AppendLine($"\tObject[ArrayPos1].Value1={(replaceCheckpoints.Checked ? 99 : 100)}");
			if (settings.Mode == Modes.AllStagesWarps)
			{
				optypos = 487;
				sb.AppendLine("\tTempValue4+=Screen.XSize");
				sb.AppendLine("\tTempValue4-=88");
			}
			else
				optypos += 38;
			sb.AppendLine("\t// Palettes");
			sb.AppendLine("\tArrayPos1++");
			sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],10,TempValue4,{optypos})");
			sb.AppendLine("\tObject[ArrayPos1].Priority=1");
			sb.AppendLine("\tObject[ArrayPos1].Value0=81");
			sb.AppendLine($"\tObject[ArrayPos1].Value1={(randomPalettes.Checked ? 99 : 100)}");
			optypos += 38;
			sb.AppendLine("\t// UFOs");
			sb.AppendLine("\tArrayPos1++");
			sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],10,TempValue4,{optypos})");
			sb.AppendLine("\tObject[ArrayPos1].Priority=1");
			sb.AppendLine("\tObject[ArrayPos1].Value0=82");
			sb.AppendLine($"\tObject[ArrayPos1].Value1={(randomUFOs.Checked ? new[] { 105, 106, 107, 92 }[ufoDifficulty.SelectedIndex] : 100)}");
			optypos += 38;
			sb.AppendLine("\t// Water");
			sb.AppendLine("\tArrayPos1++");
			sb.AppendLine($"\tResetObjectEntity(ArrayPos1,TypeName[Tails Unlock Scr],10,TempValue4,{optypos})");
			sb.AppendLine("\tObject[ArrayPos1].Priority=1");
			sb.AppendLine("\tObject[ArrayPos1].Value0=83");
			sb.Append($"\tObject[ArrayPos1].Value1={(randomWater.Checked ? (addWaterOnly.Checked ? 108 : 99) : 98)}");
			tmpstr = tmpstr.Replace("//REPLACE3", sb.ToString());
			newpath = Path.Combine(path, @"Data\Scripts\Secrets\TailsUnlock.txt");
			File.WriteAllText(newpath, tmpstr);
			vdir.AddFile("Scripts/Secrets/TailsUnlock.txt", newpath);
			newpath = Path.Combine(path, @"Data\Palettes\RandoSummary.act");
			File.WriteAllBytes(newpath, Properties.Resources.RandoSummaryPal);
			vdir.AddFile("Palettes/RandoSummary.act", newpath);
			newpath = Path.Combine(path, @"Data\Sprites\Secrets\RandoSummary.gif");
			File.WriteAllBytes(newpath, Properties.Resources.RandoSummaryImg);
			vdir.AddFile("Sprites/Secrets/RandoSummary.gif", newpath);
			sb.Clear();
			switch (settings.Mode)
			{
				case Modes.Segments:
					for (int i = 0; i < stagecount; i++)
					{
						Stage stg = stages[i];
						sb.AppendLine($"\tcase {i}");
						if (stg.Clear % 10 > 7)
						{
							sb.AppendLine($"\t\tGetBit(TempValue0,SpecialStage.TimeStones,{Array.IndexOf(stageids, i) / 10})");
							sb.AppendLine("\t\tif TempValue0!=0");
							sb.AppendLine($"\t\t\tTempValue0={stg.ClearGF}");
							sb.AppendLine("\t\telse");
							sb.AppendLine($"\t\t\tTempValue0={stg.Clear}");
							sb.AppendLine("\t\tendif");
						}
						else
							sb.AppendLine($"\t\tTempValue0={stg.Clear}");
						sb.AppendLine("\t\tbreak");
					}
					tmpstr = Properties.Resources.ActFinish_template;
					tmpstr = tmpstr.Replace("//REPLACE", sb.ToString());
					sb.Clear();
					sb.AppendLine("\t\t\t\t\tswitch Stage.ListPos");
					int ssnum = 0;
					for (int i = 0; i < stagecount; i++)
					{
						sb.AppendLine($"\t\t\t\t\tcase {stageids[i]}");
						if (i % 10 == 7)
						{
							sb.AppendLine($"\t\t\t\t\t\tSpecialStage.ListPos={ssnum++}");
							sb.AppendLine("\t\t\t\t\t\tbreak");
							i += 2;
						}
					}
					sb.AppendLine("\t\t\t\t\tendswitch");
					tmpstr = tmpstr.Replace("//SPECIAL", sb.ToString());
					newpath = Path.Combine(path, @"Data\Scripts\Global\ActFinish.txt");
					File.WriteAllText(newpath, tmpstr);
					vdir.AddFile("Scripts/Global/ActFinish.txt", newpath);
					sb.Clear();
					ssnum = 0;
					for (int i = 0; i < stagecount; i++)
					{
						sb.AppendLine($"\tcase {stageids[i]}");
						if (i % 10 == 7)
						{
							sb.AppendLine($"\t\tTempValue0={ssnum++}");
							sb.AppendLine("\t\tbreak");
							i += 2;
						}
					}
					newpath = Path.Combine(path, @"Data\Scripts\Global\SpecialRing.txt");
					File.WriteAllText(newpath, Properties.Resources.SpecialRing_template.Replace("//REPLACE", sb.ToString()));
					vdir.AddFile("Scripts/Global/SpecialRing.txt", newpath);
					break;
				case Modes.Shadow:
					for (int i = 0; i < stagecount; i++)
					{
						Stage stg = stages[i];
						sb.AppendLine($"\tcase {i}");
						if (Array.IndexOf(stageids, i) < 29)
						{
							if (stg.Clear == -1)
								switch (stg.TimePeriod)
								{
									case TimePeriods.Past:
										sb.AppendLine("\t\tif Warp.Destination==2");
										sb.AppendLine($"\t\t\tTempValue0={stg.Future}");
										sb.AppendLine("\t\telse");
										sb.AppendLine($"\t\t\tTempValue0={stg.Past}");
										sb.AppendLine("\t\tendif");
										break;
									case TimePeriods.GoodFuture:
									case TimePeriods.BadFuture:
										sb.AppendLine("\t\tif Warp.Destination==1");
										sb.AppendLine($"\t\t\tTempValue0={stg.Past}");
										sb.AppendLine("\t\telse");
										sb.AppendLine($"\t\t\tTempValue0={stg.Future}");
										sb.AppendLine("\t\tendif");
										break;
								}
							else if (stg.Act == 2)
							{
								if (stg.Past != -1 && stg.Future != -1)
								{
									sb.AppendLine("\t\tswitch SaveRAM[7167]");
									sb.AppendLine("\t\tcase 0");
									sb.AppendLine($"\t\t\tTempValue0={stg.Clear}");
									sb.AppendLine("\t\t\tbreak");
									sb.AppendLine("\t\tcase 1");
									sb.AppendLine($"\t\t\tTempValue0={stg.Past}");
									sb.AppendLine("\t\t\tbreak");
									sb.AppendLine("\t\tcase 2");
									sb.AppendLine($"\t\t\tTempValue0={stg.Future}");
									sb.AppendLine("\t\t\tbreak");
									sb.AppendLine("\t\tendswitch");
								}
								else if (stg.Future != -1)
								{
									sb.AppendLine("\t\tif SaveRAM[7167]!=0");
									sb.AppendLine($"\t\t\tTempValue0={stg.Future}");
									sb.AppendLine("\t\telse");
									sb.AppendLine($"\t\t\tTempValue0={stg.Clear}");
									sb.AppendLine("\t\tendif");
								}
								else if (stg.Past != -1)
								{
									sb.AppendLine("\t\tif SaveRAM[7167]!=0");
									sb.AppendLine($"\t\t\tTempValue0={stg.Past}");
									sb.AppendLine("\t\telse");
									sb.AppendLine($"\t\t\tTempValue0={stg.Clear}");
									sb.AppendLine("\t\tendif");
								}
								else
									sb.AppendLine($"\t\tTempValue0={stg.Clear}");
							}
							else
							{
								if (stg.Clear % 10 > 7)
									sb.AppendLine($"\t\tSaveRAM[7167]=Warp.Destination");
								if (stg.Past != -1 && stg.Future != -1)
								{
									sb.AppendLine("\t\tswitch Warp.Destination");
									sb.AppendLine("\t\tcase 0");
									sb.AppendLine($"\t\t\tTempValue0={stg.Clear}");
									sb.AppendLine("\t\t\tbreak");
									sb.AppendLine("\t\tcase 1");
									sb.AppendLine($"\t\t\tTempValue0={stg.Past}");
									sb.AppendLine("\t\t\tbreak");
									sb.AppendLine("\t\tcase 2");
									sb.AppendLine($"\t\t\tTempValue0={stg.Future}");
									sb.AppendLine("\t\t\tbreak");
									sb.AppendLine("\t\tendswitch");
								}
								else if (stg.Future != -1)
								{
									sb.AppendLine("\t\tif Warp.Destination!=0");
									sb.AppendLine($"\t\t\tTempValue0={stg.Future}");
									sb.AppendLine("\t\telse");
									sb.AppendLine($"\t\t\tTempValue0={stg.Clear}");
									sb.AppendLine("\t\tendif");
								}
								else if (stg.Past != -1)
								{
									sb.AppendLine("\t\tif Warp.Destination!=0");
									sb.AppendLine($"\t\t\tTempValue0={stg.Past}");
									sb.AppendLine("\t\telse");
									sb.AppendLine($"\t\t\tTempValue0={stg.Clear}");
									sb.AppendLine("\t\tendif");
								}
								else
									sb.AppendLine($"\t\tTempValue0={stg.Clear}");
							}
						}
						else
							sb.AppendLine($"\t\tTempValue0={stg.Clear}");
						sb.AppendLine("\t\tbreak");
					}
					newpath = Path.Combine(path, @"Data\Scripts\Global\ActFinish.txt");
					File.WriteAllText(newpath, Properties.Resources.ActFinish_template.Replace("//REPLACE", sb.ToString()));
					vdir.AddFile("Scripts/Global/ActFinish.txt", newpath);
					sb.Clear();
					for (int i = 0; i < 29; i++)
						sb.AppendLine($"\tcase {stageids[i]}");
					newpath = Path.Combine(path, @"Data\Scripts\Global\FuturePost.txt");
					File.WriteAllText(newpath, Properties.Resources.FuturePost_template.Replace("//REPLACE", sb.ToString()));
					vdir.AddFile("Scripts/Global/FuturePost.txt", newpath);
					newpath = Path.Combine(path, @"Data\Scripts\Global\PastPost.txt");
					File.WriteAllText(newpath, Properties.Resources.PastPost);
					vdir.AddFile("Scripts/Global/PastPost.txt", newpath);
					newpath = Path.Combine(path, @"Data\Scripts\Global\GoalPost.txt");
					File.WriteAllText(newpath, Properties.Resources.GoalPost);
					vdir.AddFile("Scripts/Global/GoalPost.txt", newpath);
					if (vdir.FileExists("Scripts/Global/BlueShield.txt"))
						tmpstr = File.ReadAllText(vdir.GetFile("Scripts/Global/BlueShield.txt").SourcePath);
					else
						tmpstr = Properties.Resources.BlueShield;
					newpath = Path.Combine(path, @"Data\Scripts\Global\BlueShield.txt");
					File.WriteAllText(newpath, tmpstr.Replace("Warp.Timer==0", "Warp.Timer<=0"));
					vdir.AddFile("Scripts/Global/BlueShield.txt", newpath);
					if (vdir.FileExists("Scripts/Global/Invincibility.txt"))
						tmpstr = File.ReadAllText(vdir.GetFile("Scripts/Global/Invincibility.txt").SourcePath);
					else
						tmpstr = Properties.Resources.BlueShield;
					newpath = Path.Combine(path, @"Data\Scripts\Global\Invincibility.txt");
					File.WriteAllText(newpath, tmpstr.Replace("Warp.Timer==0", "Warp.Timer<=0"));
					vdir.AddFile("Scripts/Global/Invincibility.txt", newpath);
					break;
				default:
					for (int i = 0; i < stagecount; i++)
					{
						Stage stg = stages[i];
						sb.AppendLine($"\tcase {i}");
						if (stg.ClearGF != -1)
						{
							sb.AppendLine("\t\tif Good_Future_Count==2");
							sb.AppendLine($"\t\t\tTempValue0={stg.ClearGF}");
							sb.AppendLine("\t\telse");
							sb.AppendLine($"\t\t\tTempValue0={stg.Clear}");
							sb.AppendLine("\t\tendif");
						}
						else
							sb.AppendLine($"\t\tTempValue0={stg.Clear}");
						sb.AppendLine("\t\tbreak");
					}
					newpath = Path.Combine(path, @"Data\Scripts\Global\ActFinish.txt");
					File.WriteAllText(newpath, Properties.Resources.ActFinish_template.Replace("//REPLACE", sb.ToString()));
					vdir.AddFile("Scripts/Global/ActFinish.txt", newpath);
					break;
			}
			if (timewarp)
			{
				sb.Clear();
				for (int i = 0; i < stagecount; i++)
				{
					Stage stg = stages[i];
					if (stg.Past == -1 && stg.Future == -1)
						continue;
					if (settings.Mode == Modes.Shadow && Array.IndexOf(stageids, i) < 29)
						continue;
					sb.AppendLine($"\t\t\tcase {i}");
					if (stg.Past != -1 && stg.Future != -1)
					{
						sb.AppendLine("\t\t\t\tif Warp.Destination==1");
						sb.AppendLine($"\t\t\t\t\tTempValue0={stg.Past}");
						sb.AppendLine($"\t\t\t\t\tTempValue1=1");
						sb.AppendLine("\t\t\t\telse");
						if (stg.GoodFuture != -1)
						{
							sb.AppendLine("\t\t\t\t\tif Transporter_Destroyed==1");
							sb.AppendLine($"\t\t\t\t\t\tTempValue0={stg.GoodFuture}");
							sb.AppendLine($"\t\t\t\t\t\tTempValue1=3");
							sb.AppendLine("\t\t\t\t\telse");
							sb.AppendLine($"\t\t\t\t\t\tTempValue0={stg.Future}");
							sb.AppendLine($"\t\t\t\t\t\tTempValue1=4");
							sb.AppendLine("\t\t\t\t\tendif");
						}
						else
						{
							sb.AppendLine($"\t\t\t\t\tTempValue0={stg.Future}");
							sb.AppendLine($"\t\t\t\t\tTempValue1=2");
						}
						sb.AppendLine("\t\t\t\tendif");
					}
					else if (stg.Future != -1)
					{
						sb.AppendLine($"\t\t\t\tTempValue0={stg.Future}");
						sb.AppendLine($"\t\t\t\tTempValue1=2");
					}
					else if (stg.Past != -1)
					{
						sb.AppendLine($"\t\t\t\tTempValue0={stg.Past}");
						sb.AppendLine($"\t\t\t\tTempValue1=1");
					}
					sb.AppendLine("\t\t\t\tbreak");
				}
				newpath = Path.Combine(path, @"Data\Scripts\Global\TimeWarp.txt");
				File.WriteAllText(newpath, Properties.Resources.TimeWarp_template.Replace("//REPLACE", sb.ToString()));
				vdir.AddFile("Scripts/Global/TimeWarp.txt", newpath);
			}
			if (randomMusic.Checked)
			{
				var scripts = new List<(VirtualFile file, string script)>();
				Dictionary<string, string> musicFiles = new Dictionary<string, string>();
				foreach (var file in vdir.GetDirectory("Scripts").GetAllFiles().Where(a => a.Name.EndsWith(".txt")))
				{
					string script = File.ReadAllText(file.SourcePath);
					MatchCollection matches = musregex.Matches(script);
					if (matches.Count > 0)
					{
						scripts.Add((file, script));
						foreach (Match match in matches)
							if (!musicFiles.ContainsKey(match.Groups[1].Value))
								musicFiles.Add(match.Groups[1].Value, match.Groups[3].Value);
					}
				}
				var loopmuslist = musicFiles.Where(a => a.Value != "0").ToArray();
				var muslist = musicFiles.Where(a => a.Value == "0").ToArray();
				var loopmuslistjp = loopmuslist.Where(a => !a.Key.StartsWith("\"US/", StringComparison.OrdinalIgnoreCase)).ToArray();
				var loopmuslistus = loopmuslist.Where(a => !a.Key.StartsWith("\"JP/", StringComparison.OrdinalIgnoreCase)).ToArray();
				var muslistjp = muslist.Where(a => !a.Key.StartsWith("\"US/", StringComparison.OrdinalIgnoreCase)).ToArray();
				var muslistus = muslist.Where(a => !a.Key.StartsWith("\"JP/", StringComparison.OrdinalIgnoreCase)).ToArray();
				foreach (var script in scripts)
				{
					script.file.SourcePath = Path.Combine(path, script.file.FullName);
					Directory.CreateDirectory(Path.GetDirectoryName(script.file.SourcePath));
					File.WriteAllText(script.file.SourcePath, musregex.Replace(script.script, m =>
					{
						if (separateSoundtracks.Checked)
							if (m.Groups[1].Value.StartsWith("\"JP/", StringComparison.OrdinalIgnoreCase))
							{
								loopmuslist = loopmuslistjp;
								muslist = muslistjp;
							}
							else if (m.Groups[1].Value.StartsWith("\"US/", StringComparison.OrdinalIgnoreCase))
							{
								loopmuslist = loopmuslistus;
								muslist = muslistus;
							}
						KeyValuePair<string, string> mus;
						if (m.Groups[3].Value == "0")
							mus = muslist[r.Next(muslist.Length)];
						else
							mus = loopmuslist[r.Next(loopmuslist.Length)];
						return $"SetMusicTrack({mus.Key},{m.Groups[2].Value},{mus.Value})";
					}));
				}
			}
			if (randomItemMode.SelectedIndex > 0)
			{
				int monitorid = gc.objects.FindIndex(a => a.name == "Monitor") + 1;
				byte[] itemmap = new byte[9];
				for (byte i = 0; i < 9; i++)
					itemmap[i] = i;
				if ((ItemMode)randomItemMode.SelectedIndex == ItemMode.OneToOne)
					Shuffle(r, itemmap);
				foreach (var stginf in gc.stageLists[1].list)
				{
					if ((ItemMode)randomItemMode.SelectedIndex == ItemMode.OneToOnePerStage)
						Shuffle(r, itemmap);
					vfile = vdir.GetFile($"Stages/{stginf.folder}/Act{stginf.id}.bin");
					RSDKv3.Scene scn;
					if (vfile.SourcePath == "Data.rsdk")
						using (MemoryStream ms = new MemoryStream(dataFile.files.Single(a => a.fullFilename == vfile.FullName).fileData))
							scn = new RSDKv3.Scene(ms);
					else
						scn = new RSDKv3.Scene(vfile.SourcePath);
					foreach (var item in scn.entities.Where(a => a.type == monitorid))
					{
						bool add = item.propertyValue > 9;
						if (add)
							item.propertyValue -= 10;
						switch ((ItemMode)randomItemMode.SelectedIndex)
						{
							case ItemMode.OneToOne:
							case ItemMode.OneToOnePerStage:
								item.propertyValue = itemmap[item.propertyValue];
								break;
							case ItemMode.Wild:
								item.propertyValue = (byte)r.Next(9);
								break;
						}
						if (add)
							item.propertyValue += 10;
					}
					vfile.SourcePath = Path.Combine(path, vfile.FullName);
					Directory.CreateDirectory(Path.GetDirectoryName(vfile.SourcePath));
					scn.write(vfile.SourcePath);
				}
			}
			if (randomUFOs.Checked)
			{
				foreach (var stginf in gc.stageLists[2].list)
				{
					vfile = vdir.GetFile($"Stages/{stginf.folder}/StageConfig.bin");
					RSDKv3.StageConfig stgcnf;
					if (vfile.SourcePath == "Data.rsdk")
						using (MemoryStream ms = new MemoryStream(dataFile.files.Single(a => a.fullFilename == vfile.FullName).fileData))
							stgcnf = new RSDKv3.StageConfig(ms);
					else
						stgcnf = new RSDKv3.StageConfig(vfile.SourcePath);
					int ufoid = stgcnf.objects.FindIndex(a => a.name == "UFO") + 1;
					int ufonodeid = stgcnf.objects.FindIndex(a => a.name == "UFO Node") + 1;
					int xmin = 0;
					int xmax = 4096;
					int ymin = 0;
					int ymax = 4096;
					foreach (var m in ssboundregex.Matches(File.ReadAllText(vdir.GetDirectory("Scripts").GetFile(stgcnf.objects.Find(a => a.name == "BGEffects").script).SourcePath)).Cast<Match>())
						switch (m.Groups[1].Value)
						{
							case "XBoundary1":
								xmin = int.Parse(m.Groups[2].Value) >> 16;
								break;
							case "XBoundary2":
								xmax = int.Parse(m.Groups[2].Value) >> 16;
								break;
							case "YBoundary1":
								ymin = int.Parse(m.Groups[2].Value) >> 16;
								break;
							case "YBoundary2":
								ymax = int.Parse(m.Groups[2].Value) >> 16;
								break;
						}
					xmin += 128;
					xmax -= 128;
					ymin += 128;
					ymax -= 128;
					int mindist = 0;
					int maxdist = 0;
					int mincnt = 0;
					int maxcnt = 0;
					switch ((UFODifficulty)ufoDifficulty.SelectedIndex)
					{
						case UFODifficulty.Easy:
							mindist = 64;
							maxdist = 640;
							mincnt = 4;
							maxcnt = 6;
							break;
						case UFODifficulty.Medium:
							mindist = 96;
							maxdist = 960;
							mincnt = 6;
							maxcnt = 9;
							break;
						case UFODifficulty.Hard:
							mindist = 128;
							maxdist = 1280;
							mincnt = 8;
							maxcnt = 12;
							break;
						case UFODifficulty.Wild:
							mincnt = 1;
							maxcnt = 30;
							break;
					}
					vfile = vdir.GetFile($"Stages/{stginf.folder}/Act{stginf.id}.bin");
					RSDKv3.Scene scn;
					if (vfile.SourcePath == "Data.rsdk")
						using (MemoryStream ms = new MemoryStream(dataFile.files.Single(a => a.fullFilename == vfile.FullName).fileData))
							scn = new RSDKv3.Scene(ms);
					else
						scn = new RSDKv3.Scene(vfile.SourcePath);
					scn.entities.RemoveAll(a => a.type == ufoid || a.type == ufonodeid);
					int cnt = r.Next(mincnt, maxcnt + 1);
					for (int i = 0; i <= cnt; i++)
					{
						int nodecnt = r.Next(2, 9);
						int lastx = r.Next(xmin + 64, xmax - 63);
						int lasty = r.Next(ymin + 64, ymax - 63);
						scn.entities.Add(new RSDKv3.Scene.Entity((byte)ufoid, i == cnt ? (byte)2 : (byte)r.Next(2), (short)lastx, (short)lasty));
						scn.entities.Add(new RSDKv3.Scene.Entity((byte)ufonodeid, (byte)(r.Next(1, 9) * 30), (short)lastx, (short)lasty));
						for (int j = 1; j < nodecnt; j++)
						{
							if ((UFODifficulty)ufoDifficulty.SelectedIndex == UFODifficulty.Wild)
							{
								lastx = r.Next(xmin + 64, xmax - 63);
								lasty = r.Next(ymin + 64, ymax - 63);
							}
							else
							{
								int newx, newy;
								do
								{
									int dist = r.Next(mindist, maxdist + 1);
									double ang = r.NextDouble() * (2 * Math.PI);
									newx = lastx + (int)(Math.Cos(ang) * dist);
									newy = lasty + (int)(Math.Sin(ang) * dist);
								}
								while (newx <= xmin || newx >= xmax || newy <= ymin || newy >= ymax);
								lastx = newx;
								lasty = newy;
							}
							scn.entities.Add(new RSDKv3.Scene.Entity((byte)ufonodeid, (byte)(r.Next(1, 9) * 30), (short)lastx, (short)lasty));
						}
					}
					vfile.SourcePath = Path.Combine(path, vfile.FullName);
					Directory.CreateDirectory(Path.GetDirectoryName(vfile.SourcePath));
					scn.write(vfile.SourcePath);
				}
			}
			if (randomWater.Checked)
			{
				sb.Clear();
				for (int i = 0; i < gc.stageLists[1].list.Count; i++)
				{
					var stginf = gc.stageLists[1].list[i];
					vfile = vdir.GetFile($"Stages/{stginf.folder}/StageConfig.bin");
					RSDKv3.StageConfig stgcnf;
					if (vfile.SourcePath == "Data.rsdk")
						using (MemoryStream ms = new MemoryStream(dataFile.files.Single(a => a.fullFilename == vfile.FullName).fileData))
							stgcnf = new RSDKv3.StageConfig(ms);
					else
						stgcnf = new RSDKv3.StageConfig(vfile.SourcePath);
					bool randwater = r.Next(2) == 1;
					bool haswater = false;
					int waterid = stgcnf.objects.FindIndex(a => a.name == "Water") + 1;
					if (waterid == 0)
					{
						if (!randwater)
							continue;
						waterid = stgcnf.objects.Count + 1;
						stgcnf.objects.Add(new RSDKv3.GameConfig.ObjectInfo() { name = "Water", script = "Global/Water.txt" });
						stgcnf.objects.Add(new RSDKv3.GameConfig.ObjectInfo() { name = "Water Splash", script = "R4/WaterSplash.txt" });
						stgcnf.objects.Add(new RSDKv3.GameConfig.ObjectInfo() { name = "Air Bubble", script = "R4/AirBubble.txt" });
						stgcnf.objects.Add(new RSDKv3.GameConfig.ObjectInfo() { name = "Countdown Bubble", script = "R4/CountdownBubble.txt" });
						sb.AppendLine($"\t\t\tcase {i}");
						sb.AppendLine($"\t\t\t\tObject[ArrayPos0].Value3={stgcnf.soundFX.Count}");
						stgcnf.soundFX.Add("Stage/WaterSplash.wav");
						sb.AppendLine($"\t\t\t\tObject[ArrayPos0].Value4={stgcnf.soundFX.Count}");
						stgcnf.soundFX.Add("Stage/Drowning.wav");
						sb.AppendLine($"\t\t\t\tObject[ArrayPos0].Value5={stgcnf.soundFX.Count}");
						stgcnf.soundFX.Add("Stage/AirAlert.wav");
						sb.AppendLine($"\t\t\t\tObject[ArrayPos0].Value6={stgcnf.soundFX.Count}");
						stgcnf.soundFX.Add("Stage/DrownAlert.wav");
						sb.AppendLine("\t\t\t\tbreak");
						vfile.SourcePath = Path.Combine(path, vfile.FullName);
						Directory.CreateDirectory(Path.GetDirectoryName(vfile.SourcePath));
						stgcnf.write(vfile.SourcePath);
					}
					else if (addWaterOnly.Checked)
						continue;
					else
						haswater = true;
					if (stgcnf.loadGlobalObjects)
						waterid += gc.objects.Count;
					vfile = vdir.GetFile($"Stages/{stginf.folder}/Act{stginf.id}.bin");
					RSDKv3.Scene scn;
					if (vfile.SourcePath == "Data.rsdk")
						using (MemoryStream ms = new MemoryStream(dataFile.files.Single(a => a.fullFilename == vfile.FullName).fileData))
							scn = new RSDKv3.Scene(ms);
					else
						scn = new RSDKv3.Scene(vfile.SourcePath);
					if (haswater)
					{
						if (randwater)
							scn.entities.Find(a => a.type == waterid).ypos = (short)r.Next(scn.height << 7);
						else
							scn.entities.RemoveAll(a => a.type == waterid);
					}
					else
					{
						scn.objectTypeNames.Add("Water");
						scn.objectTypeNames.Add("Water Splash");
						scn.objectTypeNames.Add("Air Bubble");
						scn.objectTypeNames.Add("Countdown Bubble");
						scn.entities.Add(new RSDKv3.Scene.Entity((byte)waterid, 0, 0, (short)r.Next(scn.height << 7)));
					}
					vfile.SourcePath = Path.Combine(path, vfile.FullName);
					Directory.CreateDirectory(Path.GetDirectoryName(vfile.SourcePath));
					scn.write(vfile.SourcePath);
					if (!haswater)
					{
						vfile = vdir.GetFile($"Stages/{stginf.folder}/Backgrounds.bin");
						RSDKv3.Backgrounds bg;
						if (vfile.SourcePath == "Data.rsdk")
							using (MemoryStream ms = new MemoryStream(dataFile.files.Single(a => a.fullFilename == vfile.FullName).fileData))
								bg = new RSDKv3.Backgrounds(ms);
						else
							bg = new RSDKv3.Backgrounds(vfile.SourcePath);
						foreach (var item in bg.hScroll)
							item.deform = true;
						vfile.SourcePath = Path.Combine(path, vfile.FullName);
						Directory.CreateDirectory(Path.GetDirectoryName(vfile.SourcePath));
						bg.write(vfile.SourcePath);
					}
				}
				newpath = Path.Combine(path, @"Data\Scripts\Global\Water.txt");
				File.WriteAllText(newpath, Properties.Resources.Water_template.Replace("//REPLACE", sb.ToString()));
				vdir.AddFile("Scripts/Global/Water.txt", newpath);
			}
			if (randomPalettes.Checked)
			{
				foreach (var file in vdir.GetDirectory("Palettes").GetAllFiles().Where(a => a.Name.EndsWith(".act")))
				{
					HueRotation hr = new HueRotation(r);
					byte[] pal;
					if (file.SourcePath == "Data.rsdk")
						pal = dataFile.files.Single(a => a.fullFilename == file.FullName).fileData;
					else
						pal = File.ReadAllBytes(file.SourcePath);
					hr.ApplyRotation(pal);
					file.SourcePath = Path.Combine(path, file.FullName);
					Directory.CreateDirectory(Path.GetDirectoryName(file.SourcePath));
					File.WriteAllBytes(file.SourcePath, pal);
				}
				foreach (var file in vdir.GetDirectory("Stages").GetAllFiles().Where(a => a.Name == "16x16Tiles.gif" || a.Name == "StageConfig.bin"))
				{
					HueRotation hr = new HueRotation(r);
					switch (file.Name)
					{
						case "16x16Tiles.gif":
							byte[] gif;
							if (file.SourcePath == "Data.rsdk")
								gif = dataFile.files.Single(a => a.fullFilename == file.FullName).fileData;
							else
								gif = File.ReadAllBytes(file.SourcePath);
							var pal = new byte[(1 << ((gif[0xA] & 7) + 1)) * 3];
							Array.Copy(gif, 0xD, pal, 0, pal.Length);
							hr.ApplyRotation(pal);
							Array.Copy(pal, 0, gif, 0xD, pal.Length);
							file.SourcePath = Path.Combine(path, file.FullName);
							Directory.CreateDirectory(Path.GetDirectoryName(file.SourcePath));
							File.WriteAllBytes(file.SourcePath, gif);
							break;
						case "StageConfig.bin":
							RSDKv3.StageConfig stgcnf;
							if (file.SourcePath == "Data.rsdk")
								using (MemoryStream ms = new MemoryStream(dataFile.files.Single(a => a.fullFilename == file.FullName).fileData))
									stgcnf = new RSDKv3.StageConfig(ms);
							else
								stgcnf = new RSDKv3.StageConfig(file.SourcePath);
							hr.ApplyRotation(stgcnf.stagePalette);
							file.SourcePath = Path.Combine(path, file.FullName);
							Directory.CreateDirectory(Path.GetDirectoryName(file.SourcePath));
							stgcnf.write(file.SourcePath);
							break;
					}
				}
			}
			spoilerLevelList.BeginUpdate();
			spoilerLevelList.Items.Clear();
			for (int i = 0; i < stagecount; i++)
				spoilerLevelList.Items.Add($"{GetStageName(stageids[i])}");
			spoilerLevelList.EndUpdate();
			spoilerLevelList.Enabled = true;
			spoilerLevelList.SelectedIndex = 0;
			saveLogButton.Enabled = true;
			makeChartButton.Enabled = true;
		}

		private static void Shuffle<T>(Random r, T[] array, int count)
		{
			int[] order = new int[count];
			for (int i = 0; i < count; i++)
				order[i] = r.Next();
			Array.Sort(order, array);
		}

		private static void Shuffle<T>(Random r, T[] array) => Shuffle(r, array, array.Length);

		private static int GetStageFromLists(Random r, List<int> curset, List<int> stagepool, int weight)
		{
			--weight;
			if (weight <= 0)
				return stagepool[r.Next(stagepool.Count)];
			int tmp = r.Next((curset.Count * weight) + stagepool.Count);
			if (tmp < curset.Count * weight)
				return curset[tmp / weight];
			else
				return stagepool[tmp - (curset.Count * weight)];
		}

		private static string GetStageName(int id)
		{
			if (id == stagecount + 1)
				return "Start";
			else if (id == stagecount)
				return "Ending";
			int rnd = Math.DivRem(id, 10, out int zon);
			return $"{RoundNames[rnd]} {ZoneNames[zon]}";
		}

		private void spoilerLevelList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (spoilerLevelList.SelectedIndex != -1)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				Stage stg = stages[stageids[spoilerLevelList.SelectedIndex]];
				if (settings.Mode == Modes.AllStagesWarps)
				{
					sb.AppendLine($"Clear -> {GetStageName(stg.Clear)} ({Array.IndexOf(stageids, stg.Clear) - spoilerLevelList.SelectedIndex:+##;-##;0})");
					if (stg.Past != -1)
						sb.AppendLine($"Past -> {GetStageName(stg.Past)} ({Array.IndexOf(stageids, stg.Past) - spoilerLevelList.SelectedIndex:+##;-##;0})");
					if (stg.Future != -1)
						sb.AppendLine($"Future -> {GetStageName(stg.Future)} ({Array.IndexOf(stageids, stg.Future) - spoilerLevelList.SelectedIndex:+##;-##;0})");
					sb.Append("Shortest Path: ");
					int[] shortestPath;
					if (maxForwJump.Value < 2)
					{
						shortestPath = new int[stagecount - 1 - spoilerLevelList.SelectedIndex];
						Array.Copy(stageids, spoilerLevelList.SelectedIndex, shortestPath, 0, shortestPath.Length);
					}
					else
						shortestPath = FindShortestPath(stageids[spoilerLevelList.SelectedIndex]);
					for (int i = 0; i < shortestPath.Length - 1; i++)
					{
						string exit;
						if (stages[shortestPath[i]].Clear == shortestPath[i + 1])
							exit = "Clear";
						else if (stages[shortestPath[i]].Past == shortestPath[i + 1])
							exit = "Past";
						else
							exit = "Future";
						sb.AppendFormat("{0} ({1}) -> ", GetStageName(shortestPath[i]), exit);
					}
					sb.AppendFormat("Ending ({0} levels)", shortestPath.Length);
				}
				else
				{
					sb.AppendLine($"Clear -> {GetStageName(stg.Clear)}");
					if (stg.ClearGF != -1)
						sb.AppendLine($"Clear (Good Future) -> {GetStageName(stg.ClearGF)}");
					if (stg.Past != -1)
						sb.AppendLine($"Past -> {GetStageName(stg.Past)}");
					if (stg.GoodFuture != -1)
					{
						sb.AppendLine($"Bad Future -> {GetStageName(stg.Future)}");
						sb.AppendLine($"Good Future -> {GetStageName(stg.GoodFuture)}");
					}
					else if (stg.Future != -1)
						sb.AppendLine($"Future -> {GetStageName(stg.Future)}");
				}
				spoilerLevelInfo.Text = sb.ToString();
			}
		}

		int[] FindShortestPath(int start)
		{
			Stack<int> stack = new Stack<int>(stagecount);
			stack.Push(start);
			return FindShortestPath(stages[start], stack, null);
		}

		int[] FindShortestPath(Stage stage, Stack<int> path, int[] shortestPath)
		{
			if (shortestPath != null && path.Count >= shortestPath.Length)
				return shortestPath;
			if (stage.Clear != -1 && !path.Contains(stage.Clear))
			{
				path.Push(stage.Clear);
				if (stage.Clear == stagecount)
				{
					if (shortestPath == null || path.Count < shortestPath.Length)
					{
						shortestPath = path.ToArray();
						Array.Reverse(shortestPath);
						path.Pop();
						return shortestPath;
					}
				}
				else
					shortestPath = FindShortestPath(stages[stage.Clear], path, shortestPath);
				path.Pop();
			}
			if (stage.Past != -1 && !path.Contains(stage.Past))
			{
				path.Push(stage.Past);
				if (stage.Past == stagecount)
				{
					if (shortestPath == null || path.Count < shortestPath.Length)
					{
						shortestPath = path.ToArray();
						Array.Reverse(shortestPath);
						path.Pop();
						return shortestPath;
					}
				}
				else
					shortestPath = FindShortestPath(stages[stage.Past], path, shortestPath);
				path.Pop();
			}
			if (stage.Future != -1 && !path.Contains(stage.Future))
			{
				path.Push(stage.Future);
				if (stage.Future == stagecount)
				{
					if (shortestPath == null || path.Count < shortestPath.Length)
					{
						shortestPath = path.ToArray();
						Array.Reverse(shortestPath);
						path.Pop();
						return shortestPath;
					}
				}
				else
					shortestPath = FindShortestPath(stages[stage.Future], path, shortestPath);
				path.Pop();
			}
			return shortestPath;
		}

		private void saveLogButton_Click(object sender, EventArgs e)
		{
			saveFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
			if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
				using (StreamWriter sw = File.CreateText(saveFileDialog1.FileName))
				{
					sw.WriteLine($"Seed: {seedSelector.Value}");
					sw.WriteLine($"Mode: {settings.Mode}");
					if (settings.Mode == Modes.AllStagesWarps)
					{
						sw.WriteLine($"Main Path: {mainPathSelector.SelectedItem}");
						sw.WriteLine($"Max Backwards Jump: {maxBackJump.Value}");
						sw.WriteLine($"Max Forwards Jump: {maxForwJump.Value}");
						sw.WriteLine($"Backwards Jump Probability: {backJumpProb.Value}");
						sw.WriteLine($"Allow Same Level: {allowSameLevel.Checked}");
					}
					sw.WriteLine($"Random Music: {randomMusic.Checked}");
					sw.WriteLine($"Separate Soundtracks: {separateSoundtracks.Checked}");
					sw.WriteLine($"Items: {randomItemMode.SelectedItem}");
					sw.WriteLine($"Time Posts: {randomTimePosts.Checked}");
					sw.WriteLine($"Replace Checkpoints: {replaceCheckpoints.Checked}");
					sw.WriteLine($"Palettes: {randomPalettes.Checked}");
					sw.WriteLine($"UFOs: {randomUFOs.Checked}");
					sw.WriteLine($"UFO Difficulty: {ufoDifficulty.SelectedItem}");
					sw.WriteLine($"Random Water: {randomWater.Checked}");
					sw.WriteLine($"Add Only: {addWaterOnly.Checked}");
					sw.WriteLine();
					for (int i = 0; i < stagecount; i++)
					{
						Stage stg = stages[stageids[i]];
						sw.WriteLine($"{GetStageName(stageids[i])}:");
						if (settings.Mode == Modes.AllStagesWarps)
						{
							sw.WriteLine($"Clear -> {GetStageName(stg.Clear)} ({Array.IndexOf(stageids, stg.Clear) - i:+##;-##;0})");
							if (stg.Past != -1)
								sw.WriteLine($"Past -> {GetStageName(stg.Past)} ({Array.IndexOf(stageids, stg.Past) - i:+##;-##;0})");
							if (stg.Future != -1)
								sw.WriteLine($"Future -> {GetStageName(stg.Future)} ({Array.IndexOf(stageids, stg.Future) - i:+##;-##;0})");
						}
						else
						{
							sw.WriteLine($"Clear -> {GetStageName(stg.Clear)}");
							if (stg.ClearGF != -1)
								sw.WriteLine($"Clear (Good Future) -> {GetStageName(stg.ClearGF)}");
							if (stg.Past != -1)
								sw.WriteLine($"Past -> {GetStageName(stg.Past)}");
							if (stg.GoodFuture != -1)
							{
								sw.WriteLine($"Bad Future -> {GetStageName(stg.Future)}");
								sw.WriteLine($"Good Future -> {GetStageName(stg.GoodFuture)}");
							}
							else if (stg.Future != -1)
								sw.WriteLine($"Future -> {GetStageName(stg.Future)}");
						}
						sw.WriteLine();
					}
				}
		}

		const int linespace = 8;
		private void makeChartButton_Click(object sender, EventArgs e)
		{
			if (saveFileDialog2.ShowDialog(this) != DialogResult.OK)
				return;
			ChartNode[] levels = new ChartNode[stagecount + 2];
			int gridmaxh = 0;
			int gridmaxv = 0;
			switch (settings.Mode)
			{
				case Modes.AllStagesWarps: // stages + warps
				case Modes.Wild: // wild
					gridmaxh = 1;
					gridmaxv = stagecount + 2;
					for (int i = 0; i <= stagecount; i++)
						levels[stageids[i]] = new ChartNode(0, i + 1);
					levels[stagecount + 1] = new ChartNode(0, 0);
					break;
				case Modes.BranchingPaths: // branching paths
					{
						int row = 0;
						int col = 0;
						int nextrow = stageids[0];
						for (int i = 0; i < stagecount; i++)
						{
							if (stageids[i] == nextrow)
							{
								++row;
								col = 0;
								nextrow = stages[stageids[i]].Clear;
							}
							levels[stageids[i]] = new ChartNode(col++, row);
							gridmaxh = Math.Max(col, gridmaxh);
						}
						levels[stagecount] = new ChartNode(0, ++row);
						gridmaxv = row + 1;
						levels[stagecount + 1] = new ChartNode(0, 0);
					}
					break;
				case Modes.Segments: // segments
					{
						gridmaxh = 2;
						int row = 0;
						for (int i = 0; i < stagecount; i++)
						{
							if (i % 10 == 8)
								levels[stageids[i]] = new ChartNode(1, row + 1);
							else
								levels[stageids[i]] = new ChartNode(0, ++row);
						}
						levels[stagecount] = new ChartNode(0, ++row);
						gridmaxv = row + 1;
						levels[stagecount + 1] = new ChartNode(0, 0);
					}
					break;
				case Modes.Shadow: // shadow
					{
						gridmaxh = 1;
						gridmaxv = 11;
						int[] stgcnts = { 1, 3, 3, 5, 5, 5, 0 };
						int[][] bosses = { new int[] { }, new[] { 8 }, new[] { 4 }, new[] { 4, 8, 10 }, new[] { 2, 6 }, new int[] { }, new[] { 0, 1, 5, 9, 10 } };
						int ind = 0;
						for (int i = 0; i < stgcnts.Length; i++)
						{
							int y = gridmaxv / 4 - stgcnts[i] / 2;
							for (int j = 0; j < stgcnts[i]; j++)
								levels[stageids[ind++]] = new ChartNode(gridmaxh, y++ * 2 + 1);
							if (bosses[i].Length > 0)
								for (int j = 0; j < bosses[i].Length; j++)
									levels[stageids[ind++]] = new ChartNode(gridmaxh, bosses[i][j]);
							gridmaxh++;
						}
						for (; ind < stagecount; ind++)
							levels[stageids[ind]] = new ChartNode(gridmaxh++, 5);
						levels[stagecount] = new ChartNode(gridmaxh++, 5);
						levels[stagecount + 1] = new ChartNode(0, 5);
					}
					break;
				default: // normal game structure
					{
						gridmaxh = 4;
						int row = 0;
						for (int i = 0; i < stagecount; i++)
							switch (i % 10)
							{
								case 0:
								case 4:
									levels[stageids[i]] = new ChartNode(1, ++row);
									break;
								case 1:
								case 5:
									levels[stageids[i]] = new ChartNode(0, row);
									break;
								case 2:
								case 6:
									levels[stageids[i]] = new ChartNode(2, row);
									break;
								case 3:
								case 7:
								case 9:
									levels[stageids[i]] = new ChartNode(3, row);
									break;
								case 8:
									levels[stageids[i]] = new ChartNode(2, ++row);
									break;
							}
						gridmaxv = row + 2;
						levels[stagecount] = new ChartNode(3, row + 1);
						levels[stagecount + 1] = new ChartNode(1, 0);
					}
					break;
			}
			levels[stagecount + 1].Connect(ConnectionType.Clear, levels[stageids[0]]);
			for (int i = 0; i < stagecount; i++)
			{
				ChartNode node = levels[i];
				Stage stage = stages[i];
				if (stage.Clear != -1)
					node.Connect(ConnectionType.Clear, levels[stage.Clear]);
				if (stage.Past != -1)
					node.Connect(ConnectionType.Past, levels[stage.Past]);
				if (stage.GoodFuture != -1)
				{
					node.Connect(ConnectionType.Future, levels[stage.GoodFuture]);
					node.Connect(ConnectionType.BadFuture, levels[stage.Future]);
				}
				else if (stage.Future != -1)
					node.Connect(ConnectionType.Future, levels[stage.Future]);
				if (stage.ClearGF != -1)
					node.Connect(ConnectionType.ClearGF, levels[stage.ClearGF]);
			}
			Size textsz = Size.Empty;
			using (var g = CreateGraphics())
			{
				foreach (string item in RoundNames)
				{
					Size tmpsz = g.MeasureString($"{item}\nZone 3 Good Future", DefaultFont).ToSize();
					if (tmpsz.Width > textsz.Width)
						textsz.Width = tmpsz.Width;
					if (tmpsz.Height > textsz.Height)
						textsz.Height = tmpsz.Height;
				}
				textsz.Width += 6;
				textsz.Height += 6;
			}
			List<(ChartNode src, ChartConnection con)> shortcons = new List<(ChartNode src, ChartConnection con)>();
			List<ChartConnection>[] vcons = new List<ChartConnection>[gridmaxh * 2];
			for (int i = 0; i < gridmaxh * 2; i++)
				vcons[i] = new List<ChartConnection>();
			List<ChartConnection>[] hcons = new List<ChartConnection>[gridmaxv * 2];
			for (int i = 0; i < gridmaxv * 2; i++)
				hcons[i] = new List<ChartConnection>();
			foreach (var item in levels)
			{
				textsz.Height = Math.Max((item.OutgoingConnections[Direction.Left].Count + item.IncomingConnections[Direction.Left].Count) * linespace, textsz.Height);
				textsz.Width = Math.Max((item.OutgoingConnections[Direction.Top].Count + item.IncomingConnections[Direction.Top].Count) * linespace, textsz.Width);
				textsz.Height = Math.Max((item.OutgoingConnections[Direction.Right].Count + item.IncomingConnections[Direction.Right].Count) * linespace, textsz.Height);
				textsz.Width = Math.Max((item.OutgoingConnections[Direction.Bottom].Count + item.IncomingConnections[Direction.Bottom].Count) * linespace, textsz.Width);
				shortcons.AddRange(item.OutgoingConnections.SelectMany(a => a.Value).Where(a => item.GetDistance(a.Node) == 1).Select(a => (item, a)));
				vcons[item.GridX * 2].AddRange(item.IncomingConnections[Direction.Left].Where(a => a.Distance != 1));
				vcons[item.GridX * 2 + 1].AddRange(item.IncomingConnections[Direction.Right].Where(a => a.Distance != 1));
				if (item.GridY > 0)
					hcons[item.GridY * 2 - 1].AddRange(item.IncomingConnections[Direction.Top].Where(a => a.Distance != 1 && a.MinY == item.GridY - 1));
				hcons[item.GridY * 2].AddRange(item.IncomingConnections[Direction.Top].Where(a => a.Distance != 1 && a.MinY != item.GridY - 1));
				hcons[item.GridY * 2 + 1].AddRange(item.IncomingConnections[Direction.Bottom].Where(a => a.Distance != 1));
			}
			int conslotsh = textsz.Height / linespace;
			int conslotsv = textsz.Width / linespace;
			int hconoff = (textsz.Height - (conslotsh * linespace)) / 2;
			int vconoff = (textsz.Width - (conslotsv * linespace)) / 2;
			foreach (var item in levels)
			{
				item.ConnectionOrder[Direction.Left] = new ChartConnection[conslotsh];
				item.ConnectionOrder[Direction.Top] = new ChartConnection[conslotsv];
				item.ConnectionOrder[Direction.Right] = new ChartConnection[conslotsh];
				item.ConnectionOrder[Direction.Bottom] = new ChartConnection[conslotsv];
			}
			foreach (var (src, con) in shortcons)
			{
				ChartConnection[] srcord = src.ConnectionOrder[src.OutgoingConnections.First(a => a.Value.Contains(con)).Key];
				ChartConnection[] dstord = con.Node.ConnectionOrder[con.Side];
				int mid = srcord.Length / 2;
				int slot = mid;
				while (slot < srcord.Length && (srcord[slot] != null || dstord[slot] != null))
					++slot;
				if (slot == srcord.Length)
				{
					slot = mid - 1;
					while (srcord[slot] != null || dstord[slot] != null)
						--slot;
				}
				srcord[slot] = con;
				dstord[slot] = con;
			}
			foreach (var item in levels)
			{
				int preslots = Array.FindIndex(item.ConnectionOrder[Direction.Left], a => a != null);
				int postslots;
				List<ChartConnection> prelist = new List<ChartConnection>();
				List<ChartConnection> postlist = new List<ChartConnection>();
				if (preslots == -1)
				{
					prelist.AddRange(item.IncomingConnections[Direction.Left]);
					prelist.AddRange(item.OutgoingConnections[Direction.Left]);
					prelist.Sort(CompareConnV);
					prelist.CopyTo(item.ConnectionOrder[Direction.Left], (item.ConnectionOrder[Direction.Left].Length - prelist.Count) / 2);
				}
				else
				{
					postslots = item.ConnectionOrder[Direction.Left].Length - Array.FindIndex(item.ConnectionOrder[Direction.Left], preslots, a => a == null);
					foreach (var con in item.IncomingConnections[Direction.Left].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Left], a) == -1))
					{
						if (con.MaxY == item.GridY)
							prelist.Add(con);
						else if (con.MinY == item.GridY)
							postlist.Add(con);
						else if (Math.Abs(con.MinY - item.GridY) > Math.Abs(con.MaxY - item.GridY))
							prelist.Add(con);
						else
							postlist.Add(con);
					}
					foreach (var con in item.OutgoingConnections[Direction.Left].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Left], a) == -1))
					{
						if (con.MinY == item.GridY)
							postlist.Add(con);
						else if (con.MaxY == item.GridY)
							prelist.Add(con);
						else if (Math.Abs(con.MinY - item.GridY) > Math.Abs(con.MaxY - item.GridY))
							prelist.Add(con);
						else
							postlist.Add(con);
					}
					if (prelist.Count > 0 || postlist.Count > 0)
					{
						prelist.Sort(CompareConnV);
						postlist.Sort(CompareConnV);
						if (prelist.Count > preslots)
						{
							postlist.InsertRange(0, prelist.Skip(preslots));
							prelist.RemoveRange(preslots, prelist.Count - preslots);
						}
						else if (postlist.Count > postslots)
						{
							prelist.AddRange(postlist.Take(postlist.Count - postslots));
							postlist.RemoveRange(0, postlist.Count - postslots);
						}
						prelist.CopyTo(item.ConnectionOrder[Direction.Left], preslots - prelist.Count);
						postlist.CopyTo(item.ConnectionOrder[Direction.Left], item.ConnectionOrder[Direction.Left].Length - postslots);
					}
				}
				preslots = Array.FindIndex(item.ConnectionOrder[Direction.Top], a => a != null);
				prelist.Clear();
				postlist.Clear();
				if (preslots == -1)
				{
					prelist.AddRange(item.IncomingConnections[Direction.Top]);
					prelist.AddRange(item.OutgoingConnections[Direction.Top]);
					prelist.Sort(CompareConnV);
					prelist.CopyTo(item.ConnectionOrder[Direction.Top], (item.ConnectionOrder[Direction.Top].Length - prelist.Count) / 2);
				}
				else
				{
					postslots = item.ConnectionOrder[Direction.Top].Length - Array.FindIndex(item.ConnectionOrder[Direction.Top], preslots, a => a == null);
					foreach (var con in item.IncomingConnections[Direction.Top].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Top], a) == -1))
					{
						if (con.MaxX == item.GridX)
							prelist.Add(con);
						else if (con.MinX == item.GridX)
							postlist.Add(con);
						else if (Math.Abs(con.MinX - item.GridX) > Math.Abs(con.MaxX - item.GridX))
							prelist.Add(con);
						else
							postlist.Add(con);
					}
					foreach (var con in item.OutgoingConnections[Direction.Top].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Top], a) == -1))
					{
						if (con.MinX == item.GridX)
							postlist.Add(con);
						else if (con.MaxX == item.GridX)
							prelist.Add(con);
						else if (Math.Abs(con.MinX - item.GridX) > Math.Abs(con.MaxX - item.GridX))
							prelist.Add(con);
						else
							postlist.Add(con);
					}
					if (prelist.Count > 0 || postlist.Count > 0)
					{
						prelist.Sort(CompareConnH);
						postlist.Sort(CompareConnH);
						if (prelist.Count > preslots)
						{
							postlist.InsertRange(0, prelist.Skip(preslots));
							prelist.RemoveRange(preslots, prelist.Count - preslots);
						}
						else if (postlist.Count > postslots)
						{
							prelist.AddRange(postlist.Take(postlist.Count - postslots));
							postlist.RemoveRange(0, postlist.Count - postslots);
						}
						prelist.CopyTo(item.ConnectionOrder[Direction.Top], preslots - prelist.Count);
						postlist.CopyTo(item.ConnectionOrder[Direction.Top], item.ConnectionOrder[Direction.Top].Length - postslots);
					}
				}
				preslots = Array.FindIndex(item.ConnectionOrder[Direction.Right], a => a != null);
				prelist.Clear();
				postlist.Clear();
				if (preslots == -1)
				{
					prelist.AddRange(item.IncomingConnections[Direction.Right]);
					prelist.AddRange(item.OutgoingConnections[Direction.Right]);
					prelist.Sort(CompareConnV);
					prelist.CopyTo(item.ConnectionOrder[Direction.Right], (item.ConnectionOrder[Direction.Right].Length - prelist.Count) / 2);
				}
				else
				{
					postslots = item.ConnectionOrder[Direction.Right].Length - Array.FindIndex(item.ConnectionOrder[Direction.Right], preslots, a => a == null);
					foreach (var con in item.IncomingConnections[Direction.Right].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Right], a) == -1))
					{
						if (con.MaxY == item.GridY)
							prelist.Add(con);
						else if (con.MinY == item.GridY)
							postlist.Add(con);
						else if (Math.Abs(con.MinY - item.GridY) > Math.Abs(con.MaxY - item.GridY))
							prelist.Add(con);
						else
							postlist.Add(con);
					}
					foreach (var con in item.OutgoingConnections[Direction.Right].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Right], a) == -1))
					{
						if (con.MinY == item.GridY)
							postlist.Add(con);
						else if (con.MaxY == item.GridY)
							prelist.Add(con);
						else if (Math.Abs(con.MinY - item.GridY) > Math.Abs(con.MaxY - item.GridY))
							prelist.Add(con);
						else
							postlist.Add(con);
					}
					if (prelist.Count > 0 || postlist.Count > 0)
					{
						prelist.Sort(CompareConnV);
						postlist.Sort(CompareConnV);
						if (prelist.Count > preslots)
						{
							postlist.InsertRange(0, prelist.Skip(preslots));
							prelist.RemoveRange(preslots, prelist.Count - preslots);
						}
						else if (postlist.Count > postslots)
						{
							prelist.AddRange(postlist.Take(postlist.Count - postslots));
							postlist.RemoveRange(0, postlist.Count - postslots);
						}
						prelist.CopyTo(item.ConnectionOrder[Direction.Right], preslots - prelist.Count);
						postlist.CopyTo(item.ConnectionOrder[Direction.Right], item.ConnectionOrder[Direction.Right].Length - postslots);
					}
				}
				preslots = Array.FindIndex(item.ConnectionOrder[Direction.Bottom], a => a != null);
				prelist.Clear();
				postlist.Clear();
				if (preslots == -1)
				{
					prelist.AddRange(item.IncomingConnections[Direction.Bottom]);
					prelist.AddRange(item.OutgoingConnections[Direction.Bottom]);
					prelist.Sort(CompareConnV);
					prelist.CopyTo(item.ConnectionOrder[Direction.Bottom], (item.ConnectionOrder[Direction.Bottom].Length - prelist.Count) / 2);
				}
				else
				{
					postslots = item.ConnectionOrder[Direction.Bottom].Length - Array.FindIndex(item.ConnectionOrder[Direction.Bottom], preslots, a => a == null);
					foreach (var con in item.IncomingConnections[Direction.Bottom].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Bottom], a) == -1))
					{
						if (con.MaxX == item.GridX)
							prelist.Add(con);
						else if (con.MinX == item.GridX)
							postlist.Add(con);
						else if (Math.Abs(con.MinX - item.GridX) > Math.Abs(con.MaxX - item.GridX))
							prelist.Add(con);
						else
							postlist.Add(con);
					}
					foreach (var con in item.OutgoingConnections[Direction.Bottom].Where(a => Array.IndexOf(item.ConnectionOrder[Direction.Bottom], a) == -1))
					{
						if (con.MinX == item.GridX)
							postlist.Add(con);
						else if (con.MaxX == item.GridX)
							prelist.Add(con);
						else if (Math.Abs(con.MinX - item.GridX) > Math.Abs(con.MaxX - item.GridX))
							prelist.Add(con);
						else
							postlist.Add(con);
					}
					if (prelist.Count > 0 || postlist.Count > 0)
					{
						prelist.Sort(CompareConnH);
						postlist.Sort(CompareConnH);
						if (prelist.Count > preslots)
						{
							postlist.InsertRange(0, prelist.Skip(preslots));
							prelist.RemoveRange(preslots, prelist.Count - preslots);
						}
						else if (postlist.Count > postslots)
						{
							prelist.AddRange(postlist.Take(postlist.Count - postslots));
							postlist.RemoveRange(0, postlist.Count - postslots);
						}
						prelist.CopyTo(item.ConnectionOrder[Direction.Bottom], preslots - prelist.Count);
						postlist.CopyTo(item.ConnectionOrder[Direction.Bottom], item.ConnectionOrder[Direction.Bottom].Length - postslots);
					}
				}
			}
			int vlanemax = 0;
			foreach (var list in vcons)
			{
				list.Sort((a, b) =>
				{
					int r = a.Distance.CompareTo(b.Distance);
					if (r == 0)
					{
						r = a.MinY.CompareTo(b.MinY);
						if (r == 0)
							r = a.Type.CompareTo(b.Type);
					}
					return r;
				});
				for (int i = 0; i < list.Count; i++)
				{
					var line = list[i];
					for (int j = 0; j < i; j++)
						if (list[j].Lane == line.Lane && line.MaxY >= list[j].MinY && list[j].MaxY >= line.MinY)
						{
							line.Lane++;
							j = -1;
						}
					vlanemax = Math.Max(line.Lane + 1, vlanemax);
				}
			}
			int hlanemax = 0;
			foreach (var list in hcons)
			{
				list.Sort((a, b) =>
				{
					int r = a.Distance.CompareTo(b.Distance);
					if (r == 0)
					{
						r = a.MinX.CompareTo(b.MinX);
						if (r == 0)
							r = a.Type.CompareTo(b.Type);
					}
					return r;
				});
				for (int i = 0; i < list.Count; i++)
				{
					var line = list[i];
					for (int j = 0; j < i; j++)
						if (list[j].Lane == line.Lane && line.MaxX >= list[j].MinX && list[j].MaxX >= line.MinX)
						{
							line.Lane++;
							j = -1;
						}
					hlanemax = Math.Max(line.Lane + 1, hlanemax);
				}
			}
			int margin = Math.Min(textsz.Width / 2, textsz.Height / 2);
			int hmargin = Math.Max(vlanemax * linespace + 5, margin);
			int vmargin = Math.Max(hlanemax * linespace + 5, margin);
			int colwidth = textsz.Width + hmargin * 2;
			int rowheight = textsz.Height + vmargin * 2;
			using (Bitmap bmp = new Bitmap(colwidth * gridmaxh, rowheight * gridmaxv))
			{
				using (Graphics gfx = Graphics.FromImage(bmp))
				{
					gfx.Clear(Color.White);
					List<int> stageorder = new List<int>(stagecount + 2);
					stageorder.Add(stagecount + 1);
					stageorder.AddRange(stageids);
					stageorder.Reverse();
					StringFormat fmt = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
					Pen pen = new Pen(Color.Black, 3) { EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor };
					foreach (var id in stageorder)
					{
						var node = levels[id];
						int x = colwidth * node.GridX + hmargin;
						int y = rowheight * node.GridY + vmargin;
						gfx.DrawRectangle(Pens.Black, x, y, textsz.Width, textsz.Height);
						string str;
						if (id == stagecount + 1)
							str = "Start";
						else if (id == stagecount)
							str = "Ending";
						else
							str = $"{RoundNames[id / 10]}\n{ZoneNames[id % 10]}";
						gfx.DrawString(str, DefaultFont, Brushes.Black, new RectangleF(x, y, textsz.Width, textsz.Height), fmt);
						foreach (var (dir, list) in node.OutgoingConnections)
							foreach (var con in list)
							{
								int srclane = Array.LastIndexOf(node.ConnectionOrder[dir], con);
								int srcx = 0;
								int srcy = 0;
								switch (dir)
								{
									case Direction.Left:
										srcx = x;
										srcy = y + hconoff + (srclane * linespace) + (linespace / 2);
										break;
									case Direction.Top:
										srcx = x + vconoff + (srclane * linespace) + (linespace / 2);
										srcy = y;
										break;
									case Direction.Right:
										srcx = x + textsz.Width + 1;
										srcy = y + hconoff + (srclane * linespace) + (linespace / 2);
										break;
									case Direction.Bottom:
										srcx = x + vconoff + (srclane * linespace) + (linespace / 2);
										srcy = y + textsz.Height + 1;
										break;
								}
								int dstlane = Array.IndexOf(con.Node.ConnectionOrder[con.Side], con);
								int dstx = colwidth * con.Node.GridX + hmargin;
								int dsty = rowheight * con.Node.GridY + vmargin;
								switch (con.Side)
								{
									case Direction.Left:
										dsty += hconoff + (dstlane * linespace) + (linespace / 2);
										break;
									case Direction.Top:
										dstx += vconoff + (dstlane * linespace) + (linespace / 2);
										break;
									case Direction.Right:
										dstx += textsz.Width + 1;
										dsty += hconoff + (dstlane * linespace) + (linespace / 2);
										break;
									case Direction.Bottom:
										dstx += vconoff + (dstlane * linespace) + (linespace / 2);
										dsty += textsz.Height + 1;
										break;
								}
								switch (con.Type)
								{
									case ConnectionType.Clear:
										pen.Color = Color.Black;
										break;
									case ConnectionType.Past:
										pen.Color = Color.Blue;
										break;
									case ConnectionType.Future:
										if (settings.Mode == Modes.Shadow && node.GridX < 7)
											pen.Color = Color.Red;
										else
											pen.Color = Color.Lime;
										break;
									case ConnectionType.BadFuture:
										pen.Color = Color.Red;
										break;
									case ConnectionType.ClearGF:
										pen.Color = Color.Fuchsia;
										break;
								}
								if (con.MaxX - con.MinX == 1 || con.MaxY - con.MinY == 1)
									pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
								else
									pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
								if (node.GetDistance(con.Node) == 1)
									gfx.DrawLine(pen, srcx, srcy, dstx, dsty);
								else
								{
									var path = new System.Drawing.Drawing2D.GraphicsPath();
									int midx = srcx;
									int midy = srcy;
									switch (dir)
									{
										case Direction.Left:
											midx -= con.Lane * linespace + (linespace / 2) + 5;
											break;
										case Direction.Top:
											midy -= con.Lane * linespace + (linespace / 2) + 5;
											break;
										case Direction.Right:
											midx += con.Lane * linespace + (linespace / 2) + 5;
											break;
										case Direction.Bottom:
											midy += con.Lane * linespace + (linespace / 2) + 5;
											break;
									}
									path.AddLine(srcx, srcy, midx, midy);
									switch (dir)
									{
										case Direction.Left:
										case Direction.Right:
											path.AddLine(midx, midy, midx, dsty);
											path.AddLine(midx, dsty, dstx, dsty);
											break;
										case Direction.Top:
										case Direction.Bottom:
											path.AddLine(midx, midy, dstx, midy);
											path.AddLine(dstx, midy, dstx, dsty);
											break;
									}
									gfx.DrawPath(pen, path);
								}
							}
					}
				}
				bmp.Save(saveFileDialog2.FileName);
			}
		}

		private static int CompareConnV(ChartConnection a, ChartConnection b)
		{
			int r = a.MinY.CompareTo(b.MinY);
			if (r == 0)
				r = a.MaxY.CompareTo(b.MaxY);
			return r;
		}

		private static int CompareConnH(ChartConnection a, ChartConnection b)
		{
			int r = a.MinX.CompareTo(b.MinX);
			if (r == 0)
				r = a.MaxX.CompareTo(b.MaxX);
			return r;
		}
	}

	static class Extensions
	{
		public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
		{
			key = kvp.Key;
			value = kvp.Value;
		}
	}

	class VirtualDirectory
	{
		class DirectoryCollection : System.Collections.ObjectModel.KeyedCollection<string, VirtualDirectory>
		{
			protected override string GetKeyForItem(VirtualDirectory item) => item.Name;
		}

		class FileCollection : System.Collections.ObjectModel.KeyedCollection<string, VirtualFile>
		{
			protected override string GetKeyForItem(VirtualFile item) => item.Name;
		}

		public string Name { get; }
		public string FullName { get; }

		readonly DirectoryCollection directories = new DirectoryCollection();
		readonly FileCollection files = new FileCollection();
		
		public VirtualDirectory(string name)
		{
			Name = name;
			FullName = name;
		}

		private VirtualDirectory(string name, string fullname)
		{
			Name = name;
			FullName = fullname;
		}

		public override string ToString()
		{
			return Name;
		}

		public VirtualDirectory AddDirectory(string path)
		{
			int dirind = path.IndexOfAny(new[] { '\\', '/' });
			if (dirind != -1)
			{
				string dir = path.Substring(0, dirind);
				VirtualDirectory vdir;
				if (!directories.Contains(dir))
					vdir = AddDirectory(dir);
				else
					vdir = directories[dir];
				return vdir.AddDirectory(path.Substring(dirind + 1));
			}
			if (directories.Contains(path))
				return directories[path];
			var vd = new VirtualDirectory(path, FullName + '/' + path);
			directories.Add(vd);
			return vd;
		}

		public VirtualFile AddFile(string path, string source)
		{
			int dirind = path.IndexOfAny(new[] { '\\', '/' });
			if (dirind != -1)
			{
				string dir = path.Substring(0, dirind);
				if (!directories.Contains(dir))
					return AddDirectory(dir).AddFile(path.Substring(dirind + 1), source);
				return directories[dir].AddFile(path.Substring(dirind + 1), source);
			}
			if (files.Contains(path))
			{
				var f = files[path];
				f.SourcePath = source;
				return f;
			}
			var vf = new VirtualFile(path, FullName + '/' + path, source);
			files.Add(vf);
			return vf;
		}

		public VirtualDirectory GetDirectory(string path)
		{
			int dirind = path.IndexOfAny(new[] { '\\', '/' });
			if (dirind != -1)
				return directories[path.Substring(0, dirind)].GetDirectory(path.Substring(dirind + 1));
			return directories[path];
		}

		public VirtualFile GetFile(string path)
		{
			int dirind = path.IndexOfAny(new[] { '\\', '/' });
			if (dirind != -1)
				return directories[path.Substring(0, dirind)].GetFile(path.Substring(dirind + 1));
			return files[path];
		}

		public bool DirectoryExists(string path)
		{
			int dirind = path.IndexOfAny(new[] { '\\', '/' });
			if (dirind != -1)
			{
				string dir = path.Substring(0, dirind);
				if (!directories.Contains(dir))
					return false;
				return directories[dir].DirectoryExists(path.Substring(dirind + 1));
			}
			return directories.Contains(path);
		}

		public bool FileExists(string path)
		{
			int dirind = path.IndexOfAny(new[] { '\\', '/' });
			if (dirind != -1)
			{
				string dir = path.Substring(0, dirind);
				if (!directories.Contains(dir))
					return false;
				return directories[dir].FileExists(path.Substring(dirind + 1));
			}
			return files.Contains(path);
		}

		public IEnumerable<VirtualDirectory> EnumerateDirectories() => directories;

		public IEnumerable<VirtualFile> EnumerateFiles() => files;

		public IEnumerable<VirtualFile> GetAllFiles()
		{
			foreach (var file in files)
				yield return file;
			foreach (var dir in directories)
				foreach (var file in dir.GetAllFiles())
					yield return file;
		}

		public void ScanDirectory(string dir) => ScanDirectory(new DirectoryInfo(dir));

		public void ScanDirectory(DirectoryInfo dir)
		{
			foreach (var item in dir.EnumerateDirectories())
			{
				VirtualDirectory vdir;
				if (directories.Contains(item.Name))
					vdir = directories[item.Name];
				else
				{
					vdir = new VirtualDirectory(item.Name, FullName + '/' + item.Name);
					directories.Add(vdir);
				}
				vdir.ScanDirectory(item);
			}
			foreach (var item in dir.EnumerateFiles())
			{
				if (files.Contains(item.Name))
					files[item.Name].SourcePath = item.FullName;
				else
					files.Add(new VirtualFile(item.Name, FullName + '/' + item.Name, item.FullName));
			}
		}
	}

	class VirtualFile
	{
		public string Name { get; }
		public string FullName { get; }
		public string SourcePath { get; set; }

		public VirtualFile(string name, string fullname, string source)
		{
			Name = name;
			FullName = fullname;
			SourcePath = source;
		}

		public override string ToString()
		{
			return Name;
		}
	}

	class Stage
	{
		public int ID { get; }
		public int Round { get; }
		public int Act { get; }
		public TimePeriods TimePeriod { get; }
		public bool HasPastPost { get; set; }
		public bool HasFuturePost { get; set; }
		public int Clear { get; set; } = -1;
		public int ClearGF { get; set; } = -1;
		public int Past { get; set; } = -1;
		public int Future { get; set; } = -1;
		public int GoodFuture { get; set; } = -1;

		public Stage(int id)
		{
			ID = id;
			Round = Math.DivRem(id, 10, out int rem);
			Act = Math.DivRem(rem, 4, out rem);
			switch (Act)
			{
				case 2:
					TimePeriod = (TimePeriods)(rem + 2);
					break;
				default:
					TimePeriod = (TimePeriods)rem;
					switch (TimePeriod)
					{
						case TimePeriods.Present:
							HasPastPost = true;
							HasFuturePost = true;
							break;
						case TimePeriods.Past:
							HasFuturePost = true;
							break;
						case TimePeriods.GoodFuture:
						case TimePeriods.BadFuture:
							HasPastPost = true;
							break;
					}
					break;
			}
		}
	}

	enum TimePeriods { Present, Past, GoodFuture, BadFuture }

	readonly struct ShadowStageSet
	{
		public readonly System.Collections.ObjectModel.ReadOnlyCollection<ShadowStage> stages;
		public readonly System.Collections.ObjectModel.ReadOnlyCollection<int> bosses;

		public ShadowStageSet(params ShadowStage[] stages)
		{
			this.stages = new System.Collections.ObjectModel.ReadOnlyCollection<ShadowStage>(stages);
			List<int> tmp = new List<int>();
			foreach (var item in stages)
			{
				if (item.boss != -1 && !tmp.Contains(item.boss))
					tmp.Add(item.boss);
				if (item.boss2 != -1 && !tmp.Contains(item.boss2))
					tmp.Add(item.boss2);
			}
			bosses = new System.Collections.ObjectModel.ReadOnlyCollection<int>(tmp);
		}

		public static readonly ShadowStageSet[] StageList = new[]
		{
			new ShadowStageSet(new ShadowStage(TimePeriods.Present)),
			new ShadowStageSet(new ShadowStage(TimePeriods.BadFuture), new ShadowStage(TimePeriods.Present), new ShadowStage(TimePeriods.GoodFuture, 0)),
			new ShadowStageSet(new ShadowStage(TimePeriods.Present, 1), new ShadowStage(TimePeriods.Present), new ShadowStage(TimePeriods.Present)),
			new ShadowStageSet(new ShadowStage(TimePeriods.BadFuture), new ShadowStage(TimePeriods.Present, 2), new ShadowStage(TimePeriods.Present), new ShadowStage(TimePeriods.Present, 3), new ShadowStage(TimePeriods.GoodFuture, 4)),
			new ShadowStageSet(new ShadowStage(TimePeriods.BadFuture, 5), new ShadowStage(TimePeriods.Present), new ShadowStage(TimePeriods.Present, 6), new ShadowStage(TimePeriods.Present), new ShadowStage(TimePeriods.GoodFuture)),
			new ShadowStageSet(new ShadowStage(TimePeriods.BadFuture, 7, 8), new ShadowStage(TimePeriods.Past, 7, 9), new ShadowStage(TimePeriods.Past, 9, 9), new ShadowStage(TimePeriods.Past, 9, 10), new ShadowStage(TimePeriods.GoodFuture, 11, 10)),
		};
	}

	readonly struct ShadowStage
	{
		public readonly TimePeriods timePeriod;
		public readonly int boss;
		public readonly int boss2;

		public ShadowStage(TimePeriods timePeriod, int boss = -1, int boss2 = -1)
		{
			this.timePeriod = timePeriod;
			this.boss = boss;
			this.boss2 = boss2;
		}
	}

	class HueRotation
	{
		readonly double _0, _1, _2;

		const double sqrtonethird = 0.57735026918962573;
		const double max = 0.52359877559829893;

		public HueRotation(Random rand) : this(rand.Next(1, 12) * max) { }

		public HueRotation(double radians)
		{
			double cosA = Math.Cos(radians);
			double sinA = Math.Sin(radians);
			double onethirdoneminuscosA = (1.0 - cosA) / 3;
			double sqrtonethirdsinA = sqrtonethird * sinA;

			_0 = cosA + onethirdoneminuscosA;
			_1 = onethirdoneminuscosA - sqrtonethirdsinA;
			_2 = onethirdoneminuscosA + sqrtonethirdsinA;
		}

		private void ApplyRotationInternal(byte rin, byte gin, byte bin, out byte rout, out byte gout, out byte bout)
		{
			rout = Clamp(rin * _0 + gin * _1 + bin * _2);
			gout = Clamp(rin * _2 + gin * _0 + bin * _1);
			bout = Clamp(rin * _1 + gin * _2 + bin * _0);
		}

		public void ApplyRotation(RSDKv3.Palette palette)
		{
			foreach (var line in palette.colors)
				ApplyRotation(line);
		}

		public void ApplyRotation(RSDKv3.Palette.Color[] palette)
		{
			foreach (var color in palette)
			{
				ApplyRotationInternal(color.R, color.G, color.B, out byte r, out byte g, out byte b);
				color.R = r;
				color.G = g;
				color.B = b;
			}
		}

		public void ApplyRotation(byte[] palette)
		{
			for (int i = 0; i < palette.Length - 2; i += 3)
			{
				ApplyRotationInternal(palette[i], palette[i + 1], palette[i + 2], out byte r, out byte g, out byte b);
				palette[i] = r;
				palette[i + 1] = g;
				palette[i + 2] = b;
			}
		}

		static byte Clamp(double val)
		{
			int ival = (int)Math.Round(val, MidpointRounding.AwayFromZero);
			if (ival < 0)
				return 0;
			else if (ival > 255)
				return 255;
			return (byte)ival;
		}
	}

	class ChartNode
	{
		public int GridX { get; }
		public int GridY { get; }
		public Dictionary<Direction, List<ChartConnection>> OutgoingConnections { get; } = new Dictionary<Direction, List<ChartConnection>>();
		public Dictionary<Direction, List<ChartConnection>> IncomingConnections { get; } = new Dictionary<Direction, List<ChartConnection>>();
		public Dictionary<Direction, ChartConnection[]> ConnectionOrder { get; } = new Dictionary<Direction, ChartConnection[]>();
		public ChartNode(int x, int y)
		{
			GridX = x;
			GridY = y;
			foreach (var item in Enum.GetValues(typeof(Direction)).Cast<Direction>())
			{
				OutgoingConnections.Add(item, new List<ChartConnection>());
				IncomingConnections.Add(item, new List<ChartConnection>());
			}
		}

		public void Connect(ConnectionType color, ChartNode dest)
		{
			Direction outdir, indir;
			int xdiff = GridX - dest.GridX;
			int ydiff = GridY - dest.GridY;
			if (ydiff == -1)
			{
				outdir = Direction.Bottom;
				indir = Direction.Top;
			}
			else if (ydiff == 1)
			{
				outdir = Direction.Top;
				indir = Direction.Bottom;
			}
			else if (xdiff == 0)
			{
				if (ydiff < 1)
				{
					outdir = Direction.Right;
					indir = Direction.Right;
				}
				else
				{
					outdir = Direction.Left;
					indir = Direction.Left;
				}
			}
			else if (ydiff == 0 && (xdiff < -1 || xdiff > 1))
			{
				outdir = Direction.Top;
				indir = Direction.Top;
			}
			else if (xdiff < 0)
			{
				outdir = Direction.Right;
				indir = Direction.Left;
			}
			else
			{
				outdir = Direction.Left;
				indir = Direction.Right;
			}
			ChartConnection c = dest.IncomingConnections[indir].Find(a => a.Type == color);
			if (c == null)
			{
				c = new ChartConnection(indir, color, this, dest);
				dest.IncomingConnections[indir].Add(c);
			}
			else
				c.AddSource(this, dest);
			OutgoingConnections[outdir].Add(c);
		}

		public int GetDistance(ChartNode other) => Math.Abs(GridX - other.GridX) + Math.Abs(GridY - other.GridY);
	}

	class ChartConnection
	{
		public ChartNode Node { get; }
		public Direction Side { get; }
		public ConnectionType Type { get; }
		public List<ChartNode> Sources { get; }
		public int MinX { get; private set; }
		public int MinY { get; private set; }
		public int MaxX { get; private set; }
		public int MaxY { get; private set; }
		public int Distance { get; private set; }
		public int Lane { get; set; }

		public ChartConnection(Direction side, ConnectionType type, ChartNode src, ChartNode dst)
		{
			Node = dst;
			Side = side;
			Type = type;
			Sources = new List<ChartNode>() { src };
			MinX = Math.Min(src.GridX, dst.GridX);
			MinY = Math.Min(src.GridY, dst.GridY);
			MaxX = Math.Max(src.GridX, dst.GridX);
			MaxY = Math.Max(src.GridY, dst.GridY);
			Distance = src.GetDistance(dst);
		}

		public void AddSource(ChartNode src, ChartNode dst)
		{
			Sources.Add(src);
			MinX = Math.Min(src.GridX, MinX);
			MinY = Math.Min(src.GridY, MinY);
			MaxX = Math.Max(src.GridX, MaxX);
			MaxY = Math.Max(src.GridY, MaxY);
			Distance = Math.Max(src.GetDistance(dst), Distance);
		}
	}

	enum ConnectionType { Clear, Past, Future, BadFuture, ClearGF }

	enum Direction { Left, Top, Right, Bottom }

	class MusicScript
	{
		public string ScriptPath { get; set; }
		public string Script { get; set; }
		public MatchCollection Matches { get; set; }

		public MusicScript(string path, string script, MatchCollection matches)
		{
			ScriptPath = path;
			Script = script;
			Matches = matches;
		}
	}

	class ModLoaderInfo
	{
		[IniCollection(IniCollectionMode.NoSquareBrackets, StartIndex = 1)]
		[IniName("Mod")]
		public List<string> Mods { get; set; }
	}

	class DecompModInfo
	{
		[IniName("mods")]
		public DecompModList Mods { get; set; }
	}

	class DecompModList
	{
		[IniCollection(IniCollectionMode.IndexOnly)]
		public Dictionary<string, bool> Mods { get; set; }
	}

	class Settings
	{
		[IniAlwaysInclude]
		public int Seed { get; set; }
		[IniAlwaysInclude]
		public bool RandomSeed { get; set; }
		[IniAlwaysInclude]
		public Modes Mode { get; set; }
		[IniAlwaysInclude]
		public MainPath MainPath { get; set; }
		[System.ComponentModel.DefaultValue(69)]
		[IniAlwaysInclude]
		public int MaxBackJump { get; set; } = 69;
		[System.ComponentModel.DefaultValue(70)]
		[IniAlwaysInclude]
		public int MaxForwJump { get; set; } = 70;
		[System.ComponentModel.DefaultValue(50)]
		[IniAlwaysInclude]
		public int BackJumpProb { get; set; } = 50;
		[IniAlwaysInclude]
		public bool AllowSameLevel { get; set; }
		[IniAlwaysInclude]
		public bool RandomMusic { get; set; }
		[IniAlwaysInclude]
		public bool SeparateSoundtracks { get; set; }
		[IniAlwaysInclude]
		public ItemMode RandomItems { get; set; }
		[IniAlwaysInclude]
		public bool RandomTimePosts { get; set; }
		[IniAlwaysInclude]
		public bool ReplaceCheckpoints { get; set; }
		[IniAlwaysInclude]
		public bool RandomPalettes { get; set; }
		[IniAlwaysInclude]
		public bool RandomUFOs { get; set; }
		[IniAlwaysInclude]
		public UFODifficulty UFODifficulty { get; set; }
		[IniAlwaysInclude]
		public bool RandomWater { get; set; }
		[IniAlwaysInclude]
		public bool AddWaterOnly { get; set; }

		public static Settings Load()
		{
			if (File.Exists("RandoSettings.ini"))
				return IniSerializer.Deserialize<Settings>("RandoSettings.ini");
			return new Settings();
		}

		public void Save()
		{
			IniSerializer.Serialize(this, "RandoSettings.ini");
		}
	}

	enum Modes
	{
		AllStagesWarps,
		Rounds,
		Acts,
		TimePeriods,
		BranchingPaths,
		Segments,
		Wild,
		Shadow
	}

	enum MainPath
	{
		ActClear,
		TimeTravel,
		AnyExit
	}

	enum ItemMode
	{
		Off,
		OneToOne,
		OneToOnePerStage,
		Wild
	}

	enum UFODifficulty
	{
		Easy,
		Medium,
		Hard,
		Wild
	}
}
