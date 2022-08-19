using NexerInsight.Models;
using System.IO.Compression;

namespace NexerInsight.Services
{
    public class ArchiveService
    {
        internal static IEnumerable<SensorReading> GetArrayFromStream(Stream archive)
        {
            IList<SensorReading> values = new List<SensorReading>();
            StreamReader reader = new(archive);
            string? line;
            while ((line = reader.ReadLine()) != null)
                values.Add(SensorReading.FromStringData(line));
            return values;
        }

        internal static ZipArchiveEntry? GetZipFileEntry(string fileName, Stream str)
        {
            using ZipArchive package = new(str, ZipArchiveMode.Read);
            ZipArchiveEntry? a = package.Entries.FirstOrDefault(e => e.Name == fileName);
            return a;
        }
    }
}
