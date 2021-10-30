using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Structures.Transfer;

public class TransferSettings
{
    public Guid Key { get; set; }
    
    public Slot HundredsGroup { get; set; }

    // What part of the company/detachment?
    public int? DivideBy { get; set; }
    public int? DivideByEquals { get; set; }

    public List<int> EnteringApproval { get; set; } = new();
    public List<int> ExitApproval { get; set; } = new();
    public bool RequireBattalion { get; set; }
}
