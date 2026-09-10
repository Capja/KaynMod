using System;
using UnityEngine;
using Verse;

namespace KaynMod
{
    public class KaynAbilityDef : Def
    {
        // Clase que ejecuta la lógica de esta habilidad
        public Type workerClass = typeof(KaynAbilityWorker);

        // Icono configurable desde XML
        public string iconPath;

        // Propiedades de lanzamiento
        public float range = 15f;
        public bool targetMustBePawn = false;
        public float explosionRadius = 0f;

        // Propiedades sensoriales
        public bool isKaynActionVisible = true;
        public bool hasVisualEffect = true;
        public float acousticRadius = 0f;
        public float visualPerceptionRadius = 28f;

        // Caché del Worker
        private KaynAbilityWorker workerInt;
        public KaynAbilityWorker Worker
        {
            get
            {
                if (workerInt == null && workerClass != null)
                {
                    workerInt = (KaynAbilityWorker)Activator.CreateInstance(workerClass);
                }
                return workerInt;
            }
        }

        // Caché de la textura del icono
        private Texture2D uiIconInt;
        public Texture2D UIIcon
        {
            get
            {
                if (uiIconInt == null && !string.IsNullOrEmpty(iconPath))
                {
                    uiIconInt = ContentFinder<Texture2D>.Get(iconPath, false);
                }
                return uiIconInt ?? ContentFinder<Texture2D>.Get("UI/Commands/Attack", false) ?? BaseContent.BadTex;
            }
        }
    }
}