using System.Collections.Generic;

namespace Verse
{
	public static class DelayedErrorWindowRequest
	{
		private struct Request
		{
			public string text;

			public string title;
		}

		private static List<Request> requests = new List<Request>();

		public static void DelayedErrorWindowRequestOnGUI()
		{
			try
			{
				for (int i = 0; i < requests.Count; i++)
				{
					WindowStack windowStack = Find.WindowStack;
					Request request = requests[i];
					string text = request.text;
					string buttonAText = "OK".Translate();
					Request request2 = requests[i];
					string title = request2.title;
					windowStack.Add(new Dialog_MessageBox(text, buttonAText, null, null, null, title));
				}
			}
			finally
			{
				requests.Clear();
			}
		}

		public static void Add(string text, string title = null)
		{
			Request item = default(Request);
			item.text = text;
			item.title = title;
			requests.Add(item);
		}
	}
}
