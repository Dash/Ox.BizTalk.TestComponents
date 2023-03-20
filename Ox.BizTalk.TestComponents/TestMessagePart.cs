using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Message.Interop;

namespace Ox.BizTalk.TestComponents
{
	/// <summary>
	/// Implements <see cref="IBaseMessagePart"/>
	/// </summary>
	[Serializable]
	public class TestMessagePart : IBaseMessagePart, IDisposable
	{
		private bool disposedValue;

		public TestMessagePart()
		{
			this.PartID = Guid.NewGuid();
		}

		public virtual void GetSize(out ulong lSize, out bool fImplemented)
		{
			lSize = (ulong)this.Data.Length;
			fImplemented = false;
		}

		public virtual Stream GetOriginalDataStream()
		{
			return this.Data;
		}

		public virtual Guid PartID { get; }
		public virtual IBasePropertyBag PartProperties { get; set; } = new TestPropertyBag();
		public virtual string ContentType { get; set; }
		public virtual string Charset { get; set; }
		public virtual Stream Data { get; set; }
		public virtual bool IsMutable => true;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					this.Data.Dispose();
				}

				disposedValue = true;
			}
		}

		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
