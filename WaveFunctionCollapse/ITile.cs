using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaveFunctionCollapse
{
    public interface ITile<T>
    {
        /// <summary>
        /// This can be anything that represents the tile, a string, a tilemap image, a mesh
        /// </summary>
        T Data { get; set; }
        double Probability { get; set; }
    }
}
