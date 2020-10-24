#include "../../openrct2/core/Path.hpp"

#include <openrct2/Context.h>
#include <openrct2/Game.h>
#include <openrct2/GameState.h>
#include <openrct2/OpenRCT2.h>
#include <openrct2/platform/platform.h>
#include <openrct2/world/Park.h>

#include "Openrct2-dll.h"


std::unique_ptr<IContext> unityContext;

extern "C"
{
    EXPORT void StartGame(const char* datapath, const char* rct2path, const char* rct1path)
    {
        if (unityContext != nullptr)
            return;

        printf("(me) StartGame( %s )\n", datapath);
        _log_levels[static_cast<uint8_t>(DiagnosticLevel::Verbose)] = true;

        gOpenRCT2Headless = true;

        Path::GetAbsolute(gCustomOpenRCT2DataPath, std::size(gCustomOpenRCT2DataPath), datapath);

        if (rct1path != nullptr)
            Path::GetAbsolute(gCustomRCT1DataPath, std::size(gCustomRCT1DataPath), rct1path);
        if (rct2path != nullptr)
            Path::GetAbsolute(gCustomRCT2DataPath, std::size(gCustomRCT2DataPath), rct2path);

        printf(
            "(me) gCustomOpenrctDataPath = %s\n(me) gCustomRCT1DataPath = %s\n(me) gCustomRCT2DataPath = %s\n",
            gCustomOpenRCT2DataPath, gCustomRCT1DataPath, gCustomRCT2DataPath
        );

        // Create a plain context
        core_init();
        unityContext = CreateContext(); // OpenRCT2::CreateContext()
        bool result = unityContext->Initialise();

        printf("(me) Initialise = %i\n", result);
    }


    EXPORT void PerformGameUpdate()
    {
        unityContext->GetGameState()->Update();
    }


    EXPORT void StopGame()
    {
        printf("(me) StopGame()\n");

        unityContext->Finish();
        unityContext = nullptr;
    }


    EXPORT void LoadPark(const char* filepath)
    {
        printf("(me) LoadPark( %s )\n", filepath);

        unityContext->LoadParkFromFile(std::string(filepath));

        Park& park = unityContext->GetGameState()->GetPark();
        const char* name = park.Name.c_str();

        printf("(me) LoadPark() = %s\n", name);

    }


    EXPORT const char* GetParkNamePtr()
    {
        Park& park = unityContext->GetGameState()->GetPark();
        const char* name = park.Name.c_str();

        printf("(me) GetParkName() = %s\n", name);
        return name;
    }
}
