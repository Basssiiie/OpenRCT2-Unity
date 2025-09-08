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

    #include "../../../entity/Guest.h"
    #include "../../../management/Marketing.h"
    #include "ScPeep.hpp"

enum class PeepAnimationType : uint8_t;

namespace OpenRCT2::Scripting
{
    static const EnumMap<ShopItem> ShopItemMap(
        {
            { "beef_noodles", ShopItem::BeefNoodles },
            { "burger", ShopItem::Burger },
            { "candyfloss", ShopItem::Candyfloss },
            { "chicken", ShopItem::Chicken },
            { "chips", ShopItem::Chips },
            { "chocolate", ShopItem::Chocolate },
            { "cookie", ShopItem::Cookie },
            { "doughnut", ShopItem::Doughnut },
            { "hot_dog", ShopItem::HotDog },
            { "fried_rice_noodles", ShopItem::FriedRiceNoodles },
            { "funnel_cake", ShopItem::FunnelCake },
            { "ice_cream", ShopItem::IceCream },
            { "meatball_soup", ShopItem::MeatballSoup },
            { "pizza", ShopItem::Pizza },
            { "popcorn", ShopItem::Popcorn },
            { "pretzel", ShopItem::Pretzel },
            { "roast_sausage", ShopItem::RoastSausage },
            { "sub_sandwich", ShopItem::SubSandwich },
            { "tentacle", ShopItem::Tentacle },
            { "toffee_apple", ShopItem::ToffeeApple },
            { "wonton_soup", ShopItem::WontonSoup },
            { "coffee", ShopItem::Coffee },
            { "drink", ShopItem::Drink },
            { "fruit_juice", ShopItem::FruitJuice },
            { "iced_tea", ShopItem::IcedTea },
            { "lemonade", ShopItem::Lemonade },
            { "soybean_milk", ShopItem::SoybeanMilk },
            { "sujeonggwa", ShopItem::Sujeonggwa },
            { "balloon", ShopItem::Balloon },
            { "hat", ShopItem::Hat },
            { "map", ShopItem::Map },
            { "sunglasses", ShopItem::Sunglasses },
            { "toy", ShopItem::Toy },
            { "tshirt", ShopItem::TShirt },
            { "umbrella", ShopItem::Umbrella },
            { "photo1", ShopItem::Photo },
            { "photo2", ShopItem::Photo2 },
            { "photo3", ShopItem::Photo3 },
            { "photo4", ShopItem::Photo4 },
            { "voucher", ShopItem::Voucher },
            { "empty_bottle", ShopItem::EmptyBottle },
            { "empty_bowl_blue", ShopItem::EmptyBowlBlue },
            { "empty_bowl_red", ShopItem::EmptyBowlRed },
            { "empty_box", ShopItem::EmptyBox },
            { "empty_burger_box", ShopItem::EmptyBurgerBox },
            { "empty_can", ShopItem::EmptyCan },
            { "empty_cup", ShopItem::EmptyCup },
            { "empty_drink_carton", ShopItem::EmptyDrinkCarton },
            { "empty_juice_cup", ShopItem::EmptyJuiceCup },
            { "rubbish", ShopItem::Rubbish },
        });
    // Since the ShopItem enum is missing values and includes ShopItem::Admission (something a
    // guest cannot carry), 6 is subtracted from the value.
    static_assert((EnumValue(ShopItem::Count) - 6) == 50, "ShopItem::Count changed, update scripting binding!");

    static const EnumMap<uint32_t> VoucherTypeMap(
        {
            { "entry_free", VOUCHER_TYPE_PARK_ENTRY_FREE },
            { "entry_half_price", VOUCHER_TYPE_PARK_ENTRY_HALF_PRICE },
            { "ride_free", VOUCHER_TYPE_RIDE_FREE },
            { "food_drink_free", VOUCHER_TYPE_FOOD_OR_DRINK_FREE },
        });

    class ScThought;
    extern ScThought gScThought;

    class ScThought final : public ScBase
    {
    public:
        JSValue New(JSContext* ctx, PeepThought thought);
        void Register(JSContext* ctx);

