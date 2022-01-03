using IniFile;
using System;
using System.Collections.Generic;
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
			}
			settings = Settings.Load();
			seedSelector.Value = settings.Seed;
			randomSeed.Checked = settings.RandomSeed;
			mainPathSelector.SelectedIndex = settings.MainPath;
			maxBackJump.Value = settings.MaxBackJump;
			maxForwJump.Value = settings.MaxForwJump;
			if (randomMusic.Enabled)
				randomMusic.Checked = settings.RandomMusic;
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			settings.Seed = (int)seedSelector.Value;
			settings.RandomSeed = randomSeed.Checked;
			settings.MainPath = mainPathSelector.SelectedIndex;
			settings.MaxBackJump = (int)maxBackJump.Value;
			settings.MaxForwJump = (int)maxForwJump.Value;
			settings.RandomMusic = randomMusic.Checked;
			settings.Save();
		}

		private void randomSeed_CheckedChanged(object sender, EventArgs e)
		{
			seedSelector.Enabled = !randomSeed.Checked;
		}

		readonly (int b, int f)[] presets =
		{
			(0, 68), // Easiest
			(5, 60), // Very Easy
			(10, 50), // Easy
			(15, 40), // Normal
			(30, 30), // Hard
			(45, 15), // Very Hard
			(68, 0), // Insane
			(0, 1), // All Levels
			(68, 68) // Full Random
		};
		private void presetSelector_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (presetSelector.SelectedIndex == -1) return;
			(int b, int f) = presets[presetSelector.SelectedIndex];
			maxBackJump.Value = b;
			maxForwJump.Value = f;
		}

		readonly string[] RoundNames =
		{
			"Palmtree Panic",
			"Collision Chaos",
			"Tidal Tempest",
			"Quartz Quadrant",
			"Wacky Workbench",
			"Stardust Speedway",
			"Metallic Madness"
		};

		readonly string[] ZoneNames =
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
		int[] stageids = new int[69];
		int stage0;
		Stage[] stages = new Stage[70];
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
			Random r = new Random(seed);
			for (int i = 0; i < 69; i++)
				stageids[i] = i;
			int[] order = new int[68];
			for (int i = 0; i < 68; i++)
				order[i] = r.Next();
			Array.Sort(order, stageids);
			stage0 = stageids[0];
			for (int i = 0; i < 70; i++)
				stages[i] = new Stage();
			if (mainPathSelector.SelectedIndex == 0)
			{
				for (int i = 0; i < 68; i++)
					stages[stageids[i]].Clear = stageids[i + 1];
			}
			else
			{
				for (int i = 0; i < 68; i++)
					switch (stageids[i] % 10)
					{
						case 0:
						case 4:
							switch (r.Next(3))
							{
								case 0:
									stages[stageids[i]].Clear = stageids[i + 1];
									break;
								case 1:
									stages[stageids[i]].Past = stageids[i + 1];
									break;
								case 2:
									stages[stageids[i]].Future = stageids[i + 1];
									break;
							}
							break;
						case 1:
						case 5:
							switch (r.Next(2))
							{
								case 0:
									stages[stageids[i]].Clear = stageids[i + 1];
									break;
								case 1:
									stages[stageids[i]].Future = stageids[i + 1];
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
									stages[stageids[i]].Clear = stageids[i + 1];
									break;
								case 1:
									stages[stageids[i]].Past = stageids[i + 1];
									break;
							}
							break;
						case 8:
						case 9:
							stages[stageids[i]].Clear = stageids[i + 1];
							break;
					}
			}
			for (int i = 0; i < 68; i++)
			{
				Stage stg = stages[stageids[i]];
				int min = Math.Max(i - (int)maxBackJump.Value, 0);
				int max = Math.Min(i + (int)maxForwJump.Value + 1, 69);
				if (stg.Clear == -1)
				{
					stg.Clear = stageids[r.Next(min, max)];
					if (stg.Clear == 68)
						stg.Clear += r.Next(2);
				}
				switch (stageids[i] % 10)
				{
					case 0:
					case 4:
						if (stg.Past == -1)
						{
							stg.Past = stageids[r.Next(min, max)];
							if (stg.Past == 68)
								stg.Past += r.Next(2);
						}
						if (stg.Future == -1)
						{
							stg.Future = stageids[r.Next(min, max)];
							if (stg.Future == 68)
								stg.Future += r.Next(2);
						}
						break;
					case 1:
					case 5:
						if (stg.Future == -1)
						{
							stg.Future = stageids[r.Next(min, max)];
							if (stg.Future == 68)
								stg.Future += r.Next(2);
						}
						break;
					case 2:
					case 3:
					case 6:
					case 7:
						if (stg.Past == -1)
						{
							stg.Past = stageids[r.Next(min, max)];
							if (stg.Past == 68)
								stg.Past += r.Next(2);
						}
						break;
				}
			}
			if (Directory.Exists(Path.Combine(path, "Data\\Scripts")))
				Directory.Delete(Path.Combine(path, "Data\\Scripts"), true);
			Directory.CreateDirectory(Path.Combine(path, @"Data\Scripts\Menu"));
			Directory.CreateDirectory(Path.Combine(path, @"Data\Scripts\Global"));
			using (StringReader sr = new StringReader(Properties.Resources.LoadSaveMenu_template))
			using (StreamWriter sw = File.CreateText(Path.Combine(path, @"Data\Scripts\Menu\LoadSaveMenu.txt")))
				while (sr.Peek() != -1)
				{
					string s = sr.ReadLine();
					if (s.Equals("//REPLACE"))
						sw.WriteLine("\t\t\tStage.ListPos={0}", stage0);
					else
						sw.WriteLine(s);
				}
			using (StringReader sr = new StringReader(Properties.Resources.ActFinish_template))
			using (StreamWriter sw = File.CreateText(Path.Combine(path, @"Data\Scripts\Global\ActFinish.txt")))
				while (sr.Peek() != -1)
				{
					string s = sr.ReadLine();
					if (s.Equals("//REPLACE"))
					{
						for (int i = 0; i < 68; i++)
						{
							sw.WriteLine("\tcase {0}", i);
							sw.WriteLine("\t\tStage.ListPos={0}", stages[i].Clear);
							sw.WriteLine("\t\tbreak");
						}
					}
					else
						sw.WriteLine(s);
				}
			using (StringReader sr = new StringReader(Properties.Resources.TimeWarp_template))
			using (StreamWriter sw = File.CreateText(Path.Combine(path, @"Data\Scripts\Global\TimeWarp.txt")))
				while (sr.Peek() != -1)
				{
					string s = sr.ReadLine();
					if (s.Equals("//REPLACE"))
					{
						for (int i = 0; i < 68; i++)
						{
							Stage stg = stages[i];
							if (stg.Past == -1 && stg.Future == -1)
								continue;
							sw.WriteLine("\t\t\tcase {0}", i);
							if (stg.Past != -1 && stg.Future != -1)
							{
								sw.WriteLine("\t\t\t\tif Warp.Destination==1");
								sw.WriteLine("\t\t\t\t\tStage.ListPos={0}", stg.Past);
								sw.WriteLine("\t\t\t\telse");
								sw.WriteLine("\t\t\t\t\tStage.ListPos={0}", stg.Future);
								sw.WriteLine("\t\t\t\tendif");
							}
							else if (stg.Future != -1)
							{
								sw.WriteLine("\t\t\t\tStage.ListPos={0}", stg.Future);
							}
							else if (stg.Past != -1)
							{
								sw.WriteLine("\t\t\t\tStage.ListPos={0}", stg.Past);
							}
							sw.WriteLine("\t\t\t\tbreak");
						}
					}
					else
						sw.WriteLine(s);
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
				foreach (var script in scripts)
				{
					Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(path, @"Data\Scripts", script.file)));
					File.WriteAllText(Path.Combine(path, @"Data\Scripts", script.file), musregex.Replace(script.script, m =>
					{
						KeyValuePair<string, string> mus;
						if (m.Groups[3].Value == "0")
							mus = muslist[r.Next(muslist.Length)];
						else
							mus = loopmuslist[r.Next(loopmuslist.Length)];
						return $@"SetMusicTrack({mus.Key},{m.Groups[2].Value},{mus.Value})";
					}));
				}
			}
			spoilerLevelList.BeginUpdate();
			spoilerLevelList.Items.Clear();
			for (int i = 0; i < 68; i++)
				spoilerLevelList.Items.Add($"{RoundNames[stageids[i] / 10]} {ZoneNames[stageids[i] % 10]}");
			spoilerLevelList.EndUpdate();
			spoilerLevelList.Enabled = true;
			spoilerLevelList.SelectedIndex = 0;
		}

		private void spoilerLevelList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (spoilerLevelList.SelectedIndex != -1)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.AppendFormat("Clear -> {0} {1}", RoundNames[stages[stageids[spoilerLevelList.SelectedIndex]].Clear / 10], ZoneNames[stages[stageids[spoilerLevelList.SelectedIndex]].Clear % 10]);
				sb.AppendLine();
				if (stages[stageids[spoilerLevelList.SelectedIndex]].Past != -1)
				{
					sb.AppendFormat("Past -> {0} {1}", RoundNames[stages[stageids[spoilerLevelList.SelectedIndex]].Past / 10], ZoneNames[stages[stageids[spoilerLevelList.SelectedIndex]].Past % 10]);
					sb.AppendLine();
				}
				if (stages[stageids[spoilerLevelList.SelectedIndex]].Future != -1)
				{
					sb.AppendFormat("Future -> {0} {1}", RoundNames[stages[stageids[spoilerLevelList.SelectedIndex]].Future / 10], ZoneNames[stages[stageids[spoilerLevelList.SelectedIndex]].Future % 10]);
					sb.AppendLine();
				}
				sb.Append("Shortest Path: ");
				int[] shortestPath;
				if (maxForwJump.Value < 2)
				{
					shortestPath = new int[69 - spoilerLevelList.SelectedIndex];
					Array.Copy(stageids, spoilerLevelList.SelectedIndex, shortestPath, 0, shortestPath.Length);
				}
				else
					shortestPath = FindShortestPath(stageids[spoilerLevelList.SelectedIndex]);
				for (int i = 0; i < shortestPath.Length - 1; i++)
				{
					string exit = "Clear";
					if (stages[shortestPath[i]].Past == shortestPath[i + 1])
						exit = "Past";
					else if (stages[shortestPath[i]].Future == shortestPath[i + 1])
						exit = "Future";
					sb.AppendFormat("{0} {1} ({2}) -> ", RoundNames[shortestPath[i] / 10], ZoneNames[shortestPath[i] % 10], exit);
				}
				sb.AppendFormat("{0} {1} ({2} levels)", RoundNames[shortestPath[shortestPath.Length - 1] / 10], ZoneNames[shortestPath[shortestPath.Length - 1] % 10], shortestPath.Length);
				spoilerLevelInfo.Text = sb.ToString();
			}
		}

		int[] FindShortestPath(int start)
		{
			Stack<int> stack = new Stack<int>(70);
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
				if (stage.Clear >= 68)
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
				if (stage.Past >= 68)
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
				if (stage.Future >= 68)
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
	}

	class Stage
	{
		public int Clear { get; set; } = -1;
		public int Past { get; set; } = -1;
		public int Future { get; set; } = -1;
	}

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
		public int MainPath { get; set; }
		[System.ComponentModel.DefaultValue(68)]
		[IniAlwaysInclude]
		public int MaxBackJump { get; set; } = 68;
		[System.ComponentModel.DefaultValue(68)]
		[IniAlwaysInclude]
		public int MaxForwJump { get; set; } = 68;
		[IniAlwaysInclude]
		public bool RandomMusic { get; set; }

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
