﻿using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Summon;
using CalamityMod.World;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class HeartoftheElements : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heart of the Elements");
            Tooltip.SetDefault("The heart of the world\n" +
                "Increases max life by 20, life regen by 1, and all damage by 8%\n" +
                "Increases movement speed by 10% and jump speed by 100%\n" +
                "Increases damage reduction by 5%\n" +
                "Increases max mana by 50 and reduces mana usage by 5%\n" +
                "You grow flowers on the grass beneath you, chance to grow very random dye plants on grassless dirt\n" +
                "Summons all elementals to protect you\n" +
                "Toggling the visibility of this accessory also toggles the elementals on and off\n" +
                "Stat increases are slightly higher if the elementals are turned off");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(5, 8));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.defense = 9;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.Rainbow;
        }

        public override bool CanEquipAccessory(Player player, int slot)
        {
            CalamityPlayer modPlayer = player.Calamity();
            if (modPlayer.brimstoneWaifu || modPlayer.sandWaifu || modPlayer.sandBoobWaifu || modPlayer.cloudWaifu || modPlayer.sirenWaifu)
            {
                return false;
            }
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            Lighting.AddLight((int)player.Center.X / 16, (int)player.Center.Y / 16, (float)Main.DiscoR / 255f, (float)Main.DiscoG / 255f, (float)Main.DiscoB / 255f);
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.allWaifus = !hideVisual;
            modPlayer.elementalHeart = true;
            if (!hideVisual)
            {
                player.lifeRegen += 1;
                player.statLifeMax2 += 20;
                player.moveSpeed += 0.1f;
                player.jumpSpeedBoost += 1.0f;
                player.endurance += 0.05f;
                player.statManaMax2 += 50;
                player.manaCost *= 0.95f;
                player.allDamage += 0.08f;

                int damage = NPC.downedMoonlord ? 150 : 90;
                float damageMult = CalamityWorld.downedDoG ? 2f : 1f;
				int elementalDmg = (int)(damage * damageMult * player.MinionDamage());

                if (player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneElementalMinion>()] > 1 || player.ownedProjectileCounts[ModContent.ProjectileType<WaterElementalMinion>()] > 1 ||
                    player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalHealer>()] > 1 || player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalMinion>()] > 1 ||
                    player.ownedProjectileCounts[ModContent.ProjectileType<CloudElementalMinion>()] > 1)
                {
                    player.ClearBuff(ModContent.BuffType<HotE>());
                }
                if (player.FindBuffIndex(ModContent.BuffType<HotE>()) == -1)
                {
                    player.AddBuff(ModContent.BuffType<HotE>(), 3600, true);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneElementalMinion>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<BrimstoneElementalMinion>(), elementalDmg, 2f, Main.myPlayer, 0f, 0f);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<WaterElementalMinion>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<WaterElementalMinion>(), elementalDmg, 2f, Main.myPlayer, 0f, 0f);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalHealer>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<SandElementalHealer>(), elementalDmg, 2f, Main.myPlayer, 0f, 0f);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalMinion>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<SandElementalMinion>(), elementalDmg, 2f, Main.myPlayer, 0f, 0f);
                }
                if (player.ownedProjectileCounts[ModContent.ProjectileType<CloudElementalMinion>()] < 1)
                {
                    Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, ModContent.ProjectileType<CloudElementalMinion>(), elementalDmg, 2f, Main.myPlayer, 0f, 0f);
                }
            }
            else
            {
                player.lifeRegen += 2;
                player.statLifeMax2 += 25;
                player.moveSpeed += 0.12f;
                player.jumpSpeedBoost += 1.1f;
                player.endurance += 0.06f;
                player.statManaMax2 += 60;
                player.manaCost *= 0.93f;
                player.allDamage += 0.1f;
                if (player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneElementalMinion>()] > 0 || player.ownedProjectileCounts[ModContent.ProjectileType<WaterElementalMinion>()] > 0 ||
                    player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalHealer>()] > 0 || player.ownedProjectileCounts[ModContent.ProjectileType<SandElementalMinion>()] > 0 ||
                    player.ownedProjectileCounts[ModContent.ProjectileType<CloudElementalMinion>()] > 0)
                {
                    player.ClearBuff(ModContent.BuffType<HotE>());
                }
            }
            if (player.velocity.Y == 0f && player.grappling[0] == -1)
            {
                int num4 = (int)player.Center.X / 16;
                int num5 = (int)(player.position.Y + (float)player.height - 1f) / 16;
                if (Main.tile[num4, num5] == null)
                {
                    Main.tile[num4, num5] = new Tile();
                }
                if (!Main.tile[num4, num5].active() && Main.tile[num4, num5].liquid == 0 && Main.tile[num4, num5 + 1] != null && WorldGen.SolidTile(num4, num5 + 1))
                {
                    Main.tile[num4, num5].frameY = 0;
                    Main.tile[num4, num5].slope(0);
                    Main.tile[num4, num5].halfBrick(false);
                    if (Main.tile[num4, num5 + 1].type == 0)
                    {
                        if (Main.rand.NextBool(1000))
                        {
                            Main.tile[num4, num5].active(true);
                            Main.tile[num4, num5].type = 227;
                            Main.tile[num4, num5].frameX = (short)(34 * Main.rand.Next(1, 13));
                            while (Main.tile[num4, num5].frameX == 144)
                            {
                                Main.tile[num4, num5].frameX = (short)(34 * Main.rand.Next(1, 13));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, num4, num5, 1, TileChangeType.None);
                        }
                    }
                    if (Main.tile[num4, num5 + 1].type == 2)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            Main.tile[num4, num5].active(true);
                            Main.tile[num4, num5].type = 3;
                            Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(6, 11));
                            while (Main.tile[num4, num5].frameX == 144)
                            {
                                Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(6, 11));
                            }
                        }
                        else
                        {
                            Main.tile[num4, num5].active(true);
                            Main.tile[num4, num5].type = 73;
                            Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(6, 21));
                            while (Main.tile[num4, num5].frameX == 144)
                            {
                                Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(6, 21));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, num4, num5, 1, TileChangeType.None);
                        }
                    }
                    else if (Main.tile[num4, num5 + 1].type == 109)
                    {
                        if (Main.rand.NextBool(2))
                        {
                            Main.tile[num4, num5].active(true);
                            Main.tile[num4, num5].type = 110;
                            Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(4, 7));
                            while (Main.tile[num4, num5].frameX == 90)
                            {
                                Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(4, 7));
                            }
                        }
                        else
                        {
                            Main.tile[num4, num5].active(true);
                            Main.tile[num4, num5].type = 113;
                            Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(2, 8));
                            while (Main.tile[num4, num5].frameX == 90)
                            {
                                Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(2, 8));
                            }
                        }
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, num4, num5, 1, TileChangeType.None);
                        }
                    }
                    else if (Main.tile[num4, num5 + 1].type == 60)
                    {
                        Main.tile[num4, num5].active(true);
                        Main.tile[num4, num5].type = 74;
                        Main.tile[num4, num5].frameX = (short)(18 * Main.rand.Next(9, 17));
                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            NetMessage.SendTileSquare(-1, num4, num5, 1, TileChangeType.None);
                        }
                    }
                }
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WifeinaBottle>());
            recipe.AddIngredient(ModContent.ItemType<WifeinaBottlewithBoobs>());
            recipe.AddIngredient(ModContent.ItemType<LureofEnthrallment>());
            recipe.AddIngredient(ModContent.ItemType<EyeoftheStorm>());
            recipe.AddIngredient(ModContent.ItemType<RoseStone>());
            recipe.AddIngredient(ModContent.ItemType<AeroStone>());
            recipe.AddIngredient(ModContent.ItemType<CryoStone>());
            recipe.AddIngredient(ModContent.ItemType<ChaosStone>());
            recipe.AddIngredient(ModContent.ItemType<BloomStone>());
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
