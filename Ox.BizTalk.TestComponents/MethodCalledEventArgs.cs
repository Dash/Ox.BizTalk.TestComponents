using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ox.BizTalk.TestComponents
{
	public class MethodCalledEventArgs : EventArgs
	{
		public object[] Args { get; }

		public MethodCalledEventArgs(params object[] args)
		{
			this.Args = args;
		}
	}
}
