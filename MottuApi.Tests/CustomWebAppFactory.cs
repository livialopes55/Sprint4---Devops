using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using MottuApi.Data;

namespace MottuApi.Tests;

public class CustomWebAppFactory : WebApplicationFactory<Program>
{
    private DbConnection? _conn;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove o DbContext real
            var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            services.Remove(descriptor);

            // SQLite em memória compartilhada
            _conn = new SqliteConnection("DataSource=:memory:");
            _conn.Open();

            services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(_conn!));

            // constrói provider e cria schema + seed
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            // Seed básico: uma Filial e um Pátio
            if (!db.Filiais.Any())
            {
                var filial = new MottuApi.Models.Filial { Nome = "Filial Centro", Endereco = "Rua A, 123" };
                db.Filiais.Add(filial);
                db.SaveChanges();

                var patio = new MottuApi.Models.Patio { Descricao = "Pátio A", Dimensao = "40x30", FilialId = filial.Id };
                db.Patios.Add(patio);
                db.SaveChanges();
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _conn?.Dispose();
    }
}
