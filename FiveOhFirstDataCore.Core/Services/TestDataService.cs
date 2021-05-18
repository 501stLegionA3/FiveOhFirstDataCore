using FiveOhFirstDataCore.Core.Account;
using FiveOhFirstDataCore.Core.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Core.Services
{
    public class TestDataService
    {
        private readonly UserManager<Trooper> _manager;

        public TestDataService(UserManager<Trooper> manager)
        {
            _manager = manager;
        }

        public async Task InitalizeAsync()
        {
            var data = TestData;
            foreach (var set in data)
            {
                var trooper = await EnsureUser(set.Item1, "foo");
                await _manager.AddClaimsAsync(trooper, set.Item2);
                await _manager.AddToRolesAsync(trooper, set.Item3);
            }
        }

        private async Task<Trooper> EnsureUser(Trooper trooper, string password)
        {
            var user = await _manager.FindByIdAsync(trooper.Id.ToString());
            if (user is null)
            {
                user = trooper;
                await _manager.CreateAsync(user, password);
            }

            if (user is null)
            {
                throw new Exception("The password is probaly not strong enough!");
            }

            return user;
        }

        private static List<(Trooper, List<Claim>, List<string>)> TestData
        {
            get
            {
                return new()
                {
                    (new Trooper()
                    {
                        Id = 42127,
                        UserName = "Soyvolon",
                        NickName = "Soyvolon",
                        Rank = TrooperRank.Trooper,
                        RTORank = RTORank.Cadet,
                        MedicRank = null,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.RTO,
                        Slot = Slot.AvalancheThreeThree,
                        Team = null,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Slotted", "SomeDate"),
                        new("Instructor", "RTO"),
                        new("Display", "42127 Soyvolon")
                    },
                    new()
                    {
                        "Trooper",
                        "RTO",
                        "Admin"
                    }),
                    (new Trooper()
                    {
                        Id = 11345,
                        UserName = "Del",
                        NickName = "Del",
                        Rank = TrooperRank.Sergeant,
                        RTORank = null,
                        MedicRank = null,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Lead,
                        Slot = Slot.AvalancheThreeThree,
                        Team = null,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Slotted", "SomeDate"),
                        new("Instructor", "Grenadier"),
                        new("Display", "11345 Del")
                    },
                    new()
                    {
                        "Trooper",
                        "NCO"
                    }),
                    (new Trooper()
                    {
                        Id = 70303,
                        UserName = "Crebar",
                        NickName = "Crebar",
                        Rank = TrooperRank.SeniorCorporal,
                        RTORank = null,
                        MedicRank = null,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Lead,
                        Slot = Slot.AvalancheThreeThree,
                        Team = Team.Alpha,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Slotted", "SomeDate"),
                        new("Display", "70303 Crebar")
                    },
                    new()
                    {
                        "Trooper",
                        "NCO"
                    }),
                    (new Trooper()
                    {
                        Id = 56273,
                        UserName = "Knight",
                        NickName = "Knight",
                        Rank = TrooperRank.VeteranTrooper,
                        RTORank = null,
                        MedicRank = null,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Trooper,
                        Slot = Slot.AvalancheThreeThree,
                        Team = Team.Alpha,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Slotted", "SomeDate"),
                        new("Display", "56273 Knight")
                    },
                    new()
                    {
                        "Trooper"
                    }),
                    (new Trooper()
                    {
                        Id = 30253,
                        UserName = "Chimera",
                        NickName = "Chimera",
                        Rank = TrooperRank.SeniorTrooper,
                        RTORank = null,
                        MedicRank = null,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Trooper,
                        Slot = Slot.AvalancheThreeThree,
                        Team = Team.Alpha,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Slotted", "SomeDate"),
                        new("Display", "30253 Chimera")
                    },
                    new()
                    {
                        "Trooper"
                    }),
                    (new Trooper()
                    {Id = 23996,
                        UserName = "Deytow",
                        NickName = "Deytow",
                        Rank = TrooperRank.SeniorTrooper,
                        RTORank = null,
                        MedicRank = MedicRank.Medic,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Medic,
                        Slot = Slot.AvalancheThreeThree,
                        Team = Team.Alpha,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Slotted", "SomeDate"),
                        new("Instructor", "Medic"),
                        new("Display", "23996 Deytow")
                    },
                    new()
                    {
                        "Trooper",
                        "Medic"
                    }),
                    (new Trooper()
                    {
                        Id = 54077,
                        UserName = "Klinger",
                        NickName = "Klinger",
                        Rank = TrooperRank.Corporal,
                        RTORank = null,
                        MedicRank = null,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Lead,
                        Slot = Slot.AvalancheThreeThree,
                        Team = Team.Bravo,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Slotted", "SomeDate"),
                        new("Display", "54077 Klinger")
                    },
                    new()
                    {
                        "Trooper",
                        "NCO"
                    }),
                    (new Trooper()
                    {
                        Id = 14577,
                        UserName = "Clover",
                        NickName = "Clover",
                        Rank = TrooperRank.SeniorTrooper,
                        RTORank = null,
                        MedicRank = null,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Trooper,
                        Slot = Slot.AvalancheThreeThree,
                        Team = Team.Bravo,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Slotted", "SomeDate"),
                        new("Display", "14577 Clover")
                    },
                    new()
                    {
                        "Trooper"
                    }),
                    (new Trooper()
                    {
                        Id = 42361,
                        UserName = "Negeta",
                        NickName = "Negeta",
                        Rank = TrooperRank.Trooper,
                        RTORank = null,
                        MedicRank = null,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Trooper,
                        Slot = Slot.AvalancheThreeThree,
                        Team = Team.Bravo,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Slotted", "SomeDate"),
                        new("Display", "42361 Negeta")
                    },
                    new()
                    {
                        "Trooper"
                    }),
                    (new Trooper()
                    {
                        Id = 87071,
                        UserName = "Bones",
                        NickName = "Bones",
                        Rank = TrooperRank.VeteranTrooper,
                        RTORank = null,
                        MedicRank = MedicRank.Cadet,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Medic,
                        Slot = Slot.AvalancheThreeThree,
                        Team = Team.Bravo,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Slotted", "SomeDate"),
                        new("Display", "87071 Bones")
                    },
                    new()
                    {
                        "Trooper",
                        "Medic"
                    }),
                    (new Trooper()
                    {
                        Id = 17077,
                        UserName = "Tal",
                        NickName = "Tal",
                        Rank = TrooperRank.VeteranTrooper,
                        RTORank = null,
                        MedicRank = null,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.ARC,
                        Slot = Slot.AvalancheThreeThree,
                        Team = null,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Slotted", "SomeDate"),
                        new("Display", "17077 Tal")
                    },
                    new()
                    {
                        "Trooper",
                        "ARC"
                    }),
                    (new Trooper()
                    {
                        Id = 123456,
                        UserName = "Reserve",
                        NickName = "Reserveist",
                        Rank = TrooperRank.SeniorTrooper,
                        RTORank = null,
                        MedicRank = null,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Trooper,
                        Slot = Slot.ZetaOneOne,
                        Team = Team.Alpha,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Display", "123456 Reservist")
                    },
                    new()
                    {
                        "Trooper"
                    }),
                    (new Trooper()
                    {
                        Id = 23456,
                        UserName = "Clerk",
                        NickName = "Roster Clerk",
                        Rank = TrooperRank.Trooper,
                        RTORank = null,
                        MedicRank = null,
                        WardenRank = null,
                        WarrantRank = null,
                        Flight = null,
                        PilotRank = null,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Trooper,
                        Slot = Slot.InactiveReserve,
                        Team = null,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "a",
                        SteamLink = "b"
                    },
                    new()
                    {
                        new("Clerk", "Roster"),
                        new("Display", "23456 Roster Clerk")
                    },
                    new()
                    {
                        "Trooper"
                    }),
                    (new Trooper()
                    {
                        Id = 554433,
                        UserName = "LinkTest",
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Trooper,
                        Slot = Slot.InactiveReserve,
                        Team = null,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                    },
                    new()
                    {
                        new("Display", "554433 LinkTest")
                    },
                    new()
                    {
                        "Trooper"
                    })
                };
            }
        }
    }
}
