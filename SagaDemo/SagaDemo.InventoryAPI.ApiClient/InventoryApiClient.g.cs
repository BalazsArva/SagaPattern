﻿//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v12.2.4.0 (NJsonSchema v9.13.36.0 (Newtonsoft.Json v11.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

using SagaDemo.Common.Errors.Swagger;

namespace SagaDemo.InventoryAPI.ApiClient
{
    #pragma warning disable

    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.2.4.0 (NJsonSchema v9.13.36.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial interface ICatalogApiClient
    {
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<SwaggerResponse<GetProductByIdResponse>> CreateItemAsync(CreateProductCommand command);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<SwaggerResponse<GetProductByIdResponse>> CreateItemAsync(CreateProductCommand command, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> GetItemAsync(string id);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<FileResponse> GetItemAsync(string id, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> ReserveItemAsync(int id);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<FileResponse> ReserveItemAsync(int id, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> TakeoutItemAsync(int id);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<FileResponse> TakeoutItemAsync(int id, System.Threading.CancellationToken cancellationToken);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<FileResponse> BringbackItemAsync(int id);
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        System.Threading.Tasks.Task<FileResponse> BringbackItemAsync(int id, System.Threading.CancellationToken cancellationToken);
    
    }
    
    [System.CodeDom.Compiler.GeneratedCode("NSwag", "12.2.4.0 (NJsonSchema v9.13.36.0 (Newtonsoft.Json v11.0.0.0))")]
    public partial class CatalogApiClient : ICatalogApiClient
    {
        private System.Net.Http.HttpClient _httpClient;
        private System.Lazy<Newtonsoft.Json.JsonSerializerSettings> _settings;
    
        public CatalogApiClient(System.Net.Http.HttpClient httpClient)
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
        public System.Threading.Tasks.Task<SwaggerResponse<GetProductByIdResponse>> CreateItemAsync(CreateProductCommand command)
        {
            return CreateItemAsync(command, System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<SwaggerResponse<GetProductByIdResponse>> CreateItemAsync(CreateProductCommand command, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append("api/Catalog");
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(command, _settings.Value));
                    content_.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/json");
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/json"));
    
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
                        if (status_ == "201") 
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            var result_ = default(GetProductByIdResponse); 
                            try
                            {
                                result_ = Newtonsoft.Json.JsonConvert.DeserializeObject<GetProductByIdResponse>(responseData_, _settings.Value);
                                return new SwaggerResponse<GetProductByIdResponse>((int)response_.StatusCode, headers_, result_); 
                            } 
                            catch (System.Exception exception_) 
                            {
                                throw new SwaggerException("Could not deserialize the response body.", (int)response_.StatusCode, responseData_, headers_, exception_);
                            }
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
            
                        return new SwaggerResponse<GetProductByIdResponse>((int)response_.StatusCode, headers_, default(GetProductByIdResponse)); 
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
        public System.Threading.Tasks.Task<FileResponse> GetItemAsync(string id)
        {
            return GetItemAsync(id, System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<FileResponse> GetItemAsync(string id, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append("api/Catalog/{id}");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Method = new System.Net.Http.HttpMethod("GET");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/octet-stream"));
    
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
                        if (status_ == "200" || status_ == "206") 
                        {
                            var responseStream_ = response_.Content == null ? System.IO.Stream.Null : await response_.Content.ReadAsStreamAsync().ConfigureAwait(false);
                            var fileResponse_ = new FileResponse((int)response_.StatusCode, headers_, responseStream_, null, response_); 
                            client_ = null; response_ = null; // response and client are disposed by FileResponse
                            return fileResponse_;
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }
            
                        return default(FileResponse);
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
        public System.Threading.Tasks.Task<FileResponse> ReserveItemAsync(int id)
        {
            return ReserveItemAsync(id, System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<FileResponse> ReserveItemAsync(int id, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");
    
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append("api/Catalog/{id}/reservations");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/octet-stream");
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/octet-stream"));
    
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
                        if (status_ == "200" || status_ == "206") 
                        {
                            var responseStream_ = response_.Content == null ? System.IO.Stream.Null : await response_.Content.ReadAsStreamAsync().ConfigureAwait(false);
                            var fileResponse_ = new FileResponse((int)response_.StatusCode, headers_, responseStream_, null, response_); 
                            client_ = null; response_ = null; // response and client are disposed by FileResponse
                            return fileResponse_;
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }
            
                        return default(FileResponse);
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
        public System.Threading.Tasks.Task<FileResponse> TakeoutItemAsync(int id)
        {
            return TakeoutItemAsync(id, System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<FileResponse> TakeoutItemAsync(int id, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");
    
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append("api/Catalog/{id}/takeout");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/octet-stream");
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/octet-stream"));
    
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
                        if (status_ == "200" || status_ == "206") 
                        {
                            var responseStream_ = response_.Content == null ? System.IO.Stream.Null : await response_.Content.ReadAsStreamAsync().ConfigureAwait(false);
                            var fileResponse_ = new FileResponse((int)response_.StatusCode, headers_, responseStream_, null, response_); 
                            client_ = null; response_ = null; // response and client are disposed by FileResponse
                            return fileResponse_;
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }
            
                        return default(FileResponse);
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
        public System.Threading.Tasks.Task<FileResponse> BringbackItemAsync(int id)
        {
            return BringbackItemAsync(id, System.Threading.CancellationToken.None);
        }
    
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public async System.Threading.Tasks.Task<FileResponse> BringbackItemAsync(int id, System.Threading.CancellationToken cancellationToken)
        {
            if (id == null)
                throw new System.ArgumentNullException("id");
    
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append("api/Catalog/{id}/bringback");
            urlBuilder_.Replace("{id}", System.Uri.EscapeDataString(ConvertToString(id, System.Globalization.CultureInfo.InvariantCulture)));
    
            var client_ = _httpClient;
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    request_.Content = new System.Net.Http.StringContent(string.Empty, System.Text.Encoding.UTF8, "application/octet-stream");
                    request_.Method = new System.Net.Http.HttpMethod("POST");
                    request_.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse("application/octet-stream"));
    
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
                        if (status_ == "200" || status_ == "206") 
                        {
                            var responseStream_ = response_.Content == null ? System.IO.Stream.Null : await response_.Content.ReadAsStreamAsync().ConfigureAwait(false);
                            var fileResponse_ = new FileResponse((int)response_.StatusCode, headers_, responseStream_, null, response_); 
                            client_ = null; response_ = null; // response and client are disposed by FileResponse
                            return fileResponse_;
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = response_.Content == null ? null : await response_.Content.ReadAsStringAsync().ConfigureAwait(false); 
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", (int)response_.StatusCode, responseData_, headers_, null);
                        }
            
                        return default(FileResponse);
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
    public partial class GetProductByIdResponse 
    {
        [Newtonsoft.Json.JsonConstructor]
        public GetProductByIdResponse(string @name, int @pointsCost, string @productId, int @stockCount)
        {
            this.ProductId = @productId;
            this.Name = @name;
            this.PointsCost = @pointsCost;
            this.StockCount = @stockCount;
        }
    
        [Newtonsoft.Json.JsonProperty("productId", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string ProductId { get; }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; }
    
        [Newtonsoft.Json.JsonProperty("pointsCost", Required = Newtonsoft.Json.Required.Always)]
        public int PointsCost { get; }
    
        [Newtonsoft.Json.JsonProperty("stockCount", Required = Newtonsoft.Json.Required.Always)]
        public int StockCount { get; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    
        public static GetProductByIdResponse FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GetProductByIdResponse>(data);
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
    public partial class CreateProductCommand 
    {
        [Newtonsoft.Json.JsonConstructor]
        public CreateProductCommand(string @name, int @pointsCost)
        {
            this.Name = @name;
            this.PointsCost = @pointsCost;
        }
    
        [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Name { get; }
    
        [Newtonsoft.Json.JsonProperty("pointsCost", Required = Newtonsoft.Json.Required.Always)]
        public int PointsCost { get; }
    
        public string ToJson() 
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    
        public static CreateProductCommand FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<CreateProductCommand>(data);
        }
    
    }

    public partial class FileResponse : System.IDisposable
    {
        private System.IDisposable _client; 
        private System.IDisposable _response; 

        public int StatusCode { get; private set; }

        public System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> Headers { get; private set; }

        public System.IO.Stream Stream { get; private set; }

        public bool IsPartial
        {
            get { return StatusCode == 206; }
        }

        public FileResponse(int statusCode, System.Collections.Generic.Dictionary<string, System.Collections.Generic.IEnumerable<string>> headers, System.IO.Stream stream, System.IDisposable client, System.IDisposable response)
        {
            StatusCode = statusCode; 
            Headers = headers; 
            Stream = stream; 
            _client = client; 
            _response = response;
        }

        public void Dispose() 
        {
            if (Stream != null)
                Stream.Dispose();
            if (_response != null)
                _response.Dispose();
            if (_client != null)
                _client.Dispose();
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