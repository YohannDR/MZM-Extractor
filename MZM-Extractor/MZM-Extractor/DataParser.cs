using System;
using System.IO;
using System.Diagnostics;

namespace MZM_Extractor
{
    public static class DataParser
    {
        public static StreamWriter Header; // .h file
        public static byte[] Source;
        public static string Destination;
        public static string Database;

        private static string[] Includes = new string[] // All of the includes needed in the header
        {
            "oam.h",
            "types.h"
        };

        static int[] Offsets = new int[]
        {
            0x2CD474,
            0x2CD49C,
            0x2CD4CC,
            0x2CD4F4,
            0x2CD51C,
            0x2CD544,
            0x2CD574,
            0x2CD59C,
            0x2CD5C4,
            0x2CD5E4
        };

        static int[] Sizes = new int[]
        {
            5,
            6,
            5,
            5,
            5,
            6,
            5,
            5,
            4,
            4
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

            Stopwatch SW = new();
            SW.Start();
            Data.CreateFile("skree_data.h");
            new Data("skree_falling_speed", 0x2cca7c, DataType.i16 | DataType.IsArray, 8).Extract();
            new Data("skree_gfx", 0x2cca8c, DataType.u8 | DataType.IsArray, 1056).Extract();
            new Data("skree_pal", 0x2cceac, DataType.u16 | DataType.IsArray, 16).Extract();
            new Data("skree_blue_gfx", 0x2ccecc, DataType.u8 | DataType.IsArray, 1056).Extract();
            new Data("skree_blue_pal", 0x2cd2ec, DataType.u16 | DataType.IsArray, 16).Extract();
            for (int i = 0; i < Offsets.Length; i++)
                new Data($"skree_oam_{i}", Offsets[i], DataType.OAM, Sizes[i]).Extract();
            Data.CreateFile("zoomer_data.h");
            Data.File.Close();
            SW.Stop();
            Console.WriteLine(SW.ElapsedMilliseconds);
            Header.WriteLine("\n#endif /* DATA_H */");
            Header.Close();
        }
    }
}
