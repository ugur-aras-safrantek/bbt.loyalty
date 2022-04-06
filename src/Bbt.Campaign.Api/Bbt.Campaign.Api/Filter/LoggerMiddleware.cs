using Bbt.Campaign.Public.BaseResultModels;
using System.Diagnostics;

namespace Bbt.Campaign.Api.Filter
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        //private ILogService _logService;
        public LoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
           // _logService = (ILogService)context.RequestServices.GetService(typeof(ILogService));
            var requestTime = DateTime.Now;
            string requestBodyText = "";
            var token = context.Request.Headers["Authorization"].Count > 0 ? context.Request.Headers["Authorization"][0] : "";
            string path = "";
            using (MemoryStream requestBodyStream = new MemoryStream())
            {
                using (MemoryStream responseBodyStream = new MemoryStream())
                {
                    Stream originalRequestBody = context.Request.Body;
                    //context.Request.EnableRewind();
                    Stream originalResponseBody = context.Response.Body;

                    try
                    {

                        await context.Request.Body.CopyToAsync(requestBodyStream);
                        requestBodyStream.Seek(0, SeekOrigin.Begin);

                        requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();
                        path = context.Request.Path;
                        if (context.Request.QueryString.HasValue)
                        {
                            path += context.Request.QueryString.Value;
                        }

                        requestBodyStream.Seek(0, SeekOrigin.Begin);
                        context.Request.Body = requestBodyStream;

                        string responseBody = "";


                        context.Response.Body = responseBodyStream;

                        Stopwatch watch = Stopwatch.StartNew();
                        await _next(context);
                        watch.Stop();

                        responseBodyStream.Seek(0, SeekOrigin.Begin);
                        responseBody = new StreamReader(responseBodyStream).ReadToEnd();
                        responseBodyStream.Seek(0, SeekOrigin.Begin);

                        //if (!path.Contains("swagger") && path.Length > 1
                        //    //&&path.Contains("/user/kullaniciyisistemdenat")
                        //    )
                        //{
                        //    try
                        //    {
                        //        //if (path.Contains("ldap-giris"))
                        //        //{
                        //        //    var baseInput = Newtonsoft.Json.JsonConvert.DeserializeObject<PersonelGirisIstekDTO>(requestBodyText);
                        //        //    if (baseInput != null)
                        //        //        baseInput.KullaniciSifre = "";
                        //        //    requestBodyText = Newtonsoft.Json.JsonConvert.SerializeObject(baseInput);
                        //        //}
                        //        //await _logService.WriteLog(requestBodyText, responseBody, context.Connection.RemoteIpAddress.ToString(), path, context.Request.Method, watch.ElapsedMilliseconds, "", false);
                        //    }
                        //    catch (Exception)
                        //    {

                        //    }
                        //}

                        await responseBodyStream.CopyToAsync(originalResponseBody);
                    }
                    catch (Exception ex)
                    {
                        string errorCode = null;

                        if (ex.Message.Contains("Unauthorized"))
                            errorCode = "401";
                        if (ex.Message.Contains("Auth Token süresi"))
                            errorCode = "402";

                        byte[] data = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(new BaseResponse<object>()
                        {
                            ErrorMessage = $"{ex.Message}"/*:{ex.StackTrace}"*/,
                            HasError = true,
                            Data = null
                        }));
                        try
                        {
                            //if (path.Contains("ldap-giris"))
                            //{
                            //    var baseInput = Newtonsoft.Json.JsonConvert.DeserializeObject<PersonelGirisIstekDTO>(requestBodyText);
                            //    if (baseInput != null)
                            //        baseInput.KullaniciSifre = "";
                            //    requestBodyText = Newtonsoft.Json.JsonConvert.SerializeObject(baseInput);
                            //}
                            var responseBody = new StreamReader(responseBodyStream).ReadToEnd();
                            string errorMessage = $"{ex.Message} - {ex.StackTrace} - {ex?.InnerException?.Message} - {ex?.InnerException?.StackTrace}";
                            //await _logService.WriteLog(requestBodyText, responseBody, context.Connection.RemoteIpAddress.ToString(), path, context.Request.Method, 0, errorMessage, true);
                        }
                        catch (Exception)
                        {

                        }
                        await originalResponseBody.WriteAsync(data, 0, data.Length);
                    }
                    finally
                    {
                        context.Request.Body = originalRequestBody;
                        context.Response.Body = originalResponseBody;
                    }
                }
            }
        }
    }
}
