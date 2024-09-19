using Microsoft.AspNetCore.Mvc;
using BancoAtlantidaAPI.Data;
using BancoAtlantidaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BancoAtlantidaAPI.Controllers.CreditCardsController;

namespace BancoAtlantidaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            return await _context.Transactions.ToListAsync();
        }

        // GET: api/transactions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            return transaction;
        }

        // GET: api/transactions/tipo/{tipo}
        [HttpGet("type/{tipo}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactionsByType([FromRoute] string tipo)
        {
            if (!Enum.TryParse(tipo, true, out TransactionType transactionType))
            {
                return BadRequest("Invalid transaction type. Use 'Compra' or 'Pago'.");
            }

            var transactions = await _context.Transactions
                                             .Where(t => t.Type == transactionType)
                                             .ToListAsync();

            if (transactions == null || transactions.Count == 0)
            {
                return NotFound($"No transactions found for type {tipo}");
            }

            return transactions;
        }

        // POST: api/transactions
        [HttpPost("create")]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            // Convertir el tipo de transacción a enumerador
            if (!Enum.TryParse(transaction.Type.ToString(), true, out TransactionType transactionType))
            {
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid Transaction Type",
                    Detail = "The transaction type must be 'Purchase' or 'Payment'."
                });
            }

            // Buscar la tarjeta de crédito asociada
            var creditCard = await _context.CreditCards.FindAsync(transaction.CreditCardId);
            if (creditCard == null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Credit Card Not Found",
                    Detail = $"No credit card found with ID {transaction.CreditCardId}."
                });
            }

            // Ajustar el CurrentBalance de la tarjeta de crédito
            if (transactionType == TransactionType.purchase)
            {
                if (creditCard.CurrentBalance + transaction.Amount > creditCard.CreditLimit)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Status = 400,
                        Title = "Credit Limit Exceeded",
                        Detail = "Esta transacción excede el límte de crédito"
                    });
                }
                creditCard.CurrentBalance += transaction.Amount;
            }
            else if (transactionType == TransactionType.payment)
            {
                if (creditCard.CurrentBalance - transaction.Amount < 0)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Status = 400,
                        Title = "Insufficient Funds",
                        Detail = "Este pago es mayor que la deuda crediticia"
                    });
                }
                creditCard.CurrentBalance -= transaction.Amount;
            }

            // Añadir la transacción y guardar cambios en la base de datos
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            // Calcular los valores automáticos
            creditCard.AvailableBalance = creditCard.CreditLimit - creditCard.CurrentBalance;
            creditCard.BonusInterest = creditCard.CurrentBalance * (creditCard.InterestRate / 100);
            creditCard.MinimumPaymentDue = creditCard.CurrentBalance * (creditCard.MinimumPaymentPercentage / 100);
            creditCard.TotalAmountWithInterest = creditCard.CurrentBalance + (creditCard.CurrentBalance * (creditCard.InterestRate / 100));

            // Calcular las compras totales de este mes y del mes pasado
            CalculateMonthlyPurchases(creditCard);

            _context.Entry(creditCard).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }

        private void CalculateMonthlyPurchases(CreditCard creditCard)
        {
            var now = DateTime.Now;
            var currentMonth = new DateTime(now.Year, now.Month, 1);
            var previousMonth = currentMonth.AddMonths(-1);

            creditCard.TotalPurchasesThisMonth = _context.Transactions
                .Where(t => t.CreditCardId == creditCard.Id && t.Type == TransactionType.purchase && t.Date >= currentMonth)
                .Sum(t => t.Amount);

            creditCard.TotalPurchasesLastMonth = _context.Transactions
                .Where(t => t.CreditCardId == creditCard.Id && t.Type == TransactionType.purchase && t.Date >= previousMonth && t.Date < currentMonth)
                .Sum(t => t.Amount);
        }


        // PUT: api/creditcards/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCreditCard(int id, CreditCard creditCard)
        {
            if (id != creditCard.Id)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid ID",
                    Detail = "The ID in the URL does not match the ID in the body."
                });
            }

            // Validar el estado del modelo
            if (!ModelState.IsValid)
            {
                return BadRequest(new ValidationProblemDetails(ModelState)
                {
                    Status = 400,
                    Title = "Invalid Model State",
                    Detail = "One or more validation errors occurred."
                });
            }

            // Buscar la tarjeta de crédito existente
            var existingCreditCard = await _context.CreditCards.FindAsync(id);
            if (existingCreditCard == null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Credit Card Not Found",
                    Detail = $"No credit card found with ID {id}."
                });
            }

            // Actualizar los campos de la tarjeta de crédito
            existingCreditCard.FirstName = creditCard.FirstName;
            existingCreditCard.LastName = creditCard.LastName;
            existingCreditCard.CardNumber = creditCard.CardNumber;
            existingCreditCard.CurrentBalance = creditCard.CurrentBalance;
            existingCreditCard.CreditLimit = creditCard.CreditLimit;
            existingCreditCard.InterestRate = creditCard.InterestRate;
            existingCreditCard.MinimumPaymentPercentage = creditCard.MinimumPaymentPercentage;
            existingCreditCard.StatementDate = creditCard.StatementDate;
            existingCreditCard.TotalPurchasesThisMonth = creditCard.TotalPurchasesThisMonth;
            existingCreditCard.TotalPurchasesLastMonth = creditCard.TotalPurchasesLastMonth;

            // Recalcular los valores automáticos
            existingCreditCard.AvailableBalance = existingCreditCard.CreditLimit - existingCreditCard.CurrentBalance;
            existingCreditCard.BonusInterest = existingCreditCard.CurrentBalance * (existingCreditCard.InterestRate / 100);
            existingCreditCard.MinimumPaymentDue = existingCreditCard.CurrentBalance * (existingCreditCard.MinimumPaymentPercentage / 100);
            existingCreditCard.TotalAmountWithInterest = existingCreditCard.CurrentBalance + (existingCreditCard.CurrentBalance * (existingCreditCard.InterestRate / 100));

            _context.Entry(existingCreditCard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CreditCardExists(id))
                {
                    return NotFound(new ProblemDetails
                    {
                        Status = 404,
                        Title = "Credit Card Not Found",
                        Detail = $"No credit card found with ID {id}."
                    });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool CreditCardExists(int id)
        {
            return _context.CreditCards.Any(e => e.Id == id);
        }


        // DELETE: api/transactions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/transactions/{type}/{creditCardId}
        [HttpGet("{type}/{creditCardId}")]
        public async Task<ActionResult<PaginatedResponse<Transaction>>> GetTransactionsByTypeAndCreditCardId(
            [FromRoute] string type,
            [FromRoute] int creditCardId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // Validar el tipo de transacción
            if (!Enum.TryParse(type, true, out TransactionType transactionType))
            {
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid Transaction Type",
                    Detail = "The transaction type must be 'Compra' or 'Pago'."
                });
            }

            // Buscar la tarjeta de crédito asociada
            var creditCard = await _context.CreditCards.FindAsync(creditCardId);
            if (creditCard == null)
            {
                return NotFound(new ProblemDetails
                {
                    Status = 404,
                    Title = "Credit Card Not Found",
                    Detail = $"No credit card found with ID {creditCardId}."
                });
            }

            // Obtener el número total de transacciones que coinciden con los criterios
            var totalRecords = await _context.Transactions
                                             .Where(t => t.Type == transactionType && t.CreditCardId == creditCardId)
                                             .CountAsync();

            // Calcular el número total de páginas
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Buscar transacciones por tipo y tarjeta de crédito, ordenadas por fecha descendente y paginadas
            var transactions = await _context.Transactions
                                             .Where(t => t.Type == transactionType && t.CreditCardId == creditCardId)
                                             .OrderByDescending(t => t.Date)
                                             .Skip((pageNumber - 1) * pageSize)
                                             .Take(pageSize)
                                             .ToListAsync();

            // Crear la respuesta paginada
            var response = new PaginatedResponse<Transaction>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Data = transactions
            };

            return Ok(response);
        }




        [HttpGet("all/{creditCardId}")]
        public async Task<ActionResult<PagedResponse<Transaction>>> GetTransactionsByCreditCardId(int creditCardId, int pageNumber = 1, int pageSize = 10)
        {
            var totalTransactions = await _context.Transactions
                                                  .Where(t => t.CreditCardId == creditCardId)
                                                  .CountAsync();

            var transactions = await _context.Transactions
                                             .Where(t => t.CreditCardId == creditCardId)
                                             .Skip((pageNumber - 1) * pageSize)
                                             .Take(pageSize)
                                             .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalTransactions / (double)pageSize);

            var response = new PagedResponse<Transaction>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalTransactions,
                TotalPages = totalPages,
                Items = transactions
            };

            return Ok(response);
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }

    public class PagedResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Items { get; set; }
    }
}
