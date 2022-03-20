using System;
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

        private static readonly string[] Includes = new string[] // All of the includes needed in the header
        {
            "oam.h",
            "types.h",
            "samus.h",
            "sprites_AI/unused_sprites.h",
            "sprites_AI/charge_beam.h",
            "sprites_AI/deorem.h",
            "sprites_AI/dragon.h",
            "sprites_AI/elevator_pad.h",
            "sprites_AI/enemy_drop.h",
            "sprites_AI/explosion_zebes_escape.h",
            "sprites_AI/geruta.h",
            "sprites_AI/hive.h",
            "sprites_AI/imago_cocoon.h",
            "sprites_AI/map_station.h",
            "sprites_AI/metroid.h",
            "sprites_AI/item_banner.h",
            "sprites_AI/zoomer.h",
            "sprites_AI/zeela.h",
            "sprites_AI/ripper.h",
            "sprites_AI/zeb.h",
            "sprites_AI/skree.h",
            "sprites_AI/morph_ball.h",
            "sprites_AI/chozo_statue.h",
            "sprites_AI/sova.h",
            "sprites_AI/multiviola.h",
            "sprites_AI/squeept.h",
            "sprites_AI/reo.h",
            "sprites_AI/gunship.h",
            "sprites_AI/skultera.h",
            "sprites_AI/dessgeega.h",
            "sprites_AI/waver.h",
            "sprites_AI/power_grip.h",
            "sprites_AI/imago_larva.h",
            "sprites_AI/morph_ball_launcher.h",
            "sprites_AI/space_pirate.h",
            "sprites_AI/gamet.h",
            "sprites_AI/unknown_item_chozo_statue.h",
            "sprites_AI/zebbo.h",
            "sprites_AI/worker_robot.h",
            "sprites_AI/parasite.h",
            "sprites_AI/piston.h",
            "sprites_AI/ridley.h",
            "sprites_AI/security_gate.h",
            "sprites_AI/zipline_generator.h",
            "sprites_AI/polyp.h",
            "sprites_AI/rinka.h",
            "sprites_AI/viola.h",
            "sprites_AI/geron_norfair.h",
            "sprites_AI/holtz.h",
            "sprites_AI/gekitai_machine.h",
            "sprites_AI/ruins_test.h",
            "sprites_AI/save_platform.h",
            "sprites_AI/kraid.h",
            "sprites_AI/ripper2.h",
            "sprites_AI/mella.h",
            "sprites_AI/atomic.h",
            "sprites_AI/area_banner.h",
            "sprites_AI/mother_brain.h",
            "sprites_AI/acid_worm.h",
            "sprites_AI/escape_ship.h",
            "sprites_AI/sidehopper.h",
            "sprites_AI/geega.h",
            "sprites_AI/zebetite.h",
            "sprites_AI/cannon.h",
            "sprites_AI/zipline.h",
            "sprites_AI/imago_larva_right_side.h",
            "sprites_AI/tangle_vine.h",
            "sprites_AI/imago.h",
            "sprites_AI/crocomire.h",
            "sprites_AI/geron.h",
            "sprites_AI/glass_tube.h",
            "sprites_AI/save_platform_chozodia.h",
            "sprites_AI/baristute.h",
            "sprites_AI/elevator_statue.h",
            "sprites_AI/rising_chozo_pillar.h",
            "sprites_AI/security_laser.h",
            "sprites_AI/boss_statues.h",
            "sprites_AI/searchlight_eye.h",
            "sprites_AI/steam.h",
            "sprites_AI/unknown_item_block.h",
            "sprites_AI/gadora.h",
            "sprites_AI/searchlight.h",
            "sprites_AI/primary_sprite_B3.h",
            "sprites_AI/space_pirate_carrying_power_bomb.h",
            "sprites_AI/water_drop.h",
            "sprites_AI/falling_chozo_pillar.h",
            "sprites_AI/mecha_ridley.h",
            "sprites_AI/escape_gate.h",
            "sprites_AI/black_space_pirate.h",
            "sprites_AI/escape_ship_pirate.h",
            "sprites_AI/chozo_ball.h",
            "sprites_AI/save_yes_no_cursor.h",
            "sprites_AI/chozo_statue_movement.h"
        };

        private static void WriteIncludes()
        {
            for (int i = 0; i < Includes.Length; i++)
                Header.WriteLine($"#include \"../src/{Includes[i]}\"");
        }

        public static void Parse()
        {
            Stopwatch SW = Stopwatch.StartNew();

            Header.WriteLine("#ifndef DATA_H\n#define DATA_H\n");
            WriteIncludes();

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