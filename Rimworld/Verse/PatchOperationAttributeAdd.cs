using System;
using System.Collections;
using System.Xml;

namespace Verse
{
	public class PatchOperationAttributeAdd : PatchOperationAttribute
	{
		protected string value;

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
					if (xmlNode.Attributes[attribute] == null)
					{
						XmlAttribute xmlAttribute = xmlNode.OwnerDocument.CreateAttribute(attribute);
						xmlAttribute.Value = value;
						xmlNode.Attributes.Append(xmlAttribute);
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
