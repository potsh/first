using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Verse
{
	public static class DirectXmlCrossRefLoader
	{
		private abstract class WantedRef
		{
			public object wanter;

			public abstract bool TryResolve(FailMode failReportMode);
		}

		private class WantedRefForObject : WantedRef
		{
			public FieldInfo fi;

			public string defName;

			public WantedRefForObject(object wanter, FieldInfo fi, string targetDefName)
			{
				base.wanter = wanter;
				this.fi = fi;
				defName = targetDefName;
			}

			public override bool TryResolve(FailMode failReportMode)
			{
				if (fi == null)
				{
					Log.Error("Trying to resolve null field for def named " + defName.ToStringSafe());
					return false;
				}
				Def defSilentFail = GenDefDatabase.GetDefSilentFail(fi.FieldType, defName);
				if (defSilentFail == null)
				{
					if (failReportMode == FailMode.LogErrors)
					{
						Log.Error("Could not resolve cross-reference: No " + fi.FieldType + " named " + defName.ToStringSafe() + " found to give to " + wanter.GetType() + " " + wanter.ToStringSafe());
					}
					return false;
				}
				SoundDef soundDef = defSilentFail as SoundDef;
				if (soundDef != null && soundDef.isUndefined)
				{
					Log.Warning("Could not resolve cross-reference: No " + fi.FieldType + " named " + defName.ToStringSafe() + " found to give to " + wanter.GetType() + " " + wanter.ToStringSafe() + " (using undefined sound instead)");
				}
				fi.SetValue(wanter, defSilentFail);
				return true;
			}
		}

		private class WantedRefForList<T> : WantedRef where T : new()
		{
			private List<string> defNames = new List<string>();

			private object debugWanterInfo;

			public WantedRefForList(object wanter, object debugWanterInfo)
			{
				base.wanter = wanter;
				this.debugWanterInfo = debugWanterInfo;
			}

			public void AddWantedListEntry(string newTargetDefName)
			{
				defNames.Add(newTargetDefName);
			}

			public override bool TryResolve(FailMode failReportMode)
			{
				bool flag = false;
				for (int i = 0; i < defNames.Count; i++)
				{
					T val = TryResolveDef<T>(defNames[i], failReportMode, debugWanterInfo);
					if (val != null)
					{
						((List<T>)wanter).Add(val);
						defNames.RemoveAt(i);
						i--;
					}
					else
					{
						flag = true;
					}
				}
				return !flag;
			}
		}

		private class WantedRefForDictionary<K, V> : WantedRef where K : new()where V : new()
		{
			private List<XmlNode> wantedDictRefs = new List<XmlNode>();

			private object debugWanterInfo;

			public WantedRefForDictionary(object wanter, object debugWanterInfo)
			{
				base.wanter = wanter;
				this.debugWanterInfo = debugWanterInfo;
			}

			public void AddWantedDictEntry(XmlNode entryNode)
			{
				wantedDictRefs.Add(entryNode);
			}

			public override bool TryResolve(FailMode failReportMode)
			{
				failReportMode = FailMode.LogErrors;
				bool flag = typeof(Def).IsAssignableFrom(typeof(K));
				bool flag2 = typeof(Def).IsAssignableFrom(typeof(V));
				List<Pair<K, V>> list = new List<Pair<K, V>>();
				foreach (XmlNode wantedDictRef in wantedDictRefs)
				{
					XmlNode xmlNode = wantedDictRef["key"];
					XmlNode xmlNode2 = wantedDictRef["value"];
					K first = (!flag) ? DirectXmlToObject.ObjectFromXml<K>(xmlNode, doPostLoad: true) : TryResolveDef<K>(xmlNode.InnerText, failReportMode, debugWanterInfo);
					V second = (!flag2) ? DirectXmlToObject.ObjectFromXml<V>(xmlNode2, doPostLoad: true) : TryResolveDef<V>(xmlNode2.InnerText, failReportMode, debugWanterInfo);
					list.Add(new Pair<K, V>(first, second));
				}
				Dictionary<K, V> dictionary = (Dictionary<K, V>)wanter;
				dictionary.Clear();
				foreach (Pair<K, V> item in list)
				{
					try
					{
						dictionary.Add(item.First, item.Second);
					}
					catch
					{
						Log.Error("Failed to load key/value pair: " + item.First + ", " + item.Second);
					}
				}
				return true;
			}
		}

		private static List<WantedRef> wantedRefs = new List<WantedRef>();

		public static bool LoadingInProgress => wantedRefs.Count > 0;

		public static void RegisterObjectWantsCrossRef(object wanter, FieldInfo fi, string targetDefName)
		{
			WantedRefForObject item = new WantedRefForObject(wanter, fi, targetDefName);
			wantedRefs.Add(item);
		}

		public static void RegisterObjectWantsCrossRef(object wanter, string fieldName, string targetDefName)
		{
			WantedRefForObject item = new WantedRefForObject(wanter, wanter.GetType().GetField(fieldName), targetDefName);
			wantedRefs.Add(item);
		}

		public static void RegisterListWantsCrossRef<T>(List<T> wanterList, string targetDefName, object debugWanterInfo = null) where T : new()
		{
			WantedRefForList<T> wantedRefForList = null;
			foreach (WantedRef wantedRef in wantedRefs)
			{
				if (wantedRef.wanter == wanterList)
				{
					wantedRefForList = (WantedRefForList<T>)wantedRef;
					break;
				}
			}
			if (wantedRefForList == null)
			{
				wantedRefForList = new WantedRefForList<T>(wanterList, debugWanterInfo);
				wantedRefs.Add(wantedRefForList);
			}
			wantedRefForList.AddWantedListEntry(targetDefName);
		}

		public static void RegisterDictionaryWantsCrossRef<K, V>(Dictionary<K, V> wanterDict, XmlNode entryNode, object debugWanterInfo = null) where K : new()where V : new()
		{
			WantedRefForDictionary<K, V> wantedRefForDictionary = null;
			foreach (WantedRef wantedRef in wantedRefs)
			{
				if (wantedRef.wanter == wanterDict)
				{
					wantedRefForDictionary = (WantedRefForDictionary<K, V>)wantedRef;
					break;
				}
			}
			if (wantedRefForDictionary == null)
			{
				wantedRefForDictionary = new WantedRefForDictionary<K, V>(wanterDict, debugWanterInfo);
				wantedRefs.Add(wantedRefForDictionary);
			}
			wantedRefForDictionary.AddWantedDictEntry(entryNode);
		}

		public static T TryResolveDef<T>(string defName, FailMode failReportMode, object debugWanterInfo = null)
		{
			T val = (T)(object)GenDefDatabase.GetDefSilentFail(typeof(T), defName);
			if (val != null)
			{
				return val;
			}
			if (failReportMode == FailMode.LogErrors)
			{
				string text = "Could not resolve cross-reference to " + typeof(T) + " named " + defName;
				if (debugWanterInfo != null)
				{
					text = text + " (wanter=" + debugWanterInfo.ToStringSafe() + ")";
				}
				Log.Error(text);
			}
			return default(T);
		}

		public static void Clear()
		{
			wantedRefs.Clear();
		}

		public static void ResolveAllWantedCrossReferences(FailMode failReportMode)
		{
			foreach (WantedRef item in wantedRefs.ListFullCopy())
			{
				if (item.TryResolve(failReportMode))
				{
					wantedRefs.Remove(item);
				}
			}
		}
	}
}
