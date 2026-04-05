using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace PressureMonitor.Models
{
    public class PressureChartModel
    {
        public PlotModel PlotModel { get; }
        private readonly LineSeries _series;

        public PressureChartModel() {
            PlotModel = new PlotModel() { Title = "Статистика"};

            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Время",
                MajorGridlineStyle = LineStyle.Solid
            });

            PlotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Давление",
                Minimum = 0,
                MajorGridlineStyle = LineStyle.Solid
            });

            _series = new LineSeries();
            PlotModel.Series.Add(_series);
        }

        public void AddPoint(int time, double pressure)
        {
            var lasPointTime = _series.Points.LastOrDefault();
            _series.Points.Add(new DataPoint(time, pressure));
            PlotModel.InvalidatePlot(true);
        }
        public List<TrialPoint> GetPoints()
        {
            return _series.Points
                .Select(p => new TrialPoint
                {
                    Time = (int)p.X,
                    Pressure = p.Y,
                })
                .ToList();
        }
        public void Clear()
        {
            _series.Points.Clear();
            PlotModel.InvalidatePlot(true);
        }
    }
}
