using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;


namespace Plugin
{
    internal class BruteAI
    {
        public static int loops = 0;
        public static int maxLoops = 10;
        private static int maxSelfDamageWithWeapon = 3;
        public static int loopsAttackCombinated = 0;
        public static int maxLoopsAttackCombinated = 10;

        static BruteAI()
        {

        }

        public static void SendEmoMessages()
        {

            if (GameFunctions.myPlayer.GetHero().GetRemainingHP() == 30 && !Plugin.saidHi)
            {
                SendEmoType(EmoteType.GREETINGS);
                Plugin.saidHi = true;
            }
            if (GameFunctions.myPlayer.GetHero().GetRemainingHP() <= 10 && !Plugin.saidGG
                || GameFunctions.ePlayer.GetHero().GetRemainingHP() <= 10 && !Plugin.saidGG)
            {
                SendEmoType(EmoteType.WELL_PLAYED);
                Plugin.saidGG = true;
            }
        }

        private static void SendEmoType(EmoteType Emo)
        {
            EmoteHandler.Get().ShowEmotes();
            Thread.Sleep(1000);
            GameState.Get().GetLocalPlayer().GetHeroCard().PlayEmote(Emo);
            Network.Get().SendEmote(Emo);
            EmoteHandler.Get().HideEmotes();
        }

        public static bool BruteHand()
        {
            Log.debug("**************BruteHand****************");
            if (BruteAI.loops >= BruteAI.maxLoops)
            {
                Log.debug(BruteAI.maxLoops.ToString() + " loops done... Skip this turn");
                return false;
            }
            else
            {
                /*if (GameFunctions.massiveDropList != null && GameFunctions.massiveDropList.Count > 0)
                {
                    GameFunctions.DoDrop(GameFunctions.massiveDropList[0]);
                    GameFunctions.massiveDropList.RemoveAt(0);
                    return true;
                }
                else
                {
                    if (!GameFunctions.canWinThisTurnFirstTimeDone)
                    {
                        if (BruteAI.CanWinThisTurn())
                        {
                            GameFunctions.canWinThisTurnFirstTimeDone = true;
                            GameFunctions.canWinThisTurn = true;
                            Log.debug("Puedo ganar este turno");
                        }
                        else
                        {
                            GameFunctions.canWinThisTurnFirstTimeDone = true;
                            GameFunctions.canWinThisTurn = false;
                        }
                    }
                }*/
                Card cardToPlay = new Card();
                GameFunctions.turnState = GameFunctions.TurnStates.CHECK_DISABLEDESTROYSILENCE;
                CardDetails targetCardDetails = GetTargetToDisableDestroySilence();
                if (targetCardDetails != null)
                {
                    GameFunctions.turnState = GameFunctions.TurnStates.DROP_DISABLEDESTROYSILENCE;
                    cardToPlay = NextDisableDestroySilence(targetCardDetails);
                    if (cardToPlay != null)
                    {
                        bool returned = GameFunctions.DoDrop(cardToPlay, targetCardDetails.Card.GetEntity());
                        Plugin.Delay(5000);
                        Log.debug("Return 1 " + returned.ToString());
                        return returned;
                    }
                }
                if (BruteAI.tryToPlayCoin())
                    return true;
                GameFunctions.turnState = GameFunctions.TurnStates.DROP_SECRETS;
                cardToPlay = NextBestSecret();
                if (cardToPlay != null)
                {
                    bool returned = GameFunctions.DoDrop(cardToPlay);
                    Log.debug("Return 2 " + returned.ToString());
                    return returned;
                }

                GameFunctions.turnState = GameFunctions.TurnStates.DROP_WEAPONS;
                cardToPlay = NextBestWeapon();
                if (cardToPlay != null)
                {
                    bool returned = GameFunctions.DoDrop(cardToPlay);
                    Log.debug("Return 3 " + returned.ToString());
                    return returned;
                }

                GameFunctions.turnState = GameFunctions.TurnStates.DROP_MINIONS;
                cardToPlay = NextBestMinionDrop();
                if (cardToPlay != null)
                {
                    bool returned = GameFunctions.DoDrop(cardToPlay);
                    Log.debug("Return 4 " + returned.ToString());
                    return returned;
                }

                GameFunctions.turnState = GameFunctions.TurnStates.DROP_SECONDSPELL;
                cardToPlay = NextBestSpell();
                if (cardToPlay != null)
                {
                    bool returned = GameFunctions.DoDrop(cardToPlay);
                    Log.debug("Return 5 " + returned.ToString());
                    return returned;
                }
                else
                {
                    bool returned = LaunchHeroPower();
                    GameFunctions.turnState = GameFunctions.TurnStates.DO_HEROPOWER;
                    Log.debug("Return 6 " + returned.ToString());
                    return returned;
                }
            }
        }

