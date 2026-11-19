using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.XLANGs.BaseTypes;

namespace Ox.BizTalk.TestComponents
{
	/// <summary>
	/// Aids running common tests on output messages from pipeline components.
	/// </summary>
	/// <remarks>
	/// Use a builder pattern to construct a callback to the needs of your test (see example).
	/// 
	/// The names of the body parts need to be known for any part-specific tests.
	/// </remarks>
	/// <example>
	/// Using a builder pattern, define a callback for testing your pipeline component result:
	/// <code>
	/// var result = myPipeline.Execute(pContext, pInMsg);
	/// 
	/// new PipelineResultValidator()
	///		.AssertPartCount(1)
	///		.AssertContentType("body", "text/xml")
	///		.AssertPartStream("body", sExpectedContent)
	///		.AssertUnexpectedParts()
	///		.Validate(result);
	/// </code>
	/// </example>
	public class PipelineResultValidator
	{
		protected string expectedContentType;

		/// <summary>
		/// Performs validation based on the previously configured options (not a lot will happen if called without configuration)
		/// </summary>
		/// <param name="message">Message to validate</param>
		public virtual void Validate(IBaseMessage message)
		{
			Assert.IsNotNull(message, "Result message is null.");

			this.ValidatePartCount(message);
			this.ValidatePartContentType(message);
			this.ValidateExtraParts(message);
			this.ValidateMissingParts(message);
			this.ValidatePartOrder(message);
			this.ValidateProperties(message);
			this.ValidatePartStreams(message);
			this.ValidatePartProperties(message);
		}

		#region Validator runners

		protected virtual void ValidateExtraParts(IBaseMessage message)
		{
			if (this.checkExtraParts)
			{
				// Get list of expected part names from other areas
				var parts = this.partStreamValidators.Keys
								.Union(this.partPropertyValidators.Keys)
								.Union(this.partContentTypeValidators.Keys)
								.Union(this.partOrder);

				this.LoopParts(message, (_, name) =>
				{
					if (!parts.Contains(name, StringComparer.OrdinalIgnoreCase))
					{
						Assert.Fail($"Unexpected part {name} found.");
					}
				});
			}
		}

		protected virtual void ValidateMissingParts(IBaseMessage message)
		{
			if (this.checkExtraParts)
			{
				// Get list of expected part names from other areas
				var parts = this.partStreamValidators.Keys
								.Union(this.partPropertyValidators.Keys)
								.Union(this.partContentTypeValidators.Keys)
								.Union(this.partOrder);

				foreach (var part in parts)
				{
					if (message.GetPart(part) == null)
					{
						Assert.Fail($"Expected part {part} not found.");
					}
				}
			}
		}

		protected void ValidatePartCount(IBaseMessage message)
		{
			if (this.partCount.HasValue)
				Assert.AreEqual(this.partCount.Value, message.PartCount, "Part count mismatch.");
		}

		protected void ValidatePartContentType(IBaseMessage message)
		{
			if (!this.partContentTypeValidators.Any()) return;

			this.LoopParts(message, (part, name) =>
			{
				// Check there is a validator for this part
				if (this.partContentTypeValidators.ContainsKey(name))
				{
					Assert.AreEqual(
						this.partContentTypeValidators[name].MediaType, 
						new System.Net.Mime.ContentType(part.ContentType).MediaType);
				}
			});
		}

		protected void ValidateProperties(IBaseMessage message)
		{
			foreach (var prop in this.propertyValidators)
			{
				object data = message.Context.Read(prop.Key.name, prop.Key.ns);
				prop.Value(data);
			}
		}
		protected void ValidatePartStreams(IBaseMessage message)
		{
			if (!this.partStreamValidators.Any())
				return;

			this.LoopParts(message, (part, partName) =>
			{
				if (partName != null && this.partStreamValidators.ContainsKey(partName))
				{
					this.partStreamValidators[partName].Invoke(part.Data);
				}
			});
		}

		protected void ValidatePartProperties(IBaseMessage message)
		{
			if (!this.partPropertyValidators.Any()) return;

			this.LoopParts(message, (part, name) =>
			{
				// Get validators for this part
				if (this.partPropertyValidators.ContainsKey(name))
				{
					foreach (var validator in this.partPropertyValidators[name])
					{
						var obj = part.PartProperties.Read(validator.Key.name, validator.Key.ns);
						validator.Value(obj);
					}
				}
			});
		}

