using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public interface IConnector
    {
        bool CanConnectTo(IConnector other);
    }
}
