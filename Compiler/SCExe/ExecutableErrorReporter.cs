﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.TypeSystem;

namespace Saltarelle.Compiler {
	public class ExecutableErrorReporter : IErrorReporter {
		private readonly TextWriter _writer;

		public ExecutableErrorReporter(TextWriter writer) {
			_writer = writer;
		}

		public DomRegion Region { get; set; }

		public void Message(MessageSeverity severity, int code, string message, params object[] args) {
			_writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}({1},{2}): {3} CS{4:0000}: {5}", Region.FileName, Region.BeginLine, Region.BeginColumn, GetSeverityText(severity), code, string.Format(message, args)));
		}

		public void InternalError(string text) {
			this.Message(7999, text);
		}

		public void InternalError(Exception ex, string additionalText = null) {
			this.Message(7999, (additionalText != null ? additionalText + ": " : "") + ex.ToString());
		}

		private static string GetSeverityText(MessageSeverity severity) {
			return severity == MessageSeverity.Error ? "error" : "warning";
		}
	}
}
