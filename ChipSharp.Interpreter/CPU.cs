using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChipSharp
{
    public class CPU
    {
        private readonly IOState _ioState;
        public static readonly ushort ProgramOffset = 0x200;
        public byte[] Memory = new byte[4096];
        public ushort ProgramCounter = ProgramOffset;
        public ushort IndexRegister = 0;
        public Stack<ushort> Stack = new Stack<ushort>(); //shortstack hehe
        public byte DelayTimer { get; set; } = 0;
        public byte SoundTimer { get; set; } = 0;
        public byte[] Registers = new byte[16];
        public byte Delay { get; set; } = 0;

        private readonly Random _random = new Random();

        private bool _romLoaded;


        public CPU(IOState ioState)
        {
            _ioState = ioState;
            LoadFont();
        }

        public void Reset()
        {
            Memory = new byte[4096];
            _ioState.Display = new bool[64 * 32];
            ProgramCounter = ProgramOffset;
            IndexRegister = 0;
            Stack.Clear();
            DelayTimer = 0;
            SoundTimer = 0;
            Registers = new byte[16];
            Delay = 0;

            LoadFont();
        }

        private void LoadFont()
        {
            int offset = 0x50;
            foreach (var symbol in Font.Symbols)
            {
                symbol.CopyTo(Memory, offset);
                offset += symbol.Length;
            }
        }

        public void LoadRom()
        {
            _ioState.Rom.CopyTo(Memory, ProgramOffset);
            _romLoaded = true;
        }

        public void Clock()
        {
            if (!_romLoaded) return;
            if (Delay > 0)
            {
                return;
            }
            if (SoundTimer > 0)
            {
                var test = SoundTimer;
                Task.Run(() =>
                {
                    if (OperatingSystem.IsWindows())
                    {
                        Console.Beep(500, test * 20);
                    }
                });

                SoundTimer = 0;
            }

            var instr = Fetch();
            var instrCategory = instr >> 12;

            var x = (byte)((instr & 0x0F00) >> 8);
            var y = (byte)((instr & 0x00F0) >> 4);
            var n = (byte)(instr & 0x000F);
            var nn = (byte)(instr & 0x00FF);
            var nnn = (ushort)(instr & 0x0FFF);

            switch (instrCategory)
            {
                case 0x00: // 0x00E0 clear screen
                    if (instr == 0x00E0)
                        for (int i = 0; i < _ioState.Display.Length; i++)
                            _ioState.Display[i] = false;
                    if (instr == 0x00EE)
                        ProgramCounter = Stack.Pop();
                    break;

                case 0x01: // 0x1NNN jump
                    ProgramCounter = nnn;
                    break;

                case 0x02: // 0x2nnn CALL addr
                    Stack.Push((ushort)(ProgramCounter));
                    ProgramCounter = nnn;
                    break;

                case 0x03: // 0x3xnn SE Vx, byte
                    if (Registers[x] == nn) ProgramCounter += 2;
                    break;

                case 0x04: // 0x4xnn SNE Vx, byte
                    if (Registers[x] != nn) ProgramCounter += 2;
                    break;

                case 0x05: // 0x5xy0 SE Vx, Vy
                    if (Registers[x] == Registers[y]) ProgramCounter += 2;
                    break;

                case 0x06: // 0x6XNN set register vX to NN
                    Registers[x] = nn;
                    break;

                case 0x07: // 0x7XNN add value NN to vX
                    Registers[x] += nn;
                    break;

                case 0x08:
                    switch (n)
                    {
                        case 0x00: // 0x8xy0 LD Vx, Vy
                            Registers[x] = Registers[y];
                            break;
                        case 0x01: // 0x8xy1 OR Vx, Vy
                            Registers[x] |= Registers[y];
                            break;
                        case 0x02: // 0x8xy2 AND Vx, Vy
                            Registers[x] &= Registers[y];
                            break;
                        case 0x03: // 0x8xy3 XOR Vx, Vy
                            Registers[x] ^= Registers[y];
                            break;
                        case 0x04: // 0x8xy4 ADD Vx, Vy
                            var result = Registers[x] + Registers[y];
                            Registers[0x0F] = result > 255 ? (byte)1 : (byte)0;
                            Registers[x] = (byte)(result & 0xFF);
                            break;
                        case 0x05: // 0x8xy5 SUB Vx, Vy
                            Registers[0x0F] = Registers[x] > Registers[y] ? (byte)1 : (byte)0;
                            Registers[x] -= Registers[y];
                            break;
                        case 0x06: // 0x8xy6 SHR Vx
                            Registers[0x0F] = (Registers[x] & 0x01) == 1 ? (byte)1 : (byte)0;
                            Registers[x] >>= 1;
                            break;
                        case 0x07: // 0x8xy7 SUBN Vx, Vy
                            Registers[0x0F] = Registers[y] > Registers[x] ? (byte)1 : (byte)0;
                            Registers[x] = (byte)(Registers[y] - Registers[x]);
                            break;
                        case 0x0E: // 0x8xyE SHL Vx
                            Registers[0x0F] = (Registers[x] >> 7 & 0x01) == 1 ? (byte)1 : (byte)0;
                            Registers[x] <<= 1;
                            break;
                    }
                    break;

                case 0x09: // 0x9xy0 SNE Vx, Vy
                    if (Registers[x] != Registers[y]) ProgramCounter += 2;
                    break;

                case 0x0A: // 0xANNN set index register I to NNN
                    IndexRegister = nnn;
                    break;

                case 0x0B: // 0xBnnn JP V0, addr
                    ProgramCounter = (ushort)(nnn + Registers[0]);
                    break;

                case 0x0C: // 0xCxnn RND Vx, byte
                    var randomByte = (byte)_random.Next(0, 255);
                    Registers[x] = (byte)(randomByte & nn);
                    break;

                case 0x0D: // 0xDXYN display

                    var yVal = Registers[y] % 32;
                    Registers[0x0F] = 0;

                    for (byte row = 0; row < n; row++)
                    {
                        var xVal = Registers[x] % 64;
                        var spriteData = Memory[IndexRegister + row];

                        for (int bitIndex = 7; bitIndex >= 0; bitIndex--) // iterate through bits of spriteData, msb first
                        {
                            var isBitSet = ((spriteData >> bitIndex) & 0x01) == 1;
                            if (_ioState.Display[xVal + 64 * yVal])
                                Registers[0x0F] = 1;
                            _ioState.Display[xVal + 64 * yVal] ^= isBitSet;
                            xVal++;
                            xVal %= 64;

                        }
                        yVal++;
                        yVal %= 32;
                    }
                    _ioState.ForceRedraw = true;
                    break;

                case 0x0E:
                    switch (nn)
                    {
                        case 0x9E: // SKP Vx
                            if (_ioState.KeyState[Registers[x]]) ProgramCounter += 2;
                            break;
                        case 0xA1: // SKNP Vx
                            if (!_ioState.KeyState[Registers[x]]) ProgramCounter += 2;
                            break;
                    }
                    break;

                case 0x0F:
                    switch (nn)
                    {
                        case 0x07: //0xFx08 LD Vx, DT
                            Registers[x] = Delay;
                            break;
                        case 0x15: // 0xFx15 LD DT, Vx
                            Delay = n;
                            break;
                        case 0x18: // 0xFx18 LD ST, Vx
                            SoundTimer = n;
                            break;
                        case 0x1E: // 0xFx1E ADD I, Vx
                            IndexRegister += Registers[x];
                            break;
                        case 0x29: // 0xFx29 LD F, Vx
                            IndexRegister = (ushort)(0x50 + Registers[x] * 5);
                            break;
                        case 0x33: // 0xFx33 LD B, Vx
                            Memory[IndexRegister + 0] = (byte)Math.Abs(Registers[x] / 100 % 10);
                            Memory[IndexRegister + 1] = (byte)Math.Abs(Registers[x] / 10 % 10);
                            Memory[IndexRegister + 2] = (byte)Math.Abs(Registers[x] / 1 % 10);
                            break;
                        case 0x55: // 0xFx65 LD [I], Vx
                            for (int i = 0; i <= x; i++)
                            {
                                Memory[IndexRegister + i] = Registers[i];
                            }
                            break;
                        case 0x65: // 0xFx65 LD Vx, [I]
                            for (int i = 0; i <= x; i++)
                            {
                                Registers[i] = Memory[IndexRegister + i];
                            }
                            break;
                        case 0x0A: // this is kludgey as all fuck lol
                            if (_ioState.KeyState.All(q => q != true)) ProgramCounter -= 2;
                            for (int i = 0; i < _ioState.KeyState.Length; i++)
                                if (_ioState.KeyState[i])
                                {
                                    Registers[x] = (byte)i;
                                    _ioState.KeyState[i] = false;
                                }

                            break;

                    }
                    break;
            }
        }

        public ushort Fetch()
        {
            var upper = FetchByte();
            var lower = FetchByte();
            return (ushort)((upper << 8) + lower);
        }

        private byte FetchByte()
        {
            return Memory[ProgramCounter++];
        }

        public void PrintDebug()
        {
            Console.WriteLine("┌──────┬─────────────────────────────────────────────────┐");
            Console.WriteLine("│ RAM  │ 00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F │");
            Console.WriteLine("├──────┼─────────────────────────────────────────────────┼──────────────────┐");

            string rowTemplate = "│ {0} │ {1}│ {2} │";
            for (int i = 0x00; i < Memory.Length - 0x10; i += 0x10)
            {
                var memoryRow = "";
                var asciiRow = "";
                for (int j = i; j <= i + 0x0F; j += 0x01)
                {
                    var val = Memory[j];
                    memoryRow += val.ToString("X2") + " ";
                    asciiRow += char.IsControl((char)val) ? '.' : (char)val;
                }

                Console.WriteLine(rowTemplate, i.ToString("X4"), memoryRow, asciiRow);
            }
            Console.WriteLine("└──────┴─────────────────────────────────────────────────┴──────────────────┘");
        }


    }
}
