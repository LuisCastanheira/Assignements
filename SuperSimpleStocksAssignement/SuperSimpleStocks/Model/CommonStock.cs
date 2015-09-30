using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSimpleStocks.Model {
	public class CommonStock : IStock {
		public string Symbol { get; set; }
		public decimal LastDividend { get; set; }
		public decimal ParValue { get; set; }
		public List<Trade> Trades { get; set; }
	}
}
