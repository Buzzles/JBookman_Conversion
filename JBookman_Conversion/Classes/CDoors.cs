using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JBookman_Conversion
{
    [Serializable]
    public class CDoors
    {
        
        protected int m_iDoorCount;
       // protected CDoor[] m_Doors;
        protected List<CDoor> m_Doors;

        public CDoors()
        {
            m_iDoorCount = 0;
          //  m_Doors = new CDoor[Constants.MAXDOORS];
            m_Doors = new List<CDoor>();
        }
        ~CDoors()
        { 
        }

        public CDoor GetDoor(int num)
        {
           return m_Doors[num];
        }
        
        public int GetDoorCount()
        {
            //return m_iDoorCount;
            return m_Doors.Count;
        }
       
        public void AddDoor(CDoor newDoor)
        {
            //m_Doors[pos] = newDoor;
            m_Doors.Add(newDoor);
        }


        //old methods, using array m_Doors[]
        /*
         public void AddDoor(CDoor newDoor, int pos) <-- Old, used when using CDoor[] array
         {
         * m_Doors[pos] = newDoor;
         * }
        public void SetDoorCount(int num)
        {
            m_iDoorCount = num;
        }*/
         
               
    }
}