		protected void ValidatePartOrder(IBaseMessage message)
		{
			if (this.partOrder.Length == 0)
				return;

			for (int i = 0; i < message.PartCount; i++)
			{
				message.GetPartByIndex(i, out string partName);
				Assert.AreEqual(this.partOrder[i].ToLower(), partName.ToLower(), $"Unexpected part name at index {i}.");
			}
		}

		#endregion

		/// <summary>
		/// Apply an action against each message part of a message
		/// </summary>
		/// <param name="message">Parent message of parts</param>
		/// <param name="callback">Callback to run, takes the message part and the part name as arguments</param>
		protected void LoopParts(IBaseMessage message, Action<IBaseMessagePart, string> callback)
		{
			for (int i = 0; i < message.PartCount; i++)
			{
				var part = message.GetPartByIndex(i, out string partName);
				callback(part, partName?.ToLower());
			}
		}

		#region Validators

		protected Dictionary<(string name, string ns), Action<object>> propertyValidators = new Dictionary<(string name, string ns), Action<object>>();
		protected Dictionary<string, Action<Stream>> partStreamValidators = new Dictionary<string, Action<Stream>>();
		/// <summary>
		/// Keyed on partname, subdictionaries have part qualifier as key and callback finally as value item.
		/// </summary>
		protected Dictionary<string, Dictionary<(string name, string ns), Action<object>>> partPropertyValidators = new Dictionary<string, Dictionary<(string name, string ns), Action<object>>>();
		/// <summary>
		/// Consumed by <see cref="ValidatePartContentType(IBaseMessage)"/>. Keyed by partName, the value the expected
		/// content-type (simple text comparison)
		/// </summary>
		protected Dictionary<string, System.Net.Mime.ContentType> partContentTypeValidators = new Dictionary<string, System.Net.Mime.ContentType>();
		protected string[] partOrder = new string[0];
		protected int? partCount = null;
		protected bool checkExtraParts = false;

		#endregion

		#region Builder methods

		/// <summary>
		/// Validates a context property matches
		/// </summary>
		/// <param name="name">Property name</param>
		/// <param name="ns">Property namespace</param>
		/// <param name="value">Expected value</param>
		public PipelineResultValidator AssertProperty(string name, string ns, object value)
		{
			this.AssertProperty(name, ns, (obj) =>
			{
				Assert.AreEqual(value, obj);
			});

			return this;
		}

		/// <summary>
		/// Validates a context property meets custom criteria
		/// </summary>
		/// <param name="name">Property name</param>
		/// <param name="ns">Property namespace</param>
		/// <param name="validator">Delegate to be called for validation, takes value of property as argument.</param>
		public PipelineResultValidator AssertProperty(string name, string ns, Action<object> validator)
		{
			if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
			if (String.IsNullOrEmpty(ns)) throw new ArgumentNullException(nameof(ns));

			if (this.propertyValidators.ContainsKey((name, ns))) throw new InvalidOperationException("Only one property callback can exist per property.");

			this.propertyValidators.Add((name, ns), validator);
			return this;
		}

		/// <summary>
		/// Validates that the message part count is as expected
		/// </summary>
		/// <param name="count">Expected part count</param>
		/// <exception cref="ArgumentOutOfRangeException">Negative part count</exception>
		public PipelineResultValidator AssertPartCount(int count)
		{
			if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

			this.partCount = count;
			return this;
		}

		/// <summary>
		/// Passes the named message part to the supplied callback for custom validation
		/// </summary>
		/// <param name="partName">Name of part to associate validator to</param>
		/// <param name="callback">Callback to validator</param>
		/// <exception cref="ArgumentNullException">Either property being null</exception>
		/// <exception cref="InvalidOperationException">Attempt to add another validator for the part name</exception>
		public PipelineResultValidator AssertPartStream(string partName, Action<Stream> callback)
		{
			partName = partName?.ToLower() ?? throw new ArgumentNullException(nameof(partName));
			if (callback == null) throw new ArgumentNullException(nameof(callback));

			if (this.partStreamValidators.ContainsKey(partName))
				throw new InvalidOperationException("Only one stream callback can exist per named part.");

			this.partStreamValidators.Add(partName, callback);
			return this;
		}

