using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSimpleStocks.Model {
	public class PreferredStock : CommonStock {
		public decimal FixedDividend { get; set; }
	}
}
