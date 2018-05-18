using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Pinger.Models;

namespace MPE.Pinger.Interfaces
{
    public interface IRepository<T>
    {
        void Write(List<T> results);
        void Write(T result);
        T Pop();
    }
}
