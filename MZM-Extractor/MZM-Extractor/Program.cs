using System;
using System.IO;
using static MZM_Extractor.DataParser;

namespace MZM_Extractor
{
    public static class Program
    {
        static void Main(string[] args)
        {
            bool auto = false;
            string source = "baserom_us.gba";
            string destination = "data";
            string dbSource = "database.json";

            if (args.Length != 0)
            {
                if (args[0] == "-a")
                    auto = true;
                else if (args[0] == "-h")
                {
                    Console.WriteLine("Metroid Zero Mission Data Extractor");
                    Console.WriteLine("Extracts all the data from MZM (OAM, Graphics, Palette, Arrays) and formats them in C");
                    Console.WriteLine("You need to provide :");
                    Console.WriteLine("- Source file (a vanilla MZM rom)");
                    Console.WriteLine("- Destination folder (where every extracted data file will be put");
                    Console.WriteLine("- Database file (file containing all the offsets, types and sizes for the data");
                    Console.WriteLine("You may use -a to automatically use the default settings (baserom_us.gba, data/, database.json)\n");
                    return;
                }
            }

            if (!auto)
            {
                Console.WriteLine("Provide source file : ");
                source = Console.ReadLine();
                Console.WriteLine("Provide destination folder : ");
                destination = Console.ReadLine();
                Console.WriteLine("Provide database file : ");
                dbSource = Console.ReadLine();
            }

            destination = $"{Directory.GetCurrentDirectory()}/{destination}";
            if (Directory.Exists(destination)) Directory.Delete(destination, true); // Delete if exists
            Directory.CreateDirectory(destination); // Create destination folder

            Source = File.ReadAllBytes(source);
            Destination = destination;
            Database = dbSource;
            File.Create($"{destination}/data.h").Close(); // Create header
            Header = new StreamWriter($"{destination}/data.h"); // Load header
            Parse();

            Console.WriteLine((ushort)(Source[10000 + 1] << 8 + Source[10000]));
            Console.WriteLine(Source[0]);
        }
    }
}
