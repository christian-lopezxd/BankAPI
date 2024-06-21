using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BancoAtlantidaAPI.Models
{
    public class CreditCard
    {
        public int Id { get; set; }

        [Required]
        public string CardHolder { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 16)]
        public string CardNumber { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal CreditLimit { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal InterestRate { get; set; }
        public decimal MinimumPaymentPercentage { get; set; }
        public DateTime StatementDate { get; set; }
        public decimal TotalPurchasesThisMonth { get; set; }
        public decimal TotalPurchasesLastMonth { get; set; }

        // Relación con Transactions
        public List<Transaction> Transactions { get; set; }

        public CreditCard()
        {
            Transactions = new List<Transaction>();
        }
    }
}
