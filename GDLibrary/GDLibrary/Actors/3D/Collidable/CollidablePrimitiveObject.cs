﻿using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class CollidablePrimitiveObject : PrimitiveObject
    {
        #region Fields
        private ICollisionPrimitive collisionPrimitive;
        private Actor collidee;

        private ObjectManager objectManager;
        #endregion

        #region Properties
        public ICollisionPrimitive CollisionPrimitive
        {
            get
            {
                return collisionPrimitive;
            }
            set
            {
                collisionPrimitive = value;
            }
        }

        public Actor Collidee
        {
            get
            {
                return collidee;
            }
            set
            {
                collidee = value;
            }
        }

        protected ObjectManager ObjectManager
        {
            get
            {
                return this.objectManager;
            }
            set
            {
                this.objectManager = value;
            }
        }
        #endregion

        #region Constructors
        public CollidablePrimitiveObject(
            string id, 
            ActorType actorType, 
            Transform3D transform,
            EffectParameters effectParameters, 
            StatusType statusType, 
            IVertexData vertexData, 
            ICollisionPrimitive collisionPrimitive, 
            ObjectManager objectManager
        ) : base(id, actorType, transform, effectParameters, statusType, vertexData) {
            this.CollisionPrimitive = collisionPrimitive;
            this.ObjectManager = objectManager;
        }
        #endregion

        #region Methods
        public override void Update(GameTime gameTime)
        {
            //Reset collidee to prevent colliding with the same object in the next update
            collidee = null;

            //Reset any movements applied in the previous update from move keys
            this.Transform.TranslateIncrement = Vector3.Zero;
            this.Transform.RotateIncrement = 0;

            //Update collision primitive with new object position
            if (collisionPrimitive != null)
                collisionPrimitive.Update(gameTime, this.Transform);

            base.Update(gameTime);
        }
        
        protected virtual void HandleInput(GameTime gameTime)
        {
        }
        
        protected virtual void HandleCollisionResponse(Actor collidee)
        {
        }

        public virtual Actor CheckCollisions(GameTime gameTime)
        {
            collidee = null;

            //Test for first collision against opaque and transparent objects
            foreach (IActor actor in objectManager.OpaqueDrawList)
            {
                if (CheckCollisionWithActor(gameTime, actor as Actor3D) != null)
                    collidee = CheckCollisionWithActor(gameTime, actor as Actor3D);
            }

            foreach (IActor actor in objectManager.TransparentDrawList)
            {
                if (CheckCollisionWithActor(gameTime, actor as Actor3D) != null)
                    collidee = CheckCollisionWithActor(gameTime, actor as Actor3D);
            }

            return collidee;
        }

        private Actor CheckCollisionWithActor(GameTime gameTime, Actor3D actor3D)
        {
            //Dont test for collision against yourself - remember the player is in the object manager list too!
            if (this != actor3D)
            {
                if (actor3D is CollidablePrimitiveObject)
                {
                    CollidablePrimitiveObject collidableObject = actor3D as CollidablePrimitiveObject;
                    if (this.CollisionPrimitive.Intersects(collidableObject.CollisionPrimitive))
                        return collidableObject;
                }
                else if (actor3D is SimpleZoneObject)
                {
                    SimpleZoneObject zoneObject = actor3D as SimpleZoneObject;
                    if (this.CollisionPrimitive.Intersects(zoneObject.CollisionPrimitive))
                        return zoneObject;
                }
            }

            return null;
        }

        //apply suggested movement since no collision will occur if the player moves to that position
        protected virtual void ApplyInput(GameTime gameTime)
        {
            //was a move/rotate key pressed, if so then these values will be > 0 in dimension
            if (this.Transform.TranslateIncrement != Vector3.Zero)
                this.Transform.TranslateBy(this.Transform.TranslateIncrement);

            if (this.Transform.RotateIncrement != 0)
                this.Transform.RotateAroundYBy(this.Transform.RotateIncrement);
        }

        public new object Clone()
        {
            if(this.CollisionPrimitive is BoxCollisionPrimitive) {
                return new CollidablePrimitiveObject(
                    "Clone " + this.ID, 
                    this.ActorType, 
                    this.Transform.Clone() as Transform3D,
                    this.EffectParameters.Clone() as EffectParameters, 
                    this.StatusType, 
                    this.VertexData,
                    this.collisionPrimitive.Clone() as BoxCollisionPrimitive, 
                    objectManager
                );
            }

            else {
                return new CollidablePrimitiveObject(
                    "Clone " + this.ID,
                    this.ActorType, 
                    this.Transform.Clone() as Transform3D,
                    this.EffectParameters.Clone() as EffectParameters,
                    this.StatusType,
                    this.VertexData,
                    (SphereCollisionPrimitive) this.collisionPrimitive.Clone(),
                    this.objectManager
                );
            }
        }
        #endregion
    }
}