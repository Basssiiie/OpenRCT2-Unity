#include "../../openrct2/core/Path.hpp"

#include <openrct2/Context.h>
#include <openrct2/Game.h>
#include <openrct2/GameState.h>
#include <openrct2/OpenRCT2.h>
#include <openrct2/platform/platform.h>
#include <openrct2/world/Park.h>

#include "Openrct2-dll.h"


std::unique_ptr<IContext> context;

extern "C"
{
    EXPORT void StartGame(const char* datapath, const char* rct2path, const char* rct1path)
    {
        if (context != nullptr)
            return;

        printf("(me) StartGame( %s )\n", datapath);
        _log_levels[DIAGNOSTIC_LEVEL_VERBOSE] = true;

        gOpenRCT2Headless = true;

        Path::GetAbsolute(gCustomOpenrctDataPath, std::size(gCustomOpenrctDataPath), datapath);

        if (rct1path != nullptr)
            Path::GetAbsolute(gCustomRCT1DataPath, std::size(gCustomRCT1DataPath), rct1path);
        if (rct2path != nullptr)
            Path::GetAbsolute(gCustomRCT2DataPath, std::size(gCustomRCT2DataPath), rct2path);

        printf(
            "(me) gCustomOpenrctDataPath = %s\n(me) gCustomRCT1DataPath = %s\n(me) gCustomRCT2DataPath = %s\n",
            gCustomOpenrctDataPath, gCustomRCT1DataPath, gCustomRCT2DataPath
        );

        // Create a plain context
        core_init();
        context = CreateContext(); // OpenRCT2::CreateContext()
        bool result = context->Initialise();

        printf("(me) Initialise = %i\n", result);
    }


    EXPORT void PerformGameUpdate()
    {
        context->GetGameState()->Update();
    }


    EXPORT void StopGame()
    {
        printf("(me) StopGame()\n");

        context = nullptr;
    }


    EXPORT void LoadPark(const char* filepath)
    {
        printf("(me) LoadPark( %s )\n", filepath);

        context->LoadParkFromFile(std::string(filepath));

        Park& park = context->GetGameState()->GetPark();
        const char* name = park.Name.c_str();

        printf("(me) LoadPark() = %s\n", name);

    }


    EXPORT const char* GetParkNamePtr()
    {
        Park& park = context->GetGameState()->GetPark();
        const char* name = park.Name.c_str();

        printf("(me) GetParkName() = %s\n", name);
        return name;
    }
}
