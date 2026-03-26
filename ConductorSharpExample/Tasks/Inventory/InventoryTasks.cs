using ConductorSharp.Engine;
using ConductorSharp.Engine.Builders.Metadata;
using MediatR;

namespace ConductorSharpExample.Tasks.Inventory;

// ─── Task 18 ───
[OriginalName("INVENTORY_check_stock")]
public class CheckStock : TaskRequestHandler<CheckStock.Request, CheckStock.Response>
{
    public class Request : IRequest<Response>
    {
        public string ProductId { get; set; }
        public string WarehouseId { get; set; }
    }

    public class Response
    {
        public int AvailableQuantity { get; set; }
        public bool InStock { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { AvailableQuantity = 42, InStock = true });
    }
}

// ─── Task 19 ───
[OriginalName("INVENTORY_reserve_stock")]
public class ReserveStock : TaskRequestHandler<ReserveStock.Request, ReserveStock.Response>
{
    public class Request : IRequest<Response>
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public string OrderId { get; set; }
    }

    public class Response
    {
        public bool Reserved { get; set; }
        public string ReservationId { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Reserved = true, ReservationId = $"SRES-{Random.Shared.Next(10000, 99999)}" });
    }
}

// ─── Task 20 ───
[OriginalName("INVENTORY_release_stock")]
public class ReleaseStock : TaskRequestHandler<ReleaseStock.Request, ReleaseStock.Response>
{
    public class Request : IRequest<Response>
    {
        public string ReservationId { get; set; }
    }

    public class Response
    {
        public bool Released { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Released = true });
    }
}

// ─── Task 21 ───
[OriginalName("INVENTORY_update_quantity")]
public class UpdateQuantity : TaskRequestHandler<UpdateQuantity.Request, UpdateQuantity.Response>
{
    public class Request : IRequest<Response>
    {
        public string ProductId { get; set; }
        public int QuantityChange { get; set; }
        public string Reason { get; set; }
    }

    public class Response
    {
        public int NewQuantity { get; set; }
        public bool Updated { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { NewQuantity = 38, Updated = true });
    }
}

// ─── Task 22 ───
[OriginalName("INVENTORY_get_warehouse")]
public class GetWarehouse : TaskRequestHandler<GetWarehouse.Request, GetWarehouse.Response>
{
    public class Request : IRequest<Response>
    {
        public string WarehouseId { get; set; }
    }

    public class Response
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public int CurrentLoad { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Name = "Warehouse-A", Location = "Dallas, TX", Capacity = 50000, CurrentLoad = 32000 });
    }
}

// ─── Task 23 ───
[OriginalName("INVENTORY_reorder_check")]
public class ReorderCheck : TaskRequestHandler<ReorderCheck.Request, ReorderCheck.Response>
{
    public class Request : IRequest<Response>
    {
        public string ProductId { get; set; }
        public int CurrentQuantity { get; set; }
    }

    public class Response
    {
        public bool NeedsReorder { get; set; }
        public int SuggestedQuantity { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var needsReorder = request.CurrentQuantity < 10;
        return Task.FromResult(new Response { NeedsReorder = needsReorder, SuggestedQuantity = needsReorder ? 100 : 0 });
    }
}

// ─── Task 24 ───
[OriginalName("INVENTORY_transfer_stock")]
public class TransferStock : TaskRequestHandler<TransferStock.Request, TransferStock.Response>
{
    public class Request : IRequest<Response>
    {
        public string ProductId { get; set; }
        public string SourceWarehouse { get; set; }
        public string DestWarehouse { get; set; }
        public int Quantity { get; set; }
    }

    public class Response
    {
        public bool Transferred { get; set; }
        public string TransferId { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Transferred = true, TransferId = $"TRN-{Random.Shared.Next(10000, 99999)}" });
    }
}

// ─── Task 25 ───
[OriginalName("INVENTORY_audit")]
public class AuditInventory : TaskRequestHandler<AuditInventory.Request, AuditInventory.Response>
{
    public class Request : IRequest<Response>
    {
        public string WarehouseId { get; set; }
    }

    public class Response
    {
        public int TotalProducts { get; set; }
        public int Discrepancies { get; set; }
        public string AuditReportUrl { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { TotalProducts = 1250, Discrepancies = 3, AuditReportUrl = "/reports/audit-latest.pdf" });
    }
}
