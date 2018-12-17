using System.Collections.Generic;
using UnityEngine;

namespace Verse.Sound
{
	public class AudioGrain_Folder : AudioGrain
	{
		[LoadAlias("clipPath")]
		[NoTranslate]
		public string clipFolderPath = string.Empty;

		public override IEnumerable<ResolvedGrain> GetResolvedGrains()
		{
			using (IEnumerator<AudioClip> enumerator = ContentFinder<AudioClip>.GetAllInFolder(clipFolderPath).GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					AudioClip folderClip = enumerator.Current;
					yield return (ResolvedGrain)new ResolvedGrain_Clip(folderClip);
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			yield break;
			IL_00c3:
			/*Error near IL_00c4: Unexpected return in MoveNext()*/;
		}
	}
}
