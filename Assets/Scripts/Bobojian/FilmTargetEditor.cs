#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(FilmTarget))]
public class FilmTargetEditor : Editor
{
    private bool showTaskSection = true;
    private bool[] showTaskFoldouts;
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        // 绘制默认属性
        DrawPropertiesExcluding(serializedObject, "possibleTasks");
        
        // 绘制任务列表
        FilmTarget filmTarget = (FilmTarget)target;
        if (showTaskFoldouts == null || showTaskFoldouts.Length != filmTarget.possibleTasks.Count)
        {
            showTaskFoldouts = new bool[filmTarget.possibleTasks.Count];
            // 默认展开第一个任务
            if (showTaskFoldouts.Length > 0)
                showTaskFoldouts[0] = true;
        }
        
        // 任务列表标题
        EditorGUILayout.Space();
        showTaskSection = EditorGUILayout.Foldout(showTaskSection, "赏金任务设置", true);
        
        if (showTaskSection)
        {
            EditorGUI.indentLevel++;
            
            SerializedProperty taskList = serializedObject.FindProperty("possibleTasks");
            
            // 显示现有任务
            for (int i = 0; i < taskList.arraySize; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                
                // 创建带有删除按钮的标题行
                EditorGUILayout.BeginHorizontal();
                showTaskFoldouts[i] = EditorGUILayout.Foldout(showTaskFoldouts[i], $"任务 {i+1}: {filmTarget.possibleTasks[i].taskName}", true);
                
                // 删除按钮
                if (GUILayout.Button("删除", GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("确认删除", "确定要删除这个任务吗？", "是", "否"))
                    {
                        taskList.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        // 更新折叠状态数组
                        if (showTaskFoldouts.Length > i)
                        {
                            bool[] newFoldouts = new bool[taskList.arraySize];
                            for (int j = 0; j < newFoldouts.Length; j++)
                            {
                                if (j < i)
                                    newFoldouts[j] = showTaskFoldouts[j];
                                else if (j >= i && j+1 < showTaskFoldouts.Length)
                                    newFoldouts[j] = showTaskFoldouts[j+1];
                            }
                            showTaskFoldouts = newFoldouts;
                        }
                        GUIUtility.ExitGUI();
                    }
                }
                
                EditorGUILayout.EndHorizontal();
                
                // 显示任务详情
                if (showTaskFoldouts[i])
                {
                    SerializedProperty taskProperty = taskList.GetArrayElementAtIndex(i);
                    
                    // 获取各个属性
                    SerializedProperty nameProperty = taskProperty.FindPropertyRelative("taskName");
                    SerializedProperty descProperty = taskProperty.FindPropertyRelative("taskDescriptions");
                    SerializedProperty difficultyProperty = taskProperty.FindPropertyRelative("baseDifficulty");
                    SerializedProperty rewardProperty = taskProperty.FindPropertyRelative("baseReward");
                    SerializedProperty requireStateProperty = taskProperty.FindPropertyRelative("requiresSpecificState");
                    SerializedProperty stateNameProperty = taskProperty.FindPropertyRelative("specificStateName");
                    SerializedProperty weightProperty = taskProperty.FindPropertyRelative("taskWeight");
                    SerializedProperty minProgressProperty = taskProperty.FindPropertyRelative("minProgressToAppear");
                    SerializedProperty maxProgressProperty = taskProperty.FindPropertyRelative("maxProgressToAppear");
                    
                    // 绘制属性
                    EditorGUILayout.PropertyField(nameProperty);
                    EditorGUILayout.PropertyField(descProperty);
                    EditorGUILayout.PropertyField(difficultyProperty);
                    EditorGUILayout.PropertyField(rewardProperty);
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("任务条件", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(requireStateProperty);
                    
                    if (requireStateProperty.boolValue)
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(stateNameProperty);
                        EditorGUI.indentLevel--;
                    }
                    
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("生成设置", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(weightProperty);
                    
                    EditorGUILayout.LabelField("进度区间");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("开始 - 结束");
                    
                    float minValue = minProgressProperty.floatValue;
                    float maxValue = maxProgressProperty.floatValue;
                    
                    EditorGUILayout.MinMaxSlider(ref minValue, ref maxValue, 0f, 1f);
                    minProgressProperty.floatValue = minValue;
                    maxProgressProperty.floatValue = maxValue;
                    
                    EditorGUILayout.LabelField($"{minValue:F2} - {maxValue:F2}", GUILayout.Width(80));
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            
            // 添加新任务按钮
            if (GUILayout.Button("添加新任务"))
            {
                taskList.arraySize++;
                SerializedProperty newTask = taskList.GetArrayElementAtIndex(taskList.arraySize - 1);
                
                // 设置默认值
                newTask.FindPropertyRelative("taskName").stringValue = "新任务";
                newTask.FindPropertyRelative("taskDescriptions").arraySize = 1;
                newTask.FindPropertyRelative("taskDescriptions").GetArrayElementAtIndex(0).stringValue = "拍摄目标";
                newTask.FindPropertyRelative("baseDifficulty").floatValue = 1.0f;
                newTask.FindPropertyRelative("baseReward").floatValue = 100.0f;
                newTask.FindPropertyRelative("requiresSpecificState").boolValue = false;
                newTask.FindPropertyRelative("specificStateName").stringValue = "";
                newTask.FindPropertyRelative("taskWeight").floatValue = 1.0f;
                newTask.FindPropertyRelative("minProgressToAppear").floatValue = 0.0f;
                newTask.FindPropertyRelative("maxProgressToAppear").floatValue = 1.0f;
                
                // 更新折叠状态数组
                bool[] newFoldouts = new bool[taskList.arraySize];
                for (int i = 0; i < showTaskFoldouts.Length; i++)
                {
                    newFoldouts[i] = showTaskFoldouts[i];
                }
                // 默认展开新任务
                newFoldouts[newFoldouts.Length - 1] = true;
                showTaskFoldouts = newFoldouts;
            }
            
            EditorGUI.indentLevel--;
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif