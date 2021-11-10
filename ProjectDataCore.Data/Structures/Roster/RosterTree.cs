﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDataCore.Data.Structures.Roster;

/// <summary>
/// A section on the roster, such as a platoon or squad.
/// </summary>
public class RosterTree : RosterObject
{
    public List<RosterTree> ChildRosters { get; set; } = new();
    public List<RosterSlot> RosterPositions { get; set; } = new();
}
