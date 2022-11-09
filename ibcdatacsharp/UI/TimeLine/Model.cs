using OxyPlot.Axes;
using OxyPlot;
using OxyPlot.Annotations;
using System.ComponentModel;
using ScottPlot;
using System;
using System.Drawing;
using ScottPlot.Plottable;
using System.Windows.Controls;

namespace ibcdatacsharp.UI.TimeLine
{
    public class Model
    {
        public delegate void TimeEventHandler(object sender, TimeArgs args);
        public event TimeEventHandler timeEvent;

        private TextBlock timer;
        private WpfPlot plot;
        private HSpan line;
        private double pos = 0;
        private double width = 0.5;
        private double minX = 0;
        private double maxX = 100;
        private double minY = 0;
        private double maxY = 1;
        private double lastTime;
        private double MIN_CHANGE_TO_NOTIFY;

        // Crea los ejes y las lineas
        public Model(WpfPlot plot, TextBlock timer, double UPDATE_TIME_MS)
        {
            this.timer = timer;
            this.plot = plot;
            plot.Plot.XAxis2.SetSizeLimit(pad:0);
            plot.Plot.XAxis2.SetSizeLimit(pad:0);
            plot.Plot.SetInnerViewLimits(xMin: minX, xMax: maxX, yMin:minY, yMax:maxY);
            plot.Plot.SetOuterViewLimits(xMin: minX, xMax: maxX, yMin: minY, yMax: maxY);
            line = plot.Plot.AddHorizontalSpan(pos - width, pos + width, Color.LightSkyBlue);
            line.DragEnabled = true;
            line.DragFixedSize = true;
            line.DragLimitMin = minX;
            line.DragLimitMax = maxX;
            plot.Plot.SetAxisLimitsX(minX, maxX);
            plot.Plot.SetAxisLimitsY(minY, maxY);
            plot.Plot.XAxis.TickLabelFormat(formatAxis);
            plot.Plot.YAxis.TickLabelFormat((_) => 
            {
                return "";
            });
            plot.Refresh();
            line.Dragged += (sender, e) =>
            {
                timeEvent?.Invoke(this, new TimeArgs() { time=line.X1 + width});
                NotifyTimeChanged();
            };
            MIN_CHANGE_TO_NOTIFY = UPDATE_TIME_MS / 1000;
        }
        public void setTime(double time)
        {
            if (time > maxX)
            {
                this.time = maxX;
            }
            else
            {
                this.time = time;
            }
            plot.Render();
            NotifyTimeChanged();
        }
        private void NotifyTimeChanged()
        {
            if (hasToNotify())
            {
                timer.Text = formatTimer(TimeSpan.FromSeconds(time));
                lastTime = time;
            }
        }
        private bool hasToNotify()
        {
            return Math.Abs(time - lastTime) > MIN_CHANGE_TO_NOTIFY;
        }
        private string formatAxis(double time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            if (time < 60 * 60)
            {
                return string.Format("{0:D2}:{1:D2}",
                timeSpan.Minutes,
                timeSpan.Seconds);
            }
            else
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}",
                timeSpan.Hours,
                timeSpan.Minutes,
                timeSpan.Seconds);
            }
        }
        private double time
        {
            get
            {
                return line.X1 + width;
            }
            set
            {
                line.X1 = value - width;
                line.X2 = value + width;
            }
        }
        public string formatTimer(TimeSpan timeSpan)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D3}",
            timeSpan.Hours,
            timeSpan.Minutes,
            timeSpan.Seconds,
            timeSpan.Milliseconds);
        }
    }
}
