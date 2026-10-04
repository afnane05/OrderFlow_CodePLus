using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;

namespace OrderFlow.Application.Common ; 
public class OrderFlowMetrics
{
    public static readonly Meter Meter = new("OrderFlow");
    public static readonly Counter<long> OrdersCreated = Meter.CreateCounter<long>("orderflow.orders.created");
    public static readonly UpDownCounter<long> PendingOrders = Meter.CreateUpDownCounter<long>("orderflow.orders.pending");
    public static readonly Counter<long> WorderRuns = Meter.CreateCounter<long>("orderflow.Worker.runs");
    public static readonly Histogram<double> WorkerDuration = Meter.CreateHistogram<double>("oderflow.worker.duration_ms");
}