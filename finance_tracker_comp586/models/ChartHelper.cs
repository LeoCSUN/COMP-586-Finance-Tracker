using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace finance_tracker_comp586.models
{
    public static class ChartHelper
    {
        public static ISeries[] GetPieSeries(Wallet wallet)
        {
            var totals = wallet.Transactions
                .Where(t => t.Category != TransactionCategory.Income)
                .GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key.ToString(), Amount = (double)g.Sum(t => t.Amount) })
                .ToList();

            var series = totals.Select(t => new PieSeries<double>
            {
                Values = new double[] { t.Amount },
                Name = t.Category,
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = 16,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle
            }).ToArray();

            return series;
        }
    }
}