using Microsoft.EntityFrameworkCore;

namespace FleetManagementAPI.Models
{
    public class Taxi
    {
        public int idtaxi { get; set; }
        public string plate { get; set; }
        public Taxi()
        {
            plate = ""; // inicializa la propiedad en el constructor
        }
    }
}
