using ConductorSharp.Engine.Builders;
using ConductorSharp.Engine.Builders.Metadata;
using ConductorSharp.Engine.Model;
using ConductorSharpExample.Tasks.Payment;
using ConductorSharpExample.Tasks.Customer;
using ConductorSharpExample.Tasks.Order;
using ConductorSharpExample.Tasks.Notification;
using ConductorSharpExample.Tasks.Auth;

namespace ConductorSharpExample.Workflows;

// ═══════════════════════════════════════════════════════════════
// Workflow 7: Payment Processing
// ═══════════════════════════════════════════════════════════════
public class PaymentProcessingInput : WorkflowInput<PaymentProcessingOutput>
{
    public string OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; }
    public int CustomerId { get; set; }
}

public class PaymentProcessingOutput : WorkflowOutput
{
    public string TransactionId { get; set; }
    public string ReceiptUrl { get; set; }
}

[OriginalName("WF_payment_processing")]
[WorkflowMetadata(OwnerEmail = "payments@example.com")]
public class PaymentProcessingWorkflow : Workflow<PaymentProcessingWorkflow, PaymentProcessingInput, PaymentProcessingOutput>
{
    public PaymentProcessingWorkflow(
        WorkflowDefinitionBuilder<PaymentProcessingWorkflow, PaymentProcessingInput, PaymentProcessingOutput> builder
    ) : base(builder) { }

    public ValidatePaymentMethod ValidateMethod { get; set; }
    public FraudCheck FraudCheck { get; set; }
    public CalculateTax CalculateTax { get; set; }
    public AuthorizePayment Authorize { get; set; }
    public CapturePayment Capture { get; set; }
    public GenerateReceipt GenerateReceipt { get; set; }
    public GetCustomer GetCustomer { get; set; }
    public SendEmail SendReceiptEmail { get; set; }
    public AuditLog AuditLog { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetCustomer,
            wf => new GetCustomer.Request { CustomerId = wf.WorkflowInput.CustomerId });

        _builder.AddTask(wf => wf.ValidateMethod,
            wf => new ValidatePaymentMethod.Request { PaymentMethod = wf.WorkflowInput.PaymentMethod, Token = "tok_default" });

        _builder.AddTask(wf => wf.FraudCheck,
            wf => new FraudCheck.Request { OrderId = wf.WorkflowInput.OrderId, Amount = wf.WorkflowInput.Amount, CustomerId = wf.WorkflowInput.CustomerId });

        _builder.AddTask(wf => wf.CalculateTax,
            wf => new CalculateTax.Request { Amount = wf.WorkflowInput.Amount, State = "TX", Country = "US" });

        _builder.AddTask(wf => wf.Authorize,
            wf => new AuthorizePayment.Request { OrderId = wf.WorkflowInput.OrderId, Amount = wf.WorkflowInput.Amount, PaymentMethod = wf.WorkflowInput.PaymentMethod });

        _builder.AddTask(wf => wf.Capture,
            wf => new CapturePayment.Request { AuthorizationCode = wf.Authorize.Output.AuthorizationCode, Amount = wf.WorkflowInput.Amount });

        _builder.AddTask(wf => wf.GenerateReceipt,
            wf => new GenerateReceipt.Request { TransactionId = wf.Capture.Output.TransactionId, CustomerEmail = wf.GetCustomer.Output.Email });

        _builder.AddTask(wf => wf.SendReceiptEmail,
            wf => new SendEmail.Request { To = wf.GetCustomer.Output.Email, Subject = "Payment Receipt", Body = $"Receipt: {wf.GenerateReceipt.Output.ReceiptUrl}" });

        _builder.AddTask(wf => wf.AuditLog,
            wf => new AuditLog.Request { UserId = $"{wf.WorkflowInput.CustomerId}", Action = "payment_captured", Resource = wf.WorkflowInput.OrderId, IpAddress = "0.0.0.0" });

