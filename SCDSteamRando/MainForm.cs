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
			}
			settings = Settings.Load();
			seedSelector.Value = settings.Seed;
			randomSeed.Checked = settings.RandomSeed;
			modeSelector.SelectedIndex = settings.Mode;
			mainPathSelector.SelectedIndex = settings.MainPath;
			maxBackJump.Value = settings.MaxBackJump;
			maxForwJump.Value = settings.MaxForwJump;
			backJumpProb.Value = settings.BackJumpProb;
			allowSameLevel.Checked = settings.AllowSameLevel;
			if (randomMusic.Enabled)
				randomMusic.Checked = settings.RandomMusic;
			separateSoundtracks.Checked = settings.SeparateSoundtracks;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			settings.Seed = (int)seedSelector.Value;
			settings.RandomSeed = randomSeed.Checked;
			settings.Mode = modeSelector.SelectedIndex;
			settings.MainPath = mainPathSelector.SelectedIndex;
			settings.MaxBackJump = (int)maxBackJump.Value;
			settings.MaxForwJump = (int)maxForwJump.Value;
			settings.BackJumpProb = (int)backJumpProb.Value;
			settings.AllowSameLevel = allowSameLevel.Checked;
			settings.RandomMusic = randomMusic.Checked;
			settings.SeparateSoundtracks = separateSoundtracks.Checked;
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
			if (Directory.Exists(Path.Combine(path, "Data\\Scripts")))
				Directory.Delete(Path.Combine(path, "Data\\Scripts"), true);
			Directory.CreateDirectory(Path.Combine(path, @"Data\Scripts\Menu"));
			Directory.CreateDirectory(Path.Combine(path, @"Data\Scripts\Global"));
			Directory.CreateDirectory(Path.Combine(path, @"Data\Scripts\R8"));
			Random r = new Random(seed);
			for (int i = 0; i < stagecount; i++)
			{
				stageids[i] = i;
				stages[i] = new Stage();
			}
			stageids[stagecount] = stagecount;
			settings.Mode = modeSelector.SelectedIndex;
			bool timewarp = true;
			switch (modeSelector.SelectedIndex)
			{
				case 0: // stages + warps
					{
						int[] order = new int[stagecount];
						for (int i = 0; i < stagecount; i++)
							order[i] = r.Next();
						Array.Sort(order, stageids);
						switch (mainPathSelector.SelectedIndex)
						{
							case 0: // Act Clear
								for (int i = 0; i < stagecount; i++)
								{
									int next;
									if (i == stagecount - 1)
										next = stagecount;
									else
										next = stageids[i + 1];
									stages[stageids[i]].Clear = next;
								}
								break;
							case 1: // Time Travel
								for (int i = 0; i < stagecount; i++)
								{
									int next;
									if (i == stagecount - 1)
										next = stagecount;
									else
										next = stageids[i + 1];
									switch (stageids[i] % 10)
									{
										case 0:
										case 4:
											switch (r.Next(2))
											{
												case 0:
													stages[stageids[i]].Past = next;
													break;
												case 1:
													stages[stageids[i]].Future = next;
													break;
											}
											break;
										case 1:
										case 5:
											stages[stageids[i]].Future = next;
											break;
										case 2:
										case 3:
										case 6:
										case 7:
											stages[stageids[i]].Past = next;
											break;
										case 8:
										case 9:
											stages[stageids[i]].Clear = next;
											break;
									}
								}
								break;
							case 2: // Any Exit
								for (int i = 0; i < stagecount; i++)
								{
									int next;
									if (i == stagecount - 1)
										next = stagecount;
									else
										next = stageids[i + 1];
									switch (stageids[i] % 10)
									{
										case 0:
										case 4:
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
											break;
										case 1:
										case 5:
											switch (r.Next(2))
											{
												case 0:
													stages[stageids[i]].Clear = next;
													break;
												case 1:
													stages[stageids[i]].Future = next;
													break;
											}
											break;
										case 2:
										case 3:
										case 6:
										case 7:
											switch (r.Next(2))
											{
												case 0:
													stages[stageids[i]].Clear = next;
													break;
												case 1:
													stages[stageids[i]].Past = next;
													break;
											}
											break;
										case 8:
										case 9:
											stages[stageids[i]].Clear = next;
											break;
									}
								}
								break;
						}
						for (int i = 0; i < stagecount; i++)
						{
							Stage stg = stages[stageids[i]];
							int min, max;
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
							if (stg.Clear == -1)
								stg.Clear = stageids[r.Next(min, max)];
							switch (stageids[i] % 10)
							{
								case 0:
								case 4:
									if (stg.Past == -1)
										stg.Past = stageids[r.Next(min, max)];
									if (stg.Future == -1)
										stg.Future = stageids[r.Next(min, max)];
									break;
								case 1:
								case 5:
									if (stg.Future == -1)
										stg.Future = stageids[r.Next(min, max)];
									break;
								case 2:
								case 3:
								case 6:
								case 7:
									if (stg.Past == -1)
										stg.Past = stageids[r.Next(min, max)];
									break;
							}
						}
					}
					break;
				case 1: // rounds
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
						int[] order = new int[7];
						for (int i = 0; i < 7; i++)
						{
							rounds[i] = i;
							order[i] = r.Next();
						}
						Array.Sort(order, rounds);
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
				case 2: // acts
					{
						for (int i = 0; i < stagecount; i++)
						{
							Stage stg = stages[i];
							switch (i % 10)
							{
								case 0:
								case 4:
									stg.Past = i + 1;
									stg.Future = i + 3;
									stg.GoodFuture = i + 2;
									break;
								case 1:
								case 5:
									stg.Future = i - 1;
									break;
								case 2:
								case 6:
									stg.Past = i - 2;
									break;
								case 3:
								case 7:
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
				case 3: // time periods
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
				case 4: // branching paths
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
								switch (curset[i] % 10)
								{
									case 0:
									case 4:
										stg.Past = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
										if (!newset.Contains(stg.Past))
											newset.Add(stg.Past);
										stg.Future = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
										if (!newset.Contains(stg.Future))
											newset.Add(stg.Future);
										break;
									case 1:
									case 5:
										stg.Future = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
										if (!newset.Contains(stg.Future))
											newset.Add(stg.Future);
										break;
									case 2:
									case 3:
									case 6:
									case 7:
										stg.Past = GetStageFromLists(r, newset, stagepool, stagepool.Count / 6);
										if (!newset.Contains(stg.Past))
											newset.Add(stg.Past);
										break;
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
			}
			string tmpstr = Properties.Resources.LoadSaveMenu_template;
			tmpstr = tmpstr.Replace("//REPLACE1", $"SaveRAM[ArrayPos1]={stageids[0] + 1}");
			tmpstr = tmpstr.Replace("//REPLACE2", $"Stage.ListPos={stageids[0]}");
			File.WriteAllText(Path.Combine(path, @"Data\Scripts\Menu\LoadSaveMenu.txt"), tmpstr);
			File.WriteAllText(Path.Combine(path, @"Data\Scripts\R8\Amy.txt"), Properties.Resources.Amy);
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i < stagecount; i++)
			{
				Stage stg = stages[i];
				int stgno = i % 10;
				sb.AppendLine($"\tcase {i}");
				if (stg.ClearGF != -1)
				{
					sb.AppendLine($"\t\tif Good_Future_Count==2");
					sb.AppendLine($"\t\t\tTempValue0={stg.ClearGF}");
					sb.AppendLine($"\t\telse");
					sb.AppendLine($"\t\t\tTempValue0={stg.Clear}");
					sb.AppendLine($"\t\tendif");
				}
				else
					sb.AppendLine($"\t\tTempValue0={stg.Clear}");
				sb.AppendLine("\t\tbreak");
			}
			File.WriteAllText(Path.Combine(path, @"Data\Scripts\Global\ActFinish.txt"), Properties.Resources.ActFinish_template.Replace("//REPLACE", sb.ToString()));
			if (timewarp)
			{
				sb.Clear();
				for (int i = 0; i < stagecount; i++)
				{
					Stage stg = stages[i];
					if (stg.Past == -1 && stg.Future == -1)
						continue;
					sb.AppendLine($"\t\t\tcase {i}");
					if (stg.Past != -1 && stg.Future != -1)
					{
						sb.AppendLine("\t\t\t\tif Warp.Destination==1");
						sb.AppendLine($"\t\t\t\t\tTempValue0={stg.Past}");
						sb.AppendLine("\t\t\t\telse");
						if (stg.GoodFuture != -1)
						{
							sb.AppendLine("\t\t\t\t\tif Transporter_Destroyed==1");
							sb.AppendLine($"\t\t\t\t\t\tTempValue0={stg.GoodFuture}");
							sb.AppendLine("\t\t\t\t\telse");
							sb.AppendLine($"\t\t\t\t\t\tTempValue0={stg.Future}");
							sb.AppendLine("\t\t\t\t\tendif");
						}
						else
							sb.AppendLine($"\t\t\t\t\tTempValue0={stg.Future}");
						sb.AppendLine("\t\t\t\tendif");
					}
					else if (stg.Future != -1)
					{
						sb.AppendLine($"\t\t\t\tTempValue0={stg.Future}");
					}
					else if (stg.Past != -1)
					{
						sb.AppendLine($"\t\t\t\tTempValue0={stg.Past}");
					}
					sb.AppendLine("\t\t\t\tbreak");
				}
				File.WriteAllText(Path.Combine(path, @"Data\Scripts\Global\TimeWarp.txt"), Properties.Resources.TimeWarp_template.Replace("//REPLACE", sb.ToString()));
			}
			if (randomMusic.Checked)
			{
				Dictionary<string, string> scriptFiles = new Dictionary<string, string>();
				string dir = Path.Combine(Directory.GetCurrentDirectory(), "Scripts");
				foreach (string item in Directory.EnumerateFiles(dir, "*.txt", SearchOption.AllDirectories))
					scriptFiles[item.Substring(dir.Length + 1)] = item;
				string[] mods = null;
				if (File.Exists(@"mods\SCDSteamModLoader.ini"))
				{
					ModLoaderInfo info = IniSerializer.Deserialize<ModLoaderInfo>(@"mods\SCDSteamModLoader.ini");
					if (info.Mods != null)
						mods = info.Mods.Select(a => Path.Combine(Directory.GetCurrentDirectory(), "mods", a)).ToArray();
				}
				else if (File.Exists(@"mods\modconfig.ini"))
				{
					DecompModInfo info = IniSerializer.Deserialize<DecompModInfo>(@"mods\modconfig.ini");
					if (info.Mods?.Mods != null)
						mods = info.Mods.Mods.Where(a => a.Value).Select(a => Path.Combine(Directory.GetCurrentDirectory(), "mods", a.Key)).ToArray();
				}
				if (mods != null)
					foreach (string mod in mods)
					{
						dir = Path.Combine(Directory.GetCurrentDirectory(), @"Data\Scripts");
						foreach (string item in Directory.EnumerateFiles(dir, "*.txt", SearchOption.AllDirectories))
							scriptFiles[item.Substring(dir.Length + 1)] = item;
					}
				var scripts = new List<(string file, string script)>();
				Dictionary<string, string> musicFiles = new Dictionary<string, string>();
				foreach (var file in scriptFiles)
				{
					string script = File.ReadAllText(file.Value);
					MatchCollection matches = musregex.Matches(script);
					if (matches.Count > 0)
					{
						scripts.Add((file.Key, script));
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
					Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(path, @"Data\Scripts", script.file)));
					File.WriteAllText(Path.Combine(path, @"Data\Scripts", script.file), musregex.Replace(script.script, m =>
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
			else
				return $"{RoundNames[id / 10]} {ZoneNames[id % 10]}";
		}

		private void spoilerLevelList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (spoilerLevelList.SelectedIndex != -1)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				Stage stg = stages[stageids[spoilerLevelList.SelectedIndex]];
				if (settings.Mode == 0)
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
					sw.WriteLine($"Mode: {modeSelector.SelectedItem}");
					if (settings.Mode == 0)
					{
						sw.WriteLine($"Main Path: {mainPathSelector.SelectedItem}");
						sw.WriteLine($"Max Backwards Jump: {maxBackJump.Value}");
						sw.WriteLine($"Max Forwards Jump: {maxForwJump.Value}");
						sw.WriteLine($"Backwards Jump Probability: {backJumpProb.Value}");
						sw.WriteLine($"Allow Same Level: {allowSameLevel.Checked}");
					}
					sw.WriteLine($"Random Music: {randomMusic.Checked}");
					sw.WriteLine($"Separate Soundtracks: {separateSoundtracks.Checked}");
					sw.WriteLine();
					for (int i = 0; i < stagecount; i++)
					{
						Stage stg = stages[stageids[i]];
						sw.WriteLine($"{GetStageName(stageids[i])}:");
						if (settings.Mode == 0)
						{
							sw.WriteLine($"Clear -> {GetStageName(stg.Clear)} ({Array.IndexOf(stageids, stg.Clear) - spoilerLevelList.SelectedIndex:+##;-##;0})");
							if (stg.Past != -1)
								sw.WriteLine($"Past -> {GetStageName(stg.Past)} ({Array.IndexOf(stageids, stg.Past) - spoilerLevelList.SelectedIndex:+##;-##;0})");
							if (stg.Future != -1)
								sw.WriteLine($"Future -> {GetStageName(stg.Future)} ({Array.IndexOf(stageids, stg.Future) - spoilerLevelList.SelectedIndex:+##;-##;0})");
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
				case 0: // stages + warps
					gridmaxh = 1;
					gridmaxv = stagecount + 2;
					for (int i = 0; i <= stagecount; i++)
						levels[stageids[i]] = new ChartNode(0, i + 1);
					levels[stagecount + 1] = new ChartNode(0, 0);
					break;
				case 4: // branching paths
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
							r = a.Color.CompareTo(b.Color);
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
							r = a.Color.CompareTo(b.Color);
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
					Pen clearpen = new Pen(Color.Black, 3) { EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor };
					Pen pastpen = new Pen(Color.Blue, 3) { EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor };
					Pen futurepen = new Pen(Color.Lime, 3) { EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor };
					Pen badfuturepen = new Pen(Color.Red, 3) { EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor };
					Pen cleargfpen = new Pen(Color.Fuchsia, 3) { EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor };
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
								Pen pen;
								switch (con.Color)
								{
									case ConnectionType.Past:
										pen = pastpen;
										break;
									case ConnectionType.Future:
										pen = futurepen;
										break;
									case ConnectionType.BadFuture:
										pen = badfuturepen;
										break;
									case ConnectionType.ClearGF:
										pen = cleargfpen;
										break;
									default:
										pen = clearpen;
										break;
								}
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

	class Stage
	{
		public int Clear { get; set; } = -1;
		public int ClearGF { get; set; } = -1;
		public int Past { get; set; } = -1;
		public int Future { get; set; } = -1;
		public int GoodFuture { get; set; } = -1;
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
			if (GridY == dest.GridY)
			{
				switch (GridX - dest.GridX)
				{
					case -1:
						outdir = Direction.Right;
						indir = Direction.Left;
						break;
					case 1:
						outdir = Direction.Left;
						indir = Direction.Right;
						break;
					case 0:
						outdir = Direction.Right;
						indir = Direction.Right;
						break;
					default:
						outdir = Direction.Top;
						indir = Direction.Top;
						break;
				}
			}
			else
			{
				int ydist = GridY - dest.GridY;
				if (ydist < 0)
				{
					if (ydist == -1)
					{
						outdir = Direction.Bottom;
						indir = Direction.Top;
					}
					else
					{
						outdir = Direction.Right;
						indir = Direction.Right;
					}
				}
				else
				{
					if (ydist == 1)
					{
						outdir = Direction.Top;
						indir = Direction.Bottom;
					}
					else
					{
						outdir = Direction.Left;
						indir = Direction.Left;
					}
				}
			}
			ChartConnection c = dest.IncomingConnections[indir].Find(a => a.Color == color);
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
		public ConnectionType Color { get; }
		public List<ChartNode> Sources { get; }
		public int MinX { get; private set; }
		public int MinY { get; private set; }
		public int MaxX { get; private set; }
		public int MaxY { get; private set; }
		public int Distance { get; private set; }
		public int Lane { get; set; }

		public ChartConnection(Direction side, ConnectionType color, ChartNode src, ChartNode dst)
		{
			Node = dst;
			Side = side;
			Color = color;
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
		public int Mode { get; set; }
		[IniAlwaysInclude]
		public int MainPath { get; set; }
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
}
