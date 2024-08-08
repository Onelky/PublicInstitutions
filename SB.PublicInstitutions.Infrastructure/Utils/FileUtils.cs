using Newtonsoft.Json;

namespace SB.PublicInstitutions.Infrastructure.Utils
{
    public static class FileUtils
    {
        public static async Task<List<T>> GetDeserializedItems<T>(string filePath) where T : class
        {
            if (!File.Exists(filePath))
            {
                return new List<T>();
            }

            var lines = await File.ReadAllLinesAsync(filePath);
            return lines.Select(line => JsonConvert.DeserializeObject<T>(line)).ToList();
        }

        public static async Task WriteAll<T>(IEnumerable<T> items, string filePath) where T : class 
        {
            var lines = items.Select(item => JsonConvert.SerializeObject((object)item));
            await File.WriteAllLinesAsync(filePath, lines);
        }

        public static async Task AppendLine<T>(T item, string filePath) where T : class
        {
            var line = JsonConvert.SerializeObject((object)item);
            await File.AppendAllLinesAsync(filePath, new[] { line });
        }

    }
}