        _builder.SetOutput(wf => new PaymentProcessingOutput
        {
            TransactionId = wf.Capture.Output.TransactionId,
            ReceiptUrl = wf.GenerateReceipt.Output.ReceiptUrl
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 8: Subscription Billing
// ═══════════════════════════════════════════════════════════════
public class SubscriptionBillingInput : WorkflowInput<SubscriptionBillingOutput>
{
    public int CustomerId { get; set; }
    public string PlanId { get; set; }
    public string BillingCycle { get; set; }
}

public class SubscriptionBillingOutput : WorkflowOutput
{
    public string SubscriptionId { get; set; }
    public string TransactionId { get; set; }
}

[OriginalName("WF_subscription_billing")]
[WorkflowMetadata(OwnerEmail = "billing@example.com")]
public class SubscriptionBillingWorkflow : Workflow<SubscriptionBillingWorkflow, SubscriptionBillingInput, SubscriptionBillingOutput>
{
    public SubscriptionBillingWorkflow(
        WorkflowDefinitionBuilder<SubscriptionBillingWorkflow, SubscriptionBillingInput, SubscriptionBillingOutput> builder
    ) : base(builder) { }

    public GetCustomer GetCustomer { get; set; }
    public ValidatePaymentMethod ValidateMethod { get; set; }
    public ProcessSubscription ProcessSub { get; set; }
    public AuthorizePayment Authorize { get; set; }
    public CapturePayment Capture { get; set; }
    public GenerateReceipt GenerateReceipt { get; set; }
    public SendEmail SendBillingEmail { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetCustomer,
            wf => new GetCustomer.Request { CustomerId = wf.WorkflowInput.CustomerId });

        _builder.AddTask(wf => wf.ValidateMethod,
            wf => new ValidatePaymentMethod.Request { PaymentMethod = "credit_card", Token = "tok_saved" });

        _builder.AddTask(wf => wf.ProcessSub,
            wf => new ProcessSubscription.Request { CustomerId = wf.WorkflowInput.CustomerId, PlanId = wf.WorkflowInput.PlanId, BillingCycle = wf.WorkflowInput.BillingCycle });

        _builder.AddTask(wf => wf.Authorize,
            wf => new AuthorizePayment.Request { OrderId = wf.ProcessSub.Output.SubscriptionId, Amount = 29.99m, PaymentMethod = "credit_card" });

        _builder.AddTask(wf => wf.Capture,
            wf => new CapturePayment.Request { AuthorizationCode = wf.Authorize.Output.AuthorizationCode, Amount = 29.99m });

        _builder.AddTask(wf => wf.GenerateReceipt,
            wf => new GenerateReceipt.Request { TransactionId = wf.Capture.Output.TransactionId, CustomerEmail = wf.GetCustomer.Output.Email });

        _builder.AddTask(wf => wf.SendBillingEmail,
            wf => new SendEmail.Request { To = wf.GetCustomer.Output.Email, Subject = "Subscription Renewed", Body = $"Your subscription is active. Receipt: {wf.GenerateReceipt.Output.ReceiptUrl}" });

        _builder.SetOutput(wf => new SubscriptionBillingOutput
        {
            SubscriptionId = wf.ProcessSub.Output.SubscriptionId,
            TransactionId = wf.Capture.Output.TransactionId
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 9: Refund Processing
// ═══════════════════════════════════════════════════════════════
public class RefundProcessingInput : WorkflowInput<RefundProcessingOutput>
{
    public string OrderId { get; set; }
    public string TransactionId { get; set; }
    public decimal RefundAmount { get; set; }
    public int CustomerId { get; set; }
    public string Reason { get; set; }
}

public class RefundProcessingOutput : WorkflowOutput
{
    public string RefundId { get; set; }
    public bool Refunded { get; set; }
}

[OriginalName("WF_refund_processing")]
[WorkflowMetadata(OwnerEmail = "payments@example.com")]
public class RefundProcessingWorkflow : Workflow<RefundProcessingWorkflow, RefundProcessingInput, RefundProcessingOutput>
{
    public RefundProcessingWorkflow(
        WorkflowDefinitionBuilder<RefundProcessingWorkflow, RefundProcessingInput, RefundProcessingOutput> builder
    ) : base(builder) { }

    public GetCustomer GetCustomer { get; set; }
    public RefundPayment Refund { get; set; }
    public UpdateOrderStatus UpdateStatus { get; set; }
    public SendEmail SendRefundEmail { get; set; }
    public AuditLog AuditLog { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetCustomer,
            wf => new GetCustomer.Request { CustomerId = wf.WorkflowInput.CustomerId });

        _builder.AddTask(wf => wf.Refund,
            wf => new RefundPayment.Request { TransactionId = wf.WorkflowInput.TransactionId, RefundAmount = wf.WorkflowInput.RefundAmount, Reason = wf.WorkflowInput.Reason });

        _builder.AddTask(wf => wf.UpdateStatus,
            wf => new UpdateOrderStatus.Request { OrderId = wf.WorkflowInput.OrderId, NewStatus = "Refunded" });

        _builder.AddTask(wf => wf.SendRefundEmail,
            wf => new SendEmail.Request { To = wf.GetCustomer.Output.Email, Subject = "Refund Processed", Body = $"Your refund of ${wf.WorkflowInput.RefundAmount} has been processed." });

        _builder.AddTask(wf => wf.AuditLog,
            wf => new AuditLog.Request { UserId = $"{wf.WorkflowInput.CustomerId}", Action = "refund_processed", Resource = wf.WorkflowInput.OrderId, IpAddress = "0.0.0.0" });

        _builder.SetOutput(wf => new RefundProcessingOutput
        {
            RefundId = wf.Refund.Output.RefundId,
            Refunded = wf.Refund.Output.Refunded
        });
    }
}
