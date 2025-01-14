﻿using System;
using NUnit.Framework;
#if PCL
using Portable.Xaml.Markup;
using Portable.Xaml.ComponentModel;
using Portable.Xaml;
using Portable.Xaml.Schema;
#else
using System.Windows.Markup;
using System.ComponentModel;
using System.Xaml;
using System.Xaml.Schema;
#endif

namespace MonoTests.Portable.Xaml
{
	[TestFixture]
	public class XamlNodeListTest
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ConstructorNull()
		{
			new XamlNodeList(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void NegativeSize()
		{
			new XamlNodeList(new XamlSchemaContext(), -100);
		}

		[Test]
		public void ReadWriteListShouldRoundtrip()
		{
			var sc = new XamlSchemaContext();
			var list = new XamlNodeList(sc);

			var reader = new XamlObjectReader(new TestClass4 { Foo = "foo", Bar = "bar" }, sc);
			XamlServices.Transform(reader, list.Writer);

			var writer = new XamlObjectWriter(sc);
			var listReader = list.GetReader();
			XamlServices.Transform(listReader, writer);

			Assert.IsNotNull(writer.Result, "#1");
			Assert.IsInstanceOf<TestClass4>(writer.Result, "#2");

			Assert.AreEqual("foo", ((TestClass4)writer.Result).Foo, "#3");
			Assert.AreEqual("bar", ((TestClass4)writer.Result).Bar, "#4");

			// try reading a 2nd time, we should not get the same reader
			writer = new XamlObjectWriter(sc);
			var listReader2 = list.GetReader();
			Assert.AreNotSame(listReader, listReader2, "#5");
			XamlServices.Transform(listReader2, writer);

			Assert.IsNotNull(writer.Result, "#6");
			Assert.IsInstanceOf<TestClass4>(writer.Result, "#7");

			Assert.AreEqual("foo", ((TestClass4)writer.Result).Foo, "#8");
			Assert.AreEqual("bar", ((TestClass4)writer.Result).Bar, "#9");
		}

		[Test]
		[ExpectedException(typeof(XamlException))]
		public void WriterShouldThrowExceptionIfNotClosed()
		{
			var sc = new XamlSchemaContext();
			var list = new XamlNodeList(sc);
			list.Writer.WriteStartObject(sc.GetXamlType(typeof(TestClass4)));
			list.Writer.WriteEndObject();
			list.GetReader();
		}

		[Test]
		public void WriterShouldNotThrowExceptionIfClosed()
		{
			var sc = new XamlSchemaContext();
			var list = new XamlNodeList(sc);
			list.Writer.WriteStartObject(sc.GetXamlType(typeof(TestClass4)));
			list.Writer.WriteEndObject();
			list.Writer.Close();
			var reader = list.GetReader();
		}
	}
}

