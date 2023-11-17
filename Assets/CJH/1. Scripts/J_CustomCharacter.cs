using Rukha93.ModularAnimeCharacter.Customization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                if(categoryData.materialDatas.Count > 0) SetColor(top.material, categoryData.materialDatas[0]);
                SetBasicBody(fakeLoader.m_MaleItems.m_Tops[categoryData.itemIndex - 1].bodyParts);
            } 

            else if(categoryData.category.Equals("bottom"))
            {
                pants.sharedMesh = fakeLoader.m_MaleItems.m_Bottoms[categoryData.itemIndex - 1].meshes[0].sharedMesh;                
                pants.material = matPants[categoryData.itemIndex - 1];
                if (categoryData.materialDatas.Count > 0) SetColor(pants.material, categoryData.materialDatas[0]);
                SetBasicBody(fakeLoader.m_MaleItems.m_Bottoms[categoryData.itemIndex - 1].bodyParts);                
            }
            else if (categoryData.category.Equals("shoes"))
            {
                shoes.sharedMesh = fakeLoader.m_MaleItems.m_Shoes[categoryData.itemIndex - 1].meshes[0].sharedMesh;
                shoes.material = matShoes[categoryData.itemIndex - 1];
                if (categoryData.materialDatas.Count > 0) SetColor(shoes.material, categoryData.materialDatas[0]);
                SetBasicBody(fakeLoader.m_MaleItems.m_Shoes[categoryData.itemIndex - 1].bodyParts);
            }
            else if(categoryData.category.Equals("hairstyle"))
            {
                for (int j = 0; j < fakeLoader.m_MaleItems.m_Hairstyles[categoryData.itemIndex - 1].objects.Length; j++)
                {
                    GameObject hair = Instantiate(fakeLoader.m_MaleItems.m_Hairstyles[categoryData.itemIndex - 1].objects[j].prefab, trBaseHead);

                    Renderer[] mr = hair.transform.GetComponentsInChildren<Renderer>();
                    for(int k = 0; k < categoryData.materialDatas.Count; k++)
                    {
                        for (int l = 0; l < mr.Length; l++)
                        {
                            for(int m = 0; m < mr[l].materials.Length; m++)
                            {
                                string n = mr[l].materials[m].name.Replace(" (Instance)", "");                                
                                
                                if (n.Equals(categoryData.materialDatas[k].name))
                                {
                                    SetColor(mr[l].materials[m], categoryData.materialDatas[k]);                                   
                                }
                            }
                        }
                    }                   
                }
            }
        }
    }

    void SetColor(Material mat, KHHMaterialData matData)
    {        
        if (mat.HasProperty("_MaskRemap"))
        {
            mat.SetColor("_Color_A_2", matData.ColorA);
            mat.SetColor("_Color_B_2", matData.ColorB);
            mat.SetColor("_Color_C_2", matData.ColorC);
        }
        else if(mat.HasProperty("_BaseColor"))
        {
            mat.SetColor("_BaseColor", matData.ColorA);
        }
    }


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
