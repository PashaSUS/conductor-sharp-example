using ConductorSharp.Engine.Builders;
using ConductorSharp.Engine.Builders.Metadata;
using ConductorSharp.Engine.Model;
using ConductorSharpExample.Tasks.Lambda;

namespace ConductorSharpExample.Workflows;

// ─── Input / Output ───
public class LambdaPipelineInput : WorkflowInput<LambdaPipelineOutput>
{
    public string RawText { get; set; }
    public string JsonPayload { get; set; }
    public double NumberA { get; set; }
    public double NumberB { get; set; }
}

public class LambdaPipelineOutput : WorkflowOutput
{
    public string GeneratedId { get; set; }
    public string Timestamp { get; set; }
    public string UpperReversed { get; set; }
    public string Hash { get; set; }
    public string Base64RoundTrip { get; set; }
    public int WordCount { get; set; }
    public double MathResult { get; set; }
    public string ExtractedValue { get; set; }
    public string FinalLabel { get; set; }
    public int RandomValue { get; set; }
    public int OriginalLength { get; set; }
}

// ─── Workflow ───
[OriginalName("WF_lambda_pipeline")]
[WorkflowMetadata(OwnerEmail = "lambda@example.com")]
public class LambdaPipelineWorkflow : Workflow<LambdaPipelineWorkflow, LambdaPipelineInput, LambdaPipelineOutput>
{
    public LambdaPipelineWorkflow(
        WorkflowDefinitionBuilder<LambdaPipelineWorkflow, LambdaPipelineInput, LambdaPipelineOutput> builder
    ) : base(builder) { }

    // All 15 lambda tasks wired into a single pipeline
    public GuidGenerate GuidGenerate { get; set; }
    public TimestampNow TimestampNow { get; set; }
    public StringLength StringLength { get; set; }
    public StringUpper StringUpper { get; set; }
    public StringReverse StringReverse { get; set; }
    public HashMd5 HashMd5 { get; set; }
    public Base64Encode Base64Encode { get; set; }
    public Base64Decode Base64Decode { get; set; }
    public WordCount WordCount { get; set; }
    public MathAdd MathAdd { get; set; }
    public MathMultiply MathMultiply { get; set; }
    public JsonKeyExtract JsonKeyExtract { get; set; }
    public StringConcat StringConcat { get; set; }
    public StringReplace StringReplace { get; set; }
    public RandomNumber RandomNumber { get; set; }

    public override void BuildDefinition()
    {
        // Step 1: Generate a unique correlation ID
        _builder.AddTask(
            wf => wf.GuidGenerate,
            wf => new GuidGenerate.Request()
        );

        // Step 2: Capture a timestamp
        _builder.AddTask(
            wf => wf.TimestampNow,
            wf => new TimestampNow.Request { Format = "yyyy-MM-dd HH:mm:ss" }
        );

        // Step 3: Measure the raw text length
        _builder.AddTask(
            wf => wf.StringLength,
            wf => new StringLength.Request { Input = wf.WorkflowInput.RawText }
        );

        // Step 4: Convert text to uppercase
        _builder.AddTask(
            wf => wf.StringUpper,
            wf => new StringUpper.Request { Input = wf.WorkflowInput.RawText }
        );

        // Step 5: Reverse the uppercased text
        _builder.AddTask(
            wf => wf.StringReverse,
            wf => new StringReverse.Request { Input = wf.StringUpper.Output.Result }
        );

        // Step 6: Hash the reversed+uppercased text
        _builder.AddTask(
            wf => wf.HashMd5,
            wf => new HashMd5.Request { Input = wf.StringReverse.Output.Result }
        );

        // Step 7: Base64-encode the original text
        _builder.AddTask(
            wf => wf.Base64Encode,
            wf => new Base64Encode.Request { Input = wf.WorkflowInput.RawText }
        );

        // Step 8: Decode it back to prove round-trip
        _builder.AddTask(
            wf => wf.Base64Decode,
            wf => new Base64Decode.Request { Encoded = wf.Base64Encode.Output.Encoded }
        );

        // Step 9: Count words in the original text
        _builder.AddTask(
            wf => wf.WordCount,
            wf => new WordCount.Request { Text = wf.WorkflowInput.RawText }
        );

        // Step 10: Add two numbers
        _builder.AddTask(
            wf => wf.MathAdd,
            wf => new MathAdd.Request { A = wf.WorkflowInput.NumberA, B = wf.WorkflowInput.NumberB }
        );

        // Step 11: Multiply the sum by NumberA
        _builder.AddTask(
            wf => wf.MathMultiply,
            wf => new MathMultiply.Request { A = wf.MathAdd.Output.Result, B = wf.WorkflowInput.NumberA }
        );

        // Step 12: Extract a key from the JSON payload
        _builder.AddTask(
            wf => wf.JsonKeyExtract,
            wf => new JsonKeyExtract.Request { Json = wf.WorkflowInput.JsonPayload, Key = "name" }
        );

        // Step 13: Build a label from the GUID + extracted name
        _builder.AddTask(
            wf => wf.StringConcat,
            wf => new StringConcat.Request
            {
                A = wf.GuidGenerate.Output.Guid,
                Separator = " :: ",
                B = wf.JsonKeyExtract.Output.Value
            }
        );

        // Step 14: Replace spaces in the label with underscores
        _builder.AddTask(
            wf => wf.StringReplace,
            wf => new StringReplace.Request
            {
                Input = wf.StringConcat.Output.Result,
                Old = " ",
                New = "_"
            }
        );

        // Step 15: Generate a random number
        _builder.AddTask(
            wf => wf.RandomNumber,
            wf => new RandomNumber.Request { Min = 1, Max = 10000 }
        );

        // Output: aggregate results from all lambda steps
        _builder.SetOutput(wf => new LambdaPipelineOutput
        {
            GeneratedId = wf.GuidGenerate.Output.Guid,
            Timestamp = wf.TimestampNow.Output.Timestamp,
            UpperReversed = wf.StringReverse.Output.Result,
            Hash = wf.HashMd5.Output.Hash,
            Base64RoundTrip = wf.Base64Decode.Output.Decoded,
            WordCount = wf.WordCount.Output.Count,
            MathResult = wf.MathMultiply.Output.Result,
            ExtractedValue = wf.JsonKeyExtract.Output.Value,
            FinalLabel = wf.StringReplace.Output.Result,
            RandomValue = wf.RandomNumber.Output.Value,
            OriginalLength = wf.StringLength.Output.Length
        });
    }
}
