using System.Diagnostics;
using System.Diagnostics.Metrics;
namespace OrderFlow.Api.Telemtry;

public static class OrderFlowTelemetry
{
    public const string ServiceName = "OrderFlow.Api" ;
    
    //ActivitySource is where custom spans/traces come from 
    public static readonly ActivitySource ActivitySource = new ActivitySource(ServiceName);

    //Meter is where custom metrics come from 
    public static readonly Meter Meter = new Meter(ServiceName);


}