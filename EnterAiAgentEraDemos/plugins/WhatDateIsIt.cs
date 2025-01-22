using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace EnterAiAgentEraDemos;

public class WhatDateIsIt
{
    [KernelFunction, Description("Get the current UTC date")]
    public string Date(IFormatProvider? formatProvider = null) =>
        DateTimeOffset.UtcNow.ToString("D", formatProvider);
}
