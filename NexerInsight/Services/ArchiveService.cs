using NexerInsight.Models;
using System.IO.Compression;

namespace NexerInsight.Services
{
    public class ArchiveService
    {
        /// <summary>
        /// Get an IEnumerable<SensorReading> from a csv.</SensorReading>
        /// </summary>
        /// <param name="archive">Stream with the csv file.</param>
        /// <returns>A list with data from csv</returns>
        public static IEnumerable<SensorReading> GetArrayFromStream(Stream archive)
        {
            IList<SensorReading> values = new List<SensorReading>();
            StreamReader reader = new(archive);
            string? line;
            while ((line = reader.ReadLine()) != null)
                values.Add(SensorReading.FromStringData(line));
            return values;
        }
    }
}
