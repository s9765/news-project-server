using BL.Logic.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
namespace BL.Logic
{

    public class TimedService : BackgroundService
    {

        public INewsService _newsService { get; set; }
        private readonly int _timerQuent;

        public TimedService(INewsService newsService, IConfiguration configuration)
        {
            _newsService = newsService;
            _timerQuent = int.Parse(configuration["TimerQuent"]);

        }
        /// <summary>
        /// Once an hour call GetAllNews func in order to update the cache
        /// </summary>
        /// <param name="stoppingToken"></param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                await _newsService.GetAllNews();
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString()) ;
                }
                await Task.Delay(TimeSpan.FromHours(_timerQuent), stoppingToken);
            }
        }
    }
}
