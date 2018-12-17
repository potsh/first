using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public static class BillUtility
	{
		public static Bill Clipboard;

		public static void TryDrawIngredientSearchRadiusOnMap(this Bill bill, IntVec3 center)
		{
			if (bill.ingredientSearchRadius < GenRadial.MaxRadialPatternRadius)
			{
				GenDraw.DrawRadiusRing(center, bill.ingredientSearchRadius);
			}
		}

		public static Bill MakeNewBill(this RecipeDef recipe)
		{
			if (recipe.UsesUnfinishedThing)
			{
				return new Bill_ProductionWithUft(recipe);
			}
			return new Bill_Production(recipe);
		}

		public static IEnumerable<IBillGiver> GlobalBillGivers()
		{
			foreach (Map map in Find.Maps)
			{
				foreach (Thing item in map.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.PotentialBillGiver)))
				{
					IBillGiver billgiver3 = item as IBillGiver;
					if (billgiver3 != null)
					{
						yield return billgiver3;
						/*Error: Unable to find new state assignment for yield return*/;
					}
					Log.ErrorOnce("Found non-bill-giver tagged as PotentialBillGiver", 13389774);
				}
				foreach (Thing item2 in map.listerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.MinifiedThing)))
				{
					IBillGiver billgiver2 = item2.GetInnerIfMinified() as IBillGiver;
					if (billgiver2 != null)
					{
						yield return billgiver2;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			foreach (Caravan caravan in Find.WorldObjects.Caravans)
			{
				foreach (Thing allThing in caravan.AllThings)
				{
					IBillGiver billgiver = allThing.GetInnerIfMinified() as IBillGiver;
					if (billgiver != null)
					{
						yield return billgiver;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			yield break;
			IL_0325:
			/*Error near IL_0326: Unexpected return in MoveNext()*/;
		}

		public static IEnumerable<Bill> GlobalBills()
		{
			foreach (IBillGiver item in GlobalBillGivers())
			{
				using (IEnumerator<Bill> enumerator2 = item.BillStack.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						Bill bill = enumerator2.Current;
						yield return bill;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			if (Clipboard != null)
			{
				yield return Clipboard;
				/*Error: Unable to find new state assignment for yield return*/;
			}
			yield break;
			IL_014b:
			/*Error near IL_014c: Unexpected return in MoveNext()*/;
		}

		public static void Notify_ZoneStockpileRemoved(Zone_Stockpile stockpile)
		{
			foreach (Bill item in GlobalBills())
			{
				item.ValidateSettings();
			}
		}

		public static void Notify_ColonistUnavailable(Pawn pawn)
		{
			try
			{
				foreach (Bill item in GlobalBills())
				{
					item.ValidateSettings();
				}
			}
			catch (Exception arg)
			{
				Log.Error("Could not notify bills: " + arg);
			}
		}

		public static WorkGiverDef GetWorkgiver(this IBillGiver billGiver)
		{
			Thing thing = billGiver as Thing;
			if (thing == null)
			{
				Log.ErrorOnce($"Attempting to get the workgiver for a non-Thing IBillGiver {billGiver.ToString()}", 96810282);
				return null;
			}
			List<WorkGiverDef> allDefsListForReading = DefDatabase<WorkGiverDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				WorkGiverDef workGiverDef = allDefsListForReading[i];
				WorkGiver_DoBill workGiver_DoBill = workGiverDef.Worker as WorkGiver_DoBill;
				if (workGiver_DoBill != null && workGiver_DoBill.ThingIsUsableBillGiver(thing))
				{
					return workGiverDef;
				}
			}
			Log.ErrorOnce($"Can't find a WorkGiver for a BillGiver {thing.ToString()}", 57348705);
			return null;
		}
	}
}
