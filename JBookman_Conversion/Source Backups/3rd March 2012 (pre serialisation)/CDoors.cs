using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JBookman_Conversion
{
    class CDoors
    {
        protected int m_iDoorCount;
        protected CDoor[] m_Doors;

        public CDoors()
        {
            m_iDoorCount = 0;
            m_Doors = new CDoor[Constants.MAXDOORS];
        }
        ~CDoors()
        { 
        }

        public CDoor GetDoor(int num)
        {
            return m_Doors[num];
        }
        public void SetDoorCount(int num)
        {
            m_iDoorCount = num;
        }
        public int GetDoorCount()
        {
            return m_iDoorCount;
        }
        public void AddDoor(CDoor newDoor, int pos)
        {
            m_Doors[pos] = newDoor;
        }

    }
}
