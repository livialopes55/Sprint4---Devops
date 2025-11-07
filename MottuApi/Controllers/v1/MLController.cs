using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MottuApi.ML;

namespace MottuApi.Controllers.v1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/ml")]
public class MLController : ControllerBase
{
    private readonly SentimentModel _model;
    public MLController(SentimentModel model) { _model = model; }

    /// <summary>Classifica sentimento (true=positivo, false=negativo)</summary>
    [Authorize]
    [HttpPost("sentiment")]
    public IActionResult Predict([FromBody] TextInput input)
        => Ok(new { positive = _model.Predict(input.Text) });

    public record TextInput(string Text);
}
