namespace ElectricCalculatorBackend.Models;

using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    // This creates a SINGLE table named "Calculations"
    public DbSet<CalculationRecord> Calculations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Configures the simple, lightweight SQLite database file
        options.UseSqlite("Data Source=electrician.db");
    }
}

// This clean class represents exactly what ONE ROW in your Excel sheet looks like
public class CalculationRecord
{
    public int Id { get; set; }
    public double Voltage { get; set; }
    public double Current { get; set; }
    public string CircuitType { get; set; } = string.Empty; // Holds "Single-phase" or "Three-phase"
    public double PowerWatts { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow; // Works universally without breaking SQLite
}
