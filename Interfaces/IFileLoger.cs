using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging.Aspects.Legacy
{
    public interface IFileLoger
    {
        void Init();
        void WriteLine(string message);
    }
}
