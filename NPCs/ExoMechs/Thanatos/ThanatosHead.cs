using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.ExoMechs.Thanatos
{
	//[AutoloadBossHead]
	public class ThanatosHead : ModNPC
    {
		public enum Phase
		{
			Charge = 0,
			UndergroundLaserBarrage = 1,
			Deathray = 2
		}

		public float AIState
		{
			get => npc.Calamity().newAI[0];
			set => npc.Calamity().newAI[0] = value;
		}

		public enum SecondaryPhase
		{
			Nothing = 0,
			Passive = 1,
			PassiveAndImmune = 2
		}

		public float SecondaryAIState
		{
			get => npc.Calamity().newAI[1];
			set => npc.Calamity().newAI[1] = value;
		}

		// Whether the head is venting heat or not, it is vulnerable to damage during venting
		private bool vulnerable = false;

		// Default life ratio for the other mechs
		private const float defaultLifeRatio = 5f;

		// Base distance from the target for most attacks
		private const float baseDistance = 1200f;

		// Base distance from target location in order to continue turning
		private const float baseTurnDistance = 160f;

		// Max distance from the target before they are unable to hear sound telegraphs
		private const float soundDistance = 2800f;

		// Length variables
		private const int minLength = 100;
        private const int maxLength = 101;

		// Variable used to stop the segment spawning loop
        private bool TailSpawned = false;

		// Used in the lerp to smoothly scale velocity up and down
		private float chargeVelocityScalar = 0f;

		// Total duration of the deathray telegraph
		private const float deathrayTelegraphDuration = 120f;

		// Total duration of the deathray
		private const float deathrayDuration = 180f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thanatos");
			Main.npcFrameCount[npc.type] = 5;
		}

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 5f;
			npc.GetNPCDamage();
			npc.width = 104;
            npc.height = 174;
            npc.defense = 80;
			npc.DR_NERD(0.99f);
			npc.Calamity().unbreakableDR = true;
			npc.LifeMaxNERB(1000000, 1150000);
			double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
			npc.lifeMax += (int)(npc.lifeMax * HPBoost);
			npc.aiStyle = -1;
            aiType = -1;
			npc.Opacity = 0f;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(10, 0, 0, 0);
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
			npc.boss = true;
			npc.chaseable = false;
			music = /*CalamityMod.Instance.GetMusicFromMusicMod("AdultEidolonWyrm") ??*/ MusicID.Boss3;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(npc.chaseable);
            writer.Write(npc.dontTakeDamage);
			writer.Write(chargeVelocityScalar);
			writer.Write(vulnerable);
			writer.Write(npc.localAI[0]);
			writer.Write(npc.localAI[1]);
			writer.Write(npc.localAI[2]);
			writer.Write(npc.localAI[3]);
			for (int i = 0; i < 4; i++)
				writer.Write(npc.Calamity().newAI[i]);
		}

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			npc.chaseable = reader.ReadBoolean();
			npc.dontTakeDamage = reader.ReadBoolean();
			chargeVelocityScalar = reader.ReadSingle();
			vulnerable = reader.ReadBoolean();
			npc.localAI[0] = reader.ReadSingle();
			npc.localAI[1] = reader.ReadSingle();
			npc.localAI[2] = reader.ReadSingle();
			npc.localAI[3] = reader.ReadSingle();
			for (int i = 0; i < 4; i++)
				npc.Calamity().newAI[i] = reader.ReadSingle();
		}

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			CalamityGlobalNPC.draedonExoMechWorm = npc.whoAmI;

			// Difficulty modes
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
			bool death = CalamityWorld.death || malice;
			bool revenge = CalamityWorld.revenge || malice;
			bool expertMode = Main.expertMode || malice;

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			// Increase aggression if player is taking a long time to kill the boss
			if (lifeRatio > calamityGlobalNPC.killTimeRatio_IncreasedAggression)
				lifeRatio = calamityGlobalNPC.killTimeRatio_IncreasedAggression;

			// Check if the other exo mechs are alive
			int otherExoMechsAlive = 0;
			bool exoPrimeAlive = false;
			bool exoSpazAlive = false;
			bool exoRetAlive = false;
			if (CalamityGlobalNPC.draedonExoMechPrime != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechPrime].active)
				{
					otherExoMechsAlive++;
					exoPrimeAlive = true;
				}
			}
			if (CalamityGlobalNPC.draedonExoMechTwinGreen != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].active)
				{
					otherExoMechsAlive++;
					exoSpazAlive = true;
				}
			}
			if (CalamityGlobalNPC.draedonExoMechTwinRed != -1)
			{
				if (Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].active)
				{
					otherExoMechsAlive++;
					exoRetAlive = true;
				}
			}

			// These are 5 by default to avoid triggering passive phases after the other mechs are dead
			float exoPrimeLifeRatio = defaultLifeRatio;
			float exoSpazLifeRatio = defaultLifeRatio;
			float exoRetLifeRatio = defaultLifeRatio;
			if (exoPrimeAlive)
				exoPrimeLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechPrime].lifeMax;
			if (exoSpazAlive)
				exoSpazLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].lifeMax;
			if (exoRetAlive)
				exoRetLifeRatio = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].life / (float)Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].lifeMax;
			float totalOtherExoMechLifeRatio = exoPrimeLifeRatio + exoSpazLifeRatio + exoRetLifeRatio;

			// Check if any of the other mechs are passive
			bool exoPrimePassive = false;
			bool exoSpazPassive = false;
			bool exoRetPassive = false;
			/*if (exoPrimeAlive)
				exoPrimePassive = Main.npc[CalamityGlobalNPC.draedonExoMechPrime].newAI[1] == (float)AresBody.SecondaryPhase.Passive;
			if (exoSpazAlive)
				exoSpazPassive = Main.npc[CalamityGlobalNPC.draedonExoMechTwinGreen].newAI[1] == (float)Apollo.SecondaryPhase.Passive;
			if (exoRetAlive)
				exoRetPassive = Main.npc[CalamityGlobalNPC.draedonExoMechTwinRed].newAI[1] == (float)Artemis.SecondaryPhase.Passive;*/
			bool anyOtherExoMechPassive = exoPrimePassive || exoSpazPassive || exoRetPassive;

			// Phases
			bool lessThan70PercentLife = lifeRatio < 0.7f;
			bool phase2 = lessThan70PercentLife && otherExoMechsAlive <= 1;
			bool berserk = lifeRatio < 0.4f || totalOtherExoMechLifeRatio > 3f;

			// Set vulnerable to false by default
			vulnerable = false;

			// If Thanatos doesn't go berserk
			bool otherMechIsBerserk = exoPrimeLifeRatio < 0.4f || exoSpazLifeRatio < 0.4f || exoRetLifeRatio < 0.4f;

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			// Target variable
			Player player = Main.player[npc.target];

			if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

			// Spawn segments
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (!TailSpawned && npc.ai[0] == 0f)
				{
					int Previous = npc.whoAmI;
					for (int num36 = 0; num36 < maxLength; num36++)
					{
						int lol;
						if (num36 >= 0 && num36 < minLength)
						{
							if (num36 % 2 == 0)
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<ThanatosBody1>(), npc.whoAmI);
							else
								lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<ThanatosBody2>(), npc.whoAmI);
						}
						else
							lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<ThanatosTail>(), npc.whoAmI);

						Main.npc[lol].realLife = npc.whoAmI;
						Main.npc[lol].ai[2] = npc.whoAmI;
						Main.npc[lol].ai[1] = Previous;
						Main.npc[Previous].ai[0] = lol;
						NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
						Previous = lol;
					}
					TailSpawned = true;
				}
			}

			// Despawn if target is dead
            if (player.dead)
            {
                npc.TargetClosest(false);

				npc.velocity.Y += 2f;
				if (npc.position.Y > Main.worldSurface * 16.0)
					npc.velocity.Y += 2f;

				if (npc.position.Y > Main.rockLayer * 16.0)
				{
					for (int a = 0; a < Main.maxNPCs; a++)
					{
						if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<ThanatosBody1>() || Main.npc[a].type == ModContent.NPCType<ThanatosBody2>() || Main.npc[a].type == ModContent.NPCType<ThanatosTail>())
							Main.npc[a].active = false;
					}
				}
			}

			// General AI pattern
			// Fly towards the target for 7 seconds, gradually speeding up for the first 5 seconds and slowing down for the last 2 seconds, fire lasers from segments that are venting
			// Fly underneath the target and fire barrages of lasers
			// Fire deathray from mouth with a telegraph similar to the railgun from enter the gungeon, turn speed is very low during this to avoid cheap hits
			// Go passive and fly underneath the target while firing lasers
			// Go passive, immune and invisible; fly far underneath the target and do nothing until next phase

			// Attack patterns
			// If spawned first
			// Phase 1 - 0, 1, 0, 2
			// Phase 2 - 4
			// Phase 3 - 3

			// If berserk, this is the last phase of thanatos
			// Phase 4 - 0, 1, 0, 2

			// If not berserk
			// Phase 4 - 4
			// Phase 5 - 0, 1, 0, 2

			// If berserk, this is the last phase of thanatos
			// Phase 6 - 0, 1, 0, 2

			// If not berserk
			// Phase 6 - 4
			// Phase 7 - 0, 1, 0, 2

			// Phase gate values
			float velocityAdjustTime = 20f;
			float speedUpTime = 300f;
			float slowDownTime = 120f;
			float chargePhaseGateValue = speedUpTime + slowDownTime;

			float laserBarrageDuration = 420f;

			// Adjust opacity
			bool invisiblePhase = SecondaryAIState == (float)SecondaryPhase.PassiveAndImmune;
			npc.dontTakeDamage = invisiblePhase;
			if (!invisiblePhase)
			{
				npc.Opacity += 0.15f;
				if (npc.Opacity > 1f)
					npc.Opacity = 1f;
			}
			else
			{
				npc.Opacity -= 0.05f;
				if (npc.Opacity < 0f)
					npc.Opacity = 0f;
			}

			// Rotation and direction
			npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
			int direction = npc.direction;
			npc.direction = npc.spriteDirection = (npc.velocity.X > 0f) ? 1 : (-1);
			if (direction != npc.direction)
				npc.netUpdate = true;

			// Default vector to fly to
			Vector2 destination = player.Center;

			// Charge variables
			float turnDistance = baseTurnDistance;
			float chargeLocationDistance = turnDistance * 0.2f;

			// Laser Barrage variables
			Vector2 laserBarrageLocation = new Vector2(0f, baseDistance);
			float laserBarrageLocationDistance = turnDistance * 5f;

			// Velocity and turn speed values
			float baseVelocity = 9f;
			float turnSpeed = MathHelper.ToRadians(1f);
			float chargeVelocityMult = MathHelper.Lerp(1f, 2f, chargeVelocityScalar);
			float chargeTurnSpeedMult = MathHelper.Lerp(1f, 2f, chargeVelocityScalar);
			float laserBarragePhaseVelocityMult = MathHelper.Lerp(1f, 2f, chargeVelocityScalar);
			float laserBarragePhaseTurnSpeedMult = MathHelper.Lerp(1f, 8f, chargeVelocityScalar);
			float deathrayVelocityMult = MathHelper.Lerp(1f, 4f, chargeVelocityScalar);
			float deathrayTurnSpeedMult = MathHelper.Lerp(1f, 4f, chargeVelocityScalar);

			// Base scale on total time spent in phase
			float chargeVelocityScalarIncrement = 1f / speedUpTime;
			float chargeVelocityScalarDecrement = 1f / slowDownTime;

			// Scalar to use during laser barrage, passive and immune phases
			float laserBarrageVelocityScalarIncrement = 0.01f;
			float laserBarrageVelocityScalarDecrement = 1f / velocityAdjustTime;

			// Distance from target
			float distanceFromTarget = Vector2.Distance(npc.Center, player.Center);

			// Passive and Immune phases
			switch ((int)SecondaryAIState)
			{
				case (int)SecondaryPhase.Nothing:

					// Spawn the other mechs if Thanatos is first
					if (otherExoMechsAlive == 0)
					{
						if (phase2 && !berserk)
						{
							SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{
								// Spawn code here
							}
						}
					}
					else
					{
						// If not spawned first, go to passive state if any other mech is passive or if Thanatos is under 70% life
						// Do not run this if berserk
						// Do not run this if any exo mech is dead
						if ((anyOtherExoMechPassive || lessThan70PercentLife) && !berserk && totalOtherExoMechLifeRatio < 5f)
						{
							// Tells Thanatos to return to the battle in passive state
							SecondaryAIState = (float)SecondaryPhase.Passive;
						}

						// Go passive and immune if one of the other mechs is berserk
						// This is only called if two exo mechs are alive
						if (otherMechIsBerserk)
							SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;
					}

					break;

				// Fly underneath target and fire lasers
				case (int)SecondaryPhase.Passive:

					// Fire lasers while passive
					AIState = (float)Phase.UndergroundLaserBarrage;

					// Enter passive and invincible phase if one of the other exo mechs is berserk
					if (otherMechIsBerserk)
						SecondaryAIState = (float)SecondaryPhase.PassiveAndImmune;

					// If Thanatos is the first mech to go berserk
					if (berserk)
					{
						// Reset
						AIState = (float)Phase.Charge;

						// Never be passive if berserk
						SecondaryAIState = (float)SecondaryPhase.Nothing;
					}

					break;

				// Fly underneath target and become immune
				case (int)SecondaryPhase.PassiveAndImmune:

					// Fire lasers while passive
					AIState = (float)Phase.UndergroundLaserBarrage;

					// Enter the fight again if any of the other exo mechs is below 70% and the other mechs aren't berserk
					if ((exoPrimeLifeRatio < 0.7f || exoSpazLifeRatio < 0.7f || exoRetLifeRatio < 0.7f) && !otherMechIsBerserk)
					{
						// Tells Thanatos to return to the battle in passive state
						// Return to normal phases if one or more mechs have been downed
						SecondaryAIState = totalOtherExoMechLifeRatio > 5f ? (float)SecondaryPhase.Nothing : (float)SecondaryPhase.Passive;
					}

					if (berserk)
					{
						// Reset
						AIState = (float)Phase.Charge;

						// Never be passive if berserk
						SecondaryAIState = (float)SecondaryPhase.Nothing;
					}

					break;
			}

			// Attacking phases
			switch ((int)AIState)
			{
				// Fly towards target and gain velocity over time
				case (int)Phase.Charge:

					// Use a lerp to smoothly scale up velocity and turn speed
					if (calamityGlobalNPC.newAI[3] == 0f)
					{
						chargeVelocityScalar += chargeVelocityScalarIncrement;
						if (chargeVelocityScalar >= 1f)
						{
							chargeVelocityScalar = 1f;
							calamityGlobalNPC.newAI[3] = 1f;
						}
					}
					else
					{
						chargeVelocityScalar -= chargeVelocityScalarDecrement;
						if (chargeVelocityScalar < 0f)
							chargeVelocityScalar = 0f;
					}

					baseVelocity *= chargeVelocityMult;
					turnSpeed *= chargeTurnSpeedMult;
					turnDistance = chargeLocationDistance;

					// Gradually turn slower if within 20 tiles of the target
					if (distanceFromTarget < 320f)
						turnSpeed *= distanceFromTarget / 320f;

					calamityGlobalNPC.newAI[2] += 1f;
					if (calamityGlobalNPC.newAI[2] >= chargePhaseGateValue)
					{
						AIState = npc.localAI[0] == 1f ? (float)Phase.Deathray : (float)Phase.UndergroundLaserBarrage;
						calamityGlobalNPC.newAI[2] = 0f;
						calamityGlobalNPC.newAI[3] = 0f;
						chargeVelocityScalar = 0f;
						npc.TargetClosest();
					}

					break;

				// Fly below and summon barrages of lasers
				case (int)Phase.UndergroundLaserBarrage:

					// Fly down
					destination += laserBarrageLocation;
					turnDistance = laserBarrageLocationDistance;

					// Use a lerp to smoothly scale up velocity and turn speed
					if (calamityGlobalNPC.newAI[3] == 0f)
					{
						chargeVelocityScalar += laserBarrageVelocityScalarIncrement;
						if (chargeVelocityScalar > 1f)
							chargeVelocityScalar = 1f;
					}

					baseVelocity *= laserBarragePhaseVelocityMult;
					turnSpeed *= laserBarragePhaseTurnSpeedMult;

					if ((destination - npc.Center).Length() < laserBarrageLocationDistance || calamityGlobalNPC.newAI[2] > 0f)
					{
						calamityGlobalNPC.newAI[2] += 1f;

						if (SecondaryAIState != (float)SecondaryPhase.Passive && SecondaryAIState != (float)SecondaryPhase.PassiveAndImmune)
						{
							if (calamityGlobalNPC.newAI[2] >= laserBarrageDuration)
							{
								// Use a lerp to smoothly scale down velocity and turn speed
								chargeVelocityScalar -= laserBarrageVelocityScalarDecrement;
								if (chargeVelocityScalar < 0f)
									chargeVelocityScalar = 0f;

								calamityGlobalNPC.newAI[3] += 1f;
								if (calamityGlobalNPC.newAI[3] >= velocityAdjustTime)
								{
									npc.localAI[0] = 1f;
									AIState = (float)Phase.Charge;
									calamityGlobalNPC.newAI[2] = 0f;
									calamityGlobalNPC.newAI[3] = 0f;
									chargeVelocityScalar = 0f;
									npc.TargetClosest();
								}
							}
						}
					}

					break;

				// Move close to target, reduce velocity and turn speed when close enough, create telegraph beams, reduce turn speed, fire deathray
				case (int)Phase.Deathray:

					// Head is vulnerable while charging and firing deathray
					vulnerable = true;

					// If close enough to the target, prepare to fire deathray
					bool readyToFireDeathray = distanceFromTarget < 800f;
					if (readyToFireDeathray)
						npc.localAI[2] = 1f;

					// Use a lerp to smoothly scale up velocity and turn speed
					if (calamityGlobalNPC.newAI[3] == 0f)
					{
						chargeVelocityScalar += chargeVelocityScalarIncrement;
						if (chargeVelocityScalar >= 1f)
						{
							chargeVelocityScalar = 1f;

							// If ready to fire deathray, start reducing the velocity scalar
							if (npc.localAI[2] == 1f)
								calamityGlobalNPC.newAI[3] = 1f;
						}
					}
					else
					{
						chargeVelocityScalar -= chargeVelocityScalarDecrement;
						if (chargeVelocityScalar < 0f)
							chargeVelocityScalar = 0f;
					}

					baseVelocity *= deathrayVelocityMult;
					turnSpeed *= deathrayTurnSpeedMult;
					turnDistance = chargeLocationDistance;

					// Gradually turn and move slower if within 50 tiles of the target
					if (npc.localAI[2] == 1f)
					{
						baseVelocity *= distanceFromTarget / 800f;
						turnSpeed *= distanceFromTarget / 800f;

						if (calamityGlobalNPC.newAI[2] < deathrayTelegraphDuration)
						{
							// Fire deathray telegraph beams
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{

							}
						}
						else
						{
							// Fire deathray
							if (Main.netMode != NetmodeID.MultiplayerClient)
							{

							}
						}

						if (calamityGlobalNPC.newAI[2] >= deathrayTelegraphDuration + deathrayDuration)
						{
							npc.localAI[0] = 0f;
							npc.localAI[2] = 0f;
							AIState = (float)Phase.Charge;
							calamityGlobalNPC.newAI[2] = 0f;
							calamityGlobalNPC.newAI[3] = 0f;
							chargeVelocityScalar = 0f;
							npc.TargetClosest();
						}
						calamityGlobalNPC.newAI[2] += 1f;
					}

					break;
			}

			// Homing only works if vulnerable is true
			npc.chaseable = vulnerable;

			// Adjust DR based on vulnerable
			npc.Calamity().DR = vulnerable ? 0f : 0.99f;
			npc.Calamity().unbreakableDR = !vulnerable;

			// Vent noise and steam
			if (vulnerable)
			{
				// Noise
				if (npc.localAI[1] == 0f)
					Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ThanatosVent"), npc.Center);

				// Steam
				float maxSteamTime = 180f;
				int maxGores = 4;
				if (npc.localAI[1] < maxSteamTime)
				{
					npc.localAI[1] += 1f;
					int goreAmt = maxGores - (int)Math.Round(npc.localAI[1] / 60f);
					CalamityUtils.ExplosionGores(npc.Center, goreAmt, true, npc.velocity);
				}
			}
			else
				npc.localAI[1] = 0f;

			// Increase velocity if velocity is ever zero
			if (npc.velocity == Vector2.Zero)
				npc.velocity = Vector2.Normalize(player.Center - npc.Center).SafeNormalize(Vector2.Zero) * baseVelocity;

			// Acceleration
			if (!((destination - npc.Center).Length() < turnDistance))
			{
				float targetAngle = npc.AngleTo(destination);
				float f = npc.velocity.ToRotation().AngleTowards(targetAngle, turnSpeed);
				npc.velocity = f.ToRotationVector2() * baseVelocity;
			}

			// Velocity upper limit
			if (npc.velocity.Length() > baseVelocity)
				npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * baseVelocity;

			// Reduce Y velocity if it's less than 1
			if (Math.Abs(npc.velocity.Y) < 1f)
				npc.velocity.Y -= 0.1f;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;

			Rectangle targetHitbox = target.Hitbox;

			float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
			float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
			float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
			float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

			float minDist = dist1;
			if (dist2 < minDist)
				minDist = dist2;
			if (dist3 < minDist)
				minDist = dist3;
			if (dist4 < minDist)
				minDist = dist4;

			return minDist <= 50f && npc.Opacity == 1f;
		}

		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			return !CalamityUtils.AntiButcher(npc, ref damage, 0.5f);
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 1.5f;
			return null;
		}

		public override void FindFrame(int frameHeight) // 5 total frames
		{
			// Swap between venting and non-venting frames
			if (AIState == (float)Phase.Charge || AIState == (float)Phase.UndergroundLaserBarrage)
			{
				npc.frameCounter += 1D;
				if (npc.frameCounter >= 12D)
				{
					npc.frame.Y -= frameHeight;
					npc.frameCounter = 0D;
				}
				if (npc.frame.Y < 0)
					npc.frame.Y = 0;
			}
			else
			{
				npc.frameCounter += 1D;
				if (npc.frameCounter >= 12D)
				{
					npc.frame.Y += frameHeight;
					npc.frameCounter = 0D;
				}
				if (npc.frame.Y >= frameHeight * Main.npcFrameCount[npc.type])
					npc.frame.Y = frameHeight * Main.npcFrameCount[npc.type];
			}
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (npc.spriteDirection == 1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			Texture2D texture = Main.npcTexture[npc.type];
			Vector2 vector = new Vector2(Main.npcTexture[npc.type].Width / 2, Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2);

			Vector2 center = npc.Center - Main.screenPosition;
			center -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[npc.type]) * npc.scale / 2f;
			center += vector * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
			spriteBatch.Draw(texture, center, npc.frame, npc.GetAlpha(drawColor), npc.rotation, vector, npc.scale, spriteEffects, 0f);

			texture = ModContent.GetTexture("CalamityMod/NPCs/ExoMechs/Thanatos/ThanatosHeadGlow");
			spriteBatch.Draw(texture, center, npc.frame, Color.White * npc.Opacity, npc.rotation, vector, npc.scale, spriteEffects, 0f);
		}

        public override void NPCLoot()
        {
            /*DropHelper.DropItem(npc, ModContent.ItemType<Voidstone>(), 80, 100);
            DropHelper.DropItem(npc, ModContent.ItemType<EidolicWail>());
            DropHelper.DropItem(npc, ModContent.ItemType<SoulEdge>());
            DropHelper.DropItem(npc, ModContent.ItemType<HalibutCannon>());

            DropHelper.DropItemCondition(npc, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas, 1, 50, 108);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas && Main.expertMode, 2, 15, 27);
            DropHelper.DropItemCondition(npc, ItemID.Ectoplasm, NPC.downedPlantBoss, 1, 21, 32);*/
        }

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
				Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1f);

			if (npc.life <= 0)
			{
				for (int num193 = 0; num193 < 2; num193++)
				{
					Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
				}
				for (int num194 = 0; num194 < 20; num194++)
				{
					int num195 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 0, new Color(0, 255, 255), 2.5f);
					Main.dust[num195].noGravity = true;
					Main.dust[num195].velocity *= 3f;
					num195 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 107, 0f, 0f, 100, new Color(0, 255, 255), 1.5f);
					Main.dust[num195].velocity *= 2f;
					Main.dust[num195].noGravity = true;
				}

				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosHead"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosHead2"), 1f);
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/Thanatos/ThanatosHead3"), 1f);
			}
		}

		public override bool CheckActive() => false;

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
        {
			if (npc.Opacity == 1f)
			{
				int duration = vulnerable ? 180 : 90;
				player.AddBuff(BuffID.Ichor, duration);
				player.AddBuff(BuffID.CursedInferno, duration);
				player.AddBuff(ModContent.BuffType<ExoFreeze>(), duration / 4);
				player.AddBuff(ModContent.BuffType<BrimstoneFlames>(), duration);
				player.AddBuff(ModContent.BuffType<Plague>(), duration);
				player.AddBuff(ModContent.BuffType<HolyFlames>(), duration);
				player.AddBuff(BuffID.Frostburn, duration);
				player.AddBuff(BuffID.OnFire, duration);
			}
        }
    }
}
