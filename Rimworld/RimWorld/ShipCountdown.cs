using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	public static class ShipCountdown
	{
		private static float timeLeft = -1000f;

		private static Building shipRoot;

		private const float InitialTime = 7.2f;

		public static bool CountingDown => timeLeft >= 0f;

		public static void InitiateCountdown(Building launchingShipRoot)
		{
			SoundDefOf.ShipTakeoff.PlayOneShotOnCamera();
			shipRoot = launchingShipRoot;
			timeLeft = 7.2f;
			ScreenFader.StartFade(Color.white, 7.2f);
		}

		public static void ShipCountdownUpdate()
		{
			if (timeLeft > 0f)
			{
				timeLeft -= Time.deltaTime;
				if (timeLeft <= 0f)
				{
					CountdownEnded();
				}
			}
		}

		public static void CancelCountdown()
		{
			timeLeft = -1000f;
		}

		private static void CountdownEnded()
		{
			List<Building> list = ShipUtility.ShipBuildingsAttachedTo(shipRoot).ToList();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Building item in list)
			{
				Building_CryptosleepCasket building_CryptosleepCasket = item as Building_CryptosleepCasket;
				if (building_CryptosleepCasket != null && building_CryptosleepCasket.HasAnyContents)
				{
					stringBuilder.AppendLine("   " + building_CryptosleepCasket.ContainedThing.LabelCap);
					Find.StoryWatcher.statsRecord.colonistsLaunched++;
					TaleRecorder.RecordTale(TaleDefOf.LaunchedShip, building_CryptosleepCasket.ContainedThing);
				}
				item.Destroy();
			}
			string victoryText = "GameOverShipLaunched".Translate(stringBuilder.ToString(), GameVictoryUtility.PawnsLeftBehind());
			GameVictoryUtility.ShowCredits(victoryText);
		}
	}
}
