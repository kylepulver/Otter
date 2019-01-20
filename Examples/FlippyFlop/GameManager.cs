using Otter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlippyFlop {
    class GameManager : Entity {

        public int CurrentGap = 180;
        public int MaxGap = 180;
        public int MinGap = 30;

        public Session Session;

        public float ScoreMultiplier = 1;
        public float Score;

        public float FinalScore;
        public float BestScore;
        public float LastScore;

        public float R, G, B;

        public enum GameState {
            Title,
            Playing,
            End
        }

        public StateMachine<GameState> GameStateMachine = new StateMachine<GameState>();

        public GameManager() : base() {
            Session = Game.Instance.Session(0);
            AddComponent(GameStateMachine);
            Session.LoadData();

            BestScore = float.Parse(Session.GetData("best", "0"));

            EventRouter.Subscribe(Events.FlippyFlipped, (EventRouter.Event e) => {
                ScoreMultiplier += 1;
            });

            EventRouter.Subscribe(Events.FlippyDied, (EventRouter.Event e) => {
                GameStateMachine.ChangeState(GameState.End);
            });

            EventRouter.Subscribe(Events.UpdateBestScore, (EventRouter.Event e) => {
                Session.Data["best"] = BestScore.ToString();
                Session.SaveData();
            });

            R = Game.Instance.Color.R;
            G = Game.Instance.Color.G;
            B = Game.Instance.Color.B;
        }

        public override void Update() {
            base.Update();

            Game.Instance.Color.R = R * Util.ScaleClamp(Score, 0, 100, 1, 0);
            Game.Instance.Color.G = G * Util.ScaleClamp(Score, 0, 100, 1, 0);
            Game.Instance.Color.B = B * Util.ScaleClamp(Score, 0, 100, 1, 0);
        }

        public override void Added() {
            base.Added();

            GameStateMachine.ChangeState(GameState.Title);

        }

        void EnterTitle() {
            Scene.Add(new HudTitleInfo());
            Scene.Add(new HudScore());
            Scene.Add(new HudHighScore());
        }
        void UpdateTitle() {
            if (Session.Controller.A.Pressed) {
                GameStateMachine.ChangeState(GameState.Playing);
            }

        }
        void ExitTitle() {

        }

        void EnterPlaying() {
            CurrentGap = 180;
            Score = 0;
            ScoreMultiplier = 1;

            Scene.Add(new Flippy(Session));
            EventRouter.Publish(Events.GameStarted);
            EventRouter.Publish(Events.UpdateBestScore, BestScore, FinalScore);
        }
        void UpdatePlaying() {
            if (GameStateMachine.Timer % 75 == 0) {
                float wallY = 0, secondWallY = 0;
                wallY = Rand.Float(-430, -210);
                secondWallY = wallY + CurrentGap + 480;

                if (CurrentGap > 60) {
                    CurrentGap -= 5;
                }
                else {
                    CurrentGap -= 2;
                }

                CurrentGap = (int)Util.Clamp(CurrentGap, MinGap, MaxGap);

                Scene.Add(new Wall(700, wallY));
                Scene.Add(new Wall(700, secondWallY));
            }
            if (GameStateMachine.Timer % 60 == 0) {
                Score++;
            }
            
            EventRouter.Publish(Events.ScoreUpdated, Score, ScoreMultiplier);
        }

        void EnterEnd() {
            FinalScore = Score * ScoreMultiplier;
            BestScore = Util.Max(FinalScore, BestScore);
            EventRouter.Publish(Events.ShowFinalScore, FinalScore);
            EventRouter.Publish(Events.UpdateBestScore, BestScore, FinalScore);
            Tween(this, new { Score = 0 }, 30);
        }

        void UpdateEnd() {
            if (GameStateMachine.Timer > 60) {
                if (Session.Controller.A.Pressed) {
                    GameStateMachine.ChangeState(GameState.Playing);
                }
            }
        }

        void ExitEnd() {

        }
    }

    public enum Events {
        ScoreUpdated,
        FlippyDied,
        ShowFinalScore,
        FlippyFlipped,
        GameStarted,
        UpdateBestScore
    }

    public enum Tags {
        Flippy,
        Wall
    }
}
