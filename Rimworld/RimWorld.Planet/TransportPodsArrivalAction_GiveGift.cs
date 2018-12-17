using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld.Planet
{
	public class TransportPodsArrivalAction_GiveGift : TransportPodsArrivalAction
	{
		private SettlementBase settlement;

		public TransportPodsArrivalAction_GiveGift()
		{
		}

		public TransportPodsArrivalAction_GiveGift(SettlementBase settlement)
		{
			this.settlement = settlement;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref settlement, "settlement");
		}

		public override FloatMenuAcceptanceReport StillValid(IEnumerable<IThingHolder> pods, int destinationTile)
		{
			FloatMenuAcceptanceReport floatMenuAcceptanceReport = base.StillValid(pods, destinationTile);
			if (!(bool)floatMenuAcceptanceReport)
			{
				return floatMenuAcceptanceReport;
			}
			if (settlement != null && settlement.Tile != destinationTile)
			{
				return false;
			}
			return CanGiveGiftTo(pods, settlement);
		}

		public override void Arrived(List<ActiveDropPodInfo> pods, int tile)
		{
			for (int i = 0; i < pods.Count; i++)
			{
				for (int j = 0; j < pods[i].innerContainer.Count; j++)
				{
					Pawn pawn = pods[i].innerContainer[j] as Pawn;
					if (pawn != null && pawn.RaceProps.Humanlike)
					{
						GenGuest.AddPrisonerSoldThoughts(pawn);
					}
				}
			}
			FactionGiftUtility.GiveGift(pods, settlement);
		}

		public static FloatMenuAcceptanceReport CanGiveGiftTo(IEnumerable<IThingHolder> pods, SettlementBase settlement)
		{
			return settlement != null && settlement.Spawned && settlement.Faction != null && settlement.Faction != Faction.OfPlayer && !settlement.Faction.def.permanentEnemy && !settlement.HasMap;
		}

		public static IEnumerable<FloatMenuOption> GetFloatMenuOptions(CompLaunchable representative, IEnumerable<IThingHolder> pods, SettlementBase settlement)
		{
			if (settlement.Faction == Faction.OfPlayer)
			{
				return Enumerable.Empty<FloatMenuOption>();
			}
			return TransportPodsArrivalActionUtility.GetFloatMenuOptions(() => CanGiveGiftTo(pods, settlement), () => new TransportPodsArrivalAction_GiveGift(settlement), "GiveGiftViaTransportPods".Translate(settlement.Faction.Name, FactionGiftUtility.GetGoodwillChange(pods, settlement).ToStringWithSign()), representative, settlement.Tile);
		}
	}
}
