using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using TechnoRex.ResultProvider.Types;

namespace TechnoRex.ResultProvider
{
    [Serializable]
    [DataContract]
    public class Result : EventArgs, IDisposable
    {
        public static Result New()
        {
            return new Result();
        }


        private static ResultConfig _config = new ResultConfig();
        public static ResultConfig Config { get { return _config; } }
        


        [DataMember]
        public List<ResultMessage> Messages { get; set; }

        [DataMember]
        public Dictionary<string, string> MetaData { get; set; }

        [DataMember]
        public string Info { get; set; }

        [DataMember]
        public bool IsError { get; protected set; }
        




        //ctor
        public Result()
        {
            Messages = new List<ResultMessage>();
            MetaData = new Dictionary<string, string>();
        }
        public Result(ResultConfig config):this()
        {
            Result._config = config;
        }


        public virtual Result AddMessage(string message, int stackDeep = 2)
        {
            ResultMessage dcMessage = new ResultMessage(MessageType.Info, message, stackDeep);
            SetTracker(dcMessage.Tracker, stackDeep + 1);
            Messages.Add(dcMessage);
            return this;
        }
        public virtual Result AddMessage(MessageType type, string message, int stackDeep = 2)
        {
            ResultMessage dcMessage = new ResultMessage(type, message, stackDeep);
            SetTracker(dcMessage.Tracker, stackDeep + 1);
            Messages.Add(dcMessage);
            if (type == MessageType.Error) this.IsError = true;
            return this;
        }
        public virtual Result AddMessage(ResultMessage message, int stackDeep = 3)
        {
            SetTracker(message.Tracker, stackDeep);
            Messages.Add(message);
            if (message.MessageType == MessageType.Error) this.IsError = true;
            return this;
        }


        public virtual Result AddMessages(IEnumerable<ResultMessage> messages, int stackDeep = 2)
        {
            foreach (var message in messages)
                AddMessage(message, stackDeep);

            return this;
        }
        public virtual Result AddMessagesFrom(Result otherServerResponse, int stackDeep = 2)
        {
            AddMessages(otherServerResponse.Messages, stackDeep);
            return this;
        }
        public virtual Result AddException(Exception exc, int stackDeep = 3)
        {
            ResultMessage message = new ResultMessage(MessageType.Error, exc.ToString(), stackDeep - 1);
            SetTracker(message.Tracker, stackDeep);
            Messages.Add(message);
            this.IsError = true;
            return this;
        }
        public virtual Result AddError(string error, int stackDeep = 2)
        {
            ResultMessage dcMessage = new ResultMessage(MessageType.Error, error, stackDeep);
            SetTracker(dcMessage.Tracker, stackDeep + 1);
            Messages.Add(dcMessage);
            this.IsError = true;
            return this;
        }

        public virtual Result ResponseTo(Result serverResponse)
        {
            return serverResponse.AddMessagesFrom(this);
        }


        public virtual Result Save(string path)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                this.Messages.ForEach(x =>
                {
                    sb.Append(x.ToString());
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                });
                File.WriteAllText(path, sb.ToString());
            }
            catch (Exception exc)
            {
                this.AddException(exc);
            }

