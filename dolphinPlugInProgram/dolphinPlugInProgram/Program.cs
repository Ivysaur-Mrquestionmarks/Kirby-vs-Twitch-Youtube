using CurlDotNet;
using CurlDotNet.Core;
using Dolphin.Memory.Access;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;


namespace dolphinPlugInProgram //uwu
{
    internal class Program
    {


        


        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
  byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesWritten);

        const int PROCESS_VM_WRITE = 0x0020;
        const int PROCESS_VM_OPERATION = 0x0008;
        const uint PROCESS_WM_READ = 0x0010;
            [STAThread]
            static void Main(string[] args)
            {
                Program meh = new Program();
                meh.createToken();


               Process process = Process.GetProcessesByName("dolphin")[0];
               IntPtr hProcess = OpenProcess(PROCESS_WM_READ, false, process.Id);
               IntPtr processHandle = OpenProcess(0x1F0FFF, false, process.Id);
                Dolphin.Memory.Access.Dolphin dol = new Dolphin.Memory.Access.Dolphin(process);
            /*
                dol.TryGetBaseMEM1Address(out IntPtr mem1);
                Console.WriteLine($"Mem1 address = 0x{mem1:X}");

                dol.TryGetBaseMEM2Address(out IntPtr mem2);
                Console.WriteLine($"Mem2 address = 0x{mem2:X}");
            */
                dol.TryGetBaseAddress(out IntPtr mem1);
                  Console.WriteLine($"Mem1 address = 0x{mem1:X}");

                   dol.TryGetBaseAddress(out IntPtr mem2);
                    Console.WriteLine($"Mem2 address = 0x{mem2:X}");

                dol.TryGetAddress(0x80000024, out IntPtr mem1Address);
                byte[] buffer1 = new byte[4];
                if (ReadProcessMemory(hProcess, mem1Address, buffer1, buffer1.Length, out IntPtr bytesRead))
                {
                    if (BitConverter.IsLittleEndian) Array.Reverse(buffer1);
                    Console.WriteLine($"*(0x80000024) = 0x{BitConverter.ToUInt32(buffer1, 0):X}");

                    int bytesWritten = 0;
                    byte[] buffer = { 0x00, 0x00, 0x00,0x00  };

                    WriteProcessMemory(processHandle, mem1Address, buffer, buffer.Length, ref bytesWritten);
                    System.Threading.Thread.Sleep(500);
                    byte[] buffer2 = { 0x00, 0x00, 0x00, 0x01 };

                    WriteProcessMemory(processHandle, mem1Address, buffer2, buffer2.Length, ref bytesWritten);
                    Console.WriteLine(bytesWritten);
                }

            System.Threading.Thread.Sleep(50000);
        }
        async void createToken() {

            var result = await Curl.ExecuteAsync(@"
                curl -X POST https://id.twitch.tv/oauth2/token \
                -H 'Content-Type: application/x-www-form-urlencoded' \
                -d 'client_id=1qr5we58yuoxy63j14didy20d4adb9&client_secret=me6n8cx9dg6gu0iitg1b6bqw5yrgyw&grant_type=client_credentials'
            ");

            if (result.IsSuccess)
            {
                Console.WriteLine("Haiii");
                Console.WriteLine(result.Body);
                string[] parts = result.Body.Split('"');
                Console.WriteLine(parts[3]);
                //var charge = result.ParseJson<StripeCharge>();
                //Console.WriteLine($"Payment successful! ID: {charge.Id}");
            }
            else {
                Console.WriteLine(":(");
            }
        }

    }

}




