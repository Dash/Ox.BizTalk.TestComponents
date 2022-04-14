using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.PipelineOM;

namespace Ox.BizTalk.TestComponents
{
	[Serializable]
	public class TestMessageFactory : IBaseMessageFactory
	{
		/// <summary>
		/// Creats a blank <see cref="TestMessagePart"/>
		/// </summary>
		/// <returns>Empty message part</returns>
		public virtual IBaseMessagePart CreateMessagePart()
		{
			return new TestMessagePart();
		}

		/// <summary>
		/// Creates a message with a blank <see cref="TestMessageContext"/>
		/// </summary>
		/// <returns>Blank message with empty context</returns>
		public virtual IBaseMessage CreateMessage()
		{
			var msg = new TestMessage(Guid.NewGuid());
			msg.Context = new TestMessageContext();
			return msg;

		}

		/// <summary>
		/// Creates a new instance of <see cref="TestPropertyBag"/>
		/// </summary>
		/// <returns>Empty property bag</returns>
		public virtual IBasePropertyBag CreatePropertyBag()
		{
			return new TestPropertyBag();
		}

		/// <summary>
		/// Creates a new <see cref="TestMessageContext"/>
		/// </summary>
		/// <returns>Empty context</returns>
		public virtual IBaseMessageContext CreateMessageContext()
		{
			return new TestMessageContext();
		}
	}
}
