using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace RimWorld
{
	public static class TransporterUtility
	{
		public static void GetTransportersInGroup(int transportersGroup, Map map, List<CompTransporter> outTransporters)
		{
			outTransporters.Clear();
			if (transportersGroup >= 0)
			{
				List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.Transporter);
				for (int i = 0; i < list.Count; i++)
				{
					CompTransporter compTransporter = list[i].TryGetComp<CompTransporter>();
					if (compTransporter.groupID == transportersGroup)
					{
						outTransporters.Add(compTransporter);
					}
				}
			}
		}

		public static Lord FindLord(int transportersGroup, Map map)
		{
			List<Lord> lords = map.lordManager.lords;
			for (int i = 0; i < lords.Count; i++)
			{
				LordJob_LoadAndEnterTransporters lordJob_LoadAndEnterTransporters = lords[i].LordJob as LordJob_LoadAndEnterTransporters;
				if (lordJob_LoadAndEnterTransporters != null && lordJob_LoadAndEnterTransporters.transportersGroup == transportersGroup)
				{
					return lords[i];
				}
			}
			return null;
		}

		public static bool WasLoadingCanceled(Thing transporter)
		{
			CompTransporter compTransporter = transporter.TryGetComp<CompTransporter>();
			if (compTransporter != null && !compTransporter.LoadingInProgressOrReadyToLaunch)
			{
				return true;
			}
			return false;
		}
	}
}
