﻿using System;
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
        
        public void EmoMsgs()
        {
            if (GameFunctions.gs.IsBeginPhase())
            {
                if (GameFunctions.gs.IsLocalPlayerTurn() && GameFunctions.myPlayer.GetHero().GetRemainingHP() == 30)
                {
                    Log.say("GREETINGS");
                    GameState.Get().GetLocalPlayer().GetHeroCard().PlayEmote(EmoteType.GREETINGS);
                }
            }
            if (GameFunctions.gs.IsGameOver() || GameFunctions.myPlayer.GetHero().GetRemainingHP() <= 0)
            {
                //this.randomEmoticon();
                GameState.Get().GetLocalPlayer().GetHeroCard().PlayEmote(EmoteType.GOOD_GAME);
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
                    GameFunctions.DoDrop(cardToPlay);

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
        {//Si el CardDetails tiene silence hay que usarla en el caso que sea necesario

            List<Card> list = Enumerable.ToList<Card>((IEnumerable<Card>)GameFunctions.myPlayer.GetHandZone().GetCards());
            if (GameFunctions.myPlayer.GetBattlefieldZone().GetCardCount() >= 7)
                return null;
            bool flag = false;
            using (List<Card>.Enumerator enumerator = GameFunctions.myPlayer.GetBattlefieldZone().GetCards().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.GetEntity().HasTaunt())
                        flag = true;
                }
            }
            if (!flag)
            {
                foreach (Card c in list)
                {
                    Entity entity = c.GetEntity();
                    if (c.GetEntity().HasTaunt() && entity.GetCardType() == TAG_CARDTYPE.MINION && (entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() && GameFunctions.CanBeUsed(c)))
                   
                        return c;
                   
                    if (HasSilence(c)&& entity.GetCardType() == TAG_CARDTYPE.MINION && (entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() && GameFunctions.CanBeUsed(c)))
                        {
                            if(NextBestAttackee().GetEntity().HasSpellPower() && ShouldBeSilenced(c))
                            return c;
                        }
                      
                    
                }
            }
            Card card = null;
            foreach (Card c in list)
            {
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

        private static bool ShouldBeSilenced(Card c)
        {
            if (CardDetails.FindInCardDetails(c).SilenceThis)
            {
                return true;
            }
            return false;
        }

        private static bool HasSilence(Card c)
        {
            if (CardDetails.FindInCardDetails(c).CanSilence)
            {
                return true;
            }
            return false;
        }

        public static bool BruteAttack()
        {
            Card attackee = BruteAI.NextBestAttackee();
            Card attacker = BruteAI.NextBestAttacker(attackee);
            if (attacker == null || attackee == null)
                return false;
            return GameFunctions.DoAttack(attacker, attackee) ? true : true;
        }

        public static Card NextBestAttacker(Card attackee)
        {
            if (attackee == null)
                return null;
            List<Card> list = Enumerable.ToList<Card>((IEnumerable<Card>)GameFunctions.myPlayer.GetBattlefieldZone().GetCards());
            if (attackee != GameFunctions.ePlayer.GetHeroCard())
            {
                Card c1 = null;
                foreach (Card c2 in list)
                {
                    Entity entity = c2.GetEntity();
                    if (entity.GetATK() >= attackee.GetEntity().GetHealth() && entity.GetHealth() < attackee.GetEntity().GetATK() && (!attackee.GetEntity().HasTaunt() && GameFunctions.CanBeUsed(c2)))
                        return c2;
                    if (entity.GetATK() >= attackee.GetEntity().GetHealth() && !attackee.GetEntity().HasTaunt() && GameFunctions.CanBeUsed(c2))
                        c1 = c2;
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
            List<Card> list1 = Enumerable.ToList<Card>((IEnumerable<Card>)GameFunctions.ePlayer.GetBattlefieldZone().GetCards());
            List<Card> list2 = new List<Card>();
            foreach (Card card in list1)
            {
                Entity entity = card.GetEntity();
                if (entity.HasTaunt() && entity.CanBeAttacked())
                    return card;
            }
            if (GameFunctions.ePlayer.GetHeroCard().GetEntity().CanBeAttacked())
                return GameFunctions.ePlayer.GetHeroCard();
            foreach (Card card in list1)
            {
                if (card.GetEntity().CanBeAttacked())
                    return card;
            }
            return null;
        }
    }
}
