using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public class PawnsArrivalModeWorker_CenterDrop : PawnsArrivalModeWorker
	{
		public const int PodOpenDelay = 520;

		public override void Arrive(List<Pawn> pawns, IncidentParms parms)
		{
			PawnsArrivalModeWorkerUtility.DropInDropPodsNearSpawnCenter(parms, pawns);
		}

		public override void TravelingTransportPodsArrived(List<ActiveDropPodInfo> dropPods, Map map)
		{
			if (!DropCellFinder.TryFindRaidDropCenterClose(out IntVec3 spot, map))
			{
				spot = DropCellFinder.FindRaidDropCenterDistant(map);
			}
			TransportPodsArrivalActionUtility.DropTravelingTransportPods(dropPods, spot, map);
		}

		public override bool TryResolveRaidSpawnCenter(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			parms.podOpenDelay = 520;
			parms.spawnRotation = Rot4.Random;
			if (!parms.spawnCenter.IsValid)
			{
				if (Rand.Chance(0.4f) && map.listerBuildings.ColonistsHaveBuildingWithPowerOn(ThingDefOf.OrbitalTradeBeacon))
				{
					parms.spawnCenter = DropCellFinder.TradeDropSpot(map);
				}
				else if (!DropCellFinder.TryFindRaidDropCenterClose(out parms.spawnCenter, map))
				{
					parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeDrop;
					return parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms);
				}
			}
			return true;
		}
	}
}
