using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin
{
    public class CardDetails
    {
        #region -[ Sección variables que pondremos en un XML ]-

        #region -[ Common ]-
        private class YseraAwakens
        {
            public static int MyMaxCardsInPlay = 2;
            public static int MyMaxCardsDestroyed = 1;
            public static int EnemyMinCardsInPlay = 3;
            public static int CardDamage = 5;
            public static int CardsDestroyed = 3;
        }
        #endregion
        #region -[ Hunter ]-
        private class ExplosiveTrap
        {
            public static int MyMaxCardsInPlay = 1;
            public static int MyMinHeroHPToForcePlay = 13;
            public static int EnemyMinCardsInPlay = 2;
            public static int CardDamage = 2;
            public static int CardsDestroyed = 2;
        }
        #endregion
        #region -[ Mage ]-
        private class ArcaneExplosion
        {
            public static int EnemyMinCardsInPlay = 0;
            public static int CardDamage = 2;
            public static int CardsDestroyed = 2;
        }

        private class Flamestrike
        {
            public static int EnemyMinCardsInPlay = 0;
            public static int CardDamage = 4;
            public static int CardsDestroyed = 2;
        }

        private class Blizzard
        {
            public static int EnemyMinCardsInPlay = 4;
            public static int CardDamage = 2;
            public static int CardsDestroyed = 2;
        }

        private class ArcaneIntellect
        {
            public static int EnemyMaxCardsInPlay = 2;
            public static int MyMinCardsInPlay = 2;
            public static int MyMaxCardsInHand = 2;
            public static int MyMinTotalCrystals = 5;
        }

        private class Fireball
        {
            public static int UseToKillTaunt = 1;
            public static int UseToKillNormal = 1;
            public static int MinHPToKillTaunt = 5;
            public static int MinHPToKillNormal = 6;
        }
        #endregion
        #region -[ Priest ]-
        private class HolyNova
        {
            public static int EnemyMinCardsInPlay = 0;
            public static int CardDamage = 2;
            public static int CardsDestroyed = 2;
        }
        #endregion
        #region -[ Paladin ]-
        private class Consecration
        {
            public static int EnemyMinCardsInPlay = 0;
            public static int CardDamage = 2;
            public static int CardsDestroyed = 2;
        }
        #endregion
        #region -[ Rogue ]-
        private class FanOfKnives
        {
            public static int EnemyMinCardsInPlay = 0;
            public static int CardDamage = 1;
            public static int CardsDestroyed = 2;
        }

        private class BladeFlurry
        {
            public static int EnemyMinCardsInPlay = 0;
            public static int CardDamage = 0;
            public static int CardsDestroyed = 2;
            public static int MinWeaponDurability = 1;
        }
        #endregion
        #region -[ Shaman ]-
        private class LightningStorm
        {
            public static int EnemyMinCardsInPlay = 0;
            public static int CardDamage = 2;
            public static int CardsDestroyed = 2;
        }
        #endregion
        #region -[ Warlock ]-
        private class Hellfire
        {
            public static int MyMaxCardsInPlay = 1;
            public static int MyMaxCardsDestroyed = 0;
            public static int EnemyMinCardsInPlay = 2;
            public static int CardDamage = 2;
            public static int CardsDestroyed = 2;
        }
        #endregion
        #region -[ Warrior ]-
        private class Whirlwind
        {
            public static int MyMaxCardsInPlay = 1;
            public static int MyMaxCardsDestroyed = 0;
            public static int EnemyMinCardsInPlay = 2;
            public static int CardDamage = 2;
            public static int CardsDestroyed = 2;
        }
        #endregion

        #endregion

        public static List<CardDetails> ListCardDetails = new List<CardDetails>();
        public Card Card;

        public string CardId = "";
        public string CardName = "";

        //Propiedades más usadas para minions y cartas enemigas
        public bool KillThis = false;
        public bool KillThisEXTREME = false;
        public bool DisableThis = false;
        public bool SilenceThis = false;
        public bool SilenceFirst = false;
        public bool DestroyThis = false;



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
                {
                    if (cd.Card == null)
                        cd.Card = card;
                    return cd;
                }
            }
            return null;
        }

        /// <summary>
        /// Se usa para cartas con efectos simples, como silenciar o matar un enemigo.
        /// </summary>
        public static void SetCardDetails()
        {
            CardDetails cd;
            
            #region -[ Cartas Enemigas ]-
            #region -[ Abomination ]-
            cd = new CardDetails();
            cd.CardId = "EX1_097";
            cd.CardName = "Abomination";
            cd.SilenceThis = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Lightspawn ]-
            cd = new CardDetails();
            cd.CardId = "EX1_335";
            cd.CardName = "Lightspawn";
            cd.SilenceThis = true;
            cd.KillThis = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Ragnaros ]-
            cd = new CardDetails();
            cd.CardId = "EX1_298";
            cd.CardName = "Ragnaros";
            cd.DisableThis = true;
            cd.KillThisEXTREME = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Questing Adventurer ]-
            cd = new CardDetails();
            cd.CardId = "EX1_044";
            cd.CardName = "Questing Adventurer";
            cd.KillThis = true;
            cd.SilenceThis = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Flametongue Totem ]-
            cd = new CardDetails();
            cd.CardId = "EX1_565";
            cd.CardName = "Flametongue Totem";
            cd.KillThis = true;
            cd.SilenceThis = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Lightwell ]-
            cd = new CardDetails();
            cd.CardId = "EX1_341";
            cd.CardName = "Lightwell";
            cd.KillThis = true;
            cd.KillThisEXTREME = true;
            cd.SilenceThis = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Ysera ]-
            cd = new CardDetails();
            cd.CardId = "EX1_572";
            cd.CardName = "Ysera";
            cd.SilenceThis = true;
            cd.DisableThis = true;
            cd.KillThis = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Warsong Commander ]-
            cd = new CardDetails();
            cd.CardId = "EX1_084";
            cd.CardName = "Warsong Commander";
            cd.KillThis = true;
            cd.KillThisEXTREME = true;
            ListCardDetails.Add(cd);
            #endregion
            #region -[ Raid Leader ]-
            cd = new CardDetails();
            cd.CardId = "CS2_122";
            cd.CardName = "Raid Leader";
            cd.KillThis = true;
            cd.KillThisEXTREME = true;
            ListCardDetails.Add(cd);
            #endregion
            #endregion
            
            cd = new CardDetails();
            cd.CardId = "CS2_182";
            cd.CardName = "sdsa";
            cd.DisableThis = true;
            ListCardDetails.Add(cd);
        }

        /// <summary>
        /// Para más info ver la otra sobrecarga.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public static bool IsViableToPlay(Card card)
        {
            return IsViableToPlay(card, null, false);
        }

        /// <summary>
        /// Esta función a diferencia de la anterior se usa para cartas complejas y que requieren una toma de decisiones específica dependiendo del juego. 
        /// SIEMPRE antes de decidirse si jugar una carta, hay que correr esta función, SIN IMPORTAR que la carta no esté acá.
        /// </summary>
        /// <param name="myCard">Carta en sí que deseamos jugar</param>
        /// <param name="specialParameter">Las cartas especiales a veces necesitan parámetros (comúnmente se pasa el target).</param>
        /// <param name="playPriority">Si se pasa este parámetro entonces se está forzando a jugar la carta UNICAMENTE si está acá y cumple la condición, si no está no se juega.</param>
        /// <returns>Si es viable para jugar, o no.</returns>
        public static bool IsViableToPlay(Card myCard, object specialParameter, bool playPriority)
        {
            //Si la carta no se puede jugar ni me gasto...
            if (!GameFunctions.CanBeUsed(myCard))
                return false;            
            switch (myCard.GetEntity().GetCardId())
            {
                #region -[ Common ]-
                #region -[ The Coin ]-
                case "GAME_005":
                    return false;
                #endregion
                #region -[ Ysera Awakens ]-
                case "DREAM_02":
                    return CommonMultipleSpellDamageALL(YseraAwakens.CardDamage, YseraAwakens.CardsDestroyed, YseraAwakens.EnemyMinCardsInPlay, YseraAwakens.MyMaxCardsInPlay, YseraAwakens.MyMaxCardsDestroyed);
                #endregion
                #region -[ Dream ]-
                case "DREAM_04":
                    {
                        if (specialParameter == null)
                            return false;
                        CardDetails targetEntity = (CardDetails)specialParameter;
                        bool targetIsEnemy = GameFunctions.IsEnemyCard(targetEntity.Card);
                        if (targetEntity.DisableThis && targetIsEnemy)
                            return true;
                        return false;
                    }
                #endregion
                #region -[ Ironbeak Owl ]-
                case "CS2_203":
                    {
                        if (specialParameter == null)
                            return false;
                        CardDetails targetEntity = (CardDetails)specialParameter;
                        bool targetIsEnemy = GameFunctions.IsEnemyCard(targetEntity.Card);
                        if (targetEntity.SilenceThis && targetIsEnemy)
                            return true;
                        return false;
                    }
                #endregion
                #region -[ Spellbreaker ]-
                case "EX1_048":
                    {
                        if (specialParameter == null)
                            return false;
                        CardDetails targetEntity = (CardDetails)specialParameter;
                        bool targetIsEnemy = GameFunctions.IsEnemyCard(targetEntity.Card);
                        if (targetEntity.SilenceThis && targetIsEnemy)
                            return true;
                        return false;
                    }
                #endregion
                #region -[ Arcane Golem ]-
                case "EX1_089":
                    {
                        if (GameFunctions.gameTurn > 6)
                            return true;
                        return false;
                    }
                #endregion                   
                #endregion
                #region -[ Hunter ]-
                #region -[ Explosive Trap ]-
                case "EX1_610":
                    if ((GameFunctions.myPlayer.GetBattlefieldZone().GetCardCount() <= ExplosiveTrap.MyMaxCardsInPlay
                        || GameFunctions.myPlayer.GetRemainingHP() <= ExplosiveTrap.MyMinHeroHPToForcePlay)
                        && GameFunctions.ePlayer.GetBattlefieldZone().GetCardCount() >= ExplosiveTrap.EnemyMinCardsInPlay)
                    {
                        int count = 0;
                        foreach (Card c in GameFunctions.ePlayer.GetBattlefieldZone().GetCards())
                        {
                            if (c.GetEntity().GetRemainingHP() <= ExplosiveTrap.CardDamage)
                                count++;
                        }
                        if (count >= ExplosiveTrap.CardsDestroyed)
                            return true;
                    }
                    return false;
                #endregion
                #endregion
                #region -[ Mage ]-
                #region -[ Polymorph ]-
                case "CS2_022":
                    {
                        if (specialParameter == null)
                            return false;
                        CardDetails targetEntity = (CardDetails)specialParameter;
                        bool targetIsEnemy = GameFunctions.IsEnemyCard(targetEntity.Card);
                        if (targetEntity.DisableThis && targetIsEnemy)
                            return true;
                        return false;
                    }
                #endregion
                #region -[ Arcane Explosion ]-
                case "CS2_025":
                    return CommonMultipleSpell(ArcaneExplosion.CardDamage, ArcaneExplosion.CardsDestroyed, ArcaneExplosion.EnemyMinCardsInPlay);
                #endregion
                #region -[ Flamestrike ]-
                case "CS2_032":
                    return CommonMultipleSpell(Flamestrike.CardDamage, Flamestrike.CardsDestroyed, Flamestrike.EnemyMinCardsInPlay);
                #endregion
                #region -[ Blizzard ]-
                case "CS2_028":
                    return CommonMultipleSpell(Blizzard.CardDamage, Blizzard.CardsDestroyed, Blizzard.EnemyMinCardsInPlay);
                #endregion
                #region -[ Arcane Intellect ]-
                case "CS2_023":
                    if (GameFunctions.ePlayer.GetBattlefieldZone().GetCardCount() <= ArcaneIntellect.EnemyMaxCardsInPlay
                        && GameFunctions.myPlayer.GetHandZone().GetCardCount() <= ArcaneIntellect.MyMaxCardsInHand
                        && GameFunctions.myPlayer.GetBattlefieldZone().GetCardCount() >= ArcaneIntellect.MyMinCardsInPlay
                        && GameFunctions.myPlayer.GetRealTimeTempMana() >= ArcaneIntellect.MyMinTotalCrystals)
                        return true;
                    return false;
                #endregion
                #region -[ Fireball ]-
                case "CS2_029":
                    if (GameFunctions.ePlayer.GetRealTimeRemainingHP() < 7)
                        return true;
                    return false;
                #endregion
                #endregion
                #region -[ Priest ]-
                #region -[ Holy Nova ]-
                case "CS1_112":
                    return CommonMultipleSpell(HolyNova.CardDamage, HolyNova.CardsDestroyed, HolyNova.EnemyMinCardsInPlay);
                #endregion
                #region -[ Shadow Word: Pain ]-
                case "CS2_234":
                    {
                        if (specialParameter == null)
                            return false;
                        CardDetails targetEntity = (CardDetails)specialParameter;
                        bool targetIsEnemy = GameFunctions.IsEnemyCard(targetEntity.Card);
                        if (targetEntity.Card.GetEntity().GetATK() <= 3 && targetIsEnemy && targetEntity.DestroyThis)
                            return true;
                        return false;
                    }
                #endregion
                #region -[ Shadow Word: Death ]-
                case "EX1_622":
                    {
                        if (specialParameter == null)
                            return false;
                        CardDetails targetEntity = (CardDetails)specialParameter;
                        bool targetIsEnemy = GameFunctions.IsEnemyCard(targetEntity.Card);
                        if (targetEntity.Card.GetEntity().GetATK() >= 5 && targetIsEnemy && targetEntity.DestroyThis)
                            return true;
                        return false;
                    }
                #endregion
                #endregion
                #region -[ Paladin ]-
                #region -[ Consecration ]-
                case "CS2_093":
                    return CommonMultipleSpell(Consecration.CardDamage, Consecration.CardsDestroyed, Consecration.EnemyMinCardsInPlay);
                #endregion
                #endregion
                #region -[ Rogue ]-
                #region -[ Assassinate ]-
                case "CS2_076":
                    {
                        if (specialParameter == null)
                            return false;
                        CardDetails targetEntity = (CardDetails)specialParameter;
                        bool targetIsEnemy = GameFunctions.IsEnemyCard(targetEntity.Card);
                        if (targetEntity.DestroyThis && targetIsEnemy)
                            return true;
                        return false;
                    }
                #endregion
                #region -[ Fan of Knives ]-
                case "EX1_129":
                    return CommonMultipleSpell(FanOfKnives.CardDamage, FanOfKnives.CardsDestroyed, FanOfKnives.EnemyMinCardsInPlay);
                #endregion
                #region -[ Blade Flurry ]-
                case "CS2_233":
                    int cardDamage = GameFunctions.myPlayer.GetHero().GetWeaponCard().GetEntity().GetATK();
                    if (GameFunctions.myPlayer.GetHero().GetWeaponCard().GetEntity().GetDurability() <= BladeFlurry.MinWeaponDurability)
                        return CommonMultipleSpell(cardDamage, BladeFlurry.CardsDestroyed, BladeFlurry.EnemyMinCardsInPlay);
                    return false;
                #endregion
                #endregion
                #region -[ Shaman ]-
                #region -[ Hex ]-
                case "EX1_246":
                    {
                        if (specialParameter == null)
                            return false;
                        CardDetails targetEntity = (CardDetails)specialParameter;
                        bool targetIsEnemy = GameFunctions.IsEnemyCard(targetEntity.Card);
                        if (targetEntity.DisableThis && targetIsEnemy)
                            return true;
                        return false;
                    }
                #endregion
                #region -[ Lightning Storm ]-
                case "EX1_259":
                    return CommonMultipleSpell(LightningStorm.CardDamage, LightningStorm.CardsDestroyed, LightningStorm.EnemyMinCardsInPlay);
                #endregion
                #endregion
                #region -[ Warlock ]-
                #region -[ Hellfire ]-
                case "CS2_062":
                    return CommonMultipleSpellDamageALL(Hellfire.CardDamage, Hellfire.CardsDestroyed, Hellfire.EnemyMinCardsInPlay, Hellfire.MyMaxCardsInPlay, Hellfire.MyMaxCardsDestroyed);
                #endregion
                #endregion
                #region -[ Warrior ]-
                #region -[ Whirlwind ]-
                case "EX1_400":
                    return CommonMultipleSpellDamageALL(Whirlwind.CardDamage, Whirlwind.CardsDestroyed, Whirlwind.EnemyMinCardsInPlay, Whirlwind.MyMaxCardsInPlay, Whirlwind.MyMaxCardsDestroyed);
                #endregion
                #endregion
                default:
                    return !playPriority;
            }
        }

        /// <summary>
        /// Se usa para los hechizos de múltiples objetivos comunes
        /// </summary>
        /// <param name="CardDamage">Una condición de HP máxima de los minions oponentes</param>
        /// <param name="CardsDestroyed">Cantidad de minions que deben ser afectados por la condición</param>
        /// <param name="EnemyMinCardsInPlay">Opcional, mínimo de cartas que debe tener en juego el oponente</param>
        /// <returns>Devuelve si cumple la condición.</returns>
        private static bool CommonMultipleSpell(int CardDamage, int CardsDestroyed, int EnemyMinCardsInPlay)
        {
            if (GameFunctions.ePlayer.GetBattlefieldZone().GetCardCount() < EnemyMinCardsInPlay)
                return false;
            int count = 0;
            foreach (Card card in GameFunctions.ePlayer.GetBattlefieldZone().GetCards())
            {
                if (card.GetEntity().GetRemainingHP() <= CardDamage && !card.GetEntity().HasDivineShield())
                    count++;
            }
            if (count >= CardsDestroyed)
                return true;
            return false;
        }

        /// <summary>
        /// Se usa para los hechizos de múltiples objetivos comunes
        /// </summary>
        /// <param name="CardDamage">Una condición de HP máxima de los minions oponentes</param>
        /// <param name="CardsDestroyed">Cantidad de minions que deben ser afectados por la condición</param>
        /// <param name="EnemyMinCardsInPlay">Opcional, mínimo de cartas que debe tener en juego el oponente</param>
        /// <returns>Devuelve si cumple la condición.</returns>
        private static bool CommonMultipleSpellDamageALL(int CardDamage, int CardsDestroyed, int EnemyMinCardsInPlay, int MyMaxCardsInPlay, int MyMaxCardsDestroyed)
        {
            if (GameFunctions.ePlayer.GetBattlefieldZone().GetCardCount() <= MyMaxCardsInPlay)
            {
                int count = 0;
                foreach (Card card in GameFunctions.myPlayer.GetBattlefieldZone().GetCards())
                {
                    if (card.GetEntity().GetRemainingHP() <= CardDamage)
                        count++;
                }
                if (count <= MyMaxCardsDestroyed)
                    return CommonMultipleSpell(CardDamage, CardsDestroyed, EnemyMinCardsInPlay);
            }
            return false;
        }

        public static void SetNewValuesByCardStatus(ref CardDetails cd)
        {
            Entity entity = cd.Card.GetEntity();
            int currATK = entity.GetATK();
            int currHP = entity.GetHealth();
            int origATK = entity.GetOriginalATK();
            int origHP = entity.GetOriginalHealth();
            if (currATK - currHP > 1 && currHP < 4)
            {
                //Si el bicho tiene dif de atk/vida mayor a 1, y tiene menos de 4 de vida, trato de matarlo perdiendo poco (ej: 5-3)
                cd.SetInitValues();
                cd.KillThis = true;
                if (currATK > 5)
                    cd.DisableThis = true;
            }
            if (entity.HasTaunt())
            {
                //Se usa silence si tiene: (*)Menos de 3 ataque y más de 2 de vida (no silencia las 2-2, las va a matar luego) (*)Si está bufeada y le sumaron 2 a algún atributo
                if ((currATK < 3 && currHP > 2) || (currATK > origATK + 1) || ((currHP > origHP + 1)))
                    cd.SilenceThis = true;
                if (entity.HasDivineShield())
                {
                    //Se usa todo si es a partir de 4-4
                    if (currATK > 3 && currHP > 3)
                    {
                        cd.SilenceThis = true;
                        cd.DisableThis = true;
                    }
                }
                else
                {
                    //Bicho con taunt a partir de 3-6 o 5-3 le tiro spell si tengo
                    if ((currATK > 2 && currHP > 5) || (currHP > 2 && currATK > 4))
                        cd.DisableThis = true;
                }
            }
            if (currATK > 4 && currHP > 4)
            {
                //Si es bicho fuerte, 5-5 en adelante, trato de tirarle spell
                cd.DisableThis = true;
            }
            if (entity.HasDivineShield())
            {
                //Bicho relativamente fuerte, en adelante que pueda o no estar buffeado o tenga taunt con divine shield. Ej: Sunwalker
                if ((currATK > 3 && currHP > 1) || (currATK > origATK + 1) || (currHP > origHP + 1) || (entity.HasTaunt()))
                    cd.DisableThis = true;

                //Bicho originalmente débil, Escudera argenta o alguno con shield que haya sido buffeado, me conviene silenciarlo.
                if ((origATK <= 2 && origHP <= 2) && ((currATK > origATK + 1) || (currHP > origHP + 1)))
                    cd.SilenceThis = true;
            }
            if (entity.HasWindfury())
                cd.KillThisEXTREME = true;
        }
    }
}
