using System;
using System.IO;
using System.Diagnostics;
using System.Text.Json;
using static MZM_Extractor.DataParser;

namespace MZM_Extractor
{
    public static class JsonHandler
    {
        private static Stopwatch SW = new();
        public static void GetData()
        {
            StreamReader reader = new(Database);
            JsonDocument doc = JsonDocument.Parse(reader.ReadToEnd());
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                return;
            foreach (JsonProperty file in doc.RootElement.EnumerateObject())
            {
                if (file.Value.ValueKind != JsonValueKind.Array)
                    continue;
                Data.CreateFile(file.Name);
                foreach (JsonElement elem in file.Value.EnumerateArray())
                {
                    SW.Start();
                    byte array = elem.GetProperty("IsArray").GetByte();
                    DataType type;
                    switch (elem.GetProperty("Type").GetString())
                    {
                        // u8 is handled by default case
                        case "u16":
                            type = DataType.u16;
                            break;
                        case "u32":
                            type = DataType.u32;
                            break;
                        case "i8":
                            type = DataType.i8;
                            break;
                        case "i16":
                            type = DataType.i16;
                            break;
                        case "i32":
                            type = DataType.i32;
                            break;
                        case "OAM":
                            type = DataType.OAM;
                            break;
                        default:
                            type = DataType.u8;
                            break;
                    }
                    if (array == 1)
                        type |= DataType.IsArray;
                    else if (array == 2)
                        type |= DataType.DoubleArray;

                    string name = elem.GetProperty("Name").GetString();
                    new Data(name, elem.GetProperty("Offset").GetInt32(), type, elem.GetProperty("Size").GetInt32()).Extract();
                    SW.Stop();
                    Console.Write($"- Extracted {name} to {file.Name} ; ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"({SW.ElapsedTicks} ticks)");
                    Console.ForegroundColor = ConsoleColor.White;
                    SW.Reset();
                }
            }
        }
    }
}