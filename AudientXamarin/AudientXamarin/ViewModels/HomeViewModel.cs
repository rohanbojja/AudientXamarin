using AudientXamarin.Models;
using AudientXamarin.Services;
using Microsoft.ML;
using MvvmHelpers;
using NWaves.Audio;
using NWaves.FeatureExtractors;
using NWaves.FeatureExtractors.Multi;
using NWaves.FeatureExtractors.Options;
using NWaves.Features;
using NWaves.Signals;
using NWaves.Transforms;
using NWaves.Windows;
using Plugin.AudioRecorder;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using Microcharts;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;

namespace AudientXamarin.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        //DECLARATIONS
        System.Threading.Timer timer;
        string tag = "AudXam";
        int duration = 0;
        float[] op = new float[10];
        AudioRecorderService recorder;
        List<float[]> featureTimeList = new List<float[]>();
        public List<float[]> tdVectors;
        public List<float[]> mfccVectors;
        ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
        private static readonly HttpClient client = new HttpClient();
        public string featureString = String.Empty;
        public string _filePath = String.Empty;
        public ObservableRangeCollection<Genre> GenreList { get; set; }
        public List<Genre> genreList;
        //BACKING STORE / BINDINGS

        public Chart currentChart;
        public Chart CurrentChart { get { return currentChart; } set { SetProperty(ref currentChart, value); } }

        public Chart aggChart;
        public Chart AggChart { get { return aggChart; } set { SetProperty(ref aggChart, value); } }

        public string recordStatus = "record";
        public string RecordStatus
        {
            get { return recordStatus; }
            set
            {
                SetProperty(ref recordStatus, value);
            }
        }
        public string predictedLabel;
        public string PredictedLabel
        {
            get { return predictedLabel; }
            set
            {
                SetProperty(ref predictedLabel, value);
            }
        }

        public HomeViewModel()
        {
            GenreList = new ObservableRangeCollection<Genre>();
            PredictedLabel = "Genre";
            var entries = new[]
             {
                 new ChartEntry(212)
                 {
                     Label = "What will it be?",
                     ValueLabel = "212",
                     Color = SKColor.Parse("#2c3e50")
                 }
            };
            CurrentChart = new DonutChart() { Entries = entries, IsAnimated = true, AnimationDuration = TimeSpan.FromSeconds(5) };
            AggChart = new DonutChart() { Entries = entries, IsAnimated = true, AnimationDuration = TimeSpan.FromSeconds(5) };
            recorder = new AudioRecorderService
            {
                StopRecordingOnSilence = false, //will stop recording after 2 seconds (default)
                StopRecordingAfterTimeout = true,  //stop recording after a max timeout (defined below)
                TotalAudioTimeout = TimeSpan.FromSeconds(30) //audio will stop recording after 15 seconds
            };
            //player = new AudioPlayer();
            playCommand = new Command(() =>
            {
                var files = Assembly.GetExecutingAssembly().GetManifestResourceNames();
                Console.WriteLine($"AUDXAM ALL FILES: {files.Length} {files.ToList()} { files[0]}");
                if (recorder.IsRecording != true && player.IsPlaying == false)
                {
                    //player.Load(GetStreamFromFile());
                    player.Play();
                    var startTimeSpan = TimeSpan.Zero;
                    var periodTimeSpan = TimeSpan.FromMilliseconds(60);
                    float[] op2 = new float[10];
                    Array.Clear(op2, 0, op2.Length);
                    timer = new System.Threading.Timer((e) =>
                    {
                        if (player.IsPlaying)
                        {

                            //Console.WriteLine($"{tag} CURRENT PREDICTION: {op.Score[9]} curPos:{Convert.ToInt32(player.CurrentPosition)} M AT ZERO {featureTimeList[0].Mfcc0} AT FIVE {featureTimeList[5].Mfcc0} FINAL -> {ConsumeModel.Predict(featureTimeList[Convert.ToInt32(player.CurrentPosition)]).Score[5]}");
                            op = featureTimeList[Convert.ToInt32(player.CurrentPosition)];

                            for (int i = 0; i < 10; i++)
                            {
                                op2[i] += op[i];
                            }

                            float max_sf = 0;
                            int max_ind = 0;
                            string[] labels = "Blues, Classical, Country, Disco, HipHop, Jazz, Metal, Pop, Reggae Rock".Split(',');
                            for (int i = 0; i < 10; i++)
                            {
                                if (op2[i] > max_sf)
                                {
                                    max_sf = op2[i];
                                    max_ind = i;
                                }
                            }

                            PredictedLabel = $"{labels[max_ind]}!";

                            //Pass to ML.net

                            var entries_agg = new[]
                             {
                                 new ChartEntry(op2[0])
                                 {
                                     Label = "Blues",
                                     ValueLabel = $"{op2[0]}",
                                     Color = SKColor.Parse("#2c3e50")
                                 },
                                 new ChartEntry(op2[1])
                                 {
                                     Label = "Classical",
                                     ValueLabel = $"{op2[1]}",
                                     Color = SKColor.Parse("#77d065")
                                 },
                                 new ChartEntry(op2[2])
                                 {
                                     Label = "Country",
                                     ValueLabel = $"{op2[2]}",
                                     Color = SKColor.Parse("#b455b6")
                                 },
                                 new ChartEntry(op2[3])
                                 {
                                     Label = "Disco",
                                     ValueLabel = $"{op2[3]}",
                                     Color = SKColor.Parse("#245e50")
                                 },
                                 new ChartEntry(op2[4])
                                 {
                                     Label = "Hiphop",
                                     ValueLabel = $"{op2[4]}",
                                     Color = SKColor.Parse("#3498db")
                                 },
                                 new ChartEntry(op2[5])
                                 {
                                     Label = "Jazz",
                                     ValueLabel = $"{op2[5]}",
                                     Color = SKColor.Parse("#263e50")
                                 },
                                 new ChartEntry(op2[6])
                                 {
                                     Label = "Metal",
                                     ValueLabel = $"{op2[6]}",
                                     Color = SKColor.Parse("#123456")
                                 },
                                 new ChartEntry(op2[7])
                                 {
                                     Label = "Pop",
                                     ValueLabel = $"{op2[7]}",
                                     Color = SKColor.Parse("#654321")
                                 },
                                 new ChartEntry(op2[8])
                                 {
                                     Label = "Reggae",
                                     ValueLabel = $"{op2[8]}",
                                     Color = SKColor.Parse("#526784")
                                 },
                                 new ChartEntry(op2[9])
                                 {
                                     Label = "Rock",
                                     ValueLabel = $"{op2[9]}",
                                     Color = SKColor.Parse("#404040")
                                 }
                            };



                            var entries_current = new[]
                             {
                                 new ChartEntry(op[0])
                                 {
                                     Label = "Blues",
                                     ValueLabel = $"{op[0]}",
                                     Color = SKColor.Parse("#2c3e50")
                                 },
                                 new ChartEntry(op[1])
                                 {
                                     Label = "Classical",
                                     ValueLabel = $"{op[1]}",
                                     Color = SKColor.Parse("#77d065")
                                 },
                                 new ChartEntry(op[2])
                                 {
                                     Label = "Country",
                                     ValueLabel = $"{op[2]}",
                                     Color = SKColor.Parse("#b455b6")
                                 },
                                 new ChartEntry(op[3])
                                 {
                                     Label = "Disco",
                                     ValueLabel = $"{op[3]}",
                                     Color = SKColor.Parse("#245e50")
                                 },
                                 new ChartEntry(op[4])
                                 {
                                     Label = "Hiphop",
                                     ValueLabel = $"{op[4]}",
                                     Color = SKColor.Parse("#3498db")
                                 },
                                 new ChartEntry(op[5])
                                 {
                                     Label = "Jazz",
                                     ValueLabel = $"{op[5]}",
                                     Color = SKColor.Parse("#263e50")
                                 },
                                 new ChartEntry(op[6])
                                 {
                                     Label = "Metal",
                                     ValueLabel = $"{op[6]}",
                                     Color = SKColor.Parse("#123456")
                                 },
                                 new ChartEntry(op[7])
                                 {
                                     Label = "Pop",
                                     ValueLabel = $"{op[7]}",
                                     Color = SKColor.Parse("#654321")
                                 },
                                 new ChartEntry(op[8])
                                 {
                                     Label = "Reggae",
                                     ValueLabel = $"{op[8]}",
                                     Color = SKColor.Parse("#526784")
                                 },
                                 new ChartEntry(op[9])
                                 {
                                     Label = "Rock",
                                     ValueLabel = $"{op[9]}",
                                     Color = SKColor.Parse("#404040")
                                 }
                            };

                            //Update and draw graph
                            AggChart = new DonutChart() { Entries = entries_agg, IsAnimated = false, AnimationDuration = TimeSpan.FromMilliseconds(0) };
                            //CurrentChart = new DonutChart() { Entries = entries_current, IsAnimated = false, AnimationDuration = TimeSpan.FromMilliseconds(0) };
                            //Console.WriteLine($"{tag} Updated chart! C {op.Score[9]} A ->{op2[9]}");

                            //Graph 1 -> Current prediciton

                            //Graph 2 -> Aggregate of predictions so far
                        }
                        else
                        {
                            player.Seek(0.0);
                            timer.Dispose();
                        }
                    }, null, startTimeSpan, periodTimeSpan);
                }
            });

            recordCommand = new Command(async () =>
            {
                if (RecordStatus != "record")
                {
                    PredictedLabel = "Processing audio...";
                    await recorder.StopRecording().ConfigureAwait(true);
                    if (_filePath != null)
                    {
                        //await extractFeatures();
                        var stream = new FileStream(_filePath, FileMode.Open);
                        AudioFunctions.WriteWavHeader(stream, 1, 48000, 16, (int)stream.Length - 44);
                        stream.Close();
                        player.Load(GetStreamFromFile());
                        //player.Play();
                        duration = Convert.ToInt32(player.Duration);
                        Console.WriteLine($"{tag} DUR: {duration}");
                       // player.Dispose();
                        extractFeatures();
                        //await predict
                    }
                    RecordStatus = "record";
                }
                else
                {
                    PredictedLabel = "Recording...";
                    IsBusy = true;
                    RecordStatus = "stop";
                    var recordTask = await recorder.StartRecording().ConfigureAwait(true);
                    _filePath = await recordTask.ConfigureAwait(true);
                    //player.Play(_filePath
                }
            });
        }
        public Command recordCommand { get; }
        public Command playCommand { get; }

        Stream GetStreamFromFile()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;

            var stream = new FileStream(_filePath, FileMode.Open);
            MemoryStream streamCopy = new MemoryStream();
            stream.CopyTo(streamCopy);
            streamCopy.Position = 0;
            stream.Close();
            return streamCopy;
        }
        public void extractFeatures()
        {
            //NWaves
            //Initial setup
            if (_filePath != null)
            {
                DiscreteSignal signal;

                // load
                var mfcc_no = 24;
                var samplingRate = 44100;
                var mfccOptions = new MfccOptions
                {
                    SamplingRate = samplingRate,
                    FeatureCount = mfcc_no,
                    FrameDuration = 0.025/*sec*/,
                    HopDuration = 0.010/*sec*/,
                    PreEmphasis = 0.97,
                    Window = WindowTypes.Hamming
                };

                var opts = new MultiFeatureOptions
                {
                    SamplingRate = samplingRate,
                    FrameDuration = 0.025,
                    HopDuration = 0.010
                };
                var tdExtractor = new TimeDomainFeaturesExtractor(opts);
                var mfccExtractor = new MfccExtractor(mfccOptions);

                // Read from file.
                featureString = String.Empty;
                featureString = $"green,";
                //MFCC
                var avg_vec_mfcc = new List<float>(mfcc_no + 1);
                //TD Features
                var avg_vec_td = new List<float>(4);
                //Spectral features
                var avg_vec_spect = new List<float>(10);

                for (var i = 0; i < mfcc_no; i++)
                {
                    avg_vec_mfcc.Add(0f);
                }
                for (var i = 0; i < 4; i++)
                {
                    avg_vec_td.Add(0f);
                }

                for (var i = 0; i < 10; i++)
                {
                    avg_vec_spect.Add(0f);
                }

                string specFeatures = String.Empty;
                Console.WriteLine($"{tag} Reading from file");
                using (var stream = new FileStream(_filePath, FileMode.Open))
                {
                    var waveFile = new WaveFile(stream);
                    signal = waveFile[channel: Channels.Left];
                    ////Compute MFCC
                    float[] mfvfuck = new float[25];
                    var sig_sam = signal.Samples;
                    mfccVectors = mfccExtractor.ComputeFrom(sig_sam);

                    var fftSize = 1024;
                    tdVectors = tdExtractor.ComputeFrom(signal.Samples);
                    var fft = new Fft(fftSize);
                    var resolution = (float)samplingRate / fftSize;

                    var frequencies = Enumerable.Range(0, fftSize / 2 + 1)
                                                .Select(f => f * resolution)
                                                .ToArray();

                    var spectrum = new Fft(fftSize).MagnitudeSpectrum(signal).Samples;

                    var centroid = Spectral.Centroid(spectrum, frequencies);
                    var spread = Spectral.Spread(spectrum, frequencies);
                    var flatness = Spectral.Flatness(spectrum, 0);
                    var noiseness = Spectral.Noiseness(spectrum, frequencies, 3000);
                    var rolloff = Spectral.Rolloff(spectrum, frequencies, 0.85f);
                    var crest = Spectral.Crest(spectrum);
                    var decrease = Spectral.Decrease(spectrum);
                    var entropy = Spectral.Entropy(spectrum);
                    specFeatures = $"{centroid},{spread},{flatness},{noiseness},{rolloff},{crest},{decrease},{entropy}";
                    //}
                    Console.WriteLine($"{tag} All features ready");
                    for (int calibC = 0; calibC < mfccVectors.Count; calibC += (mfccVectors.Count / duration)-1)
                    {
                        featureString = String.Empty;
                        var tmp = new ModelInput();
                        for (var i = 0; i < mfcc_no; i++)
                        {
                            avg_vec_mfcc[i] = mfccVectors[calibC][i];
                        }
                        for (var i = 0; i < 4; i++)
                        {
                            avg_vec_td[i] = tdVectors[calibC][i];
                        }
                        featureString += String.Join(",", avg_vec_mfcc);
                        featureString += ",";
                        featureString += String.Join(",", avg_vec_td);
                        featureString += ",";
                        featureString += specFeatures;
                        Console.WriteLine($"{tag} Feature String ready {featureString}");
                        if(File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp")))
                        {
                            File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp"));
                            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp"), featureString);
                        }
                        else
                        {
                            File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp"), featureString);
                        }
                        
                        MLContext mLContext = new MLContext();
                        
                        string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "temp");
                        
                        IDataView dataView = mLContext.Data.LoadFromTextFile<ModelInput>(
                                            path: fileName,
                                            hasHeader: false,
                                            separatorChar: ',',
                                            allowQuoting: true,
                                            allowSparse: false);

                        // Use first line of dataset as model input
                        // You can replace this with new test data (hardcoded or from end-user application)
                        ModelInput sampleForPrediction = mLContext.Data.CreateEnumerable<ModelInput>(dataView, false)
                                                                                    .First();
                        ModelOutput opm = ConsumeModel.Predict(sampleForPrediction);
                        featureTimeList.Add(opm.Score);
                        Console.WriteLine($"{tag} Feature vs time list ready");
                    }
                    //Console.WriteLine($"{tag} MFCC: {mfccVectors.Count}");
                    //Console.WriteLine($"{tag} TD: {tdVectors.Count}");
                    //Console.WriteLine($"{tag} featureTimeArray: {featureTimeList.Count} {featureString}");
                }
            }
        }
    }
}
