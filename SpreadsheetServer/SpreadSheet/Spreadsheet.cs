// Written by Daniel Bowen for CS 3500, October 2012
// Branched to PS5
// Brancehd to PS6

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SpreadsheetUtilities;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace SS
{
	/// <summary>
	/// This class represents a SpreadSheet
	/// </summary>
	public class Spreadsheet : AbstractSpreadsheet
	{
		//change
		//keep track of the association between cell names and cells
		private readonly SortedDictionary<string, Cell> myCells = new SortedDictionary<string, Cell>();

		//keep track of the relationships among spreadsheet cells
		private readonly DependencyGraph myCellDependencyGraph = new DependencyGraph();

		private static XElement LoadSpreadsheetElement(string filePath)
		{
			try
			{
				return XDocument.Load(filePath).Root;
			}
			catch (Exception ex)
			{
				throw new SpreadsheetReadWriteException(ex.Message);
			}
		}

		private static string GetSpreadsheetVersion(XElement spreadsheetElement)
		{
			var versionAttribute = spreadsheetElement.Attribute("version");

            if (versionAttribute == null)
            {
                 throw new SpreadsheetReadWriteException("the spreadsheet element doesn't have a version attribute.");
                 //var defaultVersion = new XElement(spreadsheetElement).Attribute("version");
                 //versionAttribute = defaultVersion;
            }

			return versionAttribute.Value;
		}

		private void LoadXml(XElement spreadsheetElement)
		{
			var actualVersion = GetSpreadsheetVersion(spreadsheetElement);
			var expectedVersion = Version;

			if (actualVersion != expectedVersion)
			{
				throw new SpreadsheetReadWriteException("the version \"" + actualVersion + "\" doesn't match expected version \"" + expectedVersion + "\".");
			}

			foreach (var cellElement in spreadsheetElement.Descendants("cell"))
			{
				var nameElement = cellElement.Descendants("name").FirstOrDefault();

				if (nameElement == null)
				{
					throw new SpreadsheetReadWriteException("A \"name\" element does not exist inside the \"cell\" element.");
				}

				var cellName = nameElement.Value;
				var contentElement = cellElement.Descendants("contents").FirstOrDefault();

				if (contentElement == null)
				{
					throw new SpreadsheetReadWriteException("A \"contents\" element does not exist inside the \"cell\" element.");
				}

				var cellContent = contentElement.Value;

				SetContentsOfCell(cellName, cellContent);
			}

			return;
		}

		private void LoadFile(string filePath)
		{
			var spreadsheetElement = LoadSpreadsheetElement(filePath);
			LoadXml(spreadsheetElement);
		}

		/// <summary>
		/// Constructs a spreadsheet
		/// </summary>
		public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version) :
			base(isValid, normalize, version)
		{

		}

		/// <summary>
		/// creates an empty spreadsheet that imposes no extra validity conditions, normalizes every cell name to itself, and has version "default".
		/// </summary>
		public Spreadsheet() :
			this(cellName => true, cellName => cellName, "default")
		{
		}

		/// <summary>
		/// Reads a saved spreadsheet from a file and uses it to construct a new spreadsheet. 
		/// The new spreadsheet uses the provided validity delegate, nomalization delegate and version.
		/// </summary>
		public Spreadsheet(string filePath, Func<string, bool> isValid, Func<string, string> normalize, string version) :
			base(isValid, normalize, version)
		{
			LoadFile(filePath);
		}

		public Spreadsheet(XElement spreadsheetElement, Func<string, bool> isValid, Func<string, string> normalize, string version) :
			base(isValid, normalize, version)
		{
			LoadXml(spreadsheetElement);
		}

		public override IEnumerable<string> GetNamesOfAllNonemptyCells()
		{
			return myCells.Keys;
		}

		private bool IsValidCellName(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return false;
			}

			if (!Regex.IsMatch(name, "^([a-z])+([1-9]){1}([0-9])*$", RegexOptions.Singleline|RegexOptions.IgnoreCase|RegexOptions.CultureInvariant))
			{
				return false;
			}

			return IsValid(name);
		}

		public override object GetCellContents(string name)
		{
			if (!IsValidCellName(name))
			{
				throw new InvalidNameException();
			}

			name = Normalize(name);

			Cell cell;

			if (!myCells.TryGetValue(name, out cell))
			{
				throw new InvalidNameException();
			}

			return cell.Contents;
		}

		private ISet<string> SetCellContents(string name, Cell cell, IEnumerable<string> dependees)
		{
			if (!IsValidCellName(name))
			{
				throw new InvalidNameException();
			}

			name = Normalize(name);

			var cells = myCells;

			if (!cell.IsEmpty)
			{
				cells[name] = cell;
			}
			else if (cells.ContainsKey(name))
			{
				cells.Remove(name);
			}

			var cellDependencyGraph = myCellDependencyGraph;
			cellDependencyGraph.ReplaceDependees(name, dependees);
			Changed = true;

			return new SortedSet<string>(GetCellsToRecalculate(name));
		}

		private ISet<string> SetCellContents(string name, Cell cell)
		{
			return SetCellContents(name, cell, new string[0]);
		}

		protected override ISet<string> SetCellContents(string name, double number)
		{
			return SetCellContents(name, new Cell(number));
		}

		protected override ISet<string> SetCellContents(string name, string text)
		{
			if (text == null)
			{
				throw new ArgumentNullException();
			}
			return SetCellContents(name, new Cell(text));
		}

		protected override ISet<string> SetCellContents(string name, Formula formula)
		{
			if (formula==null)
			{
				throw new ArgumentNullException("formula");
			}
			return SetCellContents(name, new Cell(formula), formula.GetVariables());
		}

		protected override IEnumerable<string> GetDirectDependents(string name)
		{
			if (name==null)
			{
				throw new ArgumentNullException("name");
			}

			if (!IsValidCellName(name))
			{
				throw new InvalidNameException();
			}

			name = Normalize(name);

			return myCellDependencyGraph.GetDependents(name);
		}

		private bool myIsChanged;
		/// <summary>
		/// has the spreadsheet changed
		/// </summary>
		public override bool Changed
		{
			get
			{
				return myIsChanged;
			}
			protected set
			{
				myIsChanged = value;
			}
		}

		public override string GetSavedVersion(string filename)
		{
			var spreadsheetElement = LoadSpreadsheetElement(filename);
			return GetSpreadsheetVersion(spreadsheetElement);
		}

		public void Save(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartDocument();
			xmlWriter.WriteStartElement("spreadsheet");
			xmlWriter.WriteAttributeString("version", Version);

			foreach (var item in myCells)
			{
				item.Value.Write(item.Key, xmlWriter);
			}

			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndDocument();

			return;
		}

		public override void Save(string filename)
		{
			try
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(filename))
				{
					Save(xmlWriter);
				}
				Changed = false;
			}
			catch (Exception ex)
			{
				throw new SpreadsheetReadWriteException(ex.Message);
			}
			return;
		}

		public override object GetCellValue(string name)
		{
			if (!IsValidCellName(name))
			{
				throw new InvalidNameException();
			}

			name = Normalize(name);

			Cell cell;

			if (!myCells.TryGetValue(name, out cell))
			{
				throw new InvalidNameException();
			}

			Func<string, double> calculateCellValue = cellName =>
			{
				try
				{
					var value = GetCellValue(cellName);

					if (!(value is double))
					{
						throw new ArgumentException();
					}

					return (double)value;
				}
				catch
				{
					throw new ArgumentException();
				}
			};

			return cell.CalculateValue(calculateCellValue);
		}

		public override ISet<string> SetContentsOfCell(string name, string content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}

			if (!IsValidCellName(name))
			{
				throw new InvalidNameException();
			}

			name = Normalize(name);

			double number;

			if (double.TryParse(content, out number))
			{
				return SetCellContents(name, number);
			}
			else if (content.StartsWith("=", StringComparison.Ordinal))
			{
				return SetCellContents(name, new Formula(IsValid, Normalize, content.Substring(1)));
			}
			else
			{
				return SetCellContents(name, content);
			}
		}
	}
}
