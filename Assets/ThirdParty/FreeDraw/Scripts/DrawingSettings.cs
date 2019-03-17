using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FreeDraw
{
    // Helper methods used to set drawing settings
    public class DrawingSettings : MonoBehaviour
    {
        public static bool isCursorOverUI = false;
        public float Transparency = 1f;

        // Changing pen settings is easy as changing the static properties Drawable.Pen_Colour and Drawable.Pen_Width
        public void SetMarkerColour(Color new_color)
        {
            Drawable.Pen_Colour = new_color;
        }
        // new_width is radius in pixels
        public void SetMarkerWidth(int new_width)
        {
            Drawable.Pen_Width = new_width;
        }
        public void SetMarkerWidth(float new_width)
        {
            SetMarkerWidth((int)new_width);
        }

        public void SetTransparency(float amount)
        {
            Transparency = amount;
            Color c = Drawable.Pen_Colour;
            c.a = amount;
            Drawable.Pen_Colour = c;
        }

        public void SetMarkerColor(Color color) {
            color.a = Transparency;
            SetMarkerColour(color);
            Drawable.drawable.SetPenBrush();
        }

        // Call these these to change the pen settings
        public void SetMarkerRed() {
            SetMarkerColor(Color.red);
        }
        public void SetMarkerGreen()
        {
            SetMarkerColor(Color.green);
        }
        public void SetMarkerBlue()
        {
            SetMarkerColor(Color.blue);
        }

        public void SetMarkerBlack() {
            SetMarkerColor(Color.black);
        }
        
        public void SetMarkerGray() {
            SetMarkerColor(Color.gray);
        }
        
        public void SetMarkerWhite() {
            SetMarkerColor(Color.white);
        }
        
        public void SetEraser()
        {
            SetMarkerColour(new Color(255f, 255f, 255f, 0f));
        }

        public void PartialSetEraser()
        {
            SetMarkerColour(new Color(255f, 255f, 255f, 0.5f));
        }
    }
}