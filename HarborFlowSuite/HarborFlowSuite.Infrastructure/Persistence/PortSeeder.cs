using System.Text.Json;
using HarborFlowSuite.Core.Models;

namespace HarborFlowSuite.Infrastructure.Persistence;

public class PortSeeder
{
    private readonly ApplicationDbContext _context;

    public PortSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        Console.WriteLine("PortSeeder: SeedAsync started.");
        
        // Clear existing ports to ensure a fresh seed
        if (_context.Ports.Any())
        {
            Console.WriteLine("PortSeeder: Deleting existing ports.");
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
                var targetCountries = new[] { "Indonesia", "Malaysia", "Papua New Guinea", "Timor-Leste", "Brunei", "Singapore" };
                var filteredPorts = ports
                    .Where(p => targetCountries.Contains(p.Country, StringComparer.OrdinalIgnoreCase))
                    .ToList();

                Console.WriteLine($"PortSeeder: Filtered down to {filteredPorts.Count} ports for target countries.");

                foreach (var port in filteredPorts)
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
