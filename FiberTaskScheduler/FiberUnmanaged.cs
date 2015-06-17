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
    using System.Runtime.InteropServices;

    internal class FiberUnmanaged
    {
        public delegate void FiberProc(uint lpParameter);

        [DllImport("Kernel32.dll")]
        public static extern uint ConvertThreadToFiber(uint lpParameter);

        [DllImport("Kernel32.dll")]
        public static extern uint ConvertFiberToThread();

        //[DllImport("Kernel32.dll")]
        //public static extern uint ConvertThreadToFiberEx(uint lpParameter, uint dwFlags);

        [DllImport("Kernel32.dll")]
        public static extern void SwitchToFiber(uint lpFiber);

        [DllImport("Kernel32.dll")]
        public static extern void DeleteFiber(uint lpFiber);

        [DllImport("Kernel32.dll")]
        public static extern uint CreateFiber(uint dwStackSize, FiberProc lpStartAddress, uint lpParameter);

        [DllImport("Kernel32.dll")]
        public static extern bool IsThreadAFiber();

        //[DllImport("Kernel32.dll")]
        //public static extern uint CreateFiberEx(uint dwStackCommitSize, uint dwStackReserveSize, uint dwFlags, LPFIBER_START_ROUTINE lpStartAddress, uint lpParameter);
    }
}