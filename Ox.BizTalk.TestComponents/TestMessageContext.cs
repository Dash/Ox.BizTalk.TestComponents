using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Message.Interop;

namespace Ox.BizTalk.TestComponents
{
	/// <summary>
	/// Implements <see cref="IBaseMessageContext"/>
	/// </summary>
	[Serializable]
	public class TestMessageContext : IBaseMessageContext, ICloneable
	{
		protected Dictionary<(string name, string ns), (object val, ContextPropertyType type)> properties = new Dictionary<(string name, string ns), (object val, ContextPropertyType type)>();

		public virtual object ReadAt(int index, out string strName, out string strNamespace)
		{
			var prop = this.properties.ElementAt(index);
			(strName, strNamespace) = prop.Key;
			return prop.Value;
		}

		public virtual object Read(string strName, string strNamespace)
		{
			(object val, ContextPropertyType type) prop;
			this.properties.TryGetValue((strName, strNamespace), out prop);
			return prop.val;
		}

		public virtual void Write(string strName, string strNameSpace, object obj)
		{
			this.properties[(strName, strNameSpace)] = (obj, ContextPropertyType.PropWritten);
		}

		public virtual void Promote(string strName, string strNameSpace, object obj)
		{
			this.properties[(strName, strNameSpace)] = (obj, ContextPropertyType.PropPromoted);
		}

		public virtual void AddPredicate(string strName, string strNameSpace, object obj)
		{
			this.properties[(strName, strNameSpace)] = (obj, ContextPropertyType.PropPredicate);
		}

		public virtual bool IsPromoted(string strName, string strNameSpace)
		{
			return this.properties[(strName, strNameSpace)].type == ContextPropertyType.PropPromoted;
		}

		public virtual ContextPropertyType GetPropertyType(string strName, string strNameSpace)
		{
			return this.properties[(strName, strNameSpace)].type;
		}

		public object Clone()
		{
			var clone = new TestMessageContext();
			
			foreach(var prop in this.properties)
			{
				clone.properties.Add(prop.Key, prop.Value);
			}

			return clone;
		}

		public virtual uint CountProperties => (uint)this.properties.Count;
	}
}
