namespace Bbt.Campaign.Public.BaseResultModels
{
    public interface IBaseResult
    {
        string ErrorMessage { get; set; }
        bool HasError { get; set; }
    }
    public interface IBaseResponse<out T> : IBaseResult
    {
        T Data { get; }
    }
}
