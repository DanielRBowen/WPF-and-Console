using System;
using SpreadsheetUtilities;
using System.Xml;

namespace SS
{
	/// <summary>
	/// Represents a Cell in a spreadsheet
	/// A cell has a value
	/// </summary>
	internal sealed class Cell
	{
		private readonly CellType myType;
		public CellType Type { get { return myType; } }

		private readonly object myContents;
		public object Contents { get { return myContents; } }

		public object CalculateValue(Func<string, double> calculateCellValue)
		{
			if (myType!=CellType.Formula)
			{
				return myContents;
			}
			return ((Formula)myContents).Evaluate(calculateCellValue);
		}

		public bool IsEmpty
		{
			get
			{
				if (myType!=CellType.Text)
				{
					return false;
				}
				return string.IsNullOrEmpty((string)myContents);
			}
		}

		public Cell(string contents)
		{
			myType = CellType.Text;
			myContents = contents;
		}

		public Cell(double contents)
		{
			myType = CellType.Number;
			myContents = contents;
		}

		public Cell(Formula contents)
		{
			myType = CellType.Formula;
			myContents = contents;
		}

		public override string ToString()
		{
			return Contents.ToString();
		}

		public void Write(string cellName, XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("cell");

			xmlWriter.WriteStartElement("name");
			xmlWriter.WriteString(cellName);
			xmlWriter.WriteEndElement();

			xmlWriter.WriteStartElement("contents");

			if (myType == CellType.Formula)
			{
				xmlWriter.WriteString("=");
			}

			xmlWriter.WriteString(myContents.ToString());
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
		}
	}
}