        public static bool CanWinThisTurn()
        {
            Log.debug("**************CanWinThisTurn****************");
            GameFunctions.turnState = GameFunctions.TurnStates.CHECK_CANWIN;
            List<Card> tauntCards = new List<Card>();
            foreach (Card hisCard in GameFunctions.ePlayer.GetBattlefieldZone().GetCards())
            {
                Entity hisEntity = hisCard.GetEntity();
                if (hisEntity.HasTaunt())
                    tauntCards.Add(hisCard);
            }
            List<Card> remainingCards = new List<Card>(GameFunctions.myPlayer.GetBattlefieldZone().GetCards());
            foreach (Card hisCard in tauntCards)
            {
                Entity myEntity = hisCard.GetEntity();
                List<Card> list = GetBestAttackerCombination(hisCard, remainingCards, true);
                if (list != null && list.Count > 0)
                {
                    foreach (Card card in list)
                    {
                        remainingCards.Remove(card);
                    }
                }
            }
            int hpSum = 0;
            foreach (Card card in remainingCards)
            {
                if (GameFunctions.CanAttack(card))
                    hpSum += card.GetEntity().GetRealTimeAttack();
            }
            /*
            List<Card> damageCards = new List<Card>();
            foreach (Card myHandCard in GameFunctions.myPlayer.GetHandZone().GetCards())
            {
                if (myHandCard.GetEntity().HasCharge())
                    damageCards.Add(myHandCard);
            }
            List<List<Card>> listOfCardCombinationsDamage = GetAllCardsCombinations(damageCards);
            int bestCombinationDamage = 0;
            List<Card> bestCombination = new List<Card>();
            foreach (List<Card> myCombination in listOfCardCombinationsDamage)
            {
                int thisCombinationDamage = 0;
                int thisCrystals = 0;
                foreach (Card myCombinationCard in myCombination)
                {
                    thisCrystals += myCombinationCard.GetEntity().GetRealTimeCost();
                    if (thisCrystals <= GameFunctions.myPlayer.GetNumAvailableResources())
                    {
                        Log.debug("Esta combinación puedo jugarla");
                        thisCombinationDamage += myCombinationCard.GetEntity().GetATK();
                    }
                    else
                    {
                        Log.debug("Aca2");
                        thisCombinationDamage = 0;
                        continue;
                    }
                }
                if (thisCombinationDamage > bestCombinationDamage)
                {
                    Log.debug("Aca3");
                    bestCombination = myCombination;
                    bestCombinationDamage = thisCombinationDamage;
                }
            }
            hpSum += bestCombinationDamage;
             */
            //Realtime da la armor del enemy enemigo?
            if (hpSum >= GameFunctions.ePlayer.GetHero().GetRealTimeRemainingHP())
            {
                /*if (bestCombination.Count > 0)
                    GameFunctions.massiveDropList = new List<Card>(bestCombination);*/
                return true;
            }
            return false;
        }

        private static Card NextDisableDestroySilence(CardDetails targetCardDetails)
        {
            foreach (Card card in GameFunctions.myPlayer.GetHandZone().GetCards())
            {
                if (card.GetEntity().GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() 
                    && GameFunctions.CanBeUsed(card) 
                    && CardDetails.IsViableToPlay(card, targetCardDetails, true))
                    return card;
            }
            return null;
        }

