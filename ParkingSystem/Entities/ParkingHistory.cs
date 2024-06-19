using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ParkingSystem.Entities;

public partial class ParkingHistory
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public bool IsEnter { get; set; }

    public string Date { get; set; } = null!;

    public int SpaceAvailable { get; set; }
}
