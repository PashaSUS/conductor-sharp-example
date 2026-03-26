using ConductorSharp.Engine.Builders;
using ConductorSharp.Engine.Builders.Metadata;
using ConductorSharp.Engine.Model;
using ConductorSharpExample.Tasks.Auth;
using ConductorSharpExample.Tasks.Customer;
using ConductorSharpExample.Tasks.Notification;
using ConductorSharpExample.Tasks.Product;

namespace ConductorSharpExample.Workflows;

// ═══════════════════════════════════════════════════════════════
// Workflow 27: User Authentication Flow
// ═══════════════════════════════════════════════════════════════
public class AuthFlowInput : WorkflowInput<AuthFlowOutput>
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string MfaCode { get; set; }
    public string IpAddress { get; set; }
}

public class AuthFlowOutput : WorkflowOutput
{
    public string Token { get; set; }
    public bool Authenticated { get; set; }
}

[OriginalName("WF_auth_flow")]
[WorkflowMetadata(OwnerEmail = "security@example.com")]
public class AuthFlowWorkflow : Workflow<AuthFlowWorkflow, AuthFlowInput, AuthFlowOutput>
{
    public AuthFlowWorkflow(
        WorkflowDefinitionBuilder<AuthFlowWorkflow, AuthFlowInput, AuthFlowOutput> builder
    ) : base(builder) { }

    public Login Login { get; set; }
    public VerifyMfa VerifyMfa { get; set; }
    public ValidateToken ValidateToken { get; set; }
    public AuditLog AuditLog { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.Login,
            wf => new Login.Request { Username = wf.WorkflowInput.Username, PasswordHash = wf.WorkflowInput.PasswordHash });

        _builder.AddTask(wf => wf.VerifyMfa,
            wf => new VerifyMfa.Request { UserId = wf.Login.Output.UserId, MfaCode = wf.WorkflowInput.MfaCode });

        _builder.AddTask(wf => wf.ValidateToken,
            wf => new ValidateToken.Request { Token = wf.Login.Output.Token });

        _builder.AddTask(wf => wf.AuditLog,
            wf => new AuditLog.Request { UserId = wf.Login.Output.UserId, Action = "login", Resource = "auth", IpAddress = wf.WorkflowInput.IpAddress });