        /// <summary>
        /// Verifica si es necesario jugar un spell antes que otra carta, siempre con prioridad en spell y luego en silence
        /// </summary>
        /// <returns>El CardDetails a la cual le apuntamos el spell</returns>
        private static CardDetails GetTargetToDisableDestroySilence()
        {
            List<CardDetails> listCards = GameFunctions.GetBattlefieldCardDetails();
            bool spellPriority = false;
            CardDetails target = null;
            foreach (CardDetails cd in listCards)
            {
                if (cd.Card.GetEntity().CanBeTargetedByAbilities())
                {
                    if (cd.DisableThis)
                    {
                        target = cd;
                        spellPriority = true;
                    }
                    else
                        if (cd.SilenceThis && !spellPriority)
                            target = cd;
                }
            }
            return target;
        }

        public static Card NextBestSecret()
        {
            foreach (Card c in GameFunctions.myPlayer.GetHandZone().GetCards())
            {
                if (!(CardDetails.IsViableToPlay(c, null, true)))
                    continue;
                Entity entity = c.GetEntity();
                if (entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() && entity.IsSecret() && GameFunctions.CanBeUsed(c))
                    return c;
            }
            return null;
        }

        public static Card NextBestSpell()
        {
            foreach (Card c in Enumerable.ToList<Card>(GameFunctions.myPlayer.GetHandZone().GetCards()))
            {
                CardDetails cd = CardDetails.FindInCardDetails(c);
                if (!(CardDetails.IsViableToPlay(c, null, false)))
                    continue;
                Entity entity = c.GetEntity();
                if (entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() && entity.IsSpell())
                    return c;
            }
            return null;
        }

        public static Card NextBestWeapon()
        {
            if (GameFunctions.myPlayer.GetHero().GetWeaponCard() != null)
                return null;
            foreach (Card c in Enumerable.ToList<Card>(GameFunctions.myPlayer.GetHandZone().GetCards()))
            {
                Entity entity = c.GetEntity();
                if (entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() && entity.IsWeapon() && GameFunctions.CanBeUsed(c))
                    return c;
            }
            return null;
        }

        public static bool LaunchHeroPower()
        {
            if (GameFunctions.myPlayer.GetNumAvailableResources() < GameFunctions.myPlayer.GetHeroPower().GetCost()
                || GameFunctions.myPlayer.GetHeroPower().IsExhausted()
                || GameFunctions.myPlayer.GetHeroPower().IsBusy()
                || !GameFunctions.myPlayer.GetHeroPower().CanAttack()
                || !GameFunctions.CanBeUsed(GameFunctions.myPlayer.GetHeroPower().GetCard()))
                return false;

            Entity targetEntity = null;
            Log.debug("LaunchHeroPower");
            //Lógica de si conviene o no jugar el heropower
            switch (GameFunctions.myPlayer.GetHero().GetClass())
            {
                case TAG_CLASS.WARLOCK:
                    if ((GameFunctions.myPlayer.GetHero().GetRemainingHP() > 10 && GameFunctions.myPlayer.GetHandZone().GetCardCount() <= 3)
                        || GameFunctions.myPlayer.GetHero().GetRemainingHP() > 18)
                        break;
                    return false;
                case TAG_CLASS.HUNTER:
                case TAG_CLASS.PALADIN:
                case TAG_CLASS.ROGUE:
                case TAG_CLASS.SHAMAN:
                case TAG_CLASS.WARRIOR:
                case TAG_CLASS.DRUID:
                    break;
                case TAG_CLASS.MAGE:
                    targetEntity = GetBestMageHeroPowerTarget();
                    if (targetEntity != null)
                    {
                        Log.debug("Heropower mage en " + targetEntity.GetName());
                        break;
                    }
                    GameFunctions.Cancel();
                    return false;
                case TAG_CLASS.PRIEST:
                    if (GameFunctions.myPlayer.GetHero().CanBeTargetedByHeroPowers())
                    {
                        targetEntity = GameFunctions.myPlayer.GetHero();
                        break;
                    }
                    GameFunctions.Cancel();
                    return false;
                default:
                    return false;
            }
            return GameFunctions.DoDrop(GameFunctions.myPlayer.GetHeroPower().GetCard(), targetEntity);
        }

