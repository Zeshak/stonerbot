using System;
using System.Collections.Generic;
using UnityEngine;

namespace Plugin
{
    public static class GameFunctions
    {
        public static GameState gs;
        public static Player myPlayer;
        public static Player ePlayer;
        private static ZoneHand myHandZone;
        private static ZonePlay myPlayZone;
        private static ZoneWeapon myWeaponZone;
        private static ZoneSecret mySecretZone;
        public static List<CardDetails> CardDetails;

        public static void populateZones()
        {
            using (List<Zone>.Enumerator enumerator = ZoneMgr.Get().GetZones().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Zone current = enumerator.Current;
                    if (current.m_Side == Player.Side.FRIENDLY)
                    {
                        if (current is ZoneHand)
                            GameFunctions.myHandZone = (ZoneHand)current;
                        else if (current is ZonePlay)
                            GameFunctions.myPlayZone = (ZonePlay)current;
                        else if (current is ZoneWeapon)
                            GameFunctions.myWeaponZone = (ZoneWeapon)current;
                        else if (current is ZoneSecret)
                            GameFunctions.mySecretZone = (ZoneSecret)current;

                    }
                }
            }
        }

        public static void cancel()
        {
            GameState gameState = GameState.Get();
            if (gameState.IsInMainOptionMode())
                gameState.CancelCurrentOptionMode();
            if (!gameState.IsInTargetMode())
                return;
            gameState.GetGameEntity().NotifyOfTargetModeCancelled();
        }

