using LiveChartsCore;
using LiveChartsCore.Drawing; // for SolidColorPaint
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp; // for SKColor
using System.Collections.Generic;
using System.Linq;

namespace finance_tracker_comp586.models
{
    public static class ChartHelper
    {
        public static ISeries[] GetPieSeries(Wallet wallet)
        {
            // Group transactions by category and sum amounts
            var totals = wallet.Transactions
                .Where(t => t.Category != TransactionCategory.Income)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key.ToString(), Amount = (double)g.Sum(t => t.Amount) })
                .ToList();

            // Convert each category into a PieSeries
            var series = totals.Select(t => new PieSeries<double>
            {
                Values = new double[] { t.Amount },
                Name = t.Category,
                DataLabelsPaint = new SolidColorPaint(SKColors.Black), // ✅ Use SKColors from SkiaSharp
                DataLabelsSize = 16,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
            }).ToArray();

            return series;
        }
    }
}