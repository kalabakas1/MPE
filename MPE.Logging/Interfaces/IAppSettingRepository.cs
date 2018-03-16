using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPE.Logging.Interfaces
{
    public interface IAppSettingRepository
    {
        T Get<T>(string key);
    }
}
