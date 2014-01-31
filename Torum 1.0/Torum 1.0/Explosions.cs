using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Torum_1._0
{
    class Explosions
    {
        static int iMapWidth = 1920;

        //AnimatedSprite asSprite;
        int iX = 0;
        int iY = -100;
        bool bActive = true;
        int iBackgroundOffset = 0;
        Vector2 v2motion = new Vector2(0f, 0f);
        float fSpeed = 1f;

        public int X
        {
            get { return iX; }
            set { iX = value; }
        }
        public int Y
        {
            get { return iY; }
            set { iY = value; }
        }
        public bool IsActive
        {
            get { return bActive; }
        }
        public int Offset
        {
            get { return iBackgroundOffset; }
            set { iBackgroundOffset = value; }
        }
        public float Speed
        {
            get { return fSpeed; }
            set { fSpeed = value; }
        }
        public Vector2 Motion
        {
            get { return v2motion; }
            set { v2motion = value; }
        }

    }
}
