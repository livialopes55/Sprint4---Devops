using System.ComponentModel.DataAnnotations;

namespace MottuApi.Models;

public class Moto
{
    public long Id { get; set; }

    [Required, StringLength(10)]
    public string Placa { get; set; } = string.Empty;

    [Required, StringLength(80)]
    public string Modelo { get; set; } = string.Empty;

    public int Ano { get; set; }

    [StringLength(40)]
    public string? Status { get; set; }

    [Required]
    public long PatioId { get; set; }
    public Patio? Patio { get; set; }
}
