using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
	public static class GenHostility
	{
		public static bool HostileTo(this Thing a, Thing b)
		{
			if (a.Destroyed || b.Destroyed || a == b)
			{
				return false;
			}
			Pawn pawn = a as Pawn;
			Pawn pawn2 = b as Pawn;
			if ((pawn != null && pawn.MentalState != null && pawn.MentalState.ForceHostileTo(b)) || (pawn2 != null && pawn2.MentalState != null && pawn2.MentalState.ForceHostileTo(a)))
			{
				return true;
			}
			if (pawn != null && pawn2 != null && (IsPredatorHostileTo(pawn, pawn2) || IsPredatorHostileTo(pawn2, pawn)))
			{
				return true;
			}
			if ((a.Faction != null && pawn2 != null && pawn2.HostFaction == a.Faction && (pawn == null || pawn.HostFaction == null) && PrisonBreakUtility.IsPrisonBreaking(pawn2)) || (b.Faction != null && pawn != null && pawn.HostFaction == b.Faction && (pawn2 == null || pawn2.HostFaction == null) && PrisonBreakUtility.IsPrisonBreaking(pawn)))
			{
				return true;
			}
			if ((a.Faction != null && pawn2 != null && pawn2.HostFaction == a.Faction) || (b.Faction != null && pawn != null && pawn.HostFaction == b.Faction))
			{
				return false;
			}
			if (pawn != null && pawn.IsPrisoner && pawn2 != null && pawn2.IsPrisoner)
			{
				return false;
			}
			if (pawn != null && pawn2 != null && ((pawn.IsPrisoner && pawn.HostFaction == pawn2.HostFaction && !PrisonBreakUtility.IsPrisonBreaking(pawn)) || (pawn2.IsPrisoner && pawn2.HostFaction == pawn.HostFaction && !PrisonBreakUtility.IsPrisonBreaking(pawn2))))
			{
				return false;
			}
			if (pawn != null && pawn2 != null && ((pawn.HostFaction != null && pawn2.Faction != null && !pawn.HostFaction.HostileTo(pawn2.Faction) && !PrisonBreakUtility.IsPrisonBreaking(pawn)) || (pawn2.HostFaction != null && pawn.Faction != null && !pawn2.HostFaction.HostileTo(pawn.Faction) && !PrisonBreakUtility.IsPrisonBreaking(pawn2))))
			{
				return false;
			}
			if ((a.Faction != null && a.Faction.IsPlayer && pawn2 != null && pawn2.mindState.WillJoinColonyIfRescued) || (b.Faction != null && b.Faction.IsPlayer && pawn != null && pawn.mindState.WillJoinColonyIfRescued))
			{
				return false;
			}
			if (a.Faction == null || b.Faction == null)
			{
				return false;
			}
			return a.Faction.HostileTo(b.Faction);
		}

		public static bool HostileTo(this Thing t, Faction fac)
		{
			if (t.Destroyed)
			{
				return false;
			}
			Pawn pawn = t as Pawn;
			if (pawn != null)
			{
				MentalState mentalState = pawn.MentalState;
				if (mentalState != null && mentalState.ForceHostileTo(fac))
				{
					return true;
				}
				if (IsPredatorHostileTo(pawn, fac))
				{
					return true;
				}
				if (pawn.HostFaction == fac && PrisonBreakUtility.IsPrisonBreaking(pawn))
				{
					return true;
				}
				if (pawn.HostFaction == fac)
				{
					return false;
				}
				if (pawn.HostFaction != null && !pawn.HostFaction.HostileTo(fac) && !PrisonBreakUtility.IsPrisonBreaking(pawn))
				{
					return false;
				}
				if (fac.IsPlayer && pawn.mindState.WillJoinColonyIfRescued)
				{
					return false;
				}
			}
			if (t.Faction == null)
			{
				return false;
			}
			return t.Faction.HostileTo(fac);
		}

		private static bool IsPredatorHostileTo(Pawn predator, Pawn toPawn)
		{
			if (toPawn.Faction == null)
			{
				return false;
			}
			if (toPawn.Faction.HasPredatorRecentlyAttackedAnyone(predator))
			{
				return true;
			}
			Pawn preyOfMyFaction = GetPreyOfMyFaction(predator, toPawn.Faction);
			if (preyOfMyFaction != null && predator.Position.InHorDistOf(preyOfMyFaction.Position, 12f))
			{
				return true;
			}
			return false;
		}

		private static bool IsPredatorHostileTo(Pawn predator, Faction toFaction)
		{
			if (toFaction.HasPredatorRecentlyAttackedAnyone(predator))
			{
				return true;
			}
			if (GetPreyOfMyFaction(predator, toFaction) != null)
			{
				return true;
			}
			return false;
		}

		private static Pawn GetPreyOfMyFaction(Pawn predator, Faction myFaction)
		{
			Job curJob = predator.CurJob;
			if (curJob != null && curJob.def == JobDefOf.PredatorHunt && !predator.jobs.curDriver.ended)
			{
				Pawn pawn = curJob.GetTarget(TargetIndex.A).Thing as Pawn;
				if (pawn != null && !pawn.Dead && pawn.Faction == myFaction)
				{
					return pawn;
				}
			}
			return null;
		}

		public static bool AnyHostileActiveThreatToPlayer(Map map)
		{
			return AnyHostileActiveThreatTo(map, Faction.OfPlayer);
		}

		public static bool AnyHostileActiveThreatTo(Map map, Faction faction)
		{
			HashSet<IAttackTarget> hashSet = map.attackTargetsCache.TargetsHostileToFaction(faction);
			foreach (IAttackTarget item in hashSet)
			{
				if (IsActiveThreatTo(item, faction))
				{
					return true;
				}
			}
			return false;
		}

		public static bool IsActiveThreatToPlayer(IAttackTarget target)
		{
			return IsActiveThreatTo(target, Faction.OfPlayer);
		}

		public static bool IsActiveThreatTo(IAttackTarget target, Faction faction)
		{
			if (!target.Thing.HostileTo(faction))
			{
				return false;
			}
			if (!(target.Thing is IAttackTargetSearcher))
			{
				return false;
			}
			if (target.ThreatDisabled(null))
			{
				return false;
			}
			Pawn pawn = target.Thing as Pawn;
			if (pawn != null)
			{
				Lord lord = pawn.GetLord();
				if (lord != null && lord.LordJob is LordJob_DefendAndExpandHive && (pawn.mindState.duty == null || pawn.mindState.duty.def != DutyDefOf.AssaultColony))
				{
					return false;
				}
			}
			Pawn pawn2 = target.Thing as Pawn;
			if (pawn2 != null && (pawn2.MentalStateDef == MentalStateDefOf.PanicFlee || pawn2.IsPrisoner))
			{
				return false;
			}
			if (target.Thing.Spawned)
			{
				TraverseParms traverseParms = (pawn2 == null) ? TraverseParms.For(TraverseMode.PassDoors) : TraverseParms.For(pawn2);
				if (!target.Thing.Map.reachability.CanReachUnfogged(target.Thing.Position, traverseParms))
				{
					return false;
				}
			}
			return true;
		}

		public static void Notify_PawnLostForTutor(Pawn pawn, Map map)
		{
			if (!map.IsPlayerHome && map.mapPawns.FreeColonistsSpawnedCount != 0 && !AnyHostileActiveThreatToPlayer(map))
			{
				LessonAutoActivator.TeachOpportunity(ConceptDefOf.ReformCaravan, OpportunityType.Important);
			}
		}
	}
}
