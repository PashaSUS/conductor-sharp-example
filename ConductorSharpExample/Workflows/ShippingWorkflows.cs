using ConductorSharp.Engine.Builders;
using ConductorSharp.Engine.Builders.Metadata;
using ConductorSharp.Engine.Model;
using ConductorSharpExample.Tasks.Shipping;
using ConductorSharpExample.Tasks.Notification;
using ConductorSharpExample.Tasks.Customer;
using ConductorSharpExample.Tasks.Order;

namespace ConductorSharpExample.Workflows;

// ═══════════════════════════════════════════════════════════════
// Workflow 4: Shipping & Fulfillment
// ═══════════════════════════════════════════════════════════════
public class ShipOrderInput : WorkflowInput<ShipOrderOutput>
{
    public string OrderId { get; set; }
    public int CustomerId { get; set; }
    public string ShippingMethod { get; set; }
}

public class ShipOrderOutput : WorkflowOutput
{
    public string TrackingNumber { get; set; }
    public string EstimatedDelivery { get; set; }
}

[OriginalName("WF_ship_order")]
[WorkflowMetadata(OwnerEmail = "shipping@example.com")]
public class ShipOrderWorkflow : Workflow<ShipOrderWorkflow, ShipOrderInput, ShipOrderOutput>
{
    public ShipOrderWorkflow(
        WorkflowDefinitionBuilder<ShipOrderWorkflow, ShipOrderInput, ShipOrderOutput> builder
    ) : base(builder) { }

    public GetCustomer GetCustomer { get; set; }
    public ValidateAddress ValidateAddress { get; set; }
    public CalculateShippingRate CalculateRate { get; set; }
    public CreateShippingLabel CreateLabel { get; set; }
    public EstimateDelivery EstimateDelivery { get; set; }
    public SchedulePickup SchedulePickup { get; set; }
    public UpdateOrderStatus UpdateStatus { get; set; }
    public SendEmail SendShippingNotification { get; set; }
    public SendSms SendSmsNotification { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetCustomer,
            wf => new GetCustomer.Request { CustomerId = wf.WorkflowInput.CustomerId });

        _builder.AddTask(wf => wf.ValidateAddress,
            wf => new ValidateAddress.Request { Street = wf.GetCustomer.Output.Address, City = "Springfield", State = "IL", Zip = "62701" });

        _builder.AddTask(wf => wf.CalculateRate,
            wf => new CalculateShippingRate.Request { OriginZip = "75001", DestinationZip = "62701", WeightKg = 2.5m });

        _builder.AddTask(wf => wf.CreateLabel,
            wf => new CreateShippingLabel.Request { OrderId = wf.WorkflowInput.OrderId, Address = wf.ValidateAddress.Output.NormalizedAddress, ShippingMethod = wf.WorkflowInput.ShippingMethod });

        _builder.AddTask(wf => wf.EstimateDelivery,
            wf => new EstimateDelivery.Request { DestinationZip = "62701", ShippingMethod = wf.WorkflowInput.ShippingMethod });

        _builder.AddTask(wf => wf.SchedulePickup,
            wf => new SchedulePickup.Request { WarehouseId = "WH-001", PreferredDate = "2026-03-21" });

        _builder.AddTask(wf => wf.UpdateStatus,
            wf => new UpdateOrderStatus.Request { OrderId = wf.WorkflowInput.OrderId, NewStatus = "Shipped" });

        _builder.AddTask(wf => wf.SendShippingNotification,
            wf => new SendEmail.Request { To = wf.GetCustomer.Output.Email, Subject = "Your order has shipped!", Body = $"Track your package: {wf.CreateLabel.Output.TrackingNumber}" });

        _builder.AddTask(wf => wf.SendSmsNotification,
            wf => new SendSms.Request { PhoneNumber = wf.GetCustomer.Output.Phone, Message = "Your order has shipped!" });

