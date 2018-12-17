using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	public class IncidentWorker_AnimalInsanityMass : IncidentWorker
	{
		public static bool AnimalUsable(Pawn p)
		{
			return p.Spawned && !p.Position.Fogged(p.Map) && (!p.InMentalState || !p.MentalStateDef.IsAggro) && !p.Downed && p.Faction == null;
		}

		public static void DriveInsane(Pawn p)
		{
			p.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Manhunter, null, forceWake: true);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (parms.points <= 0f)
			{
				Log.Error("AnimalInsanity running without points.");
				parms.points = (float)(int)(map.strengthWatcher.StrengthRating * 50f);
			}
			float adjustedPoints = parms.points;
			if (adjustedPoints > 250f)
			{
				adjustedPoints -= 250f;
				adjustedPoints *= 0.5f;
				adjustedPoints += 250f;
			}
			IEnumerable<PawnKindDef> source = from def in DefDatabase<PawnKindDef>.AllDefs
			where def.RaceProps.Animal && def.combatPower <= adjustedPoints && (from p in map.mapPawns.AllPawnsSpawned
			where p.kindDef == def && AnimalUsable(p)
			select p).Count() >= 3
			select def;
			if (!source.TryRandomElement(out PawnKindDef animalDef))
			{
				return false;
			}
			List<Pawn> list = (from p in map.mapPawns.AllPawnsSpawned
			where p.kindDef == animalDef && AnimalUsable(p)
			select p).ToList();
			float combatPower = animalDef.combatPower;
			float num = 0f;
			int num2 = 0;
			Pawn pawn = null;
			list.Shuffle();
			foreach (Pawn item in list)
			{
				if (num + combatPower > adjustedPoints)
				{
					break;
				}
				DriveInsane(item);
				num += combatPower;
				num2++;
				pawn = item;
			}
			if (num == 0f)
			{
				return false;
			}
			string label;
			string text;
			LetterDef textLetterDef;
			if (num2 == 1)
			{
				label = "LetterLabelAnimalInsanitySingle".Translate(pawn.LabelShort, pawn.Named("ANIMAL"));
				text = "AnimalInsanitySingle".Translate(pawn.LabelShort, pawn.Named("ANIMAL"));
				textLetterDef = LetterDefOf.ThreatSmall;
			}
			else
			{
				label = "LetterLabelAnimalInsanityMultiple".Translate(animalDef.GetLabelPlural());
				text = "AnimalInsanityMultiple".Translate(animalDef.GetLabelPlural());
				textLetterDef = LetterDefOf.ThreatBig;
			}
			Find.LetterStack.ReceiveLetter(label, text, textLetterDef, pawn);
			SoundDefOf.PsychicPulseGlobal.PlayOneShotOnCamera(map);
			if (map == Find.CurrentMap)
			{
				Find.CameraDriver.shaker.DoShake(1f);
			}
			return true;
		}
	}
}
