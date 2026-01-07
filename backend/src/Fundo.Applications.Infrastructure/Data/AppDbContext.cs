using Fundo.Applications.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace Fundo.Applications.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Loan>()
               .Property(l => l.CurrentBalance)
               .HasColumnType("money")
               .HasPrecision(18, 4);
            modelBuilder.Entity<Loan>()
               .Property(l => l.Amount)
               .HasColumnType("money")
               .HasPrecision(18, 4);

            // Seed data
            modelBuilder.Entity<Loan>().HasData(
                new Loan
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Amount = 1500.00m,
                    CurrentBalance = 500.00m,
                    ApplicantName = "Maria Silva",
                    Status = LoanStatus.Active
                },
                new Loan
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Amount = 2500.00m,
                    CurrentBalance = 0.00m,
                    ApplicantName = "John Doe",
                    Status = LoanStatus.Paid
                }
            );
        }
    }
}
