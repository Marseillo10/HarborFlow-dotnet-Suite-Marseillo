using System.Text.Json;
using HarborFlowSuite.Core.Models;
using Microsoft.Extensions.Configuration;

namespace HarborFlowSuite.Infrastructure.Persistence;

public class PortSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public PortSeeder(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task SeedAsync()
    {
        Console.WriteLine("PortSeeder: SeedAsync started.");

        // ALWAYS clear the table first to ensure we remove old duplicates
        if (_context.Ports.Any())
        {
            Console.WriteLine("PortSeeder: Clearing existing ports to ensure clean slate...");
            _context.Ports.RemoveRange(_context.Ports);
            await _context.SaveChangesAsync();
        }

        var portsJsonPath = Path.Combine(AppContext.BaseDirectory, "ports.json");
        Console.WriteLine($"PortSeeder: Checking for ports.json at {portsJsonPath}");

        // If running in development, it might be in the project root, so check there too if not found
        if (!File.Exists(portsJsonPath))
        {
            var altPath = Path.Combine(Directory.GetCurrentDirectory(), "ports.json");
            Console.WriteLine($"PortSeeder: File not found at base directory. Checking {altPath}");
            portsJsonPath = altPath;
        }

        if (File.Exists(portsJsonPath))
        {
            Console.WriteLine("PortSeeder: ports.json found. Reading file...");
            var jsonString = await File.ReadAllTextAsync(portsJsonPath);
            var ports = JsonSerializer.Deserialize<List<Port>>(jsonString);

            if (ports != null)
            {
                Console.WriteLine($"PortSeeder: Deserialized {ports.Count} ports from JSON.");

                var targetCountries = _configuration.GetSection("PortSeeder:TargetCountries").Get<string[]>();

                List<Port> filteredPorts;
                if (targetCountries != null && targetCountries.Any())
                {
                    filteredPorts = ports
                        .Where(p => targetCountries.Contains(p.Country, StringComparer.OrdinalIgnoreCase))
                        .ToList();
                    Console.WriteLine($"PortSeeder: Filtered down to {filteredPorts.Count} ports for target countries: {string.Join(", ", targetCountries)}");
                }
                else
                {
                    var defaultCountries = new[] { "Indonesia", "Malaysia", "Papua New Guinea", "Timor-Leste", "Brunei", "Singapore" };
                    filteredPorts = ports
                        .Where(p => defaultCountries.Contains(p.Country, StringComparer.OrdinalIgnoreCase))
                        .ToList();
                    Console.WriteLine($"PortSeeder: No configuration found. Using default filter. Filtered down to {filteredPorts.Count} ports.");
                }

                // DATA CORRECTION: Fix Jakarta's coordinates if they are incorrect (positive latitude)
                // Jakarta should be approx -6.1, 106.8
                var jakartaPorts = filteredPorts.Where(p => p.City.Equals("Jakarta", StringComparison.OrdinalIgnoreCase)).ToList();
                foreach (var jakarta in jakartaPorts)
                {
                    if (jakarta.Latitude > 0) // If positive (North), it's wrong
                    {
                        Console.WriteLine($"PortSeeder: Fixing incorrect Jakarta coordinates: {jakarta.Latitude}, {jakarta.Longitude} -> -6.10, 106.80");
                        jakarta.Latitude = -6.10;
                        jakarta.Longitude = 106.80;
                    }
                }

                // DEDUPLICATION LOGIC: Group by Country + City and take the first one
                var uniquePorts = filteredPorts
                    .GroupBy(p => new { p.Country, p.City })
                    .Select(g => g.First())
                    .ToList();

                Console.WriteLine($"PortSeeder: Deduplicated ports. Reduced from {filteredPorts.Count} to {uniquePorts.Count} unique ports.");

                foreach (var port in uniquePorts)
                {
                    port.Id = Guid.NewGuid();
                    _context.Ports.Add(port);
                }

                await _context.SaveChangesAsync();
                Console.WriteLine("PortSeeder: Successfully saved ports to database.");
            }
            else
            {
                Console.WriteLine("PortSeeder: Failed to deserialize ports (null result).");
            }
        }
        else
        {
            Console.WriteLine("PortSeeder: ports.json NOT found.");
        }
    }
}
