using ConductorSharp.Engine.Builders;
using ConductorSharp.Engine.Builders.Metadata;
using ConductorSharp.Engine.Model;
using ConductorSharpExample.Tasks.Inventory;
using ConductorSharpExample.Tasks.Notification;
using ConductorSharpExample.Tasks.Product;
using ConductorSharpExample.Tasks.Reporting;

namespace ConductorSharpExample.Workflows;

// ═══════════════════════════════════════════════════════════════
// Workflow 10: Inventory Replenishment
// ═══════════════════════════════════════════════════════════════
public class InventoryReplenishInput : WorkflowInput<InventoryReplenishOutput>
{
    public string ProductId { get; set; }
    public string WarehouseId { get; set; }
}

public class InventoryReplenishOutput : WorkflowOutput
{
    public bool Replenished { get; set; }
    public int NewQuantity { get; set; }
}

[OriginalName("WF_inventory_replenish")]
[WorkflowMetadata(OwnerEmail = "inventory@example.com")]
public class InventoryReplenishWorkflow : Workflow<InventoryReplenishWorkflow, InventoryReplenishInput, InventoryReplenishOutput>
{
    public InventoryReplenishWorkflow(
        WorkflowDefinitionBuilder<InventoryReplenishWorkflow, InventoryReplenishInput, InventoryReplenishOutput> builder
    ) : base(builder) { }

    public CheckStock CheckStock { get; set; }
    public ReorderCheck ReorderCheck { get; set; }
    public GetWarehouse GetWarehouse { get; set; }
    public UpdateQuantity UpdateQuantity { get; set; }
    public SendSlack NotifyTeam { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.CheckStock,
            wf => new CheckStock.Request { ProductId = wf.WorkflowInput.ProductId, WarehouseId = wf.WorkflowInput.WarehouseId });

        _builder.AddTask(wf => wf.ReorderCheck,
            wf => new ReorderCheck.Request { ProductId = wf.WorkflowInput.ProductId, CurrentQuantity = wf.CheckStock.Output.AvailableQuantity });

        _builder.AddTask(wf => wf.GetWarehouse,
            wf => new GetWarehouse.Request { WarehouseId = wf.WorkflowInput.WarehouseId });

        _builder.AddTask(wf => wf.UpdateQuantity,
            wf => new UpdateQuantity.Request { ProductId = wf.WorkflowInput.ProductId, QuantityChange = wf.ReorderCheck.Output.SuggestedQuantity, Reason = "Auto-replenishment" });

        _builder.AddTask(wf => wf.NotifyTeam,
            wf => new SendSlack.Request { Channel = "#inventory", Message = $"Replenished {wf.WorkflowInput.ProductId} at {wf.GetWarehouse.Output.Name}" });

