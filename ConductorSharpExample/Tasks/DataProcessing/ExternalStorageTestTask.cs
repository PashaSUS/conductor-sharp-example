using System.Text;
using ConductorSharp.Engine;
using ConductorSharp.Engine.Builders.Metadata;
using MediatR;

namespace ConductorSharpExample.Tasks.DataProcessing;

[OriginalName("DATA_external_storage_test")]
public class ExternalStorageTest : TaskRequestHandler<ExternalStorageTest.Request, ExternalStorageTest.Response>
{
    public class Request : IRequest<Response>
    {
        public int RecordCount { get; set; }
        public int FieldsPerRecord { get; set; }
        public int FieldValueLength { get; set; }
    }

    public class Response
    {
        public int TotalRecords { get; set; }
        public long ApproximateSizeBytes { get; set; }
        public List<Dictionary<string, string>> Records { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        var recordCount = request.RecordCount > 0 ? request.RecordCount : 5000;
        var fieldsPerRecord = request.FieldsPerRecord > 0 ? request.FieldsPerRecord : 20;
        var fieldValueLength = request.FieldValueLength > 0 ? request.FieldValueLength : 200;

        var records = new List<Dictionary<string, string>>(recordCount);
        var rng = new Random(42);
        long totalSize = 0;

        for (var i = 0; i < recordCount; i++)
        {
            var record = new Dictionary<string, string>(fieldsPerRecord);
            for (var f = 0; f < fieldsPerRecord; f++)
            {
                var key = $"field_{f:D3}";
                var value = GenerateRandomString(rng, fieldValueLength);
                record[key] = value;
                totalSize += key.Length + value.Length;
            }
            records.Add(record);
        }

        return Task.FromResult(new Response
        {
            TotalRecords = recordCount,
            ApproximateSizeBytes = totalSize,
            Records = records
        });
    }

    private static string GenerateRandomString(Random rng, int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var sb = new StringBuilder(length);
        for (var i = 0; i < length; i++)
            sb.Append(chars[rng.Next(chars.Length)]);
        return sb.ToString();
    }
}
