namespace Verse.AI
{
	public class JobGiver_WanderNearMaster : JobGiver_Wander
	{
		public JobGiver_WanderNearMaster()
		{
			wanderRadius = 3f;
			ticksBetweenWandersRange = new IntRange(125, 200);
			wanderDestValidator = delegate(Pawn p, IntVec3 c, IntVec3 root)
			{
				if (MustUseRootRoom(p))
				{
					Room room = root.GetRoom(p.Map);
					if (room != null && !WanderRoomUtility.IsValidWanderDest(p, c, root))
					{
						return false;
					}
				}
				return true;
			};
		}

		protected override IntVec3 GetWanderRoot(Pawn pawn)
		{
			return WanderUtility.BestCloseWanderRoot(pawn.playerSettings.Master.PositionHeld, pawn);
		}

		private bool MustUseRootRoom(Pawn pawn)
		{
			Pawn master = pawn.playerSettings.Master;
			return !master.playerSettings.animalsReleased;
		}
	}
}
