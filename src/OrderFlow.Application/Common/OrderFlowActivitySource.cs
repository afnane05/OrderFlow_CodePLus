using System.Diagnostics;

namespace OrderFlow.Application.Common;

public static class OrderFlowActivitySource
{
    public static readonly ActivitySource Source = new("OrderFlow");
}