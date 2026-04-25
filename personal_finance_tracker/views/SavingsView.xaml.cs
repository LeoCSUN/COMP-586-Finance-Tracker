using personal_finance_tracker;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace personal_finance_tracker.views
{
    public partial class SavingsView : Page
    {
        public string FullName => App.CurrentUser?.FullName ?? "";
        private personal_finance_tracker.Savings savings;
        private int _interestTabIndex = 0;

        public SavingsView()
        {
            InitializeComponent();
            savings = App.CurrentUser!.GetSavings();
            DataContext = savings;
            savings.PropertyChanged += (_, _) => UpdateActiveChart();
            UpdateActiveChart();
        }

        private void InterestRange_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                _interestTabIndex = rb.Tag switch
                {
                    "Year"     => 1,
                    "Lifetime" => 2,
                    _          => 0
                };
                UpdateActiveChart();
            }
        }

        private void UpdateActiveChart()
        {
            if (savings == null) return;

            var stroke = new SKColor(139, 0, 0);
            var fill   = new SKColor(139, 0, 0, 40);

            switch (_interestTabIndex)
            {
                case 1:
                    InterestChart.Series = BuildYearSeries(stroke, fill);
                    InterestChart.XAxes  = BuildXAxes("MMM");
                    break;
                case 2:
                    InterestChart.Series = BuildLifetimeSeries(stroke, fill);
                    InterestChart.XAxes  = BuildXAxes("MMM yy");
                    break;
                default:
                    InterestChart.Series = BuildMonthSeries(stroke, fill);
                    InterestChart.XAxes  = BuildXAxes("dd MMM");
                    break;
            }
            InterestChart.YAxes = BuildYAxes();
        }

        private ISeries[] BuildMonthSeries(SKColor stroke, SKColor fill)
        {
            if (!savings.HasBalance || savings.PrincipalStartDate == null) return [];

            var today        = DateTime.Today;
            var firstOfMonth = new DateTime(today.Year, today.Month, 1);
            var startDate    = savings.PrincipalStartDate.Value > firstOfMonth
                                   ? savings.PrincipalStartDate.Value
                                   : firstOfMonth;

            double dailyRate = (double)savings.APY / 100.0 / 365.0;
            var points       = new List<DateTimePoint>();

            for (var d = startDate; d <= today; d = d.AddDays(1))
            {
                double days     = (d - savings.PrincipalStartDate.Value).TotalDays;
                double interest = (double)savings.Principal * (Math.Pow(1.0 + dailyRate, days) - 1.0);
                points.Add(new DateTimePoint(d, Math.Max(0.0, interest)));
            }

            if (points.Count < 2) return [];
            return [CreateLineSeries(points, stroke, fill)];
        }

        private ISeries[] BuildYearSeries(SKColor stroke, SKColor fill)
        {
            if (!savings.HasBalance || savings.PrincipalStartDate == null) return [];

            var today      = DateTime.Today;
            double monthly = (double)savings.APY / 100.0 / 12.0;
            var points     = new List<DateTimePoint>();

            for (int m = 1; m <= today.Month; m++)
            {
                var date    = new DateTime(today.Year, m, 1);
                double months = (date.Year - savings.PrincipalStartDate.Value.Year) * 12
                              + (date.Month - savings.PrincipalStartDate.Value.Month);
                if (months < 0) months = 0;
                double interest = (double)savings.Principal * (Math.Pow(1.0 + monthly, months) - 1.0);
                points.Add(new DateTimePoint(date, Math.Max(0.0, interest)));
            }

            if (points.Count < 2) return [];
            return [CreateLineSeries(points, stroke, fill)];
        }

        private ISeries[] BuildLifetimeSeries(SKColor stroke, SKColor fill)
        {
            if (!savings.HasBalance || savings.PrincipalStartDate == null) return [];

            var today      = DateTime.Today;
            double monthly = (double)savings.APY / 100.0 / 12.0;
            var points     = new List<DateTimePoint>();

            for (var cur = savings.PrincipalStartDate.Value; cur <= today; cur = cur.AddMonths(1))
            {
                double months   = (cur.Year - savings.PrincipalStartDate.Value.Year) * 12
                                + (cur.Month - savings.PrincipalStartDate.Value.Month);
                double interest = (double)savings.Principal * (Math.Pow(1.0 + monthly, months) - 1.0);
                points.Add(new DateTimePoint(cur, Math.Max(0.0, interest)));
            }

            if (points.Count < 2) return [];
            return [CreateLineSeries(points, stroke, fill)];
        }

        private static ISeries CreateLineSeries(IEnumerable<DateTimePoint> points, SKColor stroke, SKColor fill) =>
            new LineSeries<DateTimePoint>
            {
                Values         = points.ToList(),
                Stroke         = new SolidColorPaint(stroke) { StrokeThickness = 2 },
                Fill           = new SolidColorPaint(fill),
                GeometrySize   = 6,
                GeometryFill   = new SolidColorPaint(stroke),
                GeometryStroke = new SolidColorPaint(stroke) { StrokeThickness = 2 },
            };

        private static Axis[] BuildXAxes(string format) =>
            [new Axis { Labeler = value => new DateTime((long)value).ToString(format), LabelsRotation = 0 }];

        private static Axis[] BuildYAxes() =>
            [new Axis { IsVisible = false }];

        private void Home_Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new HomeView());
        }

        private void LogOff_Button_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentUser = null;
            NavigationService?.Navigate(new LoginView());
        }

        private async void Add_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow { Owner = Window.GetWindow(this), Label = "Enter Amount to Add" };
            bool? result = amountWindow.ShowDialog();

            if (result == true)
            {
                decimal enteredAmount = amountWindow.EnteredAmount;
                savings.Deposit(enteredAmount);
                try { await App.Users.UpdateUser(App.CurrentUser!); }
                catch { }
            }
        }

        private async void Subtract_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow { Owner = Window.GetWindow(this), Label = "Enter Amount to Subtract" };
            bool? result = amountWindow.ShowDialog();

            if (result == true)
            {
                decimal enteredAmount = amountWindow.EnteredAmount;
                savings.Withdraw(enteredAmount);
                try { await App.Users.UpdateUser(App.CurrentUser!); }
                catch { }
            }
        }

        private async void Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            var amountWindow = new AmountInputWindow { Owner = Window.GetWindow(this), Label = "Enter New APY" };
            bool? result = amountWindow.ShowDialog();

            if (result == true)
            {
                decimal enteredAmount = amountWindow.EnteredAmount;
                savings.SetApy(enteredAmount);
                try { await App.Users.UpdateUser(App.CurrentUser!); }
                catch { }
            }
        }
    }
}
