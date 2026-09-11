using MediatR;

namespace Atlantic.Excel.Commands.UploadExcel
{
    public class UploadExcelCommand : IRequest<string>
    {
        public IFormFile File { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

    }
}
