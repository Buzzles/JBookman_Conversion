using System;
using System.Collections.Generic;
//using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace JBookman_Conversion
{
    [Serializable]
    public class CMap : System.Object
    {
    //public ushort[,] m_MapSectors;
    public CMapSector[,] m_MapSectors;

    protected CPeople m_PeopleInMap;
    protected CDoors m_DoorsInMap;
    public CContainers m_ContainersInMap;

    public ushort m_MapTypeID;
    public ushort m_MapCols;
    public ushort m_MapRows;

    public CMap()
    {
        //constructor!
        m_PeopleInMap = new CPeople();
        m_DoorsInMap = new CDoors();
        m_ContainersInMap = new CContainers();
        

    }
    
    /*public CMap(string filename)
    {
        //instantiate objects

        m_PeopleInMap = new CPeople();
        m_DoorsInMap = new CDoors();
        m_ContainersInMap = new CContainers();
        
        //call load functions to populate Containers/Doors/Sectors/People
        OpenMapFiles(filename);
                     
        
    }*/


    //MAP HANDLING ROUTINES


  /*  public void OldOpenMapFiles(string fName)
    {
        string mapFileName = ConstructFileName(fName);
        //MessageBox.Show("Mapfilename: " + mapFileName);
        ReadMapFile(mapFileName);
        ReadPeopleFile(mapFileName);
        ReadContainerFile(mapFileName);
        ReadDoorFile(mapFileName);

    }*/

    public void OpenMapFiles(string fName)
    {
        string mapFileName = ConstructFileName(fName);
        //MessageBox.Show("Mapfilename: " + mapFileName);
        ReadMapFile(mapFileName);
       // ReadPeopleFile(mapFileName);
       // ReadContainerFile(mapFileName);
       // ReadDoorFile(mapFileName);

    }

       
    //private methods
    public string ConstructFileName(string fileName)
    {
        //string newFileName = "\\Map";
        string newFileName = string.Concat("Maps\\", fileName);

        return newFileName;
    }


   /* private void OldReadMapFile(string fileName)
    {

        BinaryReader binReader;
        try
        {
            //set up bin reader and try and open the file
            binReader = new BinaryReader(File.Open(fileName, FileMode.Open));

            //test as suggested by BinaryReader msdn article
            byte[] testArray = new byte[3];
            int count = binReader.Read(testArray, 0, 3);

            if (count != 0)
            {
                //reset reader
                binReader.BaseStream.Seek(0, SeekOrigin.Begin);


                //get map type first
                m_MapTypeID = binReader.ReadUInt16();

                //read columns
                m_MapCols = binReader.ReadUInt16();

                //read rows
                m_MapRows = binReader.ReadUInt16();

                m_MapSectors = new ushort[m_MapRows, m_MapCols];

                //read sectors
                for (short y = 0; y < m_MapRows; y++)
                {
                    for (short x = 0; x < m_MapCols; x++)
                    {
                        ushort currentSector = 0;
                        currentSector = binReader.ReadUInt16();
                        //map_Sectors[y][x] = currentSector;
                        m_MapSectors[y, x] = currentSector;
                    }

                }

               // MessageBox.Show("Map loaded");
            }
            binReader.Close();
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
        //binReader.Close();

    }
        */

    public static void SaveMapFile(string fileName, CMap mapToSave)
    {
       // CMap mapToSave = this;

        BinaryFormatter binFormatter = new BinaryFormatter();

        try
        {
            //set up stream reader and try and open the file
            Stream fileStream = File.Open(fileName, FileMode.Create,FileAccess.ReadWrite);
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
    public static CMap ReadMapFile(string fileName)
    {
        CMap mapToLoad = new CMap();

        BinaryFormatter binFormatter = new BinaryFormatter();

        try
        {
            //set up stream reader and try and open the file
            Stream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read);
            mapToLoad = (CMap)binFormatter.Deserialize(fileStream);
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
        /* while (streamIn.PeekChar() != 13 && streamIn.PeekChar() != -1)
         {
             buf += streamIn.ReadChar();

         }*/

        buf = streamIn.ReadString();
        return buf;
    }





    }
}
