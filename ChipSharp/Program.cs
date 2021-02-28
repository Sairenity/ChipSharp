using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChipSharp
{

    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var ioState = new IOState();
            var form = new Display(ioState);
            var cpu = new CPU(ioState);
            var cpuTicks = 0;
            var secondStart = DateTime.Now;

            form.Show();
            cpu.LoadRom();

            Task.Run(() =>
            {
                while (true)
                {
                    if (cpu.Delay > 0)
                        cpu.Delay--;
                    Thread.Sleep(1000 / 60);

                }
            });
            while (form.Visible)
            {
                Application.DoEvents();
                var clockCycleStart = DateTime.Now;
                if (ioState.Reset)
                {
                    ioState.Reset = false;
                    cpu.Reset();
                    cpu.LoadRom();
                }
                cpu.Clock();
                cpuTicks++;

                if (ioState.ForceRedraw)
                {
                    ioState.ForceRedraw = false;
                    form.RenderDisplay();
                }

                var delta = (DateTime.Now - secondStart).TotalMilliseconds;
                if (delta > 1000)
                {
                    form.IPS = cpuTicks;
                    secondStart = DateTime.Now;
                    cpuTicks = 0;
                }
                while ((DateTime.Now - clockCycleStart).TotalMilliseconds < 1.0) { }
            }

        }

        private static void PrintDebug(CPU cpu)
        {
            Console.SetCursorPosition(0, 0);

            var row = "";
            for (int i = 0; i <= 0x0f; i++)
                row += $"v{i:X1}: {cpu.Registers[i]}\t{(i == 7 ? Environment.NewLine : string.Empty)}";
            row += Environment.NewLine;
            row += $"I: {cpu.IndexRegister}\tSP: {cpu.Stack.Count}\tPC: {cpu.ProgramCounter:x3}";
            Console.WriteLine(row);
        }
    }
}
