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
using System.Collections.Generic;
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
    public sealed class ContextTimeoutManager
    {
        /// <summary>
        /// Use a Thread or a Timer to monitor the ugly
        /// </summary>
        public enum MonitorType
        {
            Thread,
            Timer
        }
        
        private MonitorType _monitoringType;

        private Timer _internalTimer = null;

        private Thread _internalThread = null;

        private readonly LocklessQueue<HttpClientContext> _contexts;

        private bool _shuttingDown;

        private int _monitorMS = 1000;

        public ContextTimeoutManager(MonitorType pMonitorType)
        {
            _contexts = new LocklessQueue<HttpClientContext>();
            _monitoringType = pMonitorType;
            switch (pMonitorType)
            {
                case MonitorType.Thread:
                    _internalThread = new Thread(ThreadRunProcess);
                    _internalThread.Priority = ThreadPriority.Lowest;
                    _internalThread.IsBackground = true;
                    _internalThread.CurrentCulture = new CultureInfo("en-US", false);
                    _internalThread.Name = "HttpServer Internal Zombie Timeout Checker";
                    _internalThread.Start();
                    break;
                case MonitorType.Timer:
                    _internalTimer = new Timer(TimerCallbackCheck, null, _monitorMS, _monitorMS);
                    
                    break;
            }
        }

        
        public ContextTimeoutManager(MonitorType pMonitorType, int pMonitorMs) : this(pMonitorType)
        {
            _monitorMS = pMonitorMs;
        }
        

        public void StopMonitoring()
        {
            _shuttingDown = true;
            switch (_monitoringType)
            {
                case MonitorType.Timer:
                    _internalTimer.Dispose();
                    break;
                case MonitorType.Thread:
                    _internalThread.Join();
                    break;
            }
        }

        private void TimerCallbackCheck(object o)
        {
            ProcessContextTimeouts();
        }

        private void ThreadRunProcess(object o)
        {
            while (!_shuttingDown)
            {
                ProcessContextTimeouts();
                Thread.Sleep(_monitorMS);
            }
        }

        /// <summary>
        /// Causes the watcher to immediately check the connections. 
        /// </summary>
        public void ProcessContextTimeouts()
        {
            try
            {
                for (int i = 0; i < _contexts.Count; i++)
                {
                    HttpClientContext context = null;
                    if (_contexts.TryDequeue(out context))
                    {
                        SocketError disconnectError = SocketError.InProgress;
                        bool stopMonitoring;
                        if (!ContextTimedOut(context, out disconnectError, out stopMonitoring))
                        {
                            _contexts.Enqueue(context);
                        }
                        else
                        {
                            if (!stopMonitoring)
                            {

                                context.Disconnect(disconnectError);
                                context.Cleanup();
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

        private bool ContextTimedOut(HttpClientContext context, out SocketError disconnectError, out bool stopMonitoring)
        {
            stopMonitoring = false;
            disconnectError = SocketError.InProgress;

            // First our error conditions
            if (context == null)
            {
                stopMonitoring = true;
                return true;
            }

            // Recycled context
            if (context.Available)
            {
                stopMonitoring = true;
                return true;
            }

            // Next our special use conditions
            // Special case when multiple client contexts are being responded to by a single thread
            //if (context.EndWhenDone)
            //{
            //    stopMonitoring = true;
            //    return true;
            //}

            // Special case for websockets
            if (context.StreamPassedOff)
            {
                stopMonitoring = true;
                return true;
            }

            // Now for the case when the context has the stop monitoring bool set
            if (context.StopMonitoring)
            {
                stopMonitoring = true;
                return true;
            }

            // Now we start checking for actual timeouts

            // First we check that we got at least one line within context.TimeoutFirstLine milliseconds
            if (!context.FirstRequestLineReceived)
            {
                if (EnvironmentTickCountAdd(context.TimeoutFirstLine, context.MonitorStartMS) <= EnvironmentTickCount())
                {
                    disconnectError = SocketError.TimedOut;
                    context.MonitorStartMS = 0;
                    return true;
                }
            }

            // First we check that we got at least one line within context.TimeoutFirstLine milliseconds
            if (!context.FullRequestReceived)
            {
                if (EnvironmentTickCountAdd(context.TimeoutRequestReceived, context.MonitorStartMS) <= EnvironmentTickCount())
                {
                    disconnectError = SocketError.TimedOut;
                    context.MonitorStartMS = 0;
                    return true;
                }
            }

            // First we check that we got at least one line within context.TimeoutFirstLine milliseconds
            if (!context.FullRequestProcessed && !context.EndWhenDone)
            {
                if (EnvironmentTickCountAdd(context.TimeoutFullRequestProcessed, context.MonitorStartMS) <= EnvironmentTickCount())
                {
                    disconnectError = SocketError.TimedOut;
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
            {
                stopMonitoring = true;
                return true;
            }

            if (context.MonitorKeepaliveMS != 0 &&
                EnvironmentTickCountAdd(context.TimeoutKeepAlive, context.MonitorKeepaliveMS) <= EnvironmentTickCount())
            {
                disconnectError = SocketError.TimedOut;
                context.MonitorStartMS = 0;
                context.MonitorKeepaliveMS = 0;
                return true;
            }

            return false;
        }

        public void StartMonitoringContext(HttpClientContext context)
        {
            context.MonitorStartMS = EnvironmentTickCount();
            _contexts.Enqueue(context);
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
