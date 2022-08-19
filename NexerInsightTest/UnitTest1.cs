using NexerInsight.Models;
using Xunit;
using Moq;
using System;

namespace NexerInsightTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            string data = "2019-01-10T00:01:05;9,41";
            string date = "2019-01-10T00:01:05";

            //Act
            SensorReading fromStr = SensorReading.FromStringData(data);

            //Assert
            SensorReading sr = new()
            {
                Date = DateTime.Parse(date),
                MeasuredValue = 9.41
            };
            Assert.Equal(fromStr, sr);
        }
    }
}