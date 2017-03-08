using System.Runtime.Serialization;

namespace TechnoRex.ResultProvider.Types
{
    [DataContract]
    public enum MessageType
    {
        [EnumMember(Value = "Success")]
        Success = 1,
        [EnumMember(Value = "Error")]
        Error = 2,
        [EnumMember(Value = "Warning")]
        Warning = 3,
        [EnumMember(Value = "Info")]
        Info = 4
    }
}