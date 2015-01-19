using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JBookman_Conversion
{
    [Serializable]
    public class Player
    {
        protected int m_iSector,
                      m_iHitPoints,
                      m_iMaxHitPoints,
                      m_iArmour,
                      m_iWeapon,
                      m_iGold,
                      m_iKeys,
                      m_iPotions,
                      m_iExperience;

        public Player()
    {
        m_iSector = 0;
        m_iHitPoints = 0;
        m_iMaxHitPoints = 0;
        m_iArmour = 0;
        m_iWeapon = 0;
        m_iGold = 0;
        m_iKeys = 0;
        m_iPotions = 0;
        m_iExperience = 0;
        
    }

    ~Player()
    {
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

    //current hitpoint mutator
    public void SetHitPoints(int num)
    {
        m_iHitPoints = num;
    }
    public int GetHitPoints()
    {
        return m_iHitPoints;
    }

    //max hit points mutator
    public void SetMaxHitPoints(int num)
    {
        m_iMaxHitPoints = num;
    }
    public int GetMaxHitPoints()
    {
        return m_iMaxHitPoints;
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
        m_iPotions = num;
    }
    public int GetPotions(int num)
    {
        return m_iPotions;
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
    
    // experience mutators
    public void SetExperience(int num)
    {
        m_iExperience = num;
    }
    public int GetExperience()
    {
        return m_iExperience;
    }

//end-class
    }
}
