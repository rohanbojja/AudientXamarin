// This file was auto-generated by ML.NET Model Builder. 

using System;
using System.IO;
using System.Linq;
using Microsoft.ML;
using AudientXamarinML.Model;

namespace AudientXamarinML.ConsoleApp
{
    class Program
    {
        //Dataset to use for predictions 
        private const string DATA_FILEPATH = @"C:\Users\rohan\source\repos\Audient\audient-feature-extractor\audient-feature-extractor\bin\Debug\netcoreapp3.1\Data.csv";

        static void Main(string[] args)
        {
            // Create single instance of sample data from first line of dataset for model input
            ModelInput sampleData = CreateSingleDataSample(DATA_FILEPATH);

            // Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);

            Console.WriteLine("Using model to make single prediction -- Comparing actual Genre with predicted Genre from sample data...\n\n");
            Console.WriteLine($"mfcc0: {sampleData.Mfcc0}");
            Console.WriteLine($"mfcc1: {sampleData.Mfcc1}");
            Console.WriteLine($"mfcc2: {sampleData.Mfcc2}");
            Console.WriteLine($"mfcc3: {sampleData.Mfcc3}");
            Console.WriteLine($"mfcc4: {sampleData.Mfcc4}");
            Console.WriteLine($"mfcc5: {sampleData.Mfcc5}");
            Console.WriteLine($"mfcc6: {sampleData.Mfcc6}");
            Console.WriteLine($"mfcc7: {sampleData.Mfcc7}");
            Console.WriteLine($"mfcc8: {sampleData.Mfcc8}");
            Console.WriteLine($"mfcc9: {sampleData.Mfcc9}");
            Console.WriteLine($"mfcc10: {sampleData.Mfcc10}");
            Console.WriteLine($"mfcc11: {sampleData.Mfcc11}");
            Console.WriteLine($"mfcc12: {sampleData.Mfcc12}");
            Console.WriteLine($"mfcc13: {sampleData.Mfcc13}");
            Console.WriteLine($"mfcc14: {sampleData.Mfcc14}");
            Console.WriteLine($"mfcc15: {sampleData.Mfcc15}");
            Console.WriteLine($"mfcc16: {sampleData.Mfcc16}");
            Console.WriteLine($"mfcc17: {sampleData.Mfcc17}");
            Console.WriteLine($"mfcc18: {sampleData.Mfcc18}");
            Console.WriteLine($"mfcc19: {sampleData.Mfcc19}");
            Console.WriteLine($"mfcc20: {sampleData.Mfcc20}");
            Console.WriteLine($"mfcc21: {sampleData.Mfcc21}");
            Console.WriteLine($"mfcc22: {sampleData.Mfcc22}");
            Console.WriteLine($"mfcc23: {sampleData.Mfcc23}");
            Console.WriteLine($"energy: {sampleData.Energy}");
            Console.WriteLine($"rms: {sampleData.Rms}");
            Console.WriteLine($"zcr: {sampleData.Zcr}");
            Console.WriteLine($"entropy: {sampleData.Entropy}");
            Console.WriteLine($"centroid: {sampleData.Centroid}");
            Console.WriteLine($"spread: {sampleData.Spread}");
            Console.WriteLine($"flatness: {sampleData.Flatness}");
            Console.WriteLine($"noiseness: {sampleData.Noiseness}");
            Console.WriteLine($"roloff: {sampleData.Roloff}");
            Console.WriteLine($"crest: {sampleData.Crest}");
            Console.WriteLine($"decrease: {sampleData.Decrease}");
            Console.WriteLine($"spectral_entropy: {sampleData.Spectral_entropy}");
            Console.WriteLine($"\n\nActual Genre: {sampleData.Genre} \nPredicted Genre value {predictionResult.Prediction} \nPredicted Genre scores: [{String.Join(",", predictionResult.Score)}]\n\n");
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
        }

        // Change this code to create your own sample data
        #region CreateSingleDataSample
        // Method to load single row of dataset to try a single prediction
        private static ModelInput CreateSingleDataSample(string dataFilePath)
        {
            // Create MLContext
            MLContext mlContext = new MLContext();

            // Load dataset
            IDataView dataView = mlContext.Data.LoadFromTextFile<ModelInput>(
                                            path: dataFilePath,
                                            hasHeader: true,
                                            separatorChar: ',',
                                            allowQuoting: true,
                                            allowSparse: false);

            // Use first line of dataset as model input
            // You can replace this with new test data (hardcoded or from end-user application)
            ModelInput sampleForPrediction = mlContext.Data.CreateEnumerable<ModelInput>(dataView, false)
                                                                        .First();
            return sampleForPrediction;
        }
        #endregion
    }
}
