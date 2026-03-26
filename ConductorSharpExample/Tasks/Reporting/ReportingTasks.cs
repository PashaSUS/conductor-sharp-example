using ConductorSharp.Engine;
using ConductorSharp.Engine.Builders.Metadata;
using MediatR;

namespace ConductorSharpExample.Tasks.Reporting;

// ─── Task 67 ───
[OriginalName("REPORT_generate_sales")]
public class GenerateSalesReport : TaskRequestHandler<GenerateSalesReport.Request, GenerateSalesReport.Response>
{
    public class Request : IRequest<Response>
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class Response
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public string ReportUrl { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { TotalRevenue = 125430.50m, TotalOrders = 842, ReportUrl = "/reports/sales-latest.pdf" });
    }
}

// ─── Task 68 ───
[OriginalName("REPORT_generate_inventory")]
public class GenerateInventoryReport : TaskRequestHandler<GenerateInventoryReport.Request, GenerateInventoryReport.Response>
{
    public class Request : IRequest<Response>
    {
        public string WarehouseId { get; set; }
    }

    public class Response
    {
        public int TotalSku { get; set; }
        public int LowStockItems { get; set; }
        public string ReportUrl { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { TotalSku = 3420, LowStockItems = 14, ReportUrl = "/reports/inventory-latest.pdf" });
    }
}

// ─── Task 69 ───
[OriginalName("REPORT_generate_customer")]
public class GenerateCustomerReport : TaskRequestHandler<GenerateCustomerReport.Request, GenerateCustomerReport.Response>
{
    public class Request : IRequest<Response>
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class Response
    {
        public int NewCustomers { get; set; }
        public int ChurnedCustomers { get; set; }
        public decimal RetentionRate { get; set; }
        public string ReportUrl { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { NewCustomers = 320, ChurnedCustomers = 18, RetentionRate = 94.4m, ReportUrl = "/reports/customer-latest.pdf" });
    }
}

// ─── Task 70 ───
[OriginalName("REPORT_aggregate_metrics")]
public class AggregateMetrics : TaskRequestHandler<AggregateMetrics.Request, AggregateMetrics.Response>
{
    public class Request : IRequest<Response>
    {
        public string MetricType { get; set; }
        public string Period { get; set; }
    }

    public class Response
    {
        public decimal Average { get; set; }
        public decimal Min { get; set; }
        public decimal Max { get; set; }
        public int DataPoints { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Average = 4250.30m, Min = 120.00m, Max = 18500.00m, DataPoints = 842 });
    }
}

// ─── Task 71 ───
[OriginalName("REPORT_export_csv")]
public class ExportCsv : TaskRequestHandler<ExportCsv.Request, ExportCsv.Response>
{
    public class Request : IRequest<Response>
    {
        public string ReportType { get; set; }
        public string DateRange { get; set; }
    }

    public class Response
    {
        public string FileUrl { get; set; }
        public int RowCount { get; set; }
        public long FileSizeBytes { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { FileUrl = "/exports/report-latest.csv", RowCount = 1250, FileSizeBytes = 45000 });
    }
}

// ─── Task 72 ───
[OriginalName("REPORT_send_scheduled")]
public class SendScheduledReport : TaskRequestHandler<SendScheduledReport.Request, SendScheduledReport.Response>
{
    public class Request : IRequest<Response>
    {
        public string ReportUrl { get; set; }
        public string RecipientEmail { get; set; }
        public string ReportName { get; set; }
    }

    public class Response
    {
        public bool Sent { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Sent = true });
    }
}

// ─── Task 73 ───
[OriginalName("REPORT_generate_financial")]
public class GenerateFinancialReport : TaskRequestHandler<GenerateFinancialReport.Request, GenerateFinancialReport.Response>
{
    public class Request : IRequest<Response>
    {
        public string FiscalQuarter { get; set; }
        public string Year { get; set; }
    }

    public class Response
    {
        public decimal Revenue { get; set; }
        public decimal Expenses { get; set; }
        public decimal NetIncome { get; set; }
        public string ReportUrl { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Revenue = 523000m, Expenses = 412000m, NetIncome = 111000m, ReportUrl = "/reports/financial-latest.pdf" });
    }
}
