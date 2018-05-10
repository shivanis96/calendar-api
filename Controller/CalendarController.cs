using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controller
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CalendarController : ControllerBase
    {
        private readonly TodoContext _context;

        private static readonly Expression<Func<Todo, Todo>> AsTodo =
            x => new Todo
            {
                Id = x.Id,
                UserId = x.UserId,
                Description = x.Description,
                IsDone = x.IsDone,
                Pubdate = x.Pubdate
            };

        public CalendarController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/calendar - Get all calendar entries
        [HttpGet]
        public IEnumerable<Todo> GetTodos()
        {
            return _context.Todos;
        }


        [Route("/{pubdate:datetime}")]
        [HttpGet]
        public IQueryable<Todo> GetDayEntries(DateTime pubdate)
        {
            var x = _context.Todos.Where(b =>  b.Pubdate.Date == pubdate.Date)
                .Select(AsTodo);
            return x;
        }

        [Route("month/{pubdate:datetime}")]
        [HttpGet]
        public IQueryable<Todo> GetMonthEntries(DateTime pubdate)
        {
            var firstDayOfMonth = new DateTime(pubdate.Year, pubdate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var x = _context.Todos.Where(b =>  (b.Pubdate.Date >= firstDayOfMonth) && (b.Pubdate.Date <=lastDayOfMonth))
                .Select(AsTodo);
            return x;
        }



        // GET: api/Todos/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var todo = await _context.Todos.SingleOrDefaultAsync(m => m.Id == id);

            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo);
        }
        


        // PUT: api/Todos/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodo([FromRoute] int id, [FromBody] Todo todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != todo.Id)
            {
                return BadRequest();
            }

            _context.Entry(todo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
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

        // POST: api/Todos
        [HttpPost]
        public async Task<IActionResult> PostTodo([FromBody] Todo todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodo", new { id = todo.Id }, todo);
        }

        // DELETE: api/Todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var todo = await _context.Todos.SingleOrDefaultAsync(m => m.Id == id);
            if (todo == null)
            {
                return NotFound();
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            return Ok(todo);
        }

        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.Id == id);
        }
    }
}