using System;
using System.Collections.Generic;
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

        private static double minTimeBetweenRuns = 1.0;
        public static float maxQueueTime = 120f;
        public static bool playVsHumans = true;
        public static bool playRanked = false;
        public static bool playExpert = false;
        private float timeLastRun;
        private System.Random rng;
        private static bool run;
        private static bool finishAfterThisGame;
        public static float timeLastQueued;
        public static bool statisticsAdded;
        public static bool mulliganDone;
        public static long currentDeckId;
        public static int modulo;
        private static Thread socket;

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
            CheatMgr.Get().RegisterCheatHandler("startbot", new CheatMgr.ProcessCheatCallback(Plugin.StartBot));
            CheatMgr.Get().RegisterCheatHandler("startvsai", new CheatMgr.ProcessCheatCallback(Plugin.StartBotVsAI));
            CheatMgr.Get().RegisterCheatHandler("startvsaiexpert", new CheatMgr.ProcessCheatCallback(Plugin.StartBotVsAIExpert));
            CheatMgr.Get().RegisterCheatHandler("startbotranked", new CheatMgr.ProcessCheatCallback(Plugin.StartBotRanked));
            CheatMgr.Get().RegisterCheatHandler("stopbot", new CheatMgr.ProcessCheatCallback(Plugin.StopBot));
            CheatMgr.Get().RegisterCheatHandler("analyze", new CheatMgr.ProcessCheatCallback(Plugin.AnalyzeCards));
            CheatMgr.Get().RegisterCheatHandler("deckid", new CheatMgr.ProcessCheatCallback(Plugin.GetDeckId));
            CheatMgr.Get().RegisterCheatHandler("finishthisgame", new CheatMgr.ProcessCheatCallback(Plugin.FinishThisGame));
            CheatMgr.Get().RegisterCheatHandler("help", new CheatMgr.ProcessCheatCallback(Plugin.help));            
        }

        public void Update()    // This is called every frame from Unity's main thread
        {
            if (UnityEngine.Time.realtimeSinceStartup - this.timeLastRun < Plugin.minTimeBetweenRuns)
                return;
            this.timeLastRun = UnityEngine.Time.realtimeSinceStartup;
            Plugin.minTimeBetweenRuns = new System.Random().NextDouble() * 2.0 + 2.0;
            try
            {
                if (Plugin.run)
                    this.Mainloop();
            }
            catch (Exception ex)
            {
                Log.error(ex);
            }
        }

        public void Start()     // This is called after control is given back to Unity
        {
            Plugin.socket = new Thread(new ThreadStart(SocketHandler.InitSocketListener));
            Plugin.socket.Start();
            rng = new System.Random();
            timeLastRun = Time.realtimeSinceStartup;
            timeLastQueued = Time.realtimeSinceStartup;
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

        private void Mainloop()
        {
            SceneMgr.Mode curMode = SceneMgr.Get().GetMode();
            switch (curMode)
            {
                case SceneMgr.Mode.LOGIN:
                    DoLogin();
                    break;
                case SceneMgr.Mode.HUB:
                    DoHub();
                    break;
                case SceneMgr.Mode.GAMEPLAY:
                    DoGameplay();
                    break;
                case SceneMgr.Mode.PRACTICE:
                    DoPractice();
                    break;
                case SceneMgr.Mode.TOURNAMENT:
                    DoTournament();
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
            if (!((UnityEngine.Object)WelcomeQuests.Get() != (UnityEngine.Object)null))
                return;
            Log.say("Clicking through welcome quest");
            WelcomeQuests.Get().m_clickCatcher.TriggerRelease();
        }

        private void DoHub()
        {
            if (playVsHumans)
                SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
            else
                SceneMgr.Get().SetNextMode(SceneMgr.Mode.PRACTICE);
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
                    if (GameFunctions.myPlayer.GetHero().GetRemainingHP() <= 0)
                    {
                        Log.say("Defeat...");
                    }
                    else
                    {
                        Log.say("Victory!");
                    }
                    Plugin.statisticsAdded = true;
                    if (!((UnityEngine.Object)EndGameScreen.Get() != (UnityEngine.Object)null))
                        return;
                    EndGameScreen.Get().ContinueEvents();
                    return;
                }
                catch (Exception ex)
                {
                    Log.error(ex);
                    return;
                }
            }
            if (!GameFunctions.gs.IsLocalPlayerTurn())
                return;
            try
            {
                if (GameState.Get().IsBlockingServer())
                    Thread.Sleep(500);
                GameFunctions.populateZones();
                if (BruteAI.BruteHand())
                {
                    ++BruteAI.loops;
                }
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
                    if (!SceneMgr.Get().IsInGame() && !Network.IsMatching() || (double)num1 > (double)Plugin.maxQueueTime)
                    {
                        Plugin.statisticsAdded = false;
                        Plugin.mulliganDone = false;
                        Plugin.currentDeckId = DeckPickerTrayDisplay.Get().GetSelectedDeckID();
                        Log.say("Queuing for game against human with deck " + (object)Plugin.currentDeckId);
                        GameMgr.Get().SetNextGame(GameMode.UNRANKED_PLAY, MissionID.MULTIPLAYER_1v1);
                        if (Plugin.playRanked)
                        {
                            Network.TrackClient(Network.TrackLevel.LEVEL_INFO, Network.TrackWhat.TRACK_PLAY_TOURNAMENT_WITH_CUSTOM_DECK);
                            Network.RankedMatch(Plugin.currentDeckId);
                        }
                        else
                        {
                            Network.TrackClient(Network.TrackLevel.LEVEL_INFO, Network.TrackWhat.TRACK_PLAY_CASUAL_WITH_CUSTOM_DECK);
                            Network.UnrankedMatch(Plugin.currentDeckId);
                        }
                        Plugin.timeLastQueued = UnityEngine.Time.realtimeSinceStartup;
                        FriendChallengeMgr.Get().OnEnteredMatchmakerQueue();
                        PresenceMgr.Get().SetStatus(new Enum[1]
                                    {
                                      (Enum) PresenceStatus.PLAY_QUEUE
                                    });
                    }
                    else
                    {
                        //Plugin.socketSendStatus("Q");
                        Plugin.timeLastQueued = UnityEngine.Time.realtimeSinceStartup;
                    }
                }
                catch (Exception ex)
                {
                    Log.error("Error in tournament loop... " + ex.StackTrace);
                }
            }
            else
            {
                Plugin.run = false;
                Plugin.finishAfterThisGame = false;
            }
        }

        #endregion

        #region -[ Cheat Handlers ]-

        public static bool help(string func, string[] args, string rawArgs)
        {
            Log.say("'startbot'-'startvsai'-'startvsaiexpert'-'startbotranked'-'stopbot'-'analyze'-'deckid'-'finishthisgame'-'help'");
            return true;
        }

        public static bool StartBot(string func, string[] args, string rawArgs)
        {
            Log.say("Bot started");
            Plugin.playVsHumans = true;
            Plugin.timeLastQueued = UnityEngine.Time.realtimeSinceStartup;
            Plugin.run = true;
            Plugin.playRanked = false;
            return true;
        }

        public static bool StartBotVsAI(string func, string[] args, string rawArgs)
        {
            Log.say("Bot started VS AI");
            Plugin.playVsHumans = false;
            Plugin.playExpert = false;
            Plugin.run = true;
            return true;
        }

        public static bool StartBotVsAIExpert(string func, string[] args, string rawArgs)
        {
            Log.say("Bot started VS AI Expert");
            Plugin.playVsHumans = false;
            Plugin.playExpert = true;
            Plugin.run = true;
            return true;
        }

        public static bool GetDeckId(string func, string[] args, string rawArgs)
        {
            Log.say("Deck id : " + DeckPickerTrayDisplay.Get().GetSelectedDeckID().ToString());
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
            Plugin.run = true;
            Plugin.playVsHumans = true;
            Plugin.playRanked = true;
            return true;
        }

        public static bool StopBot(string func, string[] args, string rawArgs)
        {
            Log.say("Bot stopped");
            Plugin.run = false;
            return true;
        }

        #endregion
    }
}
