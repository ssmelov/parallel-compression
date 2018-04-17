using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;

namespace GzipApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                UserInputValidator validator = new UserInputValidator();
                var validation_result = validator.Validate(args);
                if (!validation_result.IsValid)
                {
                    Console.WriteLine(validation_result.Message);
                    return;
                }

                string operation_type = args[0].ToLower();
                string input_file_path = args[1];
                string output_file_path = args[2];

                var compressor = new Compressor(new FileReader(input_file_path), new FileWriter(output_file_path));

                if (operation_type == "compress")
                {
                    Console.WriteLine("compressing...");
                    compressor.Compress();
                }
                else if (operation_type == "decompress")
                {
                    Console.WriteLine("decompressing...");
                    compressor.Decompress();
                }

                if (compressor.ErrorMessages.Count == 0)
                    Console.WriteLine("Operation completed");
                else
                {
                    foreach (var error_message in compressor.ErrorMessages)
                    {
                        Console.WriteLine(error_message); 
                    }

                    Console.WriteLine("Operation failed");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e.Message);
                Console.WriteLine("Operation failed");
                return;
            }
        }
    }
}
