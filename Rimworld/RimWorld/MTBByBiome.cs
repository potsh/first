using System.Xml;
using Verse;

namespace RimWorld
{
	public class MTBByBiome
	{
		public BiomeDef biome;

		public float mtbDays;

		public void LoadDataFromXmlCustom(XmlNode xmlRoot)
		{
			if (xmlRoot.ChildNodes.Count != 1)
			{
				Log.Error("Misconfigured MTBByBiome: " + xmlRoot.OuterXml);
			}
			else
			{
				DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "biome", xmlRoot.Name);
				mtbDays = (float)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(float));
			}
		}
	}
}