        public static bool tryToPlayCoin()
        {
            bool flag = false;
            Card c = null;
            foreach (Card card in Enumerable.ToList<Card>((IEnumerable<Card>)GameFunctions.myPlayer.GetHandZone().GetCards()))
            {
                Entity entity = card.GetEntity();
                if (entity.GetCost() == GameFunctions.myPlayer.GetNumAvailableResources() + 1 && entity.IsMinion())
                    flag = true;
                if (entity.GetCardId() == "GAME_005")
                    c = card;
            }
            if (!flag || !(c != null))
                return false;
            GameFunctions.DoDrop(c);
            return true;
        }

        public static Entity GetBestMageHeroPowerTarget()
        {
            List<CardDetails> listCards = GameFunctions.GetBattlefieldCardDetails();
            Entity target = new Entity();
            int maxAtk = 0;
            int index = -1;
            foreach (CardDetails cd in listCards)
            {
                Entity possibleTarget = cd.Card.GetEntity();
                if (possibleTarget.GetRealTimeRemainingHP() == 1 || possibleTarget.HasDivineShield())
                {
                    int estimatedAtk = possibleTarget.GetRealTimeAttack();
                    if (possibleTarget.HasWindfury())
                        estimatedAtk *= 2;
                    if (possibleTarget.HasTaunt())
                        estimatedAtk += 1;
                    if (maxAtk == 0 || estimatedAtk > maxAtk)
                    {
                        if (possibleTarget.CanBeTargetedByHeroPowers())
                        {
                            index = listCards.IndexOf(cd);
                            maxAtk = estimatedAtk;
                        }
                    }
                }
            }
            if (index != -1)
                return listCards[index].Card.GetEntity();
            else if (GameFunctions.ePlayer.GetHeroCard().GetEntity().CanBeDamaged())
                return GameFunctions.ePlayer.GetHeroCard().GetEntity();
            else return null;
        }

        public static Card NextBestMinionDrop()
        {
            List<Card> listCardsInHand = GameFunctions.myPlayer.GetHandZone().GetCards();
            if (GameFunctions.myPlayer.GetBattlefieldZone().GetCardCount() >= 7)
                return null;
            bool haveInPlayWithTaunt = false;
            foreach (Card c in GameFunctions.myPlayer.GetBattlefieldZone().GetCards())
            {
                if (c.GetEntity().HasTaunt())
                    haveInPlayWithTaunt = true;
            }
            if (!haveInPlayWithTaunt)
            {
                foreach (Card c in listCardsInHand)
                {
                    if (!CardDetails.IsViableToPlay(c))
                        continue;
                    Entity entity = c.GetEntity();
                    if (c.GetEntity().HasTaunt() && entity.GetCardType() == TAG_CARDTYPE.MINION && (entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() && GameFunctions.CanBeUsed(c)))
                        return c;
                }
            }
            Card card = null;
            foreach (Card c in listCardsInHand)
            {
                if (!CardDetails.IsViableToPlay(c))
                    continue;
                Entity entity = c.GetEntity();
                if (entity.GetCardType() == TAG_CARDTYPE.MINION && entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources())
                {
                    if (GameFunctions.myPlayer.GetNumAvailableResources() == entity.GetCost() && GameFunctions.CanBeUsed(c))
                        return c;
                    card = c;
                }
            }
            return card;
        }

        /// <summary>
        /// Realiza el ataque con las cartas y setea ataques conjuntos de ser necesario.
        /// </summary>
        /// <returns>true si hay que seguir entrando acá, false si se terminaron los ataques.</returns>
        public static bool BruteAttack()
        {
            if (GameFunctions.massiveAttackList == null || GameFunctions.massiveAttackList.Count == 0)
            {
                Log.debug("massiveAttackList estaba vacía");
                GameFunctions.massiveAttackList = new List<Card>();
                GameFunctions.massiveAttackAttackee = BruteAI.GetBestAttackee();
                if (GameFunctions.massiveAttackAttackee == null)
                    return false;
                Log.debug("El attackee resultó ser: " + GameFunctions.massiveAttackAttackee.ToString());
                GameFunctions.massiveAttackList = BruteAI.GetBestAttackerCombination(GameFunctions.massiveAttackAttackee, GameFunctions.myPlayer.GetBattlefieldZone().GetCards(), GameFunctions.canWinThisTurn);
                if (GameFunctions.massiveAttackAttackee == null || GameFunctions.massiveAttackList == null || GameFunctions.massiveAttackList.Count == 0)
                {
                    Log.debug("massiveAttackList está vacía");
                    return false;
                }
                return (GameFunctions.DoMassiveAttack(GameFunctions.massiveAttackList, GameFunctions.massiveAttackAttackee)) ? true : true;
            }
            else
            {
                loopsAttackCombinated++;
                if (loopsAttackCombinated > maxLoopsAttackCombinated)
                {
                    GameFunctions.massiveAttackList = new List<Card>();
                    Log.debug("Vacío massiveAttackList, bugged?");
                }
                else
                {
                    Log.debug("massiveAttackList no estaba vacía");
                    if (GameFunctions.massiveAttackAttackee == null)
                        return true;
                    GameFunctions.DoAttack(GameFunctions.massiveAttackList[0], GameFunctions.massiveAttackAttackee);
                }
                return true;
            }
        }

