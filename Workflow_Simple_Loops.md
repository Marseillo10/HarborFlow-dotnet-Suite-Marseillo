* **Workflow Loops**
Please adhere to the following instructions for the continued development of the web application: (make sur don't regressing the project)

*   **Thorough Review:** Conduct a thorough and detailed review of both the project's source code and the web application itself (Use codebase investigator to only read as groups or parts of project if necessary (only do this to the part of codes not entire project)).
*   **Continued Development:** Continue the development of the application, with a specific focus on implementing the functionality outlined in the documentations located at: `/Users/marseillosatrian/Downloads/HarborFflow_dotnet_Suite_Marseillo_v2/target_tech_spec_HarborFlow_dotnet_suite.md` and `/Users/marseillosatrian/Downloads/HarborFlow-dotnet-Suite-Marseillo 2/docs`
*   **UI/UX Enhancement:** Focus on improving the applica
*   tion's user interface (UI) and user experience (UX). This includes:
    *   Enhancing or adding new icons and background images.
    *   Improving or adding animations.
*   **Information Accuracy:** Ensure that all information displayed to the user is accurate and clearly communicates the application's intent.
*   **Bug Fixes:** Continuously identify and fix any problems or bugs within the application (If you Stuck in fixing bugs, please just note the bugs and continue work anything else, we will continue fixing its later!).
*   **Task**
*   **Always Use Browser and chrome developer tools:** Throughout the development and debugging process, consistently use `browser extension` and `chrome developer tools` to visit and review the web application running at `https://localhost:7163/ or http://localhost:5205`. This will prevent "blind" development, as `HarborFlow.Web` and `HarborFlow.Api` are always running by you.
*   **Login Credentials:** Use the following credentials for registration and login purposes:
    *   **Nama:** Marseillo
    *   **Email:** `rafaelbintang207@gmail.com`
    *   **Password:** `password123`
Always rerun the project after you done with the changes!
* dotnet run --project HarborFlowSuite/HarborFlowSuite.Server --launch-profile https
* dotnet run --project HarborFlowSuite/HarborFlowSuite.Client --launch-profile https   

* **"DefaultConnection": "Host=localhost;Port=5432;Database=harborflowdb;Username=marseillosatrian;Password=bizero11"**  
* dotnet ef migrations add FixServiceRequestModel --project HarborFlowSuite.Infrastructure --startup-project HarborFlowSuite.Server 


Do you have any plan to implementing GFW API to complement AISstream API in my application? Tell me first the plan! And also please read this documentations:  https://globalfishingwatch.org/our-apis/ ; https://globalfishingwatch.org/our-apis/documentation#introduction ; https://globalfishingwatch.github.io/gfw-api-python-client/ ; https://github.com/GlobalFishingWatch/gfwr ; https://github.com/GlobalFishingWatch/vessel-classification ; https://github.com/GlobalFishingWatch/ShipDataProcess ;

---

I have completed my search and found several excellent free APIs that could significantly enhance the HarborFlow project. Here is a summary of the best options,
  categorized by the features they could add.

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
