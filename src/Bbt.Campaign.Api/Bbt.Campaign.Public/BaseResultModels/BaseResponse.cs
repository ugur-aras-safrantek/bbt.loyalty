namespace Bbt.Campaign.Public.BaseResultModels
{
    public class BaseResponse<T> : IBaseResponse<T>
    {
        public BaseResponse()
        {
        }

        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public static new BaseResponse<T> Fail()
        {
            return new BaseResponse<T> { HasError = true };
        }

        public static new BaseResponse<T> Fail(string message)
        {
            return new BaseResponse<T> { HasError = true, ErrorMessage = message };
        }

        public static new Task<BaseResponse<T>> FailAsync()
        {
            return Task.FromResult(Fail());
        }

        public static new Task<BaseResponse<T>> FailAsync(string message)
        {
            return Task.FromResult(Fail(message));
        }

        public static new BaseResponse<T> Success()
        {
            return new BaseResponse<T> { HasError = false };
        }

        public static new BaseResponse<T> Success(string message)
        {
            return new BaseResponse<T> { HasError = false, ErrorMessage = message };
        }

        public static BaseResponse<T> Success(T data)
        {
            return new BaseResponse<T> { HasError = false, Data = data };
        }

        public static BaseResponse<T> Success(T data, string message)
        {
            return new BaseResponse<T> { HasError = false, Data = data, ErrorMessage = message };
        }

        public static new Task<BaseResponse<T>> SuccessAsync()
        {
            return Task.FromResult(Success());
        }

        public static new Task<BaseResponse<T>> SuccessAsync(string message)
        {
            return Task.FromResult(Success(message));
        }

        public static Task<BaseResponse<T>> SuccessAsync(T data)
        {
            return Task.FromResult(Success(data));
        }

        public static Task<BaseResponse<T>> SuccessAsync(T data, string message)
        {
            return Task.FromResult(Success(data, message));
        }
    }
}
