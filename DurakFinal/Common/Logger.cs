using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Durak.Common
{
    // Утиліта для запису інформації в текстовий файл
    public class Logger : TextWriter
    {
        #region Class-Level

        // Отримує формат часу для позначок часу журналу
        public static readonly string LOG_TIME_FORMAT = "HH:mm:ss";
        // Отримує формат дати для іменування файлу журналу
        public static readonly string LOG_FILE_TIME_FORMAT = "yyyy-MM-dd_HH_mm";

        // Зберігає єдиний екземпляр реєстратора для статичного використання
        private static Logger mySingleton;

        // Отримує єдиний екземпляр реєстратора, створений під час виклику або посилання на будь-яку функцію реєстратора
        public static Logger Singleton
        {
            get { return mySingleton; }
        }

        // Статичний конструктор, створює новий екземпляр реєстратора
        static Logger()
        {
            mySingleton = new Logger();
        }

        #endregion

        #region Instance-Level

        // Потік для запису
        private StreamWriter myStream;

        // Створює новий екземпляр реєстратора
        public Logger()
        {
            bool didOpen = false;
            int index = 0;

            while (!didOpen)
            {
                try
                {
                    string fileName = "log_" + DateTime.Now.ToString(LOG_FILE_TIME_FORMAT) + (index > 0 ? "(" + index + ")" : "") + ".txt";
                    myStream = new StreamWriter(fileName);
                    didOpen = true;
                }
                catch (IOException) { index++; }
            }
        }

        new private void WriteLine(string line)
        {
            myStream.WriteLine(line);
            mySingleton.myStream.Flush();
        }

        // Записує виключення в журнал
        public static void Write(Exception e)
        {
            Write("Encounted {0} at:", e.GetType().Name);

            StackTrace trace = new StackTrace(e, true);

            foreach (StackFrame frame in trace.GetFrames())
            {
                Write("\t{0}.{1} - line {2}", frame.GetMethod().DeclaringType, frame.GetMethod().Name, frame.GetFileLineNumber());
            }
        }

        // Записує один рядок неформатованого тексту до файлу журналу
        new public static void Write(string rawText)
        {
            mySingleton.WriteLine(rawText);
        }

        // Записує один рядок відформатованого тексту до файлу журналу
        new public static void Write(string format, params object[] parameters)
        {
            mySingleton.WriteLine(string.Format("[{0}] {1}", DateTime.Now.ToString(LOG_TIME_FORMAT), string.Format(format, parameters)));
        }

        // Отримує кодування для цього реєстратора
        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
        
        #endregion
    }
}
