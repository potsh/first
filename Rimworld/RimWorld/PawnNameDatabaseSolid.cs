using System;
using System.Collections;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public static class PawnNameDatabaseSolid
	{
		private static Dictionary<GenderPossibility, List<NameTriple>> solidNames;

		private const float PreferredNameChance = 0.5f;

		static PawnNameDatabaseSolid()
		{
			solidNames = new Dictionary<GenderPossibility, List<NameTriple>>();
			IEnumerator enumerator = Enum.GetValues(typeof(GenderPossibility)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					GenderPossibility key = (GenderPossibility)enumerator.Current;
					solidNames.Add(key, new List<NameTriple>());
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}

		public static void AddPlayerContentName(NameTriple newName, GenderPossibility genderPos)
		{
			solidNames[genderPos].Add(newName);
		}

		public static List<NameTriple> GetListForGender(GenderPossibility gp)
		{
			return solidNames[gp];
		}

		public static IEnumerable<NameTriple> AllNames()
		{
			foreach (KeyValuePair<GenderPossibility, List<NameTriple>> solidName in solidNames)
			{
				using (List<NameTriple>.Enumerator enumerator2 = solidName.Value.GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						NameTriple name = enumerator2.Current;
						yield return name;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			yield break;
			IL_0114:
			/*Error near IL_0115: Unexpected return in MoveNext()*/;
		}
	}
}
