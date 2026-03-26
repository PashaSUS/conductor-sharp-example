using ConductorSharp.Engine;
using ConductorSharp.Engine.Builders.Metadata;
using MediatR;

namespace ConductorSharpExample.Tasks.Notification;

// ─── Task 43 ───
[OriginalName("NOTIFICATION_send_email")]
public class SendEmail : TaskRequestHandler<SendEmail.Request, SendEmail.Response>
{
    public class Request : IRequest<Response>
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class Response
    {
        public bool Sent { get; set; }
        public string MessageId { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Sent = true, MessageId = $"MSG-{Random.Shared.Next(100000, 999999)}" });
    }
}

// ─── Task 44 ───
[OriginalName("NOTIFICATION_send_sms")]
public class SendSms : TaskRequestHandler<SendSms.Request, SendSms.Response>
{
    public class Request : IRequest<Response>
    {
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
    }

    public class Response
    {
        public bool Sent { get; set; }
        public string SmsId { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Sent = true, SmsId = $"SMS-{Random.Shared.Next(100000, 999999)}" });
    }
}

// ─── Task 45 ───
[OriginalName("NOTIFICATION_send_push")]
public class SendPush : TaskRequestHandler<SendPush.Request, SendPush.Response>
{
    public class Request : IRequest<Response>
    {
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public class Response
    {
        public bool Sent { get; set; }
        public int DevicesReached { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Sent = true, DevicesReached = 2 });
    }
}

// ─── Task 46 ───
[OriginalName("NOTIFICATION_send_webhook")]
public class SendWebhook : TaskRequestHandler<SendWebhook.Request, SendWebhook.Response>
{
    public class Request : IRequest<Response>
    {
        public string WebhookUrl { get; set; }
        public string Payload { get; set; }
    }

    public class Response
    {
        public bool Delivered { get; set; }
        public int StatusCode { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Delivered = true, StatusCode = 200 });
    }
}

// ─── Task 47 ───
[OriginalName("NOTIFICATION_render_template")]
public class RenderTemplate : TaskRequestHandler<RenderTemplate.Request, RenderTemplate.Response>
{
    public class Request : IRequest<Response>
    {
        public string TemplateName { get; set; }
        public string CustomerName { get; set; }
        public string OrderId { get; set; }
    }

    public class Response
    {
        public string RenderedSubject { get; set; }
        public string RenderedBody { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response
        {
            RenderedSubject = $"Order {request.OrderId} Confirmation",
            RenderedBody = $"Dear {request.CustomerName}, your order {request.OrderId} has been placed."
        });
    }
}

// ─── Task 48 ───
[OriginalName("NOTIFICATION_check_preference")]
public class CheckNotificationPreference : TaskRequestHandler<CheckNotificationPreference.Request, CheckNotificationPreference.Response>
{
    public class Request : IRequest<Response>
    {
        public int CustomerId { get; set; }
        public string Channel { get; set; }
    }

    public class Response
    {
        public bool OptedIn { get; set; }
        public string PreferredChannel { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { OptedIn = true, PreferredChannel = "email" });
    }
}

// ─── Task 49 ───
[OriginalName("NOTIFICATION_send_slack")]
public class SendSlack : TaskRequestHandler<SendSlack.Request, SendSlack.Response>
{
    public class Request : IRequest<Response>
    {
        public string Channel { get; set; }
        public string Message { get; set; }
    }

    public class Response
    {
        public bool Sent { get; set; }
        public string Timestamp { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Sent = true, Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() });
    }
}

// ─── Task 50 ───
[OriginalName("NOTIFICATION_log_event")]
public class LogNotificationEvent : TaskRequestHandler<LogNotificationEvent.Request, LogNotificationEvent.Response>
{
    public class Request : IRequest<Response>
    {
        public string EventType { get; set; }
        public string Recipient { get; set; }
        public string Status { get; set; }
    }

    public class Response
    {
        public bool Logged { get; set; }
        public string EventId { get; set; }
    }

    public override Task<Response> Handle(Request request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Response { Logged = true, EventId = $"EVT-{Random.Shared.Next(100000, 999999)}" });
    }
}
