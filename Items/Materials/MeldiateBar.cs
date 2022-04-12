﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Materials
{
    public class MeldiateBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meld Construct");
        }

        public override void SetDefaults()
        {
            Item.width = 15;
            Item.height = 12;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(gold: 1, silver: 20);
            Item.rare = ItemRarityID.Cyan;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3).
                AddIngredient<MeldBlob>(6).
                AddIngredient<Stardust>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
