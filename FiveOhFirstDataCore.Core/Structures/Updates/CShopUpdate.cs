using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Structures.Updates
{
    public class CShopUpdate : UpdateBase
    {
        public CShop Added { get; set; }
        public CShop Removed { get; set; }

        public CShop OldCShops { get; set; }

        public Trooper ChangedBy { get; set; }
        public int ChangedById { get; set; }
    }
}
