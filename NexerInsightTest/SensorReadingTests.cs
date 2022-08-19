using NexerInsight.Models;
using Xunit;
using Moq;
using System;

namespace NexerInsightTest
{
    public class SensorReadingTests
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            string csvLine = "2019-01-10T00:01:05;9,41";
            string date = "2019-01-10T00:01:05";
            SensorReading sr = new()
            {
                Date = Convert.ToDateTime(date),
                MeasuredValue = 9.41
            };

            //Act
            SensorReading fromStr = SensorReading.FromStringData(csvLine);

            //Assert
            Assert.Equal(sr, fromStr);
        }
    }
}