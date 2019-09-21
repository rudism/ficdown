namespace Ficdown.Parser
{
    using System;
    using System.Collections.Generic;

    public class Logger
    {
        private static bool _initialized = false;
        private static bool _debug = false;
        private static Dictionary<Type, Logger> _cache;

        public Type Type { get; private set; }

        private Logger(Type type)
        {
            Type = type;
        }

        public static void Initialize(bool debug)
        {
            _debug = debug;
            _cache = new Dictionary<Type, Logger>();
            _initialized = true;
        }

        public static Logger GetLogger<T>()
        {
            var type = typeof(T);
            lock(_cache)
            {
                if(!_cache.ContainsKey(type))
                    _cache.Add(type, new Logger(type));
            }
            return _cache[type];
        }

        private string Decorate(string message)
        {
            return $"{DateTime.Now.ToString("")} <{Type.Name}> {message}";
        }

        public void Raw(string message)
        {
            Console.WriteLine(message);
        }

        public void Log(string message)
        {
            Raw(Decorate(message));
        }

        public void Debug(string message)
        {
            if(!_debug) return;
            Log($"DEBUG: {message}");
        }

        public void Error(string message, Exception ex = null)
        {
            Console.Error.WriteLine(Decorate($"ERROR: {message}"));
        }
    }
}
