using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EO
{
    public class Part
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<Wavelength> WavelengthList { get; set; }

        public Part()
        {
            this.WavelengthList = new List<Wavelength>();
        }
    }
}
