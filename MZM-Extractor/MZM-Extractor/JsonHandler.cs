using System;
using System.IO;
using System.Diagnostics;
using System.Text.Json;
using System.Collections.Generic;
using static MZM_Extractor.DataParser;

namespace MZM_Extractor
{
    public static class JsonHandler
    {
        private static Stopwatch SW = new Stopwatch();
        public static void GetData()
        {
            StreamReader reader = new StreamReader(Database);
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
                        case "Pointer":
                            type = DataType.Pointer;
                            break;
                        default:
                            type = DataType.u8;
                            break;
                    }
                    if (array == 1)
                        type |= DataType.SingleArray;
                    else if (array == 2)
                        type |= DataType.DoubleArray;

                    string name = elem.GetProperty("Name").GetString();
                    if ((type & (DataType)127) != DataType.Pointer)
                        new Data(name, elem.GetProperty("Offset").GetInt32(), type, elem.GetProperty("Size").GetInt64()).Extract();
                    else
                    {
                        List<string> pointerData = new List<string>();
                        foreach (JsonElement p in elem.GetProperty("Pointers").EnumerateArray())
                            pointerData.Add(p.GetString());
                        string realType = elem.GetProperty("pType").GetString();
                        new Data(name, elem.GetProperty("Offset").GetInt32(), type, elem.GetProperty("Size").GetInt64(), realType, pointerData).Extract();
                    }
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