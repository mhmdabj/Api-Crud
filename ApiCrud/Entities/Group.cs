using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json.Serialization;

namespace ApiCrud.Entities
{
    public class Group
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Group_lvl { get; set; }
        public DateTime Creation_date { get; set; } = DateTime.Now;
    }

    public class GroupWithUser
    {
        public string UserName { get; set; }
        public string GroupName { get; set; }
    }

}
