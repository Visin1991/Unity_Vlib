using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CubeMapWizardGuid : EditorWindow {

    // Add menu named "My Window" to the Window menu
    [MenuItem("Help/如何截取CubeMap")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CubeMapWizardGuid window = (CubeMapWizardGuid)EditorWindow.GetWindow(typeof(CubeMapWizardGuid));
        window.Show();
    }

    void OnGUI()
    {
        string guid1 = " 1. Disable 水, 或者disable 水的MeshRender.";
        string guid2 = " 2. 创建CubeMap，在Assets任意子目录下点击鼠标右键，Create-> Legacy ->Cubemap";
        string guid3 = " 3. 点击选择被创建的Cubemap，在Inspector 中点击Readable选项 ";
        string guid4 = " 4. 选取合适的位置，一般为水中心位置(注意：位置水平高度应与水面保持一致)，创建一个Cube；并且disable Cube的MeshRender； 作为截取Cubemap 的位置参照物。";
        string guid5 = " 5. 如果使用水本身作为位置参照则不能Disable水所在的整个GameObject，只能Disable 水的MeshRender";
        string guid6 = " 6. 点击Unity Editor 上面的 GameObject -> Render into CubeMap";
        string guid7 = " 7. 把创建好的Cubemap 放在插件的Cubemap选项中，把位置参照物放在Render From Position 选项中， 点击Render";
        string guid8 = " 8. 删除参照物Cube 或者 激活水的MeshRender";

        GUILayout.TextArea(guid1);
        GUILayout.TextArea(guid2);
        GUILayout.TextArea(guid3);
        GUILayout.TextArea(guid4);
        GUILayout.TextArea(guid5);
        GUILayout.TextArea(guid6);
        GUILayout.TextArea(guid7);
        GUILayout.TextArea(guid8);
    }
}
