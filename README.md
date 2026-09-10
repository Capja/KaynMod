# KaynMod
# 🌑 Kayn - Character Framework & Perception System

[![RimWorld Version](https://img.shields.io/badge/RimWorld-1.5%20%7C%201.6-brightgreen.svg)]()
[![Mod Architecture](https://img.shields.io/badge/Architecture-Modular%20%2F%20Data--Driven-blue.svg)]()
[![Status](https://img.shields.io/badge/Status-Technical%20Prototype-orange.svg)]()

> **"Un poder absoluto que desafía las leyes naturales, contenido por la prudencia de no ser descubierto."**

Mod para **RimWorld** centrado en el personaje de **Kayn**: una entidad con un poder abrumador desde el inicio de la partida. En lugar de una progresión tradicional de débil a fuerte, la mecánica principal gira en torno a la **contención, el sigilo, la percepción de testigos y la atribución de culpabilidad**.

El mod está diseñado bajo una estricta **arquitectura modular desacoplada**: no depende de librerías externas ni de otros frameworks psiónicos, aunque cuenta con integración nativa con *Vanilla Psycasts Expanded (VPE)* y el DLC *Royalty*.

---

## 📑 Tabla de Contenidos
1. [Premisa y Filosofía de Diseño](#-premisa-y-filosofía-de-diseño)
2. [Motor de Percepción y Testigos](#-motor-de-percepción-y-testigos)
3. [La Esencia de Kayn (Salud y Estados)](#-la-esencia-de-kayn-salud-y-estados)
4. [Catálogo de Habilidades Implementadas](#-catálogo-de-habilidades-implementadas)
5. [Arquitectura del Código](#-arquitectura-del-código)
6. [Guía de Extensión (Cómo añadir contenido)](#-guía-de-extensión)
7. [Hoja de Ruta (Roadmap)](#-hoja-de-ruta)

---

## 🎯 Premisa y Filosofía de Diseño

* **Poder Total desde el Inicio:** Kayn no necesita desbloquear su fuerza; ya está roto. El reto radica en **cuándo y cómo usarlo sin ser descubierto**.
* **Zero Bloat en Salud:** Toda la batería de capacidades, regeneración y fases de poder se concentran en **un único Hediff** (`Esencia de Kayn`) para mantener limpia la interfaz del colonista.
* **Control Directo:** Pensado para jugarse en control directo con mods como *Perspective Shift* y *Orders! for Perspective Shift*.
* **Open/Closed Principle:** La base del mod está cerrada a modificaciones pero abierta a extensiones. Añadir una habilidad o un estado no requiere editar el código central.

---

## 👁️ Motor de Percepción y Testigos

El núcleo técnico del mod no evalúa la cercanía como un simple radio binario, sino que divide la percepción de cualquier habilidad en una matriz de tres preguntas:

```text
       [ Kayn ejecuta un poder ]
                   │
    ┌──────────────┴──────────────┐
    ▼                             ▼
¿Percibe a Kayn?             ¿Percibe el Efecto?
(LoS, Cono 180°,            (LoS, Visión,
 Invisibilidad, Distancia)    Sonido a través de muros)
    │                             │
    └──────────────┬──────────────┘
                   ▼
       ¿CORRELACIONA A KAYN?
    (¿Sabe el testigo quién fue?)
```

### Factores Evaluados por el Motor:
* **Cono de Visión y Punto Ciego (180°):** Utiliza el vector de rotación de los peones (`pawn.Rotation`). Si un colono está de espaldas a Kayn, este se encuentra en su punto ciego y **no lo ve actuar**, permitiendo el acecho físico sin invisibilidad.
* **Invisibilidad Psicológica:** Comprueba el estado de invisibilidad activa (`InvisibilityUtility`).
* **Línea de Visión (LoS):** Muros, puertas y obstáculos bloquean la visión tanto de Kayn como del impacto.
* **Sentidos y Estado Fisiológico:** Comprueba el nivel de Vista, Oído, Consciencia, si el colono duerme o si está incapacitado.
* **Propagación Acústica:** Los efectos ruidosos se propagan a través de paredes y alertan a peones que estén de espaldas.
* **Filtro de Entidades:** Descarta automáticamente fauna salvaje e insectos para centrar el registro en colonos y humanoides inteligentes.

### 💾 Persistencia y Autolimpieza ("Los muertos no hablan")
A través de `GameComponent_KaynTracker`, los eventos con testigos útiles se almacenan directamente en el archivo `.rws` de la partida:
* Si un evento no tiene testigos que hayan visto algo útil, **se descarta en el acto**.
* Si un testigo muere o es eliminado por Kayn, **se purga de la lista**.
* Si todos los testigos de un evento son eliminados, **el evento se destruye por completo de la memoria**, impidiendo el crecimiento infinito del archivo de guardado.

---

## ⚡ La Esencia de Kayn (Salud y Estados)

Todo el poder pasivo y el rendimiento físico se gestionan desde un único estado: **`Esencia de Kayn` (`Kayn_Awakened`)**.

### 1. Regeneración Automática
* **Curación Celular Pasiva:** Cierra cortes, disparos, contusiones y quemaduras a gran velocidad de forma continua (`HediffComp_KaynRegeneration`).
* **Estabilización Sanguínea:** Detiene y revierte hemorragias activas automáticamente.

### 2. Batería Psiónica (Royalty / Vanilla Psycasts Expanded)
* **Psicofoco Permanente:** Fijado al **100% constante** sin necesidad de meditar.
* **Calor Neuronal Nulo:** Cualquier incremento de calor se disipa instantáneamente a **0**.
* **Enlace Psíquico:** Otorga nivel 6 automático.
* **Compatibilidad VPE:** Si *Vanilla Psycasts Expanded* está instalado, otorga **100 puntos psiónicos** automáticos para desbloquear árboles completos.

### 3. Niveles de Liberación Física (Data-Driven)
Un botón conmutador (*Gizmo*) en la barra permite alterar el grado de contención física en tiempo real:

| Estado | Modificador Salud | Modificadores de Estadísticas | Descripción |
| :--- | :--- | :--- | :--- |
| **Contenido** | `+0%` | Estadísticas estándar | Kayn se camufla como un colono humano ordinario. |
| **Despertado** | `+500%` Movimiento<br>`+500%` Manipulación | `+500%` Vel. Trabajo<br>`+500 kg` Capacidad carga<br>`+25%` Esquiva | Salto físico sobrehumano evidente. |
| **Liberado** | `+1000%` Movimiento<br>`+1000%` Manipulación | `+1000%` Vel. Trabajo<br>`+1000 kg` Capacidad carga<br>`+50%` Esquiva | Fuerza absoluta desatada. Cruza mapas en segundos. |

*(Controlable mediante **clic izquierdo** para rotar de fase o **clic derecho** para desplegar menú de selección directa).*

---

## 🔮 Catálogo de Habilidades Implementadas

Todas las habilidades proyectan su radio máximo alrededor de Kayn al pasar el ratón por encima (`GizmoUpdateOnMouseover`):

| Habilidad | DefName | Tipo | Alcance | Descripción |
| :--- | :--- | :--- | :--- | :--- |
| **Paso Sombrío** | `Kayn_Teleport` | Suelo | 30 | Teletransporte instantáneo atravesando muros en 0 fotogramas (sin interpolación visual). |
| **Velo de Sombras** | `Kayn_Toggle_Invisibility` | Pawn | 20 | Conmutador (Toggle) de invisibilidad permanente. Solo aplicable a Kayn, colonos o mascotas aliadas. Se disipa al morir. |
| **Restaurar Miembros** | `Kayn_Regenerate_Limbs` | Pawn | 15 | Reconstruye instantáneamente extremidades u órganos perdidos en Kayn o aliados. |
| **Restauración Sombría**| `Kayn_Heal_Wounds` | Pawn | 20 | Cierra todas las heridas abiertas sin alterar prótesis biónicas, implantes ni estados biológicos. |
| **Fisura Abisal** | `Kayn_Test_Cataclysm` | Área | 35 | Detonación masiva de radio 4.9. Dibuja el círculo de impacto en vivo bajo el cursor. Ruido: 50 casillas. |
| **Chispa Remota** | `Kayn_Test_Flame` | Suelo | 15 | Inicia fuego a distancia. Acción discreta de Kayn con efecto visible. |
| **Presión Mental** | `Kayn_Test_TargetPawn` | Pawn | 25 | Aturde a un objetivo. Kayn visible sin manifestación gráfica del proyectil. |
| **Transferir Esencia** | `Kayn_Awaken_Other` | Pawn | 3 | Despierta a otro humano otorgándole la `Esencia de Kayn`. |
| **Pulso Fantasma** | `Kayn_Test_Invisible` | Área | 20 | Disparo de prueba 100% imperceptible (sin ruido, sin luz, sin gestos). |

---

## 🏗️ Arquitectura del Código

El proyecto está organizado en módulos desacoplados bajo el espacio de nombres `KaynMod`:

```text
KaynMod/
├── About/
│   └── About.xml                    # Metadatos del mod para RimWorld
├── Assemblies/
│   └── KaynMod.dll                  # Ensamblado compilado final (.NET Framework 4.8)
├── Defs/
│   ├── AbilityDefs/
│   │   └── Kayn_TestAbilities.xml   # Definiciones de las habilidades
│   ├── HediffDefs/
│   │   └── Kayn_Hediffs.xml         # Esencia de Kayn y Velo de Sombras
│   └── StateDefs/
│       └── Kayn_States.xml          # Definición XML de los estados de liberación
├── Patches/
│   └── KaynPatch.xml                # Inyección del componente en humanos
└── Source/
    ├── KaynMod.csproj               # Archivo de proyecto SDK moderno
    ├── Abilities/
    │   ├── CompKayn.cs              # Puente ligero ThingComp (~35 líneas)
    │   ├── KaynAbilityDef.cs        # Def personalizado con datos sensoriales y Worker
    │   ├── KaynAbilityWorker.cs     # Clase base modular de habilidades
    │   ├── KaynTargeting.cs         # Gestión de ratón, targeting e invocación del motor
    │   ├── Command_KaynAbility.cs   # Gizmo de habilidad con dibujado de alcance
    │   ├── HediffComp_KaynRegeneration.cs  # Componente pasivo de salud
    │   ├── HediffComp_KaynPsycastBattery.cs # Componente de calor/psicofoco
    │   └── Workers/                 # Lógica individualizada por habilidad
    │       ├── Worker_Teleport.cs
    │       ├── Worker_ToggleInvisibility.cs
    │       ├── Worker_RegenerateLimbs.cs
    │       ├── Worker_HealWounds.cs
    │       ├── Worker_Cataclysm.cs
    │       ├── Worker_Flame.cs
    │       ├── Worker_Stun.cs
    │       └── Worker_Awaken.cs
    ├── States/
    │   ├── KaynStateDef.cs          # Estructura de datos de un estado
    │   └── Gizmo_KaynStateSelector.cs # Selector dinámico que auto-descubre estados
    ├── Perception/
    │   ├── PerceptionEvaluator.cs   # Algoritmo de conos, distancias y correlación
    │   ├── KaynAbilityEvent.cs      # Modelo de datos del evento registrado
    │   └── GameComponent_KaynTracker.cs # Memoria persistente serializable
    └── Debug/
        └── KaynDebugger.cs          # Formateador del reporte de testigos en consola
```

---

## 🛠️ Guía de Extensión

### ¿Cómo añadir una nueva Habilidad?
1. Crea una clase que herede de `KaynAbilityWorker` en `Source/Abilities/Workers/`:
   ```csharp
   namespace KaynMod
   {
       public class Worker_MiHabilidad : KaynAbilityWorker
       {
           public override void Apply(KaynAbilityDef def, Pawn caster, LocalTargetInfo target)
           {
               // Tu lógica aquí
           }
       }
   }
   ```
2. Añade la definición en `Defs/AbilityDefs/`:
   ```xml
   <KaynMod.KaynAbilityDef>
     <defName>Kayn_MiHabilidad</defName>
     <label>Mi Habilidad</label>
     <workerClass>KaynMod.Worker_MiHabilidad</workerClass>
     <range>25</range>
     <isKaynActionVisible>true</isKaynActionVisible>
     <hasVisualEffect>true</hasVisualEffect>
     <acousticRadius>12</acousticRadius>
   </KaynMod.KaynAbilityDef>
   ```
3. Compila con `dotnet build`. **El mod la integrará automáticamente.**

### ¿Cómo añadir un nuevo Estado de Liberación?
1. Añade una fase (`<li>`) en `Defs/HediffDefs/Kayn_Hediffs.xml` con su modificador (`minSeverity`).
2. Añade un bloque en `Defs/StateDefs/Kayn_States.xml`:
   ```xml
   <KaynMod.KaynStateDef>
     <defName>KaynState_Overload</defName>
     <label>Sobrecarga (+2000%)</label>
     <order>3</order>
     <hediffSeverity>3.0</hediffSeverity>
     <iconPath>UI/Commands/DesirePower</iconPath>
     <iconColor>(0.8, 0.2, 1.0)</iconColor>
     <moteText>¡SOBRECARGA!</moteText>
     <moteColor>(0.8, 0.2, 1.0)</moteColor>
   </KaynMod.KaynStateDef>
   ```
*(No requiere recompilar C#; el juego lo carga y añade al menú de inmediato).*

---

## 🗺️ Hoja de Ruta

- [x] **Fase 1: Laboratorio Técnico y Motor de Percepción** (Completada)
  - [x] Sistema propio desacoplado de habilidades.
  - [x] Conos de visión, línea de visión y acústica.
  - [x] Correlación de culpa e invisibilidad psicológica.
  - [x] Memoria persistente serializable con autolimpieza.
  - [x] Arquitectura modular basada en Workers y States.
- [ ] **Fase 2: Consecuencias y Red de Rumores**
  - [ ] Testigos que escapan del mapa llevan información a su facción de origen.
  - [ ] Sistema de sospecha gradual antes de la hostilidad abierta.
  - [ ] Reacciones de los colonos según sus rasgos (miedo, adoración, desconfianza).
- [ ] **Fase 3: Manifestación Visual e Identidad de Kayn**
  - [ ] Texturas personalizadas para los iconos de la interfaz.
  - [ ] Efectos de partículas (*Flecks*) y sonidos propios para cada poder.
  - [ ] Escenario de inicio narrativo preconfigurado.

---

## 📄 Licencia y Créditos
* **Autor:** Proyecto Kayn Mod
* **Compatibilidad:** RimWorld 1.5 y 1.6
* Código diseñado como software libre bajo fines de aprendizaje y modding de RimWorld.