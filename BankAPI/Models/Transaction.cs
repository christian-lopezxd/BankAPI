using System;
using System.ComponentModel.DataAnnotations;

namespace BancoAtlantidaAPI.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [EnumDataType(typeof(TransactionType))]
        public TransactionType Type { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Solo incluye la clave externa de la tarjeta de crédito
        public int CreditCardId { get; set; }
    }

    public enum TransactionType
    {
        purchase,
        payment
    }
}
