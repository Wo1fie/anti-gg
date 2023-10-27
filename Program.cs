using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Anti_GG
{
    internal class Program
    {
        static bool running = true;
        static TaskCompletionSource taskCompletionSource;
        #region Imports
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(int dwDesiredAccess, bool bInheritHandle, int dwThreadId);
        [DllImport("kernel32.dll")]
        static extern int SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(Keys vKey);
        #endregion
        enum Keys : int { NumPad0 = 0x60, NumPad1 = 0x61 }; // https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
        static Task Main(string[] args)
        {
            taskCompletionSource = new TaskCompletionSource();
            StartAntiGG();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                Console.WriteLine("Cancellation requested...");
                running = false;
                Thread.Sleep(500); //wait to allow thread to end and GG to be released
                taskCompletionSource.SetResult();
            };
            return taskCompletionSource.Task;
        }
        static void StartAntiGG()
        {
            Thread T = new Thread(AntiGG) { IsBackground = true };
            T.Start();
        }
        static bool Paused = false;
        static DateTime Last = DateTime.Now;
        static void AntiGG()
        {
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Process process = null;
            Process[] processes = Process.GetProcessesByName("GameMon");
            if (processes.Length > 0)
            {
                process = processes[0];
                Console.WriteLine("GameGuard detected.  Crippling GameGuard.");
                for (int counter = 0; running; Thread.Sleep(10),counter++)
                {
                    if (!Paused)
                        PauseProcess(process);
                    if (DateTime.Now.Subtract(Last).TotalMinutes >= 1)
                        UnpauseProcess(process);
                    if(counter >= 5)
                    {
                        ProcessKeyInput(process);
                        counter = 0;
                    }
                }
                //Unpause if running is ended
                Console.WriteLine("Program ending, releasing GameGuard.");
                UnpauseProcess(process);
            }
            else
            {
                Console.WriteLine("GameGuard not detected.  Closing.");
                running = false;
                taskCompletionSource.SetResult();
            }
        }
        static void UnpauseProcess(Process process)
        {
            Paused = false;
            foreach (ProcessThread threadId in process.Threads)
            {
                IntPtr threadHandle = OpenThread(0x0002, false, threadId.Id);
                ResumeThread(threadHandle);
                CloseHandle(threadHandle);
            }
        }
        static void PauseProcess(Process process)
        {
            Last = DateTime.Now;
            Paused = true;
            foreach (ProcessThread threadId in process.Threads)
            {
                IntPtr threadHandle = OpenThread(0x0002, false, threadId.Id);
                SuspendThread(threadHandle);
                CloseHandle(threadHandle);
            }
        }
        static void ProcessKeyInput(Process process)
        {
            if (GetAsyncKeyState(Keys.NumPad0) == -32767)
            {
                UnpauseProcess(process);
                Console.WriteLine("Manual unpause.");
                Thread.Sleep(100);
            }
        }
    }
}
