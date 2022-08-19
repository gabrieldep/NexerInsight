namespace NexerInsight.Models
{
    public class SensorReading
    {
        public DateTime Date { get; set; }
        public double MeasuredValue { get; set; }

        /// <summary>
        /// Cast a csv string to object
        /// </summary>
        /// <param name="data">String with csv data</param>
        /// <returns>A sensorreading object</returns>
        internal static SensorReading FromStringData(string data)
        {
            SensorReading sensorReading = new();
            string[] obj = data.Split(';');

            sensorReading.Date = Convert.ToDateTime(obj[0]);

            if (obj[1][0] == ',')
                obj[1] = "0" + obj[1];
            sensorReading.MeasuredValue = Convert.ToDouble(obj[1]);
            return sensorReading;
        }
    }
}
