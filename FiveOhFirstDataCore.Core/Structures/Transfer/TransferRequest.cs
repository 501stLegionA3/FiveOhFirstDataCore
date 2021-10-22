using FiveOhFirstDataCore.Data.Account;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Structures.Transfer;

public class TransferRequest
{
    public Guid Key { get; set; }
    public Trooper TransferFor { get; set; }
    public int TransferForId { get; set; }
    public Trooper FiledBy { get; set; }
    public int FiledById {  get; set; }
    public List<Trooper> Signees { get; set; } = new();
    public List<Slot> FiledTo { get; set; } = new();
    public List<Slot> ApprovedAt { get; set; } = new();
    
    public Slot DeniedAt { get; set; }
    public int DeniedById { get; set; }
    public Trooper DeniedBy { get; set; }

    public Slot TransferFrom { get; set; }
    public Slot TransferTo { get; set; }

    public Role TransferFromRole { get; set; }
    public Role TransferToRole { get; set; }

    /// <summary>
    /// Has this paperwork been completed?
    /// </summary>
    public bool Completed { get; set; }
    /// <summary>
    /// Was the completed paperwork approved?
    /// </summary>
    public bool Approved { get; set; }
    public string? Reason { get; set; }
}
