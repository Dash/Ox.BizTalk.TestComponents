using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.BizTalk.TransportProxy.Interop;

namespace Ox.BizTalk.TestComponents
{
	/// <summary>
	/// Implements <see cref="IBTTransportProxy"/>
	/// </summary>
	public class TestTransportProxy : IBTTransportProxy
	{
		public TestTransportProxy()
		{
			this.NewBatch = (callback, callbackCookie) => new TestTransportBatch(callback, callbackCookie);
		}

		protected NewBatchDelegate NewBatch;

		public delegate IBTTransportBatch NewBatchDelegate(IBTBatchCallBack callback, object callbackCookie);

		public TestTransportProxy(NewBatchDelegate newBatchMethod)
		{
			this.NewBatch = newBatchMethod ?? throw new ArgumentNullException(nameof(newBatchMethod));
		}

		public event EventHandler<MethodCalledEventArgs> OnGetBatch;
		public virtual IBTTransportBatch GetBatch(IBTBatchCallBack callback, object callbackCookie)
		{
			OnGetBatch?.Invoke(this, new MethodCalledEventArgs(callback, callbackCookie));

			return this.NewBatch(callback, callbackCookie);
		}

		public event EventHandler<MethodCalledEventArgs> OnGetMessageFactory;

		public virtual IBaseMessageFactory GetMessageFactory()
		{
			OnGetMessageFactory?.Invoke(this, new MethodCalledEventArgs());
			return new TestMessageFactory();
		}

		public event EventHandler<MethodCalledEventArgs> OnRegisterIsolatedReceiver;

		public virtual void RegisterIsolatedReceiver(string url, IBTTransportConfig callback)
		{
			OnRegisterIsolatedReceiver?.Invoke(this, new MethodCalledEventArgs(url, callback));
		}

		public event EventHandler<MethodCalledEventArgs> OnReceiverShuttingdown;

		public virtual void ReceiverShuttingdown(string receiveLocationUrl, Exception exception)
		{
			OnRegisterIsolatedReceiver?.Invoke(this, new MethodCalledEventArgs(receiveLocationUrl, exception));
		}

		public event EventHandler<MethodCalledEventArgs> OnTerminateIsolatedReceiver;

		public virtual void TerminateIsolatedReceiver()
		{
			OnTerminateIsolatedReceiver?.Invoke(this, new MethodCalledEventArgs());
		}

		public event EventHandler<MethodCalledEventArgs> OnSetErrorInfo;

		public virtual void SetErrorInfo(Exception exception)
		{
			OnSetErrorInfo?.Invoke(this, new MethodCalledEventArgs(exception));
		}
	}
}
