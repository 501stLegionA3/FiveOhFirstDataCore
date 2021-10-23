using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Structures.Transfer;

public class TransferSettings
{
    public Guid Key { get; set; }
    public List<Slot> SettingsFor { get; set; } = new();


}
