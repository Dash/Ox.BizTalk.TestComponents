using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.TransportProxy.Interop;

namespace Ox.BizTalk.TestComponents
{
	public class TestTransportBatch : IBTTransportBatch
	{
		public List<IBaseMessage> submittedMessages = new List<IBaseMessage>();
		public List<IBaseMessage> suspendedMessges = new List<IBaseMessage>();

		public IBTBatchCallBack Callback;
		public object CallbackCookie;

		public TestTransportBatch()
		{ }

		// I'm not sure of the behaviour with TransportProxy, these arguments are passed into GetBatch, but not sure where they're usually called from
		public TestTransportBatch(IBTBatchCallBack callback, object callbackCookie)
		{
			this.Callback = callback;
			this.CallbackCookie = callbackCookie;
		}

		public event EventHandler<MethodCalledEventArgs> OnSubmitMessage;

		public virtual void SubmitMessage(IBaseMessage msg)
		{
			this.submittedMessages.Add(msg);
			OnSubmitMessage?.Invoke(this, new MethodCalledEventArgs(msg));
		}

		public event EventHandler<MethodCalledEventArgs> OnClear;

		public virtual void Clear()
		{
			this.submittedMessages.Clear();
			OnClear?.Invoke(this, new MethodCalledEventArgs());
		}

		public event EventHandler<MethodCalledEventArgs> OnDone;

		public virtual IBTDTCCommitConfirm Done(object transaction)
		{
			OnDone?.Invoke(this, new MethodCalledEventArgs(transaction));
			return null;
		}

		public event EventHandler<MethodCalledEventArgs> OnDeleteMessage;

		public virtual void DeleteMessage(IBaseMessage msg)
		{
			this.submittedMessages.Remove(msg);
			OnDeleteMessage?.Invoke(this, new MethodCalledEventArgs(msg));
		}

		public event EventHandler<MethodCalledEventArgs> OnMoveToSuspendQ;

		public virtual void MoveToSuspendQ(IBaseMessage msg)
		{
			this.suspendedMessges.Add(msg);
			OnMoveToSuspendQ?.Invoke(this, new MethodCalledEventArgs(msg));
		}

		public event EventHandler<MethodCalledEventArgs> OnResubmit;

		public virtual void Resubmit(IBaseMessage msg, DateTime timestamp)
		{
			OnResubmit?.Invoke(this, new MethodCalledEventArgs(msg, timestamp));
		}

		public event EventHandler<MethodCalledEventArgs> OnMoveToNextTransport;

		public virtual void MoveToNextTransport(IBaseMessage msg)
		{
			OnMoveToNextTransport?.Invoke(this, new MethodCalledEventArgs(msg));
		}

		public event EventHandler<MethodCalledEventArgs> OnSubmitRequestMessage;

		public virtual void SubmitRequestMessage(IBaseMessage requestMsg, string correlationToken, bool firstResponseOnly, DateTime expirationTime, IBTTransmitter responseCallback)
		{
			OnSubmitRequestMessage?.Invoke(this, new MethodCalledEventArgs(requestMsg, correlationToken, firstResponseOnly, expirationTime, responseCallback));
		}

		public event EventHandler<MethodCalledEventArgs> OnCancelResponseMessage;

		public virtual void CancelResponseMessage(string correlationToken)
		{
			OnCancelResponseMessage?.Invoke(this, new MethodCalledEventArgs(correlationToken));
		}

		public event EventHandler<MethodCalledEventArgs> OnSubmitResponseMessage;

		public virtual void SubmitResponseMessage(IBaseMessage solicitMsgSent, IBaseMessage responseMsgToSubmit)
		{
			OnSubmitResponseMessage?.Invoke(this, new MethodCalledEventArgs(solicitMsgSent, responseMsgToSubmit));
		}

		public IResourceTracker ResourceTracker => new TestResourceTracker();

		
	}
}
