using System; 
namespace FleetManagementAPI.Models
{
    public class Trajectorie
    {
        public int idtrajectorie { get; set; }
        public int taxi_id { get; set; }
        public DateTime date { get; set; }
        public float latitude{ get; set; }
        public float longitude { get; set; }
        public float? plate { get; set; }

    }
}
