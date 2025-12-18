#!/usr/bin/dotnet run

#:package Testcontainers.MsSql
#:package Testcontainers.PostgreSql
#:package Testcontainers

using System.Reflection;

Console.WriteLine("Getting Docker image tags from Testcontainers libraries...");

var mssqlImage = Testcontainers.MsSql.MsSqlBuilder.MsSqlImage;
var postgresImage = Testcontainers.PostgreSql.PostgreSqlBuilder.PostgreSqlImage;

// Use reflection to get the private RyukImage field from ResourceReaper
var resourceReaperType = typeof(DotNet.Testcontainers.Containers.ResourceReaper);
var ryukImageField = resourceReaperType.GetField("RyukImage", BindingFlags.NonPublic | BindingFlags.Static);
var ryukImageValue = ryukImageField?.GetValue(null) as DotNet.Testcontainers.Images.IImage;
var ryukImage = ryukImageValue?.FullName?.ToString() ?? "testcontainers/ryuk:0.14.0"; // Fallback if reflection fails

Console.WriteLine($"MSSQL_IMAGE={mssqlImage}");
Console.WriteLine($"POSTGRES_IMAGE={postgresImage}");
Console.WriteLine($"RYUK_IMAGE={ryukImage}");

Console.WriteLine("Done.");

