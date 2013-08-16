using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ttapiOrderBookWatcher
{
    using TradingTechnologies.TTAPI;

    public class TTAPIEvents : IDisposable
    {
        private TradingFrameworkUtility util;
        private UniversalLoginTTAPI apiInstance = null;
        private WorkerDispatcher workDispatch = null;
        private bool disposed = false;
        private object m_lock = new object();

        private string username = string.Empty;
        private string password = string.Empty;

        #region Constructors
        
        internal TTAPIEvents()
        {

        }

        #endregion

        /// <summary>
        /// Create and start the Dispatcher
        /// </summary>
        public void Start(TradingFrameworkUtility tfu)
        {
            util = tfu;
            username = Properties.Settings.Default.USER;
            password = Properties.Settings.Default.PASSWORD;

            util.dbug_write("user is: {0}", username);
            // Attach a WorkerDispatcher to the current thread
            workDispatch = Dispatcher.AttachWorkerDispatcher();
            workDispatch.BeginInvoke(new Action(Init));
            workDispatch.Run();
        }

        /// <summary>
        /// Initialize TT API
        /// </summary>
        public void Init()
        {
            // Use "Universal Login" Login Mode
            TTAPI.UniversalLoginModeDelegate ulDelegate = new
            TTAPI.UniversalLoginModeDelegate(ttApiInitComplete);
            TTAPI.CreateUniversalLoginTTAPI(Dispatcher.Current, ulDelegate);
        }


        /// <summary>
        /// Event notification for status of TT API initialization
        /// </summary>
        public void ttApiInitComplete(UniversalLoginTTAPI api, Exception ex)
        {
            if (ex == null)
            {
                Console.WriteLine("TT API Initialization Succeeded");
                // Authenticate your credentials
                apiInstance = api;
                apiInstance.AuthenticationStatusUpdate += new
                EventHandler<AuthenticationStatusUpdateEventArgs>(
                apiInstance_AuthenticationStatusUpdate);
                apiInstance.Authenticate(username, password);
            }
            else
            {
                Console.WriteLine("TT API Initialization Failed: {0}", ex.Message);
       
            }
        }

        /// <summary>
        /// Event notification for status of authentication
        /// </summary>
        public void apiInstance_AuthenticationStatusUpdate(object sender,
            AuthenticationStatusUpdateEventArgs e)
        {
            if (e.Status.IsSuccess)
            {
                Console.WriteLine("TT API User Authentication Succeeded: {0}", e.Status.StatusMessage);
                // Add code here to begin working with the TT API

            }
            else
            {
                Console.WriteLine("TT API User Authentication failed: {0}", e.Status.StatusMessage);
            
            }
        }


        /// <summary>
        /// Shuts down the TT API
        /// </summary>
        public void Dispose()
        {

            lock (m_lock)
            {
                if (!disposed)
                {
                    // Shutdown the TT API
                    if (apiInstance != null)
                    {
                        apiInstance.Shutdown();
                        apiInstance = null;
                    }
                    // Shutdown the Dispatcher
                    if (workDispatch != null)
                    {
                        workDispatch.BeginInvokeShutdown();
                        workDispatch = null;
                    }
                    disposed = true;
                }
            }
        }

    }
}
