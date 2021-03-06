﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace GDLibrary
{
    //To use this class you will need to MANUALLY add the video.dll in the Dependencies folder.
    //Add under GDApp\References i.e. where we added SkinnedModel.dll
    public class Video3DController : Controller
    {
        private enum State : sbyte
        {
            Playing,
            Paused,
            Stopped,
            NeverPlayed
        }

        #region Variables
        private State videoState;
        private VideoPlayer videoPlayer;
        private Video video;
        private Texture2D startTexture;
        private string videoName;
        private float volumeIncrement;
        private float startVolume;

        #endregion

        #region Properties
        public VideoPlayer VideoPlayer
        {
            get
            {
                return videoPlayer;
            }
        }
        public Video Video
        { 
            get
            {
                return video;
            }
            set
            {
                video = value; 
            }
        }
        public string name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        #endregion

        public Video3DController(string id, ControllerType controllerType, Texture2D startTexture, 
            Video video, string videoName, float volumeIncrement, float startVolume, 
            EventDispatcher eventDispatcher)
            : base(id, controllerType)
        {
            //used to idenitify video when we receive events (i.e. am i supposed to play my video?)
            this.videoName = videoName;

            //amount by which to raise/lower video volume
            this.volumeIncrement = volumeIncrement;

            //video WMV file
            this.video = video;
            
            //set initial texture?
            this.startTexture = startTexture;
        //    SetTexture(startTexture);

            //set initial volume 0 - 1
            this.startVolume = startVolume;

            //register for events
            eventDispatcher.VideoChanged += EventDispatcher_Video;

            Set(State.NeverPlayed);
        }

        private void Set(State videoState)
        {
            this.videoState = videoState;
        }

        public virtual void EventDispatcher_Video(EventData eventData)
        {
            //use the ID to send event to a particular controller based on controller ID
            if (eventData.ID.Equals(this.ID))
                ProcessEvent(eventData);
        }

        private void ProcessEvent(EventData eventData)
        {
            if (this.videoPlayer == null)
            {
                this.videoPlayer = new VideoPlayer();
                this.videoPlayer.Volume = 0.1f;
            }

            if (eventData.EventType == EventActionType.OnPlay)
            {
                if (this.videoPlayer.State != MediaState.Playing)
                {
                    this.videoPlayer.Play(video);
                    this.videoState = State.Playing;
                }
            }
            else if (eventData.EventType == EventActionType.OnPause)
            {
                if (this.videoPlayer.State == MediaState.Playing)
                {
                    this.videoPlayer.Pause();
                    this.videoState = State.Paused;
                }
            }
            else if (eventData.EventType == EventActionType.OnStop)
            {
                if ((this.videoPlayer.State == MediaState.Playing) || (this.videoPlayer.State == MediaState.Paused))
                {
                    this.videoPlayer.Stop();
                    this.videoState = State.Stopped;
                }
            }
            else if (eventData.EventType == EventActionType.OnVolumeUp)
            {
                float newVolume = this.videoPlayer.Volume + volumeIncrement;
                this.videoPlayer.Volume = (newVolume <= 1) ? newVolume : 1;
            }
            else if (eventData.EventType == EventActionType.OnVolumeDown)
            {
                float newVolume = this.videoPlayer.Volume - volumeIncrement;
                this.videoPlayer.Volume = (newVolume >= 0) ? newVolume : 0;
            }
        }

        private void SetTexture(Texture2D texture, IActor actor)
        {
            (actor as PrimitiveObject).EffectParameters.Texture = texture;
        }
        public override void Update(GameTime gameTime, IActor actor)
        {
            if(videoState == State.Playing)
                SetTexture(videoPlayer.GetTexture(), actor);
            else if (videoState == State.Stopped)
                SetTexture(startTexture, actor);
        }


        public void Dispose()
        {
            this.videoPlayer.Dispose();
        }
    }
}
