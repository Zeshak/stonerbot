using System.Collections.Generic;
using System.Linq;

namespace StonerBot
{
    internal class BruteAI
    {
        public static int loops = 0;
        public static int maxLoops = 10;

        static BruteAI()
        {
        }

        public static bool BruteHand()
        {
            if (BruteAI.loops >= BruteAI.maxLoops)
            {
                Log.debug((string)(object)BruteAI.maxLoops + (object)" loops done... Skip this turn");
                return false;
            }
            else
            {
                if (BruteAI.tryToPlayCoin())
                    return true;
                Card c1 = BruteAI.NextBestSecret();
                if ((UnityEngine.Object)c1 == (UnityEngine.Object)null)
                {
                    Card c2 = BruteAI.NextBestWeapon();
                    if ((UnityEngine.Object)c2 == (UnityEngine.Object)null)
                    {
                        Card c3 = BruteAI.NextBestMinionDrop();
                        if ((UnityEngine.Object)c3 == (UnityEngine.Object)null)
                        {
                            Card c4 = BruteAI.NextBestSpell();
                            if ((UnityEngine.Object)c4 == (UnityEngine.Object)null)
                                return BruteAI.LaunchNextBestHeroPower();
                            GameFunctions.doDropSpell(c4);
                            return true;
                        }
                        else
                        {
                            if (!GameFunctions.doDropMinion(c3))
                                return true;
                            int num = (UnityEngine.Object)c3.GetBattlecrySpell() != (UnityEngine.Object)null ? 1 : 0;
                            return true;
                        }
                    }
                    else
                    {
                        GameFunctions.doDropWeapon(c2);
                        return true;
                    }
                }
                else
                {
                    GameFunctions.doDropSecret(c1);
                    return true;
                }
            }
        }

        public static Card NextBestSecret()
        {
            foreach (Card c in Enumerable.ToList<Card>((IEnumerable<Card>)GameFunctions.myPlayer.GetHandZone().GetCards()))
            {
                Entity entity = c.GetEntity();
                if (entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() && entity.IsSecret() && GameFunctions.canBeUsed(c))
                    return c;
            }
            return (Card)null;
        }

        public static Card NextBestSpell()
        {
            foreach (Card c in Enumerable.ToList<Card>((IEnumerable<Card>)GameFunctions.myPlayer.GetHandZone().GetCards()))
            {
                Entity entity = c.GetEntity();
                if (entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() && entity.IsSpell() && (entity.GetCardId() != "GAME_005" && GameFunctions.canBeUsed(c)))
                    return c;
            }
            return (Card)null;
        }

        public static Card NextBestWeapon()
        {
            if ((UnityEngine.Object)GameFunctions.myPlayer.GetHero().GetWeaponCard() != (UnityEngine.Object)null)
                return (Card)null;
            foreach (Card c in Enumerable.ToList<Card>((IEnumerable<Card>)GameFunctions.myPlayer.GetHandZone().GetCards()))
            {
                Entity entity = c.GetEntity();
                if (entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() && entity.IsWeapon() && GameFunctions.canBeUsed(c))
                    return c;
            }
            return (Card)null;
        }

        public static bool LaunchNextBestHeroPower()
        {
            if (GameFunctions.myPlayer.GetNumAvailableResources() < GameFunctions.myPlayer.GetHeroPower().GetCost() || GameFunctions.myPlayer.GetHeroPower().IsExhausted() || (GameFunctions.myPlayer.GetHeroPower().IsBusy() || !GameFunctions.myPlayer.GetHeroPower().CanAttack()))
                return false;
            Log.debug("Use Heropower");
            switch (GameFunctions.myPlayer.GetHero().GetClass())
            {
                case TAG_CLASS.DRUID:
                case TAG_CLASS.HUNTER:
                case TAG_CLASS.PALADIN:
                case TAG_CLASS.ROGUE:
                case TAG_CLASS.SHAMAN:
                case TAG_CLASS.WARRIOR:
                    if (GameFunctions.canBeUsed(GameFunctions.myPlayer.GetHeroPower().GetCard()))
                        GameFunctions.doDropSpell(GameFunctions.myPlayer.GetHeroPower().GetCard());
                    return true;
                case TAG_CLASS.MAGE:
                    if (!GameFunctions.canBeUsed(GameFunctions.myPlayer.GetHeroPower().GetCard()))
                        return true;
                    GameFunctions.doDropSpell(GameFunctions.myPlayer.GetHeroPower().GetCard());
                    return GameFunctions.doTargetting(GameFunctions.ePlayer.GetHero());
                case TAG_CLASS.PRIEST:
                    if (!GameFunctions.canBeUsed(GameFunctions.myPlayer.GetHeroPower().GetCard()))
                        return true;
                    GameFunctions.doDropSpell(GameFunctions.myPlayer.GetHeroPower().GetCard());
                    return GameFunctions.doTargetting(GameFunctions.myPlayer.GetHero());
                case TAG_CLASS.WARLOCK:
                    return false;
                default:
                    return false;
            }
        }

