//Gabriel Souza Varela

namespace AcademiaDoZe.Application.DTOs
{
    public class ArquivoDTO
    {
        public byte[]? Conteudo { get; set; }
        public string? Tipo { get; set; }   // “.jpg”, “.png”, “.pdf”
    }
}
