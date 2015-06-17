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

namespace TestFiberTaskScheduler
{
    using System;
    using FiberTaskScheduler;

    public class Task1 : FiberTask
    {
        protected override void Run()
        {
            Console.WriteLine("Hello Task1 !");

            FiberTask task2 = new Task2();
            task2.Start();

            for (int i = 0; i < 10; ++i)
            {
                Console.WriteLine("Task1: {0}", i);
                Yield();

                if (i == 5)
                {
                    task2.Kill();
                }
            }
        }
    }

    public class Task2 : FiberTask
    {
        protected override void Run()
        {
            Console.WriteLine("Hello Task2 !");

            for (int i = 10; i < 20; ++i)
            {
                Console.WriteLine("Task2: {0}", i);
                Yield();
            }
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            FiberTask rootTask = new Task1();
            FiberTaskScheduler fiberTaskScheduler = new FiberTaskScheduler(rootTask);

            fiberTaskScheduler.Wait();
        }
    }
}