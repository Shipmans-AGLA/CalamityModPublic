﻿using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.SummonItems;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class ArmoredDiggerHead : ModNPC
    {
        bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Armored Digger");
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 90;
            NPC.npcSlots = 10f;
            NPC.width = 54;
            NPC.height = 54;
            NPC.defense = 10;
            NPC.DR_NERD(0.1f);
            NPC.lifeMax = 20000;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.value = Item.buyPrice(0, 5, 0, 0);
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<ArmoredDiggerBanner>();
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
        }

        public override void AI()
        {
            bool death = CalamityWorld.death;
            if (NPC.ai[3] > 0f)
            {
                NPC.realLife = (int)NPC.ai[3];
            }
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest(true);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned)
                {
                    NPC.ai[3] = (float)NPC.whoAmI;
                    NPC.realLife = NPC.whoAmI;
                    int num2 = NPC.whoAmI;
                    int num3 = 30;
                    for (int j = 0; j <= num3; j++)
                    {
                        int num4 = ModContent.NPCType<ArmoredDiggerBody>();
                        if (j == num3)
                        {
                            num4 = ModContent.NPCType<ArmoredDiggerTail>();
                        }
                        int num5 = NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)(NPC.position.X + (float)(NPC.width / 2)), (int)(NPC.position.Y + (float)NPC.height), num4, NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                        Main.npc[num5].ai[3] = (float)NPC.whoAmI;
                        Main.npc[num5].realLife = NPC.whoAmI;
                        Main.npc[num5].ai[1] = (float)num2;
                        Main.npc[num2].ai[0] = (float)num5;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num5, 0f, 0f, 0f, 0, 0, 0);
                        num2 = num5;
                    }
                    TailSpawned = true;
                }
            }
            int num12 = (int)(NPC.position.X / 16f) - 1;
            int num13 = (int)((NPC.position.X + (float)NPC.width) / 16f) + 2;
            int num14 = (int)(NPC.position.Y / 16f) - 1;
            int num15 = (int)((NPC.position.Y + (float)NPC.height) / 16f) + 2;
            if (num12 < 0)
            {
                num12 = 0;
            }
            if (num13 > Main.maxTilesX)
            {
                num13 = Main.maxTilesX;
            }
            if (num14 < 0)
            {
                num14 = 0;
            }
            if (num15 > Main.maxTilesY)
            {
                num15 = Main.maxTilesY;
            }
            bool flag2 = false;
            if (!flag2)
            {
                for (int k = num12; k < num13; k++)
                {
                    for (int l = num14; l < num15; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[k, l].TileType] || (Main.tileSolidTop[(int)Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            Vector2 vector2;
                            vector2.X = (float)(k * 16);
                            vector2.Y = (float)(l * 16);
                            if (NPC.position.X + (float)NPC.width > vector2.X && NPC.position.X < vector2.X + 16f && NPC.position.Y + (float)NPC.height > vector2.Y && NPC.position.Y < vector2.Y + 16f)
                            {
                                flag2 = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!flag2)
            {
                NPC.localAI[1] = 1f;
                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int num16 = 1000;
                bool flag3 = true;
                if (NPC.position.Y > Main.player[NPC.target].position.Y)
                {
                    for (int m = 0; m < 255; m++)
                    {
                        if (Main.player[m].active)
                        {
                            Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X - num16, (int)Main.player[m].position.Y - num16, num16 * 2, num16 * 2);
                            if (rectangle.Intersects(rectangle2))
                            {
                                flag3 = false;
                                break;
                            }
                        }
                    }
                    if (flag3)
                    {
                        flag2 = true;
                    }
                }
            }
            else
            {
                NPC.localAI[1] = 0f;
            }
            float num17 = death ? 13.5f : 10f;
            if (Main.player[NPC.target].dead || (double)Main.player[NPC.target].position.Y < Main.rockLayer * 16.0)
            {
                flag2 = false;
                NPC.velocity.Y = NPC.velocity.Y + 1f;
                if ((double)NPC.position.Y > Main.rockLayer * 16.0)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 1f;
                    num17 = death ? 27f : 20f;
                }
                if ((double)NPC.position.Y > (double)((Main.maxTilesY - 200) * 16))
                {
                    for (int a = 0; a < 200; a++)
                    {
                        if (Main.npc[a].type == ModContent.NPCType<ArmoredDiggerHead>() || Main.npc[a].type == ModContent.NPCType<ArmoredDiggerBody>() ||
                            Main.npc[a].type == ModContent.NPCType<ArmoredDiggerTail>())
                        {
                            Main.npc[a].active = false;
                        }
                    }
                }
            }
            float num18 = death ? 0.0675f : 0.05f;
            float num19 = death ? 0.10125f : 0.075f;
            Vector2 vector3 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float num20 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float num21 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            num20 = (float)((int)(num20 / 16f) * 16);
            num21 = (float)((int)(num21 / 16f) * 16);
            vector3.X = (float)((int)(vector3.X / 16f) * 16);
            vector3.Y = (float)((int)(vector3.Y / 16f) * 16);
            num20 -= vector3.X;
            num21 -= vector3.Y;
            float num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
            if (!flag2)
            {
                NPC.TargetClosest(true);
                NPC.velocity.Y = NPC.velocity.Y + 0.15f;
                if (NPC.velocity.Y > num17)
                {
                    NPC.velocity.Y = num17;
                }
                if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)num17 * 0.4)
                {
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X - num18 * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X + num18 * 1.1f;
                    }
                }
                else if (NPC.velocity.Y == num17)
                {
                    if (NPC.velocity.X < num20)
                    {
                        NPC.velocity.X = NPC.velocity.X + num18;
                    }
                    else if (NPC.velocity.X > num20)
                    {
                        NPC.velocity.X = NPC.velocity.X - num18;
                    }
                }
                else if (NPC.velocity.Y > 4f)
                {
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X + num18 * 0.9f;
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X - num18 * 0.9f;
                    }
                }
            }
            else
            {
                if (NPC.soundDelay == 0)
                {
                    float num24 = num22 / 40f;
                    if (num24 < 10f)
                    {
                        num24 = 10f;
                    }
                    if (num24 > 20f)
                    {
                        num24 = 20f;
                    }
                    NPC.soundDelay = (int)num24;
                    SoundEngine.PlaySound(SoundID.Roar, (int)NPC.position.X, (int)NPC.position.Y, 1, 1f, 0f);
                }
                num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
                float num25 = Math.Abs(num20);
                float num26 = Math.Abs(num21);
                float num27 = num17 / num22;
                num20 *= num27;
                num21 *= num27;
                if (((NPC.velocity.X > 0f && num20 > 0f) || (NPC.velocity.X < 0f && num20 < 0f)) && ((NPC.velocity.Y > 0f && num21 > 0f) || (NPC.velocity.Y < 0f && num21 < 0f)))
                {
                    if (NPC.velocity.X < num20)
                    {
                        NPC.velocity.X = NPC.velocity.X + num19;
                    }
                    else if (NPC.velocity.X > num20)
                    {
                        NPC.velocity.X = NPC.velocity.X - num19;
                    }
                    if (NPC.velocity.Y < num21)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + num19;
                    }
                    else if (NPC.velocity.Y > num21)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num19;
                    }
                }
                if ((NPC.velocity.X > 0f && num20 > 0f) || (NPC.velocity.X < 0f && num20 < 0f) || (NPC.velocity.Y > 0f && num21 > 0f) || (NPC.velocity.Y < 0f && num21 < 0f))
                {
                    if (NPC.velocity.X < num20)
                    {
                        NPC.velocity.X = NPC.velocity.X + num18;
                    }
                    else if (NPC.velocity.X > num20)
                    {
                        NPC.velocity.X = NPC.velocity.X - num18;
                    }
                    if (NPC.velocity.Y < num21)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + num18;
                    }
                    else if (NPC.velocity.Y > num21)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num18;
                    }
                    if ((double)Math.Abs(num21) < (double)num17 * 0.2 && ((NPC.velocity.X > 0f && num20 < 0f) || (NPC.velocity.X < 0f && num20 > 0f)))
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + num18 * 2f;
                        }
                        else
                        {
                            NPC.velocity.Y = NPC.velocity.Y - num18 * 2f;
                        }
                    }
                    if ((double)Math.Abs(num20) < (double)num17 * 0.2 && ((NPC.velocity.Y > 0f && num21 < 0f) || (NPC.velocity.Y < 0f && num21 > 0f)))
                    {
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + num18 * 2f;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - num18 * 2f;
                        }
                    }
                }
                else if (num25 > num26)
                {
                    if (NPC.velocity.X < num20)
                    {
                        NPC.velocity.X = NPC.velocity.X + num18 * 1.1f;
                    }
                    else if (NPC.velocity.X > num20)
                    {
                        NPC.velocity.X = NPC.velocity.X - num18 * 1.1f;
                    }
                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)num17 * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + num18;
                        }
                        else
                        {
                            NPC.velocity.Y = NPC.velocity.Y - num18;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.Y < num21)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + num18 * 1.1f;
                    }
                    else if (NPC.velocity.Y > num21)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - num18 * 1.1f;
                    }
                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)num17 * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + num18;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - num18;
                        }
                    }
                }
            }
            NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + 1.57f;
            if (flag2)
            {
                if (NPC.localAI[0] != 1f)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[0] = 1f;
            }
            else
            {
                if (NPC.localAI[0] != 0f)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[0] = 0f;
            }
            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 6, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 6, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<ArmoredDiggerHead>(),
                ModContent.NPCType<ArmoredDiggerBody>(),
                ModContent.NPCType<ArmoredDiggerTail>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<DemonicBoneAsh>(), 1, 2, 4);
            npcLoot.Add(ModContent.ItemType<MysteriousCircuitry>(), 1, 4, 8);
            npcLoot.Add(ModContent.ItemType<DubiousPlating>(), 1, 4, 8);
        }
    }
}
