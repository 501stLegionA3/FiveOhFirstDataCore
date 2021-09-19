using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Updates;

namespace FiveOhFirstDataCore.Data.Structures
{
    public class TrooperUpdateConflicts
    {
        public enum EditGroups
        {
            Rank,
            Slot,
            CShop,
            Qual,
            Time,
            Notes,
            MP
        }

        public Trooper Edit { get; set; }
        public Trooper Current { get; set; }

        public Trooper MergeResult { get; set; }

        public bool EditRank { get; set; } = true;
        public bool CurrentRank { get; set; } = false;
        public bool EditSlot { get; set; } = true;
        public bool CurrentSlot { get; set; } = false;
        public bool EditCShop { get; set; } = true;
        public bool CurrentCShop { get; set; } = false;
        public bool EditQual { get; set; } = true;
        public bool CurrentQual { get; set; } = false;
        public bool EditTime { get; set; } = true;
        public bool CurrentTime { get; set; } = false;
        public bool EditNotes { get; set; } = true;
        public bool CurrentNotes { get; set; } = false;
        public bool EditMP { get; set; } = true;
        public bool CurrentMP { get; set; } = false;

        public Dictionary<CShop, List<ClaimUpdateData>> CShopClaims { get; set; } = new();
        public List<ClaimUpdateData> RemovedClaims { get; set; } = new();

        private Dictionary<CShop, List<ClaimUpdateData>> EditCShopClaims { get; set; } = new();
        private List<ClaimUpdateData> EditRemovedClaims { get; set; } = new();

        private Dictionary<CShop, List<ClaimUpdateData>> CurrentCShopClaims { get; set; } = new();
        private List<ClaimUpdateData> CurrentRemovedClaims { get; set; } = new();

        public TrooperUpdateConflicts(Trooper edit, Trooper current, Dictionary<CShop, List<ClaimUpdateData>> ecshop,
            Dictionary<CShop, List<ClaimUpdateData>> ccshop, List<ClaimUpdateData> erc)
        {
            Edit = edit;
            Current = current;
            MergeResult = Edit.ShallowCopy();

            EditCShopClaims = ecshop;
            EditRemovedClaims = erc;
            CurrentCShopClaims = ccshop;

            ResolveConflict(EditGroups.CShop);
        }

        public void AddNewClaimGroup(CShop shop)
        {
            if (CShopClaims.TryGetValue(shop, out var list))
            {
                list.Add(new());
            }
            else
            {
                CShopClaims.Add(shop, new() { new() });
            }
        }

        public void RemoveClaimGroup(CShop shop, int item)
        {
            if (CShopClaims.TryGetValue(shop, out var list))
            {
                var claim = list[item];
                RemovedClaims.Add(claim);
                list.Remove(claim);
            }
        }

        public List<EditGroups> GetGroupsWithConflicts()
        {
            var groups = new List<EditGroups>();

            if (Edit.Rank != Current.Rank
                || Edit.RTORank != Current.RTORank
                || Edit.MedicRank != Current.MedicRank
                || Edit.PilotRank != Current.PilotRank
                || Edit.WarrantRank != Current.WarrantRank
                || Edit.WardenRank != Current.WardenRank)
            {
                groups.Add(EditGroups.Rank);
            }

            if (Edit.Slot != Current.Slot
                || Edit.Role != Current.Role
                || Edit.Team != Current.Team
                || Edit.Flight != Current.Flight)
            {
                groups.Add(EditGroups.Slot);
            }

            if (Edit.CShops != Current.CShops)
                groups.Add(EditGroups.CShop);
            if (Edit.Qualifications != Current.Qualifications)
                groups.Add(EditGroups.Qual);

            if (Edit.LastPromotion != Current.LastPromotion
                || Edit.StartOfService != Current.StartOfService
                || Edit.LastBilletChange != Current.LastBilletChange
                || Edit.GraduatedBCTOn != Current.GraduatedBCTOn
                || Edit.GraduatedUTCOn != Current.GraduatedUTCOn)
            {
                groups.Add(EditGroups.Time);
            }

            if (!(Edit.InitialTraining?.Equals(Current.InitialTraining ?? "")
                    ?? (Edit.InitialTraining == Current.InitialTraining))
                || !(Edit.UTC?.Equals(Current.UTC ?? "")
                    ?? (Edit.UTC == Current.UTC))
                || !Edit.Notes.Equals(Current.Notes))
            {
                groups.Add(EditGroups.Notes);
            }

            if (Edit.MilitaryPolice != Current.MilitaryPolice)
                groups.Add(EditGroups.MP);

            return groups;
        }

