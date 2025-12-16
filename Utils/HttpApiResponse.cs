using System.Net;
using System.Text;

namespace CommunityEventsApi.Utils
{
    /// <summary>
    /// Generic API response class inspired by HttpResponseMessage, providing a standardized way to return API responses
    /// with status codes, reason phrases, data, and additional metadata.
    /// </summary>
    /// <typeparam name="T">The type of data to be returned in the response</typeparam>
    public class HttpApiResponse<T>
    {
        private const HttpStatusCode DefaultStatusCode = HttpStatusCode.OK;
        private static readonly Version DefaultVersion = new Version(1, 1);

        private HttpStatusCode _statusCode;
        private string? _reasonPhrase;
        private Version _version;
        private bool _disposed;

        /// <summary>
        /// Gets or sets the HTTP version used for the response.
        /// </summary>
        public Version Version
        {
            get => _version;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                CheckDisposed();
                _version = value;
            }
        }

        /// <summary>
        /// Gets or sets the data content of the response.
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code of the response.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get => _statusCode;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative((int)value, nameof(value));
                ArgumentOutOfRangeException.ThrowIfGreaterThan((int)value, 999, nameof(value));
                CheckDisposed();
                _statusCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the reason phrase associated with the status code.
        /// </summary>
        public string? ReasonPhrase
        {
            get
            {
                if (_reasonPhrase != null)
                {
                    return _reasonPhrase;
                }
                // Provide a default if one was not set.
                return GetDefaultReasonPhrase(_statusCode);
            }
            set
            {
                if ((value != null) && ContainsNewLine(value))
                {
                    throw new FormatException("Reason phrase cannot contain new line characters.");
                }
                CheckDisposed();
                _reasonPhrase = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the response indicates success.
        /// </summary>
        public bool IsSuccess => ((int)_statusCode >= 200) && ((int)_statusCode <= 299);

        /// <summary>
        /// Gets or sets additional headers for the response.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new();

        /// <summary>
        /// Gets or sets any additional metadata for the response.
        /// </summary>
        public object? Metadata { get; set; }

        /// <summary>
        /// Initializes a new instance of the HttpApiResponse class with default status code.
        /// </summary>
        public HttpApiResponse() : this(DefaultStatusCode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpApiResponse class with the specified status code.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the response.</param>
        public HttpApiResponse(HttpStatusCode statusCode)
        {
            ArgumentOutOfRangeException.ThrowIfNegative((int)statusCode, nameof(statusCode));
            ArgumentOutOfRangeException.ThrowIfGreaterThan((int)statusCode, 999, nameof(statusCode));

            _statusCode = statusCode;
            _version = DefaultVersion;
        }

        /// <summary>
        /// Initializes a new instance of the HttpApiResponse class with status code, success indicator, reason phrase, and data.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="reasonPhrase">The reason phrase.</param>
        /// <param name="data">The response data.</param>
        public HttpApiResponse(HttpStatusCode statusCode, string? reasonPhrase, T? data)
            : this(statusCode)
        {
            ReasonPhrase = reasonPhrase;
            Data = data;
        }

        /// <summary>
        /// Ensures that the response has a success status code, throwing an exception if not.
        /// </summary>
        /// <returns>The current instance if successful.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the status code does not indicate success.</exception>
        public HttpApiResponse<T> EnsureSuccessStatusCode()
        {
            if (!IsSuccess)
            {
                throw new InvalidOperationException(
                    $"Response status code does not indicate success: {(int)_statusCode} ({ReasonPhrase})");
            }
            return this;
        }

        /// <summary>
        /// Adds a header to the response.
        /// </summary>
        /// <param name="key">The header key.</param>
        /// <param name="value">The header value.</param>
        public void AddHeader(string key, string value)
        {
            CheckDisposed();
            Headers[key] = value;
        }

        /// <summary>
        /// Returns a string representation of the response.
        /// </summary>
        /// <returns>A string representation of the response.</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("StatusCode: ");
            sb.Append((int)_statusCode);
            sb.Append(", ReasonPhrase: '");
            sb.Append(ReasonPhrase ?? "<null>");
            sb.Append("', Version: ");
            sb.Append(_version);
            sb.Append(", Data: ");
            sb.Append(Data == null ? "<null>" : Data.GetType().ToString());
            sb.Append(", IsSuccess: ");
            sb.Append(IsSuccess);

            if (Headers.Any())
            {
                sb.AppendLine(", Headers:");
                foreach (var header in Headers)
                {
                    sb.AppendLine($"  {header.Key}: {header.Value}");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a successful response with the specified data.
        /// </summary>
        /// <param name="data">The response data.</param>
        /// <returns>A successful HttpApiResponse instance.</returns>
        public static HttpApiResponse<T> Success(T data)
        {
            return new HttpApiResponse<T>(HttpStatusCode.OK, "Success", data);
        }

        /// <summary>
        /// Creates a successful response with the specified data and custom message.
        /// </summary>
        /// <param name="data">The response data.</param>
        /// <param name="message">The success message.</param>
        /// <returns>A successful HttpApiResponse instance.</returns>
        public static HttpApiResponse<T> Success(T data, string message)
        {
            return new HttpApiResponse<T>(HttpStatusCode.OK, message, data);
        }

        /// <summary>
        /// Creates an error response with the specified status code and message.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="message">The error message.</param>
        /// <returns>An error HttpApiResponse instance.</returns>
        public static HttpApiResponse<T> Error(HttpStatusCode statusCode, string message)
        {
            return new HttpApiResponse<T>(statusCode, message, default);
        }

        /// <summary>
        /// Creates a not found response.
        /// </summary>
        /// <param name="message">The not found message.</param>
        /// <returns>A not found HttpApiResponse instance.</returns>
        public static HttpApiResponse<T> NotFound(string message = "Resource not found")
        {
            return new HttpApiResponse<T>(HttpStatusCode.NotFound, message, default);
        }

        /// <summary>
        /// Creates a bad request response.
        /// </summary>
        /// <param name="message">The bad request message.</param>
        /// <returns>A bad request HttpApiResponse instance.</returns>
        public static HttpApiResponse<T> BadRequest(string message = "Bad request")
        {
            return new HttpApiResponse<T>(HttpStatusCode.BadRequest, message, default);
        }

        /// <summary>
        /// Creates an unauthorized response.
        /// </summary>
        /// <param name="message">The unauthorized message.</param>
        /// <returns>An unauthorized HttpApiResponse instance.</returns>
        public static HttpApiResponse<T> Unauthorized(string message = "Unauthorized")
        {
            return new HttpApiResponse<T>(HttpStatusCode.Unauthorized, message, default);
        }

        /// <summary>
        /// Creates a forbidden response.
        /// </summary>
        /// <param name="message">The forbidden message.</param>
        /// <returns>A forbidden HttpApiResponse instance.</returns>
        public static HttpApiResponse<T> Forbidden(string message = "Forbidden")
        {
            return new HttpApiResponse<T>(HttpStatusCode.Forbidden, message, default);
        }

        /// <summary>
        /// Creates an internal server error response.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>An internal server error HttpApiResponse instance.</returns>
        public static HttpApiResponse<T> InternalServerError(string message = "Internal server error")
        {
            return new HttpApiResponse<T>(HttpStatusCode.InternalServerError, message, default);
        }

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                // Dispose of any disposable resources if needed
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private void CheckDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }

        private static string? GetDefaultReasonPhrase(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.OK => "OK",
                HttpStatusCode.Created => "Created",
                HttpStatusCode.Accepted => "Accepted",
                HttpStatusCode.NoContent => "No Content",
                HttpStatusCode.BadRequest => "Bad Request",
                HttpStatusCode.Unauthorized => "Unauthorized",
                HttpStatusCode.Forbidden => "Forbidden",
                HttpStatusCode.NotFound => "Not Found",
                HttpStatusCode.MethodNotAllowed => "Method Not Allowed",
                HttpStatusCode.Conflict => "Conflict",
                HttpStatusCode.InternalServerError => "Internal Server Error",
                HttpStatusCode.NotImplemented => "Not Implemented",
                HttpStatusCode.BadGateway => "Bad Gateway",
                HttpStatusCode.ServiceUnavailable => "Service Unavailable",
                _ => null
            };
        }

        private static bool ContainsNewLine(string value)
        {
            return value.Contains('\r') || value.Contains('\n');
        }
    }
}