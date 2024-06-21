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
        public TransactionType Tipo { get; set; }

        [Required]
        public DateTime Date { get; set; }

        // Relación con CreditCard
        public int CreditCardId { get; set; }
        public CreditCard CreditCard { get; set; }
    }

    public enum TransactionType
    {
        Compra,
        Pago
    }
}
