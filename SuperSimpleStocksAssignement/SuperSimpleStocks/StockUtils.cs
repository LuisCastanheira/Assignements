using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSimpleStocks.Model;

namespace SuperSimpleStocks {
	public static class StockUtils {
        /// <summary>
        /// calculated with the n square of the combined product of n values.
        /// lighter but prone to overflow/underflow
        /// </summary>
        /// <param name="values">the decimal values</param>
        /// <returns>the calculated geometric mean</returns>
		public static decimal GetGeometricMean(params decimal[] values) {
			decimal output = 0;
			decimal product = 1m;
			foreach (var price in values) {
				if (price == 0) continue;
				product *= price;
			}
			output = (decimal)System.Math.Pow((double)product, 1d / values.Length);
			return output;
		}
        /// <summary>
        /// calculated using the logarithms
        /// heavier, yet safer against overflow/underflow
        /// </summary>
        /// <param name="values">the decimal values</param>
        /// <returns>the calculated geometric mean</returns>
		public static decimal GetGeometricMeanUsingLog(params decimal[] values) {
			decimal output = 0;
			decimal logSum = 0;
			foreach (var price in values) {
				if (price == 0) continue;
				logSum += (decimal)Math.Log((double)price);
			}
			var GeometricMeanLog = logSum / values.Length;
			output = (decimal)System.Math.Exp((double)GeometricMeanLog);
			return output;
		}

		public static decimal GetDividendYield(decimal lastDividend, decimal tickerPrice) {
			return lastDividend / tickerPrice;
		}

		public static decimal GetDividendYield(decimal fixedDividend, decimal parValue, decimal tickerPrice) {
			return fixedDividend * parValue / tickerPrice;
		}

		public static decimal GetPriceEarningsRatio(decimal tickerPrice, decimal dividend) {
			var output = 0m;
			if (dividend != 0)
				output = tickerPrice / dividend;
			return output;
		}

		private const int MINUTES = 15;
        /// <summary>
        /// returns the stock price based on the weighted average from the trades over the past 15 minutes
        /// assuming the stock wouldn't have trades over the past 15 minutes the method would then return the last trade price
        /// </summary>
        /// <param name="trades">the trade list</param>
        /// <returns>the stock price</returns>
		public static decimal GetStockPrice(List<Trade> trades) {
			var output = 0m;
			if (trades != null && trades.Count > 0) {
				var periodStart = DateTime.Now.AddMinutes(-MINUTES);
				decimal totalValue = 0, totalQuantity = 0;
				int i = trades.Count;
				do {
					i--;
					var t = trades[i];
					totalValue += t.Price * t.Quantity;
					totalQuantity += t.Quantity;
				}
				while (i > 0 && trades[i].Timestamp > periodStart);
                output = totalValue / totalQuantity;
			}
			return output;
		}

		public static decimal GetStockPrice(this IStock stock) {
			return GetStockPrice(stock.Trades);
		}
		public static decimal GetTickerPrice(List<Trade> trades) {
			return trades.Count != 0 ? trades[trades.Count - 1].Price : 0m;
		}
		public static decimal GetTickerPrice(this IStock stock) {
			var t = stock.Trades;
			return t.Count != 0 ? t[t.Count - 1].Price : 0m;
		}
		public static decimal GetDividendYield(this IStock stock) {
			var output = 0m;
			if (stock is PreferredStock) {
				output = GetDividendYield(((PreferredStock)stock).FixedDividend, stock.ParValue, stock.GetTickerPrice());
			}
			else{
				output = GetDividendYield(stock.LastDividend, stock.GetTickerPrice());
			}
			return output;
		}
		public static decimal GetPriceEarningsRatio(this IStock stock) {
            return StockUtils.GetPriceEarningsRatio(stock.GetTickerPrice(), stock.LastDividend);
		}
		public static void SetTrade(this IStock stock, Trade trade) {
			if (stock.Trades != null) {
				stock.Trades.Add(trade);
			}
			else {
				stock.Trades = new List<Trade>() { trade };
			}
		}
		public static void SetTrade(this IStock stock, bool isBuy, decimal price, decimal quantity, DateTime timestamp) {
			SetTrade(stock, new Trade { IsBuy = isBuy, Price = price, Quantity = quantity, Timestamp = timestamp });
		}
		public static decimal GetAllShareIndex(List<IStock> stocks) {
			decimal output = 0;
			decimal[] stockPrices = new decimal[stocks.Count];
			for (int i = 0; i < stockPrices.Length; i++) 
				stockPrices[i] = stocks[i].GetStockPrice();
			output = GetGeometricMeanUsingLog(stockPrices);
			//output = GetGeometricMeanUsingLog(stocks.Select(s => s.GetStockPrice()).ToArray());
			return output;
		}
	}
}
