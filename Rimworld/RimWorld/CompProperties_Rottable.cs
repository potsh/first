using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class CompProperties_Rottable : CompProperties
	{
		public float daysToRotStart = 2f;

		public bool rotDestroys;

		public float rotDamagePerDay = 40f;

		public float daysToDessicated = 999f;

		public float dessicatedDamagePerDay;

		public bool disableIfHatcher;

		public int TicksToRotStart => Mathf.RoundToInt(daysToRotStart * 60000f);

		public int TicksToDessicated => Mathf.RoundToInt(daysToDessicated * 60000f);

		public CompProperties_Rottable()
		{
			compClass = typeof(CompRottable);
		}

		public CompProperties_Rottable(float daysToRotStart)
		{
			this.daysToRotStart = daysToRotStart;
		}

		public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
		{
			using (IEnumerator<string> enumerator = base.ConfigErrors(parentDef).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string e = enumerator.Current;
					yield return e;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			if (parentDef.tickerType != TickerType.Normal && parentDef.tickerType != TickerType.Rare)
			{
				yield return "CompRottable needs tickerType " + TickerType.Rare + " or " + TickerType.Normal + ", has " + parentDef.tickerType;
				/*Error: Unable to find new state assignment for yield return*/;
			}
			yield break;
			IL_0147:
			/*Error near IL_0148: Unexpected return in MoveNext()*/;
		}
	}
}
