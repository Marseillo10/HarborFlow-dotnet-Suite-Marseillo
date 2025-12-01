* **Workflow Loops**

*   **Login Credentials:** Use the following credentials for registration and login purposes:
    *   **Nama:** Marseillo
    *   **Email:** `rafaelbintang207@gmail.com`
    *   **Password:** `********`
Always rerun the project after you done with the changes!
* dotnet run --project HarborFlowSuite/HarborFlowSuite.Server --launch-profile https

Using launch settings from HarborFlowSuite/HarborFlowSuite.Server/Properties/launchSettings.json...
Building...
PortSeeder: SeedAsync started.
PortSeeder: Clearing existing ports to ensure clean slate...
PortSeeder: Checking for ports.json at /Users/marseillosatrian/Downloads/HarborFlow-dotnet-Suite-Marseillo/HarborFlowSuite/HarborFlowSuite.Server/bin/Debug/net9.0/ports.json
PortSeeder: ports.json found. Reading file...
PortSeeder: Deserialized 3898 ports from JSON.
PortSeeder: Filtered down to 262 ports for target countries: Indonesia, Timor-Leste, Singapore, Malaysia, Brunei
PortSeeder: Fixing incorrect Jakarta coordinates: 5.19, 105.61 -> -6.10, 106.80
PortSeeder: Deduplicated ports. Reduced from 262 to 196 unique ports.
PortSeeder: Successfully saved ports to database.
Connected to AIS Stream.
Sending subscription (Length: 214)
Subscribed to AIS Stream.

* dotnet run --project HarborFlowSuite/HarborFlowSuite.Client --launch-profile https   

Using launch settings from HarborFlowSuite/HarborFlowSuite.Client/Properties/launchSettings.json...
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7163
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5205
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/marseillosatrian/Downloads/HarborFlow-dotnet-Suite-Marseillo/HarborFlowSuite/HarborFlowSuite.Client

* **"DefaultConnection": "Host=localhost;Port=5432;Database=harborflowdb;Username=marseillosatrian;Password=********".  

These are documentations for GFW API:  https://globalfishingwatch.org/our-apis/ ; https://globalfishingwatch.org/our-apis/documentation#introduction ; https://globalfishingwatch.github.io/gfw-api-python-client/ ; https://github.com/GlobalFishingWatch/gfwr ; https://github.com/GlobalFishingWatch/vessel-classification ; https://github.com/GlobalFishingWatch/ShipDataProcess ;

These are documentation for AISstream API: https://aisstream.io/documentation and https://github.com/aisstream 

---
  Recommended APIs to Enhance HarborFlow

  1. Weather & Sea Conditions

   * API: Open-Meteo
   * Why it's a good fit: It's completely open-source and free for non-commercial use with no API key required. It provides hyper-detailed global weather forecasts,
     including wind speed, wave height, and ocean currents, which could be displayed on the map or in a vessel's detail panel.
   * Potential Feature: Add a "Weather Layer" to the map or show current/forecasted conditions for a selected vessel's location.

  2. Maritime News Feed

   * API: NewsAPI.org
   * Why it's a good fit: It offers a simple, developer-friendly API with a generous free tier. We can use it to fetch news articles from around the world by
     filtering for keywords like "maritime," "shipping," "port congestion," etc.
   * Potential Feature: A "Maritime News" widget or page within the application to provide users with relevant industry news.

  3. Port & Harbor Information

   * API/Data: Static JSON Datasets on GitHub (e.g., from user tayljordan/ports)
   * Why it's a good fit: Instead of using an API for static data, we can pre-load your application's database with information on thousands of ports worldwide
     (name, location, country). This is highly efficient and has no rate limits. For dynamic data like port congestion, the Sinay API offers a free plan with 500
     calls/month.
   * Potential Feature: Make ports clickable on the map to show their names and details. Use the dynamic API to show real-time congestion status for major ports.

  4. Advanced Map Layers

   * API: NASA EarthData (GIBS) or Copernicus Data Space Ecosystem
   * Why it's a good fit: These APIs provide free access to a vast catalog of satellite imagery. We could use this to offer different base maps, such as a live cloud
     cover layer, sea surface temperature, or a more detailed satellite view.
   * Potential Feature: A map layer selector that allows users to switch between a standard map, a satellite view, and other scientific overlays.

  Summary & Recommendation


  ┌────────────┬─────────────────────────────┬────────────────────────────────────────────────────────────┐
  │ Category   │ Recommended API             │ Key Benefit for HarborFlow                                 │
  ├────────────┼─────────────────────────────┼────────────────────────────────────────────────────────────┤
  │ Weather    │ Open-Meteo                  │ Adds real-world environmental context to vessel movements. │
  │ News       │ NewsAPI.org                 │ Provides relevant, real-time industry news within the app. │
  │ Ports      │ Static JSON Data            │ Efficiently adds thousands of global ports to the map.     │
  │ Map Layers │ NASA EarthData / Copernicus │ Offers rich, scientific map visualizations.                │
  └────────────┴─────────────────────────────┴────────────────────────────────────────────────────────────┘


  I recommend we start by integrating the Static JSON Port Data and the Open-Meteo Weather API. These two would provide the most immediate and high-impact
  enhancements to the application's core functionality.
