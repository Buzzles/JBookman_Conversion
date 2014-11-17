using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JBookman_Conversion
{

    //class CContainers : CContainer
    class CContainers
    {
        protected int m_iContainerCount;
        protected CContainer[] m_Containers;

        //constructor/deconstructor
        public CContainers()
        {
            m_Containers = new CContainer[Constants.MAXCONTAINERS];
            m_iContainerCount = 0;
        }
        ~CContainers()
        {
        }

        public CContainer GetContainer(int num)
        {
            return m_Containers[num];
        }
        public void SetContainerCount(int count)
        {
            m_iContainerCount = count;
        }
        public int GetContainerCount()
        {
            return m_iContainerCount;
        }
        public void AddContainer(CContainer newContainer, int pos)
        {
            m_Containers[pos] = newContainer;

        }


    //end-class
    }
}
