using System;
using System.IO;
using System.Text;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using static MZM_Extractor.DataParser;

namespace MZM_Extractor
{
    public enum DataType
    {
        u8 = 0,
        u16 = 1,
        u32 = 2,
        i8 = 3,
        i16 = 4,
        i32 = 5,
        Pointer = 6,
        OAM = 7,
        IsArray = 128 // Flag
    }

    public class Data
    {
        public static StreamWriter File; // Current file to write

        public string Name;
        public int Offset;
        public DataType Type;
        public int Size; // Number of elements
        public static List<OAMData> OAM;

        public Data(string name, int offset, DataType type, int size)
        {
            Name = name;
            Offset = offset;
            Type = type;
            Size = size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetByte(int offset) => Source[offset];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort GetShort(int offset) => (ushort)(Source[offset] | (uint)Source[offset + 1] << 8);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetInt(int offset) => Source[offset] | (uint)Source[offset + 1] << 8 | (uint)Source[offset + 2] << 16 | (uint)Source[offset + 3] << 24;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint GetPointer(int offset) => Source[offset] | (uint)Source[offset + 1] << 8 | (uint)Source[offset + 2] << 16 | (uint)Source[offset + 3] - 8 << 24;

        public static void CreateFile(string name)
        {
            if (OAM != null)
            {
                foreach (OAMData O in OAM)
                    O.Write();
            }
            OAM = new();
            Header.WriteLine($"\n/* {name} */\n");
            File?.Close();
            File = new(System.IO.File.Create($"{Destination}/{name}"));
            File.WriteLine("#include \"data.h\"\n");
        }

        public void Extract()
        {
            StringBuilder text = new();
            switch (Type & (DataType)127) // Remove array flag
            {
                case DataType.i8:
                case DataType.u8:
                    byte u8_data;
                    if ((Type & DataType.IsArray) != 0)
                    {
                        File.WriteLine($"{Type & (DataType)127} {Name}[{Size}] = {{"); // Write definition
                        Header.WriteLine($"{Type & (DataType)127} {Name}[{Size}];"); // Write in header
                        for (int i = 0; i < Size; i++)
                        {
                            u8_data = GetByte(Offset + i);
                            text.Append("   0x").Append(u8_data.ToString("X")); // Write Data
                            if (i == Size - 1)
                                text.Append("\n};");
                            else
                                text.Append(",\n");
                        }
                        File.WriteLine(text);
                    }
                    else
                    {
                        Header.WriteLine($"{Type} {Name};");
                        File.WriteLine($"{Type} {Name} = 0x{GetByte(Offset)};");
                    }
                    File.WriteLine();
                    break;

                case DataType.i16:
                case DataType.u16:
                    ushort u16_data;
                    if ((Type & DataType.IsArray) != 0)
                    {
                        File.WriteLine($"{Type & (DataType)127} {Name}[{Size}] = {{"); // Write definition
                        Header.WriteLine($"{Type & (DataType)127} {Name}[{Size}];"); // Write in header
                        for (int i = 0; i < Size; i++)
                        {
                            u16_data = GetShort(Offset + (i * 2));
                            text.Append("   0x").Append(u16_data.ToString("X")); // Write Data
                            if (i == Size - 1)
                                text.Append("\n};");
                            else
                                text.Append(",\n");
                        }
                        File.WriteLine(text);
                    }
                    else
                    {
                        Header.WriteLine($"{Type} {Name};");
                        File.WriteLine($"{Type} {Name} = 0x{GetShort(Offset)};");
                    }
                    File.WriteLine();
                    break;

                case DataType.i32:
                case DataType.u32:
                    uint u32_data;
                    if ((Type & DataType.IsArray) != 0)
                    {
                        File.WriteLine($"{Type & (DataType)127} {Name}[{Size}] = {{"); // Write definition
                        Header.WriteLine($"{Type & (DataType)127} {Name}[{Size}];"); // Write in header
                        for (int i = 0; i < Size; i++)
                        {
                            u32_data = GetInt(Offset + (i * 4));
                            text.Append("   0x").Append(u32_data.ToString("X")); // Write Data
                            if (i == Size - 1)
                                text.Append("\n};");
                            else
                                text.Append(",\n");
                        }
                        File.WriteLine(text);
                    }
                    else
                    {
                        u32_data = GetInt(Offset);
                        Header.WriteLine($"{Type} {Name};");
                        File.WriteLine($"{Type} {Name} = 0x{u32_data};");
                    }
                    File.WriteLine();
                    break;

                case DataType.OAM:
                    for (int i = 0; i < Size; i++)
                    {
                        if (GetInt(Offset + (i * 8) + 4) != 0)
                            ExtractOAMFrame((int)GetPointer(Offset + (i * 8)), i);
                    }

                    OAMData oam = new(this);
                    for (int i = 0; i < Size; i++)
                    {
                        if (GetInt(Offset + (i * 8) + 4) != 0)
                            oam.Info.Add(($"{Name}{i + 1}", GetInt(Offset + (i * 8) + 4)));
                    }
                    OAM.Add(oam);
                    break;
            }
        }

        private void ExtractOAMFrame(int offset, int id)
        {
            ushort part_count;
            File.WriteLine($"struct oam_frame {Name}{id + 1} = {{");
            Header.WriteLine($"struct oam_frame {Name}{id + 1};");
            part_count = GetShort(offset);
            File.WriteLine($"    0x{part_count:X},\n    {{");
            for (int i = 0; i < part_count; i++)
                File.WriteLine($"        {{ 0x{GetShort(offset + (i * 2) + 2):X}, 0x{GetShort(offset + (i * 2) + 4):X}, 0x{GetShort(offset + (i * 2) + 6):X} }},");
            File.WriteLine("    }\n};\n");
        }
    }
}
