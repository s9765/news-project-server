using BL.Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class NewsController : ControllerBase
{

    private readonly INewsService _INewsService;


    public NewsController(INewsService iNewsService)
    {
        _INewsService = iNewsService;
    }

    /// <summary>
    /// Get all news 
    /// </summary>
    /// <returns>If succeed return all news </returns>
    [HttpGet]
    public async Task<IActionResult> GetAllNews()
    {
        try
        {
            return Ok(await _INewsService.GetAllNews());
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get one new by news id
    /// </summary>
    /// <param name="id"></param>
    /// <returns> If find return the newItem</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetNewsId([FromRoute]string id)
    {
        try
        {       
            var decodedId = Uri.UnescapeDataString(id);
            var result = await _INewsService.GetNewById(decodedId);
            if (result == null)
                return NotFound();

            return Ok(result);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

