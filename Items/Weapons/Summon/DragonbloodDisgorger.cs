using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DragonbloodDisgorger : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragonblood Disgorger");
            Tooltip.SetDefault("Summons a skeletal dragon and her two children\n" +
                               "Requires 6 minion slots to be summoned\n" +
                               "There can only be one family");
        }

        public override void SetDefaults()
        {
            item.damage = 300;
            item.mana = 30;
            item.width = 64;
            item.height = 62;
            item.useTime = item.useAnimation = 36;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 4f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 8;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.UseSound = SoundID.DD2_SkeletonDeath;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SkeletalDragonMother>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            return false;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 12);
            r.AddTile(TileID.LunarCraftingStation);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
