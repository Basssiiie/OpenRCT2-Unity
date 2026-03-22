/*****************************************************************************
 * Copyright (c) 2014-2026 OpenRCT2 developers
 *
 * For a complete list of all authors, please refer to contributors.md
 * Interested in contributing? Visit https://github.com/OpenRCT2/OpenRCT2
 *
 * OpenRCT2 is licensed under the GNU General Public License version 3.
 *****************************************************************************/

#pragma once

#include "../core/FlagHolder.hpp"
#include "../core/StringTypes.h"
#include "../interface/ColourWithFlags.h"
#include "TextColour.h"

#include <cstdint>

using StringId = uint16_t;
struct ScreenCoordsXY;

enum class FontStyle : uint8_t;
enum class TextDarkness : uint8_t;

namespace OpenRCT2::Drawing
{
    struct RenderTarget;

    enum class TextDrawFlag : uint8_t
    {
        noFormatting,
        yOffsetEffect,
        ttf,
        noDraw,
    };
    using TextDrawFlags = FlagHolder<uint8_t, TextDrawFlag>;

    struct TextDrawInfo
    {
        int32_t startX{};
        int32_t startY{};
        int32_t x{};
        int32_t y{};
        int32_t maxX{};
        int32_t maxY{};
        TextDrawFlags textDrawFlags{};
        OpenRCT2::ColourFlags colourFlags{};
        TextDarkness darkness{};
        TextColours palette{};
        FontStyle fontStyle{};
        const int8_t* yOffset{};
    };

    void GfxDrawStringLeftCentred(
        RenderTarget& rt, StringId format, void* args, ColourWithFlags colour, const ScreenCoordsXY& coords);
    void DrawStringCentredRaw(
        RenderTarget& rt, const ScreenCoordsXY& coords, int32_t numLines, const utf8* text, FontStyle fontStyle);
    void DrawNewsTicker(
        RenderTarget& rt, const ScreenCoordsXY& coords, int32_t width, Colour colour, StringId format, u8string_view args,
        int32_t ticks);
    void GfxDrawStringWithYOffsets(
        RenderTarget& rt, const utf8* text, ColourWithFlags colour, const ScreenCoordsXY& coords, const int8_t* yOffsets,
        bool forceSpriteFont, FontStyle fontStyle);

    int32_t GfxWrapString(
        u8string_view text, int32_t width, FontStyle fontStyle, u8string* outWrappedText, int32_t* outNumLines);
    int32_t GfxGetStringWidth(std::string_view text, FontStyle fontStyle);
    int32_t GfxGetStringWidthNewLined(std::string_view text, FontStyle fontStyle);
    int32_t GfxGetStringWidthNoFormatting(std::string_view text, FontStyle fontStyle);
    int32_t StringGetHeightRaw(std::string_view text, FontStyle fontStyle);
    int32_t GfxClipString(char* buffer, int32_t width, FontStyle fontStyle);
    u8string ShortenPath(const u8string& path, int32_t availableWidth, FontStyle fontStyle);
    void TTFDrawString(
        RenderTarget& rt, u8string_view text, ColourWithFlags colour, const ScreenCoordsXY& coords, bool noFormatting,
        FontStyle fontStyle, TextDarkness darkness);

} // namespace OpenRCT2::Drawing
