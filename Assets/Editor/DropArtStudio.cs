#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Kamilunavo.PerfectDrop.Editor;

// Native Editor bridge: commands are consumed sequentially after imports finish.
[InitializeOnLoad]
public sealed class DropArtStudio:EditorWindow
{
 const string Command="/tmp/perfect-drop-art-studio.command";
 static string output=>Environment.GetEnvironmentVariable("DROP_ART_OUTPUT")??"/tmp/perfect-drop-art-studio";
 static string status="Perfect Drop — original terrace, tower and interface";
 static DropArtStudio(){EditorApplication.update+=Poll;}
 [MenuItem("Kamilunavo/Perfect Drop Art Studio")]
 public static void Open(){Directory.CreateDirectory(output);GetWindow<DropArtStudio>("Perfect Drop · Art Studio").Show();}
 void OnGUI(){EditorGUILayout.LabelField("PERFECT DROP",EditorStyles.boldLabel);EditorGUILayout.HelpBox(status,MessageType.Info);if(GUILayout.Button("Author original Concept studio & reflections"))Run("author");if(GUILayout.Button("Validate menu boundaries"))Run("boundary");if(GUILayout.Button("Validate all"))Run("validate");if(GUILayout.Button("Build native preview"))Run("build");}
 static void Poll(){if(EditorApplication.isCompiling||EditorApplication.isUpdating||!File.Exists(Command))return;var command=File.ReadAllText(Command).Trim();File.Delete(Command);Run(command);}
 static void Run(string command)
 {
  try{Directory.CreateDirectory(output);switch(command){case "assets":DropConceptStudioSync.Sync();break;case "polish":DropNativeArtAuthoring.PolishGold();break;case "refine":DropNativeArtAuthoring.RefineReflectionRig();break;case "state":foreach(var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())Debug.Log("DROP_EDITOR_ROOT "+root.name);Debug.Log("DROP_EDITOR_SCENE "+UnityEngine.SceneManagement.SceneManager.GetActiveScene().path);break;case "reflection":DropNativeArtAuthoring.Validate();break;case "author":DropNativeArtAuthoring.Author();break;case "boundary":DropMenuBoundaryValidation.Validate();break;case "validate":ArcadeValidation.Validate();break;case "build":BuildAutomation.BuildMacPreview();break;case "quit":EditorApplication.Exit(0);return;default:throw new InvalidOperationException("Unknown Drop Art Studio command: "+command);}status=command+" completed";File.WriteAllText(Path.Combine(output,"last-success.txt"),status);}
  catch(Exception e){status=command+": "+e.GetBaseException().Message;File.WriteAllText(Path.Combine(output,"command-failure.txt"),e.ToString());Debug.LogException(e);}
 }
}
#endif
