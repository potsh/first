using System;
using System.Collections;
using System.Xml;

namespace Verse
{
	public class PatchOperationAttributeRemove : PatchOperationAttribute
	{
		protected override bool ApplyWorker(XmlDocument xml)
		{
			bool result = false;
			IEnumerator enumerator = xml.SelectNodes(xpath).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					XmlNode xmlNode = current as XmlNode;
					if (xmlNode.Attributes[attribute] != null)
					{
						xmlNode.Attributes.Remove(xmlNode.Attributes[attribute]);
						result = true;
					}
				}
				return result;
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
