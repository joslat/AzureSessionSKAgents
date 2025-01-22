using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace SKIntroduction;

public class WhatDateIsIt
{
    [KernelFunction, Description("Get the current UTC date")]
    public string Date(IFormatProvider? formatProvider = null) =>
        DateTimeOffset.UtcNow.ToString("D", formatProvider);
}
