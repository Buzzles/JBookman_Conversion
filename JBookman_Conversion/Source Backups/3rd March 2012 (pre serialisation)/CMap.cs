using System;
using System.Collections.Generic;
//using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace JBookman_Conversion
{
    [Serializable]
    class CMap : System.Object
    {
    public ushort[,] m_MapSectors;
    
    protected CPeople m_PeopleInMap;
    protected CDoors m_DoorsInMap;
    protected CContainers m_ContainersInMap;

    public ushort m_MapTypeID;
    public ushort m_MapCols;
    public ushort m_MapRows;

    public CMap()
    {
        //constructor!

    }
    public CMap(string filename)
    {
        //call load functions to populate Containers/Doors/Sectors/People
        OpenMapFiles(filename);
                     
        //instantiate objects
        
        m_PeopleInMap = new CPeople();
        m_DoorsInMap = new CDoors();
        m_ContainersInMap = new CContainers();
    }


    //MAP HANDLING ROUTINES


    public void OpenMapFiles(string fName)
    {
        string mapFileName = ConstructFileName(fName);
        //MessageBox.Show("Mapfilename: " + mapFileName);
        ReadMapFile(mapFileName);
        ReadPeopleFile(mapFileName);
        ReadContainerFile(mapFileName);
        ReadDoorFile(mapFileName);

    }

       
    //private methods
    public string ConstructFileName(string fileName)
    {
        //string newFileName = "\\Map";
        string newFileName = string.Concat("Maps\\", fileName);

        return newFileName;
    }


    private void ReadMapFile(string fileName)
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

    public void ReadPeopleFile(string fileName)
    {
        //append .peo to fileName string to access
        //correct people file.
        string peopleFileName = string.Concat(fileName, ".peo");
        string buffer = null;
        BinaryReader binReader;
        try
        {
            //set up bin reader, try and read file
            binReader = new BinaryReader(File.Open(peopleFileName, FileMode.Open));

            //test as suggested by BinaryReader msdn article
            byte[] testArray = new byte[3];
            int count = binReader.Read(testArray, 0, 3);

            if (count != 0)
            {
                //reset reader
                binReader.BaseStream.Seek(0, SeekOrigin.Begin);

                //get number of people in file
                //buffer = GetStringFromFile(binReader, buffer);
                buffer = binReader.ReadString();
                m_PeopleInMap.SetPersonCount(int.Parse(buffer));

                for (int personCount = 0; personCount < m_PeopleInMap.GetPersonCount(); personCount++)
                {
                    CPerson newPerson = new CPerson();
                    newPerson.SetName(binReader.ReadString());
                    newPerson.SetSector(int.Parse(binReader.ReadString()));
                    newPerson.SetCanMove(bool.Parse(binReader.ReadString()));
                    newPerson.SetTile(int.Parse(binReader.ReadString()));
                    m_PeopleInMap.AddPerson(newPerson, personCount);
                }

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
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine("Directory not found exception; " + e);
        }
        //binReader.Close();

    }
    public void ReadContainerFile(string fileName)
    {
        //append .itm to map file (eg, FirstTown.map.itm)
        string containerFileName = string.Concat(fileName, ".itm");
        string buffer = null;
        BinaryReader binReader;
        try
        {
            //set up binreader, try and read file
            binReader = new BinaryReader(File.Open(containerFileName, FileMode.Open));
            //test as suggested by BinaryReader msdn article
            byte[] testArray = new byte[3];
            int count = binReader.Read(testArray, 0, 3);

            if (count != 0)
            {
                //reset reader
                binReader.BaseStream.Seek(0, SeekOrigin.Begin);

                //get number of containers in mapfile
                //buffer = GetStringFromFile(binReader, buffer);
                buffer = binReader.ReadString();
                m_ContainersInMap.SetContainerCount(int.Parse(buffer));


                for (int containerCount = 0; containerCount < m_ContainersInMap.GetContainerCount(); containerCount++)
                {
                    CContainer newContainer = new CContainer();
                    //////////////////////
                    //what's in the box?
                    ////////////////
                    //gold
                    newContainer.SetGold(int.Parse(binReader.ReadString()));
                    //keys
                    newContainer.SetKeys(int.Parse(binReader.ReadString()));
                    //potions
                    newContainer.SetPotion(int.Parse(binReader.ReadString()));
                    //armour type
                    newContainer.SetArmour(int.Parse(binReader.ReadString()));
                    //weapon type
                    newContainer.SetWeapon(int.Parse(binReader.ReadString()));
                    //locked?
                    buffer = binReader.ReadString();
                    if (buffer.Equals("F"))
                        newContainer.SetLocked(false);
                    else
                        newContainer.SetLocked(true);
                    //sector
                    newContainer.SetSector(int.Parse(binReader.ReadString()));
                    //tile numer
                    newContainer.SetTile(int.Parse(binReader.ReadString()));

                    m_ContainersInMap.AddContainer(newContainer, containerCount);
                }

            }
            binReader.Close();
        }
        catch (EndOfStreamException e)
        {
            //end of stream exception.
            //in case of this, should wack some default values into the m_Containers object via instantiation.
            string errorString = e.ToString();
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("Container File not found exception: " + e);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine("Directory not found exception; " + e);
        }

    }

    /////////////////////////////
    // Reading the door files
    ////////////////////////////
    public void ReadDoorFile(string fileName)
    {
        //append .dor to map file (eg, FirstTown.map.dor)
        string doorFileName = string.Concat(fileName, ".dor");
        string buffer = null;
        BinaryReader binReader;
        try
        {
            //set up binreader, try to read file
            binReader = new BinaryReader(File.Open(doorFileName, FileMode.Open));

            //test as suggested by BinaryReader msdn article
            byte[] testArray = new byte[3];
            int count = binReader.Read(testArray, 0, 3);

            if (count != 0)
            {
                //reset reader
                binReader.BaseStream.Seek(0, SeekOrigin.Begin);

                //get number of doors in file
                //buffer = GetStringFromFile(binReader, buffer);
                buffer = binReader.ReadString();
                m_DoorsInMap.SetDoorCount(int.Parse(buffer));

                for (int doorCount = 0; doorCount < m_DoorsInMap.GetDoorCount(); doorCount++)
                {
                    CDoor newDoor = new CDoor();
                    //secret?
                    buffer = binReader.ReadString();
                    if (buffer.Equals("F"))
                        newDoor.SetSecret(false);
                    else
                        newDoor.SetSecret(true);
                    //locked?
                    buffer = binReader.ReadString();
                    if (buffer.Equals("F"))
                        newDoor.SetLocked(false);
                    else
                        newDoor.SetLocked(true);
                    //sector
                    newDoor.SetSector(int.Parse(binReader.ReadString()));
                    //tile number
                    newDoor.SetTile(int.Parse(binReader.ReadString()));

                    m_DoorsInMap.AddDoor(newDoor, doorCount);
                }

            }
            binReader.Close();
        }
        catch (EndOfStreamException e)
        {
            //end of stream exception.
            //in case of this, should wack some default values into the m_Doors object via instantiation.
            string errorString = e.ToString();
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine("Door file not found exception: " + e);
        }
        catch (DirectoryNotFoundException e)
        {
            Console.WriteLine("Directory not found exception; " + e);
        }


    }
    /*
    public void GetStringFromFile(StreamReader streamIn, string buf)
    {

    }
    */
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
