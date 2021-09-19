using FiveOhFirstDataCore.Data.Structures;

namespace FiveOhFirstDataCore.Data.Structures
{
    public class CShopRoleBinding
    {
        public CShop Key { get; set; }

        public List<CShopDepartmentBinding> Departments { get; set; } = new();
    }

    public class CShopDepartmentBinding
    {
        public Guid Key { get; set; }
        public string Id { get; set; }

        public List<CShopRoleBindingData> Roles { get; set; } = new();

        public CShop ParentKey { get; set; }
        public CShopRoleBinding Parent { get; set; }
    }

    public class CShopRoleBindingData
    {
        public Guid Key { get; set; }
        public string Id { get; set; }

        public List<ulong> Roles { get; set; } = new();

        public Guid ParentKey { get; set; }
        public CShopDepartmentBinding Parent { get; set; }
    }
}