        public void ResolveConflict(EditGroups group)
        {
            switch (group)
            {
                case EditGroups.Rank:
                    if (EditRank && CurrentRank)
                    {
                        MergeResult.Rank = Edit.Rank > Current.Rank ? Edit.Rank : Current.Rank;
                        MergeResult.RTORank = Edit.RTORank > Current.RTORank ? Edit.RTORank : Current.RTORank;
                        MergeResult.MedicRank = Edit.MedicRank > Current.MedicRank ? Edit.MedicRank : Current.MedicRank;
                        MergeResult.PilotRank = Edit.PilotRank > Current.PilotRank ? Edit.PilotRank : Current.PilotRank;
                        MergeResult.WardenRank = Edit.WardenRank > Current.WardenRank ? Edit.WardenRank : Current.WardenRank;
                        MergeResult.WarrantRank = Edit.WarrantRank > Current.WarrantRank ? Edit.WarrantRank : Current.WarrantRank;
                    }
                    else if (EditRank)
                    {
                        MergeResult.Rank = Edit.Rank;
                        MergeResult.RTORank = Edit.RTORank;
                        MergeResult.MedicRank = Edit.MedicRank;
                        MergeResult.PilotRank = Edit.PilotRank;
                        MergeResult.WardenRank = Edit.WardenRank;
                        MergeResult.WarrantRank = Edit.WarrantRank;
                    }
                    else if (CurrentRank)
                    {
                        MergeResult.Rank = Current.Rank;
                        MergeResult.RTORank = Current.RTORank;
                        MergeResult.MedicRank = Current.MedicRank;
                        MergeResult.PilotRank = Current.PilotRank;
                        MergeResult.WardenRank = Current.WardenRank;
                        MergeResult.WarrantRank = Current.WarrantRank;
                    }
                    else
                    {
                        MergeResult.Rank = default;
                        MergeResult.RTORank = default;
                        MergeResult.MedicRank = default;
                        MergeResult.PilotRank = default;
                        MergeResult.WardenRank = default;
                        MergeResult.WarrantRank = default;
                    }
                    break;
                case EditGroups.Slot:
                    if (EditSlot && CurrentSlot)
                    {
                        MergeResult.Slot = Edit.Slot > Current.Slot ? Edit.Slot : Current.Slot;
                        MergeResult.Role = Edit.Role > Current.Role ? Edit.Role : Current.Role;
                        MergeResult.Team = Edit.Team > Current.Team ? Edit.Team : Current.Team;
                        MergeResult.Flight = Edit.Flight > Current.Flight ? Edit.Flight : Current.Flight;
                    }
                    else if (EditSlot)
                    {
                        MergeResult.Slot = Edit.Slot;
                        MergeResult.Role = Edit.Role;
                        MergeResult.Team = Edit.Team;
                        MergeResult.Flight = Edit.Flight;
                    }
                    else if (CurrentSlot)
                    {
                        MergeResult.Slot = Current.Slot;
                        MergeResult.Role = Current.Role;
                        MergeResult.Team = Current.Team;
                        MergeResult.Flight = Current.Flight;
                    }
                    else
                    {
                        MergeResult.Slot = default;
                        MergeResult.Role = default;
                        MergeResult.Team = default;
                        MergeResult.Flight = default;
                    }
                    break;
                case EditGroups.CShop:
                    CShopClaims.Clear();
                    RemovedClaims.Clear();

                    if (EditCShop && CurrentCShop)
                    {
                        MergeResult.CShops = Edit.CShops | Current.CShops;

                        foreach (var i in EditCShopClaims)
                        {
                            var set = new List<ClaimUpdateData>();
                            foreach (var s in i.Value)
                                set.Add(new(s.Key, s.Value));

                            CShopClaims.TryAdd(i.Key, set);
                        }

                        foreach (var r in EditRemovedClaims)
                            RemovedClaims.Add(new(r.Key, r.Value));

                        foreach (var i in CurrentCShopClaims)
                        {
                            var set = new List<ClaimUpdateData>();
                            foreach (var s in i.Value)
                                set.Add(new(s.Key, s.Value));

                            CShopClaims.TryAdd(i.Key, set);
                        }

                        foreach (var r in CurrentRemovedClaims)
                            RemovedClaims.Add(new(r.Key, r.Value));
                    }
                    else if (EditCShop)
                    {
                        MergeResult.CShops = Edit.CShops;

                        foreach (var i in EditCShopClaims)
                        {
                            var set = new List<ClaimUpdateData>();
                            foreach (var s in i.Value)
                                set.Add(new(s.Key, s.Value));

                            CShopClaims.TryAdd(i.Key, set);
                        }

                        foreach (var r in EditRemovedClaims)
                            RemovedClaims.Add(new(r.Key, r.Value));
                    }
                    else if (CurrentCShop)
                    {
                        MergeResult.CShops = Current.CShops;

                        foreach (var i in CurrentCShopClaims)
                        {
                            var set = new List<ClaimUpdateData>();
                            foreach (var s in i.Value)
                                set.Add(new(s.Key, s.Value));

                            CShopClaims.TryAdd(i.Key, set);
                        }

                        foreach (var r in CurrentRemovedClaims)
                            RemovedClaims.Add(new(r.Key, r.Value));
                    }
                    else
                    {
                        MergeResult.CShops = default;
                    }
                    break;
                case EditGroups.Qual:
                    if (EditQual && CurrentQual)
                    {
                        MergeResult.Qualifications = Edit.Qualifications | Current.Qualifications;
                    }
                    else if (EditQual)
                    {
                        MergeResult.Qualifications = Edit.Qualifications;
                    }
                    else if (CurrentQual)
                    {
                        MergeResult.Qualifications = Current.Qualifications;
                    }
                    else
                    {
                        MergeResult.Qualifications = default;
                    }
                    break;
                case EditGroups.Time:
                    if (EditTime && CurrentTime)
                    {
                        MergeResult.LastPromotion = Edit.LastPromotion > Current.LastPromotion ? Edit.LastPromotion : Current.LastPromotion;
                        MergeResult.StartOfService = Edit.StartOfService > Current.StartOfService ? Edit.StartOfService : Current.StartOfService;
                        MergeResult.LastBilletChange = Edit.LastBilletChange > Current.LastBilletChange ? Edit.LastBilletChange : Current.LastBilletChange;
                        MergeResult.GraduatedBCTOn = Edit.GraduatedBCTOn > Current.GraduatedBCTOn ? Edit.GraduatedBCTOn : Current.GraduatedBCTOn;
                        MergeResult.GraduatedUTCOn = Edit.GraduatedUTCOn > Current.GraduatedUTCOn ? Edit.GraduatedUTCOn : Current.GraduatedUTCOn;
                    }
                    else if (EditTime)
                    {
                        MergeResult.LastPromotion = Current.LastPromotion;
                        MergeResult.StartOfService = Current.StartOfService;
                        MergeResult.LastBilletChange = Current.LastBilletChange;
                        MergeResult.GraduatedBCTOn = Current.GraduatedBCTOn;
                        MergeResult.GraduatedUTCOn = Current.GraduatedUTCOn;
                    }
                    else if (CurrentTime)
                    {
                        MergeResult.LastPromotion = Current.LastPromotion;
                        MergeResult.StartOfService = Current.StartOfService;
                        MergeResult.LastBilletChange = Current.LastBilletChange;
                        MergeResult.GraduatedBCTOn = Current.GraduatedBCTOn;
                        MergeResult.GraduatedUTCOn = Current.GraduatedUTCOn;
                    }
                    else
                    {
                        MergeResult.LastPromotion = default;
                        MergeResult.StartOfService = default;
                        MergeResult.LastBilletChange = default;
                        MergeResult.GraduatedBCTOn = default;
                        MergeResult.GraduatedUTCOn = default;
                    }
                    break;
                case EditGroups.Notes:
                    if (EditNotes && CurrentNotes)
                    {
                        MergeResult.InitialTraining = Edit.InitialTraining + " | " + Current.InitialTraining;
                        MergeResult.UTC = Edit.UTC + " | " + Current.UTC;
                        MergeResult.Notes = Edit.Notes + ", " + Current.Notes;
                    }
                    else if (EditNotes)
                    {
                        MergeResult.InitialTraining = Edit.InitialTraining;
                        MergeResult.UTC = Edit.UTC;
                        MergeResult.Notes = Edit.Notes;
                    }
                    else if (CurrentNotes)
                    {
                        MergeResult.InitialTraining = Current.InitialTraining;
                        MergeResult.UTC = Current.UTC;
                        MergeResult.Notes = Current.Notes;
                    }
                    else
                    {
                        MergeResult.InitialTraining = default;
                        MergeResult.UTC = default;
                        MergeResult.Notes = "";
                    }
                    break;
                case EditGroups.MP:
                    if (EditMP && CurrentMP)
                    {
                        MergeResult.MilitaryPolice = Edit.MilitaryPolice || Current.MilitaryPolice;
                    }
                    else if (EditMP)
                    {
                        MergeResult.MilitaryPolice = Edit.MilitaryPolice;
                    }
                    else if (CurrentMP)
                    {
                        MergeResult.MilitaryPolice = Current.MilitaryPolice;
                    }
                    else
                    {
                        MergeResult.MilitaryPolice = false;
                    }
                    break;
            }
        }
    }
}
