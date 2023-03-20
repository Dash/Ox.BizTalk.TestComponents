using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ox.BizTalk.TestComponents
{
	/// <summary>
	/// Implements <see cref="IPipelineContext"/> for use with Pipeline testing
	/// </summary>
	public class TestPipelineContext : IPipelineContext
	{
		protected IBaseMessageFactory _messageFactory = new TestMessageFactory();

		/// <summary>
		/// Value for <see cref="GetGroupSigningCertificate"/>
		/// </summary>
		public string GroupSigningCertificate { get; set; }

		/// <summary>
		/// Value for <see cref="GetEventStream"/>
		/// </summary>
		public Microsoft.BizTalk.Bam.EventObservation.EventStream BamEventStream { get; set; }

		/// <summary>
		/// Assign a method to this to resolve doc specs by .NET type
		/// </summary>
		public Func<string, IDocumentSpec> GetDocumentSpecByTypeResolver;

		/// <summary>
		/// Assign a method to this to resolve docs by message type
		/// </summary>
		public Func<string, IDocumentSpec> GetDocumentSpecByNameResolver;

		public IBaseMessageFactory GetMessageFactory()
		{
			return this._messageFactory;
		}

		public IDocumentSpec GetDocumentSpecByType(string DocType)
		{
			return this.GetDocumentSpecByTypeResolver?.Invoke(DocType);
		}

		public IDocumentSpec GetDocumentSpecByName(string DocSpecName)
		{
			return this.GetDocumentSpecByNameResolver?.Invoke(DocSpecName);
		}

		public string GetGroupSigningCertificate()
		{
			return this.GroupSigningCertificate;
		}

		public Microsoft.BizTalk.Bam.EventObservation.EventStream GetEventStream()
		{
			return this.BamEventStream;
		}

		public Guid PipelineID { get; set; } = Guid.NewGuid();
		public string PipelineName { get; set; } = Guid.NewGuid().ToString();
		public Guid StageID { get; set; } = Guid.NewGuid();
		public int StageIndex { get; set; } = 0;
		public int ComponentIndex { get; set; } = 0;
		public IResourceTracker ResourceTracker { get; set; } = new TestResourceTracker();
	}
}
