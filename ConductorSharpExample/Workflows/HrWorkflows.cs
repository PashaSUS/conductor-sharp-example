using ConductorSharp.Engine.Builders;
using ConductorSharp.Engine.Builders.Metadata;
using ConductorSharp.Engine.Model;
using ConductorSharpExample.Tasks.HR;
using ConductorSharpExample.Tasks.Auth;
using ConductorSharpExample.Tasks.Notification;

namespace ConductorSharpExample.Workflows;

// ═══════════════════════════════════════════════════════════════
// Workflow 14: Employee Onboarding
// ═══════════════════════════════════════════════════════════════
public class EmployeeOnboardingInput : WorkflowInput<EmployeeOnboardingOutput>
{
    public string EmployeeName { get; set; }
    public string Department { get; set; }
    public string StartDate { get; set; }
    public string Office { get; set; }
}

public class EmployeeOnboardingOutput : WorkflowOutput
{
    public string EmployeeId { get; set; }
    public string Email { get; set; }
    public string DeskAssignment { get; set; }
}

[OriginalName("WF_employee_onboarding")]
[WorkflowMetadata(OwnerEmail = "hr@example.com")]
public class EmployeeOnboardingWorkflow : Workflow<EmployeeOnboardingWorkflow, EmployeeOnboardingInput, EmployeeOnboardingOutput>
{
    public EmployeeOnboardingWorkflow(
        WorkflowDefinitionBuilder<EmployeeOnboardingWorkflow, EmployeeOnboardingInput, EmployeeOnboardingOutput> builder
    ) : base(builder) { }

    public OnboardEmployee Onboard { get; set; }
    public CreateApiKey CreateApiKey { get; set; }
    public ProvisionEquipment ProvisionLaptop { get; set; }
    public SetupWorkspace SetupWorkspace { get; set; }
    public SendEmail SendWelcomeEmail { get; set; }
    public SendSlack NotifyTeam { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.Onboard,
            wf => new OnboardEmployee.Request { EmployeeName = wf.WorkflowInput.EmployeeName, Department = wf.WorkflowInput.Department, StartDate = wf.WorkflowInput.StartDate });

        _builder.AddTask(wf => wf.CreateApiKey,
            wf => new CreateApiKey.Request { UserId = wf.Onboard.Output.EmployeeId, Scope = "employee" });

        _builder.AddTask(wf => wf.ProvisionLaptop,
            wf => new ProvisionEquipment.Request { EmployeeId = wf.Onboard.Output.EmployeeId, EquipmentType = "Laptop" });

        _builder.AddTask(wf => wf.SetupWorkspace,
            wf => new SetupWorkspace.Request { EmployeeId = wf.Onboard.Output.EmployeeId, Office = wf.WorkflowInput.Office });

        _builder.AddTask(wf => wf.SendWelcomeEmail,
            wf => new SendEmail.Request { To = wf.Onboard.Output.Email, Subject = "Welcome to the team!", Body = $"Hi {wf.WorkflowInput.EmployeeName}, your desk is {wf.SetupWorkspace.Output.DeskAssignment}" });

        _builder.AddTask(wf => wf.NotifyTeam,
            wf => new SendSlack.Request { Channel = $"#{wf.WorkflowInput.Department}", Message = $"Welcome {wf.WorkflowInput.EmployeeName} to the team! Starting {wf.WorkflowInput.StartDate}" });

