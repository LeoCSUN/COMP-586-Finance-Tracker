using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace personal_finance_tracker.utils
{
    public static class ChartHelper
    {
        private static readonly SKColor[] SectorPalette =
        [
            new SKColor(59,  130, 246),
            new SKColor(16,  185, 129),
            new SKColor(245, 158, 11),
            new SKColor(139, 92,  246),
            new SKColor(236, 72,  153),
            new SKColor(20,  184, 166),
            new SKColor(249, 115, 22),
            new SKColor(132, 204, 22),
            new SKColor(100, 116, 139),
            new SKColor(239, 68,  68),
        ];

        public static (ISeries[] Series, List<LegendItem> Legend) GetSectorPieSeries(IEnumerable<OwnedStock> stocks)
        {
            var groups = stocks
                .GroupBy(s => string.IsNullOrWhiteSpace(s.Stock.Sector) ? "Other" : s.Stock.Sector)
                .Select((g, i) => new
                {
                    Sector = g.Key,
                    Total  = g.Sum(s => s.Shares * s.AvgPrice),
                    Color  = SectorPalette[i % SectorPalette.Length]
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            var series = groups.Select(g =>
            {
                var ps = new PieSeries<double> { Values = [(double)g.Total], Name = g.Sector };
                ps.Fill = new SolidColorPaint(g.Color);
                return (ISeries)ps;
            }).ToArray();

            var legend = groups.Select(g => new LegendItem
            {
                Category   = g.Sector,
                Amount     = g.Total,
                ColorBrush = new SolidColorBrush(Color.FromRgb(g.Color.Red, g.Color.Green, g.Color.Blue))
            }).ToList();

            return (series, legend);
        }

        public static readonly Dictionary<string, (SKColor SkColor, SolidColorBrush WpfBrush)> CategoryColors = new()
        {
            { "Food",           (new SKColor(239, 68,  68),  new SolidColorBrush(Color.FromRgb(239, 68,  68)))  },
            { "Utilities",      (new SKColor(234, 179, 8),   new SolidColorBrush(Color.FromRgb(234, 179, 8)))   },
            { "Rent",           (new SKColor(249, 115, 22),  new SolidColorBrush(Color.FromRgb(249, 115, 22)))  },
            { "Transportation", (new SKColor(139, 92,  246), new SolidColorBrush(Color.FromRgb(139, 92,  246))) },
            { "Entertainment",  (new SKColor(34,  197, 94),  new SolidColorBrush(Color.FromRgb(34,  197, 94)))  },
            { "Other",          (new SKColor(107, 114, 128), new SolidColorBrush(Color.FromRgb(107, 114, 128))) },
        };

        public static ISeries[] GetPieSeries(Wallet wallet) =>
            wallet.Transactions
                .Where(t => t.Category != TransactionCategory.Income)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key.ToString(), Amount = (double)g.Sum(t => t.Amount) })
                .Select(t =>
                {
                    var series = new PieSeries<double> { Values = [t.Amount], Name = t.Category };
                    if (CategoryColors.TryGetValue(t.Category, out var c))
                        series.Fill = new SolidColorPaint(c.SkColor);
                    return (ISeries)series;
                })
                .ToArray();

        public static ISeries[] GetPieSeries(IEnumerable<Transaction> transactions) =>
            transactions
                .Where(t => t.Category != TransactionCategory.Income)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key.ToString(), Amount = (double)g.Sum(t => t.Amount) })
                .Select(t =>
                {
                    var series = new PieSeries<double> { Values = [t.Amount], Name = t.Category };
                    if (CategoryColors.TryGetValue(t.Category, out var c))
                        series.Fill = new SolidColorPaint(c.SkColor);
                    return (ISeries)series;
                })
                .ToArray();
    }
}
