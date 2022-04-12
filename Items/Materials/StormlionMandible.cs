﻿using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Materials
{
    public class StormlionMandible : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormlion Mandible");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 8));
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 38;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 1, copper: 40);
            Item.rare = ItemRarityID.Blue;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }
    }
}