        public static List<Card> GetBestAttackerCombination(Card hisCardInPlay, List<Card> listCardsInMyBF, bool maximizeDamage = false)
        {
            List<Card> eligibleCards = new List<Card>();
            if (hisCardInPlay == null)
                return null;
            if (hisCardInPlay != GameFunctions.ePlayer.GetHeroCard() && listCardsInMyBF != null && listCardsInMyBF.Count > 0)
            {
                //Sino ataca al heroe primero con el héroe si puede
                Entity myHero = GameFunctions.myPlayer.GetHero();
                if (myHero != null && GameFunctions.CanAttack(myHero.GetCard()))
                {
                    if (hisCardInPlay.GetEntity().GetRealTimeAttack() <= 3 && hisCardInPlay.GetEntity().GetRealTimeRemainingHP() <= myHero.GetRealTimeAttack())
                    {
                        eligibleCards.Clear();
                        eligibleCards.Add(myHero.GetCard());
                        return eligibleCards;
                    }
                }
                bool moreDamageAndAlive = false;
                //Acá entra en caso de que haya que atacar a un minion
                foreach (Card myCardInPlay in listCardsInMyBF)
                {
                    if (!GameFunctions.CanAttack(myCardInPlay))
                        continue;
                    Entity myEntityInPlay = myCardInPlay.GetEntity();
                    Entity hisEntityInPlay = hisCardInPlay.GetEntity();

                    // EJ: Mia es 4-X y la de él es X-4 y tengo divine shield
                    if (myEntityInPlay.GetRealTimeAttack() == hisEntityInPlay.GetRealTimeRemainingHP() && myEntityInPlay.HasDivineShield())
                    {
                        eligibleCards.Clear();
                        eligibleCards.Add(myCardInPlay);
                        return eligibleCards;
                    }
                    // EJ: Mia es 4-4 y la de él es 3-4
                    else if (myEntityInPlay.GetRealTimeAttack() == hisEntityInPlay.GetRealTimeRemainingHP() && myEntityInPlay.GetRealTimeRemainingHP() > hisEntityInPlay.GetRealTimeAttack())
                    {
                        eligibleCards.Clear();
                        eligibleCards.Add(myCardInPlay);
                        return eligibleCards;
                    }
                    // EJ: Mia es 4-3 y la de él es 5-4 
                    else if (myEntityInPlay.GetRealTimeAttack() == hisEntityInPlay.GetRealTimeRemainingHP()
                        && hisEntityInPlay.GetRealTimeRemainingHP() < hisEntityInPlay.GetRealTimeAttack()
                        && myEntityInPlay.GetRealTimeRemainingHP() < myEntityInPlay.GetRealTimeAttack())
                    {
                        eligibleCards.Clear();
                        eligibleCards.Add(myCardInPlay);
                        return eligibleCards;
                    }
                    // EJ: Mia es 5-4 y la de él es 3-4
                    else if (myEntityInPlay.GetRealTimeAttack() > hisEntityInPlay.GetRealTimeRemainingHP() && myEntityInPlay.GetRealTimeRemainingHP() > hisEntityInPlay.GetRealTimeAttack())
                    {
                        if (moreDamageAndAlive && (eligibleCards[0].GetEntity().GetRealTimeAttack() <= myEntityInPlay.GetRealTimeAttack()))
                            continue;
                        eligibleCards.Clear();
                        eligibleCards.Add(myCardInPlay);
                        moreDamageAndAlive = true;
                    }
                    // EJ: Mia es 4-3 la de él es 3-4
                    else if (myEntityInPlay.GetRealTimeAttack() >= hisCardInPlay.GetEntity().GetRealTimeRemainingHP() && !moreDamageAndAlive)
                    {
                        //Si no tengo cartas posibles, la agrego y sigo mirando
                        if (eligibleCards.Count == 0)
                        {
                            eligibleCards.Add(myCardInPlay);
                            continue;
                        }
                        else
                        {
                            //Si tengo cartas elegibles elijo la de menor ataque.
                            if (eligibleCards[0].GetEntity().GetRealTimeAttack() > myEntityInPlay.GetRealTimeAttack())
                            {
                                eligibleCards.Clear();
                                eligibleCards.Add(myCardInPlay);
                            }
                        }
                    }
                }

                if (eligibleCards.Count == 0)
                {
                    eligibleCards = GetCombinatedAttackersThatKill(hisCardInPlay, maximizeDamage);
                }

                if (eligibleCards != null && eligibleCards.Count > 0)
                {
                    Log.debug("Devuelve algo...");
                    return eligibleCards;
                }
            }
            else
            {
                //Sino ataca al heroe primero con el héroe si puede
                Entity hero = GameFunctions.myPlayer.GetHero();
                if (hero != null && GameFunctions.CanBeUsed(hero.GetCard()) && GameFunctions.CanAttack(hero.GetCard()))
                {
                    eligibleCards.Add(hero.GetCard());
                    return eligibleCards;
                }
            }
            Log.debug("Empezando ataque con todas las cartas");
            eligibleCards = new List<Card>();
            foreach (Card c in listCardsInMyBF)
            {
                //Sino empieza a atacar al héroe de manera random con todos los minions que se vaya pudiendo
                c.GetEntity();
                if (GameFunctions.CanAttack(c))
                {
                    if (new System.Random().NextDouble() > 0.5)
                    {
                        eligibleCards.Clear();
                        eligibleCards.Add(c);
                        return eligibleCards;
                    }
                    eligibleCards.Add(c);
                }
            }
            if (eligibleCards != null && eligibleCards.Count > 0)
                return eligibleCards;
            else
                return null;
        }

