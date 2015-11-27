using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quick.OwinMVC.Controller
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

        public ApiResult(Int32 code, String message)
        {
            this.Code = code;
            this.Message = message;
        }

        /// <summary>
        /// 返回成功结果
        /// </summary>
        /// <returns></returns>
        public static ApiResult Success()
        {
            return Success(null);
        }

        /// <summary>
        /// 返回成功结果
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static ApiResult Success(String message)
        {
            return new ApiResult(0, message);
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
            return Error(-1, message);
        }

        /// <summary>
        /// 返回失败结果
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        public static ApiResult Error(Int32 code, String message)
        {
            if (code == 0)
                throw new Exception("code value is '0',not means error.");
            return new ApiResult(code, message);
        }
    }
}
