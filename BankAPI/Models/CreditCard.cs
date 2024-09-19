using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BancoAtlantidaAPI.Models
{
    public class CreditCard
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [StringLength(16, MinimumLength = 16)]
        public string CardNumber { get; set; }

        [Required]
        public decimal CurrentBalance { get; set; }

        [Required]
        public decimal CreditLimit { get; set; }

        public decimal AvailableBalance { get; set; }

        [Required]
        public decimal InterestRate { get; set; }

        [Required]
        public decimal MinimumPaymentPercentage { get; set; }

        public decimal BonusInterest { get; set; }

        public decimal MinimumPaymentDue { get; set; }

        public decimal TotalAmountWithInterest { get; set; }

        public DateTime StatementDate { get; set; }

        public decimal TotalPurchasesThisMonth { get; set; }

        public decimal TotalPurchasesLastMonth { get; set; }

        // Relationship with Transactions
        public List<Transaction> Transactions { get; set; }
    }
}
