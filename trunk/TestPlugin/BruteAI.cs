using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Plugin
{
    internal class BruteAI
    {
        public static int loops = 0;
        public static int maxLoops = 10;

        static BruteAI()
        {

        }

        public static void SendEmoMessages()
        {
            if (GameFunctions.myPlayer.GetHero().GetRemainingHP() == 30 && !Plugin.saidHi)
            {
                GameState.Get().GetLocalPlayer().GetHeroCard().PlayEmote(EmoteType.GREETINGS);
                Plugin.saidHi = true;
            }
            if (GameFunctions.myPlayer.GetHero().GetRemainingHP() <= 10 && !Plugin.saidGG
                || GameFunctions.ePlayer.GetHero().GetRemainingHP() <= 10 && !Plugin.saidGG)
            {
                GameState.Get().GetLocalPlayer().GetHeroCard().PlayEmote(EmoteType.GOOD_GAME);
                Plugin.saidGG = true;
            }
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
                Card cardToPlay = new Card();
                //Para empezar hasta tener todo programado, hago una primera recorrida de la mano, si alguna carta es viable, la juego, sino sigo de largo.                
                cardToPlay = NextViableCard();
                if (cardToPlay != null)
                    GameFunctions.DoDrop(cardToPlay, null);

                CardDetails targetEntity = NeedsToPlaySpellCard();
                if (targetEntity != null)
                {
                    cardToPlay = NextBestSpellCard(targetEntity);
                    if (cardToPlay != null)
                        return GameFunctions.DoDropSpell(cardToPlay, targetEntity.Card.GetEntity());
                }
                if (BruteAI.tryToPlayCoin())
                    return true;
                cardToPlay = NextBestSecret();
                if (cardToPlay != null)
                    return GameFunctions.DoDropSecret(cardToPlay);
                cardToPlay = NextBestWeapon();
                if (cardToPlay != null)
                    return GameFunctions.DoDropWeapon(cardToPlay);
                cardToPlay = NextBestMinionDrop();
                if (cardToPlay != null)
                    return GameFunctions.DoDropMinion(cardToPlay);
                cardToPlay = NextBestSpell();
                if (cardToPlay != null)
                    return GameFunctions.DoDropSpell(cardToPlay);
                else
                    return LaunchHeroPower();
            }
        }

        private static Card NextViableCard()
        {
            foreach (Card card in GameFunctions.myPlayer.GetHandZone().GetCards())
            {
                if (CardDetails.IsViableToPlay(card.GetEntity(), null, true) && GameFunctions.CanBeUsed(card))
                    return card;
            }
            return null;
        }

        /// <summary>
        /// Verifica si tengo un spell o silence para tirarle a la carta que tengo apuntada
        /// </summary> 
        /// <param name="targetEntity">El CardDetails a la cual le apuntamos el spell</param>
        /// <returns>Devuelve la carta que le puedo usar</returns>
        private static Card NextBestSpellCard(CardDetails targetEntity)
        {
            Card bestCard = null;
            bool isDisable = false;
            foreach (Card card in GameFunctions.myPlayer.GetHandZone().GetCards())
            {
                CardDetails cd = CardDetails.FindInCardDetails(card);
                if (cd == null)
                    continue;
                if (targetEntity.DisableThis && cd.CanDisable && GameFunctions.CanBeUsed(card))
                {
                    if (CardDetails.IsViableToPlay(cd.Card.GetEntity(), targetEntity))
                    {
                        bestCard = card;
                        isDisable = true;
                    }
                }
                if (targetEntity.SilenceThis && cd.CanSilence && GameFunctions.CanBeUsed(card))
                {
                    if (cd.DisableFirst && isDisable)
                        continue;
                    else
                    {
                        bestCard = card;
                        isDisable = false;
                    }
                }
            }
            return bestCard;
        }

        /// <summary>
        /// Verifica si es necesario jugar un spell antes que otra carta, siempre con prioridad en spell y luego en silence
        /// </summary>
        /// <returns>El CardDetails a la cual le apuntamos el spell</returns>
        private static CardDetails NeedsToPlaySpellCard()
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
                Entity entity = c.GetEntity();
                if (entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() && entity.IsSpell() && (entity.GetCardId() != "GAME_005" && GameFunctions.CanBeUsed(c)))
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

            //Lógica de si conviene o no jugar el heropower
            switch (GameFunctions.myPlayer.GetHero().GetClass())
            {
                case TAG_CLASS.WARLOCK:
                    return false;
                case TAG_CLASS.HUNTER:
                case TAG_CLASS.PALADIN:
                case TAG_CLASS.ROGUE:
                case TAG_CLASS.SHAMAN:
                case TAG_CLASS.WARRIOR:
                case TAG_CLASS.MAGE:
                case TAG_CLASS.PRIEST:
                case TAG_CLASS.DRUID:
                    break;
                default:
                    return false;
            }
            return GameFunctions.DoDropHeroPowerSpell();
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
            GameFunctions.DoDropSpell(c);
            return true;
        }

        public static Card NextBestMinionDrop()
        {
            List<Card> listCardsInHand = GameFunctions.myPlayer.GetHandZone().GetCards();
            Log.debug("Mi zona del campo tiene " + GameFunctions.myPlayer.GetBattlefieldZone().GetCardCount().ToString() + " minions");
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
                    CardDetails cd = CardDetails.FindInCardDetails(c);
                    if (cd != null && cd.CanSilence)
                        continue;
                    Entity entity = c.GetEntity();
                    if (c.GetEntity().HasTaunt() && entity.GetCardType() == TAG_CARDTYPE.MINION && (entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() && GameFunctions.CanBeUsed(c)))
                        return c;
                }
            }
            Card card = null;
            foreach (Card c in listCardsInHand)
            {
                CardDetails cd = CardDetails.FindInCardDetails(c);
                if (cd != null && cd.CanSilence)
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
            Card attackee = BruteAI.NextBestAttackee();
            Card attacker = BruteAI.NextBestAttacker(attackee);
            if (attacker == null || attackee == null)
                return false;
            return GameFunctions.DoAttack(attacker, attackee) ? true : true;
        }

        public static List<Card> NextBestAttackerCombinated(Card attackee)
        {
            List<Card> eligibleCards = new List<Card>();
            List<Card> possibleCards = new List<Card>();
            if (attackee == null)
                return null;
            List<Card> listCardInMyHand = GameFunctions.myPlayer.GetBattlefieldZone().GetCards();
            if (attackee != GameFunctions.ePlayer.GetHeroCard())
            {
                //Acá entra en caso de que haya que atacar a un minion
                foreach (Card card in listCardInMyHand)
                {
                    Entity entity = card.GetEntity();
                    // EJ: Mia es 4-4 y la de él es 3-4
                    if (entity.GetATK() >= attackee.GetEntity().GetHealth() && entity.GetHealth() < attackee.GetEntity().GetATK() && GameFunctions.CanBeUsed(card))
                    {
                        eligibleCards.Add(card);
                        return eligibleCards;
                    }

                    // EJ: Mia es 4-3 la de él es 3-4
                    if (entity.GetATK() >= attackee.GetEntity().GetHealth() && GameFunctions.CanBeUsed(card))
                    {
                        //Si no tengo cartas posibles, la agrego y sigo mirando
                        if (possibleCards.Count == 0)
                        {
                            possibleCards.Clear();
                            possibleCards.Add(card);
                            continue;
                        }
                        else
                        {
                            //Si tengo cartas posibles elijo la de menor ataque.
                            if (possibleCards[0].GetEntity().GetATK() > entity.GetATK())
                            {
                                possibleCards.Clear();
                                possibleCards.Add(card);
                            }
                        }
                    }
                }

                if (eligibleCards == null)
                {
                    GetCombinatedAttackersThatKill(listCardInMyHand, eligibleCards, attackee);
                }

                if (eligibleCards != null)
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
            foreach (Card c in listCardInMyHand)
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
            return eligibleCards;
        }

        private static void GetCombinatedAttackersThatKill(List<Card> listCardInMyHand, List<Card> eligibleCards, Card attackee)
        {
            List<Card> orderedList = new List<Card>(listCardInMyHand);
            orderedList.Sort((x, y) => x.GetEntity().GetATK().CompareTo(y.GetEntity().GetATK()));
            int bestAtk = 0;
            int[] avoidedIndexes = new int[orderedList.Count];
            int[] bestAvoidedIndexes = new int[orderedList.Count];
            int possibleCombinations = GetCombinationsWithoutRepetition(orderedList.Count);
            for (int i = 0; i < possibleCombinations; i++)
            {
                int thisAttack = 0;
                thisAttack = orderedList.Sum(
                    delegate(Card card)
                    {
                        int index = orderedList.FindIndex(s => s == card);
                        if (!avoidedIndexes.Contains(index))
                            return card.GetEntity().GetATK();
                        else
                            return 0;
                    }
                );
                if (thisAttack == attackee.GetEntity().GetHealth())
                {
                    bestAvoidedIndexes = avoidedIndexes;
                    break;
                }
                else if (thisAttack > attackee.GetEntity().GetHealth() && (thisAttack < bestAtk || bestAtk == 0))
                    bestAvoidedIndexes = avoidedIndexes;
            }
        }

        private static int GetCombinationsWithoutRepetition(int items)
        {
            int result = 0;
            for (int i = 1; i <= items; i++)
            {
                result += GetPermutationsWithoutRepetition(items, i);
            }
            return result;
        }

        private static int GetPermutationsWithoutRepetition(int items, int count)
        {
            return (Factorial(items) / (Factorial(count) * Factorial(items - count)));
        }

        private static int Factorial(int i)
        {
            if (i <= 1)
                return 1;
            return i * Factorial(i - 1);
        }

        public static Card NextBestAttacker(Card attackee)
        {
            if (attackee == null)
                return null;
            List<Card> list = GameFunctions.myPlayer.GetBattlefieldZone().GetCards();
            if (attackee != GameFunctions.ePlayer.GetHeroCard())
            {
                Card c1 = null;
                int bestSum = 0;
                int thisSum = 0;
                bool attackeeHasDivine = attackee.GetEntity().HasDivineShield();
                foreach (Card c2 in list)
                {
                    Entity myCardEntity = c2.GetEntity();
                    CardDetails cd = CardDetails.FindInCardDetails(c2);
                    //Para los que no son extreme, y no tienen taunt, sólo los ataco con esa si el ataque de la carta es mayor a la HP.
                    if (cd != null && !cd.KillThisEXTREME && !attackee.GetEntity().HasTaunt() 
                        && (!(myCardEntity.GetATK() > myCardEntity.GetHealth()) || !(myCardEntity.GetHealth() > attackee.GetEntity().GetATK())))
                        continue;
                    if (!attackeeHasDivine)
                    {
                        if (myCardEntity.GetATK() >= attackee.GetEntity().GetHealth() && myCardEntity.GetHealth() < attackee.GetEntity().GetATK() && GameFunctions.CanBeUsed(c2))
                            return c2;
                        thisSum = myCardEntity.GetATK() + myCardEntity.GetHealth();
                        if (myCardEntity.GetATK() >= attackee.GetEntity().GetHealth() && GameFunctions.CanBeUsed(c2) && (thisSum < bestSum || bestSum == 0))
                        {
                            c1 = c2;
                            bestSum = thisSum;
                        }
                    }
                    else
                    {
                        thisSum = myCardEntity.GetATK();
                        if (myCardEntity.GetHealth() < attackee.GetEntity().GetATK())
                            return c2;
                        else
                        {
                            thisSum = myCardEntity.GetATK();
                            if (thisSum < bestSum || bestSum == 0)
                            {
                                c1 = c2;
                                bestSum = thisSum;
                            }
                        }
                    }
                }
                if (c1 != null && GameFunctions.CanBeUsed(c1))
                    return c1;
            }
            else
            {
                Entity hero = GameFunctions.myPlayer.GetHero();
                if (hero != null && GameFunctions.CanBeUsed(hero.GetCard()))
                    return hero.GetCard();
            }
            Card card = null;
            foreach (Card c in list)
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

        public static Card NextBestAttackee()
        {
            List<Card> list1 = GameFunctions.ePlayer.GetBattlefieldZone().GetCards();
            Card attackee = null;
            bool selHasTaunt = false;
            bool selEXTREME = false;
            bool selKillThis = false;
            foreach (Card card in list1)
            {
                Entity entity = card.GetEntity();
                if (!entity.CanBeAttacked())
                    continue;
                CardDetails cd = CardDetails.FindInCardDetails(card);
                if (entity.HasTaunt())
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
                foreach (Card card in list1)
                {
                    if (card.GetEntity().CanBeAttacked())
                        return card;
                }
            }
            return attackee;
        }
    }
}
