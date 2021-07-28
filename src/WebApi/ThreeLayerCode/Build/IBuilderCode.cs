using Sword.Nine.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeLayerCode
{
    public interface IBuilderCode
    {
        string Build(TableDto tb, ConfigModel config, List<string> ngroColumns);
        void Generator(string saveRootDire, DataSourceDto dataSourceDto);
    }
}
