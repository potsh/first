using RuntimeAudioClipLoader;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Verse
{
	public static class ModContentLoader<T> where T : class
	{
		private static string[] AcceptableExtensionsAudio = new string[7]
		{
			".wav",
			".mp3",
			".ogg",
			".xm",
			".it",
			".mod",
			".s3m"
		};

		private static string[] AcceptableExtensionsTexture = new string[2]
		{
			".png",
			".jpg"
		};

		private static string[] AcceptableExtensionsString = new string[1]
		{
			".txt"
		};

		private static bool IsAcceptableExtension(string extension)
		{
			string[] array;
			if (typeof(T) == typeof(AudioClip))
			{
				array = AcceptableExtensionsAudio;
			}
			else if (typeof(T) == typeof(Texture2D))
			{
				array = AcceptableExtensionsTexture;
			}
			else
			{
				if (typeof(T) != typeof(string))
				{
					Log.Error("Unknown content type " + typeof(T));
					return false;
				}
				array = AcceptableExtensionsString;
			}
			string[] array2 = array;
			foreach (string b in array2)
			{
				if (extension.ToLower() == b)
				{
					return true;
				}
			}
			return false;
		}

		public static IEnumerable<LoadedContentItem<T>> LoadAllForMod(ModContentPack mod)
		{
			string contentDirPath = Path.Combine(mod.RootDir, GenFilePaths.ContentPath<T>());
			DirectoryInfo contentDir = new DirectoryInfo(contentDirPath);
			if (contentDir.Exists)
			{
				DeepProfiler.Start("Loading assets of type " + typeof(T) + " for mod " + mod);
				FileInfo[] files = contentDir.GetFiles("*.*", SearchOption.AllDirectories);
				foreach (FileInfo file in files)
				{
					if (IsAcceptableExtension(file.Extension))
					{
						LoadedContentItem<T> loadedItem = LoadItem(file.FullName, contentDirPath);
						if (loadedItem != null)
						{
							yield return loadedItem;
							/*Error: Unable to find new state assignment for yield return*/;
						}
					}
				}
				DeepProfiler.End();
			}
		}

		public static LoadedContentItem<T> LoadItem(string absFilePath, string contentDirPath = null)
		{
			string text = absFilePath;
			if (contentDirPath != null)
			{
				text = text.Substring(contentDirPath.ToString().Length);
			}
			text = text.Substring(0, text.Length - Path.GetExtension(text).Length);
			text = text.Replace('\\', '/');
			try
			{
				if (typeof(T) == typeof(string))
				{
					return new LoadedContentItem<T>(text, (T)(object)GenFile.TextFromRawFile(absFilePath));
				}
				if (typeof(T) == typeof(Texture2D))
				{
					return new LoadedContentItem<T>(text, (T)(object)LoadPNG(absFilePath));
				}
				if (typeof(T) == typeof(AudioClip))
				{
					if (Prefs.LogVerbose)
					{
						DeepProfiler.Start("Loading file " + text);
					}
					T val;
					try
					{
						bool doStream = ShouldStreamAudioClipFromPath(absFilePath);
						val = (T)(object)Manager.Load(absFilePath, doStream);
					}
					finally
					{
						if (Prefs.LogVerbose)
						{
							DeepProfiler.End();
						}
					}
					UnityEngine.Object @object = val as UnityEngine.Object;
					if (@object != null)
					{
						@object.name = Path.GetFileNameWithoutExtension(new FileInfo(absFilePath).Name);
					}
					return new LoadedContentItem<T>(text, val);
				}
			}
			catch (Exception ex)
			{
				Log.Error("Exception loading " + typeof(T) + " from file.\nabsFilePath: " + absFilePath + "\ncontentDirPath: " + contentDirPath + "\nException: " + ex.ToString());
			}
			if (typeof(T) == typeof(Texture2D))
			{
				return (LoadedContentItem<T>)new LoadedContentItem<Texture2D>(absFilePath, BaseContent.BadTex);
			}
			return null;
		}

		private static bool ShouldStreamAudioClipFromPath(string absPath)
		{
			if (!File.Exists(absPath))
			{
				return false;
			}
			FileInfo fileInfo = new FileInfo(absPath);
			return fileInfo.Length > 307200;
		}

		private static Texture2D LoadPNG(string filePath)
		{
			Texture2D texture2D = null;
			if (File.Exists(filePath))
			{
				byte[] data = File.ReadAllBytes(filePath);
				texture2D = new Texture2D(2, 2, TextureFormat.Alpha8, mipmap: true);
				texture2D.LoadImage(data);
				texture2D.Compress(highQuality: true);
				texture2D.name = Path.GetFileNameWithoutExtension(filePath);
				texture2D.filterMode = FilterMode.Bilinear;
				texture2D.anisoLevel = 2;
				texture2D.Apply(updateMipmaps: true, makeNoLongerReadable: true);
			}
			return texture2D;
		}
	}
}
