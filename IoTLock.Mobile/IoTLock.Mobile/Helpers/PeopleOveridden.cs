using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face.Contract;

namespace IoTLock.Mobile.Helpers
{
    public class PeopleOveridden : Person
    {
        public override string ToString()
        {
            return this.Name;
        }
    }
}
