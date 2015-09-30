using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SuperSimpleStocks.Model;
using SuperSimpleStocks;
using System.Collections;

namespace NUnitSuperSimpleStocks
{
    [TestFixture]
    public class StocksUtilsTests
    {
        decimal[] prices;
        List<Trade> trades;
        List<IStock> stocks;
        IStock common;
        IStock preferred;
        Exchange gbce;

        private static List<Trade> GetTradesMock()
        {
            return new List<Trade> {
 				new Trade { IsBuy = true, Price = 107, Quantity = 50, Timestamp = DateTime.Now.AddMinutes(-14) },
 				new Trade { IsBuy = false, Price = 106, Quantity = 550, Timestamp = DateTime.Now.AddMinutes(-13.5) },
 				new Trade { IsBuy = true, Price = 104, Quantity = 50, Timestamp = DateTime.Now.AddMinutes(-13) },
 				new Trade { IsBuy = false, Price = 105, Quantity = 500, Timestamp = DateTime.Now.AddMinutes(-12) },
 				new Trade { IsBuy = true, Price = 107, Quantity = 50, Timestamp = DateTime.Now.AddMinutes(-11) },
 				new Trade { IsBuy = true, Price = 108, Quantity = 250, Timestamp = DateTime.Now.AddMinutes(-10) },
 				new Trade { IsBuy = false, Price = 107, Quantity = 50, Timestamp = DateTime.Now.AddMinutes(-9) },
 				new Trade { IsBuy = false, Price = 109, Quantity = 150, Timestamp = DateTime.Now.AddMinutes(-8) },
 				new Trade { IsBuy = true, Price = 110, Quantity = 50, Timestamp = DateTime.Now.AddMinutes(-7) },
 				new Trade { IsBuy = false, Price = 109, Quantity = 50, Timestamp = DateTime.Now.AddMinutes(-6) },
 				new Trade { IsBuy = true, Price = 106, Quantity = 150, Timestamp = DateTime.Now.AddMinutes(-5) },
 				new Trade { IsBuy = false, Price = 108, Quantity = 50, Timestamp = DateTime.Now.AddMinutes(-4) },
 				new Trade { IsBuy = true, Price = 107, Quantity = 50, Timestamp = DateTime.Now.AddMinutes(-3) },
 				new Trade { IsBuy = false, Price = 109, Quantity = 150, Timestamp = DateTime.Now.AddMinutes(-2) },
 				new Trade { IsBuy = true, Price = 100, Quantity = 60, Timestamp = DateTime.Now.AddMinutes(-1) },
			};
        }
        private static List<IStock> GetStocksMock()
        {
            return new List<IStock>() { 
				new CommonStock { Symbol = "TEA", LastDividend = 0, ParValue = 100, Trades = GetTradesMock() },
				new CommonStock { Symbol = "POP", LastDividend = 8, ParValue = 100, Trades = GetTradesMock() },
				new CommonStock { Symbol = "ALE", LastDividend = 23, ParValue = 60, Trades = GetTradesMock() },
				new PreferredStock { Symbol = "GIN", LastDividend = 8, ParValue = 100, FixedDividend=0.02m, Trades = GetTradesMock() },
				new CommonStock { Symbol = "JOE", LastDividend = 13, ParValue = 250, Trades = GetTradesMock() }
			};
        }

        [SetUp]
        public void Init()
        {
            prices = new decimal[] { 6, 50, 9, 1200 };
            trades = GetTradesMock();
            stocks = GetStocksMock();
            common = stocks[1];
            preferred = stocks[3];
            gbce = new Exchange();
            gbce.SetStocks(GetStocksMock());
        }

        [Test]
        public void TestGeometricMean()
        {
            Assert.AreEqual(42.43m, Math.Round(StockUtils.GetGeometricMean(prices), 2), "Test GeometricMean");
            Assert.AreEqual(42.43m, Math.Round(StockUtils.GetGeometricMeanUsingLog(prices), 2), "Test GeometricMean using logarithm");
        }

