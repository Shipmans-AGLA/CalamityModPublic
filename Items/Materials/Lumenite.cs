﻿using CalamityMod.Tiles.Abyss;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Materials
{
    public class Lumenite : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lumenyl");
            Tooltip.SetDefault("A shard of lumenous energy");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<LumenylCrystals>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 80);
            Item.rare = ItemRarityID.Lime;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
    }
}
