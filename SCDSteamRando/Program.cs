using System;
using System.Collections.Generic;
using System.IO;

namespace SCDSteamRando
{
	static class Program
	{
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

		static void Main(string[] args)
		{
			int seed;
			if (args.Length < 1 || !int.TryParse(args[0], out seed))
			{
				Console.Write("Seed: ");
				if (!int.TryParse(Console.ReadLine(), out seed))
					seed = (int)DateTime.Now.Ticks;
			}
			Random r = new Random(seed);
			int[] stageids = new int[69];
			for (int i = 0; i < 69; i++)
				stageids[i] = i;
			int[] order = new int[68];
			for (int i = 0; i < 68; i++)
				order[i] = r.Next();
			Array.Sort(order, stageids);
			int stage0 = stageids[0];
			Stage[] stages = new Stage[70];
			for (int i = 0; i < 70; i++)
				stages[i] = new Stage();
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
			stageids = new int[70];
			for (int i = 0; i < 70; i++)
				stageids[i] = r.Next(70);
			if (Array.IndexOf(stageids, 69) == -1)
				stageids[r.Next(70)] = 69;
			int j = 0;
			for (int i = 0; i < 68; i++)
			{
				if (stages[i].Clear == -1)
					stages[i].Clear = stageids[j++];
				switch (i % 10)
				{
					case 0:
					case 4:
						if (stages[i].Past == -1)
							stages[i].Past = stageids[j++];
						if (stages[i].Future == -1)
							stages[i].Future = stageids[j++];
						break;
					case 1:
					case 5:
						if (stages[i].Future == -1)
							stages[i].Future = stageids[j++];
						break;
					case 2:
					case 3:
					case 6:
					case 7:
						if (stages[i].Past == -1)
							stages[i].Past = stageids[j++];
						break;
				}
			}
			Directory.CreateDirectory(@"Data\Scripts\Menu");
			Directory.CreateDirectory(@"Data\Scripts\Global");
			using (StringReader sr = new StringReader(Properties.Resources.LoadSaveMenu_template))
			using (StreamWriter sw = File.CreateText(@"Data\Scripts\Menu\LoadSaveMenu.txt"))
				while (sr.Peek() != -1)
				{
					string s = sr.ReadLine();
					if (s.Equals("//REPLACE"))
						sw.WriteLine("\t\t\tStage.ListPos={0}", stage0);
					else
						sw.WriteLine(s);
				}
			using (StringReader sr = new StringReader(Properties.Resources.ActFinish_template))
			using (StreamWriter sw = File.CreateText(@"Data\Scripts\Global\ActFinish.txt"))
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
			using (StreamWriter sw = File.CreateText(@"Data\Scripts\Global\TimeWarp.txt"))
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
			using (StreamWriter sw = File.CreateText("spoilers.log"))
			{
				sw.WriteLine("Seed: {0}", seed);
				sw.WriteLine("Starting Level: {0} {1}", RoundNames[stage0 / 10], ZoneNames[stage0 % 10]);
				sw.WriteLine();
				for (int i = 0; i < 68; i++)
				{
					sw.WriteLine("{0} {1}", RoundNames[i / 10], ZoneNames[i % 10]);
					sw.WriteLine("Clear -> {0} {1}", RoundNames[stages[i].Clear / 10], ZoneNames[stages[i].Clear % 10]);
					if (stages[i].Past != -1)
						sw.WriteLine("Past -> {0} {1}", RoundNames[stages[i].Past / 10], ZoneNames[stages[i].Past % 10]);
					if (stages[i].Future != -1)
						sw.WriteLine("Past -> {0} {1}", RoundNames[stages[i].Future / 10], ZoneNames[stages[i].Future % 10]);
					sw.WriteLine();
				}
				sw.WriteLine("Shortest Paths:");
				int[] shortestPath = null;
				FindShortestPath(stages[stage0], new Stack<int>(stage0), stages, ref shortestPath, 68);
				for (int i = shortestPath.Length - 1; i > 0; i--)
				{
					string exit = "Clear";
					if (stages[shortestPath[i]].Past == shortestPath[i - 1])
						exit = "Past";
					else if (stages[shortestPath[i]].Future == shortestPath[i - 1])
						exit = "Future";
					sw.Write("{0} {1} ({2}) -> ", RoundNames[shortestPath[i] / 10], ZoneNames[shortestPath[i] % 10], exit);
				}
				sw.WriteLine("Metallic Madness Zone 3 Good Future ({0} levels)", shortestPath.Length);
				shortestPath = null;
				FindShortestPath(stages[stage0], new Stack<int>(stage0), stages, ref shortestPath, 69);
				for (int i = shortestPath.Length - 1; i > 0; i--)
				{
					string exit = "Clear";
					if (stages[shortestPath[i]].Past == shortestPath[i - 1])
						exit = "Past";
					else if (stages[shortestPath[i]].Future == shortestPath[i - 1])
						exit = "Future";
					sw.Write("{0} {1} ({2}) -> ", RoundNames[shortestPath[i] / 10], ZoneNames[shortestPath[i] % 10], exit);
				}
				sw.WriteLine("Metallic Madness Zone 3 Bad Future ({0} levels)", shortestPath.Length);
			}
		}

		static void FindShortestPath(Stage stage, Stack<int> path, Stage[] stages, ref int[] shortestPath, int target)
		{
			if (shortestPath != null && path.Count >= shortestPath.Length)
				return;
			if (stage.Clear != -1 && !path.Contains(stage.Clear))
			{
				path.Push(stage.Clear);
				if (stage.Clear == target)
				{
					if (shortestPath == null || path.Count < shortestPath.Length)
						shortestPath = path.ToArray();
				}
				else
					FindShortestPath(stages[stage.Clear], path, stages, ref shortestPath, target);
				path.Pop();
			}
			if (stage.Past != -1 && !path.Contains(stage.Past))
			{
				path.Push(stage.Past);
				if (stage.Past == target)
				{
					if (shortestPath == null || path.Count < shortestPath.Length)
						shortestPath = path.ToArray();
				}
				else
					FindShortestPath(stages[stage.Past], path, stages, ref shortestPath, target);
				path.Pop();
			}
			if (stage.Future != -1 && !path.Contains(stage.Future))
			{
				path.Push(stage.Future);
				if (stage.Future == target)
				{
					if (shortestPath == null || path.Count < shortestPath.Length)
						shortestPath = path.ToArray();
				}
				else
					FindShortestPath(stages[stage.Future], path, stages, ref shortestPath, target);
				path.Pop();
			}
		}
	}

	class Stage
	{
		public int Clear = -1;
		public int Past = -1;
		public int Future = -1;
	}
}
