﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v12.2.4.0 (NJsonSchema v9.13.36.0 (Newtonsoft.Json v11.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

using SagaDemo.Common.Errors.Swagger;

namespace SagaDemo.DeliveryAPI.ApiClient
{
    #pragma warning disable

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.2.4.0 (NJsonSchema v9.13.36.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial interface IDeliveryApiClient
    {
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<SwaggerResponse> CreateDeliveryRequestAsync(string transactionId, Address address);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<SwaggerResponse> CreateDeliveryRequestAsync(string transactionId, Address address, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<SwaggerResponse> RegisterDeliveryAttemptAsync(string transactionId);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<SwaggerResponse> RegisterDeliveryAttemptAsync(string transactionId, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<SwaggerResponse> CompleteDeliveryAsync(string transactionId);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<SwaggerResponse> CompleteDeliveryAsync(string transactionId, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<SwaggerResponse> CancelDeliveryAsync(string transactionId);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<SwaggerResponse> CancelDeliveryAsync(string transactionId, System.Threading.CancellationToken cancellationToken);
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.2.4.0 (NJsonSchema v9.13.36.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class DeliveryApiClient : IDeliveryApiClient
    {
        private System.Net.Http.HttpClient _httpClient;
        private System.Lazy<Newtonsoft.Json.JsonSerializerSettings> _settings;
    
        public DeliveryApiClient(System.Net.Http.HttpClient httpClient)
        {
            _httpClient = httpClient; 
            _settings = new System.Lazy<Newtonsoft.Json.JsonSerializerSettings>(() => 
            {
                var settings = new Newtonsoft.Json.JsonSerializerSettings();
                UpdateJsonSerializerSettings(settings);
                return settings;
            });
        }
    
        protected Newtonsoft.Json.JsonSerializerSettings JsonSerializerSettings { get { return _settings.Value; } }
    
        partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings);
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url);
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, System.Text.StringBuilder urlBuilder);
        partial void ProcessResponse(System.Net.Http.HttpClient client, System.Net.Http.HttpResponseMessage response);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwaggerResponse> CreateDeliveryRequestAsync(string transactionId, Address address)
        {
            return CreateDeliveryRequestAsync(transactionId, address, System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<SwaggerResponse> CreateDeliveryRequestAsync(string transactionId, Address address, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append("api/Delivery/{transactionId}");
            urlBuilder_.Replace("{transactionId}", System.Uri.EscapeDataString(ConvertToString(transactionId, System.Globalization.CultureInfo.InvariantCulture)));
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(address, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
    
                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);
    
                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }
    
                        ProcessResponse(client_, response_);
    
                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "204") 
                        {
                            return new SwaggerResponse((int)response_.StatusCode, headers_);
                        }
                        else
                        if (status_ == "400") 
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            var result_ = default(ValidationProblemDetails); 
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<ValidationProblemDetails>(responseData_, _settings.Value);
                            } 
                            catch (System.Exception exception_) 
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
                            throw new SwaggerException<ValidationProblemDetails>("A server side error occurred.", (int)response_.StatusCode, responseData_, headers_, result_, null);
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }
    
                        return new SwaggerResponse((int)response_.StatusCode, headers_); 
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
            }
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwaggerResponse> RegisterDeliveryAttemptAsync(string transactionId)
        {
            return RegisterDeliveryAttemptAsync(transactionId, System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<SwaggerResponse> RegisterDeliveryAttemptAsync(string transactionId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append("api/Delivery/{transactionId}/delivery-attempts");
            urlBuilder_.Replace("{transactionId}", System.Uri.EscapeDataString(ConvertToString(transactionId, System.Globalization.CultureInfo.InvariantCulture)));
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                    request_.Method = new System.Net.Http.HttpMethod("POST");
    
                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);
    
                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }
    
                        ProcessResponse(client_, response_);
    
                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "204") 
                        {
                            return new SwaggerResponse((int)response_.StatusCode, headers_);
                        }
                        else
                        if (status_ == "400") 
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            var result_ = default(ValidationProblemDetails); 
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<ValidationProblemDetails>(responseData_, _settings.Value);
                            } 
                            catch (System.Exception exception_) 
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
                            throw new SwaggerException<ValidationProblemDetails>("A server side error occurred.", (int)response_.StatusCode, responseData_, headers_, result_, null);
                        }
                        else
                        if (status_ == "404") 
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            var result_ = default(ProblemDetails); 
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<ProblemDetails>(responseData_, _settings.Value);
                            } 
                            catch (System.Exception exception_) 
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
                            throw new SwaggerException<ProblemDetails>("A server side error occurred.", (int)response_.StatusCode, responseData_, headers_, result_, null);
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }
    
                        return new SwaggerResponse((int)response_.StatusCode, headers_); 
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
            }
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwaggerResponse> CompleteDeliveryAsync(string transactionId)
        {
            return CompleteDeliveryAsync(transactionId, System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<SwaggerResponse> CompleteDeliveryAsync(string transactionId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append("api/Delivery/{transactionId}/complete");
            urlBuilder_.Replace("{transactionId}", System.Uri.EscapeDataString(ConvertToString(transactionId, System.Globalization.CultureInfo.InvariantCulture)));
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                    request_.Method = new System.Net.Http.HttpMethod("POST");
    
                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);
    
                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }
    
                        ProcessResponse(client_, response_);
    
                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "204") 
                        {
                            return new SwaggerResponse((int)response_.StatusCode, headers_);
                        }
                        else
                        if (status_ == "400") 
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            var result_ = default(ValidationProblemDetails); 
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<ValidationProblemDetails>(responseData_, _settings.Value);
                            } 
                            catch (System.Exception exception_) 
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
                            throw new SwaggerException<ValidationProblemDetails>("A server side error occurred.", (int)response_.StatusCode, responseData_, headers_, result_, null);
                        }
                        else
                        if (status_ == "404") 
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            var result_ = default(ProblemDetails); 
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<ProblemDetails>(responseData_, _settings.Value);
                            } 
                            catch (System.Exception exception_) 
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
                            throw new SwaggerException<ProblemDetails>("A server side error occurred.", (int)response_.StatusCode, responseData_, headers_, result_, null);
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }
    
                        return new SwaggerResponse((int)response_.StatusCode, headers_); 
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
            }
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task<SwaggerResponse> CancelDeliveryAsync(string transactionId)
        {
            return CancelDeliveryAsync(transactionId, System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<SwaggerResponse> CancelDeliveryAsync(string transactionId, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append("api/Delivery/{transactionId}/cancel");
            urlBuilder_.Replace("{transactionId}", System.Uri.EscapeDataString(ConvertToString(transactionId, System.Globalization.CultureInfo.InvariantCulture)));
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json");
                    request_.Method = new System.Net.Http.HttpMethod("POST");
    
                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);
    
                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        if (response_.Content != null && response_.Content.Headers != null)
                        {
                            foreach (var item_ in response_.Content.Headers)
                                headers_[item_.Key] = item_.Value;
                        }
    
                        ProcessResponse(client_, response_);
    
                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "204") 
                        {
                            return new SwaggerResponse((int)response_.StatusCode, headers_);
                        }
                        else
                        if (status_ == "400") 
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            var result_ = default(ValidationProblemDetails); 
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<ValidationProblemDetails>(responseData_, _settings.Value);
                            } 
                            catch (System.Exception exception_) 
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
                            throw new SwaggerException<ValidationProblemDetails>("A server side error occurred.", (int)response_.StatusCode, responseData_, headers_, result_, null);
                        }
                        else
                        if (status_ == "404") 
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            var result_ = default(ProblemDetails); 
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<ProblemDetails>(responseData_, _settings.Value);
                            } 
                            catch (System.Exception exception_) 
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
                            throw new SwaggerException<ProblemDetails>("A server side error occurred.", (int)response_.StatusCode, responseData_, headers_, result_, null);
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }
    
                        return new SwaggerResponse((int)response_.StatusCode, headers_); 
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
            }
        }
    
        private string ConvertToString(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value is System.Enum)
            {
                string name = System.Enum.GetName(value.GetType(), value);
                if (name != null)
                {
                    var field = System.Reflection.IntrospectionExtensions.GetTypeInfo(value.GetType()).GetDeclaredField(name);
                    if (field != null)
                    {
                        var attribute = System.Reflection.CustomAttributeExtensions.GetCustomAttribute(field, typeof(System.Runtime.Serialization.EnumMemberAttribute)) 
                            as System.Runtime.Serialization.EnumMemberAttribute;
                        if (attribute != null)
                        {
                            return attribute.Value != null ? attribute.Value : name;
                        }
                    }
                }
            }
            else if (value is bool) {
                return System.Convert.ToString(value, cultureInfo).ToLowerInvariant();
            }
            else if (value is byte[])
            {
                return System.Convert.ToBase64String((byte[]) value);
            }
            else if (value != null && value.GetType().IsArray)
            {
                var array = System.Linq.Enumerable.OfType<object>((System.Array) value);
                return string.Join(",", System.Linq.Enumerable.Select(array, o => ConvertToString(o, cultureInfo)));
            }
        
            return System.Convert.ToString(value, cultureInfo);
        }
    }
    
    

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.36.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ProblemDetails 
    {
        [Newtonsoft.Json.JsonConstructor]
        public ProblemDetails(string @detail, string @instance, int? @status, string @title, string @type)
        {
            this.Type = @type;
            this.Title = @title;
            this.Status = @status;
            this.Detail = @detail;
            this.Instance = @instance;
        }
    
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Type { get; }
    
        [Newtonsoft.Json.JsonProperty("title", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Title { get; }
    
        [Newtonsoft.Json.JsonProperty("status", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public int? Status { get; }
    
        [Newtonsoft.Json.JsonProperty("detail", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Detail { get; }
    
        [Newtonsoft.Json.JsonProperty("instance", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Instance { get; }
    
        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();
    
        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    
        public static ProblemDetails FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ProblemDetails>(data);
        }
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.36.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class ValidationProblemDetails : ProblemDetails
    {
        [Newtonsoft.Json.JsonConstructor]
        public ValidationProblemDetails(string @detail, System.Collections.Generic.IDictionary<string, System.Collections.Generic.ICollection<string>> @errors, string @instance, int? @status, string @title, string @type)
            : base(detail, instance, status, title, type)
        {
            this.Errors = @errors;
        }
    
        [Newtonsoft.Json.JsonProperty("errors", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public System.Collections.Generic.IDictionary<string, System.Collections.Generic.ICollection<string>> Errors { get; }
    
        private System.Collections.Generic.IDictionary<string, object> _additionalProperties = new System.Collections.Generic.Dictionary<string, object>();
    
        [Newtonsoft.Json.JsonExtensionData]
        public System.Collections.Generic.IDictionary<string, object> AdditionalProperties
        {
            get { return _additionalProperties; }
            set { _additionalProperties = value; }
        }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    
        public static ValidationProblemDetails FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ValidationProblemDetails>(data);
        }
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.13.36.0 (Newtonsoft.Json v11.0.0.0)")]
    public partial class Address 
    {
        [Newtonsoft.Json.JsonConstructor]
        public Address(string @city, string @country, string @house, string @state, string @street, string @zip)
        {
            this.Country = @country;
            this.State = @state;
            this.City = @city;
            this.Zip = @zip;
            this.Street = @street;
            this.House = @house;
        }
    
        [Newtonsoft.Json.JsonProperty("country", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Country { get; }
    
        [Newtonsoft.Json.JsonProperty("state", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string State { get; }
    
        [Newtonsoft.Json.JsonProperty("city", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string City { get; }
    
        [Newtonsoft.Json.JsonProperty("zip", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Zip { get; }
    
        [Newtonsoft.Json.JsonProperty("street", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Street { get; }
    
        [Newtonsoft.Json.JsonProperty("house", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string House { get; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    
        public static Address FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Address>(data);
        }
    
    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.2.4.0 (NJsonSchema v9.13.36.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class SwaggerResponse
    {
        public int StatusCode { get; private set; }

        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }
        
        public SwaggerResponse(int statusCode, System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> headers) 
        {
            StatusCode = statusCode; 
            Headers = headers;
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.2.4.0 (NJsonSchema v9.13.36.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class SwaggerResponse<TResult> : SwaggerResponse
    {
        public TResult Result { get; private set; }
        
        public SwaggerResponse(int statusCode, System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> headers, TResult result) 
            : base(statusCode, headers)
        {
            Result = result;
        }
    }

    #pragma warning restore
}