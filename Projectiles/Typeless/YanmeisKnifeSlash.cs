﻿using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Typeless;

namespace CalamityMod.Projectiles.Typeless
{
    public class YanmeisKnifeSlash : ModProjectile
    {
        // This is a rather weird thing, but it's what the patron asked for.
        public static readonly Func<NPC, bool> CanRecieveCoolEffectsFrom = (npc) =>
        {
            bool validBoss = npc.boss && npc.type != ModContent.NPCType<DevourerofGodsBody>()
                && npc.type != ModContent.NPCType<AstrumDeusBody>()
                && npc.type != ModContent.NPCType<ThanatosHead>()
                && npc.type != ModContent.NPCType<ThanatosBody1>()
                && npc.type != ModContent.NPCType<ThanatosBody2>()
                && npc.type != ModContent.NPCType<ThanatosTail>();
            if (validBoss)
                return true;
            bool bossMinion = CalamityLists.bossMinionList.Contains(npc.type);
            if (bossMinion)
                return true;
            bool miniboss = CalamityLists.minibossList.Contains(npc.type) || AcidRainEvent.AllMinibosses.Contains(npc.type);
            return miniboss;
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yanmei's Knife");
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 180;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Frames and crap
            Projectile.frameCounter++;
            if (Projectile.frameCounter % 7 == 0)
            {
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.Kill();
            }

            // Create idle light and dust.
            Vector2 origin = Projectile.Center + Projectile.velocity * 3f;
            Lighting.AddLight(origin, 0f, 1.5f, 0.1f);

            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);

            // Rotation and directioning.
            float velocityAngle = Projectile.velocity.ToRotation();
            Projectile.rotation = velocityAngle + (Projectile.spriteDirection == -1).ToInt() * MathHelper.Pi;
            Projectile.direction = (Math.Cos(velocityAngle) > 0).ToDirectionInt();

            // Positioning close to the end of the player's arm.
            Projectile.position = playerRotatedPoint - Projectile.Size * 0.5f + velocityAngle.ToRotationVector2() * 80f;

            // Sprite and player directioning.
            Projectile.spriteDirection = Projectile.direction;
            player.ChangeDir(Projectile.direction);

            // Prevents the projectile from dying
            Projectile.timeLeft = 2;

            // Player item-based field manipulation.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }

        public void HandleAttachmentMovement(Player player, Vector2 playerRotatedPoint)
        {
            float speed = 1f;
            if (player.ActiveItem().shoot == Projectile.type)
            {
                speed = player.ActiveItem().shootSpeed * Projectile.scale;
            }
            Vector2 newVelocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction) * speed;

            // Sync if a velocity component changes.
            if (Projectile.velocity.X != newVelocity.X || Projectile.velocity.Y != newVelocity.Y)
            {
                Projectile.netUpdate = true;
            }
            Projectile.velocity = newVelocity;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!CanRecieveCoolEffectsFrom(target))
                return;
            target.AddBuff(ModContent.BuffType<KamiFlu>(), 600);
            if (!Main.dedServ)
            {
                for (int i = 0; i < 60; i++)
                {
                    Dust dust = Dust.NewDustDirect(Main.player[Projectile.owner].position, Main.player[Projectile.owner].width, Main.player[Projectile.owner].height, 267);
                    dust.position.X += Main.rand.NextFloat(-16f, 16f);
                    dust.color = Main.hslToRgb(Main.rand.NextFloat(0.26f, 0.37f), 1f, 0.75f);
                    dust.velocity = Main.rand.NextVector2Circular(24f, 24f);
                    dust.scale = Main.rand.NextFloat(1.4f, 1.8f);
                    dust.noGravity = true;
                }
            }
            if (Projectile.ai[0] == 0f)
            {
                SoundEngine.PlaySound(YanmeisKnife.HitSound, Projectile.position);
                Projectile.ai[0] = 1f;
            }
            Main.player[Projectile.owner].AddBuff(ModContent.BuffType<KamiBuff>(), 600);
        }
        public override Color? GetAlpha(Color lightColor) => new Color(0, 215, 0, 0);
    }
}
