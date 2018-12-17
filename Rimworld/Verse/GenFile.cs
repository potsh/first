using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Verse
{
	public static class GenFile
	{
		public static string TextFromRawFile(string filePath)
		{
			return File.ReadAllText(filePath);
		}

		public static string TextFromResourceFile(string filePath)
		{
			TextAsset textAsset = Resources.Load("Text/" + filePath) as TextAsset;
			if (textAsset == null)
			{
				Log.Message("Found no text asset in resources at " + filePath);
				return null;
			}
			return GetTextWithoutBOM(textAsset);
		}

		public static string GetTextWithoutBOM(TextAsset textAsset)
		{
			string text = null;
			using (MemoryStream stream = new MemoryStream(textAsset.bytes))
			{
				using (StreamReader streamReader = new StreamReader(stream, detectEncodingFromByteOrderMarks: true))
				{
					return streamReader.ReadToEnd();
				}
			}
		}

		public static IEnumerable<string> LinesFromFile(string filePath)
		{
			string rawText = TextFromResourceFile(filePath);
			using (IEnumerator<string> enumerator = GenText.LinesFromString(rawText).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					string line = enumerator.Current;
					yield return line;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			yield break;
			IL_00ca:
			/*Error near IL_00cb: Unexpected return in MoveNext()*/;
		}

		public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, bool useLinuxLineEndings = false)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(sourceDirName);
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			if (!directoryInfo.Exists)
			{
				throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
			}
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}
			FileInfo[] files = directoryInfo.GetFiles();
			FileInfo[] array = files;
			foreach (FileInfo fileInfo in array)
			{
				string text = Path.Combine(destDirName, fileInfo.Name);
				if (useLinuxLineEndings && (fileInfo.Extension == ".sh" || fileInfo.Extension == ".txt"))
				{
					if (!File.Exists(text))
					{
						File.WriteAllText(text, File.ReadAllText(fileInfo.FullName).Replace("\r\n", "\n").Replace("\r", "\n"));
					}
				}
				else
				{
					fileInfo.CopyTo(text, overwrite: false);
				}
			}
			if (copySubDirs)
			{
				DirectoryInfo[] array2 = directories;
				foreach (DirectoryInfo directoryInfo2 in array2)
				{
					string destDirName2 = Path.Combine(destDirName, directoryInfo2.Name);
					DirectoryCopy(directoryInfo2.FullName, destDirName2, copySubDirs, useLinuxLineEndings);
				}
			}
		}

		public static string SanitizedFileName(string fileName)
		{
			char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
			string text = string.Empty;
			for (int i = 0; i < fileName.Length; i++)
			{
				if (!invalidFileNameChars.Contains(fileName[i]))
				{
					text += fileName[i];
				}
			}
			if (text.Length == 0)
			{
				text = "unnamed";
			}
			return text;
		}
	}
}
