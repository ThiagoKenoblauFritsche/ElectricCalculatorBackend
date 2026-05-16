namespace ElectricCalculatorBackend.Models;

public abstract class Circuit
{
    public int id { get; set; }
    public double Voltage { get; private set; }
    public double Current { get; private set; }

    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    protected Circuit(double voltage, double current)
    {
        if (voltage < 0 || current < 0)
        {
            throw new ArgumentException("Voltage and current cannot be negative values.");
        }

        Voltage = voltage;
        Current = current;
    }
}
