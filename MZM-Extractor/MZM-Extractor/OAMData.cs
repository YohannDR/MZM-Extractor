using System;
using System.Collections.Generic;
using static MZM_Extractor.DataParser;

namespace MZM_Extractor;

public class OAMData
{
    // Array of already written OAM frames
    private static Dictionary<int, string> WrittenOAMFrames = new Dictionary<int, string>();

    public class FrameData
    {
        public string Name;
        public List<int> OAMFramePointers;
        public List<uint> Timers;

        public FrameData(string name)
        {
            Name = name;
            OAMFramePointers = new List<int>();
            Timers = new List<uint>();
        }
    }

    internal class OAMFrame : IComparable<OAMFrame>
    {
        public int Offset;
        public string Name;

        public OAMFrame(int offset, string name)
        {
            Offset = offset;
            Name = name;
        }

        public int CompareTo(OAMFrame other)
        {
            if (Offset > other.Offset)
                return 1;
            else if (Offset < other.Offset)
                return -1;
            else
                return 0;
        }
    }

    public bool HasContent => CurrentFrameData.Count != 0;
    // Array of the current OAM Frames for this file
    private List<OAMFrame> CurrentOAMFrames = new List<OAMFrame>();
    // Array of the current Frame Data for this file
    private List<FrameData> CurrentFrameData = new List<FrameData>();

    public OAMData()
    {
        CurrentOAMFrames = new List<OAMFrame>();
        CurrentFrameData = new List<FrameData>();
    }

    public void RegisterFrameData(FrameData frameData)
    {
        CurrentFrameData.Add(frameData);
    }

    public void RegisterOAMFrame(int offset, int id, uint timer, FrameData frameData)
    {
        frameData.OAMFramePointers.Add(offset);
        frameData.Timers.Add(timer);

        if (WrittenOAMFrames.ContainsKey(offset))
            return; // If already in global list, no need to do anything else

        string frame_name = $"{frameData.Name}_frame{id}";

        WrittenOAMFrames.Add(offset, frame_name); // Register to global

        CurrentOAMFrames.Add(new OAMFrame(offset, frame_name)); // Register to current
    }

    public void WriteData()
    {
        CurrentOAMFrames.Sort();

        foreach (OAMFrame OF in CurrentOAMFrames)
            WriteOAMFrame(OF);

        foreach (FrameData FD in CurrentFrameData)
            WriteFrameData(FD);
    }

    private void WriteOAMFrame(OAMFrame frame)
    {
        int offset = frame.Offset;
        string frame_name = frame.Name;

        ushort part_count = Data.GetShort(offset);

        Data.File.WriteLine($"u16 {frame_name}[{part_count * 3 + 1}] = {{");
        Header.WriteLine($"extern u16 {frame_name}[{part_count * 3 + 1}];");
        Data.File.WriteLine($"    0x{part_count:X},");
        offset += 2;
        for (int i = 0; i < part_count; i++)
            Data.File.WriteLine($"    0x{Data.GetShort(offset + (i * 6)):X}, 0x{Data.GetShort(offset + (i * 6) + 2):X}, 0x{Data.GetShort(offset + (i * 6) + 4):X}, ");
        Data.File.WriteLine("};\n");
    }
    private void WriteFrameData(FrameData frameData)
    {
        Data.File.WriteLine($"struct FrameData {frameData.Name}[{frameData.OAMFramePointers.Count + 1}] = {{");
        Header.WriteLine($"extern struct FrameData {frameData.Name}[{frameData.OAMFramePointers.Count + 1}];");
        for (int i = 0; i < frameData.OAMFramePointers.Count; i++)
        {
            if (frameData.Timers[i] != 0)
            {
                Data.File.WriteLine($"    {WrittenOAMFrames[frameData.OAMFramePointers[i]]},");
                Data.File.WriteLine($"    0x{frameData.Timers[i]:X},");
            }
        }
        Data.File.WriteLine("    NULL,\n    0x0\n};\n");
    }

}