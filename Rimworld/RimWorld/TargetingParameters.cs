using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class TargetingParameters
	{
		public bool canTargetLocations;

		public bool canTargetSelf;

		public bool canTargetPawns = true;

		public bool canTargetFires;

		public bool canTargetBuildings = true;

		public bool canTargetItems;

		public List<Faction> onlyTargetFactions;

		public Predicate<TargetInfo> validator;

		public bool onlyTargetFlammables;

		public Thing targetSpecificThing;

		public bool mustBeSelectable;

		public bool neverTargetDoors;

		public bool neverTargetIncapacitated;

		public bool onlyTargetThingsAffectingRegions;

		public bool onlyTargetDamagedThings;

		public bool mapObjectTargetsMustBeAutoAttackable = true;

		public bool onlyTargetIncapacitatedPawns;

		public bool CanTarget(TargetInfo targ)
		{
			if (validator != null && !validator(targ))
			{
				return false;
			}
			if (targ.Thing == null)
			{
				return canTargetLocations;
			}
			if (neverTargetDoors && targ.Thing.def.IsDoor)
			{
				return false;
			}
			if (onlyTargetDamagedThings && targ.Thing.HitPoints == targ.Thing.MaxHitPoints)
			{
				return false;
			}
			if (onlyTargetFlammables && !targ.Thing.FlammableNow)
			{
				return false;
			}
			if (mustBeSelectable && !ThingSelectionUtility.SelectableByMapClick(targ.Thing))
			{
				return false;
			}
			if (targetSpecificThing != null && targ.Thing == targetSpecificThing)
			{
				return true;
			}
			if (canTargetFires && targ.Thing.def == ThingDefOf.Fire)
			{
				return true;
			}
			if (canTargetPawns && targ.Thing.def.category == ThingCategory.Pawn)
			{
				if (((Pawn)targ.Thing).Downed)
				{
					if (neverTargetIncapacitated)
					{
						return false;
					}
				}
				else if (onlyTargetIncapacitatedPawns)
				{
					return false;
				}
				if (onlyTargetFactions != null && !onlyTargetFactions.Contains(targ.Thing.Faction))
				{
					return false;
				}
				return true;
			}
			if (canTargetBuildings && targ.Thing.def.category == ThingCategory.Building)
			{
				if (onlyTargetThingsAffectingRegions && !targ.Thing.def.AffectsRegions)
				{
					return false;
				}
				if (onlyTargetFactions != null && !onlyTargetFactions.Contains(targ.Thing.Faction))
				{
					return false;
				}
				return true;
			}
			if (canTargetItems)
			{
				if (mapObjectTargetsMustBeAutoAttackable && !targ.Thing.def.isAutoAttackableMapObject)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		public static TargetingParameters ForSelf(Pawn p)
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.targetSpecificThing = p;
			targetingParameters.canTargetPawns = false;
			targetingParameters.canTargetBuildings = false;
			targetingParameters.mapObjectTargetsMustBeAutoAttackable = false;
			return targetingParameters;
		}

		public static TargetingParameters ForArrest(Pawn arrester)
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.canTargetBuildings = false;
			targetingParameters.mapObjectTargetsMustBeAutoAttackable = false;
			targetingParameters.validator = delegate(TargetInfo targ)
			{
				if (!targ.HasThing)
				{
					return false;
				}
				Pawn pawn = targ.Thing as Pawn;
				if (pawn == null || pawn == arrester || !pawn.CanBeArrestedBy(arrester))
				{
					return false;
				}
				if (pawn.Downed)
				{
					return false;
				}
				return true;
			};
			return targetingParameters;
		}

		public static TargetingParameters ForAttackHostile()
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.canTargetBuildings = true;
			targetingParameters.canTargetItems = true;
			targetingParameters.mapObjectTargetsMustBeAutoAttackable = true;
			targetingParameters.validator = delegate(TargetInfo targ)
			{
				if (!targ.HasThing)
				{
					return false;
				}
				if (targ.Thing.HostileTo(Faction.OfPlayer))
				{
					return true;
				}
				Pawn pawn = targ.Thing as Pawn;
				if (pawn != null && pawn.NonHumanlikeOrWildMan())
				{
					return true;
				}
				return false;
			};
			return targetingParameters;
		}

		public static TargetingParameters ForAttackAny()
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.canTargetBuildings = true;
			targetingParameters.canTargetItems = true;
			targetingParameters.mapObjectTargetsMustBeAutoAttackable = true;
			return targetingParameters;
		}

		public static TargetingParameters ForRescue(Pawn p)
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.onlyTargetIncapacitatedPawns = true;
			targetingParameters.canTargetBuildings = false;
			targetingParameters.mapObjectTargetsMustBeAutoAttackable = false;
			return targetingParameters;
		}

		public static TargetingParameters ForStrip(Pawn p)
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.canTargetItems = true;
			targetingParameters.mapObjectTargetsMustBeAutoAttackable = false;
			targetingParameters.validator = delegate(TargetInfo targ)
			{
				if (!targ.HasThing)
				{
					return false;
				}
				return StrippableUtility.CanBeStrippedByColony(targ.Thing);
			};
			return targetingParameters;
		}

		public static TargetingParameters ForTrade()
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.canTargetBuildings = false;
			targetingParameters.mapObjectTargetsMustBeAutoAttackable = false;
			targetingParameters.validator = ((TargetInfo x) => (x.Thing as ITrader)?.CanTradeNow ?? false);
			return targetingParameters;
		}

		public static TargetingParameters ForDropPodsDestination()
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetLocations = true;
			targetingParameters.canTargetSelf = false;
			targetingParameters.canTargetPawns = false;
			targetingParameters.canTargetFires = false;
			targetingParameters.canTargetBuildings = false;
			targetingParameters.canTargetItems = false;
			targetingParameters.validator = ((TargetInfo x) => DropCellFinder.IsGoodDropSpot(x.Cell, x.Map, allowFogged: false, canRoofPunch: true));
			return targetingParameters;
		}

		public static TargetingParameters ForQuestPawnsWhoWillJoinColony(Pawn p)
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = true;
			targetingParameters.canTargetBuildings = false;
			targetingParameters.mapObjectTargetsMustBeAutoAttackable = false;
			targetingParameters.validator = delegate(TargetInfo x)
			{
				Pawn pawn = x.Thing as Pawn;
				return pawn != null && !pawn.Dead && pawn.mindState.WillJoinColonyIfRescued;
			};
			return targetingParameters;
		}

		public static TargetingParameters ForOpen(Pawn p)
		{
			TargetingParameters targetingParameters = new TargetingParameters();
			targetingParameters.canTargetPawns = false;
			targetingParameters.canTargetBuildings = true;
			targetingParameters.mapObjectTargetsMustBeAutoAttackable = false;
			targetingParameters.validator = ((TargetInfo x) => (x.Thing as IOpenable)?.CanOpen ?? false);
			return targetingParameters;
		}
	}
}
