using System.Reflection;
using ConductorSharp.Engine;
using ConductorSharp.Engine.Extensions;
using ConductorSharp.Patterns.Extensions;
using ConductorSharpExample.Workflows;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var conductorSection = builder.Configuration.GetSection("Conductor");
var baseUrl = conductorSection["BaseUrl"] ?? "http://localhost:8080";
var maxWorkers = int.TryParse(conductorSection["MaxConcurrentWorkers"], out var mw) ? mw : 10;
var sleepInterval = int.TryParse(conductorSection["SleepInterval"], out var si) ? si : 500;
var longPollInterval = int.TryParse(conductorSection["LongPollInterval"], out var lp) ? lp : 100;
var domain = conductorSection["Domain"] ?? null;

builder
    .Services.AddConductorSharp(baseUrl: baseUrl)
    .AddExecutionManager(
        maxConcurrentWorkers: maxWorkers,
        sleepInterval: sleepInterval,
        longPollInterval: longPollInterval,
        domain: domain,
        typeof(Program).Assembly
    )
    .AddCSharpLambdaTasks()
    .AddPipelines(pipelines =>
    {
        pipelines.AddRequestResponseLogging();
        pipelines.AddValidation();
    });

// Register all task handlers via reflection
var assembly = typeof(Program).Assembly;
var registerMethods = typeof(TaskRegistrationExtensions)
    .GetMethods(BindingFlags.Static | BindingFlags.Public)
    .Where(m => m.Name == "RegisterWorkerTask" && m.IsGenericMethodDefinition)
    .ToArray();

var registerMethod = registerMethods.First(m => m.GetParameters().Length == 2);

var taskHandlerTypes = assembly
    .GetTypes()
    .Where(t => t is { IsAbstract: false, IsClass: true })
    .Where(t =>
        t.BaseType is { IsGenericType: true }
        && t.BaseType.GetGenericTypeDefinition() == typeof(TaskRequestHandler<,>)
    );

// Find the TaskDefinitionOptions type from the second parameter
var optionsType = registerMethod.GetParameters()[1].ParameterType.GenericTypeArguments[0];

// Build a no-op Action<TaskDefinitionOptions> — domain is set globally via AddExecutionManager
var optionsParam = System.Linq.Expressions.Expression.Parameter(optionsType, "opts");
var noOpLambda = System
    .Linq.Expressions.Expression.Lambda(
        typeof(Action<>).MakeGenericType(optionsType),
        System.Linq.Expressions.Expression.Empty(),
        optionsParam
    )
    .Compile();

foreach (var taskType in taskHandlerTypes)
{
    var generic = registerMethod.MakeGenericMethod(taskType);
    generic.Invoke(null, new object[] { builder.Services, noOpLambda });
}

// Register all workflows
builder.Services.RegisterWorkflow<OrderProcessingWorkflow>();
builder.Services.RegisterWorkflow<CustomerOnboardingWorkflow>();
builder.Services.RegisterWorkflow<OrderCancellationWorkflow>();
builder.Services.RegisterWorkflow<ShipOrderWorkflow>();
builder.Services.RegisterWorkflow<ReturnProcessingWorkflow>();
builder.Services.RegisterWorkflow<TrackingNotificationWorkflow>();
builder.Services.RegisterWorkflow<PaymentProcessingWorkflow>();
builder.Services.RegisterWorkflow<SubscriptionBillingWorkflow>();
builder.Services.RegisterWorkflow<RefundProcessingWorkflow>();
builder.Services.RegisterWorkflow<InventoryReplenishWorkflow>();
builder.Services.RegisterWorkflow<WarehouseAuditWorkflow>();
builder.Services.RegisterWorkflow<StockTransferWorkflow>();
builder.Services.RegisterWorkflow<ProductCatalogUpdateWorkflow>();
builder.Services.RegisterWorkflow<EmployeeOnboardingWorkflow>();
builder.Services.RegisterWorkflow<EmployeeOffboardingWorkflow>();
builder.Services.RegisterWorkflow<PtoRequestWorkflow>();
builder.Services.RegisterWorkflow<PayrollProcessingWorkflow>();
builder.Services.RegisterWorkflow<MonthlySalesReportWorkflow>();
builder.Services.RegisterWorkflow<QuarterlyFinancialWorkflow>();
builder.Services.RegisterWorkflow<DataPipelineWorkflow>();
builder.Services.RegisterWorkflow<DataArchivalWorkflow>();
builder.Services.RegisterWorkflow<FileUploadPipelineWorkflow>();
builder.Services.RegisterWorkflow<ImageProcessingWorkflow>();
builder.Services.RegisterWorkflow<CsvImportWorkflow>();
builder.Services.RegisterWorkflow<DocumentProcessingWorkflow>();
builder.Services.RegisterWorkflow<FileBackupWorkflow>();
builder.Services.RegisterWorkflow<AuthFlowWorkflow>();
builder.Services.RegisterWorkflow<ApiKeyProvisionWorkflow>();
builder.Services.RegisterWorkflow<ProfileMergeWorkflow>();
builder.Services.RegisterWorkflow<PriceUpdateWorkflow>();
builder.Services.RegisterWorkflow<LambdaPipelineWorkflow>();
builder.Services.RegisterWorkflow<CSharpLambdaPipelineWorkflow>();

var host = builder.Build();
await host.RunAsync();
