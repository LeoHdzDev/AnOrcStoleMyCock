#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Menú: Tools > Crear Escena Comic
// Construye la escena "Comic" con las 4 viñetas y el botón SKIP, la guarda en Assets/Scenes
// y la agrega al Build Settings.
public static class CrearEscenaComic
{
    const string RutaEscena = "Assets/Scenes/Comic.unity";
    const string RutaInterfaces = "Assets/Interfaces/";

    [MenuItem("Tools/Crear Escena Comic")]
    public static void Crear()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        // Sprites
        List<Sprite> vinetas = new List<Sprite>();
        for (int i = 1; i <= 4; i++)
        {
            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(RutaInterfaces + "Comic" + i + ".png");
            if (s == null)
            {
                EditorUtility.DisplayDialog("Crear Escena Comic", "No encontré " + RutaInterfaces + "Comic" + i + ".png", "OK");
                return;
            }
            vinetas.Add(s);
        }
        Sprite skipNormal = AssetDatabase.LoadAssetAtPath<Sprite>(RutaInterfaces + "Skip1.png");
        Sprite skipHover = AssetDatabase.LoadAssetAtPath<Sprite>(RutaInterfaces + "Skip2.png");
        Sprite skipPresionado = AssetDatabase.LoadAssetAtPath<Sprite>(RutaInterfaces + "Skip3.png");

        // Escena nueva
        Scene escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Cámara (fondo negro)
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.orthographic = true;
        camObj.AddComponent<AudioListener>();

        // Canvas (misma resolución de referencia que el menú: 960x540)
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960, 540);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        // Imagen de la viñeta (conserva la proporción 16:9 del dibujo)
        GameObject imgObj = new GameObject("Vineta", typeof(RectTransform));
        imgObj.transform.SetParent(canvasObj.transform, false);
        Image imagen = imgObj.AddComponent<Image>();
        imagen.sprite = vinetas[0];
        imagen.preserveAspect = true;
        imagen.raycastTarget = false;
        RectTransform rtImg = imgObj.GetComponent<RectTransform>();
        rtImg.anchorMin = Vector2.zero;
        rtImg.anchorMax = Vector2.one;
        rtImg.offsetMin = Vector2.zero;
        rtImg.offsetMax = Vector2.zero;

        // Botón SKIP (esquina inferior derecha)
        GameObject btnObj = new GameObject("BotonSkip", typeof(RectTransform));
        btnObj.transform.SetParent(canvasObj.transform, false);
        Image imgBoton = btnObj.AddComponent<Image>();
        imgBoton.sprite = skipNormal;
        imgBoton.preserveAspect = true;
        Button boton = btnObj.AddComponent<Button>();
        boton.targetGraphic = imgBoton;
        boton.transition = Selectable.Transition.SpriteSwap;
        SpriteState estados = new SpriteState();
        estados.highlightedSprite = skipHover;
        estados.pressedSprite = skipPresionado;
        estados.selectedSprite = skipNormal;
        boton.spriteState = estados;
        RectTransform rtBtn = btnObj.GetComponent<RectTransform>();
        rtBtn.anchorMin = new Vector2(1f, 0f);
        rtBtn.anchorMax = new Vector2(1f, 0f);
        rtBtn.pivot = new Vector2(1f, 0f);
        rtBtn.sizeDelta = new Vector2(150f, 42f);
        rtBtn.anchoredPosition = new Vector2(-20f, 20f);

        // Controlador del cómic
        GameObject ctrlObj = new GameObject("ControladorComic");
        IntroduccionComic ctrl = ctrlObj.AddComponent<IntroduccionComic>();
        ctrl.imagenDisplay = imagen;
        ctrl.vinetas = vinetas.ToArray();
        ctrl.nombreEscenaJuego = "Stage";
        UnityEventTools.AddPersistentListener(boton.onClick, ctrl.Omitir);

        // EventSystem (con el módulo de entrada que use el proyecto)
        GameObject esObj = new GameObject("EventSystem");
        esObj.AddComponent<EventSystem>();
        Type moduloNuevo = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (moduloNuevo != null) esObj.AddComponent(moduloNuevo);
        else esObj.AddComponent<StandaloneInputModule>();

        // Guardar
        EditorSceneManager.SaveScene(escena, RutaEscena);
        AgregarABuildSettings(RutaEscena);

        EditorUtility.DisplayDialog("Crear Escena Comic",
            "Listo: " + RutaEscena + "\n\nSe agregó al Build Settings.", "OK");
    }

    static void AgregarABuildSettings(string ruta)
    {
        List<EditorBuildSettingsScene> lista = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (EditorBuildSettingsScene s in lista)
            if (s.path == ruta) return;

        lista.Add(new EditorBuildSettingsScene(ruta, true));
        EditorBuildSettings.scenes = lista.ToArray();
    }
}
#endif
