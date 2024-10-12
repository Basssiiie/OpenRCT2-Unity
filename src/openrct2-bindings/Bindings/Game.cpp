#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"

#include <openrct2/Context.h>
#include <openrct2/Diagnostic.h>
#include <openrct2/GameState.h>
#include <openrct2/OpenRCT2.h>
#include <openrct2/core/Path.hpp>
#include <openrct2/scenes/Scene.h>
#include <openrct2/world/Park.h>

std::unique_ptr<IContext> unityContext;

extern "C"
{
    EXPORT void StartGame(const char* datapath, const char* rct2path, const char* rct1path)
    {
        if (unityContext != nullptr)
            return;

        dll_log("StartGame( %s )", datapath);
        _log_levels[EnumValue(DiagnosticLevel::Verbose)] = true;

        gOpenRCT2Headless = true;
        gCustomOpenRCT2DataPath = Path::GetAbsolute(datapath);

        if (rct1path != nullptr)
        {
            gCustomRCT1DataPath = Path::GetAbsolute(rct1path);
        }
        if (rct2path != nullptr)
        {
            gCustomRCT2DataPath = Path::GetAbsolute(rct2path);
        }

        dll_log(
            "gCustomOpenrctDataPath = %s\n(me) gCustomRCT1DataPath = %s\n(me) gCustomRCT2DataPath = %s", datapath, rct1path,
            rct2path);

        // Create a plain context
        unityContext = CreateContext(); // OpenRCT2::CreateContext()
        bool result = unityContext->Initialise();

        dll_log("Initialise = %i", result);
    }

    EXPORT void PerformGameUpdate()
    {
        unityContext->GetActiveScene()->Tick();
    }

    EXPORT void StopGame()
    {
        dll_log("StopGame()");

        if (unityContext != nullptr)
        {
            unityContext->Finish();
            unityContext = nullptr;
        }
    }

    static const char* GetActiveParkName()
    {
        const Park::ParkData& park = unityContext->GetActiveScene()->GetGameState().Park;
        const char* name = park.Name.c_str();
        return name;
    }

    EXPORT void LoadPark(const char* filepath)
    {
        dll_log("LoadPark( %s )", filepath);

        unityContext->LoadParkFromFile(std::string(filepath));
        unityContext->SetActiveScene(unityContext->GetGameScene());

        dll_log("LoadPark() = %s", GetActiveParkName());
    }

    EXPORT const char* GetParkName()
    {
        const char* name = GetActiveParkName();
        dll_log("GetParkName() = %s", name);
        return name;
    }
}
