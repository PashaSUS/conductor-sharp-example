using ConductorSharp.Engine.Builders;
using ConductorSharp.Engine.Builders.Metadata;
using ConductorSharp.Engine.Model;
using ConductorSharpExample.Tasks.FileProcessing;
using ConductorSharpExample.Tasks.Notification;
using ConductorSharpExample.Tasks.DataProcessing;
using ConductorSharpExample.Tasks.Auth;
using ConductorSharpExample.Tasks.Customer;

namespace ConductorSharpExample.Workflows;

// ═══════════════════════════════════════════════════════════════
// Workflow 22: Secure File Upload Pipeline
// ═══════════════════════════════════════════════════════════════
public class FileUploadPipelineInput : WorkflowInput<FileUploadPipelineOutput>
{
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long SizeBytes { get; set; }
    public string UploaderUserId { get; set; }
}

public class FileUploadPipelineOutput : WorkflowOutput
{
    public string FileId { get; set; }
    public string StorageUrl { get; set; }
    public bool VirusScanClean { get; set; }
}

[OriginalName("WF_file_upload_pipeline")]
[WorkflowMetadata(OwnerEmail = "platform@example.com")]
public class FileUploadPipelineWorkflow : Workflow<FileUploadPipelineWorkflow, FileUploadPipelineInput, FileUploadPipelineOutput>
{
    public FileUploadPipelineWorkflow(
        WorkflowDefinitionBuilder<FileUploadPipelineWorkflow, FileUploadPipelineInput, FileUploadPipelineOutput> builder
    ) : base(builder) { }

    public ValidateFile ValidateFile { get; set; }
    public UploadFile Upload { get; set; }
    public ScanVirus ScanVirus { get; set; }
    public ExtractMetadata ExtractMeta { get; set; }
    public AuditLog AuditLog { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.ValidateFile,
            wf => new ValidateFile.Request { FileId = wf.WorkflowInput.FileName, ExpectedContentType = wf.WorkflowInput.ContentType, MaxSizeBytes = 50_000_000 });

        _builder.AddTask(wf => wf.Upload,
            wf => new UploadFile.Request { FileName = wf.WorkflowInput.FileName, ContentType = wf.WorkflowInput.ContentType, SizeBytes = wf.WorkflowInput.SizeBytes });

        _builder.AddTask(wf => wf.ScanVirus,
            wf => new ScanVirus.Request { FileId = wf.Upload.Output.FileId, StorageUrl = wf.Upload.Output.StorageUrl });

        _builder.AddTask(wf => wf.ExtractMeta,
            wf => new ExtractMetadata.Request { FileId = wf.Upload.Output.FileId });

        _builder.AddTask(wf => wf.AuditLog,
            wf => new AuditLog.Request { UserId = wf.WorkflowInput.UploaderUserId, Action = "file_uploaded", Resource = wf.Upload.Output.FileId, IpAddress = "0.0.0.0" });

