﻿/*
Function: 		Applies a scale change to a UI actor based on a sine wave.
Author: 		NMCG
Version:		1.0
Date Updated:	6/10/17
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class UIScaleSineLerpController : UIController
    {
        #region Fields
        private TrigonometricParameters trigonometricParameters;
        #endregion

        #region Properties
        public TrigonometricParameters TrigonometricParameters
        {
            get
            {
                return this.trigonometricParameters;
            }
            set
            {
                this.trigonometricParameters = value;
            }
        }
        #endregion

        public UIScaleSineLerpController(
            string id,
            ControllerType controllerType,
            TrigonometricParameters trigonometricParameters
        ) : base(id, controllerType) {
            this.TrigonometricParameters = trigonometricParameters;
        }

        public override void SetActor(IActor actor)
        {
            UIObject uiObject = actor as UIObject;
            uiObject.Transform.Scale = uiObject.Transform.OriginalTransform2D.Scale;
        }

        protected override void ApplyController(GameTime gameTime, UIObject uiObject, float totalElapsedTime)
        {
            //Sine wave in the range 0 -> max amplitude
            float lerpFactor = MathUtility.SineLerpByElapsedTime(this.TrigonometricParameters, totalElapsedTime);

            //Apply scale change
            uiObject.Transform.Scale = uiObject.Transform.OriginalTransform2D.Scale + Vector2.One * lerpFactor;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is UIScaleSineLerpController other))
                return false;
            else if (this == other)
                return true;

            return this.trigonometricParameters.Equals(other.TrigonometricParameters)
                    && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = hash * 31 + this.ID.GetHashCode();
            hash = hash * 17 + this.trigonometricParameters.GetHashCode();
            return hash;
        }

        public override object Clone()
        {
            return new UIScaleSineLerpController(
                "Clone - " + this.ID,
                this.ControllerType,
                (TrigonometricParameters)this.trigonometricParameters.Clone()
            );
        }
    }
}