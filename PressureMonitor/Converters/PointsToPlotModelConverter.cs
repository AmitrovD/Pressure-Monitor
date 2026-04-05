using OxyPlot.Series;
using OxyPlot;
using PressureMonitor.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PressureMonitor.Converters
{
    public class PointsToPlotModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not List<TrialPoint> points) return new PlotModel();

            var model = new PlotModel();
            var series = new LineSeries();
            foreach (var p in points)
                series.Points.Add(new DataPoint(p.Time, p.Pressure));
            model.Series.Add(series);
            return model;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
            => throw new NotImplementedException();
    }
}
