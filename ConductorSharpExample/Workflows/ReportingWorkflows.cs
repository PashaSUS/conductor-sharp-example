using ConductorSharp.Engine.Builders;
using ConductorSharp.Engine.Builders.Metadata;
using ConductorSharp.Engine.Model;
using ConductorSharpExample.Tasks.Reporting;
using ConductorSharpExample.Tasks.Notification;
using ConductorSharpExample.Tasks.DataProcessing;

namespace ConductorSharpExample.Workflows;

// ═══════════════════════════════════════════════════════════════
// Workflow 18: Monthly Sales Report
// ═══════════════════════════════════════════════════════════════
public class MonthlySalesReportInput : WorkflowInput<MonthlySalesReportOutput>
{
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string RecipientEmail { get; set; }
}

public class MonthlySalesReportOutput : WorkflowOutput
{
    public string ReportUrl { get; set; }
    public string CsvUrl { get; set; }
}

[OriginalName("WF_monthly_sales_report")]
[WorkflowMetadata(OwnerEmail = "reporting@example.com")]
public class MonthlySalesReportWorkflow : Workflow<MonthlySalesReportWorkflow, MonthlySalesReportInput, MonthlySalesReportOutput>
{
    public MonthlySalesReportWorkflow(
        WorkflowDefinitionBuilder<MonthlySalesReportWorkflow, MonthlySalesReportInput, MonthlySalesReportOutput> builder
    ) : base(builder) { }

    public GenerateSalesReport GenSalesReport { get; set; }
    public AggregateMetrics AggregateMetrics { get; set; }
    public ExportCsv ExportCsv { get; set; }
    public SendScheduledReport SendReport { get; set; }
    public SendSlack NotifyTeam { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GenSalesReport,
            wf => new GenerateSalesReport.Request { StartDate = wf.WorkflowInput.StartDate, EndDate = wf.WorkflowInput.EndDate });

        _builder.AddTask(wf => wf.AggregateMetrics,
            wf => new AggregateMetrics.Request { MetricType = "revenue", Period = "monthly" });

        _builder.AddTask(wf => wf.ExportCsv,
            wf => new ExportCsv.Request { ReportType = "sales", DateRange = $"{wf.WorkflowInput.StartDate}-{wf.WorkflowInput.EndDate}" });

        _builder.AddTask(wf => wf.SendReport,
            wf => new SendScheduledReport.Request { ReportUrl = wf.GenSalesReport.Output.ReportUrl, RecipientEmail = wf.WorkflowInput.RecipientEmail, ReportName = "Monthly Sales Report" });

        _builder.AddTask(wf => wf.NotifyTeam,
            wf => new SendSlack.Request { Channel = "#reports", Message = $"Monthly sales report: ${wf.GenSalesReport.Output.TotalRevenue} revenue, {wf.GenSalesReport.Output.TotalOrders} orders" });

