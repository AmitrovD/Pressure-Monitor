using LiteDB;
using PressureMonitor.Models;

namespace PressureMonitor.Services
{
    public class DatabaseService : IDisposable
    {
        private readonly LiteDatabase _db;
        private readonly ILiteCollection<Trial> _trials;

        public DatabaseService()
        {
            _db = new LiteDatabase("pressuremonitor.db");
            _trials = _db.GetCollection<Trial>("trials");

            _trials.EnsureIndex(x => x.Date);
            _trials.EnsureIndex(x => x.Name);
        }
        public void SaveTrial(Trial trial)
        {
            _trials.Insert(trial);
        }
        public List<Trial> GetAllTrials()
        {
            return _trials.FindAll().ToList();
        }
        public List<Trial> GetTrialsSortedByDate()
        {
            return _trials.FindAll().OrderBy(d => d.Date).ToList();
        }
        public List<Trial> GetTrialSortedByName()
        {
            return _trials.FindAll().OrderBy(n => n.Name).ToList();
        }
        public Trial GetById(int id)
        {
            return _trials.FindById(id);
        }
        public void DeleteTrial(int id)
        {
            _trials.Delete(id);
        }
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
