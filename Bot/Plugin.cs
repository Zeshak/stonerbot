using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace StonerBot
{
    public class Plugin : MonoBehaviour
    {
        private static double minTimeBetweenRuns = 1.0;
        private static string laststatus = "";
        public static float maxQueueTime = 120f;
        public static bool playVsHumans = true;
        public static bool playRanked = false;
        public static bool playExpert = false;
        private float timeLastRun;
        private System.Random rng;
        private static bool run;
        private static Thread socket;
        private static bool finishAfterThisGame;
        private static TcpClient client;
        private static TcpListener serverSocket;
        private static NetworkStream netStream;
        public static float timeLastQueued;
        public static bool statisticsAdded;
        public static bool mulliganDone;
        public static long currentDeckId;
        public static int modulo;

        static Plugin()
        {
        }

        public static void init()
        {
            GameObject gameObject = SceneMgr.Get().gameObject;
            Plugin.run = true;
            Plugin.finishAfterThisGame = false;
            Log.newLog();
            Log.module = "BOT";
            GameObject.Destroy(gameObject.GetComponent("Plugin"));
            foreach (var x in gameObject.GetComponents<Plugin>())
            {
                Log.debug("destroying old component: " + x.ToString());
                GameObject.Destroy(x);
            }
            gameObject.AddComponent<Plugin>();
            Log.debug("Bot loaded");
            Plugin.modulo = 1;
        }

        private void OnDestroy()
        {
            Log.debug("Hearthstone closing detected... Stopping threads & sockets");
            Plugin.socket.Abort();
            if (Plugin.client != null)
                Plugin.client.Close();
            Plugin.serverSocket.Stop();
            ((Stream)Plugin.netStream).Close();
        }

        public void Awake()
        {
            CheatMgr.Get().RegisterCheatHandler("startbot", new CheatMgr.ProcessCheatCallback(Plugin.startBot));
            CheatMgr.Get().RegisterCheatHandler("startvsai", new CheatMgr.ProcessCheatCallback(Plugin.startBotVsAi));
            CheatMgr.Get().RegisterCheatHandler("startvsaiexpert", new CheatMgr.ProcessCheatCallback(Plugin.startBotVsAiExpert));
            CheatMgr.Get().RegisterCheatHandler("startbotranked", new CheatMgr.ProcessCheatCallback(Plugin.startBotRanked));
            CheatMgr.Get().RegisterCheatHandler("stopbot", new CheatMgr.ProcessCheatCallback(Plugin.stopBot));
            CheatMgr.Get().RegisterCheatHandler("analyze", new CheatMgr.ProcessCheatCallback(Plugin.analyzeCards));
            CheatMgr.Get().RegisterCheatHandler("deckid", new CheatMgr.ProcessCheatCallback(Plugin.getDeckId));
            CheatMgr.Get().RegisterCheatHandler("finishthisgame", new CheatMgr.ProcessCheatCallback(Plugin.finishThisGame));
        }

        public void Start()
        {
            Plugin.socket = new Thread(new ThreadStart(Plugin.socketThread));
            Plugin.socket.Start();
            this.rng = new System.Random();
            this.timeLastRun = UnityEngine.Time.realtimeSinceStartup;
            Plugin.timeLastQueued = UnityEngine.Time.realtimeSinceStartup;
            Log.say("Bot loaded");
        }

        public void Update()
        {
            if ((double)UnityEngine.Time.realtimeSinceStartup - (double)this.timeLastRun < Plugin.minTimeBetweenRuns)
                return;
            this.timeLastRun = UnityEngine.Time.realtimeSinceStartup;
            Plugin.minTimeBetweenRuns = new System.Random().NextDouble() * 2.0 + 2.0;
            try
            {
                if (Plugin.run) { }
                //this.Mainloop();
                //Log.say("asd");
                else
                    Plugin.socketSendStatus("S");
                    //Log.say("zxc");
            }
            catch (Exception ex)
            {
                Log.error("ERROR IN MAIN LOOP : " + ((object)ex.StackTrace).ToString());
            }
        }

        public static void socketThread()
        {
            Plugin.serverSocket = new TcpListener(8888);
            Plugin.serverSocket.Start();
            while (true)
            {
                try
                {
                    do
                    {
                        do
                            ;
                        while (!Plugin.listenSocket(Plugin.serverSocket));
                        Plugin.netStream = Plugin.client.GetStream();
                    }
                    while (!Plugin.netStream.CanRead);
                    byte[] numArray = new byte[Plugin.client.ReceiveBufferSize];
                    Plugin.netStream.Read(numArray, 0, Plugin.client.ReceiveBufferSize);
                    Plugin.socketComputing(Encoding.UTF8.GetString(numArray));
                    Thread.Sleep(50);
                }
                catch (Exception ex)
                {
                    Log.debug(ex.StackTrace);
                }
            }
        }

        public static void socketComputing(string data)
        {
            if (data.Contains("startbotranked"))
                Plugin.startBotRanked((string)null, (string[])null, (string)null);
            else if (data.Contains("stopbot"))
                Plugin.stopBot((string)null, (string[])null, (string)null);
            else if (data.Contains("finishthisgame"))
                Plugin.finishThisGame((string)null, (string[])null, (string)null);
            else if (data.Contains("startbot"))
            {
                Plugin.startBot((string)null, (string[])null, (string)null);
            }
            else
            {
                if (!data.Contains("startvsai"))
                    return;
                Plugin.startBotVsAi((string)null, (string[])null, (string)null);
            }
        }

        public static bool listenSocket(TcpListener serverSocket)
        {
            if (Plugin.client != null)
            {
                if (Plugin.client.Connected)
                    return true;
                Plugin.client.Close();
                ((Stream)Plugin.netStream).Close();
                Plugin.client = serverSocket.AcceptTcpClient();
                Log.say("Remote connected");
                return true;
            }
            else
            {
                Plugin.client = serverSocket.AcceptTcpClient();
                Log.say("Remote connected");
                return true;
            }
        }

        public static void socketSendStatus(string status)
        {
            if (status.Equals(Plugin.laststatus))
                return;
            Plugin.socketSendCmd(status);
            Plugin.laststatus = status;
        }

        public static void socketSendCmd(string cmd)
        {
            if (Plugin.client == null || Plugin.netStream == null || (!Plugin.client.Connected || !Plugin.netStream.CanWrite))
                return;
            Plugin.netStream.WriteTimeout = 1;
            byte[] bytes = Encoding.UTF8.GetBytes(cmd);
            Plugin.netStream.Write(bytes, 0, bytes.Length);
            Plugin.netStream.Flush();
        }

        public static bool startBot(string func, string[] args, string rawArgs)
        {
            Log.say("Bot started");
            Plugin.playVsHumans = true;
            Plugin.timeLastQueued = UnityEngine.Time.realtimeSinceStartup;
            Plugin.run = true;
            Plugin.playRanked = false;
            return true;
        }

        public static bool finishThisGame(string func, string[] args, string rawArgs)
        {
            Log.say("Bot will stop after this game");
            Plugin.finishAfterThisGame = true;
            return true;
        }

        public static bool startBotRanked(string func, string[] args, string rawArgs)
        {
            Log.say("Bot started in ranked mode");
            Plugin.timeLastQueued = UnityEngine.Time.realtimeSinceStartup;
            Plugin.run = true;
            Plugin.playVsHumans = true;
            Plugin.playRanked = true;
            return true;
        }

        public static bool stopBot(string func, string[] args, string rawArgs)
        {
            Log.say("Bot stopped");
            Plugin.run = false;
            return true;
        }

        public static bool startBotVsAi(string func, string[] args, string rawArgs)
        {
            Log.say("Bot started VS AI");
            Plugin.playVsHumans = false;
            Plugin.playExpert = false;
            Plugin.run = true;
            return true;
        }

        public static bool startBotVsAiExpert(string func, string[] args, string rawArgs)
        {
            Log.say("Bot started VS AI");
            Plugin.playVsHumans = false;
            Plugin.playExpert = true;
            Plugin.run = true;
            return true;
        }

        public static bool getDeckId(string func, string[] args, string rawArgs)
        {
            Log.say("Deck id : " + DeckPickerTrayDisplay.Get().GetSelectedDeckID().ToString());
            return true;
        }

        public static bool analyzeCards(string func, string[] args, string rawArgs)
        {
            Log.say("-------------------------- ANALYSIS -----------------------");
            Log.say("-------------- My hand --------------");
            foreach (Card card in Enumerable.ToList<Card>((IEnumerable<Card>)GameState.Get().GetLocalPlayer().GetHandZone().GetCards()))
            {
                Entity entity = card.GetEntity();
                Log.debug("Card : " + card.ToString());
                Log.debug("Type : " + entity.GetType().ToString());
                if (entity.HasBattlecry())
                    Log.debug("Battlecry : " + card.GetBattlecrySpell().GetType().ToString());
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

        public void Init_Game()
        {
            try
            {
                GameFunctions.gs = GameState.Get();
                GameFunctions.myPlayer = GameFunctions.gs.GetLocalPlayer();
                GameFunctions.ePlayer = GameFunctions.gs.GetFirstOpponentPlayer(GameFunctions.myPlayer);
                InactivePlayerKicker.Get().SetShouldCheckForInactivity(false);
            }
            catch (Exception ex)
            {
                Log.error("Error in initgame function... " + ex.StackTrace);
            }
        }

        public void Mainloop()
        {
            SceneMgr.Mode mode = SceneMgr.Get().GetMode();
            switch (mode)
            {
                case SceneMgr.Mode.LOGIN:
                    if (!((UnityEngine.Object)WelcomeQuests.Get() != (UnityEngine.Object)null))
                        break;
                    Log.say("Clicking through welcome quest");
                    WelcomeQuests.Get().m_clickCatcher.TriggerRelease();
                    break;
                case SceneMgr.Mode.HUB:
                    if (Plugin.playVsHumans)
                    {
                        SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
                        break;
                    }
                    else
                    {
                        SceneMgr.Get().SetNextMode(SceneMgr.Mode.PRACTICE);
                        break;
                    }
                case SceneMgr.Mode.GAMEPLAY:
                    this.Init_Game();
                    Plugin.socketSendStatus("G");
                    if (GameFunctions.gs.IsMulliganPhase())
                    {
                        if (!Plugin.mulliganDone)
                        {
                            try
                            {
                                Plugin.mulliganDone = GameFunctions.doMulligan();
                                break;
                            }
                            catch (Exception ex)
                            {
                                Log.error("Error in mulligan function... " + ex.StackTrace);
                                break;
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
                                if (!Plugin.statisticsAdded)
                                    Plugin.addStatistics(Plugin.currentDeckId, false);
                            }
                            else
                            {
                                Log.say("Victory!");
                                if (!Plugin.statisticsAdded)
                                    Plugin.addStatistics(Plugin.currentDeckId, true);
                            }
                            Plugin.statisticsAdded = true;
                            if (!((UnityEngine.Object)EndGameScreen.Get() != (UnityEngine.Object)null))
                                break;
                            EndGameScreen.Get().ContinueEvents();
                            break;
                        }
                        catch (Exception ex)
                        {
                            Log.error("Error in endgame function... " + ex.StackTrace);
                            break;
                        }
                    }
                    else
                    {
                        if (!GameFunctions.gs.IsLocalPlayerTurn())
                            break;
                        try
                        {
                            if (GameState.Get().IsBlockingServer())
                                Thread.Sleep(500);
                            GameFunctions.populateZones();
                            if (BruteAI.BruteHand())
                            {
                                ++BruteAI.loops;
                                break;
                            }
                            else
                            {
                                if (BruteAI.BruteAttack())
                                    break;
                                GameFunctions.doEndTurn();
                                BruteAI.loops = 0;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.error("Error in playerturn function... " + ex.StackTrace);
                            break;
                        }
                    }
                case SceneMgr.Mode.TOURNAMENT:
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
                                GameMgr.Get().SetNextGame(GameMode.PLAY, MissionID.MULTIPLAYER_1v1);
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
                                break;
                            }
                            else
                            {
                                Plugin.socketSendStatus("Q");
                                Plugin.timeLastQueued = UnityEngine.Time.realtimeSinceStartup;
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.error("Error in tournament loop... " + ex.StackTrace);
                            break;
                        }
                    }
                    else
                    {
                        Plugin.run = false;
                        Plugin.finishAfterThisGame = false;
                        break;
                    }
                case SceneMgr.Mode.PRACTICE:
                    if (SceneMgr.Get().IsInGame())
                        break;
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
                    {
                        GameMgr.Get().StartGame(GameMode.PRACTICE, mission1, selectedDeckId);
                        break;
                    }
                    else
                    {
                        GameMgr.Get().StartGame(GameMode.PRACTICE, mission2, selectedDeckId);
                        break;
                    }
                default:
                    Log.say("Fail in mainloop : " + ((object)mode).ToString());
                    Log.say("Bot stopped");
                    Plugin.run = false;
                    break;
            }
        }

        public static void addStatistics(long deckId, bool win)
        {
            string str = win ? "1" : "0";
            System.IO.File.AppendAllText("statistics", deckId.ToString() + "_" + str + Environment.NewLine);
        }
    }
}
