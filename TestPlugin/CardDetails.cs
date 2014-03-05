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

        //Propiedades más usadas para minions propios y efectos
        public bool CanHeal = false;
        public int CanHealAmount = 0;
        public bool CanDamage = false;
        public int CanDamageAmount = 0;
        public int CanDamageAmountOnCombo = 0;
        public bool CanDamageOwn = false;
        public bool CanDamageSeveral = false;
        public bool CanSilence = false;
        public bool CanDisable = false;
        public static List<CardDetails> ListCardDetails = new List<CardDetails>();

        //Propiedades más usadas para minions y cartas enemigas
        //KillThis: Voy a tratar de matarlo perdiendo menos que el daño que le tengo que hacer. EJ: a una 4-3 la ataco con una 1-1 y una 2-1
        public bool KillThis = false;
        //KillThisEXTREME: No me importan las pérdidas, tengo que matar a este.
        public bool KillThisEXTREME = false;
        //SpellOnThis: Trato de usar spell que mate o disablee (Polymorph, Hex, Assasinate)
        public bool DisableThis = false;
        //SilenceThis: Voy a silenciarlo.
        public bool SilenceThis = false;
        public bool DisableFirst = false;

        public Card Card;

        public CardDetails()
        {
            SetInitValues();
        }

        public void SetInitValues()
        {
            KillThis = false;
            KillThisEXTREME = false;
            DisableThis = false;
            SilenceThis = false;
        }

        public static CardDetails FindInCardDetails(Card card)
        {
            foreach (CardDetails cd in ListCardDetails)
            {
                if (cd.CardId == card.GetEntity().GetCardId())
                    return cd;
            }
            return null;
        }

        public static void SetCardDetails()
        {
            CardDetails cd;

            #region -[ Questing Adventurer ]-
            cd = new CardDetails();
            cd.CardId = "EX1_044";
            cd.CardName = "Questing Adventurer";
            cd.KillThis = true;
            cd.SilenceThis = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Polymorph ]-
            cd = new CardDetails();
            cd.CardId = "CS2_022";
            cd.CardName = "Polymorph";
            cd.CanDisable = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Assassinate ]-
            cd = new CardDetails();
            cd.CardId = "CS2_076";
            cd.CardName = "Assassinate";
            cd.CanDisable = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Hex ]-
            cd = new CardDetails();
            cd.CardId = "EX1_246";
            cd.CardName = "Hex";
            cd.CanDisable = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Mind Control ]-
            cd = new CardDetails();
            cd.CardId = "CS1_113";
            cd.CardName = "Mind Control";
            cd.CanDisable = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Abomination ]-
            cd = new CardDetails();
            cd.CardId = "EX1_097";
            cd.CardName = "Abomination";
            cd.DisableThis = true;
            cd.DisableFirst = true;
            cd.SilenceThis = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ TEST ]-
            cd = new CardDetails();
            cd.CardId = "EX1_048";
            cd.CardName = "Spellbreaker";
            cd.DisableThis = true;
            ListCardDetails.Add(cd);            
            #endregion
        }

    }
}
