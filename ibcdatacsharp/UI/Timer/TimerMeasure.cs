using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.ComponentModel;

namespace ibcdatacsharp.UI.Timer
{
    // Timer que puede bajar de 10 ms y de alta precision
    public class TimerMeasure
    {
        #region Timer Members

        #region Fields

        private Timer timer;
        private const int INIT_FRAME = 0;
        private int frame;
        private DateTime? initTime;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the time period has elapsed.
        /// </summary>
        public event EventHandler<FrameArgs> Tick;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Timer class with the specified IContainer.
        /// </summary>
        /// <param name="container">
        /// The IContainer to which the Timer will add itself.
        /// </param>
        public TimerMeasure(IContainer container)
        {
            timer = new Timer(container);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the Timer class.
        /// </summary>
        public TimerMeasure()
        {
            timer = new Timer();
            Initialize();
        }

        // Initialize timer with default values.
        private void Initialize()
        {
            frame = INIT_FRAME;

            timer.Tick += delegate (object sender, EventArgs e)
            {
                Tick?.Invoke(this, new FrameArgs { frame = frame, elapsed = DateTime.Now.Subtract((DateTime)initTime).TotalSeconds });
                frame++;
            };
        }

        #endregion

        #region Methods

        // Se llama al clicar el boton stop. Detiene el timer y reinicia la cuenta de tiempo.
        public void stopAndReset(object sender)
        {
            timer.Stop();
            frame = INIT_FRAME;
            initTime = null;
        }
        public void Start()
        {
            timer.Start();
            if(initTime == null)
            {
                initTime = DateTime.Now;
            }
        }
        // Se llama al clicar el boton pause. Pausa o inicia el timer
        public void onPause(object sender, PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                Stop();
            }
            else if (pauseState == PauseState.Play)
            {
                Start();
            }
        }
        /// <summary>
        /// Stops timer.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>
        public void Stop()
        {
            timer.Stop();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the object used to marshal event-handler calls.
        /// </summary>
        public ISynchronizeInvoke SynchronizingObject
        {
            get
            {
                return timer.SynchronizingObject;
            }
            set
            {
                timer.SynchronizingObject = value;
            }
        }

        /// <summary>
        /// Gets or sets the time between Tick events.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>   
        public int Period
        {
            get
            {
                return timer.Period;
            }
            set
            {
                timer.Period = value;
            }
        }

        /// <summary>
        /// Gets or sets the timer resolution.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>        
        /// <remarks>
        /// The resolution is in milliseconds. The resolution increases 
        /// with smaller values; a resolution of 0 indicates periodic events 
        /// should occur with the greatest possible accuracy. To reduce system 
        /// overhead, however, you should use the maximum value appropriate 
        /// for your application.
        /// </remarks>
        public int Resolution
        {
            get
            {
                return timer.Resolution;
            }
            set
            {
                timer.Resolution = value;
            }
        }

        /// <summary>
        /// Gets the timer mode.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// If the timer has already been disposed.
        /// </exception>
        public TimerMode Mode
        {
            get
            {
                return timer.Mode;
            }
            set
            {
                timer.Mode = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Timer is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return timer.IsRunning;
            }
        }

        /// <summary>
        /// Gets the timer capabilities.
        /// </summary>
        public static TimerCaps Capabilities
        {
            get
            {
                return Timer.Capabilities;
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Frees timer resources.
        /// </summary>
        public void Dispose()
        {
            timer.Dispose();
        }

        #endregion
    }
}
