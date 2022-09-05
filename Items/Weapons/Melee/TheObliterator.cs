﻿using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.Rarities;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheObliterator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Obliterator");
            Tooltip.SetDefault("Ruins nearby enemies with death lasers\n" +
            "An exceptionally agile yoyo");
            ItemID.Sets.Yoyo[Item.type] = true;
            ItemID.Sets.GamepadExtraRange[Item.type] = 15;
            ItemID.Sets.GamepadSmartQuickReach[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 40;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.damage = 255;
            Item.knockBack = 7.5f;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item1;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<ObliteratorYoyo>();
            Item.shootSpeed = 16f;

            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }
    }
}
