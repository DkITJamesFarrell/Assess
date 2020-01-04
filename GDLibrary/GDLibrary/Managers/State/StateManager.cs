﻿/*
Function: 		The "brain" for dynamic events in the game. This class listens for events and responds with changes to the game.
                For example, if the player wins/loses the game then this class will determine what happens as a consequence.
                It may, in this case, show certain UITextObjects, play sounds, reset controllers, generate new collidable objects.
Author: 		NMCG
Version:		1.0
Date Updated:	16/11/17
Bugs:			
Fixes:			None
*/

using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class StateManager : PausableGameComponent
    {
        #region Fields
        private static int currentLevel;

        private static bool playerTurn;
        private static bool enemyTurn;
        private static bool inCombat;
        private static bool dodged;
        private static bool restart;
        private static bool isKeyBinding;

        private static bool inProximityOfATrigger;
        private static bool inProximityOfAnItem;
        private static bool inProximityOfAGate;

        private static bool isRotating;
        private static bool isMoving;

        private static bool isPerspective;
        #endregion

        #region Properties
        public static int CurrentLevel
        {
            get
            {
                return currentLevel;
            }
            set
            {
                currentLevel = value;
            }
        }

        public static bool PlayerTurn
        {
            get
            {
                return playerTurn;
            }
            set
            {
                playerTurn = value;
            }
        }

        public static bool EnemyTurn
        {
            get
            {
                return enemyTurn;
            }
            set
            {
                enemyTurn = value;
            }
        }

        public static bool InCombat
        {
            get
            {
                return inCombat;
            }
            set
            {
                inCombat = value;
            }
        }

        public static bool Dodged
        {
            get
            {
                return dodged;
            }
            set
            {
                dodged = value;
            }
        }

        public static bool Restart
        {
            get
            {
                return restart;
            }
            set
            {
                restart = value;
            }
        }

        public static bool IsKeyBinding
        {
            get
            {
                return isKeyBinding;
            }
            set
            {
                isKeyBinding = value;
            }
        }

        public static bool InProximityOfATrigger
        {
            get
            {
                return inProximityOfATrigger;
            }
            set
            {
                inProximityOfATrigger = value;
            }
        }

        public static bool InProximityOfAnItem
        {
            get
            {
                return inProximityOfAnItem;
            }
            set
            {
                inProximityOfAnItem = value;
            }
        }

        public static bool InProximityOfAGate
        {
            get
            {
                return inProximityOfAGate;
            }
            set
            {
                inProximityOfAGate = value;
            }
        }

        public static bool IsPerspective
        {
            get
            {
                return isPerspective;
            }
            set
            {
                isPerspective = value;
            }
        }

        public static bool ScreamPlayed { get; set; }
        public static bool RatsPlayed { get; set; }
        public static bool ChainsPlayed { get; set; }
        public static bool GameWon { get; set; }

        public static bool IsRotating { get => isRotating; set => isRotating = value; }
        public static bool IsMoving { get => isMoving; set => isMoving = value; }
        public static bool BlockMoved { get; internal set; }
        #endregion

        #region Constructor
        public StateManager(
            Game game, 
            EventDispatcher eventDispatcher, 
            StatusType statusType
        ) : base(game, eventDispatcher, statusType) {
            PlayerTurn = true;
            EnemyTurn = false;
            IsKeyBinding = false;
            InProximityOfATrigger = false;
            InProximityOfAnItem = false;
            InProximityOfAGate = false;
            InCombat = false;
            IsRotating = false;
            IsMoving = false;
            IsPerspective = true;
        }
        #endregion

        #region Event Handling
        protected override void RegisterForEventHandling(EventDispatcher eventDispatcher)
        {
            //eventDispatcher.GameChanged += EventDispatcher_GameChanged;
        }

        protected override void EventDispatcher_MenuChanged(EventData eventData)
        {
            //Did the event come from the main menu and is it a start game event
            if (eventData.EventType == EventActionType.OnStart)
            {
                //Turn on update and draw i.e. hide the menu
                this.StatusType = StatusType.Update | StatusType.Drawn;
            }

            //Did the event come from the main menu and is it a pause game event
            else if (eventData.EventType == EventActionType.OnPause)
            {
                //Turn off update and draw i.e. show the menu since the game is paused
                this.StatusType = StatusType.Off;
            }
        }

        private void EventDispatcher_GameChanged(EventData eventData)
        {
            //Did the event come from the game being won?
            if (eventData.EventType == EventActionType.OnWin)
            {
                CurrentLevel++;
            }
        }
        #endregion
    }
}