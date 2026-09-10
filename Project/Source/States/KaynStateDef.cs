using UnityEngine;
using Verse;

namespace KaynMod
{
    public class KaynStateDef : Def
    {
        public int order = 0;              // Orden en el ciclo (0, 1, 2, 3...)
        public float hediffSeverity = 0f;  // Severidad que activa esta fase en Kayn_Awakened
        public string iconPath;            // Icono del botón
        public Color iconColor = Color.white;
        public string moteText;            // Mensaje flotante (ej: "¡LIBERADO!")
        public Color moteColor = Color.white;

        private Texture2D iconInt;
        public Texture2D Icon => iconInt ?? (iconInt = ContentFinder<Texture2D>.Get(iconPath, false) ?? BaseContent.BadTex);
    }
}