        public static List<Card> GetCombinatedAttackersThatKill(Card hisCard, bool maximizeDamage)
        {
            List<Card> myCardsInBF = GameFunctions.myPlayer.GetBattlefieldZone().GetCards();
            if (myCardsInBF == null || myCardsInBF.Count == 0)
                return new List<Card>();
            List<List<Card>> listOfCardCombinations = GetAllCardsCombinations(myCardsInBF);

            List<Card> bestCards = new List<Card>();
            int bestGroupCardsDead = 0;
            int bestGroupCardsDamage = 0;
            int bestGroupDamageTaken = 0;

            int thisGroupCardsDead = 0;
            int thisGroupCardsDamage = 0;
            int thisGroupDamageTaken = 0;
            bool first = true;
            /*Log.debug("****Cartas para jugar en Combinated****");
            Log.debug("Su carta es: " + hisCard.ToString() + " ATK: " + hisCard.GetEntity().GetRealTimeAttack() + " HP: " + hisCard.GetEntity().GetRealTimeRemainingHP());
            Log.debug("Tengo " + myCardsInBF.Count.ToString() + " cartas en juego");
            Log.debug("Puedo hacer " + listOfCardCombinations.Count.ToString() + " combinaciones");
            Log.debug("En juego tengo:");
            foreach (Card c in myCardsInBF)
                Log.debug(c.ToString() + " ATK: " + c.GetEntity().GetRealTimeAttack() + " HP: " + c.GetEntity().GetRealTimeRemainingHP());*/
            foreach (List<Card> thisList in listOfCardCombinations)
            {
                thisGroupCardsDead = 0;
                thisGroupCardsDamage = 0;
                thisGroupDamageTaken = 0;
                GetCombinationResults(hisCard, ref thisGroupCardsDead, ref thisGroupCardsDamage, ref thisGroupDamageTaken, thisList);
                if (first)
                {
                    first = false;
                    bestCards = thisList;
                    bestGroupCardsDead = thisGroupCardsDead;
                    bestGroupCardsDamage = thisGroupCardsDamage;
                    bestGroupDamageTaken = thisGroupDamageTaken;
                    continue;
                }

                //Esto me dice si entra en el grupo de los que PUEDEN matarlo.
                if (thisGroupCardsDamage >= hisCard.GetEntity().GetRealTimeRemainingHP())
                {
                    if (maximizeDamage)
                    {
                        if (thisGroupCardsDamage < bestGroupCardsDamage || bestGroupCardsDamage <= hisCard.GetEntity().GetRealTimeRemainingHP())
                        {
                            bestCards = thisList;
                            bestGroupCardsDead = thisGroupCardsDead;
                            bestGroupCardsDamage = thisGroupCardsDamage;
                            bestGroupDamageTaken = thisGroupDamageTaken;
                        }
                    }
                    else
                    {
                        //Log.debug("Llega a preguntar. TGCD: " + thisGroupCardsDamage.ToString() + " - BGCD: " + bestGroupCardsDamage.ToString() + " - TGDK: " + thisGroupDamageTaken.ToString() + " - BGDT: " + bestGroupDamageTaken.ToString() + " - HP: " + hisCard.GetEntity().GetRealTimeRemainingHP().ToString() + " ");
                        if
                            ((thisGroupCardsDamage < bestGroupCardsDamage && thisGroupDamageTaken < bestGroupDamageTaken)
                            || bestGroupCardsDamage < hisCard.GetEntity().GetRealTimeRemainingHP())
                        {
                            bestCards = thisList;
                            bestGroupCardsDead = thisGroupCardsDead;
                            bestGroupCardsDamage = thisGroupCardsDamage;
                            bestGroupDamageTaken = thisGroupDamageTaken;
                        }
                    }
                }
            }
            if (bestGroupCardsDamage == 0)
                return new List<Card>();
            //En caso de que ni con todas las cartas la mate, elijo el último item que va a tener todas mis cartas
            if (bestGroupCardsDamage < hisCard.GetEntity().GetRealTimeRemainingHP() && listOfCardCombinations.Count > 0)
            {
                bestCards = new List<Card>();
                foreach (Card card in listOfCardCombinations[listOfCardCombinations.Count - 1])
                {
                    if (GameFunctions.CanAttack(card))
                        bestCards.Add(card);
                }
            }
            //foreach (Card c in bestCards)
            //    Log.debug("Mis atacantes: " + c.ToString());
            //Log.debug("***************************************");

            return bestCards;
        }

