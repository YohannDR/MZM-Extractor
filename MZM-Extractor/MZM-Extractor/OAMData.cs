using System;
using System.Collections.Generic;
using static MZM_Extractor.DataParser;

namespace MZM_Extractor
{
    public class OAMData
    {
        public Data Current;
        public List<(string name, uint timer)> Info = new List<(string name, uint timer)>();

        public OAMData(Data data) => Current = data;

        public void Write()
        {
            Data.File.WriteLine($"struct FrameData {Current.Name}[{Current.Size}] = {{");
            Header.WriteLine($"struct FrameData {Current.Name}[{Current.Size}];");
            for (int i = 0; i < Info.Count; i++)
            {
                if (Info[i].timer != 0)
                {
                    Data.File.WriteLine($"    {Info[i].name},");
                    Data.File.WriteLine($"    0x{Info[i].timer:X},");
                }
            }
            Data.File.WriteLine("    NULL,\n    0x0\n};\n");
        }
    }
}
