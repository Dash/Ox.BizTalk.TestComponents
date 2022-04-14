using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.XLANGs.BaseTypes;

namespace Ox.BizTalk.TestComponents
{
	/// <summary>
	/// Implements a <see cref="XLANGMessage"/> for the use of unit testing outside of the BizTalk runtime.
	/// </summary>
	public class TestXLANGMessage : XLANGMessage
	{
		protected IDictionary<string, XLANGPart> parts = new Dictionary<string, XLANGPart>();
		protected Dictionary<Type, object> messageProperties = new Dictionary<Type, object>();

		public override XLANGPart this[string partName] { 
			get
			{
				return this.parts[partName];
			}
		}
		public override XLANGPart this[int partIndex] { 
			get
			{
				return this.parts.Values.ElementAt(partIndex);
			}
		}

		public override string Name { get; }
		public override int Count
		{
			get
			{
				return this.parts.Count;
			}
		}

		public TestXLANGMessage(string name) : this(name, parts: null) { }

		public TestXLANGMessage(string name, IEnumerable<XLANGPart> partsArray) : this(name, partsArray.ToDictionary(x => x.Name)) { }

		public TestXLANGMessage(string name, IDictionary<string, XLANGPart> parts)
		{
			this.Name = name;
			if (parts != null)
			{
				this.parts = parts;
			}
		}

		public override void AddPart(XLANGPart part)
		{
			this.parts.Add(part.Name, part);
		}

		public override void AddPart(XLANGPart part, string partName)
		{
			this.parts.Add(partName, part);
		}

		public override void AddPart(object part, string partName)
		{
			this.parts.Add(partName, (XLANGPart)part);
		}

		public override void Dispose()
		{
			
		}

		public override IEnumerator GetEnumerator()
		{
			return this.parts.GetEnumerator();
		}

		public override object GetPropertyValue(Type propType)
		{
			if(this.messageProperties.ContainsKey(propType))
			{
				return this.messageProperties[propType];
			}

			return null;
		}

		public override void SetPropertyValue(Type propType, object value)
		{
			this.messageProperties[propType] = value;
		}
	}
}
