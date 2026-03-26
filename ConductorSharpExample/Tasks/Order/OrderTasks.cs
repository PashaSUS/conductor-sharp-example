using ConductorSharp.Engine;
using ConductorSharp.Engine.Builders.Metadata;
using MediatR;

namespace ConductorSharpExample.Tasks.Order;

// ─── Task 9 ───
[OriginalName("ORDER_create")]
public class CreateOrder : TaskRequestHandler<CreateOrder.Request, CreateOrder.Response>
{
    public class Request : IRequest<Response>
    {
        public int CustomerId { get; set; }
        public string ProductIds { get; set; }
    }

    public class Response
    {
        public string OrderId { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response
        {
            OrderId = $"ORD-{Random.Shared.Next(100000, 999999)}",
            Status = "Created",
            TotalAmount = 149.99m
        });
    }
}

// ─── Task 10 ───
[OriginalName("ORDER_validate")]
public class ValidateOrder : TaskRequestHandler<ValidateOrder.Request, ValidateOrder.Response>
{
    public class Request : IRequest<Response>
    {
        public string OrderId { get; set; }
        public int CustomerId { get; set; }
    }

    public class Response
    {
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { IsValid = true, ValidationMessage = "Order is valid" });
    }
}

// ─── Task 11 ───
[OriginalName("ORDER_calculate_total")]
public class CalculateOrderTotal : TaskRequestHandler<CalculateOrderTotal.Request, CalculateOrderTotal.Response>
{
    public class Request : IRequest<Response>
    {
        public string OrderId { get; set; }
        public string DiscountCode { get; set; }
    }

    public class Response
    {
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Subtotal = 129.99m, Tax = 10.40m, Discount = 10.00m, Total = 130.39m });
    }
}

// ─── Task 12 ───
[OriginalName("ORDER_apply_discount")]
public class ApplyDiscount : TaskRequestHandler<ApplyDiscount.Request, ApplyDiscount.Response>
{
    public class Request : IRequest<Response>
    {
        public string OrderId { get; set; }
        public string DiscountCode { get; set; }
    }

    public class Response
    {
        public bool Applied { get; set; }
        public decimal DiscountAmount { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Applied = true, DiscountAmount = 10.00m });
    }
}

// ─── Task 13 ───
[OriginalName("ORDER_reserve_items")]
public class ReserveOrderItems : TaskRequestHandler<ReserveOrderItems.Request, ReserveOrderItems.Response>
{
    public class Request : IRequest<Response>
    {
        public string OrderId { get; set; }
        public string ProductIds { get; set; }
    }

    public class Response
    {
        public bool AllReserved { get; set; }
        public string ReservationId { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { AllReserved = true, ReservationId = $"RES-{Random.Shared.Next(10000, 99999)}" });
    }
}

// ─── Task 14 ───
[OriginalName("ORDER_cancel")]
public class CancelOrder : TaskRequestHandler<CancelOrder.Request, CancelOrder.Response>
{
    public class Request : IRequest<Response>
    {
        public string OrderId { get; set; }
        public string Reason { get; set; }
    }

    public class Response
    {
        public bool Cancelled { get; set; }
        public decimal RefundAmount { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Cancelled = true, RefundAmount = 149.99m });
    }
}

// ─── Task 15 ───
[OriginalName("ORDER_update_status")]
public class UpdateOrderStatus : TaskRequestHandler<UpdateOrderStatus.Request, UpdateOrderStatus.Response>
{
    public class Request : IRequest<Response>
    {
        public string OrderId { get; set; }
        public string NewStatus { get; set; }
    }

    public class Response
    {
        public bool Updated { get; set; }
        public string PreviousStatus { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Updated = true, PreviousStatus = "Created" });
    }
}

// ─── Task 16 ───
[OriginalName("ORDER_get_history")]
public class GetOrderHistory : TaskRequestHandler<GetOrderHistory.Request, GetOrderHistory.Response>
{
    public class Request : IRequest<Response>
    {
        public int CustomerId { get; set; }
    }

    public class Response
    {
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public string LastOrderDate { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { TotalOrders = 15, TotalSpent = 2340.50m, LastOrderDate = "2026-03-15" });
    }
}

// ─── Task 17 ───
[OriginalName("ORDER_generate_invoice")]
public class GenerateInvoice : TaskRequestHandler<GenerateInvoice.Request, GenerateInvoice.Response>
{
    public class Request : IRequest<Response>
    {
        public string OrderId { get; set; }
        public decimal Total { get; set; }
        public string CustomerName { get; set; }
    }

    public class Response
    {
        public string InvoiceId { get; set; }
        public string InvoiceUrl { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var invoiceId = $"INV-{Random.Shared.Next(100000, 999999)}";
        return Task.FromResult(new Response { InvoiceId = invoiceId, InvoiceUrl = $"/invoices/{invoiceId}.pdf" });
    }
}
