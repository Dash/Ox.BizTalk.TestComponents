using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Ox.BizTalk.TestComponents
{
	/// <summary>
	/// Extensions for <see cref="TestMessage"/>
	/// </summary>
	public static class TestMessageExtensions
	{
		/// <summary>
		/// Helper method to add common parts to a test message
		/// </summary>
		/// <param name="message">Message to add to</param>
		/// <param name="name">Part name</param>
		/// <param name="contentType">Part content-type (MIME)</param>
		/// <param name="data">Part data stream</param>
		/// <param name="body">Whether the body part or not</param>
		/// <returns>Updated message</returns>
		public static IBaseMessage AddPart(this IBaseMessage message, string name, string contentType, Stream data, bool body = false)
		{
			message.AddPart(name, new TestMessagePart()
			{
				ContentType = contentType,
				Data = data,
				PartProperties = new TestPropertyBag()
			}, body);

			return message;
		}
	}
}
