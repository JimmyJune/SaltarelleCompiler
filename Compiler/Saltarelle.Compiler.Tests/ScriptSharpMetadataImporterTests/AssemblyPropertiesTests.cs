﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Saltarelle.Compiler.Tests.ScriptSharpMetadataImporterTests {
	[TestFixture]
	public class AssemblyPropertiesTests : ScriptSharpMetadataImporterTestBase {
		[Test]
		public void OmitNullableChecksWorks() {
			Prepare("using System.Runtime.CompilerServices; class C {}");
			Assert.That(Metadata.OmitNullableChecks, Is.False);
			Prepare("using System.Runtime.CompilerServices; [assembly: ScriptSharpCompatibility] class C {}");
			Assert.That(Metadata.OmitNullableChecks, Is.False);
			Prepare("using System.Runtime.CompilerServices; [assembly: ScriptSharpCompatibility(OmitNullableChecks = false)] class C {}");
			Assert.That(Metadata.OmitNullableChecks, Is.False);
			Prepare("using System.Runtime.CompilerServices; [assembly: ScriptSharpCompatibility(OmitNullableChecks = true)] class C {}");
			Assert.That(Metadata.OmitNullableChecks, Is.True);
		}

		[Test]
		public void OmitDowncastsWorks() {
			Prepare("using System.Runtime.CompilerServices; class C {}");
			Assert.That(Metadata.OmitDowncasts, Is.False);
			Prepare("using System.Runtime.CompilerServices; [assembly: ScriptSharpCompatibility] class C {}");
			Assert.That(Metadata.OmitDowncasts, Is.False);
			Prepare("using System.Runtime.CompilerServices; [assembly: ScriptSharpCompatibility(OmitDowncasts = false)] class C {}");
			Assert.That(Metadata.OmitDowncasts, Is.False);
			Prepare("using System.Runtime.CompilerServices; [assembly: ScriptSharpCompatibility(OmitDowncasts = true)] class C {}");
			Assert.That(Metadata.OmitDowncasts, Is.True);
		}

		[Test]
		public void MainModuleNameWorks() {
			Prepare("using System.Runtime.CompilerServices; class C {}");
			Assert.That(Metadata.MainModuleName, Is.Null);
			Prepare("using System.Runtime.CompilerServices; [assembly: ModuleName(\"my-module\")] class C {}");
			Assert.That(Metadata.MainModuleName, Is.EqualTo("my-module"));
			Prepare("using System.Runtime.CompilerServices; [assembly: ModuleName(null)] class C {}");
			Assert.That(Metadata.MainModuleName, Is.Null);
			Prepare("using System.Runtime.CompilerServices; [assembly: ModuleName(\"\")] class C {}");
			Assert.That(Metadata.MainModuleName, Is.Null);
		}

		[Test]
		public void AsyncModuleWorks() {
			Prepare("using System.Runtime.CompilerServices; class C {}");
			Assert.That(Metadata.IsAsyncModule, Is.False);
			Prepare("using System.Runtime.CompilerServices; [assembly: AsyncModule] class C {}");
			Assert.That(Metadata.IsAsyncModule, Is.True);
		}
	}
}
