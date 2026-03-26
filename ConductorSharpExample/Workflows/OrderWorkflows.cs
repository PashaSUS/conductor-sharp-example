using ConductorSharp.Engine.Builders;
using ConductorSharp.Engine.Builders.Metadata;
using ConductorSharp.Engine.Model;
using ConductorSharpExample.Tasks.Customer;
using ConductorSharpExample.Tasks.Order;
using ConductorSharpExample.Tasks.Inventory;
using ConductorSharpExample.Tasks.Payment;
using ConductorSharpExample.Tasks.Shipping;
using ConductorSharpExample.Tasks.Notification;

namespace ConductorSharpExample.Workflows;

// ═══════════════════════════════════════════════════════════════
// Workflow 1: Full Order Processing Pipeline
// ═══════════════════════════════════════════════════════════════
public class OrderProcessingInput : WorkflowInput<OrderProcessingOutput>
{
    public int CustomerId { get; set; }
    public string ProductIds { get; set; }
    public string DiscountCode { get; set; }
    public string ShippingMethod { get; set; }
}

public class OrderProcessingOutput : WorkflowOutput
{
    public string OrderId { get; set; }
    public string TrackingNumber { get; set; }
    public string InvoiceUrl { get; set; }
}

[OriginalName("WF_order_processing")]
[WorkflowMetadata(OwnerEmail = "orders@example.com")]
public class OrderProcessingWorkflow : Workflow<OrderProcessingWorkflow, OrderProcessingInput, OrderProcessingOutput>
{
    public OrderProcessingWorkflow(
        WorkflowDefinitionBuilder<OrderProcessingWorkflow, OrderProcessingInput, OrderProcessingOutput> builder
    ) : base(builder) { }

    public GetCustomer GetCustomer { get; set; }
    public ValidateCustomer ValidateCustomer { get; set; }
    public CreateOrder CreateOrder { get; set; }
    public ValidateOrder ValidateOrder { get; set; }
    public CheckStock CheckStock { get; set; }
    public ReserveStock ReserveStock { get; set; }
    public ApplyDiscount ApplyDiscount { get; set; }
    public CalculateOrderTotal CalculateTotal { get; set; }
    public AuthorizePayment AuthorizePayment { get; set; }
    public CapturePayment CapturePayment { get; set; }
    public CreateShippingLabel CreateShippingLabel { get; set; }
    public GenerateInvoice GenerateInvoice { get; set; }
    public SendEmail SendConfirmationEmail { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetCustomer,
            wf => new GetCustomer.Request { CustomerId = wf.WorkflowInput.CustomerId });

        _builder.AddTask(wf => wf.ValidateCustomer,
            wf => new ValidateCustomer.Request { CustomerId = wf.WorkflowInput.CustomerId, Email = wf.GetCustomer.Output.Email });

        _builder.AddTask(wf => wf.CreateOrder,
            wf => new CreateOrder.Request { CustomerId = wf.WorkflowInput.CustomerId, ProductIds = wf.WorkflowInput.ProductIds });

        _builder.AddTask(wf => wf.ValidateOrder,
            wf => new ValidateOrder.Request { OrderId = wf.CreateOrder.Output.OrderId, CustomerId = wf.WorkflowInput.CustomerId });

        _builder.AddTask(wf => wf.CheckStock,
            wf => new CheckStock.Request { ProductId = wf.WorkflowInput.ProductIds, WarehouseId = "WH-001" });

        _builder.AddTask(wf => wf.ReserveStock,
            wf => new ReserveStock.Request { ProductId = wf.WorkflowInput.ProductIds, Quantity = 1, OrderId = wf.CreateOrder.Output.OrderId });

        _builder.AddTask(wf => wf.ApplyDiscount,
            wf => new ApplyDiscount.Request { OrderId = wf.CreateOrder.Output.OrderId, DiscountCode = wf.WorkflowInput.DiscountCode });

        _builder.AddTask(wf => wf.CalculateTotal,
            wf => new CalculateOrderTotal.Request { OrderId = wf.CreateOrder.Output.OrderId, DiscountCode = wf.WorkflowInput.DiscountCode });

        _builder.AddTask(wf => wf.AuthorizePayment,
            wf => new AuthorizePayment.Request { OrderId = wf.CreateOrder.Output.OrderId, Amount = wf.CalculateTotal.Output.Total, PaymentMethod = "credit_card" });

        _builder.AddTask(wf => wf.CapturePayment,
            wf => new CapturePayment.Request { AuthorizationCode = wf.AuthorizePayment.Output.AuthorizationCode, Amount = wf.CalculateTotal.Output.Total });

        _builder.AddTask(wf => wf.CreateShippingLabel,
            wf => new CreateShippingLabel.Request { OrderId = wf.CreateOrder.Output.OrderId, Address = wf.GetCustomer.Output.Address, ShippingMethod = wf.WorkflowInput.ShippingMethod });

        _builder.AddTask(wf => wf.GenerateInvoice,
            wf => new GenerateInvoice.Request { OrderId = wf.CreateOrder.Output.OrderId, Total = wf.CalculateTotal.Output.Total, CustomerName = wf.GetCustomer.Output.Name });

        _builder.AddTask(wf => wf.SendConfirmationEmail,
            wf => new SendEmail.Request { To = wf.GetCustomer.Output.Email, Subject = "Order Confirmed", Body = $"Your order has been placed." });

