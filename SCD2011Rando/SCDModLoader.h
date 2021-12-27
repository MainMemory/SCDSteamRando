#pragma once
struct ScriptEngine {
	int operands[10];
	int tempValue[8];
	int arrayPosition[3];
	int checkResult;
};

#define ModLoaderVer 1

struct HelperFunctions
{
	int Version;
	// Registers a new function that can be called from scripts.
	void (*RegisterScriptFunction)(const char* name, int argcnt, void(*func)(ScriptEngine scriptEng));
	// Registers a new variable that can be used in scripts.
	void (*RegisterScriptVariable)(const char* name, int (*get)(ScriptEngine scriptEng, int arrayVal), void (*set)(ScriptEngine scriptEng, int arrayVal, int value));
	// Registers a new alias that can be used in scripts.
	void (*RegisterScriptAlias)(const char* name, const char* value);
};

struct ModInfo
{
	int LoaderVer;
};