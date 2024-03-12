using CsvHelper.Configuration;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace NeuroGaze_Datya_Synthesis
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();
            var consolidatedData = program.ConsolidateData();
            program.AnalyzeData(consolidatedData);
        }


        public List<SelectionResult> ReadSelectionResults(string filePath, string selectionTechnique)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            }))
            {
                csv.Context.RegisterClassMap<SelectionResultMap>(); // Register the custom class map
                var records = csv.GetRecords<SelectionResult>().ToList();
                records.ForEach(r => r.SelectionTechnique = selectionTechnique);
                return records;
            }

        }

        public List<SelectionResult> ConsolidateData()
        {
            var allResults = new List<SelectionResult>();

            // Base paths for each selection technique, assuming a base directory here
            string baseFilePath = "D:/source/NeuroGaze/unity/Assets/Assessment Results"; // Update this with the actual path
            string[] participantIDs = { "2", "3", "4", "5", "6" };

            foreach (var id in participantIDs)
            {
                string eyeGazeHandsPath = $"{baseFilePath}/Eye Gaze_Hands/StaticEyeGazeHandsResults_{id}.csv";
                string neuroGazePath = $"{baseFilePath}/NeuroGaze/StaticNeuroGazeResults_{id}.csv";
                string vrControllersPath = $"{baseFilePath}/VR Controllers/StaticVRControllersResults_{id}.csv";

                allResults.AddRange(ReadSelectionResults(eyeGazeHandsPath, "EyeGazeHands"));
                allResults.AddRange(ReadSelectionResults(neuroGazePath, "NeuroGaze"));
                allResults.AddRange(ReadSelectionResults(vrControllersPath, "VRControllers"));
            }

            return allResults;
        }



        public void AnalyzeData(List<SelectionResult> results)
        {
            // Define the path for the output file
            string outputPath = "D:/source/NeuroGaze/unity/Assets/Assessment Results/analysis_results.txt";

            using (var writer = new StreamWriter(outputPath))
            {
                var groupedResults = results.GroupBy(r => r.SelectionTechnique).ToList();
                foreach (var group in groupedResults)
                {
                    var averageDuration = group.Average(g => g.RoundDuration);
                    var averageErrorRate = group.Average(g => (double)g.ErrorCount / g.TotalInteractables);

                    // Write the analysis results to the file
                    writer.WriteLine($"Selection Technique: {group.Key}");
                    writer.WriteLine($"Average Duration: {averageDuration}");
                    writer.WriteLine($"Average Error Rate: {averageErrorRate}\n");
                }
            }
        }


    }
}
