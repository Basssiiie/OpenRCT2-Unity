/*****************************************************************************
 * Copyright (c) 2014-2025 OpenRCT2 developers
 *
 * For a complete list of all authors, please refer to contributors.md
 * Interested in contributing? Visit https://github.com/OpenRCT2/OpenRCT2
 *
 * OpenRCT2 is licensed under the GNU General Public License version 3.
 *****************************************************************************/

#pragma once

#ifdef ENABLE_SCRIPTING_REFACTOR

    #include "../../../Context.h"
    #include "../../../GameState.h"
    #include "../../../entity/EntityList.h"
    #include "../../../entity/EntityRegistry.h"
    #include "../../../entity/Peep.h"
    #include "../../ScriptEngine.h"

    #include <string_view>
    #include <unordered_map>

namespace OpenRCT2::Scripting
{
    inline std::string EntityTypeToString(const EntityBase* entity)
    {
        const auto targetApiVersion = GetTargetAPIVersion();

        if (entity != nullptr)
        {
            switch (entity->Type)
            {
                case EntityType::Vehicle:
                    return "car";
                case EntityType::Guest:
                    if (targetApiVersion <= kApiVersionPeepDeprecation)
                        return "peep";
                    return "guest";
                case EntityType::Staff:
                    if (targetApiVersion <= kApiVersionPeepDeprecation)
                        return "peep";
                    return "staff";
                case EntityType::SteamParticle:
                    return "steam_particle";
                case EntityType::MoneyEffect:
                    return "money_effect";
                case EntityType::CrashedVehicleParticle:
                    return "crashed_vehicle_particle";
                case EntityType::ExplosionCloud:
                    return "explosion_cloud";
                case EntityType::CrashSplash:
                    return "crash_splash";
                case EntityType::ExplosionFlare:
                    return "explosion_flare";
                case EntityType::Balloon:
                    return "balloon";
                case EntityType::Duck:
                    return "duck";
                case EntityType::JumpingFountain:
                    return "jumping_fountain";
                case EntityType::Litter:
                    return "litter";
                case EntityType::Null:
                    return "unknown";
                default:
                    break;
            }
        }
        return "unknown";
    }

    class ScEntity;
    extern ScEntity gScEntity;

    using OpaqueEntityData = struct
    {
        EntityId id;
    };

    class ScEntity : public ScBase
    {
    private:
        static JSValue id_get(JSContext* ctx, JSValue thisVal)
        {
            auto entity = GetEntity(thisVal);
            if (entity == nullptr)
                return JS_UNDEFINED;

            return JS_NewInt32(ctx, entity->Id.ToUnderlying());
        }

        static JSValue type_get(JSContext* ctx, JSValue thisVal)
        {
            auto entity = GetEntity(thisVal);
            auto type = EntityTypeToString(entity);
            return JS_NewString(ctx, type.c_str());
        }

        // x getter and setter
        static JSValue x_get(JSContext* ctx, JSValue thisVal)
        {
            auto entity = GetEntity(thisVal);
            return JS_NewInt32(ctx, entity != nullptr ? entity->x : 0);
        }
        static JSValue x_set(JSContext* ctx, JSValue thisVal, JSValue jsValue)
        {
            JS_UNPACK_INT32(value, ctx, jsValue);
            JS_THROW_IF_GAME_STATE_NOT_MUTABLE();
            auto entity = GetEntity(thisVal);
            if (entity != nullptr)
            {
                entity->MoveTo({ value, entity->y, entity->z });
            }
            return JS_UNDEFINED;
        }

        // y getter and setter
        static JSValue y_get(JSContext* ctx, JSValue thisVal)
        {
            auto entity = GetEntity(thisVal);
            return JS_NewInt32(ctx, entity != nullptr ? entity->y : 0);
        }
        static JSValue y_set(JSContext* ctx, JSValue thisVal, JSValue jsValue)
        {
            JS_UNPACK_INT32(value, ctx, jsValue);
            JS_THROW_IF_GAME_STATE_NOT_MUTABLE();
            auto entity = GetEntity(thisVal);
            if (entity != nullptr)
            {
                entity->MoveTo({ entity->x, value, entity->z });
            }
            return JS_UNDEFINED;
        }

        // z getter and setter
        static JSValue z_get(JSContext* ctx, JSValue thisVal)
        {
            auto entity = GetEntity(thisVal);
            return JS_NewInt32(ctx, entity != nullptr ? entity->z : 0);
        }
        static JSValue z_set(JSContext* ctx, JSValue thisVal, JSValue jsValue)
        {
            JS_UNPACK_INT32(value, ctx, jsValue);
            JS_THROW_IF_GAME_STATE_NOT_MUTABLE();
            auto entity = GetEntity(thisVal);
            if (entity != nullptr)
            {
                entity->MoveTo({ entity->x, entity->y, value });
            }
            return JS_UNDEFINED;
        }

        static JSValue remove(JSContext* ctx, JSValue thisVal, int argc, JSValue* argv)
        {
            auto entity = GetEntity(thisVal);
            if (entity != nullptr)
            {
                entity->Invalidate();
                switch (entity->Type)
                {
                    case EntityType::Vehicle:
                        JS_ThrowPlainError(ctx, "Removing a vehicle is currently unsupported.");
                        break;
                    case EntityType::Guest:
                    case EntityType::Staff:
                    {
                        auto peep = entity->As<Peep>();
                        // We can't remove a single peep from a ride at the moment as this can cause complications with the
                        // vehicle car having an unsupported peep capacity.
                        if (peep == nullptr || peep->State == PeepState::OnRide || peep->State == PeepState::EnteringRide)
                        {
                            JS_ThrowPlainError(ctx, "Removing a peep that is on a ride is currently unsupported.");
                        }
                        else
                        {
                            peep->Remove();
                        }
                        break;
                    }
                    case EntityType::SteamParticle:
                    case EntityType::MoneyEffect:
                    case EntityType::CrashedVehicleParticle:
                    case EntityType::ExplosionCloud:
                    case EntityType::CrashSplash:
                    case EntityType::ExplosionFlare:
                    case EntityType::JumpingFountain:
                    case EntityType::Balloon:
                    case EntityType::Duck:
                    case EntityType::Litter:
                        getGameState().entities.EntityRemove(entity);
                        break;
                    case EntityType::Null:
                        break;
                    default:
                        break;
                }
            }
            return JS_UNDEFINED;
        }

        static EntityBase* GetEntity(JSValue thisVal) 
        {
            OpaqueEntityData* data = gScEntity.GetOpaque<OpaqueEntityData*>(thisVal);
            return OpenRCT2::getGameState().entities.GetEntity(_id);
        }

    public:
        JSValue New(JSContext* ctx)
        {
            static constexpr JSCFunctionListEntry funcs[] = {
                JS_CGETSET_DEF("id", &ScEntity::id_get, nullptr),
                JS_CGETSET_DEF("type", &ScEntity::type_get, nullptr),
                JS_CGETSET_DEF("x", &ScEntity::x_get, &ScEntity::x_set),
                JS_CGETSET_DEF("y", &ScEntity::y_get, &ScEntity::y_set),
                JS_CGETSET_DEF("z", &ScEntity::z_get, &ScEntity::z_set),
                JS_CFUNC_DEF("remove", 0, &ScEntity::remove)
            };
            return MakeWithOpaque(ctx, funcs, nullptr);
        }

        void Register(JSContext* ctx)
        {
            RegisterBaseStr(ctx, "Cheats");
        }
    };

} // namespace OpenRCT2::Scripting

#endif
