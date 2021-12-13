using UnityEngine;
using UnityEditor;


namespace Rowlan.PhotoSession
{
    [ExecuteInEditMode]
    public class MenuExtension : MonoBehaviour
    {
        [MenuItem("GameObject/Photo Session/Add Prefab", false, 10)]
        static void AddPrefab(MenuCommand menuCommand)
        {
            // create new gameobject
            GameObject go = Instantiate( Resources.Load( Constants.PhotoSessionPrefabPath)) as GameObject;

            // settings
            go.name = "Photo Session";

            // ensure gameobject gets reparented if this was a context click (otherwise does nothing)
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // register the creation in the undo system
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            Selection.activeObject = go;

            // auto setup

            Debug.Log("Photo Session Auto Setup");

            AutoSetup.Execute(go.GetComponent<PhotoSession>());
        }
    }
}