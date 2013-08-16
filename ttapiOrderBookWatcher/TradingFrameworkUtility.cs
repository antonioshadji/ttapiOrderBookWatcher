using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;


namespace ttapiOrderBookWatcher
{
    public class TradingFrameworkUtility
    {
        public TextWriterTraceListener log;

        //constructor
        public TradingFrameworkUtility()
        {
            TextWriterTraceListener tr1 = new TextWriterTraceListener(System.Console.Out);
            Debug.Listeners.Add(tr1);

            log = new TextWriterTraceListener(System.IO.File.CreateText(Process.GetCurrentProcess().ProcessName + ".log"));
            Debug.Listeners.Add(log);
        }

        public void prod_write(string text, params object[] data)
        {
            Trace.WriteLine(string.Format(text, data));
            log.Flush();
        }

        public void dbug_write(string text, params object[] data)
        {
            Debug.WriteLine(string.Format(text, data));
            log.Flush();
        }
    }
}
