
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using HarborFlow.Core.Interfaces;
using HarborFlow.Core.Models;
using HarborFlow.Wpf.Commands;

namespace HarborFlow.Wpf.ViewModels
{
    public class NewsViewModel : ViewModelBase
    {
        private readonly IRssService _rssService;
        private readonly IRssFeedManager _rssFeedManager;
        private ObservableCollection<NewsArticle> _newsArticles;

        public ObservableCollection<NewsArticle> NewsArticles
        {
            get => _newsArticles;
            set
            {
                _newsArticles = value;
                OnPropertyChanged();
            }
        }

        public AsyncRelayCommand OpenArticleCommand { get; }

        public NewsViewModel(IRssService rssService, IRssFeedManager rssFeedManager)
        {
            _rssService = rssService;
            _rssFeedManager = rssFeedManager;
            _newsArticles = new ObservableCollection<NewsArticle>();
            OpenArticleCommand = new AsyncRelayCommand(OpenArticle);
            LoadNewsAsync();
        }

        private async Task LoadNewsAsync()
        {
            var allArticles = new List<NewsArticle>();
            var feedUrls = _rssFeedManager.GetFeedUrls();
            foreach (var url in feedUrls)
            {
                var articles = await _rssService.GetNewsAsync(url);
                allArticles.AddRange(articles);
            }
            NewsArticles = new ObservableCollection<NewsArticle>(allArticles.OrderByDescending(a => a.PublishDate));
        }

        private Task OpenArticle(object? articleObj)
        {
            if (articleObj is NewsArticle article && article?.Link != null)
            {
                Process.Start(new ProcessStartInfo(article.Link) { UseShellExecute = true });
            }
            return Task.CompletedTask;
        }
    }
}
