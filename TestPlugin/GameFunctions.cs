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
        public static List<Card> massiveDropList = new List<Card>();
        public static List<Card> massiveAttackList;
        public static Card massiveAttackAttackee;
        public static TurnStates? turnState;
        public static bool canWinThisTurn = false;
        public static bool canWinThisTurnFirstTimeDone = false;
        public enum TurnStates
        {
            CHECK_CANWIN,
            CHECK_DISABLEDESTROYSILENCE,
            DROP_DISABLEDESTROYSILENCE,
            DROP_MINIONS,
            DROP_SECRETS,
            DROP_WEAPONS,
            DROP_SECONDSPELL,
            DO_HEROPOWER
        }


        public static void PopulateZones()
        {
            foreach (Zone zone in ZoneMgr.Get().GetZones())
            {
                if (zone.m_Side == Player.Side.FRIENDLY)
                {
                    if (zone is ZoneHand)
                        GameFunctions.myHandZone = (ZoneHand)zone;
                    else if (zone is ZonePlay)
                        GameFunctions.myPlayZone = (ZonePlay)zone;
                    else if (zone is ZoneWeapon)
                        GameFunctions.myWeaponZone = (ZoneWeapon)zone;
                    else if (zone is ZoneSecret)
                        GameFunctions.mySecretZone = (ZoneSecret)zone;

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

        public static bool DoMassiveAttack(List<Card> myAttackers, Card hisAttackee)
        {
            massiveAttackList = myAttackers;
            massiveAttackAttackee = hisAttackee;
            if (massiveAttackList.Count > 0)
            {
                if (DoAttack(massiveAttackList[0], hisAttackee))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool DoAttack(Card myAttacker, Card hisAttackee)
        {
            try
            {
                Log.debug("Tratando de atacar con " + myAttacker.ToString() + " ---> " + hisAttackee.ToString());
                if (!GameFunctions.CanAttack(myAttacker) || !hisAttackee.GetEntity().CanBeAttacked())
                    return true;
                myAttacker.SetDoNotSort(true);
                iTween.Stop(myAttacker.gameObject);
                KeywordHelpPanelManager.Get().HideKeywordHelp();
                CardTypeBanner.Hide();
                myAttacker.NotifyPickedUp();
                GameFunctions.gs.GetGameEntity().NotifyOfCardGrabbed(myAttacker.GetEntity());
                GameFunctions.myPlayer.GetBattlefieldZone().UnHighlightBattlefield();
                if (myAttacker.GetEntity().GetRealTimeAttack() >= hisAttackee.GetEntity().GetRealTimeRemainingHP())
                {
                    massiveAttackAttackee = null;
                    massiveAttackList.Clear();
                    Plugin.Delay(5000);
                }
                else
                {
                    if (massiveAttackList != null && massiveAttackList.Count > 0)
                    {
                        Log.debug("Saco a " + massiveAttackList[0].ToString() + " de la lista de atacantes");
                        massiveAttackList.RemoveAt(0);
                    }
                    Plugin.Delay(3000);
                }
                if (InputManager.Get().DoNetworkResponse(myAttacker.GetEntity()))
                {
                    EnemyActionHandler.Get().NotifyOpponentOfCardPickedUp(myAttacker);
                    EnemyActionHandler.Get().NotifyOpponentOfTargetModeBegin(myAttacker);
                    GameFunctions.gs.GetGameEntity().NotifyOfBattlefieldCardClicked(hisAttackee.GetEntity(), true);
                    GameFunctions.myPlayer.GetBattlefieldZone().UnHighlightBattlefield();
                    if (InputManager.Get().DoNetworkResponse(hisAttackee.GetEntity()))
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
                    cd.DestroyThis = true;
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

        public static bool DoDrop(Card myCard, Entity certainTarget, int battlefieldPosition)
        {
            Log.debug("DoDrop " + myCard.GetEntity().GetName());
            if (certainTarget != null)
                Log.debug("DoDrop on " + certainTarget.GetName());
            try
            {
                PegCursor.Get().SetMode(PegCursor.Mode.STOPDRAG);
                ReflectionFunctions.GrabCard(myCard);

                InputManager input_man = InputManager.Get();
                if (input_man.heldObject == null)
                {
                    Log.debug("Nothing held, when trying to drop");
                    return false;
                }
                ZonePlay m_myPlayZone = ReflectionFunctions.get_m_myPlayZone();
                ZoneHand m_myHandZone = ReflectionFunctions.get_m_myHandZone();

                myCard.SetDoNotSort(false);
                iTween.Stop(myCard.gameObject);
                Entity myCardEntity = myCard.GetEntity();
                myCard.NotifyLeftPlayfield();
                GameState.Get().GetGameEntity().NotifyOfCardDropped(myCardEntity);
                m_myPlayZone.UnHighlightBattlefield();
                DragCardSoundEffects component2 = myCard.GetComponent<DragCardSoundEffects>();
                if (component2)
                {
                    component2.Disable();
                }
                UnityEngine.Object.Destroy(input_man.heldObject.GetComponent<DragRotator>());
                input_man.heldObject = null;
                ProjectedShadow componentInChildren = myCard.GetActor().GetComponentInChildren<ProjectedShadow>();
                if (componentInChildren != null)
                {
                    componentInChildren.DisableShadow();
                }

                // Check that the card is on the hand or is spellpower
                Zone card_zone = myCard.GetZone();
                if (((card_zone == null) || card_zone.m_ServerTag != TAG_ZONE.HAND) && !myCardEntity.IsHeroPower())
                {
                    return false;
                }

                bool needsTargeting = false;
                bool is_minion = myCardEntity.IsMinion();
                bool is_weapon = myCardEntity.IsWeapon();

                Log.debug("Drop card continued");
                if (is_minion || is_weapon)
                {
                    Zone destinationZone = (!is_weapon) ? (Zone)m_myPlayZone : (Zone)m_myHandZone;
                    if (destinationZone)
                    {
                        GameState gameState = GameState.Get();
                        int card_position = Network.NoPosition;
                        if (is_minion)
                        {
                            if (destinationZone.GetCards().Count + 1 >= 8)
                                return true;
                            if (battlefieldPosition == -1)
                                battlefieldPosition = destinationZone.GetCards().Count + 1;
                            card_position = ZoneMgr.Get().PredictZonePosition(destinationZone, battlefieldPosition);
                            gameState.SetSelectedOptionPosition(card_position);
                        }
                        else
                            battlefieldPosition = 1;
                        if (input_man.DoNetworkResponse(myCardEntity))
                        {
                            if (is_weapon)
                            {
                                ReflectionFunctions.set_m_lastZoneChangeList(ZoneMgr.Get().AddLocalZoneChange(myCard, destinationZone, destinationZone.GetLastPos()));
                            }
                            else
                            {
                                ReflectionFunctions.set_m_lastZoneChangeList(ZoneMgr.Get().AddPredictedLocalZoneChange(myCard, destinationZone, battlefieldPosition, card_position));
                            }
                            ReflectionFunctions.ForceManaUpdate(myCardEntity);
                            if (is_minion && gameState.EntityHasTargets(myCardEntity))
                            {
                                needsTargeting = true;
                                if (TargetReticleManager.Get())
                                {
                                    bool showArrow = true;
                                    TargetReticleManager.Get().CreateFriendlyTargetArrow(myCardEntity, myCardEntity, true, showArrow, null);
                                }
                                ReflectionFunctions.set_m_battlecrySourceCard(myCard);
                            }
                        }
                        else
                        {
                            gameState.SetSelectedOptionPosition(Network.NoPosition);
                        }
                    }
                }
                else
                {
                    if (myCardEntity.IsSpell())
                    {
                        Plugin.Delay(5000);
                        if (GameState.Get().EntityHasTargets(myCardEntity))
                        {
                            needsTargeting = true;
                            if (TargetReticleManager.Get())
                            {
                                bool showArrow = true;
                                TargetReticleManager.Get().CreateFriendlyTargetArrow(myCardEntity, myCardEntity, true, showArrow, null);
                            }
                            ReflectionFunctions.set_m_battlecrySourceCard(myCard);
                        }
                        if (!GameState.Get().HasResponse(myCardEntity))
                        {
                            PlayErrors.DisplayPlayError(PlayErrors.GetPlayEntityError(myCardEntity), myCardEntity);
                        }
                        else
                        {
                            input_man.DoNetworkResponse(myCardEntity);
                            if (myCardEntity.IsSecret())
                            {
                                ZoneSecret m_mySecretZone = ReflectionFunctions.get_m_mySecretZone();
                                ReflectionFunctions.set_m_lastZoneChangeList(ZoneMgr.Get().AddLocalZoneChange(myCard, m_mySecretZone, m_mySecretZone.GetLastPos()));
                            }
                            else
                            {
                                ReflectionFunctions.set_m_lastZoneChangeList(ZoneMgr.Get().AddLocalZoneChange(myCard, TAG_ZONE.PLAY));
                            }
                            ReflectionFunctions.ForceManaUpdate(myCardEntity);
                            ReflectionFunctions.PlayPowerUpSpell(myCard);
                            ReflectionFunctions.PlayPlaySpell(myCard);
                        }
                    }
                    else if (myCardEntity.IsHeroPower())
                    {
                        Plugin.Delay(5000);
                        if (input_man.DoNetworkResponse(myCardEntity))
                        {
                            GameFunctions.myPlayer.NotifyOfSpentMana(myCardEntity.GetRealTimeCost());
                            GameFunctions.myPlayer.UpdateManaCounter();
                            ManaCrystalMgr.Get().UpdateSpentMana(myCardEntity.GetRealTimeCost());
                            if (GameFunctions.gs.EntityHasTargets(myCardEntity))
                            {
                                needsTargeting = true;
                                if (TargetReticleManager.Get())
                                {
                                    bool showArrow = true;
                                    TargetReticleManager.Get().CreateFriendlyTargetArrow(myCardEntity, myCardEntity, true, showArrow, null);
                                }
                            }
                        }
                    }
                }
                GameFunctions.myHandZone.UpdateLayout(-1, true);
                GameFunctions.myPlayZone.SortWithSpotForHeldCard(-1);
                CardDetails cd = CardDetails.FindInCardDetails(myCard);
                if (cd != null && cd.CardDelay != 0)
                    Plugin.Delay(cd.CardDelay);
                else
                    Plugin.Delay(3000);

                if (needsTargeting)
                {
                    if (EnemyActionHandler.Get())
                    {
                        EnemyActionHandler.Get().NotifyOpponentOfTargetModeBegin(myCard);

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
                        GameFunctions.DoTargetting(targetEntity);
                    }
                }
                else
                {
                    if (GameState.Get().GetResponseMode() != GameState.ResponseMode.SUB_OPTION)
                    {
                        EnemyActionHandler.Get().NotifyOpponentOfCardDropped();
                    }
                }
                Log.debug("Drop card finished");
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
            if (!target.CanBeTargetedByOpponents())
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

        public static bool CanBeUsed(Card c)
        {
            if (c.GetActor() == null)
                return false;
            else
                return c.GetActor().GetActorStateType().Equals(ActorStateType.CARD_PLAYABLE);
        }

        public static bool CanAttack(Card c)
        {
            return (c.GetEntity().CanAttack() && !c.GetEntity().IsExhausted() && !c.GetEntity().IsFrozen() && c.GetEntity().GetRealTimeAttack() > 0);
        }

        public static bool IsEnemyCard(Card c)
        {
            return GameFunctions.ePlayer.GetBattlefieldZone().GetCards().Contains(c);
        }

        public static bool DoMulligan()
        {
            if (MulliganManager.Get().GetMulliganButton() == null || !MulliganManager.Get().GetMulliganButton().IsEnabled())
                return false;
            foreach (Card card in GameFunctions.myPlayer.GetHandZone().GetCards())
                if (card.GetEntity().GetCost() >= 4 || !card.GetEntity().IsMinion())
                    MulliganManager.Get().ToggleHoldState(card);

            MulliganManager.Get().EndMulligan();
            GameFunctions.DoEndTurn();
            return true;
        }

        public static void DoEndTurn()
        {
            Log.debug("Intenta pasar el turno 2");
            InputManager.Get().DoEndTurnButton();
            Log.debug("Apretó el botón.");
            GameFunctions.turnState = null;
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
