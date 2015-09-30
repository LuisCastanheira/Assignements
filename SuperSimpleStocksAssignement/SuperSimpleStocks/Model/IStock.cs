using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuperSimpleStocks.Model {
	public interface IStock {
		string Symbol { get; set; }
		decimal LastDividend { get; set; }
		decimal ParValue { get; set; }
		List<Trade> Trades { get; set; }
	}
}
