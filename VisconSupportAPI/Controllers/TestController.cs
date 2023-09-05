using Microsoft.AspNetCore.Mvc;
using VisconSupportAPI.Data;
using VisconSupportAPI.Models;

namespace VisconSupportAPI.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;

    private readonly DatabaseContext _context;

    public TestController(ILogger<TestController> logger, DatabaseContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet]
    public IEnumerable<Test> GetAll()
    {
        return _context.Test.ToArray();
    }

    [HttpGet("{id}")]
    public ActionResult<Test> GetTest(int id)
    {
        Test? test = _context.Test.Find(id);

        return test == null ? NotFound() : test;
    }

    [HttpPost]
    public ActionResult<Test> PostTest(Test data)
    {
        Test test = new Test
        {
            Name = data.Name
        };
        
        _context.Test.Add(test);

        _context.SaveChanges();

        return CreatedAtAction(nameof(GetTest), new { id = test.Id }, test);
    }
}