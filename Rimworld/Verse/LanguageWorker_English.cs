using System;

namespace Verse
{
	public class LanguageWorker_English : LanguageWorker
	{
		public override string WithIndefiniteArticle(string str, Gender gender, bool plural = false, bool name = false)
		{
			if (str.NullOrEmpty())
			{
				return string.Empty;
			}
			if (name)
			{
				return str;
			}
			if (plural)
			{
				return str;
			}
			return "a " + str;
		}

		public override string WithDefiniteArticle(string str, Gender gender, bool plural = false, bool name = false)
		{
			if (str.NullOrEmpty())
			{
				return string.Empty;
			}
			if (name)
			{
				return str;
			}
			return "the " + str;
		}

		public override string PostProcessed(string str)
		{
			str = base.PostProcessed(str);
			if (str.StartsWith("a ", StringComparison.OrdinalIgnoreCase) && str.Length >= 3 && (str.Substring(2) == "hour" || str[2] == 'a' || str[2] == 'e' || str[2] == 'i' || str[2] == 'o' || str[2] == 'u'))
			{
				str = str.Insert(1, "n");
			}
			str = str.Replace(" a a", " an a");
			str = str.Replace(" a e", " an e");
			str = str.Replace(" a i", " an i");
			str = str.Replace(" a o", " an o");
			str = str.Replace(" a u", " an u");
			str = str.Replace(" a hour", " an hour");
			str = str.Replace(" A a", " An a");
			str = str.Replace(" A e", " An e");
			str = str.Replace(" A i", " An i");
			str = str.Replace(" A o", " An o");
			str = str.Replace(" A u", " An u");
			str = str.Replace(" A hour", " An hour");
			return str;
		}

		public override string ToTitleCase(string str)
		{
			str = base.ToTitleCase(str);
			str = str.Replace(" No. ", " no. ");
			str = str.Replace(" The ", " the ");
			str = str.Replace(" A ", " a ");
			str = str.Replace(" For ", " for ");
			str = str.Replace(" In ", " in ");
			str = str.Replace(" With ", " with ");
			return str;
		}

		public override string OrdinalNumber(int number, Gender gender = Gender.None)
		{
			int num = number % 10;
			int num2 = number / 10 % 10;
			if (num2 != 1)
			{
				switch (num)
				{
				case 1:
					return number + "st";
				case 2:
					return number + "nd";
				case 3:
					return number + "rd";
				}
			}
			return number + "th";
		}

		public override string Pluralize(string str, Gender gender, int count = -1)
		{
			if (str.NullOrEmpty())
			{
				return str;
			}
			char c = str[str.Length - 1];
			char c2 = (str.Length != 1) ? str[str.Length - 2] : '\0';
			bool flag = char.IsLetter(c2) && "oaieuyOAIEUY".IndexOf(c2) >= 0;
			bool flag2 = char.IsLetter(c2) && !flag;
			if (c == 'y' && flag2)
			{
				return str.Substring(0, str.Length - 1) + "ies";
			}
			return str + "s";
		}
	}
}
