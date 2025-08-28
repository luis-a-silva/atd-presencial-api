using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackInterface _feedbackService;

    public FeedbackController(IFeedbackInterface feedbackService)
    {
        _feedbackService = feedbackService;
    }

    // POST: api/feedback
    [HttpPost]
    public async Task<IActionResult> ReceberFeedback([FromBody] Feedback feedback)
    {
        if (feedback == null)
            return BadRequest(new { Status = false, Mensagem = "Feedback inválido!" });

        var result = await _feedbackService.ReceberFeedback(feedback);

        if (!result.Status)
            return BadRequest(result);

        return Ok(result);
    }
}
