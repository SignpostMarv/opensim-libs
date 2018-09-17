/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HttpServer
{
    /// <summary>
    /// Timeout Manager.   Checks for dead clients.  Clients with open connections that are not doing anything.   Closes sessions opened with keepalive.
    /// </summary>
    public static class ContextTimeoutManager
    {
        /// <summary>
        /// Use a Thread or a Timer to monitor the ugly
        /// </summary>
        private static Thread m_internalThread = null;
//        private static readonly LocklessQueue<HttpClientContext> m_contexts = new LocklessQueue<HttpClientContext>();
        private static ConcurrentQueue<HttpClientContext> m_contexts = new ConcurrentQueue<HttpClientContext>();
        private static bool m_shuttingDown;
        private static int m_monitorMS = 1000;

        static ContextTimeoutManager()
        {
        }

        public static void StartMonitoring()
        {
            if(m_internalThread != null)
                return;
            m_internalThread = new Thread(ThreadRunProcess);
            m_internalThread.Priority = ThreadPriority.Normal;
            m_internalThread.IsBackground = true;
            m_internalThread.CurrentCulture = new CultureInfo("en-US", false);
            m_internalThread.Name = "HttpServer Timeout Checker";
            m_internalThread.Start();
        }

        public static void StopMonitoring()
        {
            m_shuttingDown = true;
            m_internalThread.Join();
            ProcessShutDown();
        }

        private static void TimerCallbackCheck(object o)
        {
            ProcessContextTimeouts();
        }

        private static void ThreadRunProcess(object o)
        {
            while (!m_shuttingDown)
            {
                ProcessContextTimeouts();
                Thread.Sleep(m_monitorMS);
            }
        }

        public static void ProcessShutDown()
        {
            try
            {
                SocketError disconnectError = SocketError.HostDown;
                for (int i = 0; i < m_contexts.Count; i++)
                {
                    HttpClientContext context = null;
                    if (m_contexts.TryDequeue(out context))
                    {
                        try
                        {
                            context.Disconnect(disconnectError);
                        }
                        catch { }
                    }
                }
            }
            catch (NullReferenceException)
            {
                // Lockless queue so something is null or disposed
            }
            catch (ObjectDisposedException)
            {
                // Lockless queue so something is null or disposed
            }
            catch (Exception)
            {
                // We can't let this crash.
            }
        }


        /// <summary>
        /// Causes the watcher to immediately check the connections. 
        /// </summary>
        public static void ProcessContextTimeouts()
        {
            try
            {
                for (int i = 0; i < m_contexts.Count; i++)
                {
                    HttpClientContext context = null;
                    if (m_contexts.TryDequeue(out context))
                    {
                        SocketError disconnectError = SocketError.InProgress;
                        bool disconnect;
                        if (!ContextTimedOut(context, out disconnectError, out disconnect))
                        {
                            m_contexts.Enqueue(context);
                        }
                        else
                        {
                            if (disconnect)
                            {
                                context.Disconnect(disconnectError);
                            }
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                // Lockless queue so something is null or disposed
            }
            catch (ObjectDisposedException)
            {
                // Lockless queue so something is null or disposed
            }
            catch (Exception)
            {
                // We can't let this crash.
            }
        }

        private static bool ContextTimedOut(HttpClientContext context, out SocketError disconnectError, out bool disconnect)
        {
            disconnect = false;
            disconnectError = SocketError.InProgress;

            // First our error conditions
            if (context == null)
                return true;

            if (context.Available)
                return true;

            // Next our special use conditions
            // Special case when multiple client contexts are being responded to by a single thread
            //if (context.EndWhenDone)
            //{
            //    stopMonitoring = true;
            //    return true;
            //}

            // Special case for websockets
            if (context.StreamPassedOff)
                return true;

            // Now for the case when the context has the stop monitoring bool set
            if (context.StopMonitoring)
                return true;

            // Now we start checking for actual timeouts

            // First we check that we got at least one line within context.TimeoutFirstLine milliseconds
            if (!context.FirstRequestLineReceived)
            {
                if (EnvironmentTickCountAdd(context.TimeoutFirstLine, context.MonitorStartMS) <= EnvironmentTickCount())
                {
                    disconnectError = SocketError.TimedOut;
                    disconnect = true;
                    context.MonitorStartMS = 0;
                    return true;
                }
            }

            //
            if (!context.FullRequestReceived)
            {
                if (EnvironmentTickCountAdd(context.TimeoutRequestReceived, context.MonitorStartMS) <= EnvironmentTickCount())
                {
                    disconnectError = SocketError.TimedOut;
                    context.MonitorStartMS = 0;
                    return true;
                }
            }

            // 
            if (!context.FullRequestProcessed)
            {
                if (EnvironmentTickCountAdd(context.TimeoutFullRequestProcessed, context.MonitorStartMS) <= EnvironmentTickCount())
                {
                    disconnectError = SocketError.TimedOut;
                    disconnect = true;
                    context.MonitorStartMS = 0;
                    return true;
                }
            }

            if (context.TriggerKeepalive)
            {
                context.TriggerKeepalive = false;
                context.MonitorKeepaliveMS = EnvironmentTickCount();
            }

            if (context.FullRequestProcessed && context.MonitorKeepaliveMS == 0)
                return true;

            if (context.MonitorKeepaliveMS != 0 &&
                EnvironmentTickCountAdd(context.TimeoutKeepAlive, context.MonitorKeepaliveMS) <= EnvironmentTickCount())
            {
                disconnectError = SocketError.TimedOut;
                context.MonitorStartMS = 0;
                disconnect = true;
                context.MonitorKeepaliveMS = 0;
                return true;
            }

            return false;
        }

        public static void StartMonitoringContext(HttpClientContext context)
        {
            context.MonitorStartMS = EnvironmentTickCount();
            m_contexts.Enqueue(context);
        }

        /// <summary>
        /// Environment.TickCount is an int but it counts all 32 bits so it goes positive
        /// and negative every 24.9 days. This trims down TickCount so it doesn't wrap
        /// for the callers. 
        /// This trims it to a 12 day interval so don't let your frame time get too long.
        /// </summary>
        /// <returns></returns>
        public static Int32 EnvironmentTickCount()
        {
            return Environment.TickCount & EnvironmentTickCountMask;
        }
        const Int32 EnvironmentTickCountMask = 0x3fffffff;

        /// <summary>
        /// Environment.TickCount is an int but it counts all 32 bits so it goes positive
        /// and negative every 24.9 days. Subtracts the passed value (previously fetched by
        /// 'EnvironmentTickCount()') and accounts for any wrapping.
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="prevValue"></param>
        /// <returns>subtraction of passed prevValue from current Environment.TickCount</returns>
        public static Int32 EnvironmentTickCountSubtract(Int32 newValue, Int32 prevValue)
        {
            Int32 diff = newValue - prevValue;
            return (diff >= 0) ? diff : (diff + EnvironmentTickCountMask + 1);
        }

        /// <summary>
        /// Environment.TickCount is an int but it counts all 32 bits so it goes positive
        /// and negative every 24.9 days. Subtracts the passed value (previously fetched by
        /// 'EnvironmentTickCount()') and accounts for any wrapping.
        /// </summary>
        /// <param name="newValue"></param>
        /// <param name="prevValue"></param>
        /// <returns>subtraction of passed prevValue from current Environment.TickCount</returns>
        public static Int32 EnvironmentTickCountAdd(Int32 newValue, Int32 prevValue)
        {
            Int32 ret = newValue + prevValue;
            return (ret >= 0) ? ret : (ret + EnvironmentTickCountMask + 1);
        }
        /// <summary>
        /// Environment.TickCount is an int but it counts all 32 bits so it goes positive
        /// and negative every 24.9 days. Subtracts the passed value (previously fetched by
        /// 'EnvironmentTickCount()') and accounts for any wrapping.
        /// </summary>
        /// <returns>subtraction of passed prevValue from current Environment.TickCount</returns>
        public static Int32 EnvironmentTickCountSubtract(Int32 prevValue)
        {
            return EnvironmentTickCountSubtract(EnvironmentTickCount(), prevValue);
        }

        // Returns value of Tick Count A - TickCount B accounting for wrapping of TickCount
        // Assumes both tcA and tcB came from previous calls to Util.EnvironmentTickCount().
        // A positive return value indicates A occured later than B
        public static Int32 EnvironmentTickCountCompare(Int32 tcA, Int32 tcB)
        {
            // A, B and TC are all between 0 and 0x3fffffff
            int tc = EnvironmentTickCount();

            if (tc - tcA >= 0)
                tcA += EnvironmentTickCountMask + 1;

            if (tc - tcB >= 0)
                tcB += EnvironmentTickCountMask + 1;

            return tcA - tcB;
        }
    }
}
