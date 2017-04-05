using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JBookman_Conversion
{
    [Serializable]
   public class Person
    {
        protected string m_sName;
        protected int m_iSector, m_iTile;
        protected bool m_bCanMove;

        public Person()
        {
            m_sName = null;
            m_iSector = 0;
            m_iTile = 0;
            m_bCanMove = false;
        }
        ~Person()
        {
        }

        public void SetName(string str)
        {
            m_sName = str;
        }
        public string GetName()
        {
            return m_sName;
        }
        public void SetSector(int num)
        {
            m_iSector = num;
        }
        public int GetSector()
        {
            return m_iSector;
        }
        public void SetCanMove(bool val)
        {
            m_bCanMove = val;
        }
        public bool GetCanMove()
        {
            return m_bCanMove;
        }
        public void SetTile(int num)
        {
            m_iTile = num;
        }
        public int GetTile()
        {
            return m_iTile;
        }
    //end-class

    }
}
