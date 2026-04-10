using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace core8_nextjs_postgre.Services;

public interface IRabbitMQProducer
{
    Task PublishUserRegisteredEvent<T>(T message, CancellationToken cancellationToken = default);
}

public class RabbitMQProducer : IRabbitMQProducer, IAsyncDisposable
{
    private readonly ILogger<RabbitMQProducer> _logger;
    private readonly ConnectionFactory _connectionFactory;
    private const string QueueName = "user_registered_queue";

    #nullable enable
        // Disposables
        private IConnection? _connection;
        private IChannel? _channel;
    #nullable disable

    private bool _isInitialized;

    // Semaphore to prevent race conditions during lazy initialization
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public RabbitMQProducer(IConfiguration configuration, ILogger<RabbitMQProducer> logger)
    {
        _logger = logger;

        _connectionFactory = new ConnectionFactory
        {
            HostName = configuration["RabbitMQ:HostName"] ?? "localhost",
            UserName = configuration["RabbitMQ:UserName"] ?? "guest",
            Password = configuration["RabbitMQ:Password"] ?? "guest",
            VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/",
            AutomaticRecoveryEnabled = true, // Crucial for production
            TopologyRecoveryEnabled = true
            // Ssl = new SslOption 
            // {
            //     Enabled = true,
            //     ServerName = "b-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx.mq.us-east-1.amazonaws.com"
            // }            
        };
        
    }

    /// <summary>
    /// Safely and asynchronously creates the connection and channel once.
    /// </summary>
    private async Task EnsureInitializedAsync(CancellationToken cancellationToken)
    {
        if (_isInitialized && _channel is { IsOpen: true })
        {
            return;
        }

        await _connectionLock.WaitAsync(cancellationToken);
        try
        {
            if (_isInitialized && _channel is { IsOpen: true })
            {
                return;
            }

            _logger.LogInformation("Initializing RabbitMQ connection and channel...");

            _connection ??= await _connectionFactory.CreateConnectionAsync(cancellationToken);

            var options = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true
            );

            _channel = await _connection.CreateChannelAsync(options, cancellationToken);

            // Declare Queue once during initialization
            await _channel.QueueDeclareAsync(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);

            _isInitialized = true;
            _logger.LogInformation("RabbitMQ channel created and queue declared.");
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public async Task PublishUserRegisteredEvent<T>(T message, CancellationToken cancellationToken = default)
    {
        try
        {
            await EnsureInitializedAsync(cancellationToken);

            var json = JsonSerializer.Serialize(message, JsonOptions);
            var body = Encoding.UTF8.GetBytes(json);

            // v7+ uses BasicProperties directly
            var properties = new BasicProperties 
            { 
                Persistent = true, // Ensures message survives RabbitMQ crash
                ContentType = "application/json" // Best practice for downstream consumers
            };

            _logger.LogDebug("Publishing event to queue {QueueName}", QueueName);

            // 1. Publish the message
            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: QueueName,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            // 2. Wait for Publisher Confirmation (CRITICAL FOR PRODUCTION)
            // This ensures RabbitMQ safely received and persisted the message.
            // await _channel.WaitForConfirmsOrDieAsync(cancellationToken);
                
            _logger.LogDebug("Message successfully published and confirmed by broker.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message to RabbitMQ.");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Shutting down RabbitMQ Producer...");

        _connectionLock.Dispose();

        if (_channel is not null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}