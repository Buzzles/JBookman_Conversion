using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JBookman_Conversion
{
    [Serializable]
    public class Container
    {
    protected int m_iGold, 
                  m_iKeys,
                  m_iPotion,
                  m_iArmour,
                  m_iWeapon,
                  m_iSector,
                  m_iTile;
        
    protected bool m_bLocked;


    //constructors/deconstructors
    public Container()
    {
        m_iGold = 0;
        m_iKeys = 0;
        m_iPotion = 0;
        m_iArmour = 0;
        m_iWeapon = 0;
        m_bLocked = false;
        m_iSector = 0;
        m_iTile = 0;
    }

    ~Container()
    {
    }
    //Gold access modifiers
    public void SetGold(int num)
    {
        m_iGold = num;
    }
    public int GetGold()
    {
        return m_iGold;
    }

    //m_iKeys access
    public void SetKeys(int num)
    {
        m_iKeys = num;
    }
    public int GetKeys(int num)
    {
        return m_iKeys;
    }
    //potion mutators
    public void SetPotion(int num)
    {
        m_iPotion = num;
    }
    public int GetPosition(int num)
    {
        return m_iPotion;
    }
    //armour mutators
    public void SetArmour(int num)
    {
        m_iArmour = num;
    }
    public int GetArmour()
    {
        return m_iArmour;
    }
    //weapon mutators
    public void SetWeapon(int num)
    {
        m_iWeapon = num;
    }
    public int GetWeapon()
    {
        return m_iWeapon;
    }
    //locked mutators
    public void SetLocked(bool val)
    {
        m_bLocked = val;
    }
    public bool GetLocked()
    {
        return m_bLocked;
    }
    //position modifiers
    public void SetSector(int val)
    {
        m_iSector = val;
    }
    public int GetSector()
    {
        return m_iSector;
    }
    //tile mutators
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
//end-namespace
}
