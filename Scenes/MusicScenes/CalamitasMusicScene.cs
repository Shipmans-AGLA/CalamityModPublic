using CalamityMod.NPCs.Calamitas;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class CalamitasMusicScene : BaseMusicSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override int NPCType => ModContent.NPCType<CalamitasClone>();
        public override int? MusicModMusic => CalamityMod.Instance.GetMusicFromMusicMod("Calamitas");
        public override int VanillaMusic => MusicID.Boss2;
        public override int OtherworldMusic => MusicID.OtherworldlyBoss2;
    }
}
