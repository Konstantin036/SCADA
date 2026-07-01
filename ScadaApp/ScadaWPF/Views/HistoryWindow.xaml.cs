using DataConcentrator.Models;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ScadaWPF.Views
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(AnalogInput tag, List<TagValueHistory> history)
        {
            InitializeComponent();

            txtTagInfo.Text = $"{tag.TagName} | {tag.Description}";

            if (!history.Any())
            {
                txtMin.Text = "Nema podataka";
                return;
            }

            double min = history.Min(h => h.Value);
            double max = history.Max(h => h.Value);
            double avg = history.Average(h => h.Value);

            txtMin.Text = $"Min: {min:F2} {tag.Units}";
            txtMax.Text = $"Max: {max:F2} {tag.Units}";
            txtAvg.Text = $"Avg: {avg:F2} {tag.Units}";

            var model = new PlotModel
            {
                Background = OxyColor.FromRgb(45, 45, 45),
                TextColor = OxyColors.White,
                PlotAreaBorderColor = OxyColor.FromRgb(0, 122, 204)
            };

            // X osa — vreme
            model.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "HH:mm:ss",
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                Title = "Vreme"
            });

            // Y osa — vrednost
            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                TextColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                Title = tag.Units
            });

            // Grid
            model.Axes[0].MajorGridlineStyle = LineStyle.Dot;
            model.Axes[0].MajorGridlineColor = OxyColor.FromRgb(60, 60, 60);
            model.Axes[1].MajorGridlineStyle = LineStyle.Dot;
            model.Axes[1].MajorGridlineColor = OxyColor.FromRgb(60, 60, 60);

            // Linija vrednosti
            var series = new LineSeries
            {
                Title = tag.TagName,
                Color = OxyColor.FromRgb(0, 122, 204),
                StrokeThickness = 2
            };

            foreach (var h in history.OrderBy(h => h.Timestamp))
                series.Points.Add(new DataPoint(
                    DateTimeAxis.ToDouble(h.Timestamp), h.Value));

            model.Series.Add(series);

            // Alarm linije
            foreach (var alarm in tag.Alarms)
            {
                var alarmLine = new LineAnnotation
                {
                    Type = LineAnnotationType.Horizontal,
                    Y = alarm.Limit,
                    Color = alarm.Type == AlarmType.High ? OxyColors.Red : OxyColors.Orange,
                    StrokeThickness = 1,
                    LineStyle = LineStyle.Dash,
                    Text = $"{alarm.Type} Alarm ({alarm.Limit})"
                };
                model.Annotations.Add(alarmLine);
            }

            plotView.Model = model;
        }
    }
}