#!/usr/bin/dotnet run

#:package Testcontainers

using System.Reflection;
using System.Text.RegularExpressions;

Console.WriteLine("Getting Docker image tags from fixture source files...");

// NOTE: The SimpleTestcontainers fixture classes define the image tags as properties
//       **and they haven't been built yet** so we need to read the source files directly.

string? ExtractImageTagFromFile(string filePath)
{
    try
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Warning: File not found: {filePath}");
            return null;
        }

        var content = File.ReadAllText(filePath);
        
        // Match pattern: public string ImageTag => "value";
        var match = Regex.Match(content, @"public\s+string\s+ImageTag\s*=>\s*""([^""]+)""");
        
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        Console.WriteLine($"Warning: Could not find ImageTag in {filePath}");
        return null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading {filePath}: {ex.Message}");
        return null;
    }
}

// Paths relative to the script location (.github/scripts/)
var scriptDir = Directory.GetCurrentDirectory();
var mssqlFixturePath = Path.Combine(scriptDir, "src", "SimpleTestcontainers.xUnit.v3.MsSql", "SqlServerFixture.cs");
var postgresFixturePath = Path.Combine(scriptDir, "src", "SimpleTestcontainers.xUnit.v3.PostgreSQL", "PostgreSqlFixture.cs");

var mssqlImage = ExtractImageTagFromFile(mssqlFixturePath) ?? "mcr.microsoft.com/mssql/server:2022-latest";
var postgresImage = ExtractImageTagFromFile(postgresFixturePath) ?? "postgres:latest";

// Use reflection to get the private RyukImage field from ResourceReaper
var resourceReaperType = typeof(DotNet.Testcontainers.Containers.ResourceReaper);
var ryukImageField = resourceReaperType.GetField("RyukImage", BindingFlags.NonPublic | BindingFlags.Static);
var ryukImageValue = ryukImageField?.GetValue(null) as DotNet.Testcontainers.Images.IImage;
var ryukImage = ryukImageValue?.FullName?.ToString() ?? "testcontainers/ryuk:0.14.0";  // Fallback if reflection fails

Console.WriteLine($"MSSQL_IMAGE={mssqlImage}");
Console.WriteLine($"POSTGRES_IMAGE={postgresImage}");
Console.WriteLine($"RYUK_IMAGE={ryukImage}");

Console.WriteLine("Done.");

