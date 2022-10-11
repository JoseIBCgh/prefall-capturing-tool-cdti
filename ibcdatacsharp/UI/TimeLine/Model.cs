using OxyPlot.Axes;
using OxyPlot;
using OxyPlot.Annotations;
using System.ComponentModel;

namespace ibcdatacsharp.UI.TimeLine
{
    public class Model : INotifyPropertyChanged
    {
        private const int MIN_MAX_MIN_DISTANCE = 5;
        public event PropertyChangedEventHandler PropertyChanged;
        public delegate void CurrentFrameEventHandler(object sender, CurrentFrameArgs args);
        public event CurrentFrameEventHandler currentFrameEvent;
        private LineAnnotation currentFrameLine;
        private LineAnnotation minFrameLine;
        private LineAnnotation maxFrameLine;

        // Crea los ejes y las lineas
        public Model(int minFrame=0, int maxFrame=100)
        {
            PlotModel = new PlotModel();
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = 100, IntervalLength = 20, IsAxisVisible=false });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = minFrame - 1, Maximum = maxFrame + 1, Title = "frames", Unit = "kFreq", FontSize = 10, IntervalLength = 200 });
            minFrameLine = new LineAnnotation()
            {
                Type = LineAnnotationType.Vertical,
                X = minFrame,
                Color = OxyColors.DeepSkyBlue,
                StrokeThickness = 1,
            };
            maxFrameLine = new LineAnnotation()
            {
                Type = LineAnnotationType.Vertical,
                X = maxFrame,
                Color = OxyColors.DeepSkyBlue,
                StrokeThickness = 1,
            };
            currentFrameLine = new LineAnnotation(){
                Type = LineAnnotationType.Vertical,
                X=minFrame,
                Color = OxyColors.Blue,
                StrokeThickness=1,
            };
            // minFrameLine handlers
            minFrameLine.MouseDown += (sender, e) =>
            {
                e.Handled = true;
            };
            minFrameLine.MouseMove += (sender, e) =>
            {
                minFrameLine.X = minFrameLine.InverseTransform(e.Position).X;
                if (minFrameLine.X < minFrame)
                {
                    minFrameLine.X = minFrame;
                }
                if(minFrameLine.X > maxFrameLine.X - MIN_MAX_MIN_DISTANCE)
                {
                    maxFrameLine.X = minFrameLine.X + MIN_MAX_MIN_DISTANCE;
                    if(maxFrameLine.X > maxFrame)
                    {
                        maxFrameLine.X = maxFrame;
                        minFrameLine.X = maxFrameLine.X - MIN_MAX_MIN_DISTANCE;
                    }
                }
                if (minFrameLine.X > currentFrameLine.X)
                {
                    currentFrameLine.X = minFrameLine.X;
                    NotifyCurrentFrameChanged();
                }
                PlotModel.InvalidatePlot(false);
                e.Handled = true;
            };
            minFrameLine.MouseUp += (sender, e) =>
            {
                PlotModel.InvalidatePlot(false);
                e.Handled = true;
            };
            // maxFrameLine handlers
            maxFrameLine.MouseDown += (sender, e) =>
            {
                e.Handled = true;
            };
            maxFrameLine.MouseMove += (sender, e) =>
            {
                maxFrameLine.X = maxFrameLine.InverseTransform(e.Position).X;
                if (maxFrameLine.X > maxFrame)
                {
                    maxFrameLine.X = maxFrame;
                }
                if(maxFrameLine.X < minFrameLine.X + MIN_MAX_MIN_DISTANCE)
                {
                    minFrameLine.X = maxFrameLine.X - MIN_MAX_MIN_DISTANCE;
                    if(minFrameLine.X < minFrame)
                    {
                        minFrameLine.X = minFrame;
                        maxFrameLine.X = minFrameLine.X + MIN_MAX_MIN_DISTANCE;
                    }
                }
                if (maxFrameLine.X < currentFrameLine.X)
                {
                    currentFrameLine.X = maxFrameLine.X;
                    NotifyCurrentFrameChanged();
                }
                PlotModel.InvalidatePlot(false);
                e.Handled = true;
            };
            maxFrameLine.MouseUp += (sender, e) =>
            {
                PlotModel.InvalidatePlot(false);
                e.Handled = true;
            };
            // currentFrameLine handlers
            currentFrameLine.MouseDown += (sender, e) =>
            {
                CurrentFrameArgs args = new CurrentFrameArgs();
                args.start = true;
                emitCurrentFrame(args);
                e.Handled = true;
            };
            currentFrameLine.MouseMove += (sender, e) =>
            {
                currentFrameLine.X = currentFrameLine.InverseTransform(e.Position).X;
                if(currentFrameLine.X < minFrameLine.X)
                {
                    currentFrameLine.X = minFrameLine.X;
                }
                if(currentFrameLine.X > maxFrameLine.X)
                {
                    currentFrameLine.X = maxFrameLine.X;
                }
                NotifyCurrentFrameChanged();
                PlotModel.InvalidatePlot(false);
                e.Handled = true;
            };
            currentFrameLine.MouseUp += (sender, e) =>
            {
                CurrentFrameArgs args = new CurrentFrameArgs();
                args.end = true;
                emitCurrentFrame(args);
                PlotModel.InvalidatePlot(false);
                e.Handled = true;
            };
            PlotModel.Annotations.Add(minFrameLine);
            PlotModel.Annotations.Add(maxFrameLine);
            PlotModel.Annotations.Add(currentFrameLine);
            NotifyCurrentFrameChanged();
        }
        // Avanza al siguiente frame
        public void increaseFrame()
        {
            if(currentFrameLine.X < maxFrameLine.X)
            {
                currentFrameLine.X = currentFrameLine.X + 1;
            }
            else
            {
                currentFrameLine.X = minFrameLine.X;
            }
            NotifyCurrentFrameChanged();
            PlotModel.InvalidatePlot(false);
        }
        private void NotifyCurrentFrameChanged()
        {
            if(PropertyChanged!= null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("currentFrame"));
            }
        }
        // Emite los datos de la currentFrameLine cuando se mueve manualmente
        private void emitCurrentFrame(CurrentFrameArgs e) { 
            if(currentFrameEvent != null)
            {
                currentFrameEvent?.Invoke(this, e);
            }
        } 
        public PlotModel PlotModel { get; private set; }
        // Devuelve el numero del frame actual
        public string currentFrame { 
            get
            {
                return ((int)currentFrameLine.X).ToString();
            } }
    }
}
