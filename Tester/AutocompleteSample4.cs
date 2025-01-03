﻿using FastColoredTextBoxNS;
using FastColoredTextBoxNS.Types;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Tester {
	public partial class AutocompleteSample4 : Form {
		readonly AutocompleteMenu popupMenu;

		static readonly string[] sources = new string[]{
			"com",
			"com.company",
			"com.company.Class1",
			"com.company.Class1.Method1",
			"com.company.Class1.Method2",
			"com.company.Class2",
			"com.company.Class3",
			"com.example",
			"com.example.ClassX",
			"com.example.ClassX.Method1",
			"com.example.ClassY",
			"com.example.ClassY.Method1"
		};

		public AutocompleteSample4() {
			InitializeComponent();

			//create autocomplete popup menu
			popupMenu = new AutocompleteMenu(fctb) {
				SearchPattern = @"[\w\.]"
			};

			//
			var items = new List<AutocompleteItem>();
			foreach (var item in sources)
				items.Add(new MethodAutocompleteItem2(item));

			popupMenu.Items.SetAutocompleteItems(items);
		}
	}

	/// <summary>
	/// This autocomplete item appears after dot
	/// </summary>
	public class MethodAutocompleteItem2 : MethodAutocompleteItem {
		readonly string firstPart;
		readonly string lastPart;

		public MethodAutocompleteItem2(string text)
			: base(text) {
			var i = text.LastIndexOf('.');
			if (i < 0)
				firstPart = text;
			else {
				firstPart = text[..i];
				lastPart = text[(i + 1)..];
			}
		}

		public override CompareResult Compare(string fragmentText) {
			int i = fragmentText.LastIndexOf('.');

			if (i < 0) {
				if (firstPart.StartsWith(fragmentText) && string.IsNullOrEmpty(lastPart))
					return CompareResult.VisibleAndSelected;
				//if (firstPart.ToLower().Contains(fragmentText.ToLower()))
				//  return CompareResult.Visible;
			} else {
				var fragmentFirstPart = fragmentText[..i];
				var fragmentLastPart = fragmentText[(i + 1)..];


				if (firstPart != fragmentFirstPart)
					return CompareResult.Hidden;

				if (lastPart != null && lastPart.StartsWith(fragmentLastPart))
					return CompareResult.VisibleAndSelected;

				if (lastPart != null && lastPart.ToLower().Contains(fragmentLastPart.ToLower()))
					return CompareResult.Visible;

			}

			return CompareResult.Hidden;
		}

		public override string GetTextForReplace() {
			if (lastPart == null)
				return firstPart;

			return firstPart + "." + lastPart;
		}

		public override string ToString() {
			if (lastPart == null)
				return firstPart;

			return lastPart;
		}
	}
}
