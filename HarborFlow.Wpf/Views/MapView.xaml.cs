
using System.Windows.Controls;
using HarborFlow.Wpf.ViewModels;
using System.Collections.Specialized;
using HarborFlow.Core.Models;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.IO;
using System;
using System.Threading.Tasks;

namespace HarborFlow.Wpf.Views
{
    public partial class MapView : UserControl
    {
        private readonly MapViewModel _viewModel;
        private bool _isWebViewInitialized = false;

        public MapView(MapViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            InitializeWebViewAsync();

            _viewModel.VesselsUpdated += VesselsOnMap_CollectionChanged;
            _viewModel.VesselSelected += ViewModel_VesselSelected;
            _viewModel.HistoryTrackRequested += ViewModel_HistoryTrackRequested;
            _viewModel.MapLayerChanged += ViewModel_MapLayerChanged;
        }

        private async void InitializeWebViewAsync()
        {
            await WebView.EnsureCoreWebView2Async(null);
            var mapHtmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot/map/index.html");
            WebView.CoreWebView2.Navigate(new Uri(mapHtmlPath).AbsoluteUri);
            _isWebViewInitialized = true;
        }

        private void VesselsOnMap_CollectionChanged(object? sender, IEnumerable<VesselMapData> e)
        {
            UpdateVesselsOnMapAsync(e);
        }

        private async void UpdateVesselsOnMapAsync(IEnumerable<VesselMapData> vessels)
        {
            if (!_isWebViewInitialized) return;

            var json = JsonSerializer.Serialize(vessels);
            await WebView.CoreWebView2.ExecuteScriptAsync($"updateVessels('{json}')");
        }

        private async void ViewModel_VesselSelected(object? sender, Vessel e)
        {
            if (!_isWebViewInitialized) return;

            var lastPosition = e.Positions.OrderByDescending(p => p.PositionTimestamp).FirstOrDefault();
            if (lastPosition != null)
            {
                await WebView.CoreWebView2.ExecuteScriptAsync($"centerOnVessel({lastPosition.Latitude}, {lastPosition.Longitude})");
                await WebView.CoreWebView2.ExecuteScriptAsync($"openVesselPopup({lastPosition.Latitude}, {lastPosition.Longitude})");
            }
        }

        private async void ViewModel_HistoryTrackRequested(object? sender, IEnumerable<VesselPosition> e)
        {
            if (!_isWebViewInitialized) return;

            var historyTrack = e.Select(p => new { p.Latitude, p.Longitude }).ToList();
            var json = JsonSerializer.Serialize(historyTrack);
            await WebView.CoreWebView2.ExecuteScriptAsync($"drawHistoryTrack('{json}')");
        }

        private async void ViewModel_MapLayerChanged(object? sender, string e)
        {
            if (!_isWebViewInitialized) return;

            await WebView.CoreWebView2.ExecuteScriptAsync($"setMapLayer('{e}')");
        }
    }
}
