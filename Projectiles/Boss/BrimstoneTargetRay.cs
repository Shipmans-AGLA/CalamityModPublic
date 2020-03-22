﻿using CalamityMod.Projectiles.Typeless;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneTargetRay : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Target Ray");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 300;
        }

		public override void AI()
		{
			projectile.tileCollide = Collision.CanHitLine(Main.npc[(int)projectile.ai[1]].Center, 1, 1, Main.player[Main.npc[(int)projectile.ai[1]].target].Center, 1, 1);

			projectile.ai[0] += 1f;
			if (projectile.ai[0] == 48f)
			{
				projectile.ai[0] = 0f;
			}
			else if (projectile.ai[0] % 2f == 0f)
			{
				Vector2 value7 = new Vector2(5f, 10f);
				Vector2 value8 = Vector2.UnitX * -12f;

				value8 = -Vector2.UnitY.RotatedBy((double)(projectile.ai[0] * 0.1308997f + (float)Main.rand.Next(1, 3) * 3.14159274f), default(Vector2)) * value7;
				int num42 = Dust.NewDust(projectile.Center, 0, 0, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
				Main.dust[num42].scale = (float)Main.rand.Next(70, 110) * 0.008f;
				Main.dust[num42].noGravity = true;
				Main.dust[num42].position = projectile.Center + value8;
				Main.dust[num42].velocity = projectile.velocity;
			}
		}
    }
}
