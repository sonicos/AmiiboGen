using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace AmiiboGen
{
    internal class Options
    {
        [Option('i', "input", MetaValue = "FILE", Required = true, HelpText = "Input file or wildcard")]
        public string Input { get; set; }
        [Option('o', "output", MetaValue =  "FILE", HelpText = "Output filename prefix (or folder for wildcard). Default uses current filename/folder")]
        public string Output { get; set; }
        [Option('c', "count", MetaValue = "INT", DefaultValue = 1, HelpText = "Number of generated tags per Amiibo")]
        public int Count { get; set; }
        [Option('d', "own-directory", HelpText = "Creates a sub-directory for each Amiibo")]
        public bool OwnFolders { get; set; }
        [Option('s', "standardize", HelpText = "Standardize output filename based on Amiibo Data")]
        public bool Standardize { get; set; }
        [Option('p',"set-prefix", HelpText = "Prefix the filename with the Set 'Shortname' (requires -s to have effect)")]
        public bool SetPrefix { get; set; }
        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo(ThisAssembly.Title, ThisAssembly.Version),
                Copyright = new CopyrightInfo(ThisAssembly.Author, 2017),
                AdditionalNewLineAfterOption = true,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine($"Usage:");
            help.AddPreOptionsLine($"  {AppDomain.CurrentDomain.FriendlyName} -i MyTag.bin");
            help.AddPreOptionsLine($"    Will create MyTag_99B2D583303381.bin");
            help.AddPreOptionsLine($"  {AppDomain.CurrentDomain.FriendlyName} -i MyTag.bin -o NewFileName.bin");
            help.AddPreOptionsLine($"    Will create NewFileName_99B2D583303381.bin");
            help.AddPreOptionsLine($"  {AppDomain.CurrentDomain.FriendlyName} -i Sheik.bin -s -p");
            help.AddPreOptionsLine($"    Will create [SSB]_Shiek_4DE1A40ADFBB08.bin");
            help.AddPreOptionsLine($"  {AppDomain.CurrentDomain.FriendlyName} -i ZeldaFile.bin -s -d");
            help.AddPreOptionsLine($"    Will create Zelda_BotW\\Zelda_BotW_937D32795962CC");
            help.AddPreOptionsLine($"  {AppDomain.CurrentDomain.FriendlyName} -i *.bin -o generated -s -d -c 50");
            help.AddPreOptionsLine($"    Will create 50 Unique IDs for each found .bin file, placed in their own directory underneath 'generated'");
            help.AddOptions(this);
            return help;
        }
    }
}