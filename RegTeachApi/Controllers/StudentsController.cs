using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegTeachApi.Data.Models;
using RegTeachApi.Data;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly APPDBC _context;
    public StudentsController(APPDBC context)
    {
        _context = context;
    }

    // GET: api/Student
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudent()
    {
        return await _context._Students.ToListAsync();
    }

    // GET: api/Student/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Student>> GetStudent(int id)
    {
        var student = await _context._Students.FindAsync(id);

        if (student == null)
        {
            return NotFound();
        }

        return student;
    }

    // PUT: api/Student/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutStudent(int? id, Student student)
    {
        if (id != student.Id)
        {
            return BadRequest();
        }

        _context.Entry(student).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StudentExists(id))
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

    // POST: api/Student
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Student>> PostStudent(Student student)
    {
        _context._Students.Add(student);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetStudent", new { id = student.Id }, student);
    }

    // DELETE: api/Student/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int? id)
    {
        var student = await _context._Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        _context._Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool StudentExists(int? id)
    {
        return _context._Students.Any(e => e.Id == id);
    }
}
