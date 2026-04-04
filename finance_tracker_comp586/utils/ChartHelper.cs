using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace finance_tracker_comp586.utils
{
    public static class ChartHelper
    {
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
    }
}