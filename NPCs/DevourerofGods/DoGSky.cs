﻿using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    public class DoGSky : CustomSky
    {
        private bool isActive = false;
        private float intensity = 0f;
        private int DoGIndex = -1;

        public override void Update(GameTime gameTime)
        {
            if (DoGIndex == -1)
            {
                UpdateDoGIndex();
                if (DoGIndex == -1)
                    isActive = false;
            }

            if (isActive && intensity < 1f)
            {
                intensity += 0.01f;
            }
            else if (!isActive && intensity > 0f)
            {
                intensity -= 0.01f;
            }
        }

        private float GetIntensity()
        {
            if (UpdateDoGIndex())
            {
                float x = 0f;
                if (DoGIndex != -1)
                    x = Vector2.Distance(Main.player[Main.myPlayer].Center, Main.npc[DoGIndex].Center);

                float intensityScalar = 0.5f;
                return (1f - Utils.SmoothStep(3000f, 6000f, x)) * intensityScalar;
            }
            return 0f;
        }

        public override Color OnTileColor(Color inColor)
        {
            float intensity = GetIntensity();
            return new Color(Vector4.Lerp(new Vector4(0.5f, 0.8f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
        }

        private bool UpdateDoGIndex()
        {
            int DoGType = ModContent.NPCType<DevourerofGodsHead>();
            if (DoGIndex >= 0 && Main.npc[DoGIndex].active && Main.npc[DoGIndex].type == DoGType)
            {
                return true;
            }
            DoGIndex = -1;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == DoGType)
                {
                    DoGIndex = i;
                    break;
                }
            }
            return DoGIndex != -1;
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0 && DoGIndex != -1)
            {
                if (Main.npc[DoGIndex].active)
                {
                    float intensity = GetIntensity();
                    float lifeRatio = Main.npc[DoGIndex].life / (float)Main.npc[DoGIndex].lifeMax;
                    double blackScreenLife_GateValue = (lifeRatio < 0.6f && Main.npc[DoGIndex].localAI[2] <= 2f) ? 0.15 : 0.75;
                    if (Main.npc[DoGIndex].life < Main.npc[DoGIndex].lifeMax * blackScreenLife_GateValue || CalamityWorld.death || BossRushEvent.BossRushActive)
                    {
                        spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2),
                            Color.Black * (intensity + 0.5f));
                    }
                    else
                    {
                        float timeToReachNextColor = 90f;
                        float phaseTimer = Main.npc[DoGIndex].Calamity().newAI[2];
                        float colorChangeProgress = phaseTimer / timeToReachNextColor;
                        if (colorChangeProgress > 1f)
                            colorChangeProgress = 1f;

                        spriteBatch.Draw(TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth * 2, Main.screenHeight * 2),
                            (Main.npc[DoGIndex].ai[3] == 0f ? Color.Lerp(Color.Fuchsia, Color.Cyan, colorChangeProgress) : Color.Lerp(Color.Cyan, Color.Fuchsia, colorChangeProgress)) * intensity);
                    }
                }
            }
        }

        public override float GetCloudAlpha()
        {
            return 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive || intensity > 0f;
        }
    }
}
