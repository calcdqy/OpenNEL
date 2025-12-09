using System;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Display;

namespace OpenNEL.Utils
{
    public static class UiLog
    {
        public static event Action<string> Logged;
        static readonly object _lock = new object();
        static readonly System.Collections.Generic.List<string> _buffer = new System.Collections.Generic.List<string>();
        class Sink : ILogEventSink
        {
            readonly MessageTemplateTextFormatter _formatter = new MessageTemplateTextFormatter("{Timestamp:HH:mm:ss} [{Level}] {Message:lj}{NewLine}{Exception}");
            public void Emit(LogEvent logEvent)
            {
                using var sw = new System.IO.StringWriter();
                _formatter.Format(logEvent, sw);
                var s = sw.ToString();
                try
                {
                    lock (_lock)
                    {
                        _buffer.Add(s);
                        if (_buffer.Count > 2000) _buffer.RemoveAt(0);
                    }
                }
                catch { }
                try { Logged?.Invoke(s); } catch { }
            }
        }
        public static ILogEventSink CreateSink() => new Sink();
        public static System.Collections.Generic.IReadOnlyList<string> GetSnapshot()
        {
            lock (_lock)
            {
                return _buffer.ToArray();
            }
        }
    }
}
