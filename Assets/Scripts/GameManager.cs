using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace StockMarketGame
{
    public class GameManager : MonoBehaviour
    {
        const int TICKER_TAPE_ANIMATION_FRAME_LEGTH = 24;

        [SerializeField]
        private RectTransform tikerTape;

        private Context currentContext;
        private Game currentGame;

        public UnityEvent<Game> tikerTapeAnimationFinishedEvent = new UnityEvent<Game>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Bootstrap()
        {
            GameObject game = Instantiate(Resources.Load("Bootstrap")) as GameObject;
            if (game == null)
            {
                throw new ApplicationException("Boostrap could not be loaded");
            }

            DontDestroyOnLoad(game);
        }

        private void Start()
        {
            OnNewScene();
        }

        public void OnSceneChangeRequested(string sceneName)
        {
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(sceneName);
            loadScene.completed += (a) => { OnNewScene(); };
        }

        public void OnNewGameWithMoneyConditionRequested(int moneyCondition)
        {
            currentGame = new Game(moneyCondition);
        }

        public void OnNewGameWithTimeConditionRequested(float timeCondition)
        {
            currentGame = new Game(timeCondition);
        }

        private void OnNewScene()
        {
            StopAllCoroutines();
            Context newContext = FindObjectOfType<Context>();
            if (newContext == null)
            {
                throw new ArgumentException("Destination scene did not have context");
            }
            currentContext = newContext;
            tikerTapeAnimationFinishedEvent.AddListener(newContext.OnTickerTapeAnimationFinished);
            newContext.SubscribeToEvents(this);
            StartCoroutine(TickerTapeAnimationCorutine());
        }

        private IEnumerator TickerTapeAnimationCorutine()
        {
            if (currentContext.tickerTapeTargetTransform != null)
            {
                for (int i = 0; i < TICKER_TAPE_ANIMATION_FRAME_LEGTH; i++)
                {
                    LerpRectTransform(tikerTape, currentContext.tickerTapeTargetTransform, (float)i / TICKER_TAPE_ANIMATION_FRAME_LEGTH);
                    yield return null;
                }
            }
            tikerTapeAnimationFinishedEvent.Invoke(currentGame);
        }

        private static void LerpRectTransform(RectTransform changer, RectTransform target, float percent)
        {
            changer.anchorMax = Vector2.Lerp(changer.anchorMax, target.anchorMax, percent);
            changer.anchorMin = Vector2.Lerp(changer.anchorMin, target.anchorMin, percent);
            changer.offsetMax = Vector2.Lerp(changer.offsetMax, target.offsetMax, percent);
            changer.offsetMin = Vector2.Lerp(changer.offsetMin, target.offsetMin, percent);
            changer.pivot = Vector2.Lerp(changer.pivot, target.pivot, percent);
            changer.rotation = Quaternion.Lerp(changer.rotation, target.rotation, percent);

        }


    }

    public class Game
    {

        public readonly Board board;

        private Optional<int> valueWinCondition;
        private Optional<float> timeWinCondition;

        internal List<IPlayer> players;

        internal Game(int valueWinCondition) : this()
        {
            this.valueWinCondition = new Optional<int>(valueWinCondition);
        }

        internal Game(float timeWinCondition) : this()
        {
            this.timeWinCondition = new Optional<float>(timeWinCondition);
        }

        private Game()
        {
            TextAsset boardJson = (TextAsset)Resources.Load("Board");
            board = JsonUtility.FromJson<Board>(boardJson.text);
        }
    }
}