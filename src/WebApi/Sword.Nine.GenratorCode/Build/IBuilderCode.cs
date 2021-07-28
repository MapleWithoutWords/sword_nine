using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sword.Nine.GenratorCode
{
    public interface IBuilderCode
    {
        string Build(DbTable tableList, ConfigModel config, List<string> ngroColumns);
        void Generator(string saveRootDire, List<DbTable> tableList);
    }
}
