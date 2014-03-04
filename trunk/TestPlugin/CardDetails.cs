using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin
{
    public class CardDetails
    {
        public string CardId = "";
        public string CardName = "";
        //BCCanHeal: Si el Battlecry puede curar y cuanto.
        public bool BCHeal = false;
        public int BCHealAmount = 0;
        //BCDamage: Si el Battlecry puede dañar y cuanto.
        public bool BCDamage = false;
        public int BCDamageAmount = 0;
        public int BCDamageAmountOnCombo = 0;
        //BCDamageSeveral: Si el Battlecry puede silenciar.
        public bool BCSilence = false;
        //BCDamageSeveral: Si el Battlecry daña a mis propios bichos.
        public bool BCDamageOwn = false;
        //BCDamageSeveral: Si el Battlecry daña a varios bichos.
        public bool BCDamageSeveral = false;
        //KillThis: Voy a tratar de matarlo perdiendo menos que el daño que le tengo que hacer. EJ: a una 4-3 la ataco con una 1-1 y una 2-1
        public bool KillThis = false;
        //KillThisEXTREME: No me importan las pérdidas, tengo que matar a este.
        public bool KillThisEXTREME = false;
        //SpellOnThis: Trato de usar spell que mate o disablee (Polymorph, Hex, Assasinate)
        public bool SpellThis = false;
        //SilenceThis: Voy a silenciarlo.
        public bool SilenceThis = false;
        public Card Card;

        public CardDetails()
        {
            SetInitValues();
        }

        public void SetInitValues()
        {
            KillThis = false;
            KillThisEXTREME = false;
            SpellThis = false;
            SilenceThis = false;
        }
    }
}