        public static bool tryToPlayCoin()
        {
            bool flag = false;
            Card c = (Card)null;
            foreach (Card card in Enumerable.ToList<Card>((IEnumerable<Card>)GameFunctions.myPlayer.GetHandZone().GetCards()))
            {
                Entity entity = card.GetEntity();
                if (entity.GetCost() == GameFunctions.myPlayer.GetNumAvailableResources() + 1)
                    flag = true;
                if (entity.GetCardId() == "GAME_005")
                    c = card;
            }
            if (!flag || !((UnityEngine.Object)c != (UnityEngine.Object)null))
                return false;
            GameFunctions.doDropSpell(c);
            return true;
        }

        public static Card NextBestMinionDrop()
        {
            List<Card> list = Enumerable.ToList<Card>((IEnumerable<Card>)GameFunctions.myPlayer.GetHandZone().GetCards());
            if (GameFunctions.myPlayer.GetBattlefieldZone().GetCardCount() >= 7)
                return (Card)null;
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
                    if (c.GetEntity().HasTaunt() && entity.GetCardType() == TAG_CARDTYPE.MINION && (entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources() && GameFunctions.canBeUsed(c)))
                        return c;
                }
            }
            Card card = (Card)null;
            foreach (Card c in list)
            {
                Entity entity = c.GetEntity();
                if (entity.GetCardType() == TAG_CARDTYPE.MINION && entity.GetCost() <= GameFunctions.myPlayer.GetNumAvailableResources())
                {
                    if (GameFunctions.myPlayer.GetNumAvailableResources() == entity.GetCost() && GameFunctions.canBeUsed(c))
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
            if ((UnityEngine.Object)attacker == (UnityEngine.Object)null || (UnityEngine.Object)attackee == (UnityEngine.Object)null)
                return false;
            return GameFunctions.doAttack(attacker, attackee) ? true : true;
        }

        public static Card NextBestAttacker(Card attackee)
        {
            if ((UnityEngine.Object)attackee == (UnityEngine.Object)null)
                return (Card)null;
            List<Card> list = Enumerable.ToList<Card>((IEnumerable<Card>)GameFunctions.myPlayer.GetBattlefieldZone().GetCards());
            if ((UnityEngine.Object)attackee != (UnityEngine.Object)GameFunctions.ePlayer.GetHeroCard())
            {
                Card c1 = (Card)null;
                foreach (Card c2 in list)
                {
                    Entity entity = c2.GetEntity();
                    if (entity.GetATK() >= attackee.GetEntity().GetHealth() && entity.GetHealth() < attackee.GetEntity().GetATK() && (!attackee.GetEntity().HasTaunt() && GameFunctions.canBeUsed(c2)))
                        return c2;
                    if (entity.GetATK() >= attackee.GetEntity().GetHealth() && !attackee.GetEntity().HasTaunt() && GameFunctions.canBeUsed(c2))
                        c1 = c2;
                }
                if ((UnityEngine.Object)c1 != (UnityEngine.Object)null && GameFunctions.canBeUsed(c1))
                    return c1;
            }
            else
            {
                Entity hero = GameFunctions.myPlayer.GetHero();
                if (hero != null && GameFunctions.canBeUsed(hero.GetCard()))
                    return hero.GetCard();
            }
            Card card = (Card)null;
            foreach (Card c in list)
            {
                c.GetEntity();
                if (GameFunctions.canBeUsed(c))
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
                if (entity.HasTaunt() && entity.CanBeAttacked() && GameFunctions.canBeTargetted(card.GetEntity()))
                    return card;
            }
            if (GameFunctions.ePlayer.GetHeroCard().GetEntity().CanBeAttacked() && GameFunctions.canBeTargetted(GameFunctions.ePlayer.GetHeroCard().GetEntity()))
                return GameFunctions.ePlayer.GetHeroCard();
            foreach (Card card in list1)
            {
                if (card.GetEntity().CanBeAttacked() && GameFunctions.canBeTargetted(card.GetEntity()))
                    return card;
            }
            return (Card)null;
        }
    }
}
