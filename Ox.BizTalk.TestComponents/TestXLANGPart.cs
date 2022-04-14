using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Microsoft.XLANGs.BaseTypes;

namespace Ox.BizTalk.TestComponents
{
	/// <summary>
	/// Implements a <see cref="XLANGPart"/> message part for use in unit testing outside of the BizTalk runtime.
	/// </summary>
	public class TestXLANGPart : XLANGPart
	{
		public override string Name { get; }
		public override XmlSchema XmlSchema { get; }
		public override XmlSchemaCollection XmlSchemaCollection { get; }

		protected object PartData { get; }

		public TestXLANGPart(string name, object partData) : this(name, partData, null, null) { }

		public TestXLANGPart(string name, object partData, XmlSchema xmlSchema, XmlSchemaCollection xmlSchemaCollection)
		{
			this.Name = name;
			this.PartData = partData;
			this.XmlSchema = xmlSchema;
			this.XmlSchemaCollection = xmlSchemaCollection;
		}

		

		public override void Dispose()
		{
			
		}

		protected Dictionary<Type, object> partProperties = new Dictionary<Type, object>();

		public override object GetPartProperty(Type propType)
		{
			if (propType == null) throw new ArgumentNullException(nameof(propType));

			return this.partProperties[propType];
		}

		public override Type GetPartType()
		{
			return this.GetType();
		}

		public override string GetXPathValue(string xpath)
		{
			throw new NotImplementedException();
		}

		public override void LoadFrom(object source)
		{
			throw new NotImplementedException();
		}

		public override void PrefetchXPathValue(string xpath)
		{
			throw new NotImplementedException();
		}

		public override object RetrieveAs(Type t)
		{
			if (t.IsAssignableFrom(this.PartData.GetType()))
				return this.PartData;

			return Convert.ChangeType(this.PartData, t);
		}

		public override void SetPartProperty(Type propType, object value)
		{
			this.partProperties[propType] = value;
		}
	}
}
