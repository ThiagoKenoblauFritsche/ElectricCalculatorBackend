using System;
using ElectricCalculatorBackend.Models;
using ElectricCalculatorBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração do CORS Totalmente Aberta para Produção (Vercel)
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

// Injeção de Dependência da calculadora
builder.Services.AddSingleton<IElectricCalculatorService, ElectricCalculatorService>();

var app = builder.Build();

// Ativa o CORS com a política aberta
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Rota de Cálculo Principal
app.MapGet("/calculate-power", (double voltage, double current, string type, IElectricCalculatorService calculator) =>
{
    try
    {
        PowerResponse result;

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

        // DATABASE OPERATION: Salva o histórico no SQLite
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
