using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MottuApi.Models;

public class Filial
{
    public long Id { get; set; }

    [Required, StringLength(120)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Endereco { get; set; }

    public ICollection<Patio>? Patios { get; set; }
}
