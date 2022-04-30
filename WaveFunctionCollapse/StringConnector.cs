using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public class StringConnector : IConnector
    {
        private string str;

        public StringConnector(string str)
        {
            this.str = str;
        }

        public static implicit operator StringConnector(string str)
        {
            return new StringConnector(str);
        }

        public bool CanConnectTo(IConnector other)
        {
            if(other != null && other is StringConnector cc)
            {
                return cc.str == str;
            }
            return false;
        }
    }
}