        _builder.SetOutput(wf => new InventoryReplenishOutput
        {
            Replenished = wf.UpdateQuantity.Output.Updated,
            NewQuantity = wf.UpdateQuantity.Output.NewQuantity
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 11: Warehouse Audit
// ═══════════════════════════════════════════════════════════════
public class WarehouseAuditInput : WorkflowInput<WarehouseAuditOutput>
{
    public string WarehouseId { get; set; }
    public string RecipientEmail { get; set; }
}

public class WarehouseAuditOutput : WorkflowOutput
{
    public string AuditReportUrl { get; set; }
    public int Discrepancies { get; set; }
}

[OriginalName("WF_warehouse_audit")]
[WorkflowMetadata(OwnerEmail = "inventory@example.com")]
public class WarehouseAuditWorkflow : Workflow<WarehouseAuditWorkflow, WarehouseAuditInput, WarehouseAuditOutput>
{
    public WarehouseAuditWorkflow(
        WorkflowDefinitionBuilder<WarehouseAuditWorkflow, WarehouseAuditInput, WarehouseAuditOutput> builder
    ) : base(builder) { }

    public GetWarehouse GetWarehouse { get; set; }
    public AuditInventory Audit { get; set; }
    public GenerateInventoryReport GenReport { get; set; }
    public SendEmail SendAuditEmail { get; set; }
    public SendSlack NotifySlack { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetWarehouse,
            wf => new GetWarehouse.Request { WarehouseId = wf.WorkflowInput.WarehouseId });

        _builder.AddTask(wf => wf.Audit,
            wf => new AuditInventory.Request { WarehouseId = wf.WorkflowInput.WarehouseId });

        _builder.AddTask(wf => wf.GenReport,
            wf => new GenerateInventoryReport.Request { WarehouseId = wf.WorkflowInput.WarehouseId });

        _builder.AddTask(wf => wf.SendAuditEmail,
            wf => new SendEmail.Request { To = wf.WorkflowInput.RecipientEmail, Subject = $"Audit Report - {wf.GetWarehouse.Output.Name}", Body = $"Found {wf.Audit.Output.Discrepancies} discrepancies" });

        _builder.AddTask(wf => wf.NotifySlack,
            wf => new SendSlack.Request { Channel = "#warehouse-ops", Message = $"Audit complete for {wf.GetWarehouse.Output.Name}: {wf.Audit.Output.Discrepancies} discrepancies" });

        _builder.SetOutput(wf => new WarehouseAuditOutput
        {
            AuditReportUrl = wf.Audit.Output.AuditReportUrl,
            Discrepancies = wf.Audit.Output.Discrepancies
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 12: Stock Transfer
// ═══════════════════════════════════════════════════════════════
public class StockTransferInput : WorkflowInput<StockTransferOutput>
{
    public string ProductId { get; set; }
    public string SourceWarehouse { get; set; }
    public string DestWarehouse { get; set; }
    public int Quantity { get; set; }
}

public class StockTransferOutput : WorkflowOutput
{
    public string TransferId { get; set; }
    public bool Success { get; set; }
}

[OriginalName("WF_stock_transfer")]
[WorkflowMetadata(OwnerEmail = "inventory@example.com")]
public class StockTransferWorkflow : Workflow<StockTransferWorkflow, StockTransferInput, StockTransferOutput>
{
    public StockTransferWorkflow(
        WorkflowDefinitionBuilder<StockTransferWorkflow, StockTransferInput, StockTransferOutput> builder
    ) : base(builder) { }

    public CheckStock CheckSourceStock { get; set; }
    public GetWarehouse GetSourceWarehouse { get; set; }
    public TransferStock Transfer { get; set; }
    public SendSlack NotifyTransfer { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.CheckSourceStock,
            wf => new CheckStock.Request { ProductId = wf.WorkflowInput.ProductId, WarehouseId = wf.WorkflowInput.SourceWarehouse });

        _builder.AddTask(wf => wf.GetSourceWarehouse,
            wf => new GetWarehouse.Request { WarehouseId = wf.WorkflowInput.SourceWarehouse });

        _builder.AddTask(wf => wf.Transfer,
            wf => new TransferStock.Request { ProductId = wf.WorkflowInput.ProductId, SourceWarehouse = wf.WorkflowInput.SourceWarehouse, DestWarehouse = wf.WorkflowInput.DestWarehouse, Quantity = wf.WorkflowInput.Quantity });

        _builder.AddTask(wf => wf.NotifyTransfer,
            wf => new SendSlack.Request { Channel = "#warehouse-ops", Message = $"Transfer {wf.Transfer.Output.TransferId}: {wf.WorkflowInput.Quantity} units moved" });

        _builder.SetOutput(wf => new StockTransferOutput
        {
            TransferId = wf.Transfer.Output.TransferId,
            Success = wf.Transfer.Output.Transferred
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 13: Product Catalog Update
// ═══════════════════════════════════════════════════════════════
public class ProductCatalogUpdateInput : WorkflowInput<ProductCatalogUpdateOutput>
{
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
}

public class ProductCatalogUpdateOutput : WorkflowOutput
{
    public string ProductId { get; set; }
    public string Description { get; set; }
}

[OriginalName("WF_product_catalog_update")]
[WorkflowMetadata(OwnerEmail = "products@example.com")]
public class ProductCatalogUpdateWorkflow : Workflow<ProductCatalogUpdateWorkflow, ProductCatalogUpdateInput, ProductCatalogUpdateOutput>
{
    public ProductCatalogUpdateWorkflow(
        WorkflowDefinitionBuilder<ProductCatalogUpdateWorkflow, ProductCatalogUpdateInput, ProductCatalogUpdateOutput> builder
    ) : base(builder) { }

    public CreateProduct CreateProduct { get; set; }
    public ValidateProduct ValidateProduct { get; set; }
    public GenerateProductDescription GenDescription { get; set; }
    public GetProductReviews GetReviews { get; set; }
    public SendSlack NotifyTeam { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.CreateProduct,
            wf => new CreateProduct.Request { Name = wf.WorkflowInput.ProductName, Price = wf.WorkflowInput.Price, Category = wf.WorkflowInput.Category });

        _builder.AddTask(wf => wf.ValidateProduct,
            wf => new ValidateProduct.Request { ProductId = wf.CreateProduct.Output.ProductId, Price = wf.WorkflowInput.Price });

        _builder.AddTask(wf => wf.GenDescription,
            wf => new GenerateProductDescription.Request { ProductName = wf.WorkflowInput.ProductName, Category = wf.WorkflowInput.Category });

        _builder.AddTask(wf => wf.GetReviews,
            wf => new GetProductReviews.Request { ProductId = wf.CreateProduct.Output.ProductId });

        _builder.AddTask(wf => wf.NotifyTeam,
            wf => new SendSlack.Request { Channel = "#products", Message = $"New product: {wf.WorkflowInput.ProductName}" });

        _builder.SetOutput(wf => new ProductCatalogUpdateOutput
        {
            ProductId = wf.CreateProduct.Output.ProductId,
            Description = wf.GenDescription.Output.Description
        });
    }
}