    private:
        static void Finalize(JSRuntime* rt, JSValue thisVal);
        static PeepThought GetThought(JSValue thisVal);

        static JSValue type_get(JSContext* ctx, JSValue thisVal);
        static JSValue item_get(JSContext* ctx, JSValue thisVal);
        static JSValue freshness_get(JSContext* ctx, JSValue thisVal);
        static JSValue freshTimeout_get(JSContext* ctx, JSValue thisVal);
        static JSValue toString(JSContext* ctx, JSValue thisVal, int argc, JSValue* argv);
    };

    class ScGuest final : public ScPeep
    {
    public:
        static void AddFuncs(JSContext* ctx, JSValue obj);

    private:
        static Guest* GetGuest(JSValue thisVal);

        static JSValue tshirtColour_get(JSContext* ctx, JSValue thisVal);
        static JSValue tshirtColour_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue trousersColour_get(JSContext* ctx, JSValue thisVal);
        static JSValue trousersColour_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue balloonColour_get(JSContext* ctx, JSValue thisVal);
        static JSValue balloonColour_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue hatColour_get(JSContext* ctx, JSValue thisVal);
        static JSValue hatColour_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue umbrellaColour_get(JSContext* ctx, JSValue thisVal);
        static JSValue umbrellaColour_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue happiness_get(JSContext* ctx, JSValue thisVal);
        static JSValue happiness_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue happinessTarget_get(JSContext* ctx, JSValue thisVal);
        static JSValue happinessTarget_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue nausea_get(JSContext* ctx, JSValue thisVal);
        static JSValue nausea_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue nauseaTarget_get(JSContext* ctx, JSValue thisVal);
        static JSValue nauseaTarget_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue hunger_get(JSContext* ctx, JSValue thisVal);
        static JSValue hunger_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue thirst_get(JSContext* ctx, JSValue thisVal);
        static JSValue thirst_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue toilet_get(JSContext* ctx, JSValue thisVal);
        static JSValue toilet_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue mass_get(JSContext* ctx, JSValue thisVal);
        static JSValue mass_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue minIntensity_get(JSContext* ctx, JSValue thisVal);
        static JSValue minIntensity_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue maxIntensity_get(JSContext* ctx, JSValue thisVal);
        static JSValue maxIntensity_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue nauseaTolerance_get(JSContext* ctx, JSValue thisVal);
        static JSValue nauseaTolerance_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue cash_get(JSContext* ctx, JSValue thisVal);
        static JSValue cash_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue isInPark_get(JSContext* ctx, JSValue thisVal);

        static JSValue isLost_get(JSContext* ctx, JSValue thisVal);

        static JSValue lostCountdown_get(JSContext* ctx, JSValue thisVal);
        static JSValue lostCountdown_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue favouriteRide_get(JSContext* ctx, JSValue thisVal);
        static JSValue favouriteRide_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);

        static JSValue thoughts_get(JSContext* ctx, JSValue thisVal);

        static JSValue items_get(JSContext* ctx, JSValue thisVal);
        static bool has_item(JSContext* ctx, JSValue thisVal, JSValue jsValue);
        static JSValue has_item(JSContext* ctx, JSValue thisVal, int argc, JSValue* argv);
        static JSValue give_item(JSContext* ctx, JSValue thisVal, int argc, JSValue* argv);
        static JSValue remove_item(JSContext* ctx, JSValue thisVal, int argc, JSValue* argv);
        static JSValue remove_all_items(JSContext* ctx, JSValue thisVal, int argc, JSValue* argv);

        static JSValue availableAnimations_get(JSContext* ctx, JSValue thisVal);
        static JSValue getAnimationSpriteIds(JSContext* ctx, JSValue thisVal, int argc, JSValue* argv);
        static JSValue animation_get(JSContext* ctx, JSValue thisVal);
        static JSValue animation_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);
        static JSValue animationOffset_get(JSContext* ctx, JSValue thisVal);
        static JSValue animationOffset_set(JSContext* ctx, JSValue thisVal, JSValue jsValue);
        static JSValue animationLength_get(JSContext* ctx, JSValue thisVal);
    };

} // namespace OpenRCT2::Scripting

#endif
