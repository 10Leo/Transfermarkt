﻿using LJMB.Common;
using LJMB.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Page.Scraper.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Transfermarkt.Core.ParseHandling.Pages;

namespace Transfermarkt.Core.Test.ParseHandling.Pages
{
    [TestClass]
    public class PagePerformanceTest
    {
        private const string performanceFolderPath = @"..\..\..\Tests\Performance\";

        private readonly string clubPerformanceFilePath = $@"{performanceFolderPath}club.txt";
        private readonly string subjectClubPerformanceFile = $@"file://{Path.GetFullPath(@"..\..\..\")}Tests\Performance\club.html";

        private static int MinimumLoggingLevel { get; } = ConfigManager.GetAppSetting<int>(Keys.Config.MinimumLoggingLevel);
        private static string LogPath { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.LogPath);

        private static readonly ILogger logger = LoggerFactory.GetLogger((LogLevel)MinimumLoggingLevel);

        public PagePerformanceTest()
        {
            if (!File.Exists(clubPerformanceFilePath))
            {
                using (FileStream fs = File.Create(clubPerformanceFilePath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes("Init");
                    fs.Write(info, 0, info.Length);
                }
            }
        }

        [TestMethod, TestCategory("Page Parsing"), TestCategory("Performance")]
        public void TestClubParsing()
        {
            IDomain domain = null;

            List<long> ellapsedMillis = new List<long>();
            for (int i = 0; i < 20; i++)
            {
                ClubPage page = new ClubPage();

                var watch = System.Diagnostics.Stopwatch.StartNew();
                page.Connect(subjectClubPerformanceFile);
                page.Parse();
                watch.Stop();
                ellapsedMillis.Add(watch.ElapsedMilliseconds);

                domain = page.Domain;
            }

            Assert.IsNotNull(domain, "The returned Domain is null.");

            TestingConfigs.DomainElementsCheck(domain);
            for (int i = 0; i < domain.Children.Count; i++)
            {
                TestingConfigs.DomainElementsCheck(domain.Children[i]);
            }

            // Log only if the checks passed.
            Log(ellapsedMillis);
        }

        private void Log(List<long> ellapsedMillis)
        {
            try
            {
                using (StreamWriter w = File.AppendText(clubPerformanceFilePath))
                {
                    w.Write("\n{0}\tavg: {1:0.000}\t[{2}]", DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss"), ellapsedMillis.Average(), String.Join(", ", ellapsedMillis));
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
