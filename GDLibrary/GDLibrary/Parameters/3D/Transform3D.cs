﻿/*
Function: 		Encapsulates the transformation and World matrix specific parameters for any 3D entity that can have a position (e.g. a player, a prop, a camera)
Author: 		NMCG
Version:		1.0
Date Updated:	
Bugs:			None
Fixes:			None
*/

using System;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class Transform3D : ICloneable
    {
        #region Statics
        //30/11/17 - fix for Transform3D.Zero - thanks to JL
        public static Transform3D Zero 
        {
            get
            {
                return new Transform3D(Vector3.Zero, Vector3.Zero, Vector3.One, -Vector3.UnitZ, Vector3.UnitY);
            }
        }
        #endregion

        #region Fields
        private Vector3 translation;
        private Vector3 rotation;
        private Vector3 scale;
        private Vector3 look;
        private Vector3 up;

        private Matrix world;
        private readonly Transform3D originalTransform3D;
        private double distanceToCamera;
        #endregion

        #region Properties
        public Matrix Orientation
        {
            get
            {
                return Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X)) 
                    * Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y))
                    * Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z));
            }
        }

        public Matrix World
        {
            set
            {
                this.world = value;
            }
            get
            {
                if (this.IsDirty) {

                    this.world = Matrix.Identity 
                        * Matrix.CreateScale(scale) 
                        * Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X))
                        * Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y))
                        * Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z))
                        * Matrix.CreateTranslation(translation);

                    this.IsDirty = false;
                }

                return this.world;
            }
        }

        public Vector3 Translation
        {
            get
            {
                return this.translation;
            }
            set
            {
                this.IsDirty = true;
                this.translation = new Vector3(
                    (float) Math.Round(value.X, 2),
                    (float) Math.Round(value.Y, 2),
                    (float) Math.Round(value.Z, 2)
                );
            }
        }

        public Vector3 Rotation
        {
            get
            {
                return this.rotation;
            }
            set
            {
                this.IsDirty = true;
                this.rotation = new Vector3(
                    (float) Math.Round(value.X, 2),
                    (float) Math.Round(value.Y, 2),
                    (float) Math.Round(value.Z, 2)
                );
            }
        }

        public Vector3 Scale
        {
            get
            {
                return this.scale;
            }
            set
            {
                this.scale = value;
                this.IsDirty = true;
            }
        }

        public Vector3 Target
        {
            get
            {
                return this.translation + this.look;
            }
        }

        public Vector3 Up
        {
            get
            {
                return this.up;
            }
            set
            {
                this.IsDirty = true;
                this.up = Vector3.Normalize(
                    new Vector3(
                        (float) Math.Round(value.X, 5),
                        (float) Math.Round(value.Y, 5),
                        (float) Math.Round(value.Z, 5)
                    )
                );
            }
        }

        public Vector3 Look
        {
            get
            {
                return this.look;
            }
            set
            {
                this.IsDirty = true;
                this.look = Vector3.Normalize(
                    new Vector3(
                        (float) Math.Round(value.X, 5),
                        (float) Math.Round(value.Y, 5),
                        (float) Math.Round(value.Z, 5)
                    )
                );
            }
        }

        public Vector3 Right
        {
            get
            {
                return Vector3.Normalize(Vector3.Cross(this.look, this.up));
            }
        }

        public Transform3D OriginalTransform3D
        {
            get
            {
                return this.originalTransform3D;
            }
        }

        public double DistanceToCamera
        {
            get
            {
                return this.distanceToCamera;
            }
            set
            {
                this.distanceToCamera = value;
            }
        }

        public Vector3 TranslateIncrement { get; set; }
        public float RotateIncrement { get; set; }
        public bool IsDirty { get; set; }
        #endregion

        #region Constructors
        public Transform3D (
            Vector3 translation,
            Vector3 rotation,
            Vector3 scale,
            Vector3 look,
            Vector3 up
        ) {
            Initialize(translation, rotation, scale, look, up);

            //Store original values in case of reset
            this.originalTransform3D = new Transform3D();
            this.originalTransform3D.Initialize(translation, rotation, scale, look, up);
        }

        //Used by the camera
        public Transform3D (
            Vector3 translation,
            Vector3 look,
            Vector3 up
        ) : this(translation, Vector3.Zero, Vector3.One, look, up) {
        }

        //Used by zone objects
        public Transform3D (
            Vector3 translation, 
            Vector3 scale
        ) : this(translation, Vector3.Zero, scale, Vector3.UnitX, Vector3.UnitY) {
        }

        //Used internally when creating the originalTransform object
        private Transform3D () {
        }
        #endregion

        #region Methods
        protected void Initialize (
            Vector3 translation, 
            Vector3 rotation, 
            Vector3 scale, 
            Vector3 look, 
            Vector3 up
        ) {
            this.Translation = translation;
            this.Rotation = rotation;
            this.Scale = scale;

            this.Look = Vector3.Normalize(look);
            this.Up = Vector3.Normalize(up);
        }

        public void Reset () {
            this.translation = this.originalTransform3D.Translation;
            this.rotation = this.originalTransform3D.Rotation;
            this.scale = this.originalTransform3D.Scale;
            this.look = this.originalTransform3D.Look;
            this.up = this.originalTransform3D.Up;
        }

        public override bool Equals(object obj) {

            if (!(obj is Transform3D other))
                return false;
            else if (this == other)
                return true;

            return Vector3.Equals(this.translation, other.Translation)
                && Vector3.Equals(this.rotation, other.Rotation)
                && Vector3.Equals(this.scale, other.Scale)
                && Vector3.Equals(this.look, other.Look)
                && Vector3.Equals(this.up, other.Up);
        }

        public override int GetHashCode () {
            int hash = 1;
            hash = hash * 31 + this.translation.GetHashCode();
            hash = hash * 17 + this.look.GetHashCode();
            hash = hash * 13 + this.up.GetHashCode();
            return hash;
        }

        public object Clone () {
            //Deep because all variables are either C# types (e.g. primitives, structs, or enums) or  XNA types
            return this.MemberwiseClone();
        }

        public void RotateBy(Vector3 rotateBy) //in degrees
        {
            //Rotate
            this.Rotation += rotateBy;

            //X = Pitch, Y = Yaw, Z = roll
            Matrix rot = Matrix.CreateFromYawPitchRoll(
                MathHelper.ToRadians(this.Rotation.Y),
                MathHelper.ToRadians(this.Rotation.X),
                MathHelper.ToRadians(this.Rotation.Z)
            );

            //Update look vector
            this.Look = Vector3.Normalize(Vector3.Transform(this.originalTransform3D.Look, rot));

            //Update up vector
            this.Up = Vector3.Normalize(Vector3.Transform(this.originalTransform3D.Up, rot));
        }

        public void RotateAroundYBy(float magnitude) {
            this.rotation.Y += magnitude;
            this.look = Vector3.Normalize(Vector3.Transform(this.originalTransform3D.Look, Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y))));
            this.IsDirty = true;
        }

        public void TranslateTo(Vector3 translate) {
            this.translation = translate;
            this.IsDirty = true;
        }

        public void TranslateBy(Vector3 translateBy) {
            this.translation += translateBy;
            this.IsDirty = true;
        }

        public void ScaleTo(Vector3 scale) {
            this.scale = scale;
            this.IsDirty = true;
        }

        public void ScaleBy(Vector3 scaleBy) {
            this.scale *= scaleBy;
            this.IsDirty = true;
        }
        #endregion
    }
}