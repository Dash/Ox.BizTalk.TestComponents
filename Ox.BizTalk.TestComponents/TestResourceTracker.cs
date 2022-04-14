using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Component.Interop;

namespace Ox.BizTalk.TestComponents
{
	public class TestResourceTracker : IResourceTracker
	{
		protected List<object> resources = new List<object>();

		public virtual void AddResource(object obj)
		{
			this.resources.Add(obj);
		}

		public virtual void DisposeAll()
		{
			foreach (var obj in resources)
			{
				resources.Remove(obj);

				if (obj is IDisposable disposable)
					disposable.Dispose();
			}
		}
	}
}
