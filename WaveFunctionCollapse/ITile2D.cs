using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public interface ITile2D<T, TConnector> : ITile<T> where TConnector : IConnector
    {
        /// <summary>
        /// 0 = top, 1 = right, 2 = bottom, 3 = left
        /// </summary>
        public TConnector[] Connectors { get; set; }
        public int[] Rotations { get; set; }
    }
}
