using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace TechnoRex.ResultProvider.Abstract
{
    [Serializable()]
    [DataContract]
    public class ResultContract
    {
        [XmlIgnore]
        [DataMember]
        public DateTime Time { get; set; }

        public string MessageTime
        {
            get { return Time.ToString("yyyy-MM-dd HH:mm:ss"); }
        }

        public ResultContract()
        {
            Time = DateTime.Now;
        }
    }
}
