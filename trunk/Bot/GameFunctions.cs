// Type: BotOfStone.GameFunctions
// Assembly: Bot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3E3A6658-12BF-4012-BBAC-A043D3FC1901
// Assembly location: C:\Users\Nicolas\Desktop\Hearthstone Bot\plugins\Bot.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BotOfStone
{
  public static class GameFunctions
  {
    public static GameState gs;
    public static Player myPlayer;
    public static Player ePlayer;
    private static ZoneHand m_myHandZone;
    private static ZonePlay m_myPlayZone;
    private static ZoneWeapon m_myWeaponZone;
    private static ZoneSecret m_mySecretZone;

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
              GameFunctions.m_myHandZone = (ZoneHand) current;
            else if (current is ZonePlay)
              GameFunctions.m_myPlayZone = (ZonePlay) current;
            else if (current is ZoneWeapon)
              GameFunctions.m_myWeaponZone = (ZoneWeapon) current;
            else if (current is ZoneSecret)
              GameFunctions.m_mySecretZone = (ZoneSecret) current;
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

    public static bool doDropWeapon(Card c)
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
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
          component.Disable();
        ProjectedShadow componentInChildren = c.GetActor().GetComponentInChildren<ProjectedShadow>();
        if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
          componentInChildren.DisableShadow();
        c.NotifyPickedUp();
        GameFunctions.gs.GetGameEntity().NotifyOfCardGrabbed(c.GetEntity());
        c.SetDoNotSort(false);
        c.NotifyLeftPlayfield();
        bool flag1 = false;
        bool flag2 = c.GetEntity().IsWeapon();
        int num;
        Zone destinationZone;
        if (flag2)
        {
          num = 1;
          destinationZone = (Zone) GameFunctions.m_myWeaponZone;
        }
        else
        {
          destinationZone = (Zone) GameFunctions.m_myPlayZone;
          num = destinationZone.GetCards().Count + 1;
        }
        if (num >= 8 && !flag2)
          return true;
        GameFunctions.gs.GetGameEntity().NotifyOfCardDropped(c.GetEntity());
        GameFunctions.gs.SetSelectedOptionPosition(num);
        if (InputManager.Get().DoNetworkResponse(c.GetEntity()))
        {
          c.GetEntity().GetZonePosition();
          if (c.GetEntity().IsSecret())
            ZoneMgr.Get().AddLocalZoneChange(c, (Zone) GameFunctions.m_mySecretZone, GameFunctions.m_mySecretZone.GetLastPos());
          else
            ZoneMgr.Get().AddLocalZoneChange(c, destinationZone, num);
          GameFunctions.myPlayer.NotifyOfSpentMana(c.GetEntity().GetRealTimeCost());
          GameFunctions.myPlayer.UpdateManaCounter();
          ManaCrystalMgr.Get().UpdateSpentMana(c.GetEntity().GetRealTimeCost());
          if (GameFunctions.gs.EntityHasTargets(c.GetEntity()))
            flag1 = true;
        }
        else
          GameFunctions.gs.SetSelectedOptionPosition(Network.NoPosition);
        GameFunctions.myPlayer.GetHandZone().UpdateLayout(-1, true);
        GameFunctions.myPlayer.GetBattlefieldZone().SortWithSpotForHeldCard(-1);
        if (flag1)
        {
          if ((UnityEngine.Object) EnemyActionHandler.Get() != (UnityEngine.Object) null)
            EnemyActionHandler.Get().NotifyOpponentOfTargetModeBegin(c);
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
      return (Entity) null;
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
      if ((UnityEngine.Object) c.GetActor() == (UnityEngine.Object) null)
        return false;
      else
        return c.GetActor().GetActorStateType().Equals((object) ActorStateType.CARD_PLAYABLE);
    }

    public static bool doMulligan()
    {
      if ((UnityEngine.Object) MulliganManager.Get().GetMulliganButton() == (UnityEngine.Object) null || !MulliganManager.Get().GetMulliganButton().IsEnabled())
        return false;
      int num = 0;
      using (List<Card>.Enumerator enumerator = GameFunctions.myPlayer.GetHandZone().GetCards().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          Card current = enumerator.Current;
          if (current.GetEntity().GetCost() > 4)
          {
            ++num;
            MulliganManager.Get().ToggleHoldState(current);
          }
        }
      }
      GameFunctions.doEndTurn();
      TurnStartManager.Get().BeginListeningForTurnEvents();
      MulliganManager.Get().EndMulliganEarly();
      Log.say("Mulligan ended : " + (object) num + " cards changed");
      return true;
    }

    public static void doEndTurn()
    {
      InputManager.Get().DoEndTurnButton();
    }

    private static bool PlayPowerUpSpell(Card card)
    {
      if ((UnityEngine.Object) card == (UnityEngine.Object) null)
        return false;
      Spell actorSpell = card.GetActorSpell(SpellType.POWER_UP);
      if ((UnityEngine.Object) actorSpell == (UnityEngine.Object) null)
        return false;
      actorSpell.ActivateState(SpellStateType.BIRTH);
      return true;
    }

    private static bool PlayPlaySpell(Card card)
    {
      if ((UnityEngine.Object) card == (UnityEngine.Object) null)
        return false;
      Spell playSpell = card.GetPlaySpell();
      if ((UnityEngine.Object) playSpell == (UnityEngine.Object) null)
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
