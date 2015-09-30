using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSimpleStocks.Model {
	public class Trade {
		/// <summary>direction: true - buy/long; false - sell/short</summary>
		public bool IsBuy { get; set; }
		public decimal Price { get; set; }
		public decimal Quantity { get; set; }
		public DateTime Timestamp { get; set; }
	}
}
