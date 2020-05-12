using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace CLFeedPull.NetCore
{
    #region EventTraceListener

    #region TraceEventArgs
    /// <summary>
    /// Event arguments containing trace data.
    /// </summary>
    public class TraceEventArgs : EventArgs
    {
        /// <summary>
        /// Trace data that triggered the event.
        /// </summary>
        public string Data { get; protected set; }

        /// <summary>
        /// </summary>
        public TraceEventArgs(string data)
        {
            Data = data;
        }
    }
    #endregion

    #region TraceEventHandler
    /// <summary>
    /// Delegate type for trace events.
    /// </summary>
    public delegate void TraceEventHandler(object sender, TraceEventArgs e);
    #endregion

    /// <summary>
    /// A trace listener that listens for trace data, and raises an event
    /// when it receives any, so that we can hook up external code to handle it.
    /// </summary>
    public class EventTraceListener : TraceListener
    {
        #region MessageWritten
        /// <summary>
        /// An event that's raised whenever a message is written and
        /// caught by this listener.
        /// </summary>
        public event TraceEventHandler MessageWritten;
        #endregion

        #region Write
        /// <summary>
        /// Handle message writing.
        /// </summary>
        public override void Write(string message)
        {
            if (MessageWritten != null)
                MessageWritten(this, new TraceEventArgs(message));
        }
        #endregion

        #region WriteLine
        /// <summary>
        /// Handle message writing.
        /// </summary>
        public override void WriteLine(string message)
        {
            if (MessageWritten != null)
                MessageWritten(this, new TraceEventArgs(message
                    + Environment.NewLine));
        }
        #endregion

        #region Create
        /// <summary>
        /// A simple helper method to create a listener with an event pre-registered
        /// for receiving trace data.
        /// </summary>
        /// <param name="lineAction">Simplified action to perform when trace data
        /// is received.</param>
        /// <returns>The newly-created listener, NOT yet registered.</returns>
        public static EventTraceListener Create(Action<string> lineAction)
        {
            EventTraceListener TL = new EventTraceListener();
            TL.MessageWritten += (traceSender, tracee) => lineAction.Invoke(tracee.Data);
            return TL;
        }
        #endregion

        #region DebugLineFormat
        private static object ThreadLenLock = new object();
        private static int ThreadLen = 0;
        private static object PrevDateLock = new object();
        private static DateTime PrevDate = DateTime.MinValue;
        private static string ProcessID = "[" + Process.GetCurrentProcess().Id.ToString("x") + "]";
        /// <summary>
        /// Format a line of text for debug log display.
        /// </summary>
        private static void DebugLineFormat(string msg,
            Action<string> lineAction,
            bool includeTimestamps = true,
            bool includeThreadID = true,
            bool includeProcessID = false,
            bool includeMachineName = false)
        {
            List<string> Prefixes = new List<string>();

            // Include Thread ID.  Use the minimum number of digits necessary to display
            // any thread ID seen so far; increase this number as needed, but never
            // decrease it, so fixed-width text lines up nicely for console apps.
            if (includeThreadID)
            {
                string TID = Thread.CurrentThread.ManagedThreadId.ToString("x");
                int TL = TID.Length;
                lock (ThreadLenLock)
                {
                    if (TL > ThreadLen)
                        ThreadLen = TL;
                    if (ThreadLen > TL)
                        TL = ThreadLen;
                }
                if (TID.Length < TL)
                    TID = new string('0', TL - TID.Length) + TID;
                Prefixes.Insert(0, "[" + TID + "]");
            }

            // Include the pre-computed process ID; these don't change within
            // a static context.
            if (includeProcessID)
                Prefixes.Insert(0, ProcessID);

            // Include the machine name if requested.
            if (includeMachineName)
                Prefixes.Insert(0, "[" + Environment.MachineName + "]");

            // If timestamps are requested, include the time of day in each line, and if the
            // date changes during runtime, add a line to display the new date.
            if (includeTimestamps)
            {
                DateTime Now = DateTime.Now;
                bool NeedDate = false;
                lock (PrevDateLock)
                    if (Now.Date != PrevDate)
                    {
                        NeedDate = true;
                        PrevDate = Now.Date;
                    }
                if (NeedDate)
                    lineAction.Invoke("========== " + DateTime.Now.ToString("yyyy-MM-dd ddd") + " ==========");
                Prefixes.Insert(0, DateTime.Now.ToString("[HH:mm:ss.fff]"));
            }

            // Split the content by lines and invoke the action for each line.
            string AllPrefs = Prefixes.Any()
                ? (string.Join(" ", Prefixes) + " ")
                : string.Empty;
            foreach (string Line in msg.Split(Environment.NewLine.ToCharArray())
                .Where(X => !string.IsNullOrWhiteSpace(X))
                .Select(X => AllPrefs + X))
                lineAction.Invoke(Line);
        }
        #endregion

        #region CreateFormatted
        /// <summary>
        /// Create a listener that receives "formatted" messages, intended for human-readable
        /// console output.
        /// </summary>
        /// <param name="lineAction">Action to perform on each line of text, such as piping out
        /// to the Console, or appending to a WinForms textbox, or writing to a file.</param>
        /// <param name="includeTimestamps">If true (default), include a timestamp on each line.</param>
        /// <param name="includeThreadID">If true (default), include a Thread ID on each line so
        /// that parallel processes can be disambiguated in traces.</param>
        /// <param name="includeProcessID">If true, include the process ID on each line, in the
        /// event that multiple processes are multiplexed in the same stream.</param>
        /// <param name="includeMachineName">If true, include the Machine Name on each line, in the
        /// event that processes from multiple machines are multiplexed in the same stream.</param>
        /// <returns>The newly-created listener, NOT yet registered.</returns>
        public static EventTraceListener CreateFormatted(Action<string> lineAction,
            bool includeTimestamps = true,
            bool includeThreadID = true,
            bool includeProcessID = false,
            bool includeMachineName = false)
        {
            return Create(L => DebugLineFormat(L, lineAction, includeTimestamps, includeThreadID, includeProcessID, includeMachineName));
        }
        #endregion

        #region Register
        /// <summary>
        /// Register an event listener to intercept System.Diagnostics.Trace trace lines.
        /// </summary>
        /// <returns>The just-registered listener.</returns>
        public EventTraceListener Register()
        {
            if (!Trace.Listeners.Contains(this))
                Trace.Listeners.Add(this);
            return this;
        }
        #endregion

        #region Unregister
        /// <summary>
        /// Unregister an event listener to intercept System.Diagnostics.Trace trace lines.
        /// </summary>
        /// <returns>The just-unregistered listener.</returns>
        public EventTraceListener Unregister()
        {
            if (Trace.Listeners.Contains(this))
                Trace.Listeners.Remove(this);
            return this;
        }
        #endregion
    }

    #endregion

    public static class Utilities
    {
        #region Solo
        /// <summary>
        /// Wraps a single object into an IEnumerable of that
        /// type, so single results can be returned from "Get
        /// List" functions, concatenated onto other Enumerables,
        /// etc.
        /// </summary>
        public static IEnumerable<T> Solo<T>(this T input)
        {
            if (input == null)
                return new T[0];
            return new T[] { input };
        }
        #endregion

        #region QueueRecurse
        /// <summary>
        /// Traverse an object graph, using queue recursion (no use of call stack, breadth-first
        /// order).  This method supports cyclic graphs, providing a custom method to determine
        /// a unique "key" for each node; nodes with the same key will not be visited more than once.
        /// </summary>
        /// <typeparam name="TItem">Type of nodes to traverse in graph.</typeparam>
        /// <typeparam name="TKey">Type of key for "visited" list.</typeparam>
        /// <param name="root">Root node from which to start traversal.  It will be included
        /// in the returned set.</param>
        /// <param name="getPeerFunc">Function to get all "peers" (or children, if such a
        /// logical separation exists) of a given node.  These will be added to the queue
        /// to be returned.</param>
        /// <param name="getKey">Get a "key" identifying a node, to prevent visiting the
        /// same node more than once.</param>
        /// <param name="visited">Optionally supply a pre-populated "visited" hashset to skip
        /// certain nodes during traversal.</param>
        /// <returns>Enumerator for traversing the graph as specified.</returns>
        public static IEnumerable<TItem> QueueRecurse<TItem, TKey>(this TItem root,
            Func<TItem, IEnumerable<TItem>> getPeerFunc, Func<TItem, TKey> getKey, HashSet<TKey> visited = null)
        {
            if (visited == null)
                visited = new HashSet<TKey>();
            Queue<TItem> ProcQueue = new Queue<TItem>();
            ProcQueue.Enqueue(root);
            while (ProcQueue.Count > 0)
            {
                TItem U = ProcQueue.Dequeue();

                if (getKey != null)
                {
                    TKey K = getKey.Invoke(U);
                    if (visited.Contains(K))
                        continue;
                    visited.Add(K);
                }

                yield return U;

                IEnumerable<TItem> Children = getPeerFunc.Invoke(U);
                if (Children != null)
                    foreach (TItem X in Children)
                        ProcQueue.Enqueue(X);
            }
        }
        /// <summary>
        /// Traverse an object graph, using queue recursion (no use of call stack, breadth-first
        /// order).  Simplified support cyclic graphs, using the nodes themselves as keys to prevent
        /// multiple visits (using IEquatable or reference equality).
        /// </summary>
        /// <typeparam name="TItem">Type of nodes to traverse in graph.</typeparam>
        /// <param name="root">Root node from which to start traversal.  It will be included
        /// in the returned set.</param>
        /// <param name="getPeerFunc">Function to get all "peers" (or children, if such a
        /// logical separation exists) of a given node.  These will be added to the queue
        /// to be returned.</param>
        /// <param name="visited">Optionally supply a pre-populated "visited" hashset to skip
        /// certain nodes during traversal.</param>
        /// <returns>Enumerator for traversing the graph as specified.</returns>
        public static IEnumerable<TItem> QueueRecurse<TItem>(this TItem root, Func<TItem, IEnumerable<TItem>> getPeerFunc,
            HashSet<TItem> visited = null)
        {
            return QueueRecurse(root, getPeerFunc, X => X, visited);
        }
        #endregion

        #region ExceptionDump
        /// <summary>
        /// Add a line to an exception dump, but only if the value is not empty.
        /// </summary>
        private static void ExceptionDumpAddLine(StringBuilder sb, string tag, object line)
        {
            if (line == null)
                return;
            string Str = line.ToString();
            if (string.IsNullOrWhiteSpace(Str))
                return;
            sb.Append(tag);
            sb.Append(": ");
            sb.AppendLine(Str);
        }
        /// <summary>
        /// Converts an exception to a human readable dump of as much information
        /// as possible from the generic exception type, including inner exceptions.
        /// </summary>
        public static string ExceptionDump(this Exception ex)
        {
            StringBuilder SB = new StringBuilder();
            foreach (Exception e in ex.QueueRecurse(X =>
               (X is AggregateException)
                   ? ((AggregateException)X).InnerExceptions.Union(X.InnerException.Solo())
                   : X.InnerException.Solo()))
            {
                if (SB.Length > 0)
                    SB.AppendLine();
                ExceptionDumpAddLine(SB, "Type", e.GetType().FullName);
                ExceptionDumpAddLine(SB, "Message", e.Message);
                ExceptionDumpAddLine(SB, "HelpLink", e.HelpLink);
                ExceptionDumpAddLine(SB, "Source", e.Source);
                //ExceptionDumpAddLine(SB, "TargetSite", e.TargetSite);
                ExceptionDumpAddLine(SB, "StackTrace", e.StackTrace);
            }
            return SB.ToString();
        }
        #endregion

        #region ExecuteWrapped

        /// <summary>
        /// A text line that's written to the debug stream before dumping exception
        /// details, for any exception that's caught ExecuteWrapped.
        /// </summary>
        private const string EXCEPTION_DEBUG_LINE = "<<< UNHANDLED EXCEPTION >>>";

        /// <summary>
		/// A wrapper for the Main function of a console application.  It
		/// will execute the passed-in code with proper message trapping, display
		/// trace and message information to the console, and return the correct
		/// return value for Main, based on whether a fatal error occurred.
		/// </summary>
		/// <param name="mainProcess">The code to actually run (in essence,
		/// the body of the entire console application).</param>
		/// <returns>The value Main() should return for the process: 0 if
		/// all is well, and 1 if an error occurred.</returns>
		public static int ExecuteWrapped(Action mainProcess)
        {
            EventTraceListener DebugListener = EventTraceListener.CreateFormatted(L => Console.WriteLine(L));

            try
            {
                DebugListener.Register();

                try
                {
                    mainProcess.Invoke();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Empty);
                    Trace.WriteLine(EXCEPTION_DEBUG_LINE);
                    Trace.WriteLine(ex.ExceptionDump());
                    return 1;
                }

                return 0;
            }
            finally
            {
                DebugListener.Unregister();
            }
        }

        #endregion
    }
}
