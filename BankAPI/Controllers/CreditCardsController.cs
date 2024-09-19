using Microsoft.AspNetCore.Mvc;
using BancoAtlantidaAPI.Data;
using BancoAtlantidaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BancoAtlantidaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditCardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CreditCardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/creditcards
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<CreditCard>>> GetCreditCards(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string query = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = 400,
                    Title = "Invalid Pagination Parameters",
                    Detail = "Page number and page size must be greater than 0."
                });
            }

            var creditCardQuery = _context.CreditCards.AsQueryable();

            // Aplicar filtro de búsqueda
            if (!string.IsNullOrEmpty(query))
            {
                query = query.ToLower();
                creditCardQuery = creditCardQuery.Where(c =>
                    c.FirstName.ToLower().Contains(query) ||
                    c.LastName.ToLower().Contains(query) ||
                    c.CardNumber.Contains(query)
                );
            }

            var totalRecords = await creditCardQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var creditCards = await creditCardQuery
                                           .Include(c => c.Transactions)
                                           .Skip((pageNumber - 1) * pageSize)
                                           .Take(pageSize)
                                           .ToListAsync();

            var response = new PaginatedResponse<CreditCard>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                Data = creditCards
            };

            return Ok(response);
        }

        // GET: api/creditcards/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CreditCard>> GetCreditCard(int id)
        {
            var creditCard = await _context.CreditCards.Include(c => c.Transactions)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (creditCard == null)
            {
                return NotFound();
            }

            return creditCard;
        }

        // POST: api/creditcards
        [HttpPost]
        public async Task<ActionResult<CreditCard>> PostCreditCard(CreditCard creditCard)
        {
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

            
            bool cardNumberExists = await _context.CreditCards
                                                  .AnyAsync(c => c.CardNumber == creditCard.CardNumber);

            if (cardNumberExists)
            {
                return Conflict(new ProblemDetails
                {
                    Status = 409,
                    Title = "Duplicate Card Number",
                    Detail = "Ya existe esta tarjeta de credito."
                });
            }

          
            creditCard.AvailableBalance = creditCard.CreditLimit - creditCard.CurrentBalance;
            creditCard.BonusInterest = creditCard.CurrentBalance * (creditCard.InterestRate / 100);
            creditCard.MinimumPaymentDue = creditCard.CurrentBalance * (creditCard.MinimumPaymentPercentage / 100);
            creditCard.TotalAmountWithInterest = creditCard.CurrentBalance + (creditCard.CurrentBalance * (creditCard.InterestRate / 100));

            
            _context.CreditCards.Add(creditCard);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCreditCard", new { id = creditCard.Id }, creditCard);
        }



        // PUT: api/creditcards/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCreditCard(int id, CreditCard creditCard)
        {
            if (id != creditCard.Id)
            {
                return BadRequest();
            }

            _context.Entry(creditCard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CreditCardExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/creditcards/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCreditCard(int id)
        {
            var creditCard = await _context.CreditCards.FindAsync(id);
            if (creditCard == null)
            {
                return NotFound();
            }

            _context.CreditCards.Remove(creditCard);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CreditCardExists(int id)
        {
            return _context.CreditCards.Any(e => e.Id == id);
        }

        public class PaginatedResponse<T>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public int TotalRecords { get; set; }
            public int TotalPages { get; set; }
            public IEnumerable<T> Data { get; set; }
        }
    }
}
