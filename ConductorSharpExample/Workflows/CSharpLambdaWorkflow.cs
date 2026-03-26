using ConductorSharp.Engine.Builders;
using ConductorSharp.Engine.Builders.Metadata;
using ConductorSharp.Engine.Model;
using ConductorSharp.Patterns.Builders;
using ConductorSharp.Patterns.Model;
using MediatR;

namespace ConductorSharpExample.Workflows;

// ═══════════════════════════════════════════════════════════════
// C# Lambda Pipeline — inline lambdas executed server-side
// ═══════════════════════════════════════════════════════════════

// ─── Request/Response models for each inline lambda step ───

public class NormalizeInput : IRequest<NormalizeOutput>
{
    public string RawText { get; set; }
}

public class NormalizeOutput
{
    public string Normalized { get; set; }
}

public class ExtractWordsInput : IRequest<ExtractWordsOutput>
{
    public string Text { get; set; }
}

public class ExtractWordsOutput
{
    public string[] Words { get; set; }
    public int WordCount { get; set; }
}

public class ComputeStatsInput : IRequest<ComputeStatsOutput>
{
    public string Text { get; set; }
}

public class ComputeStatsOutput
{
    public int CharCount { get; set; }
    public int VowelCount { get; set; }
    public int ConsonantCount { get; set; }
    public int DigitCount { get; set; }
}

public class BuildSlugInput : IRequest<BuildSlugOutput>
{
    public string Text { get; set; }
}

public class BuildSlugOutput
{
    public string Slug { get; set; }
}

public class HashTextInput : IRequest<HashTextOutput>
{
    public string Text { get; set; }
}

public class HashTextOutput
{
    public string Sha256 { get; set; }
}

public class FormatResultInput : IRequest<FormatResultOutput>
{
    public string Slug { get; set; }
    public string Hash { get; set; }
    public int WordCount { get; set; }
    public int CharCount { get; set; }
}

public class FormatResultOutput
{
    public string Summary { get; set; }
}

// ─── Workflow Input/Output ───

public class CSharpLambdaPipelineInput : WorkflowInput<CSharpLambdaPipelineOutput>
{
    public string RawText { get; set; }
}

public class CSharpLambdaPipelineOutput : WorkflowOutput
{
    public string Normalized { get; set; }
    public int WordCount { get; set; }
    public int CharCount { get; set; }
    public int VowelCount { get; set; }
    public string Slug { get; set; }
    public string Hash { get; set; }
    public string Summary { get; set; }
}

// ─── Workflow ───

[OriginalName("WF_csharp_lambda_pipeline")]
[WorkflowMetadata(OwnerEmail = "lambda@example.com")]
public class CSharpLambdaPipelineWorkflow
    : Workflow<CSharpLambdaPipelineWorkflow, CSharpLambdaPipelineInput, CSharpLambdaPipelineOutput>
{
    public CSharpLambdaPipelineWorkflow(
        WorkflowDefinitionBuilder<CSharpLambdaPipelineWorkflow, CSharpLambdaPipelineInput, CSharpLambdaPipelineOutput> builder
    ) : base(builder) { }

    // Each property is a CSharpLambdaTaskModel — no separate handler class needed
    public CSharpLambdaTaskModel<NormalizeInput, NormalizeOutput> Normalize { get; set; }
    public CSharpLambdaTaskModel<ExtractWordsInput, ExtractWordsOutput> ExtractWords { get; set; }
    public CSharpLambdaTaskModel<ComputeStatsInput, ComputeStatsOutput> ComputeStats { get; set; }
    public CSharpLambdaTaskModel<BuildSlugInput, BuildSlugOutput> BuildSlug { get; set; }
    public CSharpLambdaTaskModel<HashTextInput, HashTextOutput> HashText { get; set; }
    public CSharpLambdaTaskModel<FormatResultInput, FormatResultOutput> FormatResult { get; set; }

    public override void BuildDefinition()
    {
        // Step 1: Normalize — trim, collapse whitespace, lowercase
        _builder.AddTask(
            wf => wf.Normalize,
            wf => new NormalizeInput { RawText = wf.WorkflowInput.RawText },
            input =>
            {
                var text = (input.RawText ?? "").Trim();
                var normalized = System.Text.RegularExpressions.Regex.Replace(text, @"\s+", " ").ToLowerInvariant();
                return new NormalizeOutput { Normalized = normalized };
            }
        );

        // Step 2: Extract words from normalized text
        _builder.AddTask(
            wf => wf.ExtractWords,
            wf => new ExtractWordsInput { Text = wf.Normalize.Output.Normalized },
            input =>
            {
                var words = (input.Text ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
                return new ExtractWordsOutput { Words = words, WordCount = words.Length };
            }
        );

        // Step 3: Compute character statistics
        _builder.AddTask(
            wf => wf.ComputeStats,
            wf => new ComputeStatsInput { Text = wf.Normalize.Output.Normalized },
            input =>
            {
                var text = input.Text ?? "";
                var vowels = new HashSet<char> { 'a', 'e', 'i', 'o', 'u' };
                var vowelCount = text.Count(c => vowels.Contains(c));
                var consonantCount = text.Count(c => char.IsLetter(c) && !vowels.Contains(c));
                var digitCount = text.Count(char.IsDigit);
                return new ComputeStatsOutput
                {
                    CharCount = text.Length,
                    VowelCount = vowelCount,
                    ConsonantCount = consonantCount,
                    DigitCount = digitCount
                };
            }
        );

        // Step 4: Build a URL-friendly slug
        _builder.AddTask(
            wf => wf.BuildSlug,
            wf => new BuildSlugInput { Text = wf.Normalize.Output.Normalized },
            input =>
            {
                var slug = System.Text.RegularExpressions.Regex.Replace(input.Text ?? "", @"[^a-z0-9\s-]", "");
                slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-").Trim('-');
                return new BuildSlugOutput { Slug = slug };
            }
        );

        // Step 5: Compute SHA-256 hash of the normalized text
        _builder.AddTask(
            wf => wf.HashText,
            wf => new HashTextInput { Text = wf.Normalize.Output.Normalized },
            input =>
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(input.Text ?? "");
                var hash = System.Security.Cryptography.SHA256.HashData(bytes);
                return new HashTextOutput { Sha256 = Convert.ToHexString(hash).ToLowerInvariant() };
            }
        );

        // Step 6: Format a human-readable summary from all previous steps
        _builder.AddTask(
            wf => wf.FormatResult,
            wf => new FormatResultInput
            {
                Slug = wf.BuildSlug.Output.Slug,
                Hash = wf.HashText.Output.Sha256,
                WordCount = wf.ExtractWords.Output.WordCount,
                CharCount = wf.ComputeStats.Output.CharCount
            },
            input => new FormatResultOutput
            {
                Summary = $"[{input.Slug}] words={input.WordCount} chars={input.CharCount} sha256={input.Hash[..16]}..."
            }
        );

        // Output: aggregate all results
        _builder.SetOutput(wf => new CSharpLambdaPipelineOutput
        {
            Normalized = wf.Normalize.Output.Normalized,
            WordCount = wf.ExtractWords.Output.WordCount,
            CharCount = wf.ComputeStats.Output.CharCount,
            VowelCount = wf.ComputeStats.Output.VowelCount,
            Slug = wf.BuildSlug.Output.Slug,
            Hash = wf.HashText.Output.Sha256,
            Summary = wf.FormatResult.Output.Summary
        });
    }
}