        _builder.SetOutput(wf => new EmployeeOnboardingOutput
        {
            EmployeeId = wf.Onboard.Output.EmployeeId,
            Email = wf.Onboard.Output.Email,
            DeskAssignment = wf.SetupWorkspace.Output.DeskAssignment
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 15: Employee Offboarding
// ═══════════════════════════════════════════════════════════════
public class EmployeeOffboardingInput : WorkflowInput<EmployeeOffboardingOutput>
{
    public string EmployeeId { get; set; }
    public string LastDay { get; set; }
}

public class EmployeeOffboardingOutput : WorkflowOutput
{
    public bool Complete { get; set; }
}

[OriginalName("WF_employee_offboarding")]
[WorkflowMetadata(OwnerEmail = "hr@example.com")]
public class EmployeeOffboardingWorkflow : Workflow<EmployeeOffboardingWorkflow, EmployeeOffboardingInput, EmployeeOffboardingOutput>
{
    public EmployeeOffboardingWorkflow(
        WorkflowDefinitionBuilder<EmployeeOffboardingWorkflow, EmployeeOffboardingInput, EmployeeOffboardingOutput> builder
    ) : base(builder) { }

    public GetEmployee GetEmployee { get; set; }
    public RevokeAccess RevokeAccess { get; set; }
    public OffboardEmployee Offboard { get; set; }
    public SendEmail SendFarewellEmail { get; set; }
    public AuditLog AuditLog { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetEmployee,
            wf => new GetEmployee.Request { EmployeeId = wf.WorkflowInput.EmployeeId });

        _builder.AddTask(wf => wf.RevokeAccess,
            wf => new RevokeAccess.Request { UserId = wf.WorkflowInput.EmployeeId, Reason = "Offboarding" });

        _builder.AddTask(wf => wf.Offboard,
            wf => new OffboardEmployee.Request { EmployeeId = wf.WorkflowInput.EmployeeId, LastDay = wf.WorkflowInput.LastDay });

        _builder.AddTask(wf => wf.SendFarewellEmail,
            wf => new SendEmail.Request { To = wf.GetEmployee.Output.Email, Subject = "Farewell", Body = "We wish you all the best in your future endeavors!" });

        _builder.AddTask(wf => wf.AuditLog,
            wf => new AuditLog.Request { UserId = wf.WorkflowInput.EmployeeId, Action = "employee_offboarded", Resource = "hr", IpAddress = "0.0.0.0" });

        _builder.SetOutput(wf => new EmployeeOffboardingOutput
        {
            Complete = wf.Offboard.Output.AccessRevoked
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 16: PTO Request
// ═══════════════════════════════════════════════════════════════
public class PtoRequestInput : WorkflowInput<PtoRequestOutput>
{
    public string EmployeeId { get; set; }
    public string StartDate { get; set; }
    public string EndDate { get; set; }
    public string ApproverId { get; set; }
}

public class PtoRequestOutput : WorkflowOutput
{
    public string RequestId { get; set; }
    public bool Approved { get; set; }
}

[OriginalName("WF_pto_request")]
[WorkflowMetadata(OwnerEmail = "hr@example.com")]
public class PtoRequestWorkflow : Workflow<PtoRequestWorkflow, PtoRequestInput, PtoRequestOutput>
{
    public PtoRequestWorkflow(
        WorkflowDefinitionBuilder<PtoRequestWorkflow, PtoRequestInput, PtoRequestOutput> builder
    ) : base(builder) { }

    public GetEmployee GetEmployee { get; set; }
    public RequestPto RequestPto { get; set; }
    public ApprovePto ApprovePto { get; set; }
    public SendEmail NotifyEmployee { get; set; }
    public SendSlack NotifyManager { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetEmployee,
            wf => new GetEmployee.Request { EmployeeId = wf.WorkflowInput.EmployeeId });

        _builder.AddTask(wf => wf.RequestPto,
            wf => new RequestPto.Request { EmployeeId = wf.WorkflowInput.EmployeeId, StartDate = wf.WorkflowInput.StartDate, EndDate = wf.WorkflowInput.EndDate });

        _builder.AddTask(wf => wf.ApprovePto,
            wf => new ApprovePto.Request { RequestId = wf.RequestPto.Output.RequestId, ApproverId = wf.WorkflowInput.ApproverId });

        _builder.AddTask(wf => wf.NotifyEmployee,
            wf => new SendEmail.Request { To = wf.GetEmployee.Output.Email, Subject = "PTO Approved", Body = $"Your PTO from {wf.WorkflowInput.StartDate} to {wf.WorkflowInput.EndDate} has been approved." });

        _builder.AddTask(wf => wf.NotifyManager,
            wf => new SendSlack.Request { Channel = "#hr-approvals", Message = $"PTO approved for {wf.GetEmployee.Output.Name}" });

        _builder.SetOutput(wf => new PtoRequestOutput
        {
            RequestId = wf.RequestPto.Output.RequestId,
            Approved = wf.ApprovePto.Output.Approved
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 17: Payroll Processing
// ═══════════════════════════════════════════════════════════════
public class PayrollProcessingInput : WorkflowInput<PayrollProcessingOutput>
{
    public string PayPeriod { get; set; }
    public string Department { get; set; }
    public string RecipientEmail { get; set; }
}

public class PayrollProcessingOutput : WorkflowOutput
{
    public string BatchId { get; set; }
    public int EmployeesProcessed { get; set; }
}

[OriginalName("WF_payroll_processing")]
[WorkflowMetadata(OwnerEmail = "hr@example.com")]
public class PayrollProcessingWorkflow : Workflow<PayrollProcessingWorkflow, PayrollProcessingInput, PayrollProcessingOutput>
{
    public PayrollProcessingWorkflow(
        WorkflowDefinitionBuilder<PayrollProcessingWorkflow, PayrollProcessingInput, PayrollProcessingOutput> builder
    ) : base(builder) { }

    public ProcessPayroll ProcessPayroll { get; set; }
    public SendEmail SendPayrollReport { get; set; }
    public SendSlack NotifyFinance { get; set; }
    public AuditLog AuditLog { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.ProcessPayroll,
            wf => new ProcessPayroll.Request { PayPeriod = wf.WorkflowInput.PayPeriod, Department = wf.WorkflowInput.Department });

        _builder.AddTask(wf => wf.SendPayrollReport,
            wf => new SendEmail.Request { To = wf.WorkflowInput.RecipientEmail, Subject = $"Payroll Complete - {wf.WorkflowInput.PayPeriod}", Body = $"Processed {wf.ProcessPayroll.Output.EmployeesProcessed} employees. Total: ${wf.ProcessPayroll.Output.TotalAmount}" });

        _builder.AddTask(wf => wf.NotifyFinance,
            wf => new SendSlack.Request { Channel = "#finance", Message = $"Payroll batch {wf.ProcessPayroll.Output.BatchId} complete" });

        _builder.AddTask(wf => wf.AuditLog,
            wf => new AuditLog.Request { UserId = "system", Action = "payroll_processed", Resource = wf.ProcessPayroll.Output.BatchId, IpAddress = "0.0.0.0" });

        _builder.SetOutput(wf => new PayrollProcessingOutput
        {
            BatchId = wf.ProcessPayroll.Output.BatchId,
            EmployeesProcessed = wf.ProcessPayroll.Output.EmployeesProcessed
        });
    }
}