		/// <summary>
		/// Validates a part data stream matches that of the supplied string
		/// </summary>
		/// <param name="partName">Name of part to associate validator to</param>
		/// <param name="content">Expected string content of the part</param>
		/// <exception cref="ArgumentNullException">Either property being null</exception>
		/// <exception cref="InvalidOperationException">Attempt to add another validator for the part name</exception>
		public PipelineResultValidator AssertPartStream(string partName, string content)
		{
			return this.AssertPartStream(partName, (data) =>
			{
				long pos = data.Position;
				Assert.AreEqual(content, new StreamReader(data).ReadToEnd(), "Stream content does not match.");
				data.Position = pos;
			});
		}

		/// <summary>
		/// Passes a part property to a custom validator via the callback
		/// </summary>
		/// <param name="partName">Name of part</param>
		/// <param name="name">Name of the property to interrogate</param>
		/// <param name="ns">Namespace of the property to interrogate</param>
		/// <param name="callback">Custom validator callback, accepts the object value of the property</param>
		/// <exception cref="ArgumentNullException">Part name, property name or namespace null</exception>
		/// <exception cref="InvalidOperationException">Attempt to add multiple validators to a part property</exception>
		public PipelineResultValidator AssertPartProperty(string partName, string name, string ns, Action<object> callback)
		{
			if (String.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
			if (String.IsNullOrEmpty(ns)) throw new ArgumentNullException(nameof(ns));

			partName = partName?.ToLower() ?? throw new ArgumentNullException(nameof(partName));

			// If this is the first validator for a part, create the dictionary for the part
			if(!this.partPropertyValidators.ContainsKey(partName))
				this.partPropertyValidators[partName] = new Dictionary<(string, string), Action<object>>();

			// Disallow multiple validators for the same property on the same part
			if (this.partPropertyValidators[partName]?.ContainsKey((name, ns)) ?? false)
				throw new InvalidOperationException("Only one property callback allowed per property, per part.");

			// Add validator
			this.partPropertyValidators[partName].Add((name, ns), callback);

			return this;
		}

		/// <summary>
		/// Validates a part's content type
		/// </summary>
		/// <param name="partName">Part name</param>
		/// <param name="contentType">Expected content/mime type</param>
		/// <exception cref="InvalidOperationException">Attempt to add multiple content types to a part</exception>
		public PipelineResultValidator AssertPartContentType(string partName, string contentType)
		{
			return this.AssertPartContentType(partName, new System.Net.Mime.ContentType(contentType));
		}

		/// <summary>
		/// Validates a part's content type
		/// </summary>
		/// <param name="partName">Part name</param>
		/// <param name="contentType">Expected content/mime type</param>
		/// <exception cref="InvalidOperationException">Attempt to add multiple content types to a part</exception>
		public PipelineResultValidator AssertPartContentType(string partName, System.Net.Mime.ContentType contentType)
		{
			partName = partName?.ToLower() ?? throw new ArgumentNullException(nameof(partName));

			if (this.partContentTypeValidators?.ContainsKey(partName) ?? false)
				throw new InvalidOperationException("Only one content type can be applied to a property part.");

			this.partContentTypeValidators.Add(partName, contentType);

			return this;
		}

		/// <summary>
		/// Must be used after <see cref="AssertPartProperty"/>, <see cref="AssertPartStream"/>, or <see cref="AssertPartContentType(string, string)"/>
		/// and will assert there are no extra parts.
		/// </summary>
		/// <returns></returns>
		public PipelineResultValidator AssertUnexpectedParts()
		{
			this.checkExtraParts = true;
			return this;
		}

		/// <summary>
		/// Validates the order of the parts is as defined
		/// </summary>
		/// <param name="partNames">Array of part names, in the expected order</param>
		/// <exception cref="ArgumentException">No part names supplied</exception>
		/// <exception cref="InvalidOperationException">Attempt to call this method more than once</exception>
		public PipelineResultValidator AssertPartOrder(params string[] partNames)
		{
			if (partNames.Length < 1)
				throw new ArgumentException("Part names must be specified when checking order.");

			if (this.partOrder.Length > 0)
				throw new InvalidOperationException("Part order can only be defined once.");

			this.partOrder = partNames.Select(x => x?.ToLower()).ToArray();

			return this;
		}

		#endregion

	}
}
