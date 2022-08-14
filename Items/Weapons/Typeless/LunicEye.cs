﻿using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Sounds;

namespace CalamityMod.Items.Weapons.Typeless
{
    public class LunicEye : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunic Eye");
            Tooltip.SetDefault("Fires lunic beams that reduce enemy protection\n" +
                "This weapon scales with all your damage stats at once");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DamageType = AverageDamageClass.Instance;
            Item.width = 60;
            Item.damage = 9;
            Item.value = CalamityGlobalItem.Rarity4BuyPrice;
            Item.rare = ItemRarityID.LightRed;
            Item.useAnimation = 15;
            Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4.5f;
            Item.UseSound = CommonCalamitySounds.LaserCannonSound;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.height = 36;
            Item.shoot = ModContent.ProjectileType<LunicBeam>();
            Item.shootSpeed = 12f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Stardust>(20).
                AddIngredient<SeaPrism>(15).
                AddIngredient<AerialiteBar>(15).
                AddIngredient(ItemID.SunplateBlock, 15).
                AddIngredient<PearlShard>(2).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
