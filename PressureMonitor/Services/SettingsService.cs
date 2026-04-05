using Newtonsoft.Json;
using PressureMonitor.Models;
using System.IO;
using System.Windows;

namespace PressureMonitor.Services
{
    public class SettingsService
    {
        private readonly string _path = "appsettings.json";
        public SettingsService()
        {
        }

        public AppSettings Load()
        {
            if (!File.Exists(_path))
                return new AppSettings();
            try
            {
                using (var stream = File.OpenRead(_path)) 
                using (var reader = new StreamReader(stream)) 
                using(var jsonReader = new JsonTextReader(reader))
                {
                    var serializer = new JsonSerializer();
                    return serializer.Deserialize<AppSettings>(jsonReader) ?? new AppSettings();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                return new AppSettings();
            }
        }
        public void Save(AppSettings settings)
        {
            try
            {
                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(_path, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}
