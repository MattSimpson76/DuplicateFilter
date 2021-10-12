using System;
using FuzzySharp;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace DuplicateFilter
{
    class DuplicateChecker
    {
        static void Main(string[] args)
        {
            // default similarity threshold of 92 gives fairly good results with few false positives; a custom threshold can be specified as first command line argument
            int duplicateSimilarityThreshold = 92;
            if ((args.Length == 0) || (int.TryParse(args[0], out duplicateSimilarityThreshold) && (0 <= duplicateSimilarityThreshold && duplicateSimilarityThreshold <= 100)))
            {
                // Get relative test data path
                string dataPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\..\\..\\..\\test_data\\org_names.json";
                string nameOutputPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\..\\..\\..\\test_data\\reduced_org_names.json";
                string listOfDuplicatesOutputPath = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\..\\..\\..\\test_data\\grouped_duplicates.json";

                // Read in data
                string companyText = System.IO.File.ReadAllText(dataPath);
                // Deserialize JSON to list of strings
                List<string> companyNames = JsonSerializer.Deserialize<List<string>>(companyText);
                Console.WriteLine("Read in data");

                // determines whether terms like Ltd., Co. are standardised to long-form (e.g. ltd. -> limited, co. -> company) or removed entirely
                // default behaviour is to remove them
                bool stripBusinessTerms = true;
                if (args.Length >= 2)
                {
                    bool.TryParse(args[1], out stripBusinessTerms);
                }
                InputSanitiser.StandardiseInput(companyNames, stripBusinessTerms);     
                Console.WriteLine("Sanitised input");

                Filter duplicateFilter = new Filter(duplicateSimilarityThreshold);
                List<List<string>> listOfDuplicates = new List<List<string>>();
                Console.WriteLine("Removing duplicates...");

                List<string> reducedList;
                //determines whether to output a grouped list of duplicates; default behaviour is yes
                bool outputGroupedDuplicates = true;
                if (args.Length >= 3) {
                    bool.TryParse(args[2], out outputGroupedDuplicates);
                }
                if (outputGroupedDuplicates)
                {
                    reducedList = duplicateFilter.RemoveAllDuplicates(companyNames, listOfDuplicates);
                }
                else
                {
                    reducedList = duplicateFilter.RemoveAllDuplicates(companyNames);
                }

                reducedList.Sort();

                Console.WriteLine("Writing to file...");
                var jsonOptions = new JsonSerializerOptions
                {
                    // using unsafe minimal escaping of characters here so output looks prettier
                    // in actual production code, characters should probably be escaped for safety
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };

                // output list without duplicates
                string serializedNames = JsonSerializer.Serialize<List<string>>(reducedList, jsonOptions);
                System.IO.File.WriteAllText(nameOutputPath, serializedNames);
                
                if (outputGroupedDuplicates)
                {
                    // output all entries grouped by duplicate matches
                    string serializedDuplicates = JsonSerializer.Serialize<List<List<string>>>(listOfDuplicates, jsonOptions);
                    System.IO.File.WriteAllText(listOfDuplicatesOutputPath, serializedDuplicates);
                }
                
                Console.WriteLine("Done.");
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("Please enter a valid integer (0-100) as first argument, used for duplicate similarity threshold.");
                Environment.Exit(1);
            }            
        }
    }
}

