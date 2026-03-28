using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace finance_tracker_comp586.utils
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

            return totals.Select(t => new PieSeries<double>
            {
                Values = new double[] { t.Amount },
                Name = t.Category,
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = 14,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,

                // FIX: Use .Coordinate.PrimaryValue instead of .PrimaryValue
                DataLabelsFormatter = point => $"${point.Coordinate.PrimaryValue:N2}"
            }).ToArray();
        }
    }
}