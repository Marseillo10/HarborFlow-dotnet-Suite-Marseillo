
import asyncio
from playwright.async_api import async_playwright
import http.server
import socketserver
import threading
import os
import json

PORT = 8000
# The path to the wwwroot directory, relative to the repository root.
WWWROOT_PATH = 'HarborFlow.Wpf/wwwroot'

class Handler(http.server.SimpleHTTPRequestHandler):
    # This handler will serve files from the current working directory.
    pass

async def main():
    # Store the original working directory
    original_cwd = os.getcwd()

    # Change the CWD to the wwwroot directory so the server finds the files.
    os.chdir(WWWROOT_PATH)

    httpd = socketserver.TCPServer(("", PORT), Handler)
    server_thread = threading.Thread(target=httpd.serve_forever)
    server_thread.daemon = True
    server_thread.start()

    async with async_playwright() as p:
        browser = await p.chromium.launch()
        page = await browser.new_page()
        # The URL is relative to the new CWD (WWWROOT_PATH)
        await page.goto(f"http://localhost:{PORT}/map/index.html")
        await page.wait_for_selector('#map')

        # Mock vessel data as a Python dictionary
        vessel_data = [
            {"Name": "Test Cargo", "IMO": "12345", "Latitude": -2.5, "Longitude": 118.0, "Course": 45, "Speed": 10, "VesselType": "Cargo", "IconUrl": "/icons/cargo.svg"},
            {"Name": "Test Tanker", "IMO": "67890", "Latitude": -2.6, "Longitude": 118.1, "Course": 90, "Speed": 12, "VesselType": "Tanker", "IconUrl": "/icons/tanker.svg"},
            {"Name": "Test Passenger", "IMO": "54321", "Latitude": -2.4, "Longitude": 118.2, "Course": 180, "Speed": 20, "VesselType": "Passenger", "IconUrl": "/icons/passenger.svg"},
            {"Name": "Unknown Vessel", "IMO": "98765", "Latitude": -2.55, "Longitude": 118.15, "Course": 270, "Speed": 8, "VesselType": "Unknown", "IconUrl": "/icons/vessel.svg"}
        ]

        # Convert the Python dictionary to a JSON string
        vessel_data_json = json.dumps(vessel_data)

        # Pass the JSON string as an argument to an anonymous function that calls updateVessels
        await page.evaluate('data => updateVessels(data)', vessel_data_json)

        await page.screenshot(path=os.path.join(original_cwd, "jules-scratch/verification/wpf_map_verification.png"))
        await browser.close()

    httpd.shutdown()
    # Restore the original working directory
    os.chdir(original_cwd)

if __name__ == "__main__":
    asyncio.run(main())
