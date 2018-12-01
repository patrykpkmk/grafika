using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.Xna.Framework;

namespace GK3D
{
    public class Lights
    {
        //Directional light
        public Vector3 DirectionalLightDirection { get; set; }
        public Vector4 DirectionalLightAmbientColor { get; set; }
        public Vector4 DirectionalLightDiffuseColor { get; set; }
        public Vector4 DirectionalLightSpecularColor { get; set; }

        //Spotlight one
        public Vector3 SpotlightOneLightPosition { get; set; }
        public Vector3 SpotlightOneSpotDirection { get; set; }
        public Vector4 SpotlightOneDiffuseColor { get; set; }
        public Vector4 SpotlightOneSpecularColor { get; set; }

        //Spotlight two
        public Vector3 SpotlightTwoLightPosition { get; set; }
        public Vector3 SpotlightTwoSpotDirection { get; set; }
        public Vector4 SpotlightTwoDiffuseColor { get; set; }
        public Vector4 SpotlightTwoSpecularColor { get; set; }


        public Lights()
        {
            DirectionalLightDirection = new Vector3(0f, 0f, 1f);
            DirectionalLightDirection.Normalize();
            DirectionalLightAmbientColor = Color.White.ToVector4();
            DirectionalLightDiffuseColor = Color.White.ToVector4();
            DirectionalLightSpecularColor = Color.White.ToVector4();

            SpotlightOneLightPosition = new Vector3(0f, 25f, -25f);
            SpotlightOneSpotDirection = new Vector3(0f, -1f, 2f);
            SpotlightOneDiffuseColor = Color.Yellow.ToVector4();
            SpotlightOneSpecularColor = Color.Yellow.ToVector4();

            SpotlightTwoLightPosition = new Vector3(-20f, 0f, 0);
            SpotlightTwoSpotDirection = new Vector3(1f, 0f, 0);
            SpotlightTwoDiffuseColor = Color.DarkRed.ToVector4();
            SpotlightTwoSpecularColor = Color.DarkRed.ToVector4();

            //Timer
            Timer colorChangeTimer = new Timer();
            colorChangeTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            colorChangeTimer.Interval = 1000;
            colorChangeTimer.Enabled = true;
        }

        private int i = 0;
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            i++;
            if (i % 3 == 0)
            {
                SpotlightOneDiffuseColor = Color.DarkOrange.ToVector4();
                SpotlightOneSpecularColor = Color.DarkOrange.ToVector4();

            }
            else if(i % 3 == 1)
            {
                SpotlightOneDiffuseColor = Color.DarkRed.ToVector4();
                SpotlightOneSpecularColor = Color.DarkRed.ToVector4();
            }
            else
            {
                SpotlightOneDiffuseColor = Color.Pink.ToVector4();
                SpotlightOneSpecularColor = Color.Pink.ToVector4();
            }
            
        }
    }
}