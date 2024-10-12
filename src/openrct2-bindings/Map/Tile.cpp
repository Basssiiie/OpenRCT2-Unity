#include "../OpenRCT2.Bindings.h"
#include "../Utilities/Logging.h"

#include <openrct2/world/Map.h>

extern "C"
{
    struct TileCounts
    {
        uint16_t total;
        uint16_t surfaces;
        uint16_t paths;
        uint16_t tracks;
        uint16_t smallScenery;
        uint16_t entrances;
        uint16_t walls;
        uint16_t largeScenery;
        uint16_t banners;
    };

    EXPORT void GetElementCounts(int x, int y, TileCounts* counts)
    {
        const TileElement* element = MapGetFirstElementAt(TileCoordsXY{ x, y });

        do
        {
            if (element == nullptr)
                break;

            counts->total++;

            const TileElementType type = element->GetType();

            switch (type)
            {
                case TileElementType::Surface:
                    counts->surfaces++;
                    continue;

                case TileElementType::Path:
                    counts->paths++;
                    continue;

                case TileElementType::Track:
                    counts->tracks++;
                    continue;

                case TileElementType::SmallScenery:
                    counts->smallScenery++;
                    continue;

                case TileElementType::Entrance:
                    counts->entrances++;
                    continue;

                case TileElementType::Wall:
                    counts->walls++;
                    continue;

                case TileElementType::LargeScenery:
                    counts->largeScenery++;
                    continue;

                case TileElementType::Banner:
                    counts->banners++;
                    continue;
            }
        } while (!(element++)->IsLastForTile());
    }
}
