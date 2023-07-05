#pragma once

#include <openrct2/Context.h>
using namespace OpenRCT2;

#define EXPORT __declspec(dllexport)

// Write to output log with a custom prefix and line ending.
void dll_log(const char* format, ...);
