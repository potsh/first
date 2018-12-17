using System.Collections.Generic;
using UnityEngine;

namespace Verse
{
	public static class UnityGUIBugsFixer
	{
		private static List<Resolution> resolutions = new List<Resolution>();

		private const float ScrollFactor = -6f;

		public static List<Resolution> ScreenResolutionsWithoutDuplicates
		{
			get
			{
				resolutions.Clear();
				Resolution[] array = Screen.resolutions;
				for (int i = 0; i < array.Length; i++)
				{
					bool flag = false;
					for (int j = 0; j < resolutions.Count; j++)
					{
						if (resolutions[j].width == array[i].width && resolutions[j].height == array[i].height)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						resolutions.Add(array[i]);
					}
				}
				return resolutions;
			}
		}

		public static void OnGUI()
		{
			FixScrolling();
			FixShift();
		}

		private static void FixScrolling()
		{
			if (Event.current.type == EventType.ScrollWheel && (Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.LinuxPlayer))
			{
				Vector2 delta = Event.current.delta;
				Event.current.delta = new Vector2(delta.x, delta.y * -6f);
			}
		}

		private static void FixShift()
		{
			if ((Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.LinuxPlayer) && !Event.current.shift)
			{
				Event.current.shift = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
			}
		}
	}
}
