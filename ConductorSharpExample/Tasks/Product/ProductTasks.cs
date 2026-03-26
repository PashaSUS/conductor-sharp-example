using ConductorSharp.Engine;
using ConductorSharp.Engine.Builders.Metadata;
using MediatR;

namespace ConductorSharpExample.Tasks.Product;

// ─── Task 51 ───
[OriginalName("PRODUCT_get")]
public class GetProduct : TaskRequestHandler<GetProduct.Request, GetProduct.Response>
{
    public class Request : IRequest<Response>
    {
        public string ProductId { get; set; }
    }

    public class Response
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public decimal Weight { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Name = $"Product-{request.ProductId}", Price = 49.99m, Category = "Electronics", Weight = 1.5m });
    }
}

// ─── Task 52 ───
[OriginalName("PRODUCT_create")]
public class CreateProduct : TaskRequestHandler<CreateProduct.Request, CreateProduct.Response>
{
    public class Request : IRequest<Response>
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
    }

    public class Response
    {
        public string ProductId { get; set; }
        public bool Created { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { ProductId = $"PROD-{Random.Shared.Next(10000, 99999)}", Created = true });
    }
}

// ─── Task 53 ───
[OriginalName("PRODUCT_update_price")]
public class UpdateProductPrice : TaskRequestHandler<UpdateProductPrice.Request, UpdateProductPrice.Response>
{
    public class Request : IRequest<Response>
    {
        public string ProductId { get; set; }
        public decimal NewPrice { get; set; }
    }

    public class Response
    {
        public bool Updated { get; set; }
        public decimal OldPrice { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Updated = true, OldPrice = 49.99m });
    }
}

// ─── Task 54 ───
[OriginalName("PRODUCT_search")]
public class SearchProducts : TaskRequestHandler<SearchProducts.Request, SearchProducts.Response>
{
    public class Request : IRequest<Response>
    {
        public string Query { get; set; }
        public string Category { get; set; }
    }

    public class Response
    {
        public int TotalResults { get; set; }
        public string TopResultId { get; set; }
        public string TopResultName { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { TotalResults = 23, TopResultId = "PROD-10001", TopResultName = "Widget Pro" });
    }
}

// ─── Task 55 ───
[OriginalName("PRODUCT_validate")]
public class ValidateProduct : TaskRequestHandler<ValidateProduct.Request, ValidateProduct.Response>
{
    public class Request : IRequest<Response>
    {
        public string ProductId { get; set; }
        public decimal Price { get; set; }
    }

    public class Response
    {
        public bool IsValid { get; set; }
        public string ValidationMessage { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var valid = request.Price > 0;
        return Task.FromResult(new Response { IsValid = valid, ValidationMessage = valid ? "OK" : "Price must be positive" });
    }
}

// ─── Task 56 ───
[OriginalName("PRODUCT_calculate_discount")]
public class CalculateProductDiscount : TaskRequestHandler<CalculateProductDiscount.Request, CalculateProductDiscount.Response>
{
    public class Request : IRequest<Response>
    {
        public string ProductId { get; set; }
        public string CustomerTier { get; set; }
    }

    public class Response
    {
        public decimal DiscountPercent { get; set; }
        public decimal FinalPrice { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { DiscountPercent = 15m, FinalPrice = 42.49m });
    }
}

// ─── Task 57 ───
[OriginalName("PRODUCT_get_reviews")]
public class GetProductReviews : TaskRequestHandler<GetProductReviews.Request, GetProductReviews.Response>
{
    public class Request : IRequest<Response>
    {
        public string ProductId { get; set; }
    }

    public class Response
    {
        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { AverageRating = 4.5m, TotalReviews = 128 });
    }
}

// ─── Task 58 ───
[OriginalName("PRODUCT_generate_description")]
public class GenerateProductDescription : TaskRequestHandler<GenerateProductDescription.Request, GenerateProductDescription.Response>
{
    public class Request : IRequest<Response>
    {
        public string ProductName { get; set; }
        public string Category { get; set; }
    }

    public class Response
    {
        public string Description { get; set; }
        public string ShortDescription { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response
        {
            Description = $"Premium {request.Category} product: {request.ProductName}. Built with quality materials.",
            ShortDescription = $"High-quality {request.ProductName}"
        });
    }
}