        _builder.SetOutput(wf => new MonthlySalesReportOutput
        {
            ReportUrl = wf.GenSalesReport.Output.ReportUrl,
            CsvUrl = wf.ExportCsv.Output.FileUrl
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 19: Quarterly Financial Report
// ═══════════════════════════════════════════════════════════════
public class QuarterlyFinancialInput : WorkflowInput<QuarterlyFinancialOutput>
{
    public string FiscalQuarter { get; set; }
    public string Year { get; set; }
    public string RecipientEmail { get; set; }
}

public class QuarterlyFinancialOutput : WorkflowOutput
{
    public string ReportUrl { get; set; }
    public decimal NetIncome { get; set; }
}

[OriginalName("WF_quarterly_financial")]
[WorkflowMetadata(OwnerEmail = "finance@example.com")]
public class QuarterlyFinancialWorkflow : Workflow<QuarterlyFinancialWorkflow, QuarterlyFinancialInput, QuarterlyFinancialOutput>
{
    public QuarterlyFinancialWorkflow(
        WorkflowDefinitionBuilder<QuarterlyFinancialWorkflow, QuarterlyFinancialInput, QuarterlyFinancialOutput> builder
    ) : base(builder) { }

    public GenerateFinancialReport GenFinancial { get; set; }
    public GenerateSalesReport GenSales { get; set; }
    public GenerateCustomerReport GenCustomer { get; set; }
    public ExportCsv ExportData { get; set; }
    public SendScheduledReport SendReport { get; set; }
    public SendEmail SendExecutiveSummary { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GenFinancial,
            wf => new GenerateFinancialReport.Request { FiscalQuarter = wf.WorkflowInput.FiscalQuarter, Year = wf.WorkflowInput.Year });

        _builder.AddTask(wf => wf.GenSales,
            wf => new GenerateSalesReport.Request { StartDate = $"{wf.WorkflowInput.Year}-01-01", EndDate = $"{wf.WorkflowInput.Year}-03-31" });

        _builder.AddTask(wf => wf.GenCustomer,
            wf => new GenerateCustomerReport.Request { StartDate = $"{wf.WorkflowInput.Year}-01-01", EndDate = $"{wf.WorkflowInput.Year}-03-31" });

        _builder.AddTask(wf => wf.ExportData,
            wf => new ExportCsv.Request { ReportType = "financial", DateRange = $"{wf.WorkflowInput.Year}-{wf.WorkflowInput.FiscalQuarter}" });

        _builder.AddTask(wf => wf.SendReport,
            wf => new SendScheduledReport.Request { ReportUrl = wf.GenFinancial.Output.ReportUrl, RecipientEmail = wf.WorkflowInput.RecipientEmail, ReportName = "Quarterly Financial Report" });

        _builder.AddTask(wf => wf.SendExecutiveSummary,
            wf => new SendEmail.Request { To = wf.WorkflowInput.RecipientEmail, Subject = $"Q{wf.WorkflowInput.FiscalQuarter} Financial Summary", Body = $"Net Income: ${wf.GenFinancial.Output.NetIncome}" });

        _builder.SetOutput(wf => new QuarterlyFinancialOutput
        {
            ReportUrl = wf.GenFinancial.Output.ReportUrl,
            NetIncome = wf.GenFinancial.Output.NetIncome
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 20: Data Pipeline (ETL)
// ═══════════════════════════════════════════════════════════════
public class DataPipelineInput : WorkflowInput<DataPipelineOutput>
{
    public string SourceUrl { get; set; }
    public string TargetTable { get; set; }
    public string SchemaName { get; set; }
}

public class DataPipelineOutput : WorkflowOutput
{
    public int RecordsProcessed { get; set; }
    public string JobId { get; set; }
}

[OriginalName("WF_data_pipeline")]
[WorkflowMetadata(OwnerEmail = "data@example.com")]
public class DataPipelineWorkflow : Workflow<DataPipelineWorkflow, DataPipelineInput, DataPipelineOutput>
{
    public DataPipelineWorkflow(
        WorkflowDefinitionBuilder<DataPipelineWorkflow, DataPipelineInput, DataPipelineOutput> builder
    ) : base(builder) { }

    public ValidateSchema ValidateSchema { get; set; }
    public TransformJson Transform { get; set; }
    public DeduplicateRecords Deduplicate { get; set; }
    public EnrichData Enrich { get; set; }
    public BatchImport Import { get; set; }
    public SendSlack NotifyComplete { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.ValidateSchema,
            wf => new ValidateSchema.Request { Data = wf.WorkflowInput.SourceUrl, SchemaName = wf.WorkflowInput.SchemaName });

        _builder.AddTask(wf => wf.Transform,
            wf => new TransformJson.Request { InputData = wf.WorkflowInput.SourceUrl, TransformSpec = "default" });

        _builder.AddTask(wf => wf.Deduplicate,
            wf => new DeduplicateRecords.Request { DatasetId = wf.WorkflowInput.SourceUrl, KeyField = "id" });

        _builder.AddTask(wf => wf.Enrich,
            wf => new EnrichData.Request { RecordId = wf.WorkflowInput.SourceUrl, DataSource = "external_api" });

        _builder.AddTask(wf => wf.Import,
            wf => new BatchImport.Request { SourceUrl = wf.WorkflowInput.SourceUrl, TargetTable = wf.WorkflowInput.TargetTable, Format = "json" });

        _builder.AddTask(wf => wf.NotifyComplete,
            wf => new SendSlack.Request { Channel = "#data-ops", Message = $"ETL complete: {wf.Import.Output.RecordsImported} records imported to {wf.WorkflowInput.TargetTable}" });

        _builder.SetOutput(wf => new DataPipelineOutput
        {
            RecordsProcessed = wf.Import.Output.RecordsImported,
            JobId = wf.Import.Output.JobId
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 21: Data Archival
// ═══════════════════════════════════════════════════════════════
public class DataArchivalInput : WorkflowInput<DataArchivalOutput>
{
    public string DatasetId { get; set; }
    public string RetentionPolicy { get; set; }
}

public class DataArchivalOutput : WorkflowOutput
{
    public string ArchiveLocation { get; set; }
    public bool Success { get; set; }
}

[OriginalName("WF_data_archival")]
[WorkflowMetadata(OwnerEmail = "data@example.com")]
public class DataArchivalWorkflow : Workflow<DataArchivalWorkflow, DataArchivalInput, DataArchivalOutput>
{
    public DataArchivalWorkflow(
        WorkflowDefinitionBuilder<DataArchivalWorkflow, DataArchivalInput, DataArchivalOutput> builder
    ) : base(builder) { }

    public BatchExport Export { get; set; }
    public ComputeHash ComputeHash { get; set; }
    public ArchiveData Archive { get; set; }
    public SendSlack NotifyArchive { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.Export,
            wf => new BatchExport.Request { SourceTable = wf.WorkflowInput.DatasetId, Format = "parquet", FilterCriteria = "all" });

        _builder.AddTask(wf => wf.ComputeHash,
            wf => new ComputeHash.Request { Data = wf.Export.Output.ExportUrl, Algorithm = "SHA256" });

        _builder.AddTask(wf => wf.Archive,
            wf => new ArchiveData.Request { DatasetId = wf.WorkflowInput.DatasetId, RetentionPolicy = wf.WorkflowInput.RetentionPolicy });

        _builder.AddTask(wf => wf.NotifyArchive,
            wf => new SendSlack.Request { Channel = "#data-ops", Message = $"Archived {wf.WorkflowInput.DatasetId} to {wf.Archive.Output.ArchiveLocation}" });

        _builder.SetOutput(wf => new DataArchivalOutput
        {
            ArchiveLocation = wf.Archive.Output.ArchiveLocation,
            Success = wf.Archive.Output.Archived
        });
    }
}
