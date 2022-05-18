namespace ApiCrud.Entities
{
    public class GroupPermission
    {
        public int Id { get; set; }
        public int GroupLevel { get; set; }
        public int ActionField { get; set; }
        public int Permission { get; set; }
        public DateTime Creation_date { get; set; } = DateTime.Now;
    }
    public class GroupPermissionView
    {
        public string GroupName { get; set; }
        public string Action{ get; set; }
        public string Permission { get; set; }
    }

}
