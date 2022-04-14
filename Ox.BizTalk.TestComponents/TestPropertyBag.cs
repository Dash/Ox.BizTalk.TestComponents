using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;

namespace Ox.BizTalk.TestComponents
{
	[Serializable]
    public class TestPropertyBag : IBasePropertyBag, IPropertyBag
    {
		protected Dictionary<(string name, string ns), object> properties = new Dictionary<(string name, string ns), object>();

		public virtual object ReadAt(int index, out string strName, out string strNamespace)
		{
			var prop = this.properties.ElementAt(index);
			(strName, strNamespace) = prop.Key;
			return prop.Value;
		}

		public virtual object Read(string strName, string strNamespace)
		{
			object result;
			this.properties.TryGetValue((strName, strNamespace), out result);
			return result;
		}

		public virtual void Write(string strName, string strNameSpace, object obj)
		{
			this.properties[(strName, strNameSpace)] = obj;
		}

		public virtual uint CountProperties => (uint) this.properties.Count;

		public virtual void Read(string propName, out object ptrVar, int errorLog)
		{
			this.properties.TryGetValue((propName, null), out ptrVar);
		}

		public virtual void Write(string propName, ref object ptrVar)
		{
			this.properties[(propName, null)] = ptrVar;
		}
	}
}
