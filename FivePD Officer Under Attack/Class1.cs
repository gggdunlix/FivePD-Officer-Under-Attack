using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using FivePD.API;
using FivePD.API.Utils;

namespace FivePD_Officer_Under_Attack
{
    [CalloutProperties("Officer Under Attack (beaten)", "GGG Dunlix", "0.0.1")]
    public class CopUnderAttack : Callout
    {
        Ped cop, suspect;

        public CopUnderAttack()
        {
            InitInfo(World.GetNextPositionOnStreet(Game.PlayerPed.GetOffsetPosition(Vector3Extension.Around(Game.PlayerPed.Position, 300f))));
            ShortName = "Officer Under Attack";
            CalloutDescription = "An officer is being attacked. Respond in Code 3 High.";
            ResponseCode = 3;
            StartDistance = 200f;
        }
        public async override Task OnAccept()
        {
            InitBlip();
            UpdateData();
            var coplist = new[]
            {
                PedHash.Cop01SFY,
                PedHash.Cop01SMY,
                PedHash.CopCutscene,
                PedHash.Hwaycop01SMY,
                PedHash.Snowcop01SMM,
                PedHash.UndercoverCopCutscene
            };
            suspect = await SpawnPed(RandomUtils.GetRandomPed(), Location);
            cop = await SpawnPed(coplist[RandomUtils.Random.Next(coplist.Length)], Location);
            suspect.AlwaysKeepTask = true;
            suspect.BlockPermanentEvents = true;
        }
        public override void OnStart(Ped player)
        {
            base.OnStart(player);
            suspect.Task.FightAgainst(cop);
            cop.Ragdoll(-1, RagdollType.Normal);
            suspect.AttachBlip();
            cop.AttachBlip();
            cop.ArmorFloat = 3000;
            if (cop.IsDead.Equals(true))
            {
                suspect.Task.FightAgainst(Game.PlayerPed);
                ShowNetworkedNotification("The officer is deceased. Coroner dispatched to your location", "CHAR_CALL911", "CHAR_CALL911", "Dispatch", "", 15f);
                FivePD.API.Utilities.RequestService(Utilities.Services.Coroner);
            }
            if (suspect.IsDead.Equals(true))
            {
                if (cop.IsDead.Equals(false))
                {
                    ShowNetworkedNotification("The officer is in critical condition. EMS unit dispatched to your location", "CHAR_CALL911", "CHAR_CALL911", "Dispatch", "", 15f);
                    FivePD.API.Utilities.RequestService(Utilities.Services.Ambulance);

                }
            }
            if (suspect.IsCuffed.Equals(true))
            {
                if (cop.IsDead.Equals(false))
                {
                    ShowNetworkedNotification("The officer is in critical condition. EMS unit dispatched to your location", "CHAR_CALL911", "CHAR_CALL911", "Dispatch", "", 15f);
                    FivePD.API.Utilities.RequestService(Utilities.Services.Ambulance);

                }
            }
        }
    }
}
