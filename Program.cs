using System;
using ElectricCalculatorBackend.Models;
using ElectricCalculatorBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// CORS Configuration to allow your React application to connect
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddOpenApi();

// Dependency Injection registration
builder.Services.AddSingleton<IElectricCalculatorService, ElectricCalculatorService>();

var app = builder.Build();

app.UseCors("AllowReact");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// API Endpoint Route
// Update your API Route inside Program.cs to use the database context

app.MapGet("/calculate-power", (double voltage, double current, string type, IElectricCalculatorService calculator) =>
{
    try
    {
        PowerResponse result;

        // 1. Process the standard math logic via Polymorphism
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

        // 2. DATABASE OPERATION: Save this action to the log history
        // 'using var' opens the database bridge and closes it safely when done
        using var db = new AppDbContext();

        // We create a new row object using our flat DTO model class
        var newRecord = new CalculationRecord
        {
            Voltage = voltage,
            Current = current,
            CircuitType = result.CircuitType, // Stores "Single-phase" or "Three-phase"
            PowerWatts = result.PowerWatts
        };

        // Add the row to the internal list
        db.Calculations.Add(newRecord);

        // Tell the database to write this new line permanently to your computer storage
        db.SaveChanges();

        // 3. Return the HTTP response back to React normally
        return Results.Ok(result);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
    catch (Exception)
    {
        return Results.Problem("An internal error occurred while processing or saving the calculation.");
    }
});

app.Run();
