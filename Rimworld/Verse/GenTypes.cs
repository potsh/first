using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Verse
{
	public static class GenTypes
	{
		public static readonly List<string> IgnoredNamespaceNames = new List<string>
		{
			"RimWorld",
			"Verse",
			"Verse.AI",
			"Verse.Sound",
			"Verse.Grammar",
			"RimWorld.Planet",
			"RimWorld.BaseGen"
		};

		private static IEnumerable<Assembly> AllActiveAssemblies
		{
			get
			{
				yield return Assembly.GetExecutingAssembly();
				/*Error: Unable to find new state assignment for yield return*/;
			}
		}

		public static IEnumerable<Type> AllTypes
		{
			get
			{
				foreach (Assembly allActiveAssembly in AllActiveAssemblies)
				{
					Type[] assemblyTypes = null;
					try
					{
						assemblyTypes = allActiveAssembly.GetTypes();
					}
					catch (ReflectionTypeLoadException)
					{
						Log.Error("Exception getting types in assembly " + allActiveAssembly.ToString());
					}
					if (assemblyTypes != null)
					{
						Type[] array = assemblyTypes;
						int num = 0;
						if (num < array.Length)
						{
							Type type = array[num];
							yield return type;
							/*Error: Unable to find new state assignment for yield return*/;
						}
					}
				}
				yield break;
				IL_0148:
				/*Error near IL_0149: Unexpected return in MoveNext()*/;
			}
		}

		public static IEnumerable<Type> AllTypesWithAttribute<TAttr>() where TAttr : Attribute
		{
			return AllTypes.Where(GenAttribute.HasAttribute<TAttr>);
		}

		public static IEnumerable<Type> AllSubclasses(this Type baseType)
		{
			return from x in AllTypes
			where x.IsSubclassOf(baseType)
			select x;
		}

		public static IEnumerable<Type> AllSubclassesNonAbstract(this Type baseType)
		{
			return from x in AllTypes
			where x.IsSubclassOf(baseType) && !x.IsAbstract
			select x;
		}

		public static IEnumerable<Type> AllLeafSubclasses(this Type baseType)
		{
			return from type in baseType.AllSubclasses()
			where !type.AllSubclasses().Any()
			select type;
		}

		public static IEnumerable<Type> InstantiableDescendantsAndSelf(this Type baseType)
		{
			if (!baseType.IsAbstract)
			{
				yield return baseType;
				/*Error: Unable to find new state assignment for yield return*/;
			}
			foreach (Type item in baseType.AllSubclasses())
			{
				if (!item.IsAbstract)
				{
					yield return item;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			yield break;
			IL_00fd:
			/*Error near IL_00fe: Unexpected return in MoveNext()*/;
		}

		public static Type GetTypeInAnyAssembly(string typeName)
		{
			Type typeInAnyAssemblyRaw = GetTypeInAnyAssemblyRaw(typeName);
			if (typeInAnyAssemblyRaw != null)
			{
				return typeInAnyAssemblyRaw;
			}
			for (int i = 0; i < IgnoredNamespaceNames.Count; i++)
			{
				string typeName2 = IgnoredNamespaceNames[i] + "." + typeName;
				typeInAnyAssemblyRaw = GetTypeInAnyAssemblyRaw(typeName2);
				if (typeInAnyAssemblyRaw != null)
				{
					return typeInAnyAssemblyRaw;
				}
			}
			return null;
		}

		private static Type GetTypeInAnyAssemblyRaw(string typeName)
		{
			foreach (Assembly allActiveAssembly in AllActiveAssemblies)
			{
				Type type = allActiveAssembly.GetType(typeName, throwOnError: false, ignoreCase: true);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		public static string GetTypeNameWithoutIgnoredNamespaces(Type type)
		{
			if (type.IsGenericType)
			{
				return type.ToString();
			}
			for (int i = 0; i < IgnoredNamespaceNames.Count; i++)
			{
				if (type.Namespace == IgnoredNamespaceNames[i])
				{
					return type.Name;
				}
			}
			return type.FullName;
		}

		public static bool IsCustomType(Type type)
		{
			string @namespace = type.Namespace;
			return !@namespace.StartsWith("System") && !@namespace.StartsWith("UnityEngine") && !@namespace.StartsWith("Steamworks");
		}
	}
}
