using ConductorSharp.Engine;
using ConductorSharp.Engine.Builders.Metadata;
using MediatR;

namespace ConductorSharpExample.Tasks.HR;

// ─── Task 82 ───
[OriginalName("HR_onboard_employee")]
public class OnboardEmployee : TaskRequestHandler<OnboardEmployee.Request, OnboardEmployee.Response>
{
    public class Request : IRequest<Response>
    {
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string StartDate { get; set; }
    }

    public class Response
    {
        public string EmployeeId { get; set; }
        public bool AccountCreated { get; set; }
        public string Email { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var id = $"EMP-{Random.Shared.Next(10000, 99999)}";
        return Task.FromResult(new Response { EmployeeId = id, AccountCreated = true, Email = $"{request.EmployeeName.ToLower().Replace(" ", ".")}@company.com" });
    }
}

// ─── Task 83 ───
[OriginalName("HR_offboard_employee")]
public class OffboardEmployee : TaskRequestHandler<OffboardEmployee.Request, OffboardEmployee.Response>
{
    public class Request : IRequest<Response>
    {
        public string EmployeeId { get; set; }
        public string LastDay { get; set; }
    }

    public class Response
    {
        public bool AccessRevoked { get; set; }
        public bool EquipmentReturned { get; set; }
        public bool FinalPayProcessed { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { AccessRevoked = true, EquipmentReturned = true, FinalPayProcessed = true });
    }
}

// ─── Task 84 ───
[OriginalName("HR_request_pto")]
public class RequestPto : TaskRequestHandler<RequestPto.Request, RequestPto.Response>
{
    public class Request : IRequest<Response>
    {
        public string EmployeeId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class Response
    {
        public string RequestId { get; set; }
        public string Status { get; set; }
        public int RemainingDays { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { RequestId = $"PTO-{Random.Shared.Next(10000, 99999)}", Status = "Pending", RemainingDays = 12 });
    }
}

// ─── Task 85 ───
[OriginalName("HR_approve_pto")]
public class ApprovePto : TaskRequestHandler<ApprovePto.Request, ApprovePto.Response>
{
    public class Request : IRequest<Response>
    {
        public string RequestId { get; set; }
        public string ApproverId { get; set; }
    }

    public class Response
    {
        public bool Approved { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Approved = true });
    }
}

// ─── Task 86 ───
[OriginalName("HR_process_payroll")]
public class ProcessPayroll : TaskRequestHandler<ProcessPayroll.Request, ProcessPayroll.Response>
{
    public class Request : IRequest<Response>
    {
        public string PayPeriod { get; set; }
        public string Department { get; set; }
    }

    public class Response
    {
        public int EmployeesProcessed { get; set; }
        public decimal TotalAmount { get; set; }
        public string BatchId { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { EmployeesProcessed = 45, TotalAmount = 225000m, BatchId = $"PAY-{Random.Shared.Next(10000, 99999)}" });
    }
}

// ─── Task 87 ───
[OriginalName("HR_get_employee")]
public class GetEmployee : TaskRequestHandler<GetEmployee.Request, GetEmployee.Response>
{
    public class Request : IRequest<Response>
    {
        public string EmployeeId { get; set; }
    }

    public class Response
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public string ManagerId { get; set; }
        public string Email { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Name = "John Doe", Department = "Engineering", ManagerId = "EMP-10001", Email = "john.doe@company.com" });
    }
}

// ─── Task 88 ───
[OriginalName("HR_provision_equipment")]
public class ProvisionEquipment : TaskRequestHandler<ProvisionEquipment.Request, ProvisionEquipment.Response>
{
    public class Request : IRequest<Response>
    {
        public string EmployeeId { get; set; }
        public string EquipmentType { get; set; }
    }

    public class Response
    {
        public string AssetId { get; set; }
        public bool Provisioned { get; set; }
        public string ShippingTrackingNumber { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response
        {
            AssetId = $"AST-{Random.Shared.Next(10000, 99999)}",
            Provisioned = true,
            ShippingTrackingNumber = $"1Z{Random.Shared.Next(100000000, 999999999)}"
        });
    }
}

// ─── Task 89 ───
[OriginalName("HR_setup_workspace")]
public class SetupWorkspace : TaskRequestHandler<SetupWorkspace.Request, SetupWorkspace.Response>
{
    public class Request : IRequest<Response>
    {
        public string EmployeeId { get; set; }
        public string Office { get; set; }
    }

    public class Response
    {
        public string DeskAssignment { get; set; }
        public bool BadgeCreated { get; set; }
        public bool ParkingAssigned { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { DeskAssignment = "Floor 3, Desk 42", BadgeCreated = true, ParkingAssigned = true });
    }
}
