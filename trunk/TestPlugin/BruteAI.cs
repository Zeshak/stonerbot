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
            GameState.Get().GetLocalPlayer().GetHeroCard().PlayEmote(Emo);
            Network.Get().SendEmote(Emo);
            EmoteHandler.Get().HideEmotes();
        }

        public static bool BruteHand()
        {
            if (BruteAI.loops >= BruteAI.maxLoops)
            {
                Log.debug(BruteAI.maxLoops.ToString() + " loops done... Skip this turn");
                return false;
            }
            else
            {

                /*if (CanWinThisTurn())
                {

                }*/
                Card cardToPlay = new Card();
                CardDetails targetCardDetails = NeedsToPlayDisableDestroySilence();
                if (targetCardDetails != null)
                {
                    GameFunctions.turnState = GameFunctions.TurnStates.DROP_FIRSTSPELL;
                    cardToPlay = NextDisableDestroySilence(targetCardDetails);
                    if (cardToPlay != null)
                        return GameFunctions.DoDrop(cardToPlay, targetCardDetails.Card.GetEntity());
                }
                if (BruteAI.tryToPlayCoin())
                    return true;
                GameFunctions.turnState = GameFunctions.TurnStates.DROP_SECRETS;
                cardToPlay = NextBestSecret();
                if (cardToPlay != null)
                    return GameFunctions.DoDrop(cardToPlay);

                GameFunctions.turnState = GameFunctions.TurnStates.DROP_WEAPONS;
                cardToPlay = NextBestWeapon();
                if (cardToPlay != null)
                    return GameFunctions.DoDrop(cardToPlay);

                GameFunctions.turnState = GameFunctions.TurnStates.DROP_MINIONS;
                cardToPlay = NextBestMinionDrop();
                if (cardToPlay != null)
                    return GameFunctions.DoDrop(cardToPlay);

                GameFunctions.turnState = GameFunctions.TurnStates.DROP_SECONDSPELL;
                cardToPlay = NextBestSpell();
                if (cardToPlay != null)
                    return GameFunctions.DoDrop(cardToPlay);
                else
                {
                    GameFunctions.turnState = GameFunctions.TurnStates.DROP_WEAPONS;
                    return LaunchHeroPower();
                }
            }
        }

        public static bool CanWinThisTurn()
        {
            List<Card> tauntCards = new List<Card>();
            foreach (Card hisCard in GameFunctions.ePlayer.GetBattlefieldZone().GetCards())
            {
                Entity hisEntity = hisCard.GetEntity();
                /*if (hisEntity.HasTaunt())
                {*/
                tauntCards.Add(hisCard);
                //}
            }
            foreach (Card myCards in tauntCards)
            {
                Entity myEntity = myCards.GetEntity();
                List<Card> list = NextBestAttackerCombinated(myCards, true);
                Log.debug("----------------------Card " + myCards.ToString() + "------------------------");
                foreach (Card c in list)
                    Log.debug(c.ToString());
                Log.debug("---------------------------------------------------");
            }


            return false;
        }

        private static Card NextDisableDestroySilence(CardDetails targetCardDetails)
        {
            foreach (Card card in GameFunctions.myPlayer.GetHandZone().GetCards())
            {
                if (CardDetails.IsViableToPlay(card, targetCardDetails, true))
                    return card;
            }
            return null;
        }

        /// <summary>
        /// Verifica si es necesario jugar un spell antes que otra carta, siempre con prioridad en spell y luego en silence
        /// </summary>
        /// <returns>El CardDetails a la cual le apuntamos el spell</returns>
        private static CardDetails NeedsToPlayDisableDestroySilence()
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
                if (!(CardDetails.IsViableToPlay(c, null, true)))
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
                || (GameFunctions.myPlayer.GetHeroPower().IsBusy()
                || !GameFunctions.myPlayer.GetHeroPower().CanAttack())
                || !GameFunctions.CanBeUsed(GameFunctions.myPlayer.GetHeroPower().GetCard()))
                return false;

            Entity targetEntity = null;
            Log.debug("LaunchHeroPower");
            //Lógica de si conviene o no jugar el heropower
            switch (GameFunctions.myPlayer.GetHero().GetClass())
            {
                case TAG_CLASS.WARLOCK:
                    if (GameFunctions.myPlayer.GetHero().GetRemainingHP() > 10 && GameFunctions.myPlayer.GetHandZone().GetCardCount() <= 3)
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
                if (entity.GetCost() == GameFunctions.myPlayer.GetNumAvailableResources() + 1)
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

        public static bool BruteAttack()
        {
            Card attackee = BruteAI.GetBestAttackee();
            Card attacker = BruteAI.GetBestAttacker(attackee);
            if (attacker == null || attackee == null)
                return false;
            return GameFunctions.DoAttack(attacker, attackee) ? true : true;
        }

        public static List<Card> NextBestAttackerCombinated(Card hisCardInPlay, bool maximizeDamage = false)
        {
            List<Card> eligibleCards = new List<Card>();
            if (hisCardInPlay == null)
                return null;
            List<Card> listCardsInMyBF = GameFunctions.myPlayer.GetBattlefieldZone().GetCards();
            if (hisCardInPlay != GameFunctions.ePlayer.GetHeroCard())
            {
                bool moreDamageAndAlive = false;
                //Acá entra en caso de que haya que atacar a un minion
                foreach (Card myCardInPlay in listCardsInMyBF)
                {
                    if (!GameFunctions.CanBeUsed(myCardInPlay))
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

                if (eligibleCards.Count > 0)
                    return eligibleCards;
            }
            else
            {
                //Sino ataca al heroe primero con el héroe si puede
                Entity hero = GameFunctions.myPlayer.GetHero();
                if (hero != null && GameFunctions.CanBeUsed(hero.GetCard()))
                {
                    eligibleCards.Add(hero.GetCard());
                    return eligibleCards;
                }
            }
            foreach (Card c in listCardsInMyBF)
            {
                //Sino empieza a atacar al héroe de manera random con todos los minions que se vaya pudiendo
                c.GetEntity();
                if (GameFunctions.CanBeUsed(c))
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
            if (eligibleCards.Count > 0)
                return eligibleCards;
            else
                return null;
        }

        public static List<Card> GetCombinatedAttackersThatKill(Card hisCard, bool maximizeDamage)
        {
            List<Card> myCardsInBF = GameFunctions.myPlayer.GetBattlefieldZone().GetCards();
            List<List<Card>> listOfCardCombinations = GetAllAttackersCombinations(myCardsInBF);

            List<Card> bestCards = new List<Card>();
            int bestGroupCardsDead = 0;
            int bestGroupCardsDamage = 0;
            int bestGroupDamageTaken = 0;

            int thisGroupCardsDead = 0;
            int thisGroupCardsDamage = 0;
            int thisGroupDamageTaken = 0;
            int deadValue = 2;
            bool first = true;
            foreach (List<Card> thisList in listOfCardCombinations)
            {
                thisGroupCardsDead = 0;
                thisGroupCardsDamage = 0;
                thisGroupDamageTaken = 0;
                GetCombinationResults(hisCard, ref thisGroupCardsDead, ref thisGroupCardsDamage, ref thisGroupDamageTaken, deadValue, thisList);
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
                        if (thisGroupCardsDamage < bestGroupCardsDamage|| bestGroupCardsDamage <= hisCard.GetEntity().GetRealTimeRemainingHP())
                        {
                            bestCards = thisList;
                            bestGroupCardsDead = thisGroupCardsDead;
                            bestGroupCardsDamage = thisGroupCardsDamage;
                            bestGroupDamageTaken = thisGroupDamageTaken;
                        }
                    }
                    else
                    {
                        if (((thisGroupCardsDamage < bestGroupCardsDamage || thisGroupCardsDamage <= hisCard.GetEntity().GetRealTimeRemainingHP() + 1) 
                            && (thisGroupDamageTaken < bestGroupDamageTaken))
                            || bestGroupCardsDamage <= hisCard.GetEntity().GetRealTimeRemainingHP())
                        {
                            bestCards = thisList;
                            bestGroupCardsDead = thisGroupCardsDead;
                            bestGroupCardsDamage = thisGroupCardsDamage;
                            bestGroupDamageTaken = thisGroupDamageTaken;
                        }
                    }
                }
            }

            //En caso de que ni con todas las cartas la mate, elijo el último item que va a tener todas mis cartas
            if (bestGroupCardsDamage < hisCard.GetEntity().GetRealTimeRemainingHP())
                bestCards = listOfCardCombinations[listOfCardCombinations.Count - 1];


            return bestCards;
        }

        private static void GetCombinationResults(Card hisCard, ref int thisGroupCardsDead, ref int thisGroupCardsDamage, ref int thisGroupDamageTaken, int deadValue, List<Card> thisList)
        {
            foreach (Card thisListCard in thisList)
            {
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

        public static List<List<Card>> GetAllAttackersCombinations(List<Card> list)
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

        public static Card GetBestAttacker(Card attackee)
        {
            if (attackee == null)
                return null;
            List<Card> myCardsInPlay = GameFunctions.myPlayer.GetBattlefieldZone().GetCards();
            int hisCardATK = attackee.GetEntity().GetRealTimeAttack();
            int hisCardHP = attackee.GetEntity().GetRealTimeRemainingHP();
            if (attackee != GameFunctions.ePlayer.GetHeroCard())
            {
                Card selectedCard = null;
                int bestSum = 0;
                int thisSum = 0;
                bool canSurvive = false;
                bool willDie = false;
                foreach (Card myCard in myCardsInPlay)
                {
                    if (!GameFunctions.CanBeUsed(myCard))
                        continue;
                    Entity myCardEntity = myCard.GetEntity();
                    CardDetails cd = CardDetails.FindInCardDetails(myCard);
                    int myCardHP = myCardEntity.GetRealTimeRemainingHP();
                    int myCardATK = myCardEntity.GetRealTimeAttack();
                    //Para los que no son extreme, y no tienen taunt, sólo los ataco con esa si el ataque de la carta es mayor a la HP y si el bicho me mataría
                    if (cd != null && !cd.KillThisEXTREME && !attackee.GetEntity().HasTaunt() && myCardHP <= hisCardATK)
                        continue;
                    if (!attackee.GetEntity().HasDivineShield())
                    {
                        if (myCardATK >= hisCardHP && myCardHP > hisCardATK)
                            return myCard;
                        thisSum = myCardATK + myCardHP;
                        if (myCardATK >= hisCardHP && (thisSum < bestSum || bestSum == 0))
                        {
                            selectedCard = myCard;
                            bestSum = thisSum;
                        }
                    }
                    else
                    {
                        //Mejorar esto, tendria que comprobar si lo puedo matar con Divine Shield.
                        if (myCardHP <= hisCardATK && !canSurvive)
                        {
                            thisSum = myCardATK;
                            if (thisSum < bestSum || bestSum == 0)
                            {
                                selectedCard = myCard;
                                bestSum = thisSum;
                                willDie = true;
                            }
                        }
                        else
                        {
                            thisSum = myCardATK;
                            if (thisSum < bestSum || bestSum == 0 || willDie)
                            {
                                selectedCard = myCard;
                                bestSum = thisSum;
                                canSurvive = true;
                            }
                        }
                    }
                }
                if (selectedCard != null)
                    return selectedCard;
            }
            else
            {
                Entity hero = GameFunctions.myPlayer.GetHero();
                if (hero != null && GameFunctions.CanBeUsed(hero.GetCard()))
                    return hero.GetCard();
            }
            Card card = null;
            foreach (Card c in myCardsInPlay)
            {
                c.GetEntity();
                if (GameFunctions.CanBeUsed(c))
                {
                    if (new System.Random().NextDouble() > 0.5)
                        return c;
                    card = c;
                }
            }
            return card;
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
                                    attackee = card;
                                }
                                else if (cd.KillThis && !selEXTREME)
                                {
                                    selKillThis = true;
                                    attackee = card;
                                }
                                else if (!selEXTREME && !selKillThis)
                                {
                                    selKillThis = true;
                                    attackee = card;
                                }
                            }
                        }
                    }
                }
                if (cd != null)
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
                        if (attackee == null || !selHasTaunt)
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
            {
                Log.debug("Can attack hero");
                targetEntity = eHero;
            }

            List<Card> myPlayedCards = GameFunctions.myPlayer.GetBattlefieldZone().GetCards();
            if (myPlayedCards.Count > 0 && targetEntity == null)
            {
                foreach (Card card in myPlayedCards)
                {
                    var e = card.GetEntity();
                    if (GameFunctions.gs.IsValidOptionTarget(e))
                    {
                        Log.debug("is valid target: " + e.GetName());
                        Log.debug("considering for battlecry: " + e.GetName());
                        targetEntity = e;
                    }
                    else
                    {
                        Log.debug("is NOT valid target: " + e.GetName());
                    }
                }
            }

            List<Card> ePlayedCards = GameFunctions.ePlayer.GetBattlefieldZone().GetCards();
            if (ePlayedCards.Count > 0 && targetEntity == null)
            {
                foreach (Card card in ePlayedCards)
                {
                    var e = card.GetEntity();
                    if (GameFunctions.gs.IsValidOptionTarget(e))
                    {
                        targetEntity = e;
                    }
                }
            }
            return targetEntity;
        }
    }
}
