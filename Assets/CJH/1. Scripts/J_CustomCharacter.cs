using Rukha93.ModularAnimeCharacter.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*
 * public enum BodyPartType
    {
        Torso_Hips = 0,
        Torso_Spine01,
        Torso_Spine02,
        Torso_Shoulders,
        Torso_Head,

        Arms_Lower = 20,
        Arms_Upper,
        Arms_Hand,

        Legs_Upper = 30,
        Legs_Knees,
        Legs_Lower,
        Legs_Feet,
    }
 */

public class J_CustomCharacter : MonoBehaviour
{
    //
    FakeLoader fakeLoader;

    //몸 기본 구조
    public GameObject[] basicBody;
    //머리카락의 부모
    public Transform trBaseHead;

    //상의
    public SkinnedMeshRenderer top;
    //상의 메터리얼
    public Material[] matTops;

    //바지
    public SkinnedMeshRenderer pants;
    //바지 메터리얼
    public Material[] matPants;

    //신발
    public SkinnedMeshRenderer shoes;
    //신발 메터리얼
    public Material[] matShoes;

    void Start()
    {
        fakeLoader = GetComponent<FakeLoader>();

        //load data
        KHHUserCustom.Init();
        KHHUserCustomData data = KHHUserCustom.LoadData();

        KHHCategoryData categoryData;
        for(int i = 0; i < data.datas.Count; i++)
        {
            categoryData = data.datas[i];

            if(categoryData.category.Equals("top"))
            {
                top.sharedMesh = fakeLoader.m_MaleItems.m_Tops[categoryData.itemIndex - 1].meshes[0].sharedMesh;
                top.material = matTops[categoryData.itemIndex - 1];
                SetBasicBody(fakeLoader.m_MaleItems.m_Tops[categoryData.itemIndex - 1].bodyParts);
            } 

            else if(categoryData.category.Equals("bottom"))
            {
                pants.sharedMesh = fakeLoader.m_MaleItems.m_Bottoms[categoryData.itemIndex - 1].meshes[0].sharedMesh;                
                pants.material = matPants[categoryData.itemIndex - 1];
                SetBasicBody(fakeLoader.m_MaleItems.m_Bottoms[categoryData.itemIndex - 1].bodyParts);                
            }
            else if (categoryData.category.Equals("shoes"))
            {
                shoes.sharedMesh = fakeLoader.m_MaleItems.m_Shoes[categoryData.itemIndex - 1].meshes[0].sharedMesh;
                shoes.material = matShoes[categoryData.itemIndex - 1];
                SetBasicBody(fakeLoader.m_MaleItems.m_Shoes[categoryData.itemIndex - 1].bodyParts);
            }
            else if(categoryData.category.Equals("hairstyle"))
            {
                for (int j = 0; j < fakeLoader.m_MaleItems.m_Hairstyles[categoryData.itemIndex - 1].objects.Length; j++)
                {
                    GameObject hair = Instantiate(fakeLoader.m_MaleItems.m_Hairstyles[categoryData.itemIndex - 1].objects[j].prefab, trBaseHead);
                    //SkinnedMeshRenderer[] mr = hair.transform.GetComponentsInChildren<SkinnedMeshRenderer>();

                    ////for(int k = 0; k < mr.Length; k++)
                    //{
                    //    mr[1].materials[0].SetColor("_Color_A_2", Color.white);
                    //    mr[1].materials[0].SetColor("_Color_B_2", Color.white);
                    //    mr[1].materials[0].SetColor("_Color_C_2", Color.white);
                    //}
                }
            }
        }
    }

    /*
     if (materials[i].HasProperty("_MaskRemap"))
            {
                Color colorA = materials[i].GetColor("_Color_A_2");
                Color colorB = materials[i].GetColor("_Color_B_2");
                Color colorC = materials[i].GetColor("_Color_C_2");
                if (materialData != null)
                {
                    //if (materialData.ColorA != Color.black)
                    colorA = materialData.ColorA;
                    //if (materialData.ColorB != Color.black)
                    colorB = materialData.ColorB;
                    //if (materialData.ColorC != Color.black)
                    colorC = materialData.ColorC;
                    OnChangeColor(auxRenderer, auxMatIndex, "_Color_A_2", colorA);
                    OnChangeColor(auxRenderer, auxMatIndex, "_Color_B_2", colorB);
                    OnChangeColor(auxRenderer, auxMatIndex, "_Color_C_2", colorC);
                }
            }
            else if (materials[i].HasProperty("_BaseColor"))
            {
                Color color = materials[i].GetColor("_BaseColor");
                if (materialData != null)
                {
                    if (materialData.ColorA != Color.black)
                        color = materialData.ColorA;
                    OnChangeColor(auxRenderer, auxMatIndex, "_BaseColor", color);
                }
            }
     */

    void SetBasicBody(BodyPartType[] bodyPartTypes)
    {
        int bodyIdx;
        for(int i = 0; i < bodyPartTypes.Length; i++)
        {            
            bodyIdx = (int)(bodyPartTypes[i]);
            if (BodyPartType.Arms_Lower <= bodyPartTypes[i] && bodyPartTypes[i] <= BodyPartType.Arms_Hand)
                bodyIdx -= 15;
            else if (BodyPartType.Legs_Upper <= bodyPartTypes[i] && bodyPartTypes[i] <= BodyPartType.Legs_Feet)
                bodyIdx -= 22;

            basicBody[bodyIdx].SetActive(false);
        }

    }

    void Update()
    {
        
    }
}