        public static bool doAttack(Card attacker, Card attackee)
        {
            Log.log("DoAttack " + attacker.GetEntity().GetName() + " -> " + attackee.GetEntity().GetName());
            try
            {
                attacker.SetDoNotSort(true);
                iTween.Stop(attacker.gameObject);
                KeywordHelpPanelManager.Get().HideKeywordHelp();
                CardTypeBanner.Hide();
                attacker.NotifyPickedUp();
                GameFunctions.gs.GetGameEntity().NotifyOfCardGrabbed(attacker.GetEntity());
                GameFunctions.myPlayer.GetBattlefieldZone().UnHighlightBattlefield();
                if (InputManager.Get().DoNetworkResponse(attacker.GetEntity()))
                {
                    EnemyActionHandler.Get().NotifyOpponentOfCardPickedUp(attacker);
                    EnemyActionHandler.Get().NotifyOpponentOfTargetModeBegin(attacker);
                    GameFunctions.gs.GetGameEntity().NotifyOfBattlefieldCardClicked(attackee.GetEntity(), true);
                    GameFunctions.myPlayer.GetBattlefieldZone().UnHighlightBattlefield();
                    if (InputManager.Get().DoNetworkResponse(attackee.GetEntity()))
                    {
                        EnemyActionHandler.Get().NotifyOpponentOfTargetEnd();
                        GameFunctions.myPlayer.GetHandZone().UpdateLayout(-1, true);
                        GameFunctions.myPlayer.GetBattlefieldZone().UpdateLayout();
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static List<CardDetails> GetBattlefieldCardDetails()
        {
            List<CardDetails> listCards = new List<CardDetails>();
            foreach (Card card in GameFunctions.myPlayer.GetBattlefieldZone().GetCards())
            {
                CardDetails c = CardDetails.Find(cd => cd.Card.GetEntity().GetCardId() == card.GetEntity().GetCardId());
                Entity entity = card.GetEntity();
                CardDetails cDet = new CardDetails();
                cDet.Card = card;
                switch (entity.GetRarity())
                {
                    case TAG_RARITY.LEGENDARY:
                        cDet.SetInitValues();
                        cDet.SpellOnThis = true;
                        break;
                    case TAG_RARITY.EPIC:
                        cDet.SetInitValues();
                        break;
                    case TAG_RARITY.RARE:
                        cDet.SetInitValues();
                        break;
                    case TAG_RARITY.COMMON:
                        cDet.SetInitValues();
                        break;
                    case TAG_RARITY.FREE:
                        cDet.SetInitValues();
                        break;
                }
                SetNewValuesByCardStatus(ref cDet);
                listCards.Add(cDet);
            }
            return listCards;
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
                    cd.SpellOnThis = true;
            }
            else if (entity.HasTaunt())
            {
                //Se usa silence si tiene: (*)Menos de 3 ataque y más de 2 de vida (no silencia las 2-2, las va a matar luego) (*)Si está bufeada y le sumaron 2 a algún atributo
                if ((currATK < 3 && currHP > 2) || (currATK > origATK + 1) || ((currHP > origHP + 1)))
                    cd.SilenceThis = true;
                //Bicho con taunt a partir de 3-6 o 5-3 le tiro spell si tengo
                if ((currATK > 2 && currHP > 5) || (currHP > 2 && currATK > 4))
                    cd.SpellOnThis = true;
            }
            else if (currATK > 4 && currHP > 4)
            {
                //Si es bicho fuerte, 5-5 en adelante, trato de tirarle spell
                cd.SpellOnThis = true;
            }
        }

        public static bool doDropWeapon(Card c)
        {
            return GameFunctions.doDrop(c);
        }

        public static bool doDropBattlecry(Card c)
        {
            return GameFunctions.doDrop(c);
        }

        public static bool doDropSpell(Card c)
        {
            return GameFunctions.doDrop(c);
        }

        public static bool doDropSecret(Card c)
        {
            return GameFunctions.doDrop(c);
        }

        public static bool doDropMinion(Card c)
        {
            return GameFunctions.doDrop(c);
        }

        private static bool doDrop(Card c)
        {
            Log.log("DoDrop " + c.GetEntity().GetName());
            try
            {
                PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
                c.SetDoNotSort(true);
                iTween.Stop(c.gameObject);
                KeywordHelpPanelManager.Get().HideKeywordHelp();
                CardTypeBanner.Hide();
                DragCardSoundEffects component = c.GetActor().GetComponent<DragCardSoundEffects>();
                if ((UnityEngine.Object)component != (UnityEngine.Object)null)
                    component.Disable();
                ProjectedShadow componentInChildren = c.GetActor().GetComponentInChildren<ProjectedShadow>();
                if ((UnityEngine.Object)componentInChildren != (UnityEngine.Object)null)
                    componentInChildren.DisableShadow();
                c.NotifyPickedUp();
                GameFunctions.gs.GetGameEntity().NotifyOfCardGrabbed(c.GetEntity());
                c.SetDoNotSort(false);
                c.NotifyLeftPlayfield();
                bool doTargetting = false;
                bool isWeapon = c.GetEntity().IsWeapon();
                bool isHeroPower = c.GetEntity().IsHeroPower();
                int dropPlace = 0;
                Zone destinationZone;
                if (isWeapon)
                {
                    dropPlace = 1;
                    destinationZone = GameFunctions.myWeaponZone;
                }
                else
                {
                    destinationZone = GameFunctions.myPlayZone;
                    dropPlace = destinationZone.GetCards().Count + 1;
                }
                if (dropPlace >= 8 && !isWeapon)
                    return true;
                GameFunctions.gs.GetGameEntity().NotifyOfCardDropped(c.GetEntity());
                GameFunctions.gs.SetSelectedOptionPosition(dropPlace);
                if (InputManager.Get().DoNetworkResponse(c.GetEntity()))
                {
                    Log.log("DoNetworkResponse Loaded");
                    c.GetEntity().GetZonePosition();
                    if (c.GetEntity().IsSecret())
                        ZoneMgr.Get().AddLocalZoneChange(c, (Zone)GameFunctions.mySecretZone, GameFunctions.mySecretZone.GetLastPos());
                    else
                        ZoneMgr.Get().AddLocalZoneChange(c, destinationZone, dropPlace);
                    GameFunctions.myPlayer.NotifyOfSpentMana(c.GetEntity().GetRealTimeCost());
                    GameFunctions.myPlayer.UpdateManaCounter();
                    ManaCrystalMgr.Get().UpdateSpentMana(c.GetEntity().GetRealTimeCost());
                    if (GameFunctions.gs.EntityHasTargets(c.GetEntity()))
                        doTargetting = true;
                }
                else
                    GameFunctions.gs.SetSelectedOptionPosition(Network.NoPosition);
                GameFunctions.myPlayer.GetHandZone().UpdateLayout(-1, true);
                GameFunctions.myPlayer.GetBattlefieldZone().SortWithSpotForHeldCard(-1);
                if (doTargetting)
                {
                    Log.log(" DoDropMinion doing targetting");
                    if (EnemyActionHandler.Get() != null)
                    {
                        EnemyActionHandler.Get().NotifyOpponentOfTargetModeBegin(c);

                        Entity targetEntity = null;

                        var eHero = ePlayer.GetHeroCard().GetEntity();
                        if (gs.IsValidOptionTarget(eHero))
                        {
                            Log.log("Can attack hero");
                            targetEntity = eHero;
                        }

                        // get a list of my cards on the battlefield
                        List<Card> myPlayedCards = myPlayer.GetBattlefieldZone().GetCards();
                        if (myPlayedCards.Count > 0 && targetEntity == null)
                        {
                            foreach (Card card in myPlayedCards)
                            {
                                var e = card.GetEntity();
                                if (gs.IsValidOptionTarget(e))
                                {
                                    Log.log("is valid target: " + e.GetName());
                                    Log.log("considering for battlecry: " + e.GetName());
                                    targetEntity = e;
                                }
                                else
                                {
                                    Log.log("is NOT valid target: " + e.GetName());
                                }
                            }
                        }

                        // get a list of enemy cards on the battlefield
                        List<Card> ePlayedCards = ePlayer.GetBattlefieldZone().GetCards();
                        if (ePlayedCards.Count > 0 && targetEntity == null)
                        {
                            foreach (Card card in ePlayedCards)
                            {
                                var e = card.GetEntity();
                                if (gs.IsValidOptionTarget(e))
                                {
                                    Log.log("is valid target: " + e.GetName());
                                    Log.log("considering for battlecry: " + e.GetName());
                                    targetEntity = e;
                                }
                                else
                                {
                                    Log.log("is NOT valid target: " + e.GetName());
                                }
                            }
                        }
                        if (targetEntity == null)
                        {
                            Log.log(" No target entity selected");
                            return false;
                        }

                        Log.log("selected targetEntity: " + targetEntity.GetName());
                        gs.GetGameEntity().NotifyOfBattlefieldCardClicked(targetEntity, true);

                        myPlayer.GetBattlefieldZone().UnHighlightBattlefield();
                        Log.log(" Response mode pre: " + gs.GetResponseMode().ToString());
                        if (InputManager.Get().DoNetworkResponse(targetEntity))
                        {
                            Log.log(" Response mode post: " + gs.GetResponseMode().ToString());
                            EnemyActionHandler.Get().NotifyOpponentOfTargetEnd();

                            myPlayer.GetHandZone().UpdateLayout(-1, true);
                            myPlayer.GetBattlefieldZone().UpdateLayout();
                            Log.log(" did battlecry on: " + targetEntity.GetName());
                        }
                        else
                        {
                            Log.log(" DoTarget outer DoNetworkReponse failed");
                        }
                    }
                }
                else
                    EnemyActionHandler.Get().NotifyOpponentOfCardDropped();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static Entity findBestSpellTarget(Spell s)
        {
            return (Entity)null;
        }

        public static bool doTargetting(Entity target)
        {
            int num = 10;
            for (int index = 0; index < num; ++index)
            {
                InputManager inputManager = InputManager.Get();
                if (!GameState.Get().IsInTargetMode())
                    return false;
                if (GameFunctions.canBeTargetted(target))
                {
                    GameFunctions.cancel();
                    return false;
                }
                else if (inputManager.DoNetworkResponse(target))
                    return true;
            }
            GameFunctions.cancel();
            return false;
        }

        public static bool canBeTargetted(Entity e)
        {
            return e.GetCard().GetActor().GetActorStateType() != ActorStateType.CARD_VALID_TARGET;
        }

        public static bool canBeUsed(Card c)
        {
            if ((UnityEngine.Object)c.GetActor() == (UnityEngine.Object)null)
                return false;
            else
                return c.GetActor().GetActorStateType().Equals((object)ActorStateType.CARD_PLAYABLE);
        }

        public static bool doMulligan()
        {
            if ((UnityEngine.Object)MulliganManager.Get().GetMulliganButton() == (UnityEngine.Object)null || !MulliganManager.Get().GetMulliganButton().IsEnabled())
                return false;
            int num = 0;
            using (List<Card>.Enumerator enumerator = GameFunctions.myPlayer.GetHandZone().GetCards().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Card current = enumerator.Current;
                    if (current.GetEntity().GetCost() >= 4)
                    {
                        ++num;
                        MulliganManager.Get().ToggleHoldState(current);
                    }
                }
            }
            GameFunctions.doEndTurn();
            TurnStartManager.Get().BeginListeningForTurnEvents();
            MulliganManager.Get().EndMulliganEarly();
            Log.say("Mulligan ended : " + (object)num + " cards changed");
            return true;
        }

        public static void doEndTurn()
        {
            InputManager.Get().DoEndTurnButton();
        }

        private static bool PlayPowerUpSpell(Card card)
        {
            if ((UnityEngine.Object)card == (UnityEngine.Object)null)
                return false;
            Spell actorSpell = card.GetActorSpell(SpellType.POWER_UP);
            if ((UnityEngine.Object)actorSpell == (UnityEngine.Object)null)
                return false;
            actorSpell.ActivateState(SpellStateType.BIRTH);
            return true;
        }

        private static bool PlayPlaySpell(Card card)
        {
            if ((UnityEngine.Object)card == (UnityEngine.Object)null)
                return false;
            Spell playSpell = card.GetPlaySpell();
            if ((UnityEngine.Object)playSpell == (UnityEngine.Object)null)
                return false;
            playSpell.ActivateState(SpellStateType.BIRTH);
            return true;
        }

        private static void ForceManaUpdate(Entity entity)
        {
            Player localPlayer = GameState.Get().GetLocalPlayer();
            int num1 = entity.GetRealTimeCost() - localPlayer.GetRealTimeTempMana();
            if (localPlayer.GetRealTimeTempMana() > 0)
            {
                int num2 = Mathf.Clamp(entity.GetRealTimeCost(), 0, localPlayer.GetRealTimeTempMana());
                localPlayer.NotifyOfUsedTempMana(num2);
                ManaCrystalMgr.Get().DestroyTempManaCrystals(num2);
            }
            if (num1 > 0)
            {
                localPlayer.NotifyOfSpentMana(num1);
                ManaCrystalMgr.Get().UpdateSpentMana(num1);
            }
            localPlayer.UpdateManaCounter();
        }
    }
}
