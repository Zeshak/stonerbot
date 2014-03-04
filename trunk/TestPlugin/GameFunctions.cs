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
        public static List<CardDetails> CardDetails = new List<CardDetails>();

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
            Log.debug("Cantidad de cartas " + GameFunctions.ePlayer.GetBattlefieldZone().GetCards().Count.ToString());
            foreach (Card card in GameFunctions.ePlayer.GetBattlefieldZone().GetCards())
            {
                /*foreach (CardDetails cd in CardDetails)
                CardDetails c = CardDetails.Find(cd => cd.Card.GetEntity().GetCardId() == card.GetEntity().GetCardId());*/
                Entity entity = card.GetEntity();
                CardDetails cDet = new CardDetails();
                cDet.Card = card;
                Log.debug("3");
                switch (entity.GetRarity())
                {
                    case TAG_RARITY.LEGENDARY:
                        cDet.SetInitValues();
                        cDet.SpellThis = true;
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
            Log.debug("Entra en SetNewValuesByCardStatus");
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
                    cd.SpellThis = true;
            }
            else if (entity.HasTaunt())
            {
                //Se usa silence si tiene: (*)Menos de 3 ataque y más de 2 de vida (no silencia las 2-2, las va a matar luego) (*)Si está bufeada y le sumaron 2 a algún atributo
                if ((currATK < 3 && currHP > 2) || (currATK > origATK + 1) || ((currHP > origHP + 1)))
                    cd.SilenceThis = true;
                //Bicho con taunt a partir de 3-6 o 5-3 le tiro spell si tengo
                if ((currATK > 2 && currHP > 5) || (currHP > 2 && currATK > 4))
                    cd.SpellThis = true;
            }
            else if (currATK > 4 && currHP > 4)
            {
                //Si es bicho fuerte, 5-5 en adelante, trato de tirarle spell
                cd.SpellThis = true;
            }
            Log.debug("Termina en SetNewValuesByCardStatus");
        }

        public static bool DoDropWeapon(Card c)
        {
            return GameFunctions.DoDrop(c);
        }

        public static bool DoDropBattlecry(Card c)
        {
            return GameFunctions.DoDrop(c);
        }

        public static bool DoDropSpell(Card c)
        {
            return GameFunctions.DoDrop(c);
        }

        public static bool DoDropHeroPowerSpell()
        {
            return GameFunctions.DoDrop(GameFunctions.myPlayer.GetHeroPower().GetCard());
        }

        public static bool doDropSecret(Card c)
        {
            return GameFunctions.DoDrop(c);
        }

        public static bool doDropMinion(Card c)
        {
            return GameFunctions.DoDrop(c);
        }

        private static bool DoDrop(Card c, Entity certainTarget = null)
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
                    dropPlace = destinationZone.GetCards().Count + 1;
                    if (dropPlace >= 8)
                        return true;
                }
                GameFunctions.gs.GetGameEntity().NotifyOfCardDropped(c.GetEntity());
                GameFunctions.gs.SetSelectedOptionPosition(dropPlace);
                if (InputManager.Get().DoNetworkResponse(c.GetEntity()))
                {
                    Log.log("DoNetworkResponse Loaded");
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
                    Log.log("DoDrop hace target");
                    if (EnemyActionHandler.Get() != null)
                    {
                        EnemyActionHandler.Get().NotifyOpponentOfTargetModeBegin(c);
                        Entity targetEntity = null;

                        if (certainTarget != null)
                        {
                            if (c.GetEntity().IsHeroPower())
                            {
                                switch (GameFunctions.myPlayer.GetHero().GetClass())
                                {
                                    case TAG_CLASS.MAGE:
                                        targetEntity = GetBestMageHeroPowerTarget();
                                        if (targetEntity == null)
                                        {
                                            GameFunctions.Cancel();
                                            return false;
                                        }
                                        Log.debug("Heropower mage en " + targetEntity.GetName());
                                        break;
                                    case TAG_CLASS.PRIEST:
                                        targetEntity = GameFunctions.myPlayer.GetHero();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                var eHero = ePlayer.GetHeroCard().GetEntity();
                                if (gs.IsValidOptionTarget(eHero))
                                {
                                    Log.log("Can attack hero");
                                    targetEntity = eHero;
                                }

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
                            }
                        }
                        else 
                            targetEntity = certainTarget;

                        DoTargetting(targetEntity);


                    }
                    else
                        EnemyActionHandler.Get().NotifyOpponentOfCardDropped();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private static Entity GetBestMageHeroPowerTarget()
        {
            Log.debug("Corro para buscar el mejor Hero power target");
            List<CardDetails> listCards = GameFunctions.GetBattlefieldCardDetails();
            Entity target = new Entity();
            int maxAtk = 0;
            int index = -1;
            foreach (CardDetails cd in listCards)
            {
                Entity possibleTarget = cd.Card.GetEntity();
                if (possibleTarget.GetHealth() == 1 || possibleTarget.HasDivineShield())
                {
                    int estimatedAtk = possibleTarget.GetATK();
                    if (possibleTarget.HasWindfury())
                        estimatedAtk *= 2;
                    if (possibleTarget.HasTaunt())
                        estimatedAtk += 1;
                    if (maxAtk == 0 || estimatedAtk > maxAtk)
                    {
                        if (GameFunctions.CanBeTargetted(possibleTarget))
                        {
                            index = listCards.IndexOf(cd);
                            maxAtk = estimatedAtk;
                        }
                    }
                }
            }
            if (index != -1)
                return listCards[index].Card.GetEntity();
            else if (GameFunctions.CanBeTargetted(ePlayer.GetHeroCard().GetEntity()))
                return ePlayer.GetHeroCard().GetEntity();
            else return null;
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
                    Log.log(" DoTarget outer DoNetworkReponse failed");
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
