using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;

using Microsoft.AspNetCore.Identity;

using System.Security.Claims;

namespace FiveOhFirstDataCore.Data.Services
{
    public class InitalAccountPopulationService
    {
        private readonly UserManager<Trooper> _manager;

        public InitalAccountPopulationService(UserManager<Trooper> manager)
        {
            _manager = manager;
        }

        public async Task InitializeAsync()
        {
#if DEBUG
            var data = TestData;
            foreach (var set in data)
            {
                var trooper = await EnsureUser(set.Item1, "foo");
                await _manager.AddClaimsAsync(trooper, set.Item2);
                await _manager.AddToRolesAsync(trooper, set.Item3);
            }
#else
            var access = Guid.NewGuid().ToString();
            var t = new Trooper()
            {
                Id = -1,
                UserName = "Admin",
                NickName = "Admin",
                AccessCode = access,
                Rank = TrooperRank.Trooper,
                Slot = Slot.Archived,
                DiscordId = "abcdefghijklmnopqrstuvwxyz",
                SteamLink = "abcdefghijklmnopqrstuvwxyz"
            };

            var trooper = await EnsureUser(t, access);
            await _manager.AddToRolesAsync(trooper, new string[] { "Admin" });
#endif
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
                throw new Exception("The password is probably not strong enough!");
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
                        BirthNumber = 42127,
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
                        Slot = Slot.AvalancheThreeThree | Slot.AvalancheThreeTwo,
                        Team = null,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        DiscordId = "133735496479145984",
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
                        BirthNumber = 11345,
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
                        BirthNumber = 70303,
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
                        BirthNumber = 56273,
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
                        BirthNumber = 30253,
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
                    {
                        Id = 23996,
                        BirthNumber = 23996,
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
                        BirthNumber = 54077,
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
                        BirthNumber = 14577,
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
                        BirthNumber = 42361,
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
                        BirthNumber = 87071,
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
                        BirthNumber = 17077,
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
                        BirthNumber = 123456,
                        UserName = "Reserve",
                        NickName = "Reservist",
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
                        BirthNumber = 23456,
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
                        BirthNumber = 554433,
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
                    }),
                    (new Trooper()
                    {
                        Id = 34554,
                        BirthNumber = 34554,
                        UserName = "Hobnob",
                        NickName = "Hobnob",
                        SecurityStamp = Guid.NewGuid().ToString(),
                        Role = Role.Pilot,
                        Slot = Slot.RazorTwoOne,
                        Flight = Flight.Bravo,
                        Team = null,
                        StartOfService = DateTime.Now,
                        LastPromotion = DateTime.Now,
                        PFP = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAHAAAABwCAYAAADG4PRLAAAas0lEQVR4nOx9CXRURdb/r5JOIIR0ErJAVgIJa4AoO/IBQkBgQFxA/FjcGEDHv44zf0dUXEbHdRRHZ3TgoDJsDgoiCkJURBhFVoEIiCABYnayhySELCT1nVt59Xzd6e708jpJO/md0yd5r9+rrle/urfuvXWrnoFzjnZ4LrxauwLtcA3tBHo42gn0cLQT6OFoJ9DD0U6gh6OdQA9HO4EejnYCPRztBHo42gn0cLQT6OFoJ9DD0U6gh6OdQA9HO4EejnYCPRztBHo42gn0cLQT6OFoJ9DDYWjtCtgDxpgPgHAAtwPoAyARwCUAHwNYwzm/2tp1bC2wtpxWyBgLBjAPwN0Ahli5bC2Ae3hbfhB3gp67LX2IN+WzBEAOVVH7GTigPx9//Rhudn6W7IxKGaRZ/hfAPwCcAHAYwOzWfja3tFdrV8ACgTcCSNcS5OPjw2dMn8q/2ZPCy4oyxGfLh+u1BJYA6AFgJID3ANSZE6981mmJ/jV8Wr0CGuJCAaw2b/Tp06bwA9/uVInTfp5c+ifttRcBNGjv9fX15WGhIbxjxw7a6x5t7WfV89MmxkDG2ChlLOslz02aOB5L/vQQhg291ua9EybNwLHU4ybn5s6ZhTm3z0SvXgnw69gRpWVlmDBxBkpKS+nragA9Oed5bnugFkSrEsgY6wjg7wAWy3N+fn5Y9fab+M3USc3ev2//Qdwy6w7U1tbC29sbN980DX9+6lHExkQ3uXbjpi249/4/ysOPOOezdH2YVoLbCWSMkfVIRB3gnDdozhsBbAIwWZ4bkNgPa1YtR0JCz2bL3botBQsWPYCGhgb07pWA1159Hv8zeqTV64nkcROm4fSZs1BU6STO+Vd6PGNrwm0EMsYiAfwTwAzFqjwNYAHn/BBjLAnABwD6yutn3jIDK5b/Db4+Ps2WffbsOdxx92KUll3C1MmT8MLzT6Gzv3+z961ZuwF/ePhxeXgAwFHF+PFWLN4jAD7nnP/s0sO3INziyDPGyJJ8V3G+JfoD2M0Y+wuAZwF0oJMhIV3wxt9ewo3TpthVdllZGXak7MC4MSMwduxYTJ821e56BQYZ4e3lhfoGoQhGKR8tfqvU/wsASznnx+wuvJWguwQyxl4G8CelV4uxiRqttq6uybWkKj/Z/G9ERUXQfXaVn52VgaysDHQJCUPv3n2bvY+er6rqCp58+nmsXvtvRx6lHsBLnPOnHLnJFTDGvADQ0FLBOa+35x5dY6GMsaVkpkvyevbojmOHv8a+b77AUDNrsldCT2z/5ANER0faTR6ha7cI9O/f3y7yCGR5zp5zt6PkQXmGJxljy7UnmSOVtROMsWTG2HYlPFhEBDLGvmSM/ZEx1tvmvXpJoKI2t8njxP79sHnTWkR066pe88Y/VuCZv7yM+PgeSNm2CV27huvy29ZQXFyCKdNnIS3tvKtFPcI5X6ZPrX4BY4x837cAzFbsBEtoUIITizjnteZf6iKBjLHBAD6Ux6NGDsPnOzabkFd39Sp2frkbRmMANm1YjfDwMD1+2iZ+98DDepBHeIIxpmtvY4zdqgQfbteSRwJuJuTE0Z0A9jDGOpiX47IRozzYNmmU9O/XBxs3rEZAQGeT63bt2oOqqsvCxyMJdDc2f7RVdBidEATgXgDP6VEYY4xshFe157p27YqkpCSEhoaiuroahYWFV3/88UdDcXGxvOQ6RRJvMynLFRWqDLrUSuPoODg4CF9/tQOxsU0d6VdeXYbevRMw48YZ8PJy7zTklSvVGPk/E5GRkaVnsT9wzge6Wghj7PcAXpfaLygoSIzpcXFxTa4lH/f06dM4ftwk0jSdc75DHrgqgW9J8sTB31+1SF5hUSGGD03C4CHD3E4eYddX/9GbPMIA6rDaYISjIKMEwDJJXkREBK6//nqrxhi1FZFbVFSEnJwceZqkVyXQ6dZkjM0HcJ88XnD3fEz7zQ0WrzUGGDFo0DUwGoOc/TmH8OmOz91VdLyzNzLGFmnJI1VpizzNfRg0aJD21BjGWIw8cIpAxhgNYsvl4NulSzCefnKJ1es7dOgg/LaWQG1tLU6dOu2u4iOcuYkxNkVpL9HegYGBGD9+vN3uU3BwsBgjFZB7M10e2FShSjhsJAA/5Voq5TKAmQAClMv48jeXsaCgQGeeTXfU1NSiqLjEXcU7bIkyxkh8Nsu2ps48btw4+NgRMtSCCMzPz5eHQ+U/FglkjPUB8BqAidK6tIZZM29iUyZPdKgy7oTB4A0/v47uKt7XkYsZYyEAtgLwb6ybARMmTEDnzp0d/uHwcJO+M0D+00SFMsZmADgGYFpz5Pn6+uCppY84XBl3gnp4VKRTms4eFDp4/SoAwrwkdTl27FihDp2BGelG+Y8JgYyxZAAfAehkT6GjrxuJ7t1jnKqQu0CW28AB/d1V/Dl7L1QszpvkMRki3bp1c/qHzVSuOnh6aX4wUqurm0NYWCjefOMVpyvkTsyaebPDY4wdoLHfrmkmxhipuJfkcUxMDBITE136cTP3y1s9rzn5ohJxsAsP3r9IBKLbIoYOuQYjRwzVu9iN9qQuKsGNdXL48fPzw/Dhw/Wui6kEMsaildQ8uxAZ0Q0Lf3un3pXSFY8/+ke9i1xh53XPABBTLzTujR49WozLrqKhwSR+UCb/kRJ4i7SU7MFTTy5Bp052DZOthutGjcDCBbp2spuam0pijPVXptMEevXqZW49Oo36epPpwVPyH0ngBHsL6tevD2beOkOXSrkbL73wNG6YZPejNYelAJqLhb4tXQ2yGq+91nZGnSOorTWZSUqT/0gC7XZMfv/AvXblrbQFkCGzfs1KLLhnvsP3ktFw/333ILG/mrZDbbWBMTaOMTZZ8fFUMMYWkGGu/I+RI0eKbAS9cPnyZe3hd+rvKtGVC/a4DiEhXXD+p1TdKtWS2H/gMN5a/rb4W15ejoaGpvYINbjRGIDJk8Zj3pyZCA8PRVraBdxxz/2wYL5UKsH8JxS/7CwZ5/RFfHw8RowYoWv9f/rpJxw9elQeRsq8VoMSJrNrQLv7zrm6Vqolcd2o4eJTWlqGkz/8iLy8i7iYn4f6q/Xw8fWBj8EHISHBGDSw0Yckg7OwsBhr1m+Er6+vCNGZgbTWY0qicIAkj6zOwYMH617/yspK+e9FrRFjsDGV3wS3zbpZ94q1NIKDgzB2zHXi/+LiIhQVWQ6u7P32IP667E3UVNdg3pxZiIrshl27v8GBg0fML10MoIs8GDp0qDt8UKE1FPwAoEYeGJQk12YxILEf4nu6fya9JRESEirM89LSEmhdvK+/2Y8PP9qKoYOT8OADixAa0shP8oSxmD13EQoKTEiPkEIQFRUlnHa9QXUsKVED9Ke1c5I0MNcqiTM2kTx+HHx8PGI9qEMICwtHVFS0UJMS1D4LF8zHn596RCUPQj12xMxbppkXIcgzGAxC+twBMmBqalShW6P9jhgpBnBGSby1iunTJrulcm0B/v6d0aNHZ1y+XCkaa8rkibh69aq57yWMnCHXJmlPNUhLPjExEf52ZIc7g8JCVeLTlfWOKgyc8xrG2DZbBJLqTEoa4JbKtSUQkfQxh1SvdXV1qKy8DC8vZmLFBgQEiNQHd0EzD/iN+XJy6Qf+E0CVtQJGjhhqomL+2yBT/UhCSSp9fHw1XzFhdboh31eANEFeXp4krUmuiCCQc56tLEe2CGm1/bfjypUq7N6zVxsVYaGhoejSpYvbfrO0tBTV1dUGxVZpsppKnY3gnD8OYJ/5Bd5eXkieMM5tFfQk0Ph4+EgqDJoIS6dOnYRqdQSOpHKeP39eSt9OJe3eBOYz8rMBZGtPhIWHITQ0xKEK/hpRWVmB7Oxc5ObkwdvgjcBAYy0UB9vc2GkO9qrb6upqZGVlSdP/fUvTWSYEcs5zlTxPNV5m77KvXzuIqG/3HUAD5/BiXrhz/mxfeZ7GKXcgLS1NSneJdt2JFk1yYjjnFwCslMeRkc6nAfxaQB2fGjP950whFdePuw6d/PzU781mCnRBVVWViH8qWM85r7R0nbW8UDUWdE2Sy9nkLoEaLy8vDxUVFa1Wh0uXylBRWSlio7W1dVi88E6xcQKhY8eOQgL1JvH777/Xlvm6teushVbUKeSoKLdleNmF8vJy4XtduXJF+FutgZKSYrFxwoQJYzGgf19ERHRFXl6jbyazxUiV6mWNUlkZGRnycA3nPMPatdYIVB2dsrJLulTKWZSWNvZ0S9M/LYHy8ktiHPLx8cFvpvyS/5r6/UnxNzCwMaGZGt1oNIqQmqtITU2VliqpzSdtXWtNhaq1KLVAYE1NDdLTf0ZGRqbLlbWFkpIS1UT304w5LYXGKaUCi99l5zRuMyPDZ3RtUVGRQy6CJeTm5pLlKQ+XcM5zbF1vjUD1fPWV6iZfkj8k9b67drmg3yjWpMgHBzcmzGl2dnI7iouLLFqYR481LveiTqWVOOrYrpBIw8S+faor/r2yUYRNWCNQdVQsbU5gXmm9QQZLXt5F9Tgw0KgmUZEP1RIEXr1aJxakWloOd/5CY3oojcnm35P1mJ+f77BzT+24f/9+7X0fcs6bLaRZhd1gwUmlgbugoFA0JI1R3ZSl1HrEA4uKikX4SILIM8/saok1hgaDD2Jj44QE5uRko7r6ivrdocONu49YS5MnMshyJvVKbeXr69ukbajtqGwinD5ktGiC1sUA3rCrnlbOq/Mi3oamiTnUgNT7Gi3ESpSX+4kBXGy+5gKJ9fX1Qo1QGWSed+kS3Orpi6RtIiIikJ5+QRxThyUDhuooDRhLoLagtqEPtRcZQTLJiZ6TJE3mehKB6enp2tsf4JxbnVwwqZ+V8+oyW2pISwgJ6SIqR5UoLCxCfX0DgoICXSKQHjDGwj5nrQ0fH19Vde/99iAuX64S45+9qRPURraGmrS0NO2wsIlz/oG9dbOmi9QUe2tbWFHPjIqKEo1OP15cXIzs7JwWMzBaEkQAPRf93bV7rzhnS/ocwblz57QJS6cBLHLkfmsECtZ8fX1s7ijRsWMHxMV1F9JIDxQWFuq2eTFHkPLZl+Z5Ky6BfEFCdnYuDh1uTO3TI6hQVlamHfcqlF2Fyx0pwxqBIkVu7pzbEBMdZbsALy8RgQgPD2sVX00LMoAWLn4Qc+9YqN3UziWQoUHuBGHzlk/FX9I6zq7zk6Cx/vRpk6XgiznnPzhaTpMxkDFG6lPs92hp3822ilM/nsEts+arkmcMMLpcJqnNnJwsYXTQuPfx1s/E6cDAQOZK6iCp4jNnzmgXrLzsyLinhSUJnCZDaa0tUfaAGvbFl/+G5BtmqOR1CQ7G0sdcX52Und04+0B4b8NmGVxmrizUhJJlTZangpTmwmW2YMkKVQN+WVnZzpbrduTk5GL12g1491/rTOK1pNJXrngdsbGu5WdmZmaIFAoorsOmzVvF/+TbOas+SaLJXdDkeB4GcKu9OxNagiUCx8t/0s6dx8OPPIFz538WISJyFyqEX+ONQGMAoqOjMHzYYOHI9+wRh7i4WLFDoTXXwxYaOMeJEz/g+PEfGgqLilhpaRkru1QOb28vdAsPR2hYCKZPmyL2Pnv3X+vxxc6vmkzhkAH10IP3if22XUFeXq5KHjX6ynfWiRkRKr979+5Ol5uZmSkcfAXkWE7lnLsUyjLZaosxNk/Zj8tp+Pt3Epbp6FEjxK5Nwl8yGEQqHkmHbwdf+HfyR3BQIHIvXsRPP53je7/dz44eO85rampsmrBkPEiT3hydO/vjuWefwD13zXOl+iJ4XVKi7k+Gg4eO4qH//4T4PyQkBH379nWq3KysLPFR6k6kDeOcn3SpsloCGWNTAWxvK+9TIiOBTHUagzTjhQlIIsiFmT93Nu6/77cu74BYVFSoWpyES+UV+N+5i1BSWibqk5SU5NRq29zcXPNIy2TO+U6XKqtAqFBl2/9PLZFnNBoRGxsr9D75ejKuR+Z1XV2daFzS6aWlpSgoKHA5uE29vGfPnkJVUaORBVhcXIwLFy6IeKG03MhHfe2VFzB/3mxdfE9Sm9Lfg6LSn3t+mSCPyo+Pj3eKPJI6Up0aLNSLPCizDlSrTPNdiCIiIoS6IIvL3gYiaaYGJ6khP6eiokIQak3tQVGL1DChoaGis9gCdZLvvvtOjMeEHnHdsfc/KU5tnCPR0FCP7OxsdcyTeO6F17A95Uvxf2RkJHr0cGxhDz0zdTiSPg2WK3FO3cJVxMx6AOoS1k6dOoldFajSbRHUMVJSUlRJv3fxPfjri884VRaVQX6e+dQPGS1k4VIzk+ZxJm2eNIbGYCEUAIi3lpzkLLy0u1MQecnJyW2WPCi+6ZAhv7zIbO26900mfu0FdYSMjPQm5G388BOsWfeBII/ao3dvm1tWW4Vmo1YoS/ju0ps8KASqNj8N0q2VOOQI4uLi1CADqeutn6Y4dD+N2+Skm2uy3Xv24h9vvSvUn8FgQL9+/ZzOcaGxXAOmXQSqJ7Q7Nbm0FVRLQ+uPnThxyu77aFwm8sz2XcF3R1LxzHPLhHEmN1p1xp+VIMPPbEHQg04XZgMmBHpC6ExCW9dz59Ptuof8u9zc7CaSdyz1BB557FkxJpJRRRanq5qIJNfM6R+qbA6vK0z0A/U+PdLiWgLatIqaWtuuC1nGeXk55lt1CBB5Dy/5s9hnm9wWanRnNuehe6lTUQegNiTLOjo6WrgQisFlUN5EquvbYOQaeeHQ04DuKQRqVaCfDVVXWVmJ/Pw8i9llKZ/twgsvvyG+owZPSEhweKKWXBiyVK3l6ZArptm0/G7G2FI9jRmDMpFoJAKpp3iKGtVaj9oXiFy9Wi9eMBIWFoJJyWNRUdF0fpSedcXKNVj33kZhbfr7+6NPnz4OPzup2eaysanckydPyg4XoLwnYpVDP2QDBmXV52y4aZGGu6BZN46E+MbX1R05mirekXTw0BGxYc/woYOaSEZtbR0+2PQxNm3eproKNOY5Sh7dZ08qPWk0kuyzZ8/KUw/qTaC6iSn5Rp4CzZSMmAecNuN28UJIib59EpqQl5GZjRdffgPfH2+c+CYJ6tWrl1PkhYXZH3el39AQmMQYG6hHIBsKgWq4gBolIqLxTWLu2KxGLxQUFJio0FWr16v/E2njxo7C40seUs+R+tryyQ78c8W/xJvMoOzpQgaLo3FUR8mDkgBF0qrpdPcCeMChQqyACNwjD86dOyfEncSexom2urGBjIVqQcSNHTMK8+fMxMCBv4S+iotL8PxLr2P/ge/U62hccmYlEY2VoaGhTtW5Z8+eWgJvU4wZhxKYLMHAOT/FGKNu6Ue9+ujRoyIiQ04sGTWWsopbG9oY48Tksbj2moGYlDxOZHFLkGW55+t9eO315SgtbZxloOfo0aOHU+QZjUaXEpliYmJw7NgxacyQ1TUCwJdOF6hA+gwfyYA2NQ75QVRhGiPoB0mdthX3Qq4CgjJ5/OzTS0zqRvU9lnoSq9duaDhy9LhJoCIyMtKpaFNISIhLMx5QAg/UATQx0tl6EvgugHnSHzx//rzYZb2srExIIKkO8pPIESa/KjY21tXfdRrUi+WmAvcuukslj9yH7JxcvLPqPez66muYz23GxcU5HKQndUudWY8tk6GE/zQEzmKM/T9L7wR0BOLpOedfM8aoN4iXH1VUVIiXLdFAT64FfWSk4dSpU6KXW3rblrtBlpy05uLj43DbzMadg9N/zsS/N2zGF1/+x8QVIgLoI6XPEVDHJWNFT80j51aVUF4QgGuUxCanoa3d75S3Oou0+oyMDGFxSb1P4yN9qEceOnRISCMZA97e3rqPkTIqRGSQtNHfrKwsQR59ZzB4I2lQIrZ8sh2HDx/Dt/sPm2z1QfWh73v26C6sT0cnY10xVmwhKChI2BYad22aqwSaJzXdCWC1VD+kOkiVmlujJJ2ZmZniQcm6Ikml/7U9vjnQ7xIxclM5uQBErtrR1is7O1sQKMNn1tYI+hgMGDw4CfPm3Cr2drvx5vnCbRgyZIhdkkTlEnHuXBF18OBBMdmr4AjnfJgr5TV5AaTymnD1zc3UY+Lj40Xv0YLULLkdVVVVonFI3dCHBnxJpHSkZaoFESD/NpdVQN/Tb+Tm5ppPjpqAfnvQwP4YN2YUpk5JVi3Rt99dh1WrN9idDkGdlMhzt/9LbXb4sCp0pDaCOedOb8Fh8Q2ejLG/AlDfJ0dqkh7OfI6L7s3PzxcSKTOYqQGIbLJiyXIjyWxOIhtT1xsNJJJAMp7o2FLdksePEa8vpzL79IlH/769Yf7mtIKCIsy763dCVSUlXdOsEWJPTFMvlJeXY8eOHdpnW8g5dzq0ZlGvcM4fZYxRtyciRQMXFBRwckRjYmKYHIzlJHDXrl2FlMj9XAoLC9VYJUkhSYl0RaRUyjX2lvbltATqA1MnT8TSx/7Q7Mazn2z7DOXlFaJutsiTHdOViVtHQR27cQ9udQpsmCuxUastwTl/hTGWDuB9elbOOSPpSE9PF35YQkKCGkOUY4d8ga9MM5TbUElL1h7IjkEPSZJB4xE97MWLF0WvbY48GvM++ni7+D8iwvoeN1SuVPctDWonzStVb9K+CdVR2GwNzvmHjLF8AFvIn1XOCTWQmpoqVCWNe1QhqSapN9O4I812GiNJlRGB2rFPEiUllD6SOJIabcNSJ7h06RI+37kbo0cPF1EXa9i0eatYK0F1s2SM0G+QunTVMXcFZMlrCAxnjPXjnDv12lG73mLNGOuqbPc009JLEEk9EmHkcrhz22FyIwICOuP99SvFfJ8l3DD1NpFRnZiY2MTwos5FUtfaUSXSJrt3m7wi/Q7OuVNLGuzSH5zzfM75XOXVn++QUGi/J9VKfuPx48fFHl+WUhdcBUk6qdSKikr8/a13LF7z1e69gjySPG2SMEmdfA9ta5MHRQLN6jHS2bIcGgA45yc554sBjFJ2j+Vm3wvyyG9zB8idgSDqG3VOT4v3N24R1SBtIFUwSR2Nhc1lfbck5KpmDYY4XZYzN3HOyQGdqCxF+5iEUPs9GTHumN0n9UySSGPpipUmu+/jbNoFnD6T1uDr6yu2QZaNRFLXFuc2zYLqTq9Zc5hApgBKDJVzfquyJPtZZYMa0cDuksLo6GihEo+fOKVueUXYnrKTjB1GBJO0kRS25SRlMwKDGGNOOaL/FwAA//+INOM5/k048AAAAABJRU5ErkJggg==")
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
