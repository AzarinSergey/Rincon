using Rincon.EntityFramwork;
using Rincon.Repository;

namespace Clt.Model.Entity
{
    public class Client : IAggregationRoot
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
    }
}
