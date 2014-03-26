using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Plugin
{
    public class Plugin : MonoBehaviour
    {
        #region -[ Attributes ]-

        public enum BotStatusList
        {
            OnLogin,
            OnHub,
            OnGamePlay,
            OnMulligan,
            OnMulliganWaitEnemy,
            OnPracticeDeckSel,
            OnPracticeQueue,
            OnMatchDeckSel,
            OnMatchDeckQueue,
            OnMatchTurnOwn,
            OnMatchTurnEnemy,
            OnEndGameScreen
        }

        public class Deck
        {
            public long DeckId { get; set; }
            public string Alias { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }

            public Deck()
            {
            }
        }

        private static DateTime delay_start;
        private static long delay_length;

        public static Deck selDeck;
        public static BotStatusList BotStatus;
        public static float maxQueueTime = 120f;
        public static bool playVsHumans = true;
        public static bool playRanked = false;
        public static bool playExpert = false;
        private System.Random rng;
        private static bool itsOn;
        private static bool finishAfterThisGame;
        public static float timeLastQueued;
        public static bool statisticsAdded;
        public static bool mulliganDone;
        public static int modulo;
        private static Thread socketThread;
        public static bool saidHi = false;
        public static bool saidGG = false;
        public static bool needsToSetQuestSet = false;
        public static string questString;

        #endregion

        #region -[ Events ]-

        public static void init()
        {
            var go = SceneMgr.Get().gameObject; // attach to SceneMgr since it always exists
            // destroy any old versions of ourself
            GameObject.Destroy(go.GetComponent("Plugin"));
            foreach (var x in go.GetComponents<Plugin>())
            {
                GameObject.Destroy(x);
            }
            go.AddComponent<Plugin>();
        }

        public void Awake()     // This is called after loading the DLL, before the loader gives back control to Unity
        {

        }

        public void Update()    // This is called every frame from Unity's main thread
        {
            if ((DateTime.Now - delay_start).TotalMilliseconds < delay_length)
                return;
            Delay(2000);
            try
            {
                if (Plugin.itsOn)
                    this.Mainloop();
            }
            catch (Exception ex)
            {
                Log.error(ex);
            }
            return;
        }

        public void Start()     // This is called after control is given back to Unity
        {
            Plugin.socketThread = new Thread(new ThreadStart(SocketHandler.InitSocketListener));
            Plugin.socketThread.Start();
            rng = new System.Random();
            timeLastQueued = Time.realtimeSinceStartup;
        }

        public void OnDestroy()
        {
            Plugin.socketThread.Abort();
            SocketHandler.tcpListener.Stop();
            SocketHandler.stream.Close();
            if (SocketHandler.tcpClient != null)
                SocketHandler.tcpClient.Close();
        }

        public void Init_Game()
        {
            try
            {
                GameFunctions.gs = GameState.Get();
                GameFunctions.myPlayer = GameFunctions.gs.GetLocalPlayer();
                GameFunctions.ePlayer = GameFunctions.gs.GetFirstOpponentPlayer(GameFunctions.myPlayer);
                CardDetails.SetCardDetails();
                InactivePlayerKicker.Get().SetShouldCheckForInactivity(false);
            }
            catch (Exception ex)
            {
                Log.error(ex);
            }
        }

        #endregion

        #region -[ Private Members ]-

        public static void Delay(long msec)
        {
            delay_start = DateTime.Now;
            delay_length = msec;
        }

        private void Mainloop()
        {
            SceneMgr.Mode curMode = SceneMgr.Get().GetMode();
            switch (curMode)
            {
                case SceneMgr.Mode.INVALID:
                case SceneMgr.Mode.FATAL_ERROR:
                case SceneMgr.Mode.RESET:
                    Log.say("Fatal Error, in AI.tick()");
                    Log.say("Force closing game!");
                    OnDestroy();
                    Environment.FailFast(null);
                    break;
                case SceneMgr.Mode.STARTUP:
                case SceneMgr.Mode.COLLECTIONMANAGER:
                case SceneMgr.Mode.PACKOPENING:
                case SceneMgr.Mode.FRIENDLY:
                case SceneMgr.Mode.DRAFT:
                case SceneMgr.Mode.CREDITS:
                    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
                    Delay(5000);
                    break;
                case SceneMgr.Mode.LOGIN:
                    DoLogin();
                    Delay(2500);
                    break;
                case SceneMgr.Mode.HUB:
                    if (!needsToSetQuestSet)
                        Delay(5000);
                    else
                        Delay(2500);
                    DoHub();
                    break;
                case SceneMgr.Mode.GAMEPLAY:
                    GameState gameState = GameState.Get();
                    if (!gameState.IsInTargetMode())
                        GameFunctions.Cancel();
                    Delay(2500);
                    DoGameplay();
                    break;
                case SceneMgr.Mode.PRACTICE:
                    DoPractice();
                    Delay(5000);
                    break;
                case SceneMgr.Mode.TOURNAMENT:
                    DoTournament();
                    Delay(5000);
                    break;
                default:
                    Log.error("Mainloop derrapó a default. Mode: " + curMode.ToString());
                    break;
            }
        }

        private void DoPractice()
        {
            if (SceneMgr.Get().IsInGame())
                return;
            Plugin.statisticsAdded = false;
            Plugin.mulliganDone = false;
            long selectedDeckId = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
            double num2 = new System.Random().NextDouble();
            MissionID mission1;
            MissionID mission2;
            if (num2 < 0.1)
            {
                mission1 = MissionID.AI_NORMAL_MAGE;
                mission2 = MissionID.AI_EXPERT_MAGE;
            }
            else if (num2 >= 0.1 && num2 < 0.2)
            {
                mission1 = MissionID.AI_NORMAL_DRUID;
                mission2 = MissionID.AI_EXPERT_DRUID;
            }
            else if (num2 >= 0.2 && num2 < 0.3)
            {
                mission1 = MissionID.AI_NORMAL_HUNTER;
                mission2 = MissionID.AI_EXPERT_HUNTER;
            }
            else if (num2 >= 0.3 && num2 < 0.4)
            {
                mission1 = MissionID.AI_NORMAL_PALADIN;
                mission2 = MissionID.AI_EXPERT_PALADIN;
            }
            else if (num2 >= 0.4 && num2 < 0.5)
            {
                mission1 = MissionID.AI_NORMAL_PRIEST;
                mission2 = MissionID.AI_EXPERT_PRIEST;
            }
            else if (num2 >= 0.5 && num2 < 0.6)
            {
                mission1 = MissionID.AI_NORMAL_ROGUE;
                mission2 = MissionID.AI_EXPERT_ROGUE;
            }
            else if (num2 >= 0.6 && num2 < 0.7)
            {
                mission1 = MissionID.AI_NORMAL_SHAMAN;
                mission2 = MissionID.AI_EXPERT_SHAMAN;
            }
            else if (num2 >= 0.7 && num2 < 0.8)
            {
                mission1 = MissionID.AI_NORMAL_WARLOCK;
                mission2 = MissionID.AI_EXPERT_WARLOCK;
            }
            else
            {
                mission1 = MissionID.AI_NORMAL_WARRIOR;
                mission2 = MissionID.AI_EXPERT_WARRIOR;
            }
            if (!Plugin.playExpert)
                GameMgr.Get().StartGame(GameMode.PRACTICE, mission1, selectedDeckId);
            else
                GameMgr.Get().StartGame(GameMode.PRACTICE, mission2, selectedDeckId);
        }

        private void DoLogin()
        {
            BotStatus = BotStatusList.OnLogin;
            if (WelcomeQuests.Get() == null)
                return;
            WelcomeQuests.Get().m_clickCatcher.TriggerRelease();
            BotStatus = BotStatusList.OnHub;
        }

        public static void FindTotalDominance()
        {
            string race = questString.Split(' ')[0];
            List<Achievement> list = AchieveManager.Get().GetActiveQuests();
            if (list.Count == 0 || !AchieveManager.Get().CanCancelQuest(list[0].ID))
            {
                Plugin.StopBot(null, null, null);
                return;
            }
            if (list[0].Name.Contains(race) && list[0].Name.Contains("Dominance"))
            {
                Plugin.StopBot(null, null, null);
                return;
            }
            Log.debug("Cancelling " + list[0].Name);
            AchieveManager.Get().CancelQuest(list[0].ID);
        }

        private void DoHub()
        {
            if (needsToSetQuestSet)
            {
                FindTotalDominance();
                return;
            }
            if (playVsHumans)
            {
                SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
                BotStatus = BotStatusList.OnMatchDeckSel;
            }
            else
            {
                SceneMgr.Get().SetNextMode(SceneMgr.Mode.PRACTICE);
                BotStatus = BotStatusList.OnPracticeDeckSel;
            }
        }

        private void DoGameplay()
        {
            Init_Game();
            if (GameFunctions.gs.IsMulliganPhase())
            {
                if (!Plugin.mulliganDone)
                {
                    try
                    {
                        BotStatus = BotStatusList.OnMulligan;
                        Plugin.mulliganDone = GameFunctions.DoMulligan();
                        return;
                    }
                    catch (Exception ex)
                    {
                        Log.error(ex);
                        return;
                    }
                }
            }
            if (GameFunctions.gs.IsGameOver())
            {
                try
                {
                    if (!Plugin.statisticsAdded)
                    {
                        Plugin.AddStatistics(!(GameFunctions.myPlayer.GetHero().GetRemainingHP() <= 0));
                        Plugin.statisticsAdded = true;
                    }
                    if (EndGameScreen.Get() == null)
                        return;
                    EndGameScreen.Get().m_hitbox.TriggerRelease();
                    saidHi = false;
                    saidGG = false;
                    return;
                }
                catch (Exception ex)
                {
                    Log.error(ex);
                    return;
                }
            }
            if (!GameFunctions.gs.IsLocalPlayerTurn())
            {
                Plugin.BotStatus = Plugin.BotStatusList.OnMatchTurnEnemy;
                return;
            }
            try
            {
                Plugin.BotStatus = Plugin.BotStatusList.OnMatchTurnOwn;
                if (GameState.Get().IsBlockingServer())
                    Thread.Sleep(500);
                GameFunctions.PopulateZones();
                BruteAI.SendEmoMessages();
                if (BruteAI.BruteHand())
                    ++BruteAI.loops;
                else
                {
                    if (BruteAI.BruteAttack())
                        return;
                    GameFunctions.DoEndTurn();
                    BruteAI.loops = 0;
                }
            }
            catch (Exception ex)
            {
                Log.error(ex);
            }
        }

        private void DoTournament()
        {
            float num1 = UnityEngine.Time.realtimeSinceStartup - Plugin.timeLastQueued;
            if (!Plugin.finishAfterThisGame)
            {
                try
                {
                    if (!SceneMgr.Get().IsInGame() && !Network.IsMatching() && !Network.IsInDraftQueue() && !(BotStatus == BotStatusList.OnMatchDeckQueue) && !(BotStatus == BotStatusList.OnPracticeQueue) || (double)num1 > (double)Plugin.maxQueueTime)
                    {
                        Plugin.statisticsAdded = false;
                        Plugin.mulliganDone = false;
                        long currentDeckId = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
                        foreach (CollectionDeck coldeck in CollectionManager.Get().GetDecks().Values)
                        {
                            if (coldeck.ID == currentDeckId)
                            {
                                Log.debug("Deck name: " + coldeck.Name + " Col id: " + coldeck.ID.ToString());
                                selDeck = new Deck();
                                selDeck.DeckId = currentDeckId;
                                selDeck.Alias = coldeck.Name;
                                break;
                            }
                        }
                        Log.say("Queuing for game against human with deck " + selDeck.DeckId);
                        if (Plugin.playRanked)
                        {
                            GameMgr.Get().SetNextGame(GameMode.RANKED_PLAY, MissionID.MULTIPLAYER_1v1);
                            Network.TrackClient(Network.TrackLevel.LEVEL_INFO, Network.TrackWhat.TRACK_PLAY_TOURNAMENT_WITH_CUSTOM_DECK);
                            Network.RankedMatch(selDeck.DeckId);
                        }
                        else
                        {
                            GameMgr.Get().SetNextGame(GameMode.UNRANKED_PLAY, MissionID.MULTIPLAYER_1v1);
                            Network.TrackClient(Network.TrackLevel.LEVEL_INFO, Network.TrackWhat.TRACK_PLAY_CASUAL_WITH_CUSTOM_DECK);
                            Network.UnrankedMatch(selDeck.DeckId);
                        }
                        BotStatus = BotStatusList.OnMatchDeckQueue;
                        Plugin.timeLastQueued = UnityEngine.Time.realtimeSinceStartup;
                        FriendChallengeMgr.Get().OnEnteredMatchmakerQueue();
                        PresenceMgr.Get().SetStatus(PresenceStatus.PLAY_QUEUE);
                    }
                    else
                    {
                        Plugin.timeLastQueued = UnityEngine.Time.realtimeSinceStartup;
                    }
                }
                catch (Exception ex)
                {
                    Log.error(ex);
                }
            }
            else
            {
                Plugin.itsOn = false;
                Plugin.finishAfterThisGame = false;
            }
        }

        public static void AddStatistics(bool win)
        {
            if (selDeck != null)
                File.AppendAllText("stat.txt", "[Date]" + DateTime.Now.ToString() + "[ID]" + selDeck.DeckId.ToString() + "[Name]" + selDeck.Alias.ToString() + "[Result]" + win.ToString() + Environment.NewLine);
            else
                File.AppendAllText("stat.txt", "[Date]" + DateTime.Now.ToString() + "[ID]El bot fue detenido[Name]El bot fue detenido[Result]" + win.ToString() + Environment.NewLine);
        }

        #endregion

        #region -[ Cheat Handlers ]-

        public static bool StartBot(string func, string[] args, string rawArgs)
        {
            Log.say("Bot started");
            Plugin.playVsHumans = true;
            Plugin.timeLastQueued = UnityEngine.Time.realtimeSinceStartup;
            Plugin.itsOn = true;
            Plugin.playRanked = false;
            return true;
        }

        public static bool StartBotVsAI(string func, string[] args, string rawArgs)
        {
            Log.say("Bot started VS AI");
            Plugin.playVsHumans = false;
            Plugin.playExpert = false;
            Plugin.itsOn = true;
            return true;
        }

        public static bool StartBotVsAIExpert(string func, string[] args, string rawArgs)
        {
            Log.say("Bot started VS AI Expert");
            Plugin.playVsHumans = false;
            Plugin.playExpert = true;
            Plugin.itsOn = true;
            return true;
        }

        public static bool AnalyzeMyHand(string func, string[] args, string rawArgs)
        {
            if (rawArgs == null)
            {
                Log.say("---------------------------------- My hand ----------------------------------");
                foreach (Card card in GameState.Get().GetLocalPlayer().GetHandZone().GetCards())
                {
                    Entity entity = card.GetEntity();
                    Log.debug("-------------------------------------------------------------");
                    Log.debug("Damage: " + entity.GetDamage());
                    Log.debug("GetRaceText: " + entity.GetRaceText());
                    Power pow = entity.GetAttackPower();
                    Log.debug("AttackPower:");
                    if (pow != null)
                    {
                        PlayErrors.PlayRequirementInfo asd = pow.GetPlayRequirementInfo();
                        Log.debug("paramMaxAtk: " + asd.paramMaxAtk);
                        Log.debug("paramMinAtk: " + asd.paramMinAtk);
                        Log.debug("paramMinNumEnemyMinions: " + asd.paramMinNumEnemyMinions);
                        Log.debug("paramMinNumTotalMinions: " + asd.paramMinNumTotalMinions);
                        Log.debug("paramNumMinionSlots: " + asd.paramNumMinionSlots);
                        Log.debug("paramNumMinionSlotsWithTarget: " + asd.paramNumMinionSlotsWithTarget);
                        Log.debug("paramRace: " + asd.paramRace);
                        Log.debug("requirementsMap: " + asd.requirementsMap);
                    }
                    pow = entity.GetMasterPower();
                    Log.debug("MasterPower:");
                    if (pow != null)
                    {
                        PlayErrors.PlayRequirementInfo asd = pow.GetPlayRequirementInfo();
                        Log.debug("paramMaxAtk: " + asd.paramMaxAtk);
                        Log.debug("paramMinAtk: " + asd.paramMinAtk);
                        Log.debug("paramMinNumEnemyMinions: " + asd.paramMinNumEnemyMinions);
                        Log.debug("paramMinNumTotalMinions: " + asd.paramMinNumTotalMinions);
                        Log.debug("paramNumMinionSlots: " + asd.paramNumMinionSlots);
                        Log.debug("paramNumMinionSlotsWithTarget: " + asd.paramNumMinionSlotsWithTarget);
                        Log.debug("paramRace: " + asd.paramRace);
                        Log.debug("requirementsMap: " + asd.requirementsMap);
                    }
                    Log.debug("ATK: " + entity.GetStringTag(entity.GetTag(GAME_TAG.ATK)));
                    Log.debug("SPELLPOWER: " + entity.GetStringTag(entity.GetTag(GAME_TAG.SPELLPOWER)));
                    Log.debug("SPELLPOWER_DOUBLE: " + entity.GetStringTag(entity.GetTag(GAME_TAG.SPELLPOWER_DOUBLE)));

                    Spell spell = card.GetBattlecrySpell();
                    if (spell != null)
                    {
                        GUIText asd = spell.guiText;
                        if (asd != null)
                            Log.debug("GUIText" + asd.text);

                        Log.debug("GetBattlecrySpell " + spell.name);
                        Card e = spell.GetPowerSourceCard();
                        if (e != null)
                            Log.debug("GetPowerSourceCard " + e.name);
                        SuperSpell sspell = spell.GetSuperSpellParent();
                    }

                    spell = card.GetAttackSpell();
                    if (spell != null)
                    {
                        GUIText asd = spell.guiText;
                        if (asd != null)
                            Log.debug("GUIText" + asd.text);

                        Log.debug("GetAttackSpell " + spell.name);
                        Card e = spell.GetPowerSourceCard();
                        if (e != null)
                            Log.debug("GetPowerSourceCard " + e.name);
                        SuperSpell sspell = spell.GetSuperSpellParent();
                    }

                    spell = card.GetBestDeathSpell();
                    if (spell != null)
                    {
                        GUIText asd = spell.guiText;
                        if (asd != null)
                            Log.debug("GUIText" + asd.text);

                        Log.debug("GetBestDeathSpell " + spell.name);
                        Card e = spell.GetPowerSourceCard();
                        if (e != null)
                            Log.debug("GetPowerSourceCard " + e.name);
                        SuperSpell sspell = spell.GetSuperSpellParent();
                    }

                    spell = card.GetBestSpawnSpell();
                    if (spell != null)
                    {
                        GUIText asd = spell.guiText;
                        if (asd != null)
                            Log.debug("GUIText" + asd.text);

                        Log.debug("GetBestSpawnSpell " + spell.name);
                        Card e = spell.GetPowerSourceCard();
                        if (e != null)
                            Log.debug("GetPowerSourceCard " + e.name);
                        SuperSpell sspell = spell.GetSuperSpellParent();
                    }

                    spell = card.GetBestSummonSpell();
                    if (spell != null)
                    {
                        GUIText asd = spell.guiText;
                        if (asd != null)
                            Log.debug("GUIText" + asd.text);

                        Log.debug("GetBestSummonSpell " + spell.name);
                        Card e = spell.GetPowerSourceCard();
                        if (e != null)
                            Log.debug("GetPowerSourceCard " + e.name);
                        SuperSpell sspell = spell.GetSuperSpellParent();
                    }

                    spell = card.GetDeathSpell();
                    if (spell != null)
                    {
                        GUIText asd = spell.guiText;
                        if (asd != null)
                            Log.debug("GUIText" + asd.text);

                        Log.debug("GetDeathSpell " + spell.name);
                        Card e = spell.GetPowerSourceCard();
                        if (e != null)
                            Log.debug("GetPowerSourceCard " + e.name);
                        SuperSpell sspell = spell.GetSuperSpellParent();
                    }

                    spell = card.GetPlaySpell();
                    if (spell != null)
                    {
                        GUIText asd = spell.guiText;
                        if (asd != null)
                            Log.debug("GUIText" + asd.text);

                        Log.debug("GetPlaySpell " + spell.name);
                        Card e = spell.GetPowerSourceCard();
                        if (e != null)
                            Log.debug("GetPowerSourceCard " + e.name);
                        SuperSpell sspell = spell.GetSuperSpellParent();
                    }

                    spell = card.GetLifetimeSpell();
                    if (spell != null)
                    {
                        GUIText asd = spell.guiText;
                        if (asd != null)
                            Log.debug("GUIText" + asd.text);

                        Log.debug("GetLifetimeSpell " + spell.name);
                        Card e = spell.GetPowerSourceCard();
                        if (e != null)
                            Log.debug("GetPowerSourceCard " + e.name);
                        SuperSpell sspell = spell.GetSuperSpellParent();
                    }


                    Log.debug("Card : " + card.ToString());
                    if (entity.HasBattlecry())
                        Log.debug("Battlecry : " + card.GetBattlecrySpell().GetType().ToString());
                    Log.debug("ActorState : " + ((object)card.GetActor().GetActorStateType()).ToString());
                    Log.debug("-------------------------------------------------------------");

                }
                Log.say("-----------------------------------------------------------------------------");
            }
            else
            {
                Card card = GameState.Get().GetLocalPlayer().GetHandZone().GetCardAtPos(Convert.ToInt32(rawArgs));
                Entity entity = card.GetEntity();
                Log.debug("-------------------------------------------------------------");
                Log.debug("Damage: " + entity.GetDamage());
                Log.debug("GetRaceText: " + entity.GetRaceText());
                Power pow = entity.GetAttackPower();
                if (pow != null)
                    Log.debug("GetAttackPower: " + pow.GetDefinition());
                pow = entity.GetMasterPower();
                Log.debug("MasterPower:");
                if (pow != null)
                {
                    PlayErrors.PlayRequirementInfo asd = pow.GetPlayRequirementInfo();
                    Log.debug("paramMaxAtk: " + asd.paramMaxAtk);
                    Log.debug("paramMinAtk: " + asd.paramMinAtk);
                    Log.debug("paramMinNumEnemyMinions: " + asd.paramMinNumEnemyMinions);
                    Log.debug("paramMinNumTotalMinions: " + asd.paramMinNumTotalMinions);
                    Log.debug("paramNumMinionSlots: " + asd.paramNumMinionSlots);
                    Log.debug("paramNumMinionSlotsWithTarget: " + asd.paramNumMinionSlotsWithTarget);
                    Log.debug("paramRace: " + asd.paramRace);
                    Log.debug("requirementsMap: " + asd.requirementsMap);
                }

                Spell spell = card.GetPlaySpell();
                if (spell != null)
                {
                    GUIText asd = spell.guiText;
                    if (asd != null)
                        Log.debug("GUIText" + asd.text);

                    Log.debug("spellname " + spell.name);

                    Card e = spell.GetPowerSourceCard();
                    if (e != null)
                        Log.debug("GetPowerSourceCard " + e.name);
                    SuperSpell sspell = spell.GetSuperSpellParent();
                }
                Log.debug("Card : " + card.ToString());
                if (entity.HasBattlecry())
                    Log.debug("Battlecry : " + card.GetBattlecrySpell().GetType().ToString());
                Log.debug("ActorState : " + ((object)card.GetActor().GetActorStateType()).ToString());
                Log.debug("-------------------------------------------------------------");
            }
            return true;
        }

        public static bool AnalyzeMyField(string func, string[] args, string rawArgs)
        {
            if (rawArgs == null)
            {
                Log.say("---------------------------------- My field ----------------------------------");
                foreach (Card card in GameState.Get().GetLocalPlayer().GetBattlefieldZone().GetCards())
                {
                    Entity entity = card.GetEntity();
                    Log.debug("Card : " + card.ToString());
                    if (entity.HasBattlecry())
                        Log.debug("Battlecry : " + card.GetBattlecrySpell().GetType().ToString());
                    Log.debug("GetRealTimeRemainingHP" + entity.GetRealTimeRemainingHP());
                    Log.debug("GetRemainingHP" + entity.GetRemainingHP());
                    Log.debug("GetHealth" + entity.GetHealth());
                    Log.debug("GetOriginalHealth" + entity.GetOriginalHealth());
                    Log.debug("GetRealTimeAttack" + entity.GetRealTimeAttack());
                    Log.debug("GetDamage" + entity.GetDamage());
                    Log.debug("GetATK" + entity.GetATK());
                    Log.debug("GetOriginalATK" + entity.GetOriginalATK());
                    Log.debug("ActorState : " + ((object)card.GetActor().GetActorStateType()).ToString());
                }
                Log.say("------------------------------------------------------------------------");
            }
            else
            {
                Card card = GameState.Get().GetLocalPlayer().GetBattlefieldZone().GetCardAtPos(Convert.ToInt32(rawArgs));
                Entity entity = card.GetEntity();
                Log.debug("Card : " + card.ToString());
                if (entity.HasBattlecry())
                    Log.debug("Battlecry : " + card.GetBattlecrySpell().GetType().ToString());
                Log.debug("GetRealTimeRemainingHP" + entity.GetRealTimeRemainingHP());
                Log.debug("GetRemainingHP" + entity.GetRemainingHP());
                Log.debug("GetHealth" + entity.GetHealth());
                Log.debug("GetOriginalHealth" + entity.GetOriginalHealth());
                Log.debug("GetRealTimeAttack" + entity.GetRealTimeAttack());
                Log.debug("GetDamage" + entity.GetDamage());
                Log.debug("GetATK" + entity.GetATK());
                Log.debug("GetOriginalATK" + entity.GetOriginalATK());
                Log.debug("ActorState : " + ((object)card.GetActor().GetActorStateType()).ToString());
            }
            return true;
        }

        public static bool AnalyzeHisField(string func, string[] args, string rawArgs)
        {
            return true;
        }

        public static bool AnalyzeCards(string func, string[] args, string rawArgs)
        {
            Log.say("-------------------------- ANALYSIS -----------------------");
            Log.say("-------------- My hand --------------");
            foreach (Card card in Enumerable.ToList<Card>((IEnumerable<Card>)GameState.Get().GetLocalPlayer().GetHandZone().GetCards()))
            {
                Entity entity = card.GetEntity();
                Log.debug("Card : " + card.ToString());
                Log.debug("Type : " + entity.GetType().ToString());
                Log.debug("Name : " + card.name);
                Log.debug("ATK : " + card.guiText.text);
                Log.debug("ATK : " + card.GetEntity().GetCardTextInHand());
                if (entity.HasBattlecry())
                    Log.debug("Battlecry : " + card.GetBattlecrySpell().ToString());
                Log.debug("ActorState : " + ((object)card.GetActor().GetActorStateType()).ToString());
                Log.debug("Rarity : " + (object)entity.GetTag(GAME_TAG.RARITY));
                Log.debug("EntityID : " + (object)entity.GetTag(GAME_TAG.ENTITY_ID));
            }
            Log.say("-------------- My battlefield --------------");
            foreach (Card card in Enumerable.ToList<Card>((IEnumerable<Card>)GameState.Get().GetLocalPlayer().GetBattlefieldZone().GetCards()))
            {
                Entity entity = card.GetEntity();
                Log.debug("Card : " + card.ToString());
                Log.debug("Type : " + entity.GetType().ToString());
                if (entity.HasBattlecry())
                    Log.debug("Battlecry : " + card.GetBattlecrySpell().GetType().ToString());
                Log.debug("ActorState : " + ((object)card.GetActor().GetActorStateType()).ToString());
            }
            Log.say("-------------- My hero Power --------------");
            Card heroPowerCard = GameState.Get().GetLocalPlayer().GetHeroPowerCard();
            Entity entity1 = heroPowerCard.GetEntity();
            Log.debug("Card : " + heroPowerCard.ToString());
            Log.debug("Type : " + entity1.GetType().ToString());
            Log.debug("ActorState : " + ((object)heroPowerCard.GetActor().GetActorStateType()).ToString());
            Log.say("-------------- My hero --------------");
            Card heroCard1 = GameState.Get().GetLocalPlayer().GetHeroCard();
            Entity entity2 = heroCard1.GetEntity();
            Log.debug("Card : " + heroCard1.ToString());
            Log.debug("Type : " + entity2.GetType().ToString());
            Log.debug("ActorState : " + ((object)heroCard1.GetActor().GetActorStateType()).ToString());
            Log.say("-------------- E battlefield --------------");
            foreach (Card card in Enumerable.ToList<Card>((IEnumerable<Card>)GameFunctions.ePlayer.GetBattlefieldZone().GetCards()))
            {
                Entity entity3 = card.GetEntity();
                Log.debug("Card : " + card.ToString());
                Log.debug("Type : " + entity3.GetType().ToString());
                Log.debug("ActorState : " + ((object)card.GetActor().GetActorStateType()).ToString());
            }
            Log.say("-------------- E hero --------------");
            Card heroCard2 = GameFunctions.ePlayer.GetHeroCard();
            Entity entity4 = heroCard2.GetEntity();
            Log.debug("Card : " + heroCard2.ToString());
            Log.debug("Type : " + entity4.GetType().ToString());
            Log.debug("ActorState : " + ((object)heroCard2.GetActor().GetActorStateType()).ToString());
            Log.say("-----------------------------------------------------------------");
            return true;
        }

        public static bool FinishThisGame(string func, string[] args, string rawArgs)
        {
            Log.say("Bot will stop after this game");
            Plugin.finishAfterThisGame = true;
            return true;
        }

        public static bool StartBotRanked(string func, string[] args, string rawArgs)
        {
            Log.say("Bot started in ranked mode");
            Plugin.timeLastQueued = UnityEngine.Time.realtimeSinceStartup;
            Plugin.itsOn = true;
            Plugin.playVsHumans = true;
            Plugin.playRanked = true;
            return true;
        }

        public static bool StopBot(string func, string[] args, string rawArgs)
        {
            Log.say("Bot stopped");
            Plugin.needsToSetQuestSet = false;
            Plugin.itsOn = false;
            return true;
        }

        #endregion
    }
}
