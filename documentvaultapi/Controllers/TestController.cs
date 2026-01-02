using documentvaultapi.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly DocumentVaultDbContext _context;

    public TestController(DocumentVaultDbContext context)
    {
        _context = context;
    }

    [HttpGet("Documents")]
    public async Task<IActionResult> GetDocuments()
    {
        var docs = await _context.documents.Take(5).ToListAsync();
        return Ok(docs);
    }
}
