﻿// Copyright (c) 2012-2019 fo-dicom contributors.
// Licensed under the Microsoft Public License (MS-PL).

using System;
using System.IO;
using System.Threading.Tasks;
using NLog.Config;
using NLog.Targets;
using Dicom;
using Dicom.Log;
using Dicom.Network;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace ConsoleTest
{
    internal static class Program
    {

        private static async Task Main(string[] args)
        {
            try
            {
                // Initialize log manager.
                LogManager.SetImplementation(NLogManager.Instance);

                DicomException.OnException += delegate(object sender, DicomExceptionEventArgs ea)
                    {
                        ConsoleColor old = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(ea.Exception);
                        Console.ForegroundColor = old;
                    };

                var config = new LoggingConfiguration();

                var target = new ColoredConsoleTarget
                {
                    Layout = @"${date:format=HH\:mm\:ss}  ${message}"
                };
                config.AddTarget("Console", target);
                config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Debug, target));

                NLog.LogManager.Configuration = config;

                var client = new DicomClient("127.0.0.1", 11112, false, "SCU", "STORESCP");
                client.NegotiateAsyncOps();
                for (int i = 0; i < 10; i++)
                {
                    await client.AddRequestAsync(new DicomCEchoRequest());
                }

                await client.AddRequestAsync(new DicomCStoreRequest(@"test1.dcm"));
                await client.AddRequestAsync(new DicomCStoreRequest(@"test2.dcm"));
                await client.SendAsync();

                foreach (DicomPresentationContext ctr in client.AdditionalPresentationContexts)
                {
                    Console.WriteLine("PresentationContext: " + ctr.AbstractSyntax + " Result: " + ctr.Result);
                }

                var samplesDir = Path.Combine(
                    Path.GetPathRoot(Environment.CurrentDirectory),
                    "Development",
                    "fo-dicom-samples");
                var testDir = Path.Combine(samplesDir, "Test");

                if (!Directory.Exists(testDir)) Directory.CreateDirectory(testDir);

                //var img = new DicomImage(samplesDir + @"\ClearCanvas\CRStudy\1.3.51.5145.5142.20010109.1105627.1.0.1.dcm");
                //img.RenderImage().Save(testDir + @"\test.jpg");

                //var df = DicomFile.Open(samplesDir + @"\User Submitted\overlays.dcm");

                //Console.WriteLine(df.FileMetaInfo.Get<DicomTransferSyntax>(DicomTag.TransferSyntaxUID).UID.Name);
                //Console.WriteLine(df.Dataset.Get<PlanarConfiguration>(DicomTag.PlanarConfiguration));

                //var img = new DicomImage(df.Dataset);
                //img.RenderImage().Save(testDir + @"\test.jpg");

                //df = df.ChangeTransferSyntax(DicomTransferSyntax.JPEGLSLossless);
                //df.Save(testDir + @"\test-jls.dcm");

                //df = df.ChangeTransferSyntax(DicomTransferSyntax.JPEG2000Lossless);
                //df.Save(testDir + @"\test-j2k.dcm");

                //df = df.ChangeTransferSyntax(DicomTransferSyntax.JPEGProcess14SV1);
                //df.Save(testDir + @"\test-jll.dcm");

                //df = df.ChangeTransferSyntax(DicomTransferSyntax.RLELossless);
                //df.Save(testDir + @"\test-rle.dcm");

                //df = df.ChangeTransferSyntax(DicomTransferSyntax.ExplicitVRLittleEndian);
                //df.Save(testDir + @"\test-ele.dcm");

                //df = df.ChangeTransferSyntax(DicomTransferSyntax.ExplicitVRBigEndian);
                //df.Save(testDir + @"\test-ebe.dcm");

                //df = df.ChangeTransferSyntax(DicomTransferSyntax.ImplicitVRLittleEndian);
                //df.Save(testDir + @"\test-ile.dcm");

                //Console.WriteLine("End...");
                //Console.ReadLine();

                //df.WriteToLog(LogManager.GetCurrentClassLogger(), LogLevel.Info);

                //Console.WriteLine(DicomValueMultiplicity.Parse("1"));
                //Console.WriteLine(DicomValueMultiplicity.Parse("3"));
                //Console.WriteLine(DicomValueMultiplicity.Parse("1-3"));
                //Console.WriteLine(DicomValueMultiplicity.Parse("1-n"));
                //Console.WriteLine(DicomValueMultiplicity.Parse("2-2n"));

                //Console.WriteLine(DicomTag.Parse("00200020"));
                //Console.WriteLine(DicomTag.Parse("0008,0040"));
                //Console.WriteLine(DicomTag.Parse("(3000,0012)"));
                //Console.WriteLine(DicomTag.Parse("2000,2000:TEST CREATOR"));
                //Console.WriteLine(DicomTag.Parse("(4000,4000:TEST_CREATOR:2)"));

                //Console.WriteLine(DicomMaskedTag.Parse("(30xx,xx90)"));
                //Console.WriteLine(DicomMaskedTag.Parse("(3000-3021,0016)"));

                //DicomRange<DateTime> r = new DicomRange<DateTime>(DateTime.Now.AddSeconds(-5), DateTime.Now.AddSeconds(5));
                //Console.WriteLine(r.Contains(DateTime.Now));
                //Console.WriteLine(r.Contains(DateTime.Today));
                //Console.WriteLine(r.Contains(DateTime.Now.AddSeconds(60)));

                //DicomDictionary dict = new DicomDictionary();
                //dict.Load(@"F:\Development\fo-dicom\DICOM\Dictionaries\dictionary.xml", DicomDictionaryFormat.XML);

                //string output = Dicom.Generators.DicomTagGenerator.Generate("Dicom", "DicomTag", dict);
                //File.WriteAllText(@"F:\Development\fo-dicom\DICOM\DicomTagGenerated.cs", output);

                //output = Dicom.Generators.DicomDictionaryGenerator.Generate("Dicom", "DicomDictionary", "LoadInternalDictionary", dict);
                //File.WriteAllText(@"F:\Development\fo-dicom\DICOM\DicomDictionaryGenerated.cs", output);

                //string output = Dicom.Generators.DicomUIDGenerator.Process(@"F:\Development\fo-dicom\DICOM\Dictionaries\dictionary.xml");
                //File.WriteAllText(@"F:\Development\fo-dicom\DICOM\DicomUIDGenerated.cs", output);
            }
            catch (Exception e)
            {
                if (!(e is DicomException)) Console.WriteLine(e.ToString());
            }

            Console.ReadLine();
        }
    }
}
