using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSimpleStocks.Model;

namespace SuperSimpleStocks
{
    public class Exchange
    {
        public Dictionary<string, IStock> StockList { get; set; }

        public void SetStocks(List<IStock> stocks)
        {
            if (StockList == null)
                StockList = new Dictionary<string, IStock>();
            foreach (var stock in stocks)
            {
                StockList[stock.Symbol] = stock;
            }
        }
        public IStock GetStock(string symbol){
            if (!StockList.ContainsKey(symbol))
            {
                throw new ArgumentException("invalid symbol");
            }
            return StockList[symbol];
        }
        public decimal GetStockPrice(string symbol)
        {
            return GetStock(symbol).GetStockPrice();
        }

        public decimal GetTickerPrice(string symbol)
        {
            return GetStock(symbol).GetTickerPrice();
        }

        public decimal GetDividendYield(string symbol)
        {
            return GetStock(symbol).GetDividendYield();
        }

        public decimal GetPriceEarningsRatio(string symbol)
        {
            return GetStock(symbol).GetPriceEarningsRatio();
        }
        public void SetTrade(string symbol, bool isBuy, decimal price, decimal quantity, DateTime timestamp)
        {
            GetStock(symbol).SetTrade(isBuy, price, quantity, timestamp);
        }
        public decimal GetAllShareIndex()
        {
            return StockUtils.GetAllShareIndex(StockList.Values.ToList());
        }
    }
}
