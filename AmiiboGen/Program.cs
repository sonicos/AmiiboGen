using System;
using System.IO;

using LibAmiibo.Data;
namespace AmiiboGen
{
    internal class Program
    {
        private static Random _rnd;
        private static void Main(string[] args)
        {
            Console.WriteLine();
            var options = new Options();
            var parser = new CommandLine.Parser(with => with.HelpWriter = Console.Error);
            _rnd = new Random();
            if (parser.ParseArgumentsStrict(args, options, () => Environment.Exit(-2)))
            {
                Run(options);
            }
            
        }

        private static void Run(Options options)
        {
            
            var errCount = 0;
            var setPrefix = options.SetPrefix;
            var ownFolders = options.OwnFolders;
            var useStandard = options.Standardize;
            string outDir;
            var allFiles = false;
            var inFile = string.Empty;
            var generatedCopies = options.Count;
            var inDir = string.Empty;
            var inGlob = string.Empty;
            var outFile = string.Empty;
            if (options.Input.IndexOf('*') > -1)
            {
                allFiles = true;
                var split = options.Input.LastIndexOf(@"\", StringComparison.Ordinal);
                if (split > -1) inDir = options.Input.Substring(0, split);
                inGlob = options.Input.Substring(split + 1);
                if (string.IsNullOrEmpty(inDir)) inDir = ".";
                outDir = !string.IsNullOrEmpty(options.Output) ? options.Output : ".";
            }
            else
            {
                inFile = options.Input;
                if (inFile.EndsWith("key_retail.bin"))
                {
                    Console.WriteLine("You can't use key_retail.bin as an input file.");
                    Environment.Exit(1);
                }
                if (!string.IsNullOrEmpty(options.Output)) outFile = options.Output;
                outDir = ".";
            }
            var fileList = allFiles ? new DirectoryInfo(inDir).GetFiles(inGlob) : new[] { new FileInfo(inFile) };
            if (fileList.Length < 1)
            {
                Console.WriteLine("No files found. Exiting");
                Environment.Exit(1);
            }
            if (!new FileInfo("key_retail.bin").Exists)
            {
                Console.WriteLine("File key_retail.bin was not found");
                Environment.Exit(1);
            }
            foreach (var f in fileList)
            {
                if (f.Name.ToLower() == "key_retail.bin") continue;
                if (!f.Exists)
                {
                    Console.WriteLine($@"File {f.Name} not found");
                    errCount++;
                    continue;
                }
                Console.WriteLine($@"Checking {f.Name}");
                var encryptedNtagData = File.ReadAllBytes(f.FullName);
                AmiiboTag amiiboTag;
                try
                {
                    amiiboTag = AmiiboTag.DecryptWithKeys(encryptedNtagData);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Error decoding {f.Name}: Invalid/corrupt Amiibo file\n");
                    errCount++;
                    continue;
                }
                if (!amiiboTag.IsDecrypted)
                {
                    Console.WriteLine("Amiibo was not properly decrypted! Invalid key_retail.bin or invalid/corrupted Amiibo file\n");
                    errCount++;
                    continue;
                }
                Console.WriteLine($@"Found Amiibo:");
                Console.WriteLine($@"  ID:        {amiiboTag.Amiibo.AmiiboNo}");
                Console.WriteLine($@"  Name:      {amiiboTag.Amiibo.StatueName}");
                Console.WriteLine($@"  Set:       {amiiboTag.Amiibo.AmiiboSetName}");
                Console.WriteLine($@"  Statue ID: {amiiboTag.Amiibo.StatueId}");
                Console.WriteLine($@"  UID:       {amiiboTag.UID.Replace(" ", "")}");
                Console.WriteLine();
                
                var standardString = string.Empty;
                var unofficialString = f.Name.Replace(f.Extension, "");
                if (setPrefix) standardString += $"[{amiiboTag.Amiibo.AmiiboSetShortName}] ";
                standardString += amiiboTag.Amiibo.StatueName.Replace(", ", "_");
                standardString = standardString.Replace(" ", "_");
                var outFn = useStandard ? standardString : unofficialString;
                if (!allFiles && !string.IsNullOrEmpty(outFile)) outFn = outFile;
                var basePath = outDir;
                if (ownFolders)
                    basePath = Path.Combine(outDir, outFn);
                Directory.CreateDirectory(basePath);

                for (var i = 0; i < generatedCopies; i++)
                {
                    amiiboTag.NtagSerial = new ArraySegment<byte>(RandomSerial());
                    var outSerial = amiiboTag.UID.Replace(" ", "");
                    Console.Write($@"  Writing {outFn}_{outSerial}{f.Extension} ..");
                    var outPath = Path.Combine(basePath, $"{outFn}_{outSerial}{f.Extension}");
                    byte[] encBytes;
                    try
                    {
                        encBytes = amiiboTag.EncryptWithKeys();
                    }
                    catch
                    {
                        Console.WriteLine(@"Coule not encrypt!");
                        errCount++;
                        continue;
                    }
                    try
                    {
                        File.WriteAllBytes(outPath, encBytes);
                        Console.WriteLine($@". Done");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($@"Error writing {outPath}: {e.Message}");
                        errCount++;
                        continue;
                    }

                }
                Console.WriteLine();
                Console.WriteLine($@"  Completed Amiibo");
                Console.WriteLine();

            }

            Console.WriteLine();
            if (!allFiles) return;
            if (errCount > 0)
            {
                Console.WriteLine($@"Completed with {errCount} errors");
                Environment.ExitCode = 1;
            }
            else
            {
                Console.WriteLine(@"Completed successfully");
            }
        }

        private static byte[] RandomSerial()
        {
            var b = new byte[8];
            _rnd.NextBytes(b);
            return b;
        }
    }
}
