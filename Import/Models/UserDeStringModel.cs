using Npoi.Mapper.Attributes;

namespace DC360.Import.Api.Import.Models
{
    public class UserDeStringModel : IUserStringModel
    {
        [Column("Vorname")]
        public string FirstName { get; set; }

        [Column("Geburtstag")]
        public string BirthDate { get; set; }
    }
}
