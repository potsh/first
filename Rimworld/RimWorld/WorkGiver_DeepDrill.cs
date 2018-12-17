using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class WorkGiver_DeepDrill : WorkGiver_Scanner
	{
		public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(ThingDefOf.DeepDrill);

		public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}

		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
			return pawn.Map.listerBuildings.AllBuildingsColonistOfDef(ThingDefOf.DeepDrill).Cast<Thing>();
		}

		public override bool ShouldSkip(Pawn pawn, bool forced = false)
		{
			List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
			for (int i = 0; i < allBuildingsColonist.Count; i++)
			{
				Building building = allBuildingsColonist[i];
				if (building.def == ThingDefOf.DeepDrill)
				{
					CompPowerTrader comp = building.GetComp<CompPowerTrader>();
					if ((comp == null || comp.PowerOn) && building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) == null)
					{
						return false;
					}
				}
			}
			return true;
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (t.Faction != pawn.Faction)
			{
				return false;
			}
			Building building = t as Building;
			if (building == null)
			{
				return false;
			}
			if (building.IsForbidden(pawn))
			{
				return false;
			}
			LocalTargetInfo target = building;
			bool ignoreOtherReservations = forced;
			if (!pawn.CanReserve(target, 1, -1, null, ignoreOtherReservations))
			{
				return false;
			}
			CompDeepDrill compDeepDrill = building.TryGetComp<CompDeepDrill>();
			if (!compDeepDrill.CanDrillNow())
			{
				return false;
			}
			if (building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) != null)
			{
				return false;
			}
			if (building.IsBurning())
			{
				return false;
			}
			return true;
		}

		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			return new Job(JobDefOf.OperateDeepDrill, t, 1500, checkOverrideOnExpiry: true);
		}
	}
}
