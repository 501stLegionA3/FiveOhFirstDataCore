using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data.Roster;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures
{
    public class GetTrooperDataResult
    {
        public bool Slotted { get; init; }
        public IAssignable<Trooper> Data { get; init; }

        public GetTrooperDataResult(bool slotted, IAssignable<Trooper> data)
        {
            this.Slotted = slotted;
            this.Data = data;
        }
    }
}
