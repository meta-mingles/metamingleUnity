using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using B83.Win32;


public class FileDragAndDrop : MonoBehaviour
{
    void OnEnable()
    {
        // must be installed on the main thread to get the right thread id.
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    }
    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        // do something with the dropped file names. aPos will contain the 
        // mouse position within the window where the files has been dropped.
        string str = "Dropped " + aFiles.Count + " files at: " + aFiles.Aggregate((a, b) => a);
        Debug.Log(str);

        string file = "";

        foreach (var f in aFiles)
        {
            var fi = new System.IO.FileInfo(f);
            var ext = fi.Extension.ToLower();
            //이미지 파일인 경우 배경 매니저 오픈
            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
            {
                //경로가 없으면 생성
                if (!System.IO.Directory.Exists(KHHEditData.FileImagePath))
                    System.IO.Directory.CreateDirectory(KHHEditData.FileImagePath);
                if (!System.IO.Directory.Exists(KHHEditData.FileImagePath + "/" + fi.Name))
                    System.IO.File.Copy(f, KHHEditData.FileImagePath + "/" + fi.Name, true);
                file = f;

                KHHEditManager.Instance.BackgroundButtonEvent();
                break;
            }

            //사운드 파일인 경우 사운드 매니저 오픈
            if (ext == ".wav")
            {
                //경로가 없으면 생성
                if (!System.IO.Directory.Exists(KHHEditData.FileSoundPath))
                    System.IO.Directory.CreateDirectory(KHHEditData.FileSoundPath);
                if (!System.IO.Directory.Exists(KHHEditData.FileSoundPath + "/" + fi.Name))
                    System.IO.File.Copy(f, KHHEditData.FileSoundPath + "/" + fi.Name, true);
                file = f;

                KHHEditManager.Instance.SoundButtonEvent();
                break;
            }
        }
    }
}
