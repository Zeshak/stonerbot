﻿using System;
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
        public static int gameTurn;


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

        public static void Cancel()
        {
            GameState gameState = GameState.Get();
            if (gameState.IsInMainOptionMode())
                gameState.CancelCurrentOptionMode();
            if (!gameState.IsInTargetMode())
                return;
            gameState.GetGameEntity().NotifyOfTargetModeCancelled();
        }

        public static bool DoAttack(Card attacker, Card attackee)
        {
            Log.debug("DoAttack " + attacker.GetEntity().GetName() + " -> " + attackee.GetEntity().GetName());
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
            foreach (Card card in GameFunctions.ePlayer.GetBattlefieldZone().GetCards())
            {
                CardDetails cd = (CardDetails.FindInCardDetails(card) ?? new CardDetails());
                Entity entity = card.GetEntity();
                cd.CardId = card.GetEntity().GetCardId();
                cd.CardName = card.name;
                cd.Card = card;
                if (entity.GetRarity() == TAG_RARITY.LEGENDARY)
                {
                    cd.DisableThis = true;
                }
                CardDetails.SetNewValuesByCardStatus(ref cd);
                listCards.Add(cd);
            }
            return listCards;
        }

        public static bool DoDrop(Card c)
        {
            return DoDrop(c, null, -1);
        }

        public static bool DoDrop(Card c, Entity certainTarget)
        {
            return DoDrop(c, certainTarget, -1);
        }

        public static bool DoDrop(Card c, Entity certainTarget, int battlefieldPosition)
        {
            Log.debug("DoDrop " + c.GetEntity().GetName());
            if (certainTarget != null)
                Log.debug("DoDrop on " + certainTarget.GetName());
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
                bool needsTargeting = false;
                int dropPlace = 0;
                Zone destinationZone;
                if (c.GetEntity().IsWeapon())
                {
                    dropPlace = 1;
                    destinationZone = GameFunctions.myWeaponZone;
                }
                else
                {
                    destinationZone = GameFunctions.myPlayZone;
                    if (destinationZone.GetCards().Count + 1 >= 8)
                        return true;
                    if (battlefieldPosition == -1)
                        dropPlace = destinationZone.GetCards().Count + 1;
                    else
                        dropPlace = battlefieldPosition;
                }
                GameFunctions.gs.GetGameEntity().NotifyOfCardDropped(c.GetEntity());
                GameFunctions.gs.SetSelectedOptionPosition(dropPlace);
                if (InputManager.Get().DoNetworkResponse(c.GetEntity()))
                {
                    c.GetEntity().GetZonePosition();
                    if (!c.GetEntity().IsHeroPower())
                    {
                        if (c.GetEntity().IsSecret())
                            ZoneMgr.Get().AddLocalZoneChange(c, (Zone)GameFunctions.mySecretZone, GameFunctions.mySecretZone.GetLastPos());
                        else
                            ZoneMgr.Get().AddLocalZoneChange(c, destinationZone, dropPlace);
                    }
                    GameFunctions.myPlayer.NotifyOfSpentMana(c.GetEntity().GetRealTimeCost());
                    GameFunctions.myPlayer.UpdateManaCounter();
                    ManaCrystalMgr.Get().UpdateSpentMana(c.GetEntity().GetRealTimeCost());
                    if (GameFunctions.gs.EntityHasTargets(c.GetEntity()))
                        needsTargeting = true;
                }
                else
                    GameFunctions.gs.SetSelectedOptionPosition(Network.NoPosition);
                GameFunctions.myPlayer.GetHandZone().UpdateLayout(-1, true);
                GameFunctions.myPlayer.GetBattlefieldZone().SortWithSpotForHeldCard(-1);
                if (needsTargeting)
                {
                    if (EnemyActionHandler.Get() != null)
                    {
                        EnemyActionHandler.Get().NotifyOpponentOfTargetModeBegin(c);
                        Entity targetEntity = null;

                        if (certainTarget == null)
                        {
                            targetEntity = BruteAI.GetGenericTarget();
                            if (targetEntity == null)
                            {
                                Cancel();
                                return false;
                            }
                        }
                        else
                            targetEntity = certainTarget;

                        Log.debug("Por castear algo directo en: " + targetEntity.GetName());
                        DoTargetting(targetEntity);


                    }
                    else
                        EnemyActionHandler.Get().NotifyOpponentOfCardDropped();
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.error(ex);
                return false;
            }
        }

        private static bool SetEntityTarget(Entity entity)
        {
            Network.Options optionsPacket = GameFunctions.gs.GetOptionsPacket();
            if (optionsPacket != null)
            {
                Network.Options.Option option = optionsPacket.List.Find(q => q.Main.ID == entity.GetEntityId());
                option.Main.Targets.Add(0);
            }
            return true;
        }

        public static Entity findBestSpellTarget(Spell s)
        {
            return (Entity)null;
        }

        public static bool DoTargetting(Entity target)
        {
            if (!GameState.Get().IsInTargetMode())
                return false;
            gs.GetGameEntity().NotifyOfBattlefieldCardClicked(target, true);
            myPlayer.GetBattlefieldZone().UnHighlightBattlefield();
            if (!GameFunctions.CanBeTargetted(target))
            {
                GameFunctions.Cancel();
                return false;
            }
            else
            {
                if (InputManager.Get().DoNetworkResponse(target))
                {
                    EnemyActionHandler.Get().NotifyOpponentOfTargetEnd();

                    myPlayer.GetHandZone().UpdateLayout(-1, true);
                    myPlayer.GetBattlefieldZone().UpdateLayout();
                    return true;
                }
                else
                    Log.debug(" DoTarget outer DoNetworkReponse failed");
                GameFunctions.Cancel();
                return false;
            }
        }

        public static bool CanBeTargetted(Entity e)
        {
            if (e.GetCard().GetActor() == null)
                return false;
            else
                return e.GetCard().GetActor().GetActorStateType().Equals(ActorStateType.CARD_VALID_TARGET);
        }

        public static bool CanBeUsed(Card c)
        {
            if (c.GetActor() == null)
                return false;
            else
                return c.GetActor().GetActorStateType().Equals(ActorStateType.CARD_PLAYABLE);
        }

        public static bool IsEnemyCard(Card c)
        {
            return GameFunctions.ePlayer.GetBattlefieldZone().GetCards().Contains(c);
        }

        public static bool DoMulligan()
        {
            if (MulliganManager.Get().GetMulliganButton() == null || !MulliganManager.Get().GetMulliganButton().IsEnabled())
                return false;
            int num = 0;
            foreach (Card card in GameFunctions.myPlayer.GetHandZone().GetCards())
            {
                if (card.GetEntity().GetCost() >= 4)
                {
                    MulliganManager.Get().ToggleHoldState(card);
                    num++;
                }
            }
            GameFunctions.DoEndTurn();
            TurnStartManager.Get().BeginListeningForTurnEvents();
            MulliganManager.Get().EndMulligan();
            GameFunctions.gameTurn = 0;
            Log.say("Mulligan ended : " + num.ToString() + " cards changed");
            return true;
        }

        public static void DoEndTurn()
        {
            InputManager.Get().DoEndTurnButton();
            GameFunctions.gameTurn++;
            Plugin.BotStatus = Plugin.BotStatusList.OnMatchTurn;
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
