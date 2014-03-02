using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin
{
    public class CardDetails
    {
        //BCCanHeal: Si el Battlecry puede curar y cuanto.
        public bool BCHeal;
        public int BCHealAmount;
        //BCDamage: Si el Battlecry puede dañar y cuanto.
        public bool BCDamage;
        public int BCDamageAmount;
        public int BCDamageAmountOnCombo;
        //BCDamageSeveral: Si el Battlecry puede silenciar.
        public bool BCSilence;
        //BCDamageSeveral: Si el Battlecry daña a mis propios bichos.
        public bool BCDamageOwn;
        //BCDamageSeveral: Si el Battlecry daña a varios bichos.
        public bool BCDamageSeveral;
        //KillThis: Voy a tratar de matarlo perdiendo menos que el daño que le tengo que hacer. EJ: a una 4-3 la ataco con una 1-1 y una 2-1
        public bool KillThis;
        //KillThisEXTREME: No me importan las pérdidas, tengo que matar a este.
        public bool KillThisEXTREME;
        //SpellOnThis: Trato de usar spell que mate o disablee (Polymorph, Hex, Assasinate)
        public bool SpellOnThis;
        //SilenceThis: Voy a silenciarlo.
        public bool SilenceThis;
        public Card Card;

        public CardDetails()
        {
            Card = null;
            BCHeal = false;
            BCHealAmount = 0;
            BCDamage = false;
            BCDamageAmount = 0;
            BCDamageAmountOnCombo = 0;
            BCSilence = false;
            BCDamageOwn = false;
            BCDamageSeveral = false;
            SetInitValues();
        }

        public void SetInitValues()
        {
            KillThis = false;
            KillThisEXTREME = false;
            SpellOnThis = false;
            SilenceThis = false;
        }
    }
}
