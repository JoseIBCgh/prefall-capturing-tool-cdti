using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ibcdatacsharp.UI.Timer
{
    // Timer que puede bajar de 10 ms y de alta precision
    public class TimerMeasure : Timer
    {
        #region Timer Members

        #region Delegates

        private delegate void FrameEventRaiser(FrameArgs e);

        #endregion

        #region Fields

        private const int INIT_FRAME = 0;
        private int frame;
        private Stopwatch stopwatch;

        // Represents the method that raises the Tick event.
        private FrameEventRaiser tickRaiser;
        #endregion

        #region Events

        /// <summary>
        /// Occurs when the Timer has started;
        /// </summary>
        public new event EventHandler Started;

        /// <summary>
        /// Occurs when the Timer has stopped;
        /// </summary>
        public new event EventHandler Stopped;
        /// <summary>
        /// Occurs when the time period has elapsed.
        /// </summary>
        public new event EventHandler<FrameArgs> Tick;

        #endregion

        #region Construction

        /// <summary>
        /// Initialize class.
        /// </summary>
        static TimerMeasure()
        {
            // Get multimedia timer capabilities.
            timeGetDevCaps(ref caps, Marshal.SizeOf(caps));
        }

        /// <summary>
        /// Initializes a new instance of the Timer class with the specified IContainer.
        /// </summary>
        /// <param name="container">
        /// The IContainer to which the Timer will add itself.
        /// </param>
        public TimerMeasure(IContainer container): base(container)
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the Timer class.
        /// </summary>
        public TimerMeasure(): base()
        {
            
        }

        // Initialize timer with default values.
        protected void Initialize()
        {
            this.mode = TimerMode.Periodic;
            this.period = Capabilities.periodMin;
            this.resolution = 1;

            running = false;

            frame = INIT_FRAME;
            stopwatch = new Stopwatch();

            timeProcPeriodic = new TimeProc(TimerPeriodicEventCallback);
            timeProcOneShot = new TimeProc(TimerOneShotEventCallback);
            tickRaiser = new FrameEventRaiser(OnTick);
        }

        #endregion

        #region Methods

        // Se llama al clicar el boton stop. Detiene el timer y reinicia la cuenta de tiempo.
        public void stopAndReset(object sender)
        {
            Stop();
            stopwatch.Reset();
            frame = INIT_FRAME;
        }
        /// <summary>
        /// Stops timer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>
        public new void Stop()
        {
            #region Require

            if (disposed)
            {
                throw new ObjectDisposedException("Timer");
            }

            #endregion

            #region Guard

            if (!running)
            {
                return;
            }

            #endregion

            // Stop and destroy timer.
            int result = timeKillEvent(timerID);

            Debug.Assert(result == TIMERR_NOERROR);

            running = false;
            stopwatch.Stop();

            if (SynchronizingObject != null && SynchronizingObject.InvokeRequired)
            {
                SynchronizingObject.BeginInvoke(
                    new EventRaiser(OnStopped),
                    new object[] { EventArgs.Empty });
            }
            else
            {
                OnStopped(EventArgs.Empty);
            }
        }

        #region Callbacks

        // Callback method called by the Win32 multimedia timer when a timer
        // periodic event occurs.
        protected void TimerPeriodicEventCallback(int id, int msg, int user, int param1, int param2)
        {
            FrameArgs frameArgs = new FrameArgs { frame = frame, elapsed = stopwatch.Elapsed.TotalMilliseconds };
            if (synchronizingObject != null)
            {
                synchronizingObject.BeginInvoke(tickRaiser, new object[] { frameArgs });
            }
            else
            {
                OnTick(frameArgs);
            }
            frame++;
        }

        // Callback method called by the Win32 multimedia timer when a timer
        // one shot event occurs.
        protected void TimerOneShotEventCallback(int id, int msg, int user, int param1, int param2)
        {
            FrameArgs frameArgs = new FrameArgs { frame = frame, elapsed = stopwatch.Elapsed.TotalMilliseconds };
            if (synchronizingObject != null)
            {
                synchronizingObject.BeginInvoke(tickRaiser, new object[] { frameArgs });
                Stop();
            }
            else
            {
                OnTick(frameArgs);
                Stop();
            }
            frame++;
        }

        #endregion

        #region Event Raiser Methods

        // Raises the Disposed event.
        protected void OnDisposed(EventArgs e)
        {
            EventHandler handler = Disposed;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        // Raises the Started event.
        protected void OnStarted(EventArgs e)
        {
            stopwatch.Start();

            EventHandler handler = Started;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        // Raises the Stopped event.
        protected void OnStopped(EventArgs e)
        {
            EventHandler handler = Stopped;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        // Raises the Tick event.
        protected void OnTick(FrameArgs e)
        {
            EventHandler<FrameArgs> handler = Tick;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #endregion

        #endregion

        #region IComponent Members

        public new event System.EventHandler Disposed;

        #endregion
    }
}