        private static void GetCombinationResults(Card hisCard, ref int thisGroupCardsDead, ref int thisGroupCardsDamage, ref int thisGroupDamageTaken, List<Card> thisList)
        {
            int deadValue = 2;
            foreach (Card thisListCard in thisList)
            {
                if (!GameFunctions.CanAttack(thisListCard))
                {
                    thisGroupCardsDead = 0;
                    thisGroupCardsDamage = 0;
                    thisGroupDamageTaken = 0;
                    break;
                }
                thisGroupCardsDamage += thisListCard.GetEntity().GetRealTimeAttack();
                if (thisListCard.GetEntity().GetRealTimeRemainingHP() <= hisCard.GetEntity().GetRealTimeAttack())
                {
                    thisGroupDamageTaken += thisListCard.GetEntity().GetRealTimeRemainingHP() + deadValue;
                    thisGroupCardsDead++;
                }
                else
                {
                    thisGroupDamageTaken += hisCard.GetEntity().GetRealTimeAttack();
                }
            }
        }

        public static List<List<Card>> GetAllCardsCombinations(List<Card> list)
        {
            List<List<Card>> allAttackersCombinations = new List<List<Card>>();
            List<Card> thisCombination = new List<Card>();
            double count = Math.Pow(2, list.Count);
            for (int i = 1; i <= count - 1; i++)
            {
                string str = Convert.ToString(i, 2).PadLeft(list.Count, '0');
                for (int j = 0; j < str.Length; j++)
                {
                    if (str[j] == '1')
                    {
                        thisCombination.Add(list[j]);
                    }
                }
                allAttackersCombinations.Add(new List<Card>(thisCombination));
                thisCombination.Clear();
            }
            return allAttackersCombinations;
        }

