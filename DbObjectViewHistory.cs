using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbShowDepends
{
	public class DbObjectViewHistory
	{
		public string TreeViewPath { get; set; }
		public int LineNumber { get; set; }
		public string ObjectName { get; set; }
		public string DatabaseName { get; set; }
		public string SearchString { get; set; }

		public override string ToString()
		{
			string res;
			res = ObjectName + $" [{LineNumber}]";
			if (!String.IsNullOrEmpty(SearchString))
				res = res + ": " + SearchString;

			return res;
		}
	}
}
