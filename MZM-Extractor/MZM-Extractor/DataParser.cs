using System.IO;

namespace MZM_Extractor
{
    public static class DataParser
    {
        public static StreamWriter Header; // .h file
        public static byte[] Source;
        public static string Destination;
        public static string Database;

        private static string[] Includes = new string[] // All of the includes in the header
        {
            "types.h",
            "oam.h"
        };

        private static void WriteIncludes()
        {
            for (int i = 0; i < Includes.Length; i++)
                Header.WriteLine($"#include \"{Includes[i]}\"");
        }

        public static void Parse()
        {
            Header.WriteLine("#ifndef DATA_H\n#define DATA_H\n");
            WriteIncludes();
            // Parse
            Header.WriteLine("#endif /* DATA_H */");
        }
    }
}
