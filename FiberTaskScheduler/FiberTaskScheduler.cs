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
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;

    public class FiberTaskScheduler
    {
        [ThreadStatic]
        internal static FiberTaskScheduler _scheduler;

        [ThreadStatic]
        internal static FiberTask _activeTask;

        private readonly ConcurrentQueue<FiberTask> _readyTasks = new ConcurrentQueue<FiberTask>();

        private readonly HashSet<FiberTask> _stoppedTasks = new HashSet<FiberTask>();

        private readonly Thread _thread;

        private uint _schedulerFiberId;

        public FiberTaskScheduler(FiberTask rootTask)
        {
            _thread = new Thread(Scheduler);
            _thread.Start(rootTask);
        }

        public void Wait()
        {
            _thread.Join();
        }

        internal void SwitchTask()
        {
            if (null != _activeTask)
            {
                switch (_activeTask.State)
                {
                    case FiberTaskState.Ready:
                        _readyTasks.Enqueue(_activeTask);
                        break;

                    case FiberTaskState.Stopped:
                        _stoppedTasks.Add(_activeTask);
                        break;
                }
            }

            uint nextFiberId = _readyTasks.TryDequeue(out _activeTask)
                                       ? _activeTask.FiberId
                                       : _schedulerFiberId;
            FiberUnmanaged.SwitchToFiber(nextFiberId);

            // we are back, check we were not killed meanwhile
            if (null != _activeTask && FiberTaskState.Killed == _activeTask.State)
            {
                Thread.CurrentThread.Abort();
            }
        }

        private void Scheduler(object data)
        {
            // schedule root task
            FiberTask rootTask = (FiberTask) data;
            _scheduler = this;

            _schedulerFiberId = FiberUnmanaged.ConvertThreadToFiber(0);

            rootTask.Start();
            SwitchTask();

            // exit
            FiberUnmanaged.ConvertFiberToThread();
        }

        internal void StartTask(FiberTask task)
        {
            _readyTasks.Enqueue(task);
        }

        internal void StopTask(FiberTask task)
        {
            _stoppedTasks.Add(task);
        }
    }
}