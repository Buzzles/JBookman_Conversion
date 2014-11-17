using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JBookman_Conversion
{
    class CPeople
    {
        protected int m_iPersonCount;
        protected CPerson[] m_People;

        public CPeople()
        {
            m_iPersonCount = 0;
            m_People = new CPerson[Constants.MAXPEOPLE];
        }
        ~CPeople()
        { }

        public CPerson GetPerson(int num)
        {
            return m_People[num];
        }
        public void SetPersonCount(int num)
        {
            m_iPersonCount = num;
        }
        public int GetPersonCount()
        {
            return m_iPersonCount;
        }
        public void AddPerson(CPerson newPerson,int loc)
        {
            m_People[loc] = newPerson;

        }


//end-class
    }
}