            return this;
        }
        public virtual Result SaveLog(string name = null, string dir = null)
        {
            try
            {
                if (_config.LogOnlyWhenError && IsError == false) return this;
                if (name == null)
                {
                    var frameOperIn = new StackFrame(1);
                    //var frameOperOut = new StackFrame(2);
                    name = frameOperIn.GetMethod().DeclaringType + "." + frameOperIn.GetMethod().Name;
                }

                if (dir == null)
                {
                    if (string.IsNullOrEmpty(_config.GetProperLogDirPath())) return this;
                    dir = _config.GetProperLogDirPath();
                }

                if (string.IsNullOrEmpty(dir)) return this;
                if (Directory.Exists(dir) == false)
                    Directory.CreateDirectory(dir);
                Save(Path.Combine(dir, DateTime.Now.ToString("yyyyMMddHHmmssfff") + "." + name + ".txt"));
            }
            catch (Exception exc)
            {
                this.AddException(exc);
            }

            return this;
        }


        public virtual Result Execute(Action func)
        {
            try
            {
                func();
            }
            catch (Exception exc)
            {
                this.AddMessage(new ResultMessage(MessageType.Error, exc.ToString()), 0);
            }
            return this;
        }
        public virtual Result SaveLogIfError()
        {
            if (IsError) SaveLog();
            return this;
        }


        protected void SetTracker(List<string> tracker, int sourceDeep = 3)
        {
            if (_config.NoTrace) return;
            string source = "";
            int deep = sourceDeep;
            while (true)
            {
                source = GetMethodSource(deep, deep + 1);
                if (source == null) break;
                if (source.Contains("System.Runtime")) break;
                if (source.Contains("System.AppDomain")) break;
                if (source.Contains("Innviero.ResponseProvider")) break;

                tracker.Add(source);
                deep++;
            }
        }
        protected string GetMethodSource(int frameInDeep = 2, int frameOutDeep = 3)
        {
            int deep = new StackTrace().GetFrames().Length;
            if (deep <= frameOutDeep) return null;




            var frameOperIn = new StackFrame(frameInDeep, true);
            var frameOperOut = new StackFrame(frameOutDeep, true);
            return "[" + frameOperOut.GetMethod().DeclaringType + "." + frameOperOut.GetMethod().Name + " => " +
                   frameOperIn.GetMethod().Name + " (" + frameOperIn.GetFileLineNumber() + ")]";
        }


        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


    }

    [Serializable]
    [DataContract]
    public class Result<T> : Result
    {
        [DataMember]
        public T Object { get; protected set; }

        public Result<T> AddMessage(string message, int stackDeep = 3)
        {
            base.AddMessage(message, stackDeep);
            return this;
        }
        public Result<T> AddMessage(MessageType type, string message, int stackDeep = 3)
        {
            base.AddMessage(type, message, stackDeep);
            return this;
        }
        public Result<T> AddMessage(ResultMessage message, int stackDeep = 4)
        {
            base.AddMessage(message, stackDeep);
            return this;
        }

        public Result<T> AddMessages(IEnumerable<ResultMessage> messages, int stackDeep = 4)
        {
            foreach (var message in messages)
                base.AddMessage(message, stackDeep);
            return this;
        }

        public Result<T> AddMessagesFrom(Result otherServerResponse, int stackDeep = 3)
        {
            base.AddMessages(otherServerResponse.Messages, stackDeep);
            return this;
        }
        public Result<T> AddMessagesFrom(Result<T> otherServerResponse, int stackDeep = 3)
        {
            this.Object = otherServerResponse.Object;
            base.AddMessages(otherServerResponse.Messages, stackDeep);
            return this;
        }
        public Result<T> AddException(Exception exc, int stackDeep = 4)
        {
            base.AddException(exc, stackDeep);
            return this;
        }
        public Result<T> AddError(string error, int stackDeep = 3)
        {
            base.AddError(error, stackDeep);
            return this;
        }
        public Result<T> SetObject(T obj)
        {
            this.Object = obj;
            return this;
        }
      

        public Result<T> ResponseTo(Result<T> serverResponse, int stackDeep = 3)
        {
            serverResponse.AddMessagesFrom(this, stackDeep);
            return serverResponse;
        }


        public Result<T> Save(string path)
        {
            base.Save(path);
            return this;
        }
        public Result<T> SaveLog(string name = null, string dir = null)
        {
            base.SaveLog(name, dir);
            return this;
        }


        public static Result<T> New()
        {
            return new Result<T>();
        }


        public static implicit operator T (Result<T> response)
        {
            return response.Object;
        }


    }

}
