#include "pch.h"
#include "SCDModLoader.h"
#include "..\..\mod-loader-common\ModLoaderCommon\IniFile.hpp"
#include <random>
#include <numeric>

const char *const RoundNames[] =
{
	"Palmtree Panic",
	"Collision Chaos",
	"Tidal Tempest",
	"Quartz Quadrant",
	"Wacky Workbench",
	"Stardust Speedway",
	"Metallic Madness"
};

const char* const ZoneNames[] =
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

struct Stage
{
	int Clear = -1;
	int Past = -1;
	int Future = -1;
};

std::default_random_engine gen;
Stage stages[70];
int stage0;

void dummyset(ScriptEngine scriptEng, int arrayVal, int value) {}

int getStartingStage(ScriptEngine scriptEng, int arrayVal)
{
	return stage0;
}

int getStageClear(ScriptEngine scriptEng, int arrayVal)
{
	return stages[arrayVal].Clear;
}

int getStagePast(ScriptEngine scriptEng, int arrayVal)
{
	return stages[arrayVal].Past;
}

int getStageFuture(ScriptEngine scriptEng, int arrayVal)
{
	return stages[arrayVal].Future;
}

void FindShortestPath(Stage& stage, std::vector<int>& path, std::vector<int>& shortestPath, int target)
{
	if (shortestPath.size() && path.size() >= shortestPath.size())
		return;
	if (stage.Clear != -1 && std::find(path.cbegin(), path.cend(), stage.Clear) != path.cend())
	{
		path.push_back(stage.Clear);
		if (stage.Clear == target)
		{
			if (!shortestPath.size() || path.size() < shortestPath.size())
				shortestPath = path;
		}
		else
			FindShortestPath(stages[stage.Clear], path, shortestPath, target);
		path.pop_back();
	}
	if (stage.Past != -1 && std::find(path.cbegin(), path.cend(), stage.Past) != path.cend())
	{
		path.push_back(stage.Past);
		if (stage.Past == target)
		{
			if (!shortestPath.size() || path.size() < shortestPath.size())
				shortestPath = path;
		}
		else
			FindShortestPath(stages[stage.Past], path, shortestPath, target);
		path.pop_back();
	}
	if (stage.Future != -1 && std::find(path.cbegin(), path.cend(), stage.Future) != path.cend())
	{
		path.push_back(stage.Future);
		if (stage.Future == target)
		{
			if (!shortestPath.size() || path.size() < shortestPath.size())
				shortestPath = path;
		}
		else
			FindShortestPath(stages[stage.Future], path, shortestPath, target);
		path.pop_back();
	}
}

#define PathCat(s) strcpy_s(pathbuf, path); strcat_s(pathbuf, s)
extern "C"
{
	__declspec(dllexport) void Init(const char* path, const HelperFunctions& helperFunctions)
	{
		char pathbuf[MAX_PATH];
		PathCat("\\config.ini");
		const IniFile* settings = new IniFile(pathbuf);
		bool spoilers = settings->getBool("", "spoil");
		unsigned int seed = settings->getInt("", "seed");
		if (seed == 0)
			seed = std::random_device()();
		std::seed_seq seq = { seed };
		gen.seed(seq);
		int stageids[70];
		std::iota(stageids, stageids + 69, 0);
		std::shuffle(stageids, stageids + 68, gen);
		stage0 = stageids[0];
		for (int i = 0; i < 68; i++)
			switch (stageids[i] % 10)
			{
			case 0:
			case 4:
				switch (gen() % 3)
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
				switch (gen() % 2)
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
				switch (gen() % 2)
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
		std::uniform_int_distribution<> stgdist(0, 69);
		bool mmz3d = false;
		for (int i = 0; i < 70; i++)
		{
			stageids[i] = stgdist(gen);
			if (stageids[i] == 69)
				mmz3d = true;
		}
		if (!mmz3d)
			stageids[stgdist(gen)] = 69;
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
		char stg0str[3];
		helperFunctions.RegisterScriptAlias("Rando.StartingStage", itoa(stage0, stg0str, 10));
		helperFunctions.RegisterScriptVariable("Rando.Stages.Clear", getStageClear, dummyset);
		helperFunctions.RegisterScriptVariable("Rando.Stages.Past", getStagePast, dummyset);
		helperFunctions.RegisterScriptVariable("Rando.Stages.Future", getStageFuture, dummyset);
		PathCat("\\spoilers.log");
		FILE* file;
		if (!fopen_s(&file, pathbuf, "w"))
		{
			fprintf_s(file, "Seed: %u\n", seed);
			fprintf_s(file, "Starting Level: %s %s\n", RoundNames[stage0 / 10], ZoneNames[stage0 % 10]);
			fprintf_s(file, "\n");
			for (int i = 0; i < 68; i++)
			{
				fprintf_s(file, "%s %s\n", RoundNames[i / 10], ZoneNames[i % 10]);
				fprintf_s(file, "Clear -> %s %s", RoundNames[stages[i].Clear / 10], ZoneNames[stages[i].Clear % 10]);
				if (stages[i].Past != -1)
					fprintf_s(file, "Past -> %s %s", RoundNames[stages[i].Past / 10], ZoneNames[stages[i].Past % 10]);
				if (stages[i].Future != -1)
					fprintf_s(file, "Past -> %s %s", RoundNames[stages[i].Future / 10], ZoneNames[stages[i].Future % 10]);
				fprintf_s(file, "\n");
			}
			fprintf_s(file, "Shortest Paths:\n");
			std::vector<int> shortestPath;
			std::vector<int> pth;
			pth.reserve(70);
			pth.push_back(stage0);
			FindShortestPath(stages[stage0], pth, shortestPath, 68);
			for (int i = 0; i < shortestPath.size() - 2; ++i)
			{
				const char* exit = "Clear";
				if (stages[shortestPath[i]].Past == shortestPath[i - 1])
					exit = "Past";
				else if (stages[shortestPath[i]].Future == shortestPath[i - 1])
					exit = "Future";
				fprintf_s(file, "%s %s (%s) -> ", RoundNames[shortestPath[i] / 10], ZoneNames[shortestPath[i] % 10], exit);
			}
			fprintf_s(file, "Metallic Madness Zone 3 Good Future (%u levels)", shortestPath.size());
			shortestPath.clear();
			FindShortestPath(stages[stage0], pth, shortestPath, 69);
			for (int i = 0; i < shortestPath.size() - 2; ++i)
			{
				const char* exit = "Clear";
				if (stages[shortestPath[i]].Past == shortestPath[i - 1])
					exit = "Past";
				else if (stages[shortestPath[i]].Future == shortestPath[i - 1])
					exit = "Future";
				fprintf_s(file, "%s %s (%s) -> ", RoundNames[shortestPath[i] / 10], ZoneNames[shortestPath[i] % 10], exit);
			}
			fprintf_s(file, "Metallic Madness Zone 3 Bad Future (%u levels)", shortestPath.size());
		}
	}

	__declspec(dllexport) ModInfo SCDModInfo = { ModLoaderVer };
}