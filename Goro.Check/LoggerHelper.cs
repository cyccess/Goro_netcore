using log4net;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Goro.Check
{

    /// <summary>
    /// 日志接口
    /// </summary>
    public interface ILogger
    {
        void Error<T>(object message);
        void Info<T>(object message);
        void Warning<T>(object message);
        void Debug<T>(object message);
    }

    /// <summary>
    /// 默认实现（log4）
    /// </summary>
    public class Log4Helper : ILogger
    {
        private static readonly log4net.ILog log4;
        static Log4Helper()
        {
            ILoggerRepository repository = LogManager.CreateRepository("Xnx");
            XmlConfigurator.Configure(repository, new FileInfo(System.Environment.CurrentDirectory + "/log4net.config"));
            log4 = LogManager.GetLogger(repository.Name, "Sms");
        }

        public void Error<T>(object message)
        {
            //log.LogError(message.ToString());
            log4.Error(message);
        }

        public void Info<T>(object message)
        {
            //log.LogInformation(message.ToString());
            log4.Info(message);
        }


        public void Warning<T>(object message)
        {
            //log.LogWarning(message.ToString());
            log4.Warn(message);
        }

        public void Debug<T>(object message)
        {
            //log.LogDebug(message.ToString());
            log4.Debug(message);
        }
    }






    /// <summary>
    /// 提供通用logger服务 默认实现（log4）
    /// </summary>
    public class LoggerHelper
    {

        private static ILogger _logger = new Log4Helper();

        private static object _locker = new object();

        public static void SetLogger(ILogger logger)
        {

            lock (_locker)
            {
                _logger = logger;
            }
        }


        public static void Error(object message)
        {
            _logger.Error<LoggerHelper>(message);
        }

        public static void Info(object message)
        {
            _logger.Info<LoggerHelper>(message);
        }


        public static void Warning(object message)
        {
            _logger.Warning<LoggerHelper>(message);
        }

        public static void Debug(object message)
        {
            _logger.Debug<LoggerHelper>(message);
        }

        public static void Error<T>(object message)
        {
            _logger.Error<LoggerHelper>(message);
        }

        public static void Info<T>(object message)
        {
            _logger.Info<LoggerHelper>(message);
        }


        public static void Warning<T>(object message)
        {
            _logger.Warning<T>(message.ToString());
        }

        public static void Debug<T>(object message)
        {
            _logger.Debug<T>(message);
        }

    }
    
}
