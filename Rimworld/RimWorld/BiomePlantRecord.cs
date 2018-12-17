using System.Xml;
using Verse;

namespace RimWorld
{
	public class BiomePlantRecord
	{
		public ThingDef plant;

		public float commonality;

		public void LoadDataFromXmlCustom(XmlNode xmlRoot)
		{
			DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "plant", xmlRoot.Name);
			commonality = (float)ParseHelper.FromString(xmlRoot.FirstChild.Value, typeof(float));
		}
	}
}
