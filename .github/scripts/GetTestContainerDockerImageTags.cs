#!/usr/bin/dotnet run

#:package Testcontainers.MsSql
#:package Testcontainers.PostgreSql

Console.WriteLine("Getting Docker image tags from Testcontainers libraries...");

var mssqlImage = Testcontainers.MsSql.MsSqlBuilder.MsSqlImage;
var postgresImage = Testcontainers.PostgreSql.PostgreSqlBuilder.PostgreSqlImage;
Console.WriteLine($"MSSQL_IMAGE={mssqlImage}");
Console.WriteLine($"POSTGRES_IMAGE={postgresImage}");

Console.WriteLine("Done.");