        _builder.SetOutput(wf => new OrderProcessingOutput
        {
            OrderId = wf.CreateOrder.Output.OrderId,
            TrackingNumber = wf.CreateShippingLabel.Output.TrackingNumber,
            InvoiceUrl = wf.GenerateInvoice.Output.InvoiceUrl
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 2: Customer Onboarding
// ═══════════════════════════════════════════════════════════════
public class CustomerOnboardingInput : WorkflowInput<CustomerOnboardingOutput>
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class CustomerOnboardingOutput : WorkflowOutput
{
    public int CustomerId { get; set; }
    public int LoyaltyScore { get; set; }
}

[OriginalName("WF_customer_onboarding")]
[WorkflowMetadata(OwnerEmail = "onboarding@example.com")]
public class CustomerOnboardingWorkflow : Workflow<CustomerOnboardingWorkflow, CustomerOnboardingInput, CustomerOnboardingOutput>
{
    public CustomerOnboardingWorkflow(
        WorkflowDefinitionBuilder<CustomerOnboardingWorkflow, CustomerOnboardingInput, CustomerOnboardingOutput> builder
    ) : base(builder) { }

    public CreateCustomer CreateCustomer { get; set; }
    public ValidateCustomer ValidateCustomer { get; set; }
    public GetCustomerPreferences GetPreferences { get; set; }
    public CalculateLoyaltyScore CalcLoyalty { get; set; }
    public RenderTemplate RenderWelcomeEmail { get; set; }
    public SendEmail SendWelcomeEmail { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.CreateCustomer,
            wf => new CreateCustomer.Request { Name = wf.WorkflowInput.Name, Email = wf.WorkflowInput.Email });

        _builder.AddTask(wf => wf.ValidateCustomer,
            wf => new ValidateCustomer.Request { CustomerId = wf.CreateCustomer.Output.CustomerId, Email = wf.WorkflowInput.Email });

        _builder.AddTask(wf => wf.GetPreferences,
            wf => new GetCustomerPreferences.Request { CustomerId = wf.CreateCustomer.Output.CustomerId });

        _builder.AddTask(wf => wf.CalcLoyalty,
            wf => new CalculateLoyaltyScore.Request { CustomerId = wf.CreateCustomer.Output.CustomerId, Tier = "Bronze" });

        _builder.AddTask(wf => wf.RenderWelcomeEmail,
            wf => new RenderTemplate.Request { TemplateName = "welcome", CustomerName = wf.WorkflowInput.Name, OrderId = "" });

        _builder.AddTask(wf => wf.SendWelcomeEmail,
            wf => new SendEmail.Request { To = wf.WorkflowInput.Email, Subject = wf.RenderWelcomeEmail.Output.RenderedSubject, Body = wf.RenderWelcomeEmail.Output.RenderedBody });

        _builder.SetOutput(wf => new CustomerOnboardingOutput
        {
            CustomerId = wf.CreateCustomer.Output.CustomerId,
            LoyaltyScore = wf.CalcLoyalty.Output.Score
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 3: Order Cancellation with Refund
// ═══════════════════════════════════════════════════════════════
public class OrderCancellationInput : WorkflowInput<OrderCancellationOutput>
{
    public string OrderId { get; set; }
    public int CustomerId { get; set; }
    public string Reason { get; set; }
}

public class OrderCancellationOutput : WorkflowOutput
{
    public bool Cancelled { get; set; }
    public string RefundId { get; set; }
}

[OriginalName("WF_order_cancellation")]
[WorkflowMetadata(OwnerEmail = "orders@example.com")]
public class OrderCancellationWorkflow : Workflow<OrderCancellationWorkflow, OrderCancellationInput, OrderCancellationOutput>
{
    public OrderCancellationWorkflow(
        WorkflowDefinitionBuilder<OrderCancellationWorkflow, OrderCancellationInput, OrderCancellationOutput> builder
    ) : base(builder) { }

    public GetCustomer GetCustomer { get; set; }
    public CancelOrder CancelOrder { get; set; }
    public ReleaseStock ReleaseStock { get; set; }
    public RefundPayment RefundPayment { get; set; }
    public SendEmail SendCancellationEmail { get; set; }
    public UpdateOrderStatus UpdateStatus { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetCustomer,
            wf => new GetCustomer.Request { CustomerId = wf.WorkflowInput.CustomerId });

        _builder.AddTask(wf => wf.CancelOrder,
            wf => new CancelOrder.Request { OrderId = wf.WorkflowInput.OrderId, Reason = wf.WorkflowInput.Reason });

        _builder.AddTask(wf => wf.ReleaseStock,
            wf => new ReleaseStock.Request { ReservationId = wf.WorkflowInput.OrderId });

        _builder.AddTask(wf => wf.RefundPayment,
            wf => new RefundPayment.Request { TransactionId = wf.WorkflowInput.OrderId, RefundAmount = wf.CancelOrder.Output.RefundAmount, Reason = wf.WorkflowInput.Reason });

        _builder.AddTask(wf => wf.UpdateStatus,
            wf => new UpdateOrderStatus.Request { OrderId = wf.WorkflowInput.OrderId, NewStatus = "Cancelled" });

        _builder.AddTask(wf => wf.SendCancellationEmail,
            wf => new SendEmail.Request { To = wf.GetCustomer.Output.Email, Subject = "Order Cancelled", Body = "Your order has been cancelled and a refund is being processed." });

        _builder.SetOutput(wf => new OrderCancellationOutput
        {
            Cancelled = wf.CancelOrder.Output.Cancelled,
            RefundId = wf.RefundPayment.Output.RefundId
        });
    }
}
