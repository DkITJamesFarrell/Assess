﻿using Microsoft.Xna.Framework;
using System;

namespace GDLibrary
{
    public class SpinController : Controller
    {
        #region Fields
        private float rotationMagnitude;
        #endregion

        public SpinController(
            string id,
            ControllerType controllerType,
            float rotationMagnitude
        ) : base(id, controllerType) {
            this.rotationMagnitude = rotationMagnitude;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            Actor3D parent = actor as Actor3D;
            if (actor != null) parent.Transform.RotateBy(new Vector3(0, this.rotationMagnitude, 0));

            base.Update(gameTime, actor);
        }
    }
}