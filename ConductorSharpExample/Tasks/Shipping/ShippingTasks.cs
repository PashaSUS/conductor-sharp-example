using ConductorSharp.Engine;
using ConductorSharp.Engine.Builders.Metadata;
using MediatR;

namespace ConductorSharpExample.Tasks.Shipping;

// ─── Task 35 ───
[OriginalName("SHIPPING_calculate_rate")]
public class CalculateShippingRate : TaskRequestHandler<CalculateShippingRate.Request, CalculateShippingRate.Response>
{
    public class Request : IRequest<Response>
    {
        public string OriginZip { get; set; }
        public string DestinationZip { get; set; }
        public decimal WeightKg { get; set; }
    }

    public class Response
    {
        public decimal StandardRate { get; set; }
        public decimal ExpressRate { get; set; }
        public decimal OvernightRate { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { StandardRate = 5.99m, ExpressRate = 12.99m, OvernightRate = 24.99m });
    }
}

// ─── Task 36 ───
[OriginalName("SHIPPING_create_label")]
public class CreateShippingLabel : TaskRequestHandler<CreateShippingLabel.Request, CreateShippingLabel.Response>
{
    public class Request : IRequest<Response>
    {
        public string OrderId { get; set; }
        public string Address { get; set; }
        public string ShippingMethod { get; set; }
    }

    public class Response
    {
        public string TrackingNumber { get; set; }
        public string LabelUrl { get; set; }
        public string Carrier { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response
        {
            TrackingNumber = $"1Z{Random.Shared.Next(100000000, 999999999)}",
            LabelUrl = "/labels/latest.pdf",
            Carrier = "UPS"
        });
    }
}

// ─── Task 37 ───
[OriginalName("SHIPPING_track_package")]
public class TrackPackage : TaskRequestHandler<TrackPackage.Request, TrackPackage.Response>
{
    public class Request : IRequest<Response>
    {
        public string TrackingNumber { get; set; }
    }

    public class Response
    {
        public string Status { get; set; }
        public string CurrentLocation { get; set; }
        public string EstimatedDelivery { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Status = "In Transit", CurrentLocation = "Memphis, TN", EstimatedDelivery = "2026-03-23" });
    }
}

// ─── Task 38 ───
[OriginalName("SHIPPING_validate_address")]
public class ValidateAddress : TaskRequestHandler<ValidateAddress.Request, ValidateAddress.Response>
{
    public class Request : IRequest<Response>
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public class Response
    {
        public bool IsValid { get; set; }
        public string NormalizedAddress { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { IsValid = true, NormalizedAddress = $"{request.Street}, {request.City}, {request.State} {request.Zip}" });
    }
}

// ─── Task 39 ───
[OriginalName("SHIPPING_schedule_pickup")]
public class SchedulePickup : TaskRequestHandler<SchedulePickup.Request, SchedulePickup.Response>
{
    public class Request : IRequest<Response>
    {
        public string WarehouseId { get; set; }
        public string PreferredDate { get; set; }
    }

    public class Response
    {
        public string PickupId { get; set; }
        public string ScheduledDate { get; set; }
        public string TimeWindow { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { PickupId = $"PU-{Random.Shared.Next(10000, 99999)}", ScheduledDate = request.PreferredDate, TimeWindow = "9:00 AM - 12:00 PM" });
    }
}

// ─── Task 40 ───
[OriginalName("SHIPPING_create_return")]
public class CreateReturn : TaskRequestHandler<CreateReturn.Request, CreateReturn.Response>
{
    public class Request : IRequest<Response>
    {
        public string OrderId { get; set; }
        public string Reason { get; set; }
    }

    public class Response
    {
        public string ReturnId { get; set; }
        public string ReturnLabelUrl { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { ReturnId = $"RET-{Random.Shared.Next(10000, 99999)}", ReturnLabelUrl = "/returns/label-latest.pdf" });
    }
}

// ─── Task 41 ───
[OriginalName("SHIPPING_estimate_delivery")]
public class EstimateDelivery : TaskRequestHandler<EstimateDelivery.Request, EstimateDelivery.Response>
{
    public class Request : IRequest<Response>
    {
        public string DestinationZip { get; set; }
        public string ShippingMethod { get; set; }
    }

    public class Response
    {
        public int EstimatedDays { get; set; }
        public string EstimatedDate { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var days = request.ShippingMethod == "express" ? 2 : 5;
        return Task.FromResult(new Response { EstimatedDays = days, EstimatedDate = DateTime.UtcNow.AddDays(days).ToString("yyyy-MM-dd") });
    }
}

// ─── Task 42 ───
[OriginalName("SHIPPING_notify_carrier")]
public class NotifyCarrier : TaskRequestHandler<NotifyCarrier.Request, NotifyCarrier.Response>
{
    public class Request : IRequest<Response>
    {
        public string Carrier { get; set; }
        public string TrackingNumber { get; set; }
        public string Message { get; set; }
    }

    public class Response
    {
        public bool Notified { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Notified = true });
    }
}
