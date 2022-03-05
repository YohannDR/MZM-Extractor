using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using static MZM_Extractor.DataParser;

namespace MZM_Extractor
{
    public static class JsonHandler
    {
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
                    bool array = elem.GetProperty("IsArray").GetBoolean();
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
                    if (array)
                        type |= DataType.IsArray;

                    new Data(elem.GetProperty("Name").GetString(), elem.GetProperty("Offset").GetInt32(), type, elem.GetProperty("Size").GetInt32()).Extract();
                }
            }
        }
    }
}