        _builder.SetOutput(wf => new FileUploadPipelineOutput
        {
            FileId = wf.Upload.Output.FileId,
            StorageUrl = wf.Upload.Output.StorageUrl,
            VirusScanClean = wf.ScanVirus.Output.Clean
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 23: Image Processing Pipeline
// ═══════════════════════════════════════════════════════════════
public class ImageProcessingInput : WorkflowInput<ImageProcessingOutput>
{
    public string FileName { get; set; }
    public long SizeBytes { get; set; }
}

public class ImageProcessingOutput : WorkflowOutput
{
    public string FileId { get; set; }
    public string ThumbnailUrl { get; set; }
    public string CompressedFileId { get; set; }
}

[OriginalName("WF_image_processing")]
[WorkflowMetadata(OwnerEmail = "platform@example.com")]
public class ImageProcessingWorkflow : Workflow<ImageProcessingWorkflow, ImageProcessingInput, ImageProcessingOutput>
{
    public ImageProcessingWorkflow(
        WorkflowDefinitionBuilder<ImageProcessingWorkflow, ImageProcessingInput, ImageProcessingOutput> builder
    ) : base(builder) { }

    public UploadFile Upload { get; set; }
    public ScanVirus ScanVirus { get; set; }
    public GenerateThumbnail GenThumbnail { get; set; }
    public CompressFile Compress { get; set; }
    public ExtractMetadata ExtractMeta { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.Upload,
            wf => new UploadFile.Request { FileName = wf.WorkflowInput.FileName, ContentType = "image/jpeg", SizeBytes = wf.WorkflowInput.SizeBytes });

        _builder.AddTask(wf => wf.ScanVirus,
            wf => new ScanVirus.Request { FileId = wf.Upload.Output.FileId, StorageUrl = wf.Upload.Output.StorageUrl });

        _builder.AddTask(wf => wf.GenThumbnail,
            wf => new GenerateThumbnail.Request { FileId = wf.Upload.Output.FileId, Width = 200, Height = 200 });

        _builder.AddTask(wf => wf.Compress,
            wf => new CompressFile.Request { FileId = wf.Upload.Output.FileId, Algorithm = "webp" });

        _builder.AddTask(wf => wf.ExtractMeta,
            wf => new ExtractMetadata.Request { FileId = wf.Upload.Output.FileId });

        _builder.SetOutput(wf => new ImageProcessingOutput
        {
            FileId = wf.Upload.Output.FileId,
            ThumbnailUrl = wf.GenThumbnail.Output.ThumbnailUrl,
            CompressedFileId = wf.Compress.Output.CompressedFileId
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 24: CSV Import Pipeline
// ═══════════════════════════════════════════════════════════════
public class CsvImportInput : WorkflowInput<CsvImportOutput>
{
    public string FileName { get; set; }
    public long SizeBytes { get; set; }
    public string TargetTable { get; set; }
}

public class CsvImportOutput : WorkflowOutput
{
    public int RecordsImported { get; set; }
    public string JobId { get; set; }
}

[OriginalName("WF_csv_import")]
[WorkflowMetadata(OwnerEmail = "data@example.com")]
public class CsvImportWorkflow : Workflow<CsvImportWorkflow, CsvImportInput, CsvImportOutput>
{
    public CsvImportWorkflow(
        WorkflowDefinitionBuilder<CsvImportWorkflow, CsvImportInput, CsvImportOutput> builder
    ) : base(builder) { }

    public UploadFile Upload { get; set; }
    public ValidateFile Validate { get; set; }
    public ParseCsv Parse { get; set; }
    public ValidateSchema ValidateSchema { get; set; }
    public DeduplicateRecords Deduplicate { get; set; }
    public BatchImport Import { get; set; }
    public SendSlack Notify { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.Upload,
            wf => new UploadFile.Request { FileName = wf.WorkflowInput.FileName, ContentType = "text/csv", SizeBytes = wf.WorkflowInput.SizeBytes });

        _builder.AddTask(wf => wf.Validate,
            wf => new ValidateFile.Request { FileId = wf.Upload.Output.FileId, ExpectedContentType = "text/csv", MaxSizeBytes = 100_000_000 });

        _builder.AddTask(wf => wf.Parse,
            wf => new ParseCsv.Request { FileId = wf.Upload.Output.FileId, Delimiter = ",", HasHeader = true });

        _builder.AddTask(wf => wf.ValidateSchema,
            wf => new ValidateSchema.Request { Data = wf.Upload.Output.FileId, SchemaName = wf.WorkflowInput.TargetTable });

        _builder.AddTask(wf => wf.Deduplicate,
            wf => new DeduplicateRecords.Request { DatasetId = wf.Upload.Output.FileId, KeyField = "id" });

        _builder.AddTask(wf => wf.Import,
            wf => new BatchImport.Request { SourceUrl = wf.Upload.Output.StorageUrl, TargetTable = wf.WorkflowInput.TargetTable, Format = "csv" });

        _builder.AddTask(wf => wf.Notify,
            wf => new SendSlack.Request { Channel = "#data-ops", Message = $"CSV import: {wf.Import.Output.RecordsImported} records into {wf.WorkflowInput.TargetTable}" });

        _builder.SetOutput(wf => new CsvImportOutput
        {
            RecordsImported = wf.Import.Output.RecordsImported,
            JobId = wf.Import.Output.JobId
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 25: Document Processing
// ═══════════════════════════════════════════════════════════════
public class DocumentProcessingInput : WorkflowInput<DocumentProcessingOutput>
{
    public string FileName { get; set; }
    public long SizeBytes { get; set; }
    public string TargetFormat { get; set; }
}

public class DocumentProcessingOutput : WorkflowOutput
{
    public string ConvertedFileId { get; set; }
    public string EncryptedFileId { get; set; }
}

[OriginalName("WF_document_processing")]
[WorkflowMetadata(OwnerEmail = "platform@example.com")]
public class DocumentProcessingWorkflow : Workflow<DocumentProcessingWorkflow, DocumentProcessingInput, DocumentProcessingOutput>
{
    public DocumentProcessingWorkflow(
        WorkflowDefinitionBuilder<DocumentProcessingWorkflow, DocumentProcessingInput, DocumentProcessingOutput> builder
    ) : base(builder) { }

    public UploadFile Upload { get; set; }
    public ScanVirus Scan { get; set; }
    public ConvertFileFormat Convert { get; set; }
    public EncryptFile Encrypt { get; set; }
    public ExtractMetadata ExtractMeta { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.Upload,
            wf => new UploadFile.Request { FileName = wf.WorkflowInput.FileName, ContentType = "application/pdf", SizeBytes = wf.WorkflowInput.SizeBytes });

        _builder.AddTask(wf => wf.Scan,
            wf => new ScanVirus.Request { FileId = wf.Upload.Output.FileId, StorageUrl = wf.Upload.Output.StorageUrl });

        _builder.AddTask(wf => wf.Convert,
            wf => new ConvertFileFormat.Request { FileId = wf.Upload.Output.FileId, TargetFormat = wf.WorkflowInput.TargetFormat });

        _builder.AddTask(wf => wf.Encrypt,
            wf => new EncryptFile.Request { FileId = wf.Convert.Output.ConvertedFileId, EncryptionAlgorithm = "AES256" });

        _builder.AddTask(wf => wf.ExtractMeta,
            wf => new ExtractMetadata.Request { FileId = wf.Convert.Output.ConvertedFileId });

        _builder.SetOutput(wf => new DocumentProcessingOutput
        {
            ConvertedFileId = wf.Convert.Output.ConvertedFileId,
            EncryptedFileId = wf.Encrypt.Output.EncryptedFileId
        });
    }
}

// ═══════════════════════════════════════════════════════════════
// Workflow 26: File Backup
// ═══════════════════════════════════════════════════════════════
public class FileBackupInput : WorkflowInput<FileBackupOutput>
{
    public string SourceFileId { get; set; }
    public string BackupDestination { get; set; }
}

public class FileBackupOutput : WorkflowOutput
{
    public string BackupFileId { get; set; }
    public string Checksum { get; set; }
}

[OriginalName("WF_file_backup")]
[WorkflowMetadata(OwnerEmail = "platform@example.com")]
public class FileBackupWorkflow : Workflow<FileBackupWorkflow, FileBackupInput, FileBackupOutput>
{
    public FileBackupWorkflow(
        WorkflowDefinitionBuilder<FileBackupWorkflow, FileBackupInput, FileBackupOutput> builder
    ) : base(builder) { }

    public ExtractMetadata ExtractMeta { get; set; }
    public CompressFile Compress { get; set; }
    public EncryptFile Encrypt { get; set; }
    public CopyFile CopyToBackup { get; set; }
    public ComputeHash VerifyHash { get; set; }

    public override void BuildDefinition()
    {
        _builder.AddTask(wf => wf.ExtractMeta,
            wf => new ExtractMetadata.Request { FileId = wf.WorkflowInput.SourceFileId });

        _builder.AddTask(wf => wf.Compress,
            wf => new CompressFile.Request { FileId = wf.WorkflowInput.SourceFileId, Algorithm = "gzip" });

        _builder.AddTask(wf => wf.Encrypt,
            wf => new EncryptFile.Request { FileId = wf.Compress.Output.CompressedFileId, EncryptionAlgorithm = "AES256" });

        _builder.AddTask(wf => wf.CopyToBackup,
            wf => new CopyFile.Request { SourceFileId = wf.Encrypt.Output.EncryptedFileId, DestinationPath = wf.WorkflowInput.BackupDestination });

        _builder.AddTask(wf => wf.VerifyHash,
            wf => new ComputeHash.Request { Data = wf.CopyToBackup.Output.NewFileId, Algorithm = "SHA256" });

        _builder.SetOutput(wf => new FileBackupOutput
        {
            BackupFileId = wf.CopyToBackup.Output.NewFileId,
            Checksum = wf.VerifyHash.Output.HashValue
        });
    }
}
