// // FiberThreadScheduler - User land scheduler based on fibers
// // Copyright (c) 2013 Pierre Chalamet
// // 
// // Licensed under the Apache License, Version 2.0 (the "License");
// // you may not use this file except in compliance with the License.
// // You may obtain a copy of the License at
// // 
// // http://www.apache.org/licenses/LICENSE-2.0
// // 
// // Unless required by applicable law or agreed to in writing, software
// // distributed under the License is distributed on an "AS IS" BASIS,
// // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// // See the License for the specific language governing permissions and
// // limitations under the License.

namespace FiberTaskScheduler
{
    using System.Threading;

    public abstract class FiberTask
    {
        private FiberTaskScheduler _scheduler;

        protected FiberTask()
        {
            State = FiberTaskState.Unknown;
            FiberUnmanaged.FiberProc fiberProc = _ => EntryPoint();
            FiberId = FiberUnmanaged.CreateFiber(0, fiberProc, 0);
        }

        internal FiberTaskState State { get; private set; }

        internal uint FiberId { get; private set; }

        public void Start()
        {
            if (State == FiberTaskState.Unknown)
            {
                _scheduler = FiberTaskScheduler._scheduler;
            }

            State = FiberTaskState.Ready;
            _scheduler.StartTask(this);
        }

        public void Stop()
        {
            if (State == FiberTaskState.Unknown || State == FiberTaskState.Killed)
            {
                return;
            }

            State = FiberTaskState.Stopped;
            _scheduler.StopTask(this);
        }

        public void Kill()
        {
            if (State == FiberTaskState.Unknown || State == FiberTaskState.Killed)
            {
                return;
            }

            State = FiberTaskState.Killed;
            if (State == FiberTaskState.Stopped)
            {
                _scheduler.StartTask(this);
            }
        }

        protected void Yield()
        {
            _scheduler.SwitchTask();
        }

        internal void EntryPoint()
        {
            try
            {
                Run();
            }
            catch (ThreadAbortException)
            {
                Thread.ResetAbort();
            }
                    // ReSharper disable EmptyGeneralCatchClause
            catch
                    // ReSharper restore EmptyGeneralCatchClause
            {
            }

            _scheduler.SwitchTask();
        }

        protected abstract void Run();
    }
}