using Atlantic.Excel.Commands.UploadExcel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Atlantic.Excel.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/excel")]
    public class ExcelUploadController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExcelUploadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { mensaje = "No se proporcionó ningún archivo." });

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var command = new UploadExcelCommand() { File = file, Email = userEmail, Role = userRole };
            var result = await _mediator.Send(command);

            return Ok(new { mensaje = "Archivo subido exitosamente", url = result });
        }
    }
}
