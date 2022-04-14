using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Message.Interop;

namespace Ox.BizTalk.TestComponents
{
	[Serializable]
	public class TestMessage : IBaseMessage, IDisposable
	{
		protected Dictionary<string, IBaseMessagePart> parts = new Dictionary<string, IBaseMessagePart>();
		protected string bodyPart;
		protected Exception errorInfo;
		private bool disposedValue;

		public TestMessage()
		{

		}

		public TestMessage(Guid messageId)
		{
			this.MessageID = messageId;
		}

		public virtual void GetSize(out ulong lSize, out bool fImplemented)
		{
			throw new NotImplementedException();
		}

		public virtual IBaseMessagePart GetPartByIndex(int index, out string partName)
		{
			var part = parts.ElementAt(index);
			partName = part.Key;
			return part.Value;
		}

		public virtual void AddPart(string partName, IBaseMessagePart part, bool bBody)
		{
			this.parts.Add(partName, part);
			if (bBody)
				this.bodyPart = partName;
		}

		public virtual IBaseMessagePart GetPart(string partName)
		{
			IBaseMessagePart result;
			this.parts.TryGetValue(partName, out result);
			return result;
		}

		public virtual void RemovePart(string partName)
		{
			this.parts.Remove(partName);
		}

		public virtual Exception GetErrorInfo()
		{
			return this.errorInfo;
		}

		public virtual void SetErrorInfo(Exception errInfo)
		{
			this.errorInfo = errInfo;
		}

		public virtual IBaseMessagePart this[int index]
		{
			get
			{
				return this.GetPartByIndex(index, out _);
			}
		}

		public virtual IBaseMessagePart this[string partName]
		{
			get
			{
				return this.GetPart(partName);
			}
		}

		public virtual Guid MessageID { get; }
		public virtual IBaseMessageContext Context { get; set; }
		public virtual IBaseMessagePart BodyPart => this.GetPart(this.bodyPart);
		public virtual string BodyPartName => this.bodyPart;
		public virtual int PartCount => this.parts.Count;
		public virtual bool IsMutable => true;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if(this.parts != null)
					{
						foreach(var part in parts)
						{
							if (part.Value is IDisposable disposable)
								disposable.Dispose();
						}
					}
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
