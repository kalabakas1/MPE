using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Pinger.Models;
using MPE.Pinger.Models.Results;

namespace MPE.Pinger.Interfaces
{
    internal interface ICollector
    {
        List<MetricResult> Collect();
    }
}
