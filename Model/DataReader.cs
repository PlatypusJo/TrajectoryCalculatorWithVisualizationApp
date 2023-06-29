using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace TrajectoryOfSensorVisualization.Model
{
    /// <summary>
    /// Статический класс для считывания данных из файла
    /// </summary>
    public static class DataReader
    {
        #region Methods to read vectors
        /// <summary>
        /// Функция для считывания информации о векторах ускорений, векторах угловых скоростей и кватернионах поворота
        /// </summary>
        /// <param name="filePath">Путь к файлу, из которого идёт считывание</param>
        /// <param name="accVectors">Список векторов ускорений</param>
        /// <param name="gyrVectors">Список векторов угловых скоростей</param>
        /// <param name="quaternions">Список кватернионов</param>
        /// <exception cref="Exception">Исключение при чтении файла или конвертации строк</exception>
        public static void ReadVectorsAndQuaternionsFromFile(string filePath, ref List<Vector3D> accVectors, ref List<Vector3D> gyrVectors, ref List<Quaternion> quaternions)
        {
            try
            {
                using StreamReader reader = new(@$"{filePath}");
                reader.ReadLine();
                string[] headerLine = reader.ReadLine().ToString().Replace('.', ',').Split(';', StringSplitOptions.None);
                int calibCount = Convert.ToInt32(headerLine[1]);
                for (int i = 0; i < calibCount + 3; i++)
                {
                    reader.ReadLine();
                }
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] separators = new string[] { ";" };
                    line = line.Replace('.', ',');
                    string[] data = line.Split(separators, StringSplitOptions.None);
                    accVectors.Add(new() { X = Convert.ToDouble(data[0]), Y = Convert.ToDouble(data[1]), Z = Convert.ToDouble(data[2]) });
                    gyrVectors.Add(new() { X = Convert.ToDouble(data[3]), Y = Convert.ToDouble(data[4]), Z = Convert.ToDouble(data[5]) });
                    quaternions.Add(new() { X = Convert.ToDouble(data[7]), Y = Convert.ToDouble(data[8]), Z = Convert.ToDouble(data[9]), W = Convert.ToDouble(data[6]) });
                }
            }
            catch
            {
                throw new Exception();
            }
        }
        /// <summary>
        /// Функция для считывания векторов ускорений из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу, из которого идёт считывание</param>
        /// <returns>Список векторов ускорений</returns>
        /// <exception cref="Exception">Исключение при чтении файла или конвертации строк</exception>
        public static List<Vector3D> ReadAccVectorsFromFile(string filePath)
        {
            try 
            { 
                using StreamReader reader = new(@$"{filePath}");
                List<Vector3D> accVectors = new();
                reader.ReadLine();
                string[] headerLine = reader.ReadLine().ToString().Replace('.', ',').Split(';', StringSplitOptions.None);
                int calibCount = Convert.ToInt32(headerLine[1]);
                for (int i = 0; i < calibCount + 3; i++)
                {
                    reader.ReadLine();
                }
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] separators = new string[] { ";" };
                    line = line.Replace('.', ',');
                    string[] data = line.Split(separators, StringSplitOptions.None);
                    accVectors.Add(new() { X = Convert.ToDouble(data[0]), Y = Convert.ToDouble(data[1]), Z = Convert.ToDouble(data[2]) });
                }
                return accVectors;
            }
            catch
            {
                throw new Exception();
            }
        }
        /// <summary>
        /// Функция для считывания векторов угловой скорости из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу, из которого идёт считывание</param>
        /// <returns>Список векторов угловой скорости</returns>
        /// <exception cref="Exception">Исключение при чтении файла или конвертации строк</exception>
        public static List<Vector3D> ReadGyrVectorsFromFile(string filePath)
        {
            try
            {
                using StreamReader reader = new(@$"{filePath}");
                List<Vector3D> gyrVectors = new();
                reader.ReadLine();
                string[] headerLine = reader.ReadLine().ToString().Replace('.', ',').Split(';', StringSplitOptions.None);
                int calibCount = Convert.ToInt32(headerLine[1]);
                for (int i = 0; i < calibCount + 3; i++)
                {
                    reader.ReadLine();
                }
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] separators = new string[] { ";" };
                    line = line.Replace('.', ',');
                    string[] data = line.Split(separators, StringSplitOptions.None);
                    gyrVectors.Add(new() { X = Convert.ToDouble(data[3]), Y = Convert.ToDouble(data[4]), Z = Convert.ToDouble(data[5]) });
                }
                return gyrVectors;
            }
            catch
            {
                throw new Exception();
            }
            
        }
        /// <summary>
        /// Функция для считывания кватернионов из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу, из которого идёт считывание</param>
        /// <returns>Список кватернионов</returns>
        /// <exception cref="Exception">Исключение при чтении файла или конвертации строк</exception>
        public static List<Quaternion> ReadQuaternionsFromFile(string filePath)
        {
            try
            {
                using StreamReader reader = new(@$"{filePath}");
                List<Quaternion> quaternions = new();
                reader.ReadLine();
                string[] headerLine = reader.ReadLine().ToString().Replace('.', ',').Split(';', StringSplitOptions.None);
                int calibCount = Convert.ToInt32(headerLine[1]);
                for (int i = 0; i < calibCount + 3; i++)
                {
                    reader.ReadLine();
                }
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] separators = new string[] { ";" };
                    line = line.Replace('.', ',');
                    string[] data = line.Split(separators, StringSplitOptions.None);
                    quaternions.Add(new() { X = Convert.ToDouble(data[7]), Y = Convert.ToDouble(data[8]), Z = Convert.ToDouble(data[9]), W = Convert.ToDouble(data[6]) });
                }
                return quaternions;
            }
            catch
            {
                throw new Exception();
            }
            
        }
        #endregion

        #region Methods to read header data
        /// <summary>
        /// Функция для считывания заголовочной информацию из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу, из которого идёт считывание</param>
        /// <param name="sampleFreq">Частота отсчётов</param>
        /// <param name="calibCount">Количество отчётов на калибровку</param>
        /// <param name="gVector">Вектор ускорения свободного падения g</param>
        /// <param name="gVectorLength">Длина вектора ускорения свободного падения g</param>
        /// <param name="calibQuaternion">Калибровочный кватернион</param>
        /// <exception cref="Exception">Исключение при чтении файла или конвертации строк</exception>
        public static void ReadHeaderInformation(string filePath, ref int sampleFreq, ref int calibCount, ref Vector3D gVector, ref double gVectorLength, ref Quaternion calibQuaternion)
        {
            try
            {
                using StreamReader reader = new(@$"{filePath}");
                reader.ReadLine();
                string[] headerLine = reader.ReadLine().ToString().Replace('.', ',').Split(';', StringSplitOptions.None);
                sampleFreq = Convert.ToInt32(headerLine[0]);
                calibCount = Convert.ToInt32(headerLine[1]);
                gVector = new() { X = Convert.ToDouble(headerLine[2]), Y = Convert.ToDouble(headerLine[3]), Z = Convert.ToDouble(headerLine[4]) };
                gVectorLength = Convert.ToDouble(headerLine[5]);
                calibQuaternion = new() { X = Convert.ToDouble(headerLine[7]), Y = Convert.ToDouble(headerLine[8]), Z = Convert.ToDouble(headerLine[9]), W = Convert.ToDouble(headerLine[6]) };
            }
            catch
            {
                throw new Exception();
            }
           
        }
        /// <summary>
        /// Функция для считывания кол-ва отсчётов в секунду из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу, из которого идёт считывание</param>
        /// <returns>Кол-во отсчётов в секунду</returns>
        /// <exception cref="Exception">Исключение при чтении файла или конвертации строк</exception>
        public static int ReadSampleFreqFromFile(string filePath)
        {
            try
            {
                using StreamReader reader = new(@$"{filePath}");
                reader.ReadLine();
                string[] headerLine = reader.ReadLine().ToString().Replace('.', ',').Split(';', StringSplitOptions.None);
                int sampleFreq = Convert.ToInt32(headerLine[0]);
                return sampleFreq;
            }
            catch
            {
                throw new Exception();
            }
            
        }
        /// <summary>
        /// Функция для считывания кол-ва отсчётов на калибровку из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу, из которого идёт считывание</param>
        /// <returns>Кол-во отсчётов на калибровку</returns>
        /// <exception cref="Exception">Исключение при чтении файла или конвертации строк</exception>
        public static int ReadCalibCountFromFile(string filePath)
        {
            try
            {
                using StreamReader reader = new(@$"{filePath}");
                reader.ReadLine();
                string[] headerLine = reader.ReadLine().ToString().Replace('.', ',').Split(';', StringSplitOptions.None);
                int calibCount = Convert.ToInt32(headerLine[1]);
                return calibCount;
            }
            catch
            {
                throw new Exception();
            }
            
        }
        /// <summary>
        /// Функция для считывания вектора ускорения свободного падения из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу, из которого идёт считывание</param>
        /// <returns>Вектор ускорения свободного падения</returns>
        /// <exception cref="Exception">Исключение при чтении файла или конвертации строк</exception>
        public static Vector3D ReadGVectorFromFile(string filePath)
        {
            try
            {
                using StreamReader reader = new(@$"{filePath}");
                reader.ReadLine();
                string[] headerLine = reader.ReadLine().ToString().Replace('.', ',').Split(';', StringSplitOptions.None);
                Vector3D gVector = new() { X = Convert.ToDouble(headerLine[2]), Y = Convert.ToDouble(headerLine[3]), Z = Convert.ToDouble(headerLine[4]) };
                return gVector;
            }
            catch
            {
                throw new Exception();
            }
            
        }
        /// <summary>
        /// Функция для считывания длины вектора ускорения свободного падения из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу, из которого идёт считывание</param>
        /// <returns>Длину вектора ускорения свободного падения</returns>
        /// <exception cref="Exception">Исключение при чтении файла или конвертации строк</exception>
        public static double ReadGVectorLengthFromFile(string filePath)
        {
            try
            {
                using StreamReader reader = new(@$"{filePath}");
                reader.ReadLine();
                string[] headerLine = reader.ReadLine().ToString().Replace('.', ',').Split(';', StringSplitOptions.None);
                double gVectorLength = Convert.ToDouble(headerLine[5]);
                return gVectorLength;
            }
            catch
            {
                throw new Exception();
            }
            
        }
        /// <summary>
        /// Функция для считывания калибровочного кватерниона из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу, из которого идёт считывание</param>
        /// <returns>Калибровочный кватернион</returns>
        /// <exception cref="Exception">Исключение при чтении файла или конвертации строк</exception>
        public static Quaternion ReadCalibQuaternionFromFile(string filePath)
        {
            try
            {
                using StreamReader reader = new(@$"{filePath}");
                reader.ReadLine();
                string[] headerLine = reader.ReadLine().ToString().Replace('.', ',').Split(';', StringSplitOptions.None);
                Quaternion calibQuaternion = new() { X = Convert.ToDouble(headerLine[7]), Y = Convert.ToDouble(headerLine[8]), Z = Convert.ToDouble(headerLine[9]), W = Convert.ToDouble(headerLine[6]) };
                return calibQuaternion;
            }
            catch
            {
                throw new Exception();
            }
            
        }
        #endregion
    }
}
