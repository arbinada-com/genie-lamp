using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;

using GenieLamp.Core.CodeWriters;

namespace GenieLamp.Core.Test
{
	[TestFixture()]
	public class CodeWritersTest
	{
		[Test()]
		public void TestTextWriter()
		{
			CodeWriterText writer = new CodeWriterText(null);
			string line1 = "Line 1";
			string line2 = "Line 2";
			writer.WriteLine(line1);
			writer.WriteLine(line2);
			Assert.AreEqual(String.Format("{0}{1}{2}{1}", line1, Environment.NewLine, line2), writer.ToString());
			
			CodeWriterText writer2 = new CodeWriterText(null);
			writer2.WriteLine(line1);
			writer.WriteFrom(writer2);
			Assert.AreEqual(String.Format("{0}{1}{2}{1}{0}{1}", line1, Environment.NewLine, line2), writer.ToString());
		}

		class Item
		{
			public Item(string name)
			{
				this.Name = name;
			}
			public string Name { get; set; }
		}

		[Test()]
		public void TestTextWriterSeparatedString()
		{
			List<Item> list = new List<Item>();
			list.Add(new Item("Item 1"));
			list.Add(new Item("Item 2"));
			list.Add(new Item("Item 3"));
			string separator = ", ";
			string result = list[0].Name + separator + list[1].Name + separator + list[2].Name;

			CodeWriterText writer = new CodeWriterText(null);
			Assert.AreEqual(result,
				writer.ToSeparatedString(list, separator,
					                        delegate(object item, int count) { return String.Format("{0}", (item as Item).Name); }) );

		}
	}
}

