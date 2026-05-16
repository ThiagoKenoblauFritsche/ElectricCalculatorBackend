namespace ElectricCalculatorBackend.Services;

using System;
using ElectricCalculatorBackend.Models;

public interface IElectricCalculatorService
{
    PowerResponse CalculateActivePower(SinglePhaseCircuit circuit);
    PowerResponse CalculateActivePower(ThreePhaseCircuit circuit);
}

public class ElectricCalculatorService : IElectricCalculatorService
{
    // Single-phase calculation: P = V * I
    public PowerResponse CalculateActivePower(SinglePhaseCircuit circuit)
    {
        double powerWatts = circuit.Voltage * circuit.Current;
        return new PowerResponse(circuit.Voltage, circuit.Current, "Single-phase", powerWatts);
    }

    // Three-phase calculation: P = V * I * √3
    public PowerResponse CalculateActivePower(ThreePhaseCircuit circuit)
    {
        double squareRootOfThree = Math.Sqrt(3);
        double powerWatts = circuit.Voltage * circuit.Current * squareRootOfThree;
        return new PowerResponse(circuit.Voltage, circuit.Current, "Three-phase", powerWatts);
    }
}

// Data Transfer Object (DTO) using C# records
public record PowerResponse(double VoltageInput, double CurrentInput, string CircuitType, double PowerWatts);
