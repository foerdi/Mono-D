﻿using D_Parser.Dom;
using D_Parser.Misc;
using D_Parser.Resolver;
using Mono.Debugging.Backend;
using Mono.Debugging.Client;
using System.Collections.Generic;

namespace MonoDevelop.D.Debugging
{
	public partial class DLocalExamBacktrace : IObjectValueSource
	{
		#region Properties
		public readonly IDBacktraceHelpers BacktraceHelper;
		public const long MaximumDisplayCount = 1000;
		public const long MaximumArrayLengthThreshold = 100000;

		bool needsStackFrameInfoUpdate = true;
		public string currentStackFrameSource {get;private set;}
		public ulong currentInstruction {get;private set;}
		public CodeLocation currentSourceLocation{ get; private set;}
		ResolutionContext ctxt;
		DebugSymbolValueProvider SymbolProvider;
		#endregion


		public DLocalExamBacktrace(IDBacktraceHelpers helper)
		{
			this.BacktraceHelper = helper;
		}

		/// <summary>
		/// Must be invoked after the program execution was interrupted.
		/// Resets internal value caches so no old values will be shown.
		/// </summary>
		public void Reset ()
		{
			needsStackFrameInfoUpdate = true;
		}

		void TryUpdateStackFrameInfo()
		{
			if (needsStackFrameInfoUpdate) {
				string file;
				ulong eip;
				CodeLocation loc;

				BacktraceHelper.GetCurrentStackFrameInfo (out file, out eip, out loc);
				currentStackFrameSource = file;
				currentInstruction = eip;
				currentSourceLocation = loc;

				needsStackFrameInfoUpdate = !string.IsNullOrWhiteSpace(file) && !loc.IsEmpty;

				var mod = GlobalParseCache.GetModule(file);
				if (ctxt == null)
				{
					var doc = Ide.IdeApp.Workbench.GetDocument(file);

					if (doc != null)
						ctxt = ResolutionContext.Create(MonoDevelop.D.Resolver.DResolverWrapper.CreateEditorData(doc), false);
					
					SymbolProvider = new DebugSymbolValueProvider(this, ctxt);

					if (doc == null)
						return;
				}

				if (SymbolProvider.ResolutionContext == null)
					SymbolProvider = new DebugSymbolValueProvider(this, ctxt);

				SymbolProvider.ResetCache();

				if (mod == null) {
					if (ctxt.CurrentContext != null)
						ctxt.CurrentContext.Set(loc);
				} else {
					var bn = D_Parser.Resolver.TypeResolution.DResolver.SearchBlockAt (mod, loc);
					if (ctxt.CurrentContext == null)
						ctxt.Push(bn, loc);
					else
						ctxt.CurrentContext.Set(bn, loc);
				}
			}
		}

		public ObjectValue[] GetParameters(EvaluationOptions evalOptions)
		{
			var l = new List<ObjectValue>();
			foreach (var p in BacktraceHelper.Parameters)
			{
				l.Add(CreateObjectValue(p, evalOptions));
			}
			return l.ToArray();
		}

		public ObjectValue[] GetLocals(EvaluationOptions evalOptions)
		{
			var l = new List<ObjectValue>();
			foreach (var p in BacktraceHelper.Locals)
			{
				l.Add(CreateObjectValue(p, evalOptions));
			}
			return l.ToArray();
		}

		public ObjectValue[] GetChildren(ObjectPath path, int index, int count, EvaluationOptions options)
		{
			return null;
		}

		public object GetRawValue(ObjectPath path, EvaluationOptions options)
		{
			return null;
		}

		public ObjectValue GetValue(ObjectPath path, EvaluationOptions options)
		{
			return null;
		}

		public void SetRawValue(ObjectPath path, object value, EvaluationOptions options)
		{
			
		}

		public EvaluationResult SetValue(ObjectPath path, string value, EvaluationOptions options)
		{
			return null;
		}
	}
}