        _builder.SetOutput(wf => new ShipOrderOutput
        {
            TrackingNumber = wf.CreateLabel.Output.TrackingNumber,
            EstimatedDelivery = wf.EstimateDelivery.Output.EstimatedDate
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 5: Return Processing
// ═══════════════════════════════════════════════════════════════
public class ReturnProcessingInput : WorkflowInput<ReturnProcessingOutput>
{
    public string OrderId { get; set; }
    public int CustomerId { get; set; }
    public string Reason { get; set; }
}

public class ReturnProcessingOutput : WorkflowOutput
{
    public string ReturnId { get; set; }
    public string ReturnLabelUrl { get; set; }
}

[OriginalName("WF_return_processing")]
[WorkflowMetadata(OwnerEmail = "returns@example.com")]
public class ReturnProcessingWorkflow : Workflow<ReturnProcessingWorkflow, ReturnProcessingInput, ReturnProcessingOutput>
{
    public ReturnProcessingWorkflow(
        WorkflowDefinitionBuilder<ReturnProcessingWorkflow, ReturnProcessingInput, ReturnProcessingOutput> builder
    ) : base(builder) { }

    public GetCustomer GetCustomer { get; set; }
    public CreateReturn CreateReturn { get; set; }
    public UpdateOrderStatus UpdateStatus { get; set; }
    public SendEmail SendReturnEmail { get; set; }
    public LogNotificationEvent LogEvent { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetCustomer,
            wf => new GetCustomer.Request { CustomerId = wf.WorkflowInput.CustomerId });

        _builder.AddTask(wf => wf.CreateReturn,
            wf => new CreateReturn.Request { OrderId = wf.WorkflowInput.OrderId, Reason = wf.WorkflowInput.Reason });

        _builder.AddTask(wf => wf.UpdateStatus,
            wf => new UpdateOrderStatus.Request { OrderId = wf.WorkflowInput.OrderId, NewStatus = "Return Initiated" });

        _builder.AddTask(wf => wf.SendReturnEmail,
            wf => new SendEmail.Request { To = wf.GetCustomer.Output.Email, Subject = "Return Label Ready", Body = $"Your return label: {wf.CreateReturn.Output.ReturnLabelUrl}" });

        _builder.AddTask(wf => wf.LogEvent,
            wf => new LogNotificationEvent.Request { EventType = "return_initiated", Recipient = wf.GetCustomer.Output.Email, Status = "sent" });

        _builder.SetOutput(wf => new ReturnProcessingOutput
        {
            ReturnId = wf.CreateReturn.Output.ReturnId,
            ReturnLabelUrl = wf.CreateReturn.Output.ReturnLabelUrl
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 6: Shipment Tracking Notification
// ═══════════════════════════════════════════════════════════════
public class TrackingNotificationInput : WorkflowInput<TrackingNotificationOutput>
{
    public string TrackingNumber { get; set; }
    public int CustomerId { get; set; }
}

public class TrackingNotificationOutput : WorkflowOutput
{
    public string Status { get; set; }
    public string EstimatedDelivery { get; set; }
}

[OriginalName("WF_tracking_notification")]
[WorkflowMetadata(OwnerEmail = "shipping@example.com")]
public class TrackingNotificationWorkflow : Workflow<TrackingNotificationWorkflow, TrackingNotificationInput, TrackingNotificationOutput>
{
    public TrackingNotificationWorkflow(
        WorkflowDefinitionBuilder<TrackingNotificationWorkflow, TrackingNotificationInput, TrackingNotificationOutput> builder
    ) : base(builder) { }

    public GetCustomer GetCustomer { get; set; }
    public CheckNotificationPreference CheckPref { get; set; }
    public TrackPackage TrackPackage { get; set; }
    public SendEmail SendTrackingEmail { get; set; }
    public SendPush SendTrackingPush { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetCustomer,
            wf => new GetCustomer.Request { CustomerId = wf.WorkflowInput.CustomerId });

        _builder.AddTask(wf => wf.CheckPref,
            wf => new CheckNotificationPreference.Request { CustomerId = wf.WorkflowInput.CustomerId, Channel = "shipping" });

        _builder.AddTask(wf => wf.TrackPackage,
            wf => new TrackPackage.Request { TrackingNumber = wf.WorkflowInput.TrackingNumber });

        _builder.AddTask(wf => wf.SendTrackingEmail,
            wf => new SendEmail.Request { To = wf.GetCustomer.Output.Email, Subject = "Shipping Update", Body = $"Your package is: {wf.TrackPackage.Output.Status}" });

        _builder.AddTask(wf => wf.SendTrackingPush,
            wf => new SendPush.Request { UserId = wf.WorkflowInput.CustomerId, Title = "Shipping Update", Body = wf.TrackPackage.Output.Status });

        _builder.SetOutput(wf => new TrackingNotificationOutput
        {
            Status = wf.TrackPackage.Output.Status,
            EstimatedDelivery = wf.TrackPackage.Output.EstimatedDelivery
        });
    }
}
