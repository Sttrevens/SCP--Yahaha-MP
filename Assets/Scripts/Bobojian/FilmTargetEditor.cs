#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(FilmTarget))]
public class FilmTargetEditor : Editor
{
    private bool showTaskSection = true;
    private bool[] showTaskFoldouts;
    private string newTaskTemplateName = "新任务模板";
    
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
                
                // 获取任务模板名称
                TaskTemplate taskTemplate = filmTarget.possibleTasks[i].task;
                string taskName = taskTemplate ? taskTemplate.taskName : "未设置任务";
                
                showTaskFoldouts[i] = EditorGUILayout.Foldout(showTaskFoldouts[i], $"任务 {i+1}: {taskName}", true);
                
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
                    SerializedProperty taskTemplateProperty = taskProperty.FindPropertyRelative("task");
                    
                    EditorGUILayout.BeginHorizontal();
                    // 任务模板引用
                    EditorGUILayout.PropertyField(taskTemplateProperty, new GUIContent("任务模板"));
                    
                    // 创建新任务模板按钮
                    if (GUILayout.Button("创建新模板", GUILayout.Width(100)))
                    {
                        TaskTemplate newTemplate = CreateNewTaskTemplate(i);
                        if (newTemplate != null)
                        {
                            taskTemplateProperty.objectReferenceValue = newTemplate;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    // 如果任务模板已设置，显示任务信息
                    if (taskTemplate != null)
                    {
                        // 显示模板信息，只读
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.LabelField("任务名称", taskTemplate.taskName);
                        EditorGUILayout.LabelField("难度系数", taskTemplate.baseDifficulty.ToString());
                        EditorGUILayout.LabelField("基础奖励", taskTemplate.baseReward.ToString());
                        
                        if (taskTemplate.requiresSpecificState)
                        {
                            EditorGUILayout.LabelField("特定状态", taskTemplate.specificStateName);
                        }
                        
                        EditorGUI.EndDisabledGroup();
                        
                        EditorGUILayout.Space();
                    }
                    
                    // 生成设置
                    EditorGUILayout.LabelField("生成设置", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(taskProperty.FindPropertyRelative("taskWeight"), new GUIContent("任务权重"));
                    
                    EditorGUILayout.LabelField("进度区间");
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("开始 - 结束");
                    
                    SerializedProperty minProgressProperty = taskProperty.FindPropertyRelative("minProgressToAppear");
                    SerializedProperty maxProgressProperty = taskProperty.FindPropertyRelative("maxProgressToAppear");
                    
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
                newTask.FindPropertyRelative("task").objectReferenceValue = null;
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
            
            // 添加独立的"创建新任务模板"区域
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("快速创建新任务模板", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            newTaskTemplateName = EditorGUILayout.TextField("模板名称", newTaskTemplateName);
            if (GUILayout.Button("创建", GUILayout.Width(80)))
            {
                TaskTemplate newTemplate = CreateNewTaskTemplate(-1);
                if (newTemplate != null)
                {
                    // 添加新任务并分配模板
                    taskList.arraySize++;
                    SerializedProperty newTask = taskList.GetArrayElementAtIndex(taskList.arraySize - 1);
                    newTask.FindPropertyRelative("task").objectReferenceValue = newTemplate;
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
                    
                    // 重置模板名称字段
                    newTaskTemplateName = "新任务模板";
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
            
            EditorGUI.indentLevel--;
        }
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private TaskTemplate CreateNewTaskTemplate(int taskIndex)
    {
        // 确定新模板的名称
        string templateName = taskIndex >= 0 ? $"Task_{((FilmTarget)target).name}_{taskIndex}" : newTaskTemplateName;
        
        // 创建保存对话框
        string path = EditorUtility.SaveFilePanelInProject(
            "创建新任务模板",
            templateName,
            "asset",
            "请选择保存任务模板的位置"
        );
        
        if (string.IsNullOrEmpty(path))
            return null;
        
        // 创建新的TaskTemplate
        TaskTemplate template = CreateInstance<TaskTemplate>();
        
        // 设置基本属性
        template.taskID = System.Guid.NewGuid().ToString().Substring(0, 8);
        template.taskName = Path.GetFileNameWithoutExtension(path);
        template.taskDescriptions = new string[] { "拍摄目标" };
        template.baseDifficulty = 1.0f;
        template.baseReward = 100.0f;
        template.descriptionFormats = new string[] { "拍摄{0}的精彩表现" };
        template.requiresSpecificState = false;
        template.specificStateName = "";
        template.minLevelProgress = 0f;
        template.maxLevelProgress = 1f;
        template.minViewersRequired = 0;
        
        // 保存资源
        AssetDatabase.CreateAsset(template, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = template;
        
        return template;
    }
}
#endif