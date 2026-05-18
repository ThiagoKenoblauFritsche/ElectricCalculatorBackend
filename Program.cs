using System;
using ElectricCalculatorBackend.Models;
using ElectricCalculatorBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Production CORS Policy Layer
// Opens up network channels completely so your web browser can exchange data smoothly with Vercel
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddOpenApi();

// Register the Calculation Service implementation via Dependency Injection
builder.Services.AddSingleton<IElectricCalculatorService, ElectricCalculatorService>();

var app = builder.Build();

// Enable the open CORS parameters early in the processing pipeline execution stream
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Main Interactive Active Power Endpoint Routing Logic
app.MapGet("/calculate-power", (double voltage, double current, string type, IElectricCalculatorService calculator) =>
{
    try
    {
        PowerResponse result;

        // 1. Process Core Electrical Calculations via Polymorphism
        if (type?.ToLower() == "threephase")
        {
            var threePhaseObj = new ThreePhaseCircuit(voltage, current);
            result = calculator.CalculateActivePower(threePhaseObj);
        }
        else
        {
            var singlePhaseObj = new SinglePhaseCircuit(voltage, current);
            result = calculator.CalculateActivePower(singlePhaseObj);
        }

        // 2. Safe Database Persistence Sandbox Block
        try
        {
            using var db = new AppDbContext();
            var newRecord = new CalculationRecord
            {
                Voltage = voltage,
                Current = current,
                CircuitType = result.CircuitType,
                PowerWatts = result.PowerWatts
            };

            db.Calculations.Add(newRecord);
            db.SaveChanges();
        }
        catch (Exception dbException)
        {
            // If the local SQLite context encounters persistent folder write barriers on the cloud disk,
            // the system logs the error warning but DOES NOT intercept the operational calculation return payload!
            Console.WriteLine($"[Database Bypass] Persistent logs restricted: {dbException.Message}");
        }

        // 3. Graceful Return Execution Payload
        return Results.Ok(result);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
    catch (Exception)
    {
        return Results.Problem("An internal system exception occurred while computing your electrical parameter data.");
    }
});

app.Run();
