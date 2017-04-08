using System;
using System.Collections.Generic;
//using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace JBookman_Conversion
{
    [Serializable]
    public class Map
    {
        //public ushort[,] m_MapSectors;
        public MapSector[,] m_MapSectors;

        public List<Door> DoorsInMap { get; set; }
        public List<Person> PeopleInMap { get; set; }
        public List<Container> ContainersInMap { get; set; }

        public ushort MapTypeId { get; set; }
        public ushort MapCols { get; set; }
        public ushort MapRows { get; set; }

        public Map()
        {
            //constructor!
            PeopleInMap = new List<Person>();
            DoorsInMap = new List<Door>();
            ContainersInMap = new List<Container>();
        }

        public void OpenMapFiles(string fName)
        {
            string mapFileName = ConstructFileName(fName);
            ReadMapFile(mapFileName);
        }

        public string ConstructFileName(string fileName)
        {
            //string newFileName = "\\Map";
            string newFileName = string.Concat("Maps\\", fileName);

            return newFileName;
        }

        public static void SaveMapFile(string fileName, Map mapToSave)
        {
            BinaryFormatter binFormatter = new BinaryFormatter();

            try
            {
                //set up stream reader and try and open the file
                Stream fileStream = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite);
                binFormatter.Serialize(fileStream, mapToSave);
                fileStream.Close();
            }
            catch (EndOfStreamException e)
            {
                //end of stream exception
                string errorString = e.ToString();
                System.Console.Write(errorString);
            }
            catch (FileNotFoundException e)
            {
                //error creating/overwriting file
                string errorString = e.ToString();
                System.Console.Write(errorString);
            }
            catch (SerializationException e)
            {
                //serialisation exception
                Console.WriteLine("Serialisation Exception" + e.ToString());
            }
        }

        //set as public static to use as a standalone method without
        //having to instantiate an object first.
        public static Map ReadMapFile(string fileName)
        {
            Map mapToLoad = new Map();

            BinaryFormatter binFormatter = new BinaryFormatter();

            try
            {
                //set up stream reader and try and open the file
                Stream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read);
                mapToLoad = (Map)binFormatter.Deserialize(fileStream);
                fileStream.Close();

            }
            catch (EndOfStreamException e)
            {
                //end of stream exception.
                //in case of this, should wack some default values into the m_People object via instantiation.
                string errorString = e.ToString();
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Person File not found exception: " + e);
                //MessageBox.Show("File not found");
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("Directory not found exception; " + e);

            }
            catch (SerializationException e)
            {
                //serialisation exception
                Console.WriteLine("Serialisation Exception" + e.ToString());
            }

            //if successful, return the CMap object

            return mapToLoad;
        }

        public string GetStringFromFile(BinaryReader streamIn, string buf)
        {
            buf = streamIn.ReadString();
            return buf;
        }

    }
}
