﻿/*
Function: 		Represent a message broker for events received and routed through the game engine. 
                Allows the receiver to receive event messages with no reference to the publisher - decouples the sender and receiver.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
Comments:       Should consider making this class a Singleton because of the static message Stack - See https://msdn.microsoft.com/en-us/library/ff650316.aspx
*/

using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class EventDispatcher : GameComponent
    {
        #region Static
        private static Queue<EventData> queue;
        private static HashSet<EventData> uniqueSet;
        #endregion

        #region Delegates
        public delegate void CameraEventHandler(EventData eventData);
        public delegate void MenuEventHandler(EventData eventData);
        public delegate void OpacityEventHandler(EventData eventData);
        public delegate void AddActorEventHandler(EventData eventData);
        public delegate void RemoveActorEventHandler(EventData eventData);
        public delegate void PlayerEventHandler(EventData eventData);
        public delegate void GlobalSoundEventHandler(EventData eventData);
        public delegate void Sound3DEventHandler(EventData eventData);
        public delegate void Sound2DEventHandler(EventData eventData);
        public delegate void ObjectPickingEventHandler(EventData eventData);
        public delegate void MouseEventHandler(EventData eventData);
        public delegate void DebugEventHandler(EventData eventData);
        public delegate void VideoEventHandler(EventData eventData);
        public delegate void TextRenderEventHandler(EventData eventData);
        public delegate void TextboxChangedEventHandler(EventData eventData);
        public delegate void LevelEventHandler(EventData eventData);
        #endregion

        #region Events
        public event CameraEventHandler CameraChanged;
        public event MenuEventHandler MenuChanged;
        public event OpacityEventHandler OpacityChanged;
        public event AddActorEventHandler AddActorChanged;
        public event RemoveActorEventHandler RemoveActorChanged;
        public event PlayerEventHandler PlayerChanged;
        public event GlobalSoundEventHandler GlobalSoundChanged;
        public event Sound3DEventHandler Sound3DChanged;
        public event Sound2DEventHandler Sound2DChanged;
        public event ObjectPickingEventHandler ObjectPickChanged;
        public event MouseEventHandler MouseChanged;
        public event DebugEventHandler DebugChanged;
        public event VideoEventHandler VideoChanged;
        public event TextRenderEventHandler TextRenderChanged;
        public event TextboxChangedEventHandler TextboxChanged;
        public event LevelEventHandler LevelChanged;
        #endregion

        #region Constructors
        public EventDispatcher(
            Game game, 
            int initialSize
        ) : base(game) {
            queue = new Queue<EventData>(initialSize);
            uniqueSet = new HashSet<EventData>(new EventDataEqualityComparer());
        }
        #endregion

        #region Class-Specific Methods
        public static void Publish(EventData eventData)
        {
            //this prevents the same event being added multiple times within a single update e.g. 10x bell ring sounds
            if (!uniqueSet.Contains(eventData))
            {
                queue.Enqueue(eventData);
                uniqueSet.Add(eventData);
            }
        }

        private void Process(EventData eventData)
        {
            //Switch - See https://msdn.microsoft.com/en-us/library/06tc147t.aspx
            //one case for each category type
            switch (eventData.EventCategoryType)
            {
                case EventCategoryType.Camera:
                    OnCamera(eventData);
                    break;

                case EventCategoryType.Menu:
                    OnMenu(eventData);
                    break;

                case EventCategoryType.Opacity:
                    OnOpacity(eventData);
                    break;

                case EventCategoryType.SystemAdd:
                    OnAddActor(eventData);
                    break;

                case EventCategoryType.SystemRemove:
                    OnRemoveActor(eventData);
                    break;

                case EventCategoryType.Player:
                    OnPlayer(eventData);
                    break;

                case EventCategoryType.Debug:
                    OnDebug(eventData);
                    break;

                case EventCategoryType.Sound3D:
                    OnSound3D(eventData);
                    break;

                case EventCategoryType.Sound2D:
                    OnSound2D(eventData);
                    break;

                case EventCategoryType.GlobalSound:
                    OnGlobalSound(eventData);
                    break;

                case EventCategoryType.ObjectPicking:
                    OnObjectPicking(eventData);
                    break;

                case EventCategoryType.Mouse:
                    OnMouse(eventData);
                    break;

                case EventCategoryType.Video:
                    OnVideo(eventData);
                    break;

                case EventCategoryType.TextRender:
                    OnTextRender(eventData);
                    break;

                case EventCategoryType.Textbox:
                    OnTextboxChanged(eventData);
                    break;

                case EventCategoryType.Level:
                    OnLevel(eventData);
                    break;
            }
        }

        //called when a video event needs to be generated e.g. play, pause, restart
        protected virtual void OnVideo(EventData eventData)
        {
            VideoChanged?.Invoke(eventData);
        }

        //Called when a text renderer event needs to be generated e.g. alarm in sector 2
        protected virtual void OnTextRender(EventData eventData)
        {
            TextRenderChanged?.Invoke(eventData);
        }

        //Called when a menu change is requested
        protected virtual void OnMenu(EventData eventData)
        {
            MenuChanged?.Invoke(eventData);
        }

        //Called when a camera event needs to be generated
        protected virtual void OnCamera(EventData eventData)
        {
            CameraChanged?.Invoke(eventData);
        }

        //Called when a drawn objects opacity changes - which necessitates moving from opaque <-> transparent list in ObjectManager - see ObjectManager::RegisterForEventHandling()
        protected virtual void OnOpacity(EventData eventData)
        {
            OpacityChanged?.Invoke(eventData);
        }

        //Called when a drawn objects needs to be added - see PickingManager::DoFireNewObject()
        protected virtual void OnAddActor(EventData eventData)
        {
            AddActorChanged?.Invoke(eventData);
        }

        //Called when a drawn objects needs to be removed - see UIMouseObject::HandlePickedObject()
        protected virtual void OnRemoveActor(EventData eventData)
        {
            RemoveActorChanged?.Invoke(eventData);
        }

        //Called when a player related event occurs (e.g. win, lose, health increase)
        protected virtual void OnPlayer(EventData eventData)
        {
            PlayerChanged?.Invoke(eventData);
        }

        //Called when a debug related event occurs (e.g. show/hide debug info)
        protected virtual void OnDebug(EventData eventData)
        {
            DebugChanged?.Invoke(eventData);
        }

        //Called when a global sound event is sent to set volume by category or mute all sounds
        protected virtual void OnGlobalSound(EventData eventData)
        {
            GlobalSoundChanged?.Invoke(eventData);
        }

        //Called when a 3D sound event is sent e.g. play "boom"
        protected virtual void OnSound3D(EventData eventData)
        {
            Sound3DChanged?.Invoke(eventData);
        }

        //Called when a 2D sound event is sent e.g. play "menu music"
        protected virtual void OnSound2D(EventData eventData)
        {
            Sound2DChanged?.Invoke(eventData);
        }

        //Called when the PickingManager picks an object
        protected virtual void OnObjectPicking(EventData eventData)
        {
            ObjectPickChanged?.Invoke(eventData);
        }

        //Called when the we want to set mouse position, appearance etc.
        protected virtual void OnMouse(EventData eventData)
        {
            MouseChanged?.Invoke(eventData);
        }

        //Called when the textbox is changed
        protected virtual void OnTextboxChanged(EventData eventData)
        {
            TextboxChanged?.Invoke(eventData);
        }

        //Called when the level is changed
        protected virtual void OnLevel(EventData eventData)
        {
            LevelChanged?.Invoke(eventData);
        }
        #endregion

        #region General Methods
        public override void Update(GameTime gameTime)
        {
            EventData eventData;

            for (int i = 0; i < queue.Count; i++)
            {
                eventData = queue.Dequeue();
                Process(eventData);
                uniqueSet.Remove(eventData);
            }

            base.Update(gameTime);
        }
        #endregion
    }
}