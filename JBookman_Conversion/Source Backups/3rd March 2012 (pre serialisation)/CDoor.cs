using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JBookman_Conversion
{
    class CDoor
    {
        protected bool m_bSecret, m_bLocked;
        protected int m_iSector, m_iTile;

        public CDoor()
        {
            m_bSecret = false;
            m_bLocked = false;
            m_iSector = 0;
            m_iTile = 0;
        }
        ~CDoor()
        {
        }

        //secret mutators
        public void SetSecret(bool val)
        {
            m_bSecret = val;
        }
        public bool GetSecret()
        {
            return m_bSecret;
        }
        //lock state mutators
        public void SetLocked(bool val)
        {
            m_bLocked = val;
        }
        public bool GetLocked()
        {
            return m_bLocked;
        }
        //sector (location) mutators
        public void SetSector(int num)
        {
            m_iSector = num;
        }
        public int GetSector()
        {
            return m_iSector;
        }
        //tile (image) value
        public void SetTile(int num)
        {
            m_iTile = num;
        }
        public int GetTile()
        {
            return m_iTile;
        }
    //class-end
    }
}
