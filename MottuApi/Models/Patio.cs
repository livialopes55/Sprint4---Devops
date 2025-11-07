using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MottuApi.Models;

public class Patio
{
    public long Id { get; set; }

    [Required, StringLength(120)]
    public string Descricao { get; set; } = string.Empty;

    [StringLength(40)]
    public string? Dimensao { get; set; }

    [Required]
    public long FilialId { get; set; }
    public Filial? Filial { get; set; }

    public ICollection<Moto>? Motos { get; set; }
}