        [Test]
        public void TestDividendYield()
        {
            decimal lastDividend = 8, tickerPrice = 100, fixedDividend = 0.02m, parValue = 100;
            Assert.AreEqual(0.08m, StockUtils.GetDividendYield(lastDividend, tickerPrice), "Test DividendYield according to common type stocks");
            Assert.AreEqual(0.02m, StockUtils.GetDividendYield(fixedDividend, parValue, tickerPrice), "Test DividendYield according to preferred type stocks");

            Assert.AreEqual(0.08m, StockUtils.GetDividendYield(common), "Test common stock DividendYield");
            Assert.AreEqual(0.02m, StockUtils.GetDividendYield(preferred), "Test preferred stock DividendYield");
            Assert.AreEqual(0.08m, gbce.GetDividendYield(common.Symbol), "Test common stock DividendYield");
        }

        [Test]
        public void TestPriceEarningsRatio()
        {
            decimal lastDividend = 8, tickerPrice = 100;
            Assert.AreEqual(12.5m, StockUtils.GetPriceEarningsRatio(tickerPrice, lastDividend), "Test Price Earning Ratio");
            Assert.AreEqual(12.5m, StockUtils.GetPriceEarningsRatio(common), "Test stock Price Earning Ratio");
            Assert.AreEqual(12.5m, gbce.GetPriceEarningsRatio(common.Symbol), "Test stock Price Earning Ratio");
        }

        [Test]
        public void TestStockPrice()
        {
            Assert.AreEqual(106.49m, Math.Round(StockUtils.GetStockPrice(trades), 2), "Test trades stockprice");
            Assert.AreEqual(106.49m, Math.Round(StockUtils.GetStockPrice(common), 2), "Test stocks stockPrice");
            Assert.AreEqual(106.49m, Math.Round(gbce.GetStockPrice(common.Symbol), 2), "Test stocks stockPrice");

        }

        [Test]
        public void TestTickerPrice()
        {
            Assert.AreEqual(100m, StockUtils.GetTickerPrice(trades), "Test trades tickerprice");
            Assert.AreEqual(100m, StockUtils.GetTickerPrice(common), "Test stocks tickerPrice");
            Assert.AreEqual(100m, gbce.GetTickerPrice(common.Symbol), "Test stocks tickerPrice");
        }

        [Test]
        public void TestSetTrade()
        {
            StockUtils.SetTrade(common, new Trade { IsBuy = false, Price = 106, Quantity = 550, Timestamp = DateTime.Now });
            StockUtils.SetTrade(preferred, true, 105, 550, DateTime.Now);
            gbce.SetTrade(common.Symbol, false, 106, 550, DateTime.Now);

            Assert.AreEqual(106, common.GetTickerPrice(), "test set trade");
            Assert.AreEqual(105, preferred.GetTickerPrice(), "test set trade attributes");
            Assert.AreEqual(106, gbce.GetTickerPrice(common.Symbol), "test set trade");
        }

        [Test]
        public void TestAllShareIndex()
        {
            Assert.AreEqual(106.49m, Math.Round(StockUtils.GetAllShareIndex(stocks), 2), "Test AllShareIndex");
            Assert.AreEqual(106.49m, Math.Round(gbce.GetAllShareIndex(), 2), "Test AllShareIndex");
            stocks[0].SetTrade(true, 105, 550, DateTime.Now);
            gbce.SetTrade(stocks[0].Symbol, true, 105, 550, DateTime.Now);
            Assert.AreEqual(106.43m, Math.Round(StockUtils.GetAllShareIndex(stocks), 2), "Test AllShareIndex with new trade in one Stock");
            Assert.AreEqual(106.43m, Math.Round(gbce.GetAllShareIndex(), 2), "Test AllShareIndex with new trade in one Stock");

        }

        [TestCaseSource("InvalidParameters")]
        public decimal TestValidSymbolsPrice(string symbol) {
            return gbce.GetTickerPrice(symbol);
        }
        static IEnumerable InvalidParameters
        {
            get
            {
                yield return new TestCaseData("TEA").Returns(100m);
                yield return new TestCaseData("POP").Returns(100m);
                yield return new TestCaseData("ALE").Returns(100m);
                yield return new TestCaseData("COKE")
                  .Throws(typeof(ArgumentException))
                  .SetName("InvalidSymbol")
                  .SetDescription("The requested symbol does not exist in the current context");
            }
        }  
    }
}
