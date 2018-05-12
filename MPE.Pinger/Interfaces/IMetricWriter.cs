﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPE.Pinger.Models;

namespace MPE.Pinger.Interfaces
{
    internal interface IMetricWriter
    {
        void Write(List<MetricResult> results);
        void Write(MetricResult result);
    }
}
