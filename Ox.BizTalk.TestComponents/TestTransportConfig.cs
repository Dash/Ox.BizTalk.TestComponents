using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.TransportProxy.Interop;

namespace Ox.BizTalk.TestComponents
{
	public class TestTransportConfig : IBTTransportConfig
	{
		protected Dictionary<string, (IPropertyBag adapterConfig, IPropertyBag biztalkConfig)> endpoints = new Dictionary<string, (IPropertyBag adapterConfig, IPropertyBag biztalkConfig)>();

		public virtual void AddReceiveEndpoint(string url, IPropertyBag adapterConfig, IPropertyBag bizTalkConfig)
		{
			this.endpoints.Add(url, (adapterConfig, bizTalkConfig));
		}

		public virtual void UpdateEndpointConfig(string url, IPropertyBag adapterConfig, IPropertyBag bizTalkConfig)
		{
			this.endpoints[url] = (adapterConfig, bizTalkConfig);
		}

		public virtual void RemoveReceiveEndpoint(string url)
		{
			this.endpoints.Remove(url);
		}
	}
}
