using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Component.Interop;

namespace Ox.BizTalk.TestComponents
{
	/// <summary>
	/// Implements <see cref="IResourceTracker"/>
	/// </summary>
	public class TestResourceTracker : IResourceTracker
	{
		protected List<object> resources = new List<object>();

		public virtual void AddResource(object obj)
		{
			this.resources.Add(obj);
		}

		public virtual void DisposeAll()
		{
			var resourcesToDispose = this.resources.ToArray();
			this.resources.Clear();

			List<Exception> exceptions = new List<Exception>();

			foreach (var obj in resourcesToDispose)
			{
				try
				{
					if (obj is IDisposable disposable)
						disposable.Dispose();
				}
				catch(Exception ex)
				{
					exceptions.Add(ex);
				}
			}

			if (exceptions.Count > 0)
			{
				throw new AggregateException(exceptions);
			}
		}
	}
}