        _builder.SetOutput(wf => new AuthFlowOutput
        {
            Token = wf.Login.Output.Token,
            Authenticated = wf.Login.Output.Authenticated
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 28: API Key Provisioning
// ═══════════════════════════════════════════════════════════════
public class ApiKeyProvisionInput : WorkflowInput<ApiKeyProvisionOutput>
{
    public string UserId { get; set; }
    public string Scope { get; set; }
}

public class ApiKeyProvisionOutput : WorkflowOutput
{
    public string ApiKey { get; set; }
    public string ExpiresAt { get; set; }
}

[OriginalName("WF_api_key_provision")]
[WorkflowMetadata(OwnerEmail = "security@example.com")]
public class ApiKeyProvisionWorkflow : Workflow<ApiKeyProvisionWorkflow, ApiKeyProvisionInput, ApiKeyProvisionOutput>
{
    public ApiKeyProvisionWorkflow(
        WorkflowDefinitionBuilder<ApiKeyProvisionWorkflow, ApiKeyProvisionInput, ApiKeyProvisionOutput> builder
    ) : base(builder) { }

    public CheckPermissions CheckPermissions { get; set; }
    public CreateApiKey CreateKey { get; set; }
    public AuditLog AuditLog { get; set; }
    public SendEmail NotifyUser { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.CheckPermissions,
            wf => new CheckPermissions.Request { UserId = wf.WorkflowInput.UserId, Resource = "api_keys", Action = "create" });

        _builder.AddTask(wf => wf.CreateKey,
            wf => new CreateApiKey.Request { UserId = wf.WorkflowInput.UserId, Scope = wf.WorkflowInput.Scope });

        _builder.AddTask(wf => wf.AuditLog,
            wf => new AuditLog.Request { UserId = wf.WorkflowInput.UserId, Action = "api_key_created", Resource = "api_keys", IpAddress = "0.0.0.0" });

        _builder.AddTask(wf => wf.NotifyUser,
            wf => new SendEmail.Request { To = "user@example.com", Subject = "API Key Created", Body = "Your new API key has been provisioned." });

        _builder.SetOutput(wf => new ApiKeyProvisionOutput
        {
            ApiKey = wf.CreateKey.Output.ApiKey,
            ExpiresAt = wf.CreateKey.Output.ExpiresAt
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 29: Customer Profile Merge
// ═══════════════════════════════════════════════════════════════
public class ProfileMergeInput : WorkflowInput<ProfileMergeOutput>
{
    public int PrimaryCustomerId { get; set; }
    public int SecondaryCustomerId { get; set; }
}

public class ProfileMergeOutput : WorkflowOutput
{
    public int MergedCustomerId { get; set; }
    public bool Success { get; set; }
}

[OriginalName("WF_profile_merge")]
[WorkflowMetadata(OwnerEmail = "customer-ops@example.com")]
public class ProfileMergeWorkflow : Workflow<ProfileMergeWorkflow, ProfileMergeInput, ProfileMergeOutput>
{
    public ProfileMergeWorkflow(
        WorkflowDefinitionBuilder<ProfileMergeWorkflow, ProfileMergeInput, ProfileMergeOutput> builder
    ) : base(builder) { }

    public GetCustomer GetPrimary { get; set; }
    public GetCustomer GetSecondary { get; set; }
    public ValidateCustomer ValidatePrimary { get; set; }
    public MergeCustomerProfiles Merge { get; set; }
    public DeleteCustomer DeleteSecondary { get; set; }
    public SendEmail NotifyCustomer { get; set; }
    public AuditLog AuditLog { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetPrimary,
            wf => new GetCustomer.Request { CustomerId = wf.WorkflowInput.PrimaryCustomerId });

        _builder.AddTask(wf => wf.GetSecondary,
            wf => new GetCustomer.Request { CustomerId = wf.WorkflowInput.SecondaryCustomerId });

        _builder.AddTask(wf => wf.ValidatePrimary,
            wf => new ValidateCustomer.Request { CustomerId = wf.WorkflowInput.PrimaryCustomerId, Email = wf.GetPrimary.Output.Email });

        _builder.AddTask(wf => wf.Merge,
            wf => new MergeCustomerProfiles.Request { PrimaryCustomerId = wf.WorkflowInput.PrimaryCustomerId, SecondaryCustomerId = wf.WorkflowInput.SecondaryCustomerId });

        _builder.AddTask(wf => wf.DeleteSecondary,
            wf => new DeleteCustomer.Request { CustomerId = wf.WorkflowInput.SecondaryCustomerId });

        _builder.AddTask(wf => wf.NotifyCustomer,
            wf => new SendEmail.Request { To = wf.GetPrimary.Output.Email, Subject = "Account Merged", Body = "Your accounts have been successfully merged." });

        _builder.AddTask(wf => wf.AuditLog,
            wf => new AuditLog.Request { UserId = $"{wf.WorkflowInput.PrimaryCustomerId}", Action = "profile_merged", Resource = "customer", IpAddress = "0.0.0.0" });

        _builder.SetOutput(wf => new ProfileMergeOutput
        {
            MergedCustomerId = wf.Merge.Output.MergedCustomerId,
            Success = wf.Merge.Output.Success
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 30: Product Price Update
// ═══════════════════════════════════════════════════════════════
public class PriceUpdateInput : WorkflowInput<PriceUpdateOutput>
{
    public string ProductId { get; set; }
    public decimal NewPrice { get; set; }
}

public class PriceUpdateOutput : WorkflowOutput
{
    public bool Updated { get; set; }
    public decimal OldPrice { get; set; }
}

[OriginalName("WF_price_update")]
[WorkflowMetadata(OwnerEmail = "products@example.com")]
public class PriceUpdateWorkflow : Workflow<PriceUpdateWorkflow, PriceUpdateInput, PriceUpdateOutput>
{
    public PriceUpdateWorkflow(
        WorkflowDefinitionBuilder<PriceUpdateWorkflow, PriceUpdateInput, PriceUpdateOutput> builder
    ) : base(builder) { }

    public GetProduct GetProduct { get; set; }
    public ValidateProduct Validate { get; set; }
    public UpdateProductPrice UpdatePrice { get; set; }
    public SendSlack NotifyTeam { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.GetProduct,
            wf => new GetProduct.Request { ProductId = wf.WorkflowInput.ProductId });

        _builder.AddTask(wf => wf.Validate,
            wf => new ValidateProduct.Request { ProductId = wf.WorkflowInput.ProductId, Price = wf.WorkflowInput.NewPrice });

        _builder.AddTask(wf => wf.UpdatePrice,
            wf => new UpdateProductPrice.Request { ProductId = wf.WorkflowInput.ProductId, NewPrice = wf.WorkflowInput.NewPrice });

        _builder.AddTask(wf => wf.NotifyTeam,
            wf => new SendSlack.Request { Channel = "#products", Message = $"Price updated: {wf.GetProduct.Output.Name} from ${wf.UpdatePrice.Output.OldPrice} to ${wf.WorkflowInput.NewPrice}" });

        _builder.SetOutput(wf => new PriceUpdateOutput
        {
            Updated = wf.UpdatePrice.Output.Updated,
            OldPrice = wf.UpdatePrice.Output.OldPrice
        });
    }
}
