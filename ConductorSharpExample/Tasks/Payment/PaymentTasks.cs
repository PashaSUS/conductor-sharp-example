using ConductorSharp.Engine;
using ConductorSharp.Engine.Builders.Metadata;
using MediatR;

namespace ConductorSharpExample.Tasks.Payment;

// ─── Task 26 ───
[OriginalName("PAYMENT_authorize")]
public class AuthorizePayment : TaskRequestHandler<AuthorizePayment.Request, AuthorizePayment.Response>
{
    public class Request : IRequest<Response>
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
    }

    public class Response
    {
        public bool Authorized { get; set; }
        public string AuthorizationCode { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Authorized = true, AuthorizationCode = $"AUTH-{Random.Shared.Next(100000, 999999)}" });
    }
}

// ─── Task 27 ───
[OriginalName("PAYMENT_capture")]
public class CapturePayment : TaskRequestHandler<CapturePayment.Request, CapturePayment.Response>
{
    public class Request : IRequest<Response>
    {
        public string AuthorizationCode { get; set; }
        public decimal Amount { get; set; }
    }

    public class Response
    {
        public bool Captured { get; set; }
        public string TransactionId { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Captured = true, TransactionId = $"TXN-{Random.Shared.Next(100000, 999999)}" });
    }
}

// ─── Task 28 ───
[OriginalName("PAYMENT_refund")]
public class RefundPayment : TaskRequestHandler<RefundPayment.Request, RefundPayment.Response>
{
    public class Request : IRequest<Response>
    {
        public string TransactionId { get; set; }
        public decimal RefundAmount { get; set; }
        public string Reason { get; set; }
    }

    public class Response
    {
        public bool Refunded { get; set; }
        public string RefundId { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Refunded = true, RefundId = $"REF-{Random.Shared.Next(100000, 999999)}" });
    }
}

// ─── Task 29 ───
[OriginalName("PAYMENT_validate_method")]
public class ValidatePaymentMethod : TaskRequestHandler<ValidatePaymentMethod.Request, ValidatePaymentMethod.Response>
{
    public class Request : IRequest<Response>
    {
        public string PaymentMethod { get; set; }
        public string Token { get; set; }
    }

    public class Response
    {
        public bool IsValid { get; set; }
        public string CardLast4 { get; set; }
        public string CardBrand { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { IsValid = true, CardLast4 = "4242", CardBrand = "Visa" });
    }
}

// ─── Task 30 ───
[OriginalName("PAYMENT_fraud_check")]
public class FraudCheck : TaskRequestHandler<FraudCheck.Request, FraudCheck.Response>
{
    public class Request : IRequest<Response>
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public int CustomerId { get; set; }
    }

    public class Response
    {
        public string RiskLevel { get; set; }
        public decimal RiskScore { get; set; }
        public bool Approved { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { RiskLevel = "Low", RiskScore = 0.12m, Approved = true });
    }
}

// ─── Task 31 ───
[OriginalName("PAYMENT_calculate_tax")]
public class CalculateTax : TaskRequestHandler<CalculateTax.Request, CalculateTax.Response>
{
    public class Request : IRequest<Response>
    {
        public decimal Amount { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }

    public class Response
    {
        public decimal TaxAmount { get; set; }
        public decimal TaxRate { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { TaxAmount = request.Amount * 0.08m, TaxRate = 0.08m });
    }
}

// ─── Task 32 ───
[OriginalName("PAYMENT_process_subscription")]
public class ProcessSubscription : TaskRequestHandler<ProcessSubscription.Request, ProcessSubscription.Response>
{
    public class Request : IRequest<Response>
    {
        public int CustomerId { get; set; }
        public string PlanId { get; set; }
        public string BillingCycle { get; set; }
    }

    public class Response
    {
        public string SubscriptionId { get; set; }
        public string NextBillingDate { get; set; }
        public bool Active { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { SubscriptionId = $"SUB-{Random.Shared.Next(10000, 99999)}", NextBillingDate = "2026-04-20", Active = true });
    }
}

// ─── Task 33 ───
[OriginalName("PAYMENT_generate_receipt")]
public class GenerateReceipt : TaskRequestHandler<GenerateReceipt.Request, GenerateReceipt.Response>
{
    public class Request : IRequest<Response>
    {
        public string TransactionId { get; set; }
        public string CustomerEmail { get; set; }
    }

    public class Response
    {
        public string ReceiptId { get; set; }
        public string ReceiptUrl { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var receiptId = $"RCP-{Random.Shared.Next(100000, 999999)}";
        return Task.FromResult(new Response { ReceiptId = receiptId, ReceiptUrl = $"/receipts/{receiptId}.pdf" });
    }
}

// ─── Task 34 ───
[OriginalName("PAYMENT_void")]
public class VoidPayment : TaskRequestHandler<VoidPayment.Request, VoidPayment.Response>
{
    public class Request : IRequest<Response>
    {
        public string AuthorizationCode { get; set; }
        public string Reason { get; set; }
    }

    public class Response
    {
        public bool Voided { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Voided = true });
    }
}
