using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using TechnoRex.ResultProvider.Abstract;
using TechnoRex.ResultProvider.Types;

namespace TechnoRex.ResultProvider
{
    [Serializable()]
    [DataContract]
    public class ResultMessage : ResultContract
    {
        [XmlIgnore]
        [DataMember]
        public string Guid { get; set; }


        [DataMember]
        public MessageType MessageType { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Source { get; set; }

        [DataMember]
        public List<string> Tracker { get; set; }

        [DataMember]
        public string MessageContext { get; set; }

        [DataMember]
        public object Meta { get; set; }

        [DataMember]
        public DateTime DateTime { get; set; }


        protected ResultMessage()
        {
            Guid = System.Guid.NewGuid().ToString();
            Tracker = new List<string>();
            this.DateTime = DateTime.Now;
        }

        public ResultMessage(MessageType messageType, string message, int stackFrameLevel = 1) : this()
        {
            MessageType = messageType;
            Message = message;

            var frame = new StackFrame(stackFrameLevel, true);
            var method = frame.GetMethod();
            var type = method.DeclaringType;
            var name = method.Name;
            Source = type + "." + name + " (" + frame.GetFileLineNumber() + ")";
        }

        public ResultMessage(MessageType messageType, string message, string source) : this()
        {
            MessageType = messageType;
            Message = message;
            Source = source;
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("Message : " + Message);
            sb.Append(Environment.NewLine);
            sb.Append("Type : " + MessageType);
            sb.Append(Environment.NewLine);
            sb.Append("Date : " + this.DateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append(Environment.NewLine);
            sb.Append("Source : " + Source);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Trace :");
            sb.Append(Environment.NewLine);

            foreach (var track in Tracker)
            {
                sb.Append(track);
                sb.Append(Environment.NewLine);
            }

            sb.Append(Environment.NewLine);
            sb.Append("MessageContext : " + MessageContext);
            sb.Append(Environment.NewLine);
            sb.Append("Guid : " + Guid);
            sb.Append(Environment.NewLine);
            sb.Append("".PadRight(100, '='));


            return sb.ToString();
        }
    }
}