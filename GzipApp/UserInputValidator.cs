using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GzipApp
{
    public class UserInputValidator
    {
        public ValidationResult Validate(string[] args)
        {
            var validation_result = new ValidationResult();
            validation_result.IsValid = false;

            if (args.Length == 0 || args.Length < 3 || args.Length > 3 || ((args[0].ToLower() != "compress" && args[0].ToLower() != "decompress")))
            {
                validation_result.Message = "GzipTest.exe compress/decompress [input_path] [output_path]";
                return validation_result;
            }

            string input_file_path = args[1];
            string output_file_path = args[2];

            if (!File.Exists(input_file_path))
            {
                validation_result.Message = "Input file is not found";
                return validation_result;
            }

            if (File.Exists(output_file_path))
            {
                validation_result.Message = "Output file already exists";
                return validation_result;
            }

            if (input_file_path == output_file_path)
            {
                validation_result.Message = "Input and output files must be different";
                return validation_result;
            }

            validation_result.IsValid = true;
            return validation_result;
        }
    }
}
