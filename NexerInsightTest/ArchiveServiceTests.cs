using NexerInsight.Models;
using Xunit;
using Moq;
using System;
using NexerInsight.Services;
using System.IO;
using System.Collections.Generic;

namespace NexerInsightTest
{
    public class ArchiveServiceTests
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            string csv = "2019-01-10T00:01:05;9,41\n2019-01-10T00:01:10;9,40\n2019-01-10T00:01:20;9,39\n2019-01-10T00:01:35;9,38";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(csv);
            writer.Flush();
            stream.Position = 0;

            var list = new List<SensorReading>();
            SensorReading sr = new()
            {
                Date = Convert.ToDateTime("2019-01-10T00:01:05"),
                MeasuredValue = 9.41
            };
            list.Add(sr);

            sr = new()
            {
                Date = Convert.ToDateTime("2019-01-10T00:01:10"),
                MeasuredValue = 9.40
            };
            list.Add(sr);

            sr = new()
            {
                Date = Convert.ToDateTime("2019-01-10T00:01:20"),
                MeasuredValue = 9.39
            };
            list.Add(sr);

            sr = new()
            {
                Date = Convert.ToDateTime("2019-01-10T00:01:35"),
                MeasuredValue = 9.38
            };
            list.Add(sr);
            //Act
            var result = ArchiveService.GetArrayFromStream(stream);

            //Assert
            Assert.Equal(list, result);
        }
    }
}
