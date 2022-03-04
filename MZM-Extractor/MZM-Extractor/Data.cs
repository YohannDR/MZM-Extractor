using System.IO;
using System.Text;
using static MZM_Extractor.DataParser;

namespace MZM_Extractor
{
    enum DataType
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

    internal class Data
    {
        public static StreamWriter File; // Current file to write

        public string Name;
        public int Offset;
        public DataType Type;
        public int Size; // Number of elements

        public Data(string name, int offset, DataType type, int size)
        {
            Name = name;
            Offset = offset;
            Type = type;
            Size = size;
        }

        public void Extract()
        {
            StringBuilder text = new();
            switch (Type & (DataType)121) // Remove array flag
            {
                case DataType.u8:
                    byte u8_data;
                    if ((Type & DataType.IsArray) != 0)
                    {
                        File.WriteLine($"u8 {Name}[{Size}] {{"); // Write definition
                        Header.WriteLine($"u8 {Name}[{Size}];"); // Write in header
                        for (int i = 0; i < Size; i++)
                        {
                            File.WriteLine($"{Source[Offset + i]}"); // Write Data
                            if (i == Size - 1)
                                text.Append("};");
                            else
                                File.WriteLine(",\n");
                        }
                        File.WriteLine(text);
                    }
                    else
                    {
                        u8_data = Source[Offset];
                        Header.WriteLine($"u8 {Name} {u8_data}");
                        File.WriteLine($"u8 {Name} {u8_data}");
                    }
                    break;

                case DataType.u16:
                    ushort u16_data;
                    if ((Type & DataType.IsArray) != 0)
                    {
                        File.WriteLine($"u16 {Name}[{Size}] {{"); // Write definition
                        Header.WriteLine($"u16 {Name}[{Size}];"); // Write in header
                        for (int i = 0; i < Size; i += 2)
                        {
                            u16_data = (ushort)(Source[Offset + 1] << 8 + Source[Offset]);
                            File.WriteLine($"{u16_data}"); // Write Data
                            if (i == Size - 1)
                                text.Append("};");
                            else
                                File.WriteLine(",\n");
                        }
                        File.WriteLine(text);
                    }
                    else
                    {
                        u16_data = (ushort)(Source[Offset + 1] << 8 + Source[Offset]);
                        Header.WriteLine($"u16 {Name} {u16_data}");
                        File.WriteLine($"u16 {Name} {u16_data}");
                    }
                    break;
            }
        }
    }
}
