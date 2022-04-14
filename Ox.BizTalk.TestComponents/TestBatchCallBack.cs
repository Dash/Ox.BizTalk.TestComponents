using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.TransportProxy.Interop;

namespace Ox.BizTalk.TestComponents
{
	public class TestBatchCallBack : IBTBatchCallBack
	{
		public event EventHandler<MethodCalledEventArgs> OnBatchComplete;

		public virtual void BatchComplete(int status, short opCount, BTBatchOperationStatus[] operationStatus, object callbackCookie)
		{
			OnBatchComplete?.Invoke(this, new MethodCalledEventArgs(status, opCount, operationStatus, callbackCookie));
		}
	}
}
