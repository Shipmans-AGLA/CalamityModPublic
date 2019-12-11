using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Buffs.StatDebuffs;
namespace CalamityMod.NPCs.AcidRain
{
    public class Radiator : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Radiator");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 67;
            npc.damage = 50;
            npc.width = 24;
            npc.height = 24;
            npc.defense = 5;
            npc.lifeMax = 200;
            npc.knockBackResist = 0f;
            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.value = Item.buyPrice(0, 0, 5, 0);
            npc.lavaImmune = false;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
			aiType = NPCID.GlowingSnail;
			//animationType = NPCID.GlowingSnail;
            //banner = npc.type;
            //bannerItem = ModContent.ItemType<RadiatorBanner>();
        }

        public override void AI()
        {
            if (npc.localAI[0] == 0f)
            {
                npc.catchItem = (Main.rand.NextBool(10)) ? (short)ModContent.ItemType<RadiatingCrystal>() : ItemID.None;
                npc.localAI[0] = 1f;
                npc.velocity.Y = -3f;
                npc.netUpdate = true;
            }
			
            Lighting.AddLight(npc.Center, 0.3f, 1.5f, 0.3f);

            int auraSize = 200; //roughly 12 blocks (half the size of Wither Beast aura)
			Player player = Main.player[Main.myPlayer];
			if (!player.dead && player.active && (double) (player.Center - npc.Center).Length() < auraSize)
			{
				player.AddBuff(ModContent.BuffType<Irradiated>(), 3, false);
				player.AddBuff(BuffID.Poisoned, 2, false);
			}
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter > 8)
            {
                npc.frameCounter = 0;
                npc.frame.Y += frameHeight;
                if (npc.frame.Y > frameHeight * 2)
                {
                    npc.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !spawnInfo.player.Calamity().ZoneSulphur || !Main.raining)
            {
                return 0f;
            }
            return 0.1f;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 75, hitDirection, -1f, 0, default, 1f);
            }
        }

        public override void OnCatchNPC(Player player, Item item)
        {
            try
            {
            } catch
            {
                return;
            }
        }
    }
}
