﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Materials
{
    public class VictideBar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Victide Bar");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 6);
            Item.rare = ItemRarityID.Green;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VictoryShard>().
                AddIngredient(ItemID.Coral).
                AddIngredient(ItemID.Starfish).
                AddIngredient(ItemID.Seashell).
                AddTile(TileID.Furnaces).
                Register();
        }
    }
}
