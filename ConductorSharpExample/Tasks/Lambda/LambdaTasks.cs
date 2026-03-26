using ConductorSharp.Engine;
using ConductorSharp.Engine.Builders.Metadata;
using MediatR;

namespace ConductorSharpExample.Tasks.Lambda;

// ─── Lambda 1: String Reverse ───
[OriginalName("LAMBDA_string_reverse")]
public class StringReverse : TaskRequestHandler<StringReverse.Request, StringReverse.Response>
{
    public class Request : IRequest<Response>
    {
        public string Input { get; set; }
    }

    public class Response
    {
        public string Result { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response { Result = new string(request.Input?.Reverse().ToArray() ?? []) });
}

// ─── Lambda 2: String Upper ───
[OriginalName("LAMBDA_string_upper")]
public class StringUpper : TaskRequestHandler<StringUpper.Request, StringUpper.Response>
{
    public class Request : IRequest<Response>
    {
        public string Input { get; set; }
    }

    public class Response
    {
        public string Result { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response { Result = request.Input?.ToUpperInvariant() ?? "" });
}

// ─── Lambda 3: String Length ───
[OriginalName("LAMBDA_string_length")]
public class StringLength : TaskRequestHandler<StringLength.Request, StringLength.Response>
{
    public class Request : IRequest<Response>
    {
        public string Input { get; set; }
    }

    public class Response
    {
        public int Length { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response { Length = request.Input?.Length ?? 0 });
}

// ─── Lambda 4: Hash MD5 ───
[OriginalName("LAMBDA_hash_md5")]
public class HashMd5 : TaskRequestHandler<HashMd5.Request, HashMd5.Response>
{
    public class Request : IRequest<Response>
    {
        public string Input { get; set; }
    }

    public class Response
    {
        public string Hash { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(request.Input ?? "");
        var hash = System.Security.Cryptography.SHA256.HashData(bytes);
        return Task.FromResult(new Response { Hash = Convert.ToHexString(hash).ToLowerInvariant() });
    }
}

// ─── Lambda 5: Base64 Encode ───
[OriginalName("LAMBDA_base64_encode")]
public class Base64Encode : TaskRequestHandler<Base64Encode.Request, Base64Encode.Response>
{
    public class Request : IRequest<Response>
    {
        public string Input { get; set; }
    }

    public class Response
    {
        public string Encoded { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response { Encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.Input ?? "")) });
}

// ─── Lambda 6: Base64 Decode ───
[OriginalName("LAMBDA_base64_decode")]
public class Base64Decode : TaskRequestHandler<Base64Decode.Request, Base64Decode.Response>
{
    public class Request : IRequest<Response>
    {
        public string Encoded { get; set; }
    }

    public class Response
    {
        public string Decoded { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response { Decoded = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(request.Encoded ?? "")) });
}

// ─── Lambda 7: Word Count ───
[OriginalName("LAMBDA_word_count")]
public class WordCount : TaskRequestHandler<WordCount.Request, WordCount.Response>
{
    public class Request : IRequest<Response>
    {
        public string Text { get; set; }
    }

    public class Response
    {
        public int Count { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response { Count = string.IsNullOrWhiteSpace(request.Text) ? 0 : request.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length });
}

// ─── Lambda 8: Timestamp Now ───
[OriginalName("LAMBDA_timestamp_now")]
public class TimestampNow : TaskRequestHandler<TimestampNow.Request, TimestampNow.Response>
{
    public class Request : IRequest<Response>
    {
        public string Format { get; set; }
    }

    public class Response
    {
        public string Timestamp { get; set; }
        public long UnixMs { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response
        {
            Timestamp = DateTimeOffset.UtcNow.ToString(string.IsNullOrEmpty(request.Format) ? "o" : request.Format),
            UnixMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
}

// ─── Lambda 9: Math Add ───
[OriginalName("LAMBDA_math_add")]
public class MathAdd : TaskRequestHandler<MathAdd.Request, MathAdd.Response>
{
    public class Request : IRequest<Response>
    {
        public double A { get; set; }
        public double B { get; set; }
    }

    public class Response
    {
        public double Result { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response { Result = request.A + request.B });
}

// ─── Lambda 10: Math Multiply ───
[OriginalName("LAMBDA_math_multiply")]
public class MathMultiply : TaskRequestHandler<MathMultiply.Request, MathMultiply.Response>
{
    public class Request : IRequest<Response>
    {
        public double A { get; set; }
        public double B { get; set; }
    }

    public class Response
    {
        public double Result { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response { Result = request.A * request.B });
}

// ─── Lambda 11: Json Key Extract ───
[OriginalName("LAMBDA_json_key_extract")]
public class JsonKeyExtract : TaskRequestHandler<JsonKeyExtract.Request, JsonKeyExtract.Response>
{
    public class Request : IRequest<Response>
    {
        public string Json { get; set; }
        public string Key { get; set; }
    }

    public class Response
    {
        public string Value { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var doc = System.Text.Json.JsonDocument.Parse(request.Json ?? "{}");
        var value = doc.RootElement.TryGetProperty(request.Key ?? "", out var prop) ? prop.ToString() : null;
        return Task.FromResult(new Response { Value = value });
    }
}

// ─── Lambda 12: String Concat ───
[OriginalName("LAMBDA_string_concat")]
public class StringConcat : TaskRequestHandler<StringConcat.Request, StringConcat.Response>
{
    public class Request : IRequest<Response>
    {
        public string A { get; set; }
        public string Separator { get; set; }
        public string B { get; set; }
    }

    public class Response
    {
        public string Result { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response { Result = $"{request.A}{request.Separator ?? ""}{request.B}" });
}

// ─── Lambda 13: Guid Generate ───
[OriginalName("LAMBDA_guid_generate")]
public class GuidGenerate : TaskRequestHandler<GuidGenerate.Request, GuidGenerate.Response>
{
    public class Request : IRequest<Response> { }

    public class Response
    {
        public string Guid { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response { Guid = System.Guid.NewGuid().ToString("D") });
}

// ─── Lambda 14: String Replace ───
[OriginalName("LAMBDA_string_replace")]
public class StringReplace : TaskRequestHandler<StringReplace.Request, StringReplace.Response>
{
    public class Request : IRequest<Response>
    {
        public string Input { get; set; }
        public string Old { get; set; }
        public string New { get; set; }
    }

    public class Response
    {
        public string Result { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response { Result = (request.Input ?? "").Replace(request.Old ?? "", request.New ?? "") });
}

// ─── Lambda 15: Random Number ───
[OriginalName("LAMBDA_random_number")]
public class RandomNumber : TaskRequestHandler<RandomNumber.Request, RandomNumber.Response>
{
    public class Request : IRequest<Response>
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class Response
    {
        public int Value { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
        => Task.FromResult(new Response { Value = Random.Shared.Next(request.Min, request.Max > request.Min ? request.Max : request.Min + 100) });
}
