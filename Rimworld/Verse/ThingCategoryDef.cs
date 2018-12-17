using System.Collections.Generic;
using UnityEngine;

namespace Verse
{
	public class ThingCategoryDef : Def
	{
		public ThingCategoryDef parent;

		[NoTranslate]
		public string iconPath;

		public bool resourceReadoutRoot;

		[Unsaved]
		public TreeNode_ThingCategory treeNode;

		[Unsaved]
		public List<ThingCategoryDef> childCategories = new List<ThingCategoryDef>();

		[Unsaved]
		public List<ThingDef> childThingDefs = new List<ThingDef>();

		[Unsaved]
		public List<SpecialThingFilterDef> childSpecialFilters = new List<SpecialThingFilterDef>();

		[Unsaved]
		public Texture2D icon = BaseContent.BadTex;

		public IEnumerable<ThingCategoryDef> Parents
		{
			get
			{
				if (parent != null)
				{
					yield return parent;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
		}

		public IEnumerable<ThingCategoryDef> ThisAndChildCategoryDefs
		{
			get
			{
				yield return this;
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public IEnumerable<ThingDef> DescendantThingDefs
		{
			get
			{
				foreach (ThingCategoryDef thisAndChildCategoryDef in ThisAndChildCategoryDefs)
				{
					using (List<ThingDef>.Enumerator enumerator2 = thisAndChildCategoryDef.childThingDefs.GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							ThingDef def = enumerator2.Current;
							yield return def;
							/*Error: Unable to find new state assignment for yield return*/;
						}
					}
				}
				yield break;
				IL_011f:
				/*Error near IL_0120: Unexpected return in MoveNext()*/;
			}
		}

		public IEnumerable<SpecialThingFilterDef> DescendantSpecialThingFilterDefs
		{
			get
			{
				foreach (ThingCategoryDef thisAndChildCategoryDef in ThisAndChildCategoryDefs)
				{
					using (List<SpecialThingFilterDef>.Enumerator enumerator2 = thisAndChildCategoryDef.childSpecialFilters.GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							SpecialThingFilterDef sf = enumerator2.Current;
							yield return sf;
							/*Error: Unable to find new state assignment for yield return*/;
						}
					}
				}
				yield break;
				IL_011f:
				/*Error near IL_0120: Unexpected return in MoveNext()*/;
			}
		}

		public IEnumerable<SpecialThingFilterDef> ParentsSpecialThingFilterDefs
		{
			get
			{
				foreach (ThingCategoryDef parent2 in Parents)
				{
					using (List<SpecialThingFilterDef>.Enumerator enumerator2 = parent2.childSpecialFilters.GetEnumerator())
					{
						if (enumerator2.MoveNext())
						{
							SpecialThingFilterDef filter = enumerator2.Current;
							yield return filter;
							/*Error: Unable to find new state assignment for yield return*/;
						}
					}
				}
				yield break;
				IL_011f:
				/*Error near IL_0120: Unexpected return in MoveNext()*/;
			}
		}

		public override void PostLoad()
		{
			treeNode = new TreeNode_ThingCategory(this);
			if (!iconPath.NullOrEmpty())
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					icon = ContentFinder<Texture2D>.Get(iconPath);
				});
			}
		}

		public static ThingCategoryDef Named(string defName)
		{
			return DefDatabase<ThingCategoryDef>.GetNamed(defName);
		}

		public override int GetHashCode()
		{
			return defName.GetHashCode();
		}
	}
}
