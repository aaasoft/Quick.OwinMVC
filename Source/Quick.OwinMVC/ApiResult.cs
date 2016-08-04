using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.OwinMVC
{
    /// <summary>
    /// WEB API的返回结果
    /// </summary>
    public class ApiResult
    {
        [JsonProperty("code")]
        public Int32 Code { get; set; }
        [JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
        public String Message { get; set; }
        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public Object Data { get; set; }

        public ApiResult(Int32 code, String message, Object data)
        {
            this.Code = code;
            this.Message = message;
            this.Data = data;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// 返回成功结果
        /// </summary>
        /// <returns></returns>
        public static ApiResult Success()
        {
            return Success(null, null);
        }

        public static ApiResult Success(Object data)
        {
            return Success(null, data);
        }

        public static ApiResult Success(String message)
        {
            return Success(message, null);
        }

        /// <summary>
        /// 返回成功结果
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static ApiResult Success(String message, Object data)
        {
            return new ApiResult(0, message, data);
        }

        /// <summary>
        /// 返回失败结果
        /// </summary>
        /// <returns></returns>
        public static ApiResult Error()
        {
            return Error(null);
        }

        /// <summary>
        /// 返回失败结果
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static ApiResult Error(String message)
        {
            return Error(-1, message, null);
        }

        /// <summary>
        /// 返回失败的结果
        /// </summary>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApiResult Error(String message, Object data)
        {
            return Error(-1, message, data);
        }

        /// <summary>
        /// 返回失败的结果
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static ApiResult Error(Object data)
        {
            return Error(-1, null, data);
        }

        /// <summary>
        /// 返回失败的结果
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResult Error(Int32 code, String message)
        {
            return Error(code, message, null);
        }

        /// <summary>
        /// 返回失败结果
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static ApiResult Error(Int32 code, String message,Object data)
        {
            if (code == 0)
                throw new Exception("code value is '0',not means error.");
            return new ApiResult(code, message,data);
        }
    }
}
