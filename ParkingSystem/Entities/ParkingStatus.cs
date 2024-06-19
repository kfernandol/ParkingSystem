using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ParkingSystem.Entities;

public partial class ParkingStatus
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int SpacesAvailable { get; set; }

    public int EntryDoorStatus { get; set; }

    public int ExitDoorStatus { get; set; }
}
