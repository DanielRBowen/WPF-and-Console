using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.IO;

namespace SpreadsheetTests
{


	/// <summary>
	///This is a test class for SpreadSheetTest and is intended
	///to contain all SpreadSheetTest Unit Tests
	///</summary>
	[TestClass()]
	public class SpreadsheetTests
	{
		private void AddItems(Spreadsheet target)
		{
			target.SetContentsOfCell("B1", "=A1*2");
			target.SetContentsOfCell("C1", "=B1+A1");
			target.SetContentsOfCell("A1", "3.14");
			target.SetContentsOfCell("D1", "awesome");
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		#region Additional test attributes
		// 
		//You can use the following additional attributes as you write your tests:
		//
		//Use ClassInitialize to run code before running the first test in the class
		//[ClassInitialize()]
		//public static void MyClassInitialize(TestContext testContext)
		//{
		//}
		//
		//Use ClassCleanup to run code after all tests in a class have run
		//[ClassCleanup()]
		//public static void MyClassCleanup()
		//{
		//}
		//
		//Use TestInitialize to run code before running each test
		//[TestInitialize()]
		//public void MyTestInitialize()
		//{
		//}
		//
		//Use TestCleanup to run code after each test has run
		//[TestCleanup()]
		//public void MyTestCleanup()
		//{
		//}
		//
		#endregion

		private void AreEqual(IEnumerable<string> expected, IEnumerable<string> actual)
		{
			var expectedEnumerator = expected.GetEnumerator();
			var actualEnumerator = actual.GetEnumerator();

			while (true)
			{
				if (!expectedEnumerator.MoveNext())
				{
					if (actualEnumerator.MoveNext())
					{
						throw new AssertFailedException("actual has more items than expected.");
					}
					return;
				}

				if (!actualEnumerator.MoveNext())
				{
					throw new AssertFailedException("actual has fewer items than expected.");
				}

				Assert.AreEqual(expectedEnumerator.Current, actualEnumerator.Current);
			}
		}

		/// <summary>
		///A test for SetCellContents
		///</summary>
		[TestMethod()]
		public void SetCellContentsTestText()
		{
			var target = new Spreadsheet_Accessor();

			target.SetCellContents("B1", new Formula("A1*2"));
			target.SetCellContents("C1", new Formula("B1+A1"));

			var actual = target.SetCellContents("A1", "test");

			var expected = new[] { "A1", "B1", "C1" };
			AreEqual(expected, actual);
		}

		/// <summary>
		///A test for SetCellContents
		///</summary>
		[TestMethod()]
		public void SetCellContentsTestNumber()
		{
			var target = new Spreadsheet_Accessor();

			target.SetCellContents("B1", new Formula("A1*2"));
			target.SetCellContents("C1", new Formula("B1+A1"));

			var actual = target.SetCellContents("A1", 3.14);

			var expected = new[] { "A1", "B1", "C1" };
			AreEqual(expected, actual);
		}

		/// <summary>
		///A test for SetCellContents
		///</summary>
		[TestMethod()]
		public void SetCellContentsTestFormula()
		{
			var target = new Spreadsheet_Accessor();

			target.SetCellContents("B1", new Formula("A1*2"));
			target.SetCellContents("C1", new Formula("B1+A1"));

			var actual = target.SetCellContents("A1", new Formula("1+2"));

			var expected = new[] { "A1", "B1", "C1" };
			AreEqual(expected, actual);

			actual = target.SetCellContents("A2", new Formula("C1-B1"));
			expected = new[] { "A2" };
			AreEqual(expected, actual);

			actual = target.SetCellContents("A1", new Formula("4-1"));
			expected = new[] { "A1", "A2", "B1", "C1" };
			AreEqual(expected, actual);

			var circular = false;

			try
			{
				target.SetCellContents("A1", new Formula("C1"));
			}
			catch (CircularException)
			{
				circular = true;
			}

			if (!circular)
			{
				Assert.Fail("expected a CircularException.");
			}
		}

		/// <summary>
		///A test for GetNamesOfAllNonemptyCells
		///</summary>
		[TestMethod()]
		public void GetNamesOfAllNonemptyCellsTest()
		{
			var target = new Spreadsheet_Accessor();

			target.SetCellContents("A1", 3.14);
			target.SetCellContents("B1", new Formula("A1*2"));
			target.SetCellContents("C1", new Formula("B1+A1"));
			target.SetCellContents("B1", string.Empty);

			var actual = target.GetNamesOfAllNonemptyCells();

			var expected = new[] { "A1", "C1" };
			AreEqual(expected, actual);
		}

		/// <summary>
		///A test for GetDirectDependents
		///</summary>
		[TestMethod()]
		[DeploymentItem("Spreadsheet.dll")]
		public void GetDirectDependentsTest()
		{
			var target = new Spreadsheet_Accessor();

			target.SetCellContents("A1", 3);
			target.SetCellContents("B1", new Formula("A1 * A1"));
			target.SetCellContents("C1", new Formula("B1 + A1"));
			target.SetCellContents("D1", new Formula("B1 - C1"));

			var actual = target.GetDirectDependents("A1");

			var expected = new[] { "B1", "C1" };
			AreEqual(expected, actual);
		}

		/// <summary>
		///A test for GetCellContents
		///</summary>
		[TestMethod()]
		public void GetCellContentsTest()
		{
			var target = new Spreadsheet_Accessor();
			var name = "A1";

			var expectedText = "test";
			target.SetCellContents(name, expectedText);
			var actualText = (string)target.GetCellContents(name);
			Assert.AreEqual(expectedText, actualText);

			var expectedNumber = 3.14;
			target.SetCellContents(name, expectedNumber);
			var actualNumber = (double)target.GetCellContents(name);
			Assert.AreEqual(expectedNumber, actualNumber);

			var expectedFormula = new Formula("1+1");
			target.SetCellContents(name, expectedFormula);
			var actualFormula = (Formula)target.GetCellContents(name);
			Assert.AreEqual(expectedFormula, actualFormula);
		}

		private const string TestVersion = "test";
		private static readonly string TestFilePath = Path.GetFullPath("test.xml");

		/// <summary>
		///A test for SpreadSheet Constructor
		///</summary>
		[TestMethod]
		public void SpreadSheetConstructorTest()
		{
			Func<string, bool> isValid = cellName => true;
			Func<string, string> normalize = cellName => cellName;

			AbstractSpreadsheet sheet1 = new Spreadsheet();
			AbstractSpreadsheet sheet2 = new Spreadsheet(isValid, normalize, TestVersion);

			try
			{
				SaveTest();
			}
			catch
			{
				Assert.Inconclusive("unable to generate a test file.");
			}

			AbstractSpreadsheet sheet3 = new Spreadsheet(TestFilePath, isValid, normalize, TestVersion);
		}

		/// <summary>
		///A test for GetSavedVersion
		///</summary>
		[TestMethod()]
		public void GetSavedVersionTest()
		{
			try
			{
				SaveTest();
			}
			catch
			{
				Assert.Inconclusive("unable to generate a test file.");
			}

			Spreadsheet target = new Spreadsheet();
			var actual = target.GetSavedVersion(TestFilePath);
			Assert.AreEqual(TestVersion, actual);
		}

		/// <summary>
		///A test for Save
		///</summary>
		[TestMethod()]
		public void SaveTest()
		{
			Func<string, string> normalize = cellName => cellName;
			Func<string, bool> isValid = cellName => true;
			Spreadsheet target = new Spreadsheet(isValid, normalize, TestVersion);
			AddItems(target);

			target.Save(TestFilePath);
		}

		/// <summary>
		///A test for GetCellValue
		///</summary>
		[TestMethod()]
		public void GetCellValueTest()
		{
			Spreadsheet target = new Spreadsheet();
			AddItems(target);

			object actual;
			actual = target.GetCellValue("C1");
			Assert.AreEqual(9.42, (double)actual);
		}

		/// <summary>
		///A test for SetContentsOfCell
		///</summary>
		[TestMethod()]
		public void SetContentsOfCellTest()
		{
			Spreadsheet target = new Spreadsheet();
			string name = "A1";
			string content = "3.14";

			AddItems(target);

			var expected = new[] { "A1", "B1", "C1" };
			ISet<string> actual;
			actual = target.SetContentsOfCell(name, content);
			AreEqual(expected, actual);
		}
	}
}