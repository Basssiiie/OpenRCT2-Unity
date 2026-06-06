#pragma once

#include <openrct2/world/tile_element/TileElement.h>
#include <openrct2/world/tile_element/TileElementType.h>

using namespace OpenRCT2;

// Get element at index
const TileElement* GetTileElementAt(int x, int y, int index);

// Get element at index of specified type
const TileElement* GetTileElementAt(int x, int y, int index, TileElementType type);
