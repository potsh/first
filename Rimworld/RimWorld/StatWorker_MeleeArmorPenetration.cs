using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public class StatWorker_MeleeArmorPenetration : StatWorker
	{
		public override bool IsDisabledFor(Thing thing)
		{
			return base.IsDisabledFor(thing) || StatDefOf.MeleeHitChance.Worker.IsDisabledFor(thing);
		}

		public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
		{
			if (req.Thing == null)
			{
				Log.Error("Getting MeleeArmorPenetration stat for " + req.Def + " without concrete pawn. This always returns 0.");
			}
			return GetArmorPenetration(req, applyPostProcess);
		}

		private float GetArmorPenetration(StatRequest req, bool applyPostProcess = true)
		{
			Pawn pawn = req.Thing as Pawn;
			if (pawn == null)
			{
				return 0f;
			}
			List<VerbEntry> updatedAvailableVerbsList = pawn.meleeVerbs.GetUpdatedAvailableVerbsList(terrainTools: false);
			if (updatedAvailableVerbsList.Count == 0)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < updatedAvailableVerbsList.Count; i++)
			{
				if (updatedAvailableVerbsList[i].IsMeleeAttack)
				{
					num += updatedAvailableVerbsList[i].GetSelectionWeight(null);
				}
			}
			if (num == 0f)
			{
				return 0f;
			}
			float num2 = 0f;
			for (int j = 0; j < updatedAvailableVerbsList.Count; j++)
			{
				if (updatedAvailableVerbsList[j].IsMeleeAttack)
				{
					float num3 = num2;
					float num4 = updatedAvailableVerbsList[j].GetSelectionWeight(null) / num;
					VerbEntry verbEntry = updatedAvailableVerbsList[j];
					VerbProperties verbProps = verbEntry.verb.verbProps;
					VerbEntry verbEntry2 = updatedAvailableVerbsList[j];
					num2 = num3 + num4 * verbProps.AdjustedArmorPenetration(verbEntry2.verb, pawn);
				}
			}
			return num2;
		}
	}
}
