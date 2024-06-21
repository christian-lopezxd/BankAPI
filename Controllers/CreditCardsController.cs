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
        public async Task<ActionResult<IEnumerable<CreditCard>>> GetCreditCards()
        {
            return await _context.CreditCards.ToListAsync();
        }

        // GET: api/creditcards/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CreditCard>> GetCreditCard(int id)
        {
            var creditCard = await _context.CreditCards.FindAsync(id);

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
    }
}
