namespace Bbt.Campaign.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DoNotLogAttribute : Attribute
    {
    }
}
