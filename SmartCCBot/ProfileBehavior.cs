﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace HREngine.Bots
{
    [Serializable]
    public class ProfileBehavior
    {

        public ProfileBehavior()
        {

        }

        public virtual bool ShouldPlayMoreMinions(Board board)
        {
            return true;
        }

        public virtual bool ShouldAttackWithWeapon(Board board)
        {
            return true;
        }

        public virtual bool ShouldAttackTargetWithWeapon(Card target)
        {
            return true;
        }
     
    }
}
