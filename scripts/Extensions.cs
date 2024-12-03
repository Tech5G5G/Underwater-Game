using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnderwaterGame
{
    public static class Extensions
    {
        /// <summary>Convert a boolean value to a float</summary>
        /// <param name="input">bool to convert</param>
        /// <returns>1f for true, 0f for false</returns>
        public static float ToFloat(this bool input) => input ? 1f : 0f;
    }
}
