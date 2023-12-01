#pragma once

#include <openrct2/world/TileElement.h>

// Get element at index
const TileElement* GetTileElementAt(int x, int y, int index);

// Get element at index of specified type
const TileElement* GetTileElementAt(int x, int y, int index, TileElementType type);
