﻿using BancoAtlantidaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BancoAtlantidaAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .HasOne<CreditCard>()
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CreditCardId);
        }
    }
}
