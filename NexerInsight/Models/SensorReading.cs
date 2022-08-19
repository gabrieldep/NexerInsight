namespace NexerInsight.Models
{
    public class SensorReading
    {
        public DateTime Date { get; set; }
        public double MeasuredValue { get; set; }

        internal static SensorReading FromStringData(string data)
        {
            SensorReading sensorReading = new SensorReading();
            string[] obj = data.Split(';');
            sensorReading.Date = Convert.ToDateTime(obj[0]);
            sensorReading.MeasuredValue = Convert.ToDouble(obj[1]);
            return sensorReading;
        }
    }
}