        private static void DoKillablesWithWeapon()
        {
            Entity hero = GameFunctions.myPlayer.GetHero();
            if (hero == null || !GameFunctions.CanBeUsed(hero.GetCard()) || hero.GetRealTimeRemainingHP() < 13)
                return;
            Card attackee = GetBestAttackee(maxSelfDamageWithWeapon);
            if (attackee != null)
                GameFunctions.DoAttack(hero.GetCard(), attackee);
        }

        public static Card GetBestAttackee()
        {
            return GetBestAttackee(0);
        }

        public static Card GetBestAttackee(int hisMaxATK)
        {
            List<Card> hisCardListInPlay = GameFunctions.ePlayer.GetBattlefieldZone().GetCards();
            Card attackee = null;
            bool selHasTaunt = false;
            bool selEXTREME = false;
            bool selKillThis = false;
            foreach (Card card in hisCardListInPlay)
            {
                Entity hisCardEntity = card.GetEntity();
                if ((!hisCardEntity.CanBeAttacked()) || (hisMaxATK != 0 && hisCardEntity.GetRealTimeAttack() > hisMaxATK))
                    continue;
                CardDetails cd = CardDetails.FindInCardDetails(card);
                if (hisCardEntity.HasTaunt())
                {
                    //Si todavía no tengo ninguna, selecciona esta para atacar.
                    if (attackee == null)
                    {
                        selHasTaunt = true;
                        attackee = card;
                    }
                    else
                    {
                        //Primero elije una carta con taunt por sobre todo
                        if (!selHasTaunt)
                        {
                            selHasTaunt = true;
                            attackee = card;
                        }
                        else
                        {
                            if (cd != null)
                            {
                                if (cd.KillThisEXTREME)
                                {
                                    selEXTREME = true;
                                    selHasTaunt = true;
                                    attackee = card;
                                }
                                else if (cd.KillThis && !selEXTREME)
                                {
                                    selKillThis = true;
                                    selHasTaunt = true;
                                    attackee = card;
                                }
                                else if (!selEXTREME && !selKillThis)
                                {
                                    selKillThis = true;
                                    selHasTaunt = true;
                                    attackee = card;
                                }
                            }
                        }
                    }
                }
                if (cd != null && !selHasTaunt)
                {
                    if (cd.KillThis)
                    {
                        if (attackee == null)
                        {
                            selKillThis = true;
                            attackee = card;
                        }
                        else if (attackee == card)
                            selKillThis = true;
                    }
                    if (cd.KillThisEXTREME)
                    {
                        if (attackee == null)
                        {
                            selEXTREME = true;
                            attackee = card;
                        }
                        else if (attackee == card)
                            selEXTREME = true;
                    }
                }
            }
            if (attackee == null)
            {
                if (GameFunctions.ePlayer.GetHeroCard().GetEntity().CanBeAttacked())
                    return GameFunctions.ePlayer.GetHeroCard();
                foreach (Card card in hisCardListInPlay)
                {
                    if (card.GetEntity().CanBeAttacked())
                        return card;
                }
            }
            return attackee;
        }

        public static Entity GetGenericTarget()
        {
            Entity targetEntity = null;
            var eHero = GameFunctions.ePlayer.GetHeroCard().GetEntity();
            if (GameFunctions.gs.IsValidOptionTarget(eHero))
                targetEntity = eHero;

            List<Card> myPlayedCards = GameFunctions.myPlayer.GetBattlefieldZone().GetCards();
            if (myPlayedCards.Count > 0 && targetEntity == null)
            {
                foreach (Card card in myPlayedCards)
                {
                    var e = card.GetEntity();
                    if (GameFunctions.gs.IsValidOptionTarget(e))
                        targetEntity = e;
                }
            }

            List<Card> ePlayedCards = GameFunctions.ePlayer.GetBattlefieldZone().GetCards();
            if (ePlayedCards.Count > 0 && targetEntity == null)
            {
                foreach (Card card in ePlayedCards)
                {
                    var e = card.GetEntity();
                    if (GameFunctions.gs.IsValidOptionTarget(e))
                        targetEntity = e;
                }
            }
            return targetEntity;
        }
    }
}
