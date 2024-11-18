using Common.Models;
using Common.Utilities;
using SyncNode.Settings;
using System.Collections.Concurrent;

namespace SyncNode.Services
{
    public class SyncWorkJobService : IHostedService
    {
        private readonly ConcurrentDictionary<Guid, SyncEntity> _documents =
            new ConcurrentDictionary<Guid, SyncEntity>();

        private readonly IEmployeeAPISettings _employeeAPISettings;

        private Timer _timer;

        public SyncWorkJobService(IEmployeeAPISettings employeeAPISettings)
        {
            _employeeAPISettings = employeeAPISettings;
        }

        public void AddItem(SyncEntity entity)
        {
            if (_documents.TryGetValue(entity.Id, out var existingEntity))
            {
                // Actualizează entitatea doar dacă are o dată mai recentă
                if (entity.LastChangeAt > existingEntity.LastChangeAt)
                {
                    _documents[entity.Id] = entity;
                }
            }
            else
            {
                _documents[entity.Id] = entity;
            }
        }

        //public Task StartAsync(CancellationToken cancellationToken)
        //{
        //    _timer = new Timer(async state => await DoSendWorkAsync(), null, TimeSpan.Zero, TimeSpan.FromSeconds(15));
        //    return Task.CompletedTask;
        //}
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _ = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await DoSendWorkAsync();
                    await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
                }
            }, cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async Task DoSendWorkAsync()
        {
            var tasks = new List<Task>();

            foreach (var document in _documents)
            {
                if (_documents.TryRemove(document.Key, out var entity))
                {
                    var receivers = _employeeAPISettings.Hosts.Where(x => !x.Contains(entity.Origin));

                    foreach (var receiver in receivers)
                    {
                        var url = $"{receiver}/{entity.ObjectType}/sync";
                        tasks.Add(SendDataToReceiverAsync(url, entity));
                    }
                }
            }

            // Așteaptă toate cererile asincrone
            await Task.WhenAll(tasks);
        }

        private async Task SendDataToReceiverAsync(string url, SyncEntity entity)
        {
            try
            {
                var result = await HttpClientUtility.SendJsonAsync(entity.JsonData, url, entity.SyncType);

                if (!result.IsSuccessStatusCode)
                {
                    // Log: Eroare la trimiterea datelor
                }
            }
            catch (Exception ex)
            {
                // Log: Excepție
            }
        }
    }
}
