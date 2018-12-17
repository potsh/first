using System;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public abstract class JobGiver_ManTurrets : ThinkNode_JobGiver
	{
		public float maxDistFromPoint = -1f;

		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_ManTurrets jobGiver_ManTurrets = (JobGiver_ManTurrets)base.DeepCopy(resolve);
			jobGiver_ManTurrets.maxDistFromPoint = maxDistFromPoint;
			return jobGiver_ManTurrets;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			Predicate<Thing> validator = delegate(Thing t)
			{
				if (!t.def.hasInteractionCell)
				{
					return false;
				}
				if (!t.def.HasComp(typeof(CompMannable)))
				{
					return false;
				}
				if (!pawn.CanReserve(t))
				{
					return false;
				}
				if (JobDriver_ManTurret.FindAmmoForTurret(pawn, (Building_TurretGun)t) == null)
				{
					return false;
				}
				return true;
			};
			Thing thing = GenClosest.ClosestThingReachable(GetRoot(pawn), pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.InteractionCell, TraverseParms.For(pawn), maxDistFromPoint, validator);
			if (thing != null)
			{
				Job job = new Job(JobDefOf.ManTurret, thing);
				job.expiryInterval = 2000;
				job.checkOverrideOnExpire = true;
				return job;
			}
			return null;
		}

		protected abstract IntVec3 GetRoot(Pawn pawn);
	}
}
