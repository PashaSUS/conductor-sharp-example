using ConductorSharp.Engine;
using ConductorSharp.Engine.Builders.Metadata;
using MediatR;

namespace ConductorSharpExample.Tasks.Customer;

// ─── Task 1 ───
[OriginalName("CUSTOMER_get")]
public class GetCustomer : TaskRequestHandler<GetCustomer.Request, GetCustomer.Response>
{
    public class Request : IRequest<Response>
    {
        public int CustomerId { get; set; }
    }

    public class Response
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Tier { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response
        {
            Name = $"Customer-{request.CustomerId}",
            Email = $"customer{request.CustomerId}@example.com",
            Address = "123 Main St, Springfield",
            Phone = "+1-555-0100",
            Tier = "Gold"
        });
    }
}

// ─── Task 2 ───
[OriginalName("CUSTOMER_create")]
public class CreateCustomer : TaskRequestHandler<CreateCustomer.Request, CreateCustomer.Response>
{
    public class Request : IRequest<Response>
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class Response
    {
        public int CustomerId { get; set; }
        public bool Success { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { CustomerId = Random.Shared.Next(1000, 9999), Success = true });
    }
}

// ─── Task 3 ───
[OriginalName("CUSTOMER_update")]
public class UpdateCustomer : TaskRequestHandler<UpdateCustomer.Request, UpdateCustomer.Response>
{
    public class Request : IRequest<Response>
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class Response
    {
        public bool Updated { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Updated = true });
    }
}

// ─── Task 4 ───
[OriginalName("CUSTOMER_delete")]
public class DeleteCustomer : TaskRequestHandler<DeleteCustomer.Request, DeleteCustomer.Response>
{
    public class Request : IRequest<Response>
    {
        public int CustomerId { get; set; }
    }

    public class Response
    {
        public bool Deleted { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Deleted = true });
    }
}

// ─── Task 5 ───
[OriginalName("CUSTOMER_validate")]
public class ValidateCustomer : TaskRequestHandler<ValidateCustomer.Request, ValidateCustomer.Response>
{
    public class Request : IRequest<Response>
    {
        public int CustomerId { get; set; }
        public string Email { get; set; }
    }

    public class Response
    {
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { IsValid = true, ValidationMessage = "Customer validated" });
    }
}

// ─── Task 6 ───
[OriginalName("CUSTOMER_get_preferences")]
public class GetCustomerPreferences : TaskRequestHandler<GetCustomerPreferences.Request, GetCustomerPreferences.Response>
{
    public class Request : IRequest<Response>
    {
        public int CustomerId { get; set; }
    }

    public class Response
    {
        public string Language { get; set; }
        public string Currency { get; set; }
        public bool MarketingOptIn { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Language = "en", Currency = "USD", MarketingOptIn = true });
    }
}

// ─── Task 7 ───
[OriginalName("CUSTOMER_calculate_loyalty")]
public class CalculateLoyaltyScore : TaskRequestHandler<CalculateLoyaltyScore.Request, CalculateLoyaltyScore.Response>
{
    public class Request : IRequest<Response>
    {
        public int CustomerId { get; set; }
        public string Tier { get; set; }
    }

    public class Response
    {
        public int Score { get; set; }
        public string NextTier { get; set; }
        public int PointsToNextTier { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Score = 850, NextTier = "Platinum", PointsToNextTier = 150 });
    }
}

// ─── Task 8 ───
[OriginalName("CUSTOMER_merge_profiles")]
public class MergeCustomerProfiles : TaskRequestHandler<MergeCustomerProfiles.Request, MergeCustomerProfiles.Response>
{
    public class Request : IRequest<Response>
    {
        public int PrimaryCustomerId { get; set; }
        public int SecondaryCustomerId { get; set; }
    }

    public class Response
    {
        public int MergedCustomerId { get; set; }
        public bool Success { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { MergedCustomerId = request.PrimaryCustomerId, Success = true });
    }
}
