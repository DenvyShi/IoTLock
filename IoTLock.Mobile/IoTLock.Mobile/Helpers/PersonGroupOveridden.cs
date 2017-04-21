using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTLock.Mobile.Helpers
{
    public class PersonGroupOveridden : PersonGroup
    {
        public override string ToString()
        {
            return this.Name;
        }
    }
}
