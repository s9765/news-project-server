using BL.Logic.Interfaces;
using BL.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.ServiceModel.Syndication;
using System.Xml;

namespace BL.Logic
{
    public class NewsService : INewsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly IHubContext<NewsHub> _hubContext;
        private readonly string _newsLink;
        private readonly int _cacheDeleteQuent;

        public NewsService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, IConfiguration configuration, IHubContext<NewsHub> hubContext)
        {
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
            _hubContext = hubContext;
            _newsLink = configuration["newsLink"];
            _cacheDeleteQuent = int.Parse(configuration["CacheDeleteQuent"]);

        }
        /// <summary>
        /// Call Rss  to get news list data ,Save in  cache ,
        /// Implement signalR
        /// </summary>
        /// <returns>List of news </returns>
        public async Task<IEnumerable<NewItemResult>> GetAllNews()
        {
            var newsItems = new List<NewItem>();
            var feed = new SyndicationFeed();
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var feedUrl = _newsLink;
                var response = await httpClient.GetAsync(feedUrl);
                using var stream = await response.Content.ReadAsStreamAsync();
                using var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse });

                feed = SyndicationFeed.Load(reader);
                newsItems = feed.Items.Select(item => new NewItem
                {
                    Id = item.Id,
                    Title = item.Title.Text,
                    Body = item.Summary?.Text,
                    Link = item.Links.FirstOrDefault()?.Uri?.AbsoluteUri
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            _memoryCache.Set("NewsFeed", newsItems, TimeSpan.FromHours(_cacheDeleteQuent));
            //Here should write logic about informing the cliend and updateing the cache
            await _hubContext.Clients.All.SendAsync("UpdateNews", newsItems);

            var newsItemsResult = feed.Items.Select(item => new NewItemResult
            {
                Id = item.Id,
                Title = item.Title.Text,
            }).ToList();

            return newsItemsResult;
        }
        /// <summary>
        /// Get specific new id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>return the new information</returns>

        public async Task<NewItem> GetNewById(string id)
        {
            //Check if the specified news title is in the cache
            if (!_memoryCache.TryGetValue("NewsFeed", out IEnumerable<NewItem> cachedNews))
            {
                await GetAllNews();
            }
            var selectedNews = cachedNews?.FirstOrDefault(item => item.Id == id);
            return selectedNews;
        }
    }

}