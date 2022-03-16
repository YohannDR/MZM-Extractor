﻿using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

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

        private static void WriteIncludes()
        {
            for (int i = 0; i < Includes.Length; i++)
                Header.WriteLine($"#include \"../src/{Includes[i]}\"");
        }

        public static void Parse()
        {
            Header.WriteLine("#ifndef DATA_H\n#define DATA_H\n");
            WriteIncludes();

            Stopwatch SW = new();
            SW.Start();

            JsonHandler.GetData();
            Data.CloseFile();
            Header.WriteLine("\n#endif /* DATA_H */");
            Header.Close();
            SW.Stop();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nExtracted everything in {SW.ElapsedMilliseconds} milliseconds